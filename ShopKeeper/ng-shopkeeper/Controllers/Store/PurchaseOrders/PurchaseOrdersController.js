"use strict";

define(['application-configuration', 'purchaseOrderServices', 'ngDialog'], function (app)
{

    app.register.directive('ngPorders', function ($compile)
    {
        return function ($scope, ngStocks)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/StoreItemStock/GetStoreItemStockObjects";
            tableOptions.itemId = 'StoreItemStockId';
            tableOptions.columnHeaders = ["StoreItemName", "CategoryName", "TypeName", "BrandName", "QuantityInStockStr", "QuantitySoldStr", "ExpiryDate"];
            var ttc = productStockInsertManager($scope, $compile, ngStocks, tableOptions, 'New Inventory', 'prepareStoreItemStockTemplate', '/StoreItemStock/DownloadContentFromFolder?path=~/BulkTemplates/NewInventory.xlsx', 'getStoreItemStock', 'getItemDetails', 'deleteStoreItemStock', 126);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.ngTable = ttc;
        };

    });
  
    app.register.controller('purchaseOrderController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'purchaseOrderServices', 
        function (ngDialog, $scope, $rootScope, $routeParams, purchaseOrderServices)
        {
       
        $scope.initializeController = function ()
            {
                $scope.search = { skuName: '' };
                $scope.initializeModel();
                $scope.getProducts();

            };


        function filterCurrency(amount, symbol) {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        function filterNumber(valueToFilter) {
            var number = $filter('number');
            var value = number(valueToFilter);
            return value;
        }

        $scope.initializeModel = function ()
        {
            $scope.selected = undefined;
            $scope.customer = null;
            $scope.genericSale =
            {
                'Sale': { 'SaleId': 0, 'RegisterId': 0, 'CustomerId': 0, 'EmployeeId': 0, 'AmountDue': '', 'Status': '', 'Date': '' },
                'StoreTransactions': [],
                'SoldItems': [],
                'paymentOption': {'StorePaymentMethodId': '', 'Name': '-- select payment option --' }
            };
            $scope.userInfo = { 'CustomerId': '', 'UserProfileName': '-- select a customer --' };
            $scope.initializeSoldItem();
            $scope.initializeTransaction(); 
            
            $scope.posAmount = ''; 
            $scope.cashAmount = '';

            $scope.lessAmount = '';
            $scope.incompleteAmountDue = false;
            $scope.buttonStatus = 1;
            $scope.buttonText = 'Add Item';
        };

        $scope.initializeSoldItem = function ()
        {
            $scope.soldItem =
            {
                'StoreItemSoldId': '',
                'TempId': '',
                'StoreItemStockId': '',
                'StoreItemStock': { 'StoreItemStockId': '', 'StoreItemName': '-- Select Product --' },
                'SaleId': '',
                'QuantitySold': '',
                'AmountSold': 0,
                'UoMId': '',
                'DateSold': '',
                'ImagePath': '/Content/images/noImage.png'
            };

            $scope.xxd = { 'ImagePath': '/Content/images/noImage.png', 'StoreItemName': '', 'SKU': '', 'Price': '' };

            $scope.category =
            {
                'StoreItemCategoryId': '',
                'ParentCategoryId': '',
                'Description': '',
                'ImagePath': '',
                'LastUpdated': '',
                'Name': '-- Select Category --'
            };

            $scope.sku = '';
            $scope.getFromSKU = false;
            $scope.ammountPaid = '';
            $scope.posAmmount = '';
            $scope.balance = 0;
            $scope.amountPaid = 0;
            $scope.cashAmmount = '';
            $scope.cash = false;
            $scope.pos = false;
            $scope.splitOption = false;
                
        };
            
        $scope.getProducts = function ()
        {
            $scope.items = [];
            $scope.page = 0;
            $scope.itemsPerPage = 50;
            $scope.initializeSoldItem();
            $scope.soldItem.StoreItemStock = { 'StoreItemStockId': '', 'StoreItemName': '-- Select Product --' };
            purchaseOrderServices.getGenericList($scope.getGenericListCompleted);
            purchaseOrderServices.getAllProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
        };

        $rootScope.getProductsBySku = function (val)
        {
            if (val == null)
            {
                return;
            }

            var d = val.originalObject;
            if (d.SKU == null || d.SKU === undefined || d.StoreItemName.length < 1)
            {
                $scope.setError('Item information could not be retrieved. Please try again later.');
                return;
            }

            //scan the stock list to get price configuration details of selected item
            var refItems = $scope.items.filter(function (s)
            {
                return (s.StoreItemStockId === d.StoreItemStockId);
            });

            if (refItems.length < 1)
            {
                $scope.setError('An unknown error was encountered. Please try again later.');
                return;
            }

            var x = refItems[0];

            if (x.ItemPriceObjects.length < 1)
            {
                $scope.setError('An unknown error was encountered. Please try again later.');
                return;
            }

            var minQty = x.ItemPriceObjects[0].MinimumQuantity;
            var minPrice = x.ItemPriceObjects[0].Price;

            //check if this item is still available by evaluating it's available 
            //quantity in relation to the minimum quantity in it's price configuration

            if (minQty > d.QuantityInStock)
            {
                $scope.setError('This item has been exhausted.');
                return;
            }

            //check if this item has been selected before
            var secondResults = $scope.genericSale.SoldItems.filter(function (s)
            {
                return (s.StoreItemStockId === d.StoreItemStockId);
            });

            //if already selected, increment the quantity to be sold, then recalculate the total price
            if (secondResults.length > 0)
            {
                secondResults[0].QuantitySold += minQty;
                $scope.updateAmount(secondResults[0]);
                return;
            }

            //if not selected before, add it to the basket
            var tempId = $scope.genericSale.SoldItems.length + 1;
           
            var soldItem =
            {
                'StoreItemSoldId': '',
                'TempId': tempId,
                'StoreItemStockId': d.StoreItemStockId,
                'StoreItemStock': { 'StoreItemStockId': d.StoreItemStockId, 'StoreItemId': d.StoreItemId, 'StoreItemName': d.StoreItemName, 'Price': minPrice, 'MinimumQuantity': minQty, 'SKU': d.SKU, 'ImagePath': d.ImagePath },
                'SaleId': 0,
                'SKU': d.SKU,
                'GotFromSKU': true,
                'Rate': minPrice,
                'ImagePath': d.ImagePath,
                'QuantitySold': minQty,
                'AmountSold': minPrice,
                'UoMId': d.UoMId,
                'DateSold': ''
            }; 
           
            $scope.clearError();
            $scope.genericSale.SoldItems.push(soldItem);
            $scope.updateAmount(soldItem);
            $rootScope.skuControl = document.getElementById('stockControl_value');
            $rootScope.skuControl.focus();
            $rootScope.searchStr = "";
        };
        
       $scope.scanSku = function (i)
       {
           if (i == null || i.length < 1)
           {
               $scope.setError('SKU not captured.');
               return;
           }
           
           var results = $scope.items.filter(function (s)
           {
               return (s.SKU === i);
           });

           if (results.length < 1)
           {
               $scope.setError('No match found!');
               return;
           }
           
           var secondResults = $scope.genericSale.SoldItems.filter(function (s)
           {
               return (s.SKU === i);
           });

           if (secondResults.length > 0)
           {
               return;
           }

           var d = results[0];

           var tempId = $scope.genericSale.SoldItems.length + 1;

           var soldItem =
           {
               'StoreItemSoldId': '',
               'TempId': tempId,
               'StoreItemStockId': d.StoreItemStockId,
               'StoreItemStock': { 'StoreItemStockId': d.StoreItemStockId, 'StoreItemId': d.StoreItemId, 'StoreItemName': d.StoreItemName, 'Price': d.Price, 'MinimumQuantity': d.MinimumQuantity, 'SKU': d.SKU, 'ImagePath': d.ImagePath },
               'SaleId': 0,
               'SKU': d.SKU,
               'GotFromSKU': true,
               'Rate': d.Price,
               'ImagePath': d.ImagePath,
               'QuantitySold': 1,
               'AmountSold': d.Price,
               'UoMId': d.UoMId,
               'DateSold': ''
           };

           $scope.getFromSKU = true;
           $scope.clearError();
           $scope.genericSale.SoldItems.push(soldItem);
       };

       $scope.setError = function (errorMessage)
       {
           $scope.error = errorMessage;
           $scope.negativefeedback = true;
           $scope.success = '';
           $scope.positivefeedback = false;
       };

       $scope.clearError = function ()
       {
           $scope.error = '';
           $scope.negativefeedback = false;
           $scope.success = '';
           $scope.positivefeedback = false;
       };
        
       $scope.setSuccessFeedback = function (successMessage)
       {
           $scope.error = '';
           $scope.negativefeedback = false;
           $scope.success = successMessage;
           $scope.positivefeedback = true;
       };

       $scope.addTransaction = function (transaction)
       {
           if ($scope.genericSale.StoreTransactions.length > 0)
           {
               var matchfound = false;
               angular.forEach($scope.genericSale.StoreTransactions, function (x, y)
               {
                   if (x.StorePaymentMethod.StorePaymentMethodId === transaction.StorePaymentMethod.StorePaymentMethodId)
                   {
                       x.TransactionAmount = transaction.TransactionAmount;
                       matchfound = true;
                   }
               });
               
               if (!matchfound)
               {
                  transaction.TempId = 1;
                  $scope.genericSale.StoreTransactions.push(transaction);
               }
           }
           else
           {
               transaction.TempId = 1;
               $scope.genericSale.StoreTransactions.push(transaction);
               
           }
           
           $scope.initializeTransaction();
       };
     
       $scope.getGenericListCompleted = function (data)
       {
           $scope.customers = data.Customers;
           $scope.paymentMethods = data.PaymentMethods;
       };
        
       $scope.prepareSaleTemplate = function ()
       {
           $scope.initializeModel();
           $scope.genericSale.Header = 'New Price(s)';
           $scope.clicked = true;
           $scope.buttonStatus = 1;
       };

       $scope.getTempSoldItem = function (itemId)
       {
           if (itemId == undefined || itemId == NaN || parseInt(itemId) < 1)
           {
               alert('Invalid selection');
               return;
           }

           angular.forEach($scope.genericSale.SoldItems, function (item, index)
           {
               if (item.TempId === parseInt(itemId))
               {
                   $scope.initializeSoldItem();
                   $scope.soldItem = item;
                   
                   $scope.xxd.ImagePath = item.ImagePath;
                   $scope.xxd.StoreItemName = item.StoreItemName;
                   $scope.xxd.SKU = item.SKU;
                   $scope.xxd.Price = item.Price;

                   $scope.buttonText = 'Edit Item';
                   $scope.buttonStatus = 2;
               }
           });
           
       };
        
       $scope.confirmDelete = function (id)
       {
           if (id < 1)
           {
               $scope.setError('Invalid selection');
               return;
           }
           $scope.todelId = id;
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Sales/VerifyUser.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });

       };

       $scope.verifyUser = function ()
       {
           if ($scope.email.length < 1)
           {
               $scope.setError('Please provide your email.');
               return;
           }

           if ($scope.password.length < 1)
           {
               $scope.setError('Please provide your password.');
               return;
           }
           var model = { Email: $scope.email, Password: $scope.password };
           purchaseOrderServices.verifyDelete(model, $scope.verifyUserCompleted);
       };

       $scope.verifyUserCompleted = function (data)
       {
           if (data.Code < 1)
           {
               $scope.setError(data.Error);
               return;
           }
          
           $scope.removeSoldItem($scope.todelId);
       };

       $scope.removeSoldItem = function (id)
       {

           ngDialog.close('/ng-shopkeeper/Views/Store/Sales/VerifyUser.html', '');

            if (id < 1)
            {
                $scope.setError('Invalid selection');
                return;
            }
           
            angular.forEach($scope.genericSale.SoldItems, function (x, y)
            {
                if (x.TempId === id)
                {
                    if (!confirm("This Item will be removed from the list. Continue?"))
                    {
                        return;
                    }

                    $scope.genericSale.SoldItems.splice(y, 1);
                    $scope.search.skuName = '';
                }
            });
            
            var totalAmount = 0;
            angular.forEach($scope.genericSale.SoldItems, function (y, i)
            {
                totalAmount += y.AmountSold;
            });
            
            if (totalAmount < 1)
            {
                $scope.genericSale.Sale.AmountDue = '';
            }
            else
            {
               $scope.genericSale.Sale.AmountDue = totalAmount;
            }
           
            $scope.initializeSoldItem();
        };

       $scope.removeSoldItem2 = function (id) {
           if (id < 1) {
               $scope.setError('Invalid selection');
               return;
           }

           angular.forEach($scope.genericSale.SoldItems, function (x, y) {
               if (x.TempId == id) {
                  $scope.genericSale.SoldItems.splice(y, 1);
               }
           });

           var totalAmount = 0;
           angular.forEach($scope.genericSale.SoldItems, function (y, i) {
               totalAmount += y.AmountSold;
           });

           if (totalAmount < 1) {
               $scope.genericSale.Sale.AmountDue = '';
           }
           else {
               $scope.genericSale.Sale.AmountDue = totalAmount;
           }

           $scope.initializeSoldItem();
       };

       $scope.validateSoldItem = function (soldItem)
       {
           if (soldItem.StoreItemStock.StoreItemStockId == undefined || soldItem.StoreItemStock.StoreItemStockId == null || StoreItemStockId.StoreItemStock.StoreItemStockId < 1)
           {
                alert("ERROR: Please select a Product. ");
                return false;
            }

           if (soldItem.QuantitySold == undefined || soldItem.QuantitySold == null || soldItem.QuantitySold < 1)
           {
                alert("ERROR: Please provide Item Quantity. ");
                return false;
           }

           if (soldItem.AmountSold == undefined || soldItem.AmountSold == null || soldItem.AmountSold < 1)
           {
               alert("ERROR: Please Provide Price. ");
               return false;
           }

           if (parseInt(purchaseOrder.MinimumQuantity) < 1)
            {
                alert("ERROR: Please Provide Quantity. ");
                return false;
            }
            return true;
       };
        
       $scope.validateSale = function (genericSale)
       {
           if (genericSale.Sale.AmountDue == undefined || genericSale.Sale.AmountDue == null || genericSale.Sale.AmountDue < 1)
           {
               $scope.setError('An unexpected error was encountered. Please review this Transaction details and try again.');
               return false;
           }
           
           if (genericSale.StoreTransactions == undefined || genericSale.StoreTransactions == null || genericSale.StoreTransactions < 1)
           {
               $scope.setError('ERROR: Please complete the transaction payment first.');
               return false;
           }
           
           if (genericSale.SoldItems == undefined || genericSale.SoldItems == null || genericSale.SoldItems < 1)
           {
               $scope.setError('ERROR: Please add at least one product for this transaction.');
               return false;
           }
           
           return true;
       };

       $scope.checkAmountPaid = function (transaction)
       {

           if (transaction.TransactionAmount !== $scope.genericSale.Sale.AmountDue)
           {
                var testResult = $scope.genericSale.Sale.AmountDue - transaction.TransactionAmount;
                if (testResult > 0)
                {
                    $scope.lessAmount = 'The Amount due is still short of: ' + 'N' + testResult;
                }
               
                if (testResult < 0)
                {
                    $scope.change = testResult;
                }
                $scope.incompleteAmountDue = true;
                return false;
           }
           
           $scope.incompleteAmountDue = false;
           return true;
       };
        
       $rootScope.setCustomer = function (user)
       {
           if (user == null || user.CustomerId < 1)
           {
               return;
           }
           
           purchaseOrderServices.getCustomerInfo(user.CustomerId, $scope.getCustomerInfoCompleted);
       };

       $scope.getCustomerInfoCompleted = function (data)
       {
           if (data == null || data.CustomerObjects[0].CustomerId < 1)
           {
               alert('Customer information could not be retrieved.');
               return;
           }

           $scope.user = data;
       };
       
       $scope.processSale = function ()
       {
          var transaction =
          {
              'StoreTransactionId': '',
              'TempId': '',
              'StoreTransactionTypeId': 1,
              'StorePaymentMethod': '',
              'EffectedByEmployeeId': '',
              'TransactionAmount': '',
              'TransactionDate': '',
              'Remark': $scope.transaction.Remark,
              'StoreOutletId': ''
          };
           
           if ($scope.cash)
           {
               transaction.TransactionAmount = $scope.cashAmount;
               transaction.StorePaymentMethod = $scope.genericSale.paymentOption;
               transaction.StorePaymentMethodId = $scope.genericSale.paymentOption.StorePaymentMethodId;
               
               if ($scope.balance > 0)
               {
                   $scope.setError('Please reconcile the Amount due with Amount paid.');
                   return;
               }

               $scope.addTransaction(transaction);
           }
           
           if ($scope.pos)
           {
               transaction.TransactionAmount = $scope.posAmount;
               transaction.StorePaymentMethod = $scope.genericSale.paymentOption;
               transaction.StorePaymentMethodId = $scope.genericSale.paymentOption.StorePaymentMethodId;
               if (!$scope.checkAmountPaid(transaction))
               {
                   return;
               }
               $scope.addTransaction(transaction);
           }

           if ($scope.splitOption)
           {
               var totalAmount = $scope.posAmount + $scope.cashAmount;

               if (totalAmount !== $scope.genericSale.Sale.AmountDue)
               {
                   $scope.lessAmount = 'N' + $scope.genericSale.Sale.AmountDue - transaction.TransactionAmount;
                   $scope.incompleteAmountDue = true;
                   return;
               }

               transaction.TransactionAmount = $scope.cashAmount;
               transaction.StorePaymentMethod =
               {
                   'StorePaymentMethodId': 1,
                   'Name': 'Cash'
               };

               transaction.StorePaymentMethodId = 1;
               $scope.addTransaction(transaction);
               
               transaction.TransactionAmount = $scope.posAmount;
               transaction.StorePaymentMethod = { 'StorePaymentMethodId': 2, 'Name': 'POS' };
               transaction.StorePaymentMethodId = 2;
               $scope.addTransaction(transaction);
           }
           
            if (!$scope.validateSale($scope.genericSale))
            {
               return;
            }

            $scope.processing = true;

            if ($scope.user.CustomerObjects !== undefined && $scope.user.CustomerObjects != null && $scope.user.CustomerObjects[0].CustomerId > 0)
            {
                $scope.genericSale.Sale.CustomerId = $scope.user.CustomerObjects[0].CustomerId;
            }
            if ($scope.genericSale.Sale.SaleId < 1)
            {
                purchaseOrderServices.addSale($scope.genericSale, $scope.processSaleCompleted);
            }
            else
            {
                purchaseOrderServices.editSale($scope.genericSale, $scope.processSaleCompleted);
            }
       };
        
       $scope.processSaleCompleted = function (data)
       {
           $scope.processing = false;
           if (data.Code < 1)
           {
              $scope.setError(data.Error);
               return;
           }
           else
           {
               $scope.setSuccessFeedback(data.Error);
               
               if (data.Outlet!= null)
               {
                   $scope.outletName = data.Outlet.OutletName;
                   $scope.outletAddress = data.Outlet.Address + ',' + data.Outlet.CityName;
                   $scope.facebook = data.Outlet.FacebookHandle;
                   $scope.twitter = data.Outlet.TwitterHandle;
               }
          
               $scope.receiptId = data.Code;
               $scope.date = data.Date;
               $scope.time = data.Time;
               $scope.cashier = data.UserName;
               $scope.change = $scope.extraAmount;
               $scope.receiptItems = $scope.genericSale.SoldItems;
               $scope.amountDue = $scope.genericSale.Sale.AmountDue;
               $scope.subtotal = $scope.presubtotal;
               $scope.amountReceived = $scope.amountPaid;
               $scope.amountToBalance = $scope.balance;
               $scope.paymentChoices = $scope.genericSale.StoreTransactions;

               
             
               //Recalculate the stock quantities of the sold items
               angular.forEach($scope.genericSale.SoldItems, function (u, i)
               {
                   for (var m = 0; m < $scope.items.length; m++)
                   {
                       var x = $scope.items[m];
                       if (x.StoreItemStockId === u.StoreItemStockId)
                       {
                           //Get the difference in stock quantities
                           var difference = x.QuantityInStock - u.QuantitySold;

                           //if the difference is 0 or less than 0, remove it the stock item from the prefetched stock list
                           if (difference === 0 || difference < 0) 
                           {
                               $scope.items.splice(m, 1);
                           } 
                           else
                           {
                               //else, it's Quantity in stock becomes the difference
                               x.QuantityInStock = difference;
                           }
                       }
                   }
               });
               
               $scope.printReceipt2();
           }
        };

       $scope.checkNearestNumbers = function (test, arr)
       {
            // just sort it generally if not sure about input, not really time consuming
            var num = result = 0;
            var flag = 0;
            for (var i = 0; i < arr.length; i++)
            {
                num = arr[i];
                if (num < test)
                {
                    result = num;
                    flag = 1;
                }
                else if (num == test)
                {
                    result = num;
                    break;
                }
                else if (flag == 1)
                {
                    if ((num - test) < (Math.abs(arr[i - 1] - test)))
                    {
                        result = num;
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
            return result;
       };

       $scope.printReceipt2 = function ()
       {
           var printContents = angular.element('#receipt').html();
           var popupWin = '';
           if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
           {
               popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.window.focus();
               popupWin.document.write('<!DOCTYPE html><html><head>' +
                   '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                   '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + printContents + '</div></div></html>');
               popupWin.onbeforeunload = function (event)
               {
                   popupWin.close();
                   return '.\n';
               };
               popupWin.onabort = function (event)
               {
                   popupWin.document.close();
                   popupWin.close();
               }
           }
           else
           {
               popupWin = window.open('', '_blank', 'width=800,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.document.open();
               popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + printContents + '</div></div></html>');
               popupWin.document.close();
           }
           $scope.initializeModel();
           popupWin.document.close();

           return true;
       };

       $scope.printReceipt = function ()
       {
           var element = angular.element('#receipt');
           var contents = element.html();
           var frame = '';
           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null)
           {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px; display:none" name="frame1"></iframe>');
               element.before(frame1);
               frame1.load(function ()
               {
                   frame = frames["frame1"];
                   frame.document.open();
                   frame.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Thanks for patronizing us</title>');
                   frame.document.write('</head><body>');
                   frame.document.write('</body></html>');
                   frame.document.body.innerHTML = contents;
                   frame.document.close();
                   setTimeout(function () {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame.document.body.innerHTML = '';
                   }, 500);
               });
           }
           else
           {
                frame.document.open();
                frame.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Thanks for patronizing us</title>');
                frame.document.write('</head><body>');
                frame.document.write('</body></html>');
                frame.document.body.innerHTML = contents;
                frame.document.close();
                setTimeout(function () {
                    window.frames["frame1"].focus();
                    window.frames["frame1"].print();
                    frame.document.body.innerHTML = '';
                }, 500);
           }

           $scope.initializeModel();
           return false;
        };

       $scope.priceLookUp = function ()
       {
           $scope.priceList = [];
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Sales/PriceLookUp.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });

           //angular.element('.twitter-typeahead:first').not('last-child:.twitter-typeahead').remove();
       };
        
       $scope.getpItemPrices = function ()
       {
           if ($scope.criteria == undefined || $scope.criteria.trim().length < 1)
           {
               $scope.priceList = [];
               return;
           }

           //purchaseOrderServices.getItemPrices($scope.criteria, $scope.getpItemPricesCompleted);
           
           var filteredList = [];
           var results = $scope.items.filter(function (item)
           {
               return (item.SKU === $scope.criteria || item.StoreItemName.toLowerCase().indexOf($scope.criteria.toLowerCase()) > -1);
           });

           if (results.length < 1) {
               $scope.priceList = [];
               return;
           }
          
           angular.forEach(results, function (i, k)
           {
               if (filteredList.indexOf(i.SKU) < 0)
               {
                   filteredList.push(i);
               }
              
           });
           
           $scope.priceList = filteredList;
       };

       $scope.getpItemPricesCompleted = function (data)
       {
           if (data == null || data.length < 1)
           {
               alert('No match found');
               return;
           }

           $scope.priceList = data;
       };
       
       
    }]);
   
});

