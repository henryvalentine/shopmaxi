"use strict";

define(['application-configuration', 'saleServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngPSku', function ()
    {
        return function ($scope, ngPSku)
        {
            ngPSku.bind("keydown keypress", function (event)
            {
                if (event.which === 13)
                {
                    $scope.criteria = event.target.value;
                    $scope.getpItemPrices();
                }
            });
            $scope.skupControl = ngPSku;
            $scope.skupControl.focus();
        };
    });
    
    app.register.controller('saleController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'saleServices', '$filter', '$locale',
        function (ngDialog, $scope, $rootScope, $routeParams, saleServices, $filter, $locale)
        {
       
        $scope.initializeController = function ()
            {
                $scope.delAuth = { email: '', password: '' };
                $scope.search = { skuName: '' };
                $scope.initializeModel();
                $scope.getProducts();

        };

        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var formats = $locale.NUMBER_FORMATS;
            var value = currency(amount, symbol);
            return value;
        }

        $scope.initializeModel = function ()
        {
            $scope.selected = undefined;
            $scope.customer = null;
            $scope.genericSale =
            {
                'Sale': { 'SaleId': 0, 'RegisterId': 0, 'CustomerId': 0, 'EmployeeId': 0, 'AmountDue': 0, 'Status': '', 'Date': '', Discount: '', VATAmount: 0, DiscountAmount: '', NetAmount: 0, applyVat : false, VAT: 0 },
                'StoreTransactions': [],
                'SoldItems': [],
                'paymentOption': {'StorePaymentMethodId': '', 'Name': '-- select payment option --' }
            };
            $scope.customerInfo = { 'CustomerId': '', 'UserProfileName': '-- select a customer --' };
            $scope.initializeSoldItem();
            $scope.initializeTransaction(); 
            
            $scope.posAmount = ''; 
            $scope.cashAmount = '';

            $scope.discountHolder = 0;
            $scope.vatHolder = 0;

            $scope.lessAmount = '';
            $scope.incompleteAmountDue = false;
            $scope.buttonStatus = 1;
            $scope.buttonText = 'Add Item';
        };

        $scope.initializeSoldItem = function ()
        {
            $scope.delAuth = { email: '', password: '' };
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

            $scope.customer = { CustomerId: '' }

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

        $scope.setPaymentOption = function(opt) 
        {
            if (opt.StorePaymentMethodId === 1 || opt.Name.toLowerCase() === 'cash')
            {
                $scope.cash = true;
                $scope.pos = false;
                $scope.splitOption = false;
            }

            if (opt.StorePaymentMethodId === 2 || opt.Name.toLowerCase() === 'pos')
            {
                $scope.pos = true;
                $scope.cash = false;
                $scope.splitOption = false;
            }

            if (opt.StorePaymentMethodId === 3 || opt.Name.toLowerCase() === 'split')
            {
                $scope.splitOption = true;
            }
            $scope.genericSale.Sale.paymentOption = opt;
            $scope.ammountPaid = '';
            $scope.posAmmount = '';
            $scope.cashAmmount = '';
        };

        $scope.getProducts = function ()
        {
            $scope.items = [];
            $scope.page = 0;
            $scope.itemsPerPage = 50;
            $scope.initializeSoldItem();
            $scope.soldItem.StoreItemStock = { 'StoreItemStockId': '', 'StoreItemName': '-- Select Product --' };
            saleServices.getGenericList($scope.getGenericListCompleted);
            saleServices.getAllProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
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
           $scope.updateAmount(soldItem);
       };

       $scope.getUSBDevices = function ()
       {
           $scope.printReceipt();
       };
    
       $scope.getCustomers = function ()
       {
           saleServices.getCustomers($scope.getCustomersCompleted);
       };

       $scope.getCustomersCompleted = function (response)
       {
           $scope.getCustomers = response;
       };
       
       $scope.setQuantity = function (storeItemStock)
       {
           if (storeItemStock.StoreItemStockId < 1)
           {
                return;
           }
           
           $scope.soldItem.QuantitySold = 1;
           $scope.addSoldItem();
       };

       $scope.getProductsCompleted = function (products)
       {
           if (products.length < 1)
           {
               return;
           }
           
           angular.forEach(products, function (x, y)
           {
               if (x.ItemPriceObjects.length > 1)
               {
                   x.ItemPriceObjects.sort(function (a, b)
                   {
                       return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
                   });
               }
               $scope.items.push(x);

           });

           $scope.page++;
           saleServices.getAllProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
       };

       $scope.sortCollection = function (collection)
       {
           collection.sort(function (a, b)
           {
               return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
           });
        };

       $scope.initializeTransaction = function ()
        {
            $scope.transaction =
            {
                'StoreTransactionId': '',
                'TempId': '',
                'StoreTransactionTypeId': '',
                'StorePaymentMethod': { 'StorePaymentMethodId': '', 'Name': '-- Select Option --' },
                'EffectedByEmployeeId': '',
                'TransactionAmount': 0,
                'TransactionDate': '',
                'Remark': '',
                'StoreOutletId': '',
            };
           
            $scope.lessAmount = '';
            $scope.incompleteAmountDue = false;
        };
        
       $scope.addSoldItem = function ()
       {
           if ($scope.soldItem.StoreItemStock.StoreItemStockId < 1)
           {
               $scope.setError('Please select a product');
               return;
           }

           if ($scope.soldItem.StoreItemStock.PurchaseOrderItemId < 1)
           {
               $scope.setError('Selected product could not be processed. Please try again.');
               return;
           }
           
           if ($scope.soldItem.QuantitySold < 1)
           {
               $scope.setError('Please provide a valid Quantity');
               return;
           }

           var price = 0;
           var rate = 0;
           $scope.presubtotal = 0;
           var totalAmount = 0;
           
          var ssz = $scope.getPrice($scope.soldItem);
          if (ssz.rate < 1) 
          {
              $scope.setError('An error was encountered. Please try again later.');
              return;
          }

          rate = ssz.rate;
          $scope.presubtotal = ssz.presubtotal;
          price = ssz.price;

           var soldItem =
           {
               'StoreItemSoldId': $scope.soldItem.StoreItemSoldId,
               'TempId': $scope.soldItem.TempId,
               'StoreItemStockId': $scope.soldItem.StoreItemStock.StoreItemStockId,
               'StoreItemStock':
               {
                   'StoreItemStockId': $scope.soldItem.StoreItemStock.StoreItemStockId,
                   'StoreItemId': $scope.soldItem.StoreItemStock.StoreItemId,
                   'PurchaseOrderItemId': $scope.soldItem.StoreItemStock.PurchaseOrderItemId,
                   'StoreItemName': $scope.soldItem.StoreItemStock.StoreItemName,
                   'Price': $scope.soldItem.StoreItemStock.Price,
                   'MinimumQuantity': $scope.soldItem.StoreItemStock.MinimumQuantity,
                   'SKU': $scope.soldItem.StoreItemStock.SKU,
                   'ImagePath': $scope.soldItem.StoreItemStock.ImagePath,
                   'ItemPriceId': $scope.soldItem.StoreItemStock.ItemPriceId
               },
               'SaleId': 0,
               'Rate' : rate,
               'QuantitySold': $scope.soldItem.QuantitySold,
               'QuantityDelivered': $scope.soldItem.QuantitySold,
               'QuantityBalance': 0,
               'AmountSold': price,
               'ImagePath': $scope.soldItem.ImagePath,
               'UoMId': $scope.soldItem.StoreItemStock.UoMId,
               'DateSold': ''
           };
           
           if ($scope.genericSale.SoldItems.length > 0)
           {
               var matchFound = false;
               for (var m = 0; m < $scope.genericSale.SoldItems.length; m++)
               {
                   var x = $scope.genericSale.SoldItems[m];
                   if (x.StoreItemStock.StoreItemStockId === soldItem.StoreItemStock.StoreItemStockId)
                   {
                       x.QuantitySold += soldItem.QuantitySold;
                        x.AmountSold += soldItem.AmountSold;
                        matchFound = true;
                   }
               }
               
               if (!matchFound)
               {
                 soldItem.TempId = $scope.genericSale.SoldItems.length + 1;
                 $scope.genericSale.SoldItems.push(soldItem);
              }
           }
           else
           {
               soldItem.TempId = 1;
               $scope.genericSale.SoldItems.push(soldItem);
           }
           
           angular.forEach($scope.genericSale.SoldItems, function (t, k)
           {
               totalAmount += t.AmountSold;
           });
           
           $scope.genericSale.Sale.AmountDue = totalAmount;
           
           $scope.initializeSoldItem();
       };
        
       $scope.getPrice = function(item)
       {
           var priceResult = {'presubtotal' : 0, 'rate' : 0, 'price' : 0};
           var hd = false;
           var items = $scope.items.filter(function (i)
           {
               return i.StoreItemStockId === item.StoreItemStockId;
           });
           
           if (items.length < 1)
            {
                $scope.setError('A fatal error was encountered. The requested operation was aborted');
                return priceResult;
            }
            var x = items[0].ItemPriceObjects;
            angular.forEach(x, function (u, m)
            {
                    if (u.MinimumQuantity === item.QuantitySold)
                    {
                        priceResult.rate = u.Price;
                        priceResult.presubtotal += priceResult.rate;
                        priceResult.price = u.Price * item.QuantitySold;
                        hd = true;
                    }
             });
                    
            if(!hd)
            {
                priceResult.rate = x[x.length - 1].Price;
                priceResult.presubtotal += priceResult.rate;
                priceResult.price = x[x.length - 1].Price * item.QuantitySold;
                
            }

           return priceResult;
       };

       $scope.checkQuantity = function (soldItem)
       {
           var test = parseFloat(soldItem.QuantitySold);

           if (isNaN(test) || test < 1)
           {
               var confirmDelete = 'Do you want to remove' + soldItem.StoreItemStock.StoreItemName + 'from the list?';
               if (!confirm(confirmDelete))
               {
                   soldItem.QuantitySold = 1;
                   $scope.updateAmount(soldItem);
                   return;
               }

               $scope.removeSoldItem2(soldItem.TempId);
               return;
           }
        };

       $scope.updateamountpaid = function (amountPaid)
       {
           if (amountPaid < 1) {
               $scope.amountPaid = 0;
              return;
           }

           $scope.amountPaid = amountPaid;
           $scope.balance = $scope.genericSale.Sale.NetAmount - amountPaid;
       };
        
       $scope.updateAmount = function (soldItem)
       {
            if (soldItem == null || soldItem.StoreItemStockId < 1)
            {
                $scope.setError('Invalid Operation');
                return;
            }

            var test = parseFloat(soldItem.QuantitySold);

            if (isNaN(test) || test < 1)
            {
                $scope.genericSale.Sale.AmountDue = $scope.genericSale.Sale.AmountDue - soldItem.AmountSold;
                $scope.genericSale.Sale.NetAmount = $scope.genericSale.Sale.NetAmount - soldItem.AmountSold;
                soldItem.AmountSold = 0;
                return;
            }

            var d = soldItem;
           
            if (d.QuantitySold > d.QuantityInStock)
            {
                $scope.setError('Only ' + d.QuantityInStock + d.UoMCode + ' of this item is available.');
                return;
            }
           
            var ssz = $scope.getPrice(soldItem);
            if (ssz.presubtotal < 1)
            {
                $scope.setError('An error was encountered. Please try again later.');
                return;
            }
         
           soldItem.AmountSold = ssz.price;
           soldItem.Rate = ssz.rate;
           $scope.presubtotal = ssz.presubtotal;
           var totalAmount = 0;
           angular.forEach($scope.genericSale.SoldItems, function (t, k)
           {
               totalAmount += t.AmountSold;
           });

           $scope.genericSale.Sale.AmountDue = totalAmount;
           $scope.genericSale.Sale.NetAmount = totalAmount;
           $scope.updateAmountForDiscount();

            $scope.posAmmount = '';
            $scope.balance = 0;
            $scope.amountPaid = 0;
            $scope.cashAmmount = '';
       };

       $scope.updateAmountForDiscount = function ()
       {
           if ($scope.genericSale.Sale.NetAmount.length < 1)
           {
               return;
           }
           
           if ($scope.genericSale.Sale.Discount.trim().length > 0)
           {
               var disc = parseFloat($scope.genericSale.Sale.Discount);
               var discountAmount = 0;

               if ($scope.discountHolder < disc)
               {
                   discountAmount = (disc * $scope.genericSale.Sale.NetAmount) / 100;
                   $scope.discountHolder = disc;
                   $scope.genericSale.Sale.DiscountAmount = discountAmount;
                   var amountLessDiscount = $scope.genericSale.Sale.NetAmount - discountAmount;
                   $scope.genericSale.Sale.NetAmount = amountLessDiscount;
               }
               else
               {
                   if ($scope.discountHolder > disc)
                   {
                       var xrem = $scope.discountHolder - disc;
                 
                       discountAmount = (disc * $scope.genericSale.Sale.AmountDue) / 100;
                       $scope.genericSale.Sale.DiscountAmount = discountAmount;
                       var reconciliator = (xrem * $scope.genericSale.Sale.AmountDue) / 100;
                       $scope.genericSale.Sale.NetAmount = (($scope.genericSale.Sale.NetAmount + reconciliator) - discountAmount).toFixed(2);

                       $scope.discountHolder = disc;
                   }
               }
           }
           else
           {
                $scope.genericSale.Sale.NetAmount = $scope.genericSale.Sale.AmountDue;
                $scope.discountHolder = 0;
                $scope.genericSale.Sale.DiscountAmount = 0;
               
           }

           $scope.applyVat();
       };

       $scope.applyVat = function ()
       {
           if (parseFloat($scope.genericSale.Sale.NetAmount) < 1)
           {
               return;
           }

           if ($scope.genericSale.Sale.applyVat === false && $scope.genericSale.Sale.VATAmount > 0)
           {
               $scope.genericSale.Sale.NetAmount -= $scope.genericSale.Sale.VATAmount;
               $scope.genericSale.Sale.VATAmount = 0;
               $scope.genericSale.Sale.VAT = 0;
               return;
           }

           if ($scope.genericSale.Sale.applyVat === true)
           {
               $scope.genericSale.Sale.VAT = $rootScope.store.VAT;
               var vatAmount = ($scope.genericSale.Sale.NetAmount * $rootScope.store.VAT) / 100;
               $scope.genericSale.Sale.VATAmount = vatAmount;
               $scope.genericSale.Sale.NetAmount = ($scope.genericSale.Sale.NetAmount + vatAmount).toFixed(2);
           }
       };

       $scope.isValue = function (val)
       {
           return !(val === null || !angular.isDefined(val) || (angular.isNumber(val) && !isFinite(val)));
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
           $scope.deleteItem = true;

       };

       $scope.verifyUser = function ()
       {
           if ($scope.delAuth.email.length < 1)
           {
               $scope.setError('Please provide your email.');
               return;
           }
          
           if ($scope.delAuth.password.length < 1)
           {
               $scope.setError('Please provide your password.');
               return;
           }
           var model = { Email: $scope.delAuth.email, Password: $scope.delAuth.password };
           saleServices.verifyDelete(model, $scope.verifyUserCompleted);
       };

       $scope.verifyUserCompleted = function (data)
       {
           if (data.Code < 1)
           {
               $scope.setError(data.UserName);
               return;
           }
          
           $scope.removeSoldItem($scope.todelId);
       };

       $scope.removeSoldItem = function (id)
       {
           $scope.email = "";
           $scope.password = "";
           $scope.deleteItem = false;

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

            $scope.updateAmountForDiscount();
            $scope.deleteItem = false;
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

           if (parseInt(sale.MinimumQuantity) < 1)
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

       $scope.checkAmountPaid = function (amountPaid)
       {
           if (amountPaid !== $scope.genericSale.Sale.NetAmount)
           {
               var testResult = $scope.genericSale.Sale.NetAmount - amountPaid;
                if (testResult > 0)
                {
                    $scope.lessAmount = 'The Net Amount is still short of: ' + 'N' + testResult;
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
           $scope.customerDetail = user;
           //saleServices.getCustomerInfo(user.CustomerId, $scope.getCustomerInfoCompleted);
       };

       $scope.getCustomerInfoCompleted = function (data)
       {
           if (data == null || data.CustomerObjects[0].CustomerId < 1)
           {
               alert('Customer information could not be retrieved.');
               return;
           }

           $scope.customerInfo = data.CustomerObjects[0];
       };

       $scope.hideDelete = function ()
       {
           $scope.deleteItem = false;
       };
       
       $scope.processSale = function () {
         
           if (parseInt($scope.genericSale.Sale.paymentOption.StorePaymentMethodId) < 1)
           {
               alert('Please select payment option!');
               return;
           }

           //if ($scope.customerInfo === undefined || $scope.customerInfo == null || $scope.customerInfo.CustomerId < 1)
           //{
           //    alert('Please select Customer!');
           //    return;
           //}

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
               transaction.StorePaymentMethod = $scope.genericSale.Sale.paymentOption;
               transaction.StorePaymentMethodId = $scope.genericSale.Sale.paymentOption.StorePaymentMethodId;
               
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
               transaction.StorePaymentMethod = $scope.genericSale.Sale.paymentOption;
               transaction.StorePaymentMethodId = $scope.genericSale.Sale.paymentOption.StorePaymentMethodId;
               if (!$scope.checkAmountPaid($scope.amountPaid))
               {
                   return;
               }
               $scope.addTransaction(transaction);
           }

           if ($scope.splitOption)
           {
               var totalAmount = $scope.posAmount + $scope.cashAmount;

               if (totalAmount !== $scope.genericSale.Sale.NetAmount)
               {
                   $scope.lessAmount = 'N' + $scope.genericSale.Sale.NetAmount - transaction.TransactionAmount;
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

            if ($scope.customerDetail && $scope.customerDetail.CustomerId > 0)
            {
                $scope.genericSale.Sale.CustomerId = $scope.customerDetail.CustomerId;
            }
            else
            {
                $scope.genericSale.Sale.CustomerId = 'null';
            }

            if ($scope.genericSale.Sale.SaleId < 1)
            {
                saleServices.addSale($scope.genericSale, $scope.processSaleCompleted);
            }
            else
            {
                saleServices.editSale($scope.genericSale, $scope.processSaleCompleted);
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

               $scope.rec =
               {
                   referenceCode: data.ReferenceCode,
                   date: data.Date,
                   customer: $scope.customerInfo,
                   time: data.Time,
                   storeAddress : $rootScope.store.StoreAddress,
                   cashier: $rootScope.user.Name,
                   change: $scope.extraAmount,
                   receiptItems: $scope.genericSale.SoldItems,
                   amountDue: $scope.genericSale.Sale.AmountDue,
                   subtotal: $scope.presubtotal,
                   amountReceived: $scope.amountPaid,
                   amountToBalance: $scope.balance,
                   paymentChoices: $scope.genericSale.StoreTransactions,
                   netAmount: $scope.genericSale.Sale.NetAmount,
                   discountAmount: $scope.genericSale.Sale.DiscountAmount,
                   vatAmount: $scope.genericSale.Sale.VATAmount
               };

               
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
               $scope.printReceipt($scope.rec);
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
       
       $scope.printReceipt = function (rec)
       {
           var contents =
               '<div class="row"><div class="col-md-4"></div><div class="col-md-4">' +
                   '<img src="' + $rootScope.store.StoreLogoPath + '" alt="" style="width: 50px; height: 50px"/>' +
                   '</div><div class="col-md-4"></div></div><div class="row" style="margin-top: 2%"><div class="col-md-12 divlesspadding">' +
                   '<label class="ng-binding">' + $rootScope.store.StoreName + '</label> | <label class="ng-binding">' + $rootScope.store.StoreEmail + '</label>' +
                   '</div></div>' +
                   '<div class="row"><div class="col-md-12 divlesspadding"><div class="row"><div class="col-md-10 divlesspadding"><h5>' + rec.storeAddress + '</h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Date: <b>' + rec.date + rec.time + '</b></h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Receipt No: <b>' + rec.referenceCode + '</b></h5>' +
                   '</div></div></div></div><br/>';

           if ($scope.customerDetail.CustomerId > 0)
               {
               contents += '<div class="row"><div class="col-md-12">Customer : <strong>' + $scope.customerDetail.UserProfileName + '</strong></div>'
                       + '</div>' +
                       //'<div class="row"><div class="col-md-12">Mobile No : <strong>' + rec.MobileNumber + '</strong></div></div><div class="row">'
                       //+ '<div class="col-md-12">Email : <strong>' + rec.ContactEmail + '</strong></div></div><div class="row">'
                       //+ '<div class="col-md-12 divlesspadding">'
                       //+ 'Address : <strong>' + $scope.customer.DeliveryAddressObject.AddressLine1 + ', ' + $scope.customer.DeliveryAddressObject.CityName + ', ' + $scope.customer.DeliveryAddressObject.StateName + ', ' + $scope.customer.DeliveryAddressObject.CountryName + '</strong>'
                       //+ '</div></div></div></div>'
                       + '<br/>';
           }

              contents += '<div class="col-md-12 divlesspadding">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
                
            angular.forEach(rec.receiptItems, function (item, i) 
            {
                contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.9em"><td><img src="' + item.StoreItemStock.ImagePath + '" style="width:50px;height:40px">&nbsp;' + item.StoreItemStock.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td style="text-align: right">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            });

           contents += '</tbody></table></div>' +
               '<table style="width: 100%;font-size:0.9em"><tr><td><h5 style="float:left; text-align:left">Payment method(s):</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Payment details:</h5></td>' +
               '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td><b style="text-align: right">' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
           });

           contents += '</table></td><td>' +
                '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b>' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td><b style="text-align: right">' + filterCurrency(rec.amountReceived, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td><b style="text-align: right">' + filterCurrency(rec.amountToBalance, '') + '</b></td></tr></table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                '<h5>Served by: <b>' + rec.cashier + '</b></h5></div></div>';

           //angular.element('#receipt').append(contents);

           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null)
           {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function ()
               {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Thanks for patronizing us</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function ()
                   {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                   },

                   500);
               });
           }
           else
           {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Thanks for patronizing us</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function ()
               {
                    window.frames["frame1"].focus();
                    window.frames["frame1"].print();
                    //frame.document.body.innerHTML = '';
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
       };
        
       $scope.getpItemPrices = function ()
       {
           if ($scope.criteria == undefined || $scope.criteria.trim().length < 1)
           {
               $scope.priceList = [];
               return;
           }

           //saleServices.getItemPrices($scope.criteria, $scope.getpItemPricesCompleted);
           
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

