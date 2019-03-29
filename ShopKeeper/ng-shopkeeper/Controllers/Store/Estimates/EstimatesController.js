"use strict";

define(['application-configuration', 'estimateServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngPSku', function() {
        return function($scope, ngPSku) {
            ngPSku.bind("keydown keypress", function (event)
            {
                if (event.which === 13) {
                    $scope.criteria = event.target.value;
                    $scope.getpItemPrices();
                }
            });
            $scope.skupControl = ngPSku;
            $scope.skupControl.focus();
        };
    });
    app.register.directive('ngEstimates', function ($compile)
    {
        return function ($scope, ngEstimates)
        {
            var tableOptions = {};

            if ($scope.isAdmin === true || $scope.isCashier === true)
            {
                tableOptions.sourceUrl = "/Estimate/GetEstimates";
            }
            else
            {
                tableOptions.sourceUrl = "/Estimate/GetMyEstimates";
            }

            tableOptions.itemId = 'Id';
            tableOptions.columnHeaders = ["EstimateNumber", "CustomerName", "DateCreatedStr", "AmountDueStr", 'NetAmountStr', "GeneratedByEmployee", 'InvoiceStatus'];
            var ttc = estimatesTableManager($scope, $compile, ngEstimates, tableOptions, 'New Estimate', 'newEstimate', 'getEstimate', 'getEstimateDetails', 123);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };

    });

    app.register.controller('estimateController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'estimateServices', '$filter', '$locale',
        function (ngDialog, $scope, $rootScope, $routeParams, estimateServices, $filter, $locale)
        {
            //date picker settings
            var xcvb = new Date();
            var year = xcvb.getFullYear();
            var month = xcvb.getMonth() + 1;
            var day = xcvb.getDate();
            var maxDate = year + '/' + month + '/' + day;

        setControlDate($scope, '', maxDate);

        $scope.goBack = function ()
            {
                $scope.newEdit = false;
                $scope.processInvoice = false;
                $scope.details = false;
                $scope.search = { skuName: '' };
                $scope.initializeModel();
            }

        $scope.initializeController = function ()
            {
                $scope.newEdit = false;
                $scope.processInvoice = false;
                $scope.details = false;
                $scope.search = { skuName: '' };
                $scope.genders = [{ genderId: 1, name: 'Male' }, { genderId: 2, name: 'Female' }];
                $scope.initializeModel();
                $scope.getProducts();
        };

        $scope.initializeGenSale = function ()
        {
            $scope.genericSale =
                 {
                     Sale:
                         {
                             SaleId: 0,
                             RegisterId: null,
                             CustomerId: $scope.estimate.CustomerId,
                             EmployeeId: 0,
                             AmountDue: $scope.estimate.AmountDue, 'Status': '', 'Date': '',
                             CustomerObject : {},
                             Discount: $scope.estimate.Discount,
                             VATAmount: $scope.estimate.VATAmount,
                             DiscountAmount: $scope.estimate.DiscountAmount,
                             NetAmount: $scope.estimate.NetAmount, VAT: $scope.estimate.VAT,
                             EstimateNumber: $scope.estimate.EstimateNumber, ProcessEstimate: true,
                             StoreItemSoldObjects : []
                     },
                     StoreTransactions: [],
                     paymentOption: { 'StorePaymentMethodId': '', 'Name': '-- select payment option --' }
                 };
            $scope.amountPaid = 0;
        };  
        
        $scope.prepareInvoice = function ()
        {
            if ($scope.estimate.Id > 0 && $scope.estimate.ConvertedToInvoice === false)
            {
                $scope.initializeGenSale();

                angular.forEach($scope.estimate.EstimateItemObjects, function (d, i)
                {
                    var tempId = $scope.genericSale.Sale.StoreItemSoldObjects.length + 1;

                    var stockList = $scope.items.filter(function (item)
                    {
                        return item.StoreItemStockId === d.StoreItemStockId;
                    });
                    
                    if (stockList.length > 0)
                    {
                        var pricesList = stockList[0].ItemPriceObjects.filter(function (p)
                        {
                            return p.Price === d.ItemPrice;
                        });

                        if (pricesList.length > 0)
                        {
                            var soldItem =
                            {
                                'StoreItemSoldId': '',
                                'TempId': tempId,
                                'StoreItemStockId': d.StoreItemStockId,
                                'StoreItemStockObject': { 'StoreItemStockId': d.StoreItemStockId, 'StoreItemId': 0, 'StoreItemName': d.StoreItemName, 'Price': d.ItemPrice, 'MinimumQuantity': d.QuantityRequested, 'SKU': d.SerialNumber, 'ImagePath': d.ImagePath },
                                'SaleId': 0,
                                'SKU': d.SerialNumber,
                                'GotFromSKU': true,
                                'Rate': d.ItemPrice,
                                'ImagePath': d.ImagePath,
                                'QuantitySold': d.QuantityRequested,
                                'AmountSold': d.ItemPrice,
                                'UoMId': pricesList[0].UoMId,
                                'UoMCode': pricesList[0].UoMCode,
                                'DateSold': ''
                            };

                            $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItem);
                        } 
                    }

                });

                $scope.CustomerObject = $scope.estimate.CustomerObject;
                $scope.CustomerObject.UserProfileName = $scope.estimate.CustomerName;
                
                if ($scope.genericSale.Sale.StoreItemSoldObjects.length > 0 && $scope.estimate.EstimateItemObjects.length === $scope.genericSale.Sale.StoreItemSoldObjects.length)
                {
                    $scope.processInvoice = true;
                }
                else
                {
                    alert('Action failed. Please try again later');
                }
            }
            
        };
            
        $scope.updateAmount = function (soldItem)
        {
            if (soldItem == null || soldItem.StoreItemStockId < 1)
            {
                $scope.setError('Invalid Operation');
                return;
            }
            
            if ($scope.genericSale === undefined || $scope.genericSale === null || $scope.genericSale.Sale === undefined || $scope.genericSale.Sale === null)
            {
                $scope.initializeGenSale();
            }

            var test1 = parseFloat(soldItem.QuantityRequested);

            var test2 = parseFloat(soldItem.ItemPrice);

            if (isNaN(test1) || test1 < 1 || isNaN(test2) || test2 < 1) {
                $scope.estimate.AmountDue = $scope.estimate.AmountDue - soldItem.AmountSold;
                $scope.genericSale.Sale.AmountDue = $scope.estimate.AmountDue;

                $scope.estimate.NetAmount = $scope.estimate.NetAmount - soldItem.AmountSold;
                $scope.genericSale.Sale.NetAmount = $scope.estimate.NetAmount;
                soldItem.AmountSold = 0;
                $scope.updateAmountForDiscount();
            }
            else
            {
                soldItem.AmountSold = test1 * test2;

                var totalAmount = 0;

                angular.forEach($scope.estimate.EstimateItemObjects, function (t, k) {
                    totalAmount += t.AmountSold;
                });

                $scope.estimate.AmountDue = totalAmount;
                $scope.estimate.NetAmount = totalAmount;

                $scope.genericSale.Sale.NetAmount = totalAmount;
                $scope.genericSale.Sale.AmountDue = totalAmount;

                $scope.updateAmountForDiscount();

                $scope.posAmmount = '';
                $scope.balance = 0;
                $scope.amountPaid = 0;
                $scope.cashAmmount = '';
            }
        };

        $scope.closeInvoice = function ()
        {
            $scope.initializeSale();
           $scope.processInvoice = false;
        };

        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        $scope.newEstimate = function ()
        {
            $scope.newEdit = true;
            $scope.initializeModel();
            $scope.initializeGenSale();
        };

        $scope.initializeModel = function ()
        {
            $scope.selected = undefined;
            $scope.customer = null;
            $scope.cash = true;
            $scope.CustomerObject = { UserProfileName: '' };
            $scope.customer = { UserProfileName: '' };
            $scope.customerInfo = { UserProfileName: '' };
            $scope.customerDetail = { UserProfileName: '' }
            $scope.amountPaid = 0;

            $scope.estimate =
            {
                Id: 0,
                ConvertedToInvoice: false,
                EstimateNumber: '',
                CreatedById: '',
                DateCreated: '',
                DateConverted: '',
                VAT: 0,
                Discount: 0,
                CustomerId: '',
                LastUpdated: '',
                OutletId: '',
                AmountDue: 0,
                NetAmount: 0,
                VATAmount: 0,
                DiscountAmount: 0,
                Header: 'New Proforma Invoice',
                applyVat: false,
                CustomerObject:
                {
                    CustomerId: 0,
                    StoreCustomerTypeId: '',
                    ReferredByCustomerId: null,
                    StoreOutletId: '',
                    UserId: 0,
                    FirstPurchaseDate: null,
                    LastName: '',
                    Birthday: '',
                    OtherNames: '',
                    UserProfileName: '',
                    Gender: '',
                    ContactPersonId: '',
                    customerType: { StoreCustomerTypeId: '', Name: '-- Select --' }
                },
                
                EstimateItemObjects: []
            }
        };

        $scope.initializeEstimateItem = function ()
        {
            $scope.delAuth = { email: '', password: '' };
            $scope.estimateItem =
            {
                StoreItemStockId: 0,
                TempId: 0,
                Id: '',
                QuantityRequested: 0,
                ItemPrice: 0,
                GotFromSKU: true,
                EstimateId: 0,
                SerialNumber: '',
                StoreItemName: '',
                AmountSold: 0,
                ImagePath: '/Content/images/noImage.png'
            }

            $scope.xxd = { 'ImagePath': '/Content/images/noImage.png', 'StoreItemName': '', 'SKU': '', 'Price': '' };
            
            $scope.sku = '';
            $scope.getFromSKU = false;
        };

        $scope.getProducts = function ()
        {
            $scope.items = [];
            $scope.page = 0;
            $scope.itemsPerPage = 50;
            $scope.initializeEstimateItem();
            $scope.estimateItem.StoreItemStock = {'StoreItemStockId': '', 'StoreItemName': '-- Select Product --' };
            $scope.getList();
            //estimateServices.getAllProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
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
            
            if (d.ItemPriceObjects.length < 1)
            {
                $scope.setError('An unknown error was encountered. Please try again later.');
                return;
            }

            d.ItemPriceObjects.sort(function (a, b) {
                return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
            });

            var minQty = 1;
            
           
            //if not selected before, add it to the Collection
            var tempId = $scope.estimate.EstimateItemObjects.length + 1;
            var estimateItem =
            {
                StoreItemStockId: d.StoreItemStockId,
                TempId: tempId,
                Id: '',
                QuantityRequested: minQty,
                ItemPrice: 0,
                GotFromSKU: true,
                StoreItemStockObject: d,
                EstimateId: $scope.estimate.Id,
                ImagePath: d.ImagePath,
                SerialNumber: d.SKU,
                StoreItemName: d.StoreItemName,
                'UomCode': d.ItemPriceObjects[0].UoMCode,
                AmountSold: 0,
                ItemPriceObjects: d.ItemPriceObjects
            }
           
            $scope.clearError();
            $scope.estimate.EstimateItemObjects.push(estimateItem);
            //$scope.updateAmount(estimateItem);
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
           
           var secondResults = $scope.estimate.EstimateItemObjects.filter(function (s)
           {
               return (s.SKU === i);
           });

           if (secondResults.length > 0)
           {
               return;
           }

           var d = results[0];

           var tempId = $scope.estimate.EstimateItemObjects.length + 1;

           var estimateItem =
           {
               'StoreestimateItemId': '',
               'TempId': tempId,
               'StoreItemStockId': d.StoreItemStockId,
               'StoreItemStock': { 'StoreItemStockId': d.StoreItemStockId, 'StoreItemId': d.StoreItemId, 'StoreItemName': d.StoreItemName, 'Price': d.Price, 'MinimumQuantity': d.MinimumQuantity, 'SKU': d.SKU, 'ImagePath': d.ImagePath },
               'Id': 0,
               'SKU': d.SKU,
               'GotFromSKU': true,
               'Rate': d.Price,
               'ImagePath': d.ImagePath,
               'QuantityRequested': 1,
               'ItemPrice': d.Price,
               'UoMId': d.UoMId,
               'DateSold': ''
           };

           $scope.getFromSKU = true;
           $scope.clearError();
           $scope.estimate.EstimateItemObjects.push(estimateItem);
           $scope.updateAmount(estimateItem);
       };

       $scope.getList = function ()
       {
           estimateServices.getList($scope.getListCompleted);
       };

       $scope.getListCompleted = function (response)
       {
           //$scope.customers = response.Customers;
           $scope.customerTypes = response.CustomerTypes;
           $scope.paymentMethods = response.PaymentMethods;
       };
       
       $scope.setQuantity = function (storeItemStock)
       {
           if (storeItemStock.StoreItemStockId < 1)
           {
                return;
           }
           
           $scope.estimateItem.QuantityRequested = 1;
           $scope.addEstimateItem();
       };

       $scope.setPaymentOption = function (opt)
       {

           if (opt.StorePaymentMethodId < 1) {
               return;
           }

           $scope.ammountPaid = '';
           $scope.posAmmount = '';
           $scope.cashAmmount = '';
           $scope.genericSale.Sale.paymentOption = opt;
           $scope.cashAmount = $scope.genericSale.Sale.NetAmount;
           $scope.updateamountpaid($scope.cashAmount);

           $scope.genericSale.Sale.paymentOption = opt;
           $scope.ammountPaid = '';
           $scope.posAmmount = '';
           $scope.cashAmmount = '';
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
           estimateServices.getAllProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
       };

       $scope.sortCollection = function (collection)
       {
           collection.sort(function (a, b)
           {
               return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
           });
        };

       $scope.addEstimateItem = function ()
       {
           if ($scope.estimateItem.StoreItemStock.StoreItemStockId < 1)
           {
               $scope.setError('Please select a product');
               return;
           }
           
           if ($scope.estimateItem.QuantityRequested < 1)
           {
               $scope.setError('Please provide a valid Quantity');
               return;
           }

           var price = 0;
           var rate = 0;
           $scope.presubtotal = 0;
           var totalAmount = 0;
           
          var ssz = $scope.getPrice($scope.estimateItem);
          if (ssz.rate < 1) 
          {
              $scope.setError('An error was encountered. Please try again later.');
              return;
          }

          rate = ssz.rate;
          $scope.presubtotal = ssz.presubtotal;
          price = ssz.price;

           $scope.estimateItem.ItemPrice = price;
           $scope.estimateItem.ImagePath = $scope.estimateItem.StoreItemStock.StoreItemStockId;
          
           if ($scope.estimate.EstimateItemObjects.length > 0)
           {
               var matchFound = false;
               for (var m = 0; m < $scope.estimate.EstimateItemObjects.length; m++)
               {
                   var x = $scope.estimate.EstimateItemObjects[m];
                   if (x.StoreItemStock.StoreItemStockId === estimateItem.StoreItemStock.StoreItemStockId)
                   {
                       x.QuantityRequested += estimateItem.QuantityRequested;
                        x.ItemPrice += estimateItem.ItemPrice;
                        matchFound = true;
                   }
               }
               
               if (!matchFound)
               {
                 estimateItem.TempId = $scope.estimate.EstimateItemObjects.length + 1;
                 $scope.estimate.EstimateItemObjects.push(estimateItem);
              }
           }
           else
           {
               estimateItem.TempId = 1;
               $scope.estimate.EstimateItemObjects.push(estimateItem);
           }
           
           angular.forEach($scope.estimate.EstimateItemObjects, function (t, k)
           {
               totalAmount += t.ItemPrice;
           });
           
           $scope.estimate.AmountDue = totalAmount;
           
           $scope.initializeEstimateItem();
       };
        
       //Get price of items prices with minimum quantity matching or nearest the estimate item quantity 
       $scope.getPrice = function (item)
       {
           var priceResult = { 'rate': 0, 'price': 0 };

           var results = $scope.items.filter(function (u)
           {
               return u.StoreItemStockId === item.StoreItemStockId;
           });

           if (results.length < 1) {
               $scope.setError('An error was encountered. Please refresh the page and try again');
               return priceResult;
           }

           var x = results[0].ItemPriceObjects;

           if (x.length < 1)
           {
               alert('A fatal error was encountered. The requested operation was aborted');
               return priceResult;
           }

           if (x.length === 1)
           {
               priceResult.rate = x[0].Price;
               priceResult.price = x[0].Price * item.QuantityRequested;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1)
               {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }

           var result = x.filter(function (u) {
               return u.MinimumQuantity === item.QuantityRequested;
           });

           if (result.length > 0) {

               priceResult.rate = result[0].Price;
               priceResult.price = result[0].Price * item.QuantityRequested;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }

           if (item.QuantityRequested >= x[x.length - 1].MinimumQuantity)
           {
               priceResult.rate = x[x.length - 1].Price;
               priceResult.price = x[x.length - 1].Price * item.QuantityRequested;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }
           if (item.QuantityRequested <= x[0].MinimumQuantity)
           {
               priceResult.rate = x[0].Price;
               priceResult.price = x[0].Price * item.QuantityRequested;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }

           else
           {
               for (var i = 0; i < x.length; i++)
               {
                   if (item.QuantityRequested <= x[i].MinimumQuantity) {
                       priceResult.rate = x[i].Price;
                       priceResult.price = x[i].Price * item.QuantityRequested;
                       if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                           priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                       }
                       return priceResult;
                   }
                   else {
                       if (item.QuantityRequested > x[i].MinimumQuantity && item.QuantityRequested <= x[i + 1].MinimumQuantity)
                       {
                           priceResult.rate = x[i + 1].Price;
                           priceResult.price = x[i + 1].Price * item.QuantityRequested;
                           if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                               priceResult.price = Math.ceil(priceResult.price / 100) * 100;
                           }
                           return priceResult;
                       }
                   }

               }
               return priceResult;
           }

       }

       $scope.checkQuantity = function (estimateItem)
       {
           var test = parseFloat(estimateItem.QuantityRequested);

           if (isNaN(test) || test < 1)
           {
               var confirmDelete = 'Do you want to remove ' + estimateItem.StoreItemName + ' from the list?';
               if (!confirm(confirmDelete))
               {
                   estimateItem.QuantityRequested = 1;
                   $scope.updateAmount(estimateItem);
                   return;
               }

               $scope.removeEstimateItem2(estimateItem.TempId);
               return;
           }
        };

       $scope.updateAmountForDiscount = function ()
       {
           if ($scope.estimate.NetAmount.length < 1)
           {
               return;
           }

           if ($scope.genericSale === undefined || $scope.genericSale === null || $scope.genericSale.Sale === undefined || $scope.genericSale.Sale === null) {
               $scope.initializeGenSale();
           }

           var discTest = parseFloat($scope.estimate.Discount);
           if (discTest > 0)
           {
               var disc = parseFloat($scope.estimate.Discount);
               var discountAmount = 0;

               if ($scope.discountHolder < disc)
               {
                   discountAmount = (disc * $scope.estimate.AmountDue) / 100;
                   $scope.discountHolder = disc;
                   $scope.estimate.DiscountAmount = discountAmount;
                   $scope.genericSale.Sale.DiscountAmount = discountAmount;
                   $scope.estimate.Discount = disc;
                   $scope.genericSale.Sale.Discount = disc;
                   var amountLessDiscount = $scope.estimate.AmountDue - discountAmount;
                   $scope.estimate.NetAmount = amountLessDiscount;
                   $scope.genericSale.Sale.NetAmount = amountLessDiscount;
               }
               else
               {
                   if ($scope.discountHolder > disc)
                   {
                       var xrem = $scope.discountHolder - disc;
                 
                       discountAmount = (disc * $scope.estimate.AmountDue) / 100;

                       $scope.estimate.DiscountAmount = discountAmount;
                       $scope.genericSale.Sale.DiscountAmount = discountAmount;

                       $scope.estimate.Discount = disc;
                       $scope.genericSale.Sale.Discount = disc;
                       var reconciliator = (xrem * $scope.estimate.AmountDue) / 100;
                       $scope.estimate.NetAmount = (($scope.estimate.NetAmount + reconciliator) - discountAmount).toFixed(2);
                       $scope.genericSale.Sale.NetAmount = $scope.estimate.NetAmount;
                       $scope.discountHolder = disc;

                   }

                   if ($scope.discountHolder === disc)
                   {
                       discountAmount = (disc * $scope.estimate.AmountDue) / 100;

                       $scope.discountHolder = disc;
                       $scope.estimate.DiscountAmount = discountAmount;
                       $scope.genericSale.Sale.DiscountAmount = discountAmount;

                       $scope.estimate.Discount = disc;

                       $scope.genericSale.Sale.Discount = disc;
;
                       $scope.estimate.NetAmount = $scope.estimate.AmountDue - discountAmount;

                       $scope.genericSale.Sale.NetAmount = $scope.estimate.NetAmount;

                   }
               }
           }
           else
           {
                $scope.estimate.NetAmount = $scope.estimate.AmountDue;
                $scope.discountHolder = 0;
                $scope.estimate.Discount = 0;
                $scope.genericSale.Sale.Discount = 0;
                $scope.estimate.DiscountAmount = 0;
                $scope.genericSale.Sale.DiscountAmount = 0;
           }

           $scope.applyVat();
       };
            
       $scope.applyVat = function ()
       {
           if (parseFloat($scope.estimate.NetAmount) < 1)
           {
               return;
           }

           if ($scope.genericSale === undefined || $scope.genericSale === null || $scope.genericSale.Sale === undefined || $scope.genericSale.Sale === null)
           {
               $scope.initializeGenSale();
           }

           if ($scope.estimate.applyVat === false && $scope.estimate.VATAmount > 0)
           {
               $scope.estimate.NetAmount -= $scope.estimate.VATAmount;
               $scope.genericSale.Sale.NetAmount  -= $scope.estimate.VATAmount;
               $scope.estimate.VATAmount = 0;
               $scope.genericSale.Sale.VATAmount = 0;
               $scope.estimate.VAT = 0;
               $scope.genericSale.Sale.VAT = 0;
               return;
           }

           if ($scope.estimate.applyVat === true)
           {
               $scope.estimate.VAT = $rootScope.store.VAT;
               $scope.genericSale.Sale.VAT = $rootScope.store.VAT;

               var vatAmount = ($scope.estimate.NetAmount * $rootScope.store.VAT) / 100;
               $scope.estimate.VATAmount = vatAmount;
               $scope.genericSale.Sale.VATAmount = vatAmount;
              
               $scope.estimate.NetAmount = ($scope.estimate.NetAmount + vatAmount).toFixed(2);
               $scope.genericSale.Sale.NetAmount = $scope.estimate.NetAmount;
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
            
       $scope.getGenericListCompleted = function (data)
       {
           $scope.customers = data.Customers;
       };
        
       $scope.prepareEstimateTemplate = function ()
       {
           $scope.initializeModel();
           $scope.estimate.Header = 'New Price(s)';
           $scope.clicked = true;
           $scope.buttonStatus = 1;
       };

       $scope.getTempEstimateItem = function (itemId)
       {
           if (itemId == undefined || itemId === NaN || parseInt(itemId) < 1)
           {
               alert('Invalid selection');
               return;
           }

           angular.forEach($scope.estimate.EstimateItemObjects, function (item, index)
           {
               if (item.TempId === parseInt(itemId))
               {
                   $scope.initializeEstimateItem();
                   $scope.estimateItem = item;
                   
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

       var index = 0;

       $scope.removeEstimateItem = function (id)
       {
           index = 0;
           $scope.email = "";
           $scope.password = "";
           $scope.deleteItem = false;

            if (id < 1)
            {
                $scope.setError('Invalid selection');
                return;
            }
            var matchedItem = {};
            
            angular.forEach($scope.estimate.EstimateItemObjects, function (x, y)
            {
                if (x.TempId === id)
                {
                    index = y;
                    matchedItem = x;
                }
            });

           
            if (matchedItem.Id > 0)
            {
                if (!confirm("This Item will be removed permanently. Continue?"))
                {
                    return;
                }
                estimateServices.deleteEstimateItem(matchedItem.Id, $scope.deleteEstimateItemCompleted);  
            }

            else
            {
                if (!confirm("This Item will be removed from the list. Continue?"))
                {
                    return;
                }
                $scope.estimate.EstimateItemObjects.splice(index, 1);
                $scope.search.skuName = '';
                $scope.completeDeleteProcess();
            }
          
       };
     
       $scope.deleteEstimateItemCompleted = function (data)
       {
           alert(data.Error);

           if (data.Code < 1)
           {
               return;
           }
           $scope.estimate.EstimateItemObjects.splice(index, 1);
           $scope.search.skuName = '';
           $scope.completeDeleteProcess();
       };

       $scope.completeDeleteProcess = function ()
       {
           
           if ($scope.genericSale === undefined || $scope.genericSale === null || $scope.genericSale.Sale === undefined || $scope.genericSale.Sale === null)
           {
               $scope.initializeGenSale();
           }

           var totalAmount = 0;
           angular.forEach($scope.estimate.EstimateItemObjects, function (y, i)
           {
               totalAmount += y.ItemPrice;
           });

           if (totalAmount < 1)
           {
               $scope.estimate.AmountDue = 0;
               $scope.genericSale.Sale.AmountDue = 0;
           }
           else
           {
               $scope.estimate.AmountDue = totalAmount;
               $scope.genericSale.Sale.AmountDue = totalAmount;
           }
           $scope.estimate.Discount = 0;
           $scope.genericSale.Sale.Discount = 0;
           $scope.estimate.VAT = 0;
           $scope.genericSale.Sale.VAT = 0;
           $scope.estimate.DiscountAmount = 0;
           $scope.genericSale.Sale.DiscountAmount = 0;
           $scope.estimate.VATAmount = 0;
           $scope.genericSale.Sale.VATAmount = 0;
           $scope.updateAmountForDiscount();
           $scope.deleteItem = false;
           $scope.initializeEstimateItem();
       };

       $scope.removeEstimateItem2 = function (id) {
           if (id < 1) {
               $scope.setError('Invalid selection');
               return;
           }

           angular.forEach($scope.estimate.EstimateItemObjects, function (x, y)
           {
               if (x.TempId === id) {
                  $scope.estimate.EstimateItemObjects.splice(y, 1);
               }
           });

           var totalAmount = 0;
           angular.forEach($scope.estimate.EstimateItemObjects, function (y, i)
           {
               totalAmount += y.ItemPrice;
           });

           if (totalAmount < 1) {
               $scope.estimate.AmountDue = '';
           }
           else {
               $scope.estimate.AmountDue = totalAmount;
           }

           $scope.initializeEstimateItem();
       };

       $scope.validateEstimateItem = function (estimateItem)
       {
           if (estimateItem.StoreItemStock.StoreItemStockId == undefined || estimateItem.StoreItemStock.StoreItemStockId == null || StoreItemStockId.StoreItemStock.StoreItemStockId < 1)
           {
                alert("ERROR: Please select a Product. ");
                return false;
            }

           if (estimateItem.QuantityRequested == undefined || estimateItem.QuantityRequested == null || estimateItem.QuantityRequested < 1)
           {
                alert("ERROR: Please provide Item Quantity. ");
                return false;
           }

           if (estimateItem.ItemPrice == undefined || estimateItem.ItemPrice == null || estimateItem.ItemPrice < 1)
           {
               alert("ERROR: Please Provide Price. ");
               return false;
           }

            return true;
       };
        
       $scope.validateEstimate = function (estimate)
       {
           if (estimate.AmountDue == undefined || estimate.AmountDue == null || estimate.AmountDue < 1)
           {
               $scope.setError('An unexpected error was encountered. Please review this Transaction details and try again.');
               return false;
           }
           
           if (estimate.EstimateItemObjects == undefined || estimate.EstimateItemObjects == null || estimate.EstimateItemObjects < 1)
           {
               $scope.setError('ERROR: Please add at least one product for this transaction.');
               return false;
           }
           
           return true;
       };

       $scope.checkAmountPaid = function (amountPaid)
       {
           if (amountPaid !== $scope.estimate.NetAmount)
           {
               var testResult = $scope.estimate.NetAmount - amountPaid;
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
       
       $scope.processEstimate = function ()
       {
            if (!$scope.validateEstimate($scope.estimate))
            {
               return;
            }

            $scope.processing = true;

            if ($scope.estimate.Id < 1)
            {
                estimateServices.addEstimate($scope.estimate, $scope.processEstimateCompleted);
            }
            else
            {
                estimateServices.editEstimate($scope.estimate, $scope.processEstimateCompleted);
            }
       };
        
       $scope.processEstimateCompleted = function (data)
       {
           $scope.processing = false;
           if (data.Code < 1)
           {
              $scope.setError(data.Error);
               return;
           }
           else
           {
               $scope.estimate.Id = data.Code;
               $scope.estimate.CustomerId = data.CustomerId;
               $scope.estimate.EstimateNumber = data.ReferenceCode;

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
                   time: data.Time,
                   customer : { UserProfileName: $scope.estimate.CustomerName },
                   storeAddress : $rootScope.store.StoreAddress,
                   cashier: $rootScope.user.Name,
                   receiptItems: $scope.estimate.EstimateItemObjects,
                   amountDue: $scope.estimate.AmountDue,
                   subtotal: $scope.presubtotal,
                   amountReceived: $scope.amountPaid,
                   netAmount: $scope.estimate.NetAmount,
                   discountAmount: $scope.estimate.DiscountAmount,
                   vatAmount: $scope.estimate.VATAmount
               };
         
               $scope.ngTable.fnClearTable();
               $scope.printEstimate($scope.rec);
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
            
       $scope.printReceipt = function (rec)
       {
           var contents =
                '<div class="row"><div class="col-md-12"><h4>Estimate</h4></div></div><br/>' +

                 '<div class="row"><div class="col-md-4"></div><div class="col-md-4">' +
                   '<img src="' + $rootScope.store.StoreLogoPath + '" alt="" style="width: 50px; height: 50px"/>' +
                   '</div><div class="col-md-4"></div></div><div class="row" style="margin-top: 2%"><div class="col-md-12 divlesspadding">' +
                   '<label class="ng-binding">' + $rootScope.store.StoreName + '</label> | <label class="ng-binding">' + $rootScope.store.StoreEmail + '</label>' +
                   '</div></div>' +
                   '<div class="row"><div class="col-md-12 divlesspadding"><div class="row"><div class="col-md-10 divlesspadding"><h5>' + rec.storeAddress + '</h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Date: <b>' + rec.date + rec.time + '</b></h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Ref. No: <b>' + rec.referenceCode + '</b></h5>' +
                   '</div></div></div></div><br/><div class="row"><div class="col-md-12 divlesspadding">Customer : <strong>' + $scope.customer.UserProfileName + '</strong></div>'
                       + '</div><br/><br/><div class="col-md-12 divlesspadding">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b style="text-align: right">' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr></table>';
                
            angular.forEach(rec.receiptItems, function (item, i) 
            {
                contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.9em"><td>' + item.StoreItemName + '(' + item.UoMCode + ')' + '</td><td>' + item.QuantityRequested + '</td><td>' + filterCurrency(item.ItemPrice, '') + '</td>' +
                    '<td style="text-align: right">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            });

           contents += '</tbody></table></div></table></td><td>' +
                '<div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                '<h5>Generated by: <b>' + rec.cashier + '</b></h5></div></div>';

           var popupWin = '';
           if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
           {
               popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.window.focus();
               popupWin.document.write('<!DOCTYPE html><html><head>' +
                   '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                   '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
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
               popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
               popupWin.document.close();
           }

           popupWin.document.close();
           $scope.initializeModel();
           return false;
       };

       $scope.printEstimate = function (rec)
       {
           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
                  + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
                  + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
                  + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
                  + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                  + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 70%; text-align: center; margin-left: 40%"><b style="border-bottom: 1px solid #000;">PROFORMA INVOICE</h3></b></td></tr>'
                  + '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date Generated: ' + rec.date + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Ref No.: ' + rec.referenceCode + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer.UserProfileName + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%"><h5>Generated by: <b>' + rec.cashier + '</b></h5></td></tr>'
                  +'<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;"></td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
                  + '</td></tr></table>';
           
           contents += '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

             angular.forEach(rec.receiptItems, function (item, i)
             {
                 contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.9em"><td>' + item.StoreItemName + '</td><td>' + item.QuantityRequested + '</td><td>' + filterCurrency(item.ItemPrice, '') + '</td>' +
                   '<td style="text-align: right">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
             });

           contents += '</tbody></table></div></table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b style="text-align: right">' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr></table>' +
               '<div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
               '</div></div>';
           
           var popupWin = '';
           if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
           {
               popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.window.focus();
               popupWin.document.write('<!DOCTYPE html><html><head>' +
                   '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                   '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
               popupWin.onbeforeunload = function (event) {
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
               popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
               popupWin.document.close();
           }
           
           popupWin.document.close();
           $scope.newEstimate();
           return false;
       };

       $scope.printEstimate2 = function (rec)
       {
           var contents =
                 '<div class="row"><div class="col-md-4"></div><div class="col-md-4">' +
                   '<img src="' + $rootScope.store.StoreLogoPath + '" alt="" style="height: 60px"/>' +
                   '</div><div class="col-md-4"></div></div><div class="row" style="margin-top: 2%"><div class="col-md-12 divlesspadding">' +
                   '<label class="ng-binding">' + $rootScope.store.StoreName + '</label> | <label class="ng-binding">' + $rootScope.store.StoreEmail + '</label>' +
                   '</div></div>' +
                   '<div class="row"><div class="col-md-12 divlesspadding"><div class="row"><div class="col-md-10 divlesspadding"><h5>' + rec.storeAddress + '</h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Date: <b>' + rec.date + rec.time + '</b></h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Ref. No: <b>' + rec.referenceCode + '</b></h5>' +
                   '</div></div></div></div><br/><div class="row"><div class="col-md-12 divlesspadding">Customer : <strong>' + $scope.customerInfo.UserProfileName + '</strong></div>'
                       + '</div><br/><br/><div class="col-md-12 divlesspadding">' +
                  '<table class="table" role="grid" style="width: 100%;font-size:1.06em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:1.06em"><th style="color: #008000;font-size:1.06em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:1.06em; width:20%;">Qty</th><th style="color: #008000;font-size:1.06em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:1.06em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i)
           {
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:1.06em"><td>" style="width:50px;height:40px">&nbsp;' + item.StoreItemName + '</td><td>' + item.QuantityRequested + '</td><td>' + filterCurrency(item.ItemPrice, '') + '</td>' +
                   '<td style="text-align: right">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
           });

           contents += '</tbody></table></div></table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:1.06em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:1.06em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b>' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr></table>' +
               '<div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:1.06em">' +
               '<h5>Generated by: <b>' + rec.cashier + '</b></h5></div></div>';
           angular.element('#invoiceInfo').html('');
           angular.element('#invoiceInfo').append(contents);
           $scope.newEdit = false;
           $scope.details = true;
       };
            
       $scope.getpItemPrices = function ()
       {
           if ($scope.criteria == undefined || $scope.criteria.trim().length < 1)
           {
               $scope.priceList = [];
               return;
           }

           //estimateServices.getItemPrices($scope.criteria, $scope.getpItemPricesCompleted);
           
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
       
       function getEstimateDetailsCompleted(rec)
       {
           if (rec == null || rec.Id < 1) {
               alert('Item could not be retrieved. Please try again later.');
               return;
           }

           angular.forEach(rec.EstimateItemObjects, function (g, i)
           {
               g.AmountSold = g.QuantityRequested * g.ItemPrice;
           });
          
           angular.forEach($scope.customers, function (c, i)
           {
               if (c.CustomerId === rec.CustomerId)
               {
                   $scope.customerInfo = c;
               }
           });
           
           if (rec.ConvertedToInvoice === true)
           {
               rec.Header = 'Estimate Invoice';
           }
           else
           {
               rec.Header = 'Estimate Details';
           }

           $scope.estimate = rec;
           
           $scope.rec =
            {
                referenceCode: rec.InvoiceNumber,
                estimateRef: rec.EstimateNumber,
                date: rec.DateCreatedStr,
                customer: rec.CustomerName,
                time: rec.Time,
                storeAddress: $rootScope.store.StoreAddress,
                cashier: rec.GeneratedByEmployee,
                receiptItems: rec.EstimateItemObjects,
                amountDue: rec.AmountDue,
                amountReceived: rec.AmountPaid,
                amountToBalance: rec.Balance,
                paymentChoices: rec.Transactions,
                netAmount: rec.NetAmount,
                discountAmount: rec.DiscountAmount,
                vatAmount: rec.VATAmount
            };
           

           $scope.buildHtml();
       };

       $scope.buildHtml = function ()
       {
           var rec = $scope.rec;

           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 50%; text-align: center; margin-left: 40%"><b style="border-bottom: 1px solid #000;">PROFORMA INVOICE</h3></b></td></tr>' +
               '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date: ' + rec.date + '</td></tr>';

           if (rec.referenceCode !== undefined && rec.referenceCode !== null && rec.referenceCode.length > 0) {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>';
           }


           if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0)
           {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
           }

           contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
           + '</td></tr></table>';


           contents += '<div class="col-md-12 divlesspadding">' +
            '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i)
           {
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.9em"><td><img src="' + item.ImagePath + '" style="width:50px;height:40px">&nbsp;' + item.StoreItemName + '</td><td>' + item.QuantityRequested + '</td><td>' + filterCurrency(item.ItemPrice, '') + '</td>' +
                   '<td>' + filterCurrency(item.QuantityRequested * item.ItemPrice, " ") + '</td></tr>';
           });
           
           contents += '</tbody></table></div>' +
               '<table style="width: 100%;font-size:0.9em"><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';
           

           contents += '<table style="width: 100%;font-size:0.9em"><tr><td></td><td><h5 style="float:right; text-align:right; margin-right:1%">Summary:</h5></td>' +
             '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';


           contents += '</table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td>' +
               '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b>' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
               '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr>';

           if (rec.amountReceived > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td><b style="text-align: right">' + filterCurrency(rec.amountReceived, '') + '</b></td></tr>';
           }

           if (rec.amountToBalance > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b style="text-align: right">' + filterCurrency(rec.amountToBalance, '') + '</b></td></tr>';
           }

           contents += '</table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
     '<h5>Served by: <b>' + rec.cashier + '</b></h5></div></div>';

           angular.element('#invoiceInfo').html('').append(contents);

           $scope.estimate.Header = 'Proforma Invoice';
           $scope.rec = rec;
           $scope.newEdit = false;
           $scope.details = true;
           $scope.rec.Processed = true;

       }

       $scope.getEstimateDetails = function (estimateNumber, status)
       {
           $scope.rec = {Processed : false};
           if (estimateNumber.length < 1)
           {
               alert('Invalid selection!');
               return;
           }

           if (status.toString() === 'Processed')
           {
               estimateServices.getEstimateInvoice(estimateNumber, $scope.getEstimateInvoiceCompleted);
           }
           else
           {
               estimateServices.getEstimateByRef(estimateNumber, getEstimateDetailsCompleted);
           }

           //var t = '<a ng-click="getEstimateDetails('+00003+',Processed)" style="cursor: pointer" title="Details"> <img src="/Content/images/details.png"></a>';

       };

       function getEstimateCompleted(data)
       {
           if (data == null || data.Id < 1)
           {
               alert('Item could not be retrieved. Please try again later.');
               return;
           }

           if ($scope.genericSale === undefined || $scope.genericSale === null || $scope.genericSale.Sale === undefined || $scope.genericSale.Sale === null) {
               $scope.initializeGenSale();
           }

           var tempId = 1;


           angular.forEach(data.EstimateItemObjects, function (g, i) {
               g.StoreItemStockObject = 
                   {
                   UoMCode: g.UoMCode,
                   QuantitySold: g.QuantityRequested,
                   ImagePath: g.ImagePath,
                   StoreItemName: g.StoreItemName,
                   SKU: g.SerialNumber
           }

               g.AmountSold = g.QuantityRequested * g.ItemPrice;
               g.TempId = tempId;
               tempId++;
           });

           $scope.initializeGenSale();

           $scope.vatHolder = data.VAT;
           $scope.discountHolder = data.Discount;
         
           $scope.genericSale.Sale.CustomerId = data.CustomerId;
           data.CustomerObject.UserProfileName = data.CustomerName;
           data.CustomerObject.CustomerName = data.CustomerName;

           $scope.CustomerObject = data.CustomerObject;
           $scope.customer = data.CustomerObject;
           $scope.customerInfo = data.CustomerObject;

           //$rootScope.skuControl = document.getElementById('customerControl_value');
           //$rootScope.skuControl.val(data.CustomerObject.UserProfileName);
           $rootScope.searchStr = data.CustomerObject.UserProfileName;

           
           data.Header = 'Update Proforma Invoice';
           
           if (data.VAT > 0)
           {
               data.applyVat = true;
           }
           $scope.estimate = data;
           $scope.newEdit = true;
       };

       $scope.getEstimate = function (id)
       {
           if (id < 1)
           {
               alert('Invalid selection!');
               return;
           }
           estimateServices.getEstimate(id, getEstimateCompleted);
       };

       $scope.initializeCustomer = function ()
       {
           $scope.estimate.CustomerObject =
             {
               CustomerId: 0,
               ReferredByCustomerId: null,
               StoreOutletId: '',
               UserId: 0,
               FirstPurchaseDate: null,
               newCustomer : false,
               LastName: '',
               OtherNames: '',
               UserProfileName: '',
               Gender: '',
               GenderObject : { genderId: '', name: '-- select --' },
               ContactPersonId: ''
             }
           //$scope.initializeGenSale();
       };

       $scope.addNewCustomer = function ()
       {
           if ($scope.estimate.CustomerId > 0)
           {
               if (!confirm('Do you want to replace the original customer attached to this estimate?'))
               {
                   return;
               }
           }

           $scope.initializeCustomer();
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Estimates/ProcessCustomers.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processCustomer = function ()
       {
           if ( $scope.estimate.CustomerObject.MobileNumber == null ||  $scope.estimate.CustomerObject.MobileNumber === undefined ||  $scope.estimate.CustomerObject.MobileNumber.length < 1) {
               alert("ERROR: Please provide customer's Mobile phone number.");
               return;
           }

           if ( $scope.estimate.CustomerObject.OtherNames == null ||  $scope.estimate.CustomerObject.OtherNames === undefined ||  $scope.estimate.CustomerObject.OtherNames.length < 1) {
               alert("ERROR: Please provide Customer's Other names.");
               return;
           }
           if ( $scope.estimate.CustomerObject.LastName == null ||  $scope.estimate.CustomerObject.LastName === undefined ||  $scope.estimate.CustomerObject.LastName.length < 1) {
               alert("ERROR: Please Customer's Last Name.");
               return;
           }

           if ( $scope.estimate.CustomerObject.customerType == null ||  $scope.estimate.CustomerObject.customerType === undefined ||  $scope.estimate.CustomerObject.customerType.StoreCustomerTypeId < 1) {
               alert("ERROR: Please select Customer Type.");
               return;
           }
           
           if ( $scope.estimate.CustomerObject.GenderObject == null ||  $scope.estimate.CustomerObject.GenderObject === undefined ||  $scope.estimate.CustomerObject.GenderObject.genderId < 1) {
               alert("ERROR: Please select a gender.");
               return;
           }

           //if ( $scope.estimate.CustomerObject.StoreCountryObject == null ||  $scope.estimate.CustomerObject.StoreCountryObject === undefined ||  $scope.estimate.CustomerObject.StoreCountryObject.StoreCountryId < 1) {
           //    alert("ERROR: Please select a Country.");
           //    return;
           //}

           //if ( $scope.estimate.CustomerObject.StoreStateObject == null ||  $scope.estimate.CustomerObject.StoreStateObject === undefined ||  $scope.estimate.CustomerObject.StoreStateObject.StoreStateId < 1) {
           //    alert("ERROR: Please select a state.");
           //    return;
           //}

           //if ( $scope.estimate.CustomerObject.StoreCityObject == null ||  $scope.estimate.CustomerObject.StoreCityObject === undefined ||  $scope.estimate.CustomerObject.StoreCityObject.StoreCityId < 1) {
           //    alert("ERROR: Please select a city.");
           //    return;
           //}

           //if ( $scope.estimate.CustomerObject.AddressLine1 == null ||  $scope.estimate.CustomerObject.AddressLine1 === undefined ||  $scope.estimate.CustomerObject.AddressLine1.length < 1) {
           //    alert("ERROR: Please provide Customer's street number.");
           //    return;
           //}

           // $scope.estimate.CustomerObject.DeliveryAddressObject.Id =  $scope.estimate.CustomerObject.DeliveryAddressObject.Id;
           // $scope.estimate.CustomerObject.DeliveryAddressObject.StateId =  $scope.estimate.CustomerObject.StoreStateObject.StoreStateId;
           // $scope.estimate.CustomerObject.DeliveryAddressObject.CountryId =  $scope.estimate.CustomerObject.StoreCountryObject.StoreCountryId;
           // $scope.estimate.CustomerObject.DeliveryAddressObject.AddressLine1 =  $scope.estimate.CustomerObject.AddressLine1;
           // $scope.estimate.CustomerObject.DeliveryAddressObject.AddressLine2 =  $scope.estimate.CustomerObject.DeliveryAddressObject.AddressLine2;
           // $scope.estimate.CustomerObject.DeliveryAddressObject.CityId =  $scope.estimate.CustomerObject.StoreCityObject.StoreCityId;

           //if ( $scope.estimate.CustomerObject.ContactPerson !== null ||  $scope.estimate.CustomerObject.ContactPerson.EmployeeId > 0){
           //     $scope.estimate.CustomerObject.ContactPersonId =  $scope.estimate.CustomerObject.ContactPerson.EmployeeId;
           //}

           
            $scope.estimate.CustomerObject.Gender =  $scope.estimate.CustomerObject.GenderObject.name;
            $scope.estimate.CustomerObject.StoreCustomerTypeId = $scope.estimate.CustomerObject.customerType.StoreCustomerTypeId;

           // $scope.estimate.CustomerObject.CustomerObjects[0].StoreOutletId =  $scope.estimate.CustomerObject.storeOutlet.StoreOutletId;

            $scope.estimate.CustomerObject.Birthday = $scope.estimate.CustomerObject.BirthdayStr;

            var profileName = $scope.estimate.CustomerObject.LastName + ' ' + $scope.estimate.CustomerObject.OtherNames + '(' + $scope.estimate.CustomerObject.MobileNumber + ')';;
            $scope.estimate.CustomerObject.UserProfileName = profileName;

            $scope.customerInfo = $scope.estimate.CustomerObject;
            $scope.customerDetail = $scope.estimate.CustomerObject;
            $scope.customer = $scope.estimate.CustomerObject;
            $scope.CustomerObject = $scope.estimate.CustomerObject;
            console.log(JSON.stringify($scope.CustomerObject));
            $scope.estimate.CustomerName = profileName;
            $scope.genericSale.Sale.CustomerObject = $scope.estimate.CustomerObject;

            $scope.genericSale.Sale.CustomerId = 0;
            $scope.estimate.CustomerId = 0;
            $scope.genericSale.Sale.CustomerId = 0;

           //if ( $scope.estimate.CustomerObject.Id == null ||  $scope.estimate.CustomerObject.Id === '' || parseInt( $scope.estimate.CustomerObject.Id) === 0 || parseInt( $scope.estimate.CustomerObject.Id) < 1 ||  $scope.estimate.CustomerObject.Id == undefined) {
           //    customerServices.addCustomer( $scope.estimate.CustomerObject, $scope.processCustomerCompleted);
           //}
           //else {
           //    customerServices.editCustomer( $scope.estimate.CustomerObject, $scope.processCustomerCompleted);
           //}

            ngDialog.close('/ng-shopkeeper/Views/Store/Estimates/ProcessCustomers.html', '');
       };

       $rootScope.setCustomer = function (user)
       {
           if (user == null)
           {
               return;
           }

           var d = user.originalObject;

           $scope.estimate.CustomerId = d.CustomerId;
           $scope.genericSale.Sale.CustomerId = d.CustomerId;
           $scope.customer = d;
           $scope.customerInfo = d;
           $scope.CustomerObject = d;
           $scope.customerDetail = d;
           $scope.estimate.CustomerObject = d;
           $scope.estimate.CustomerName = d.UserProfileName;
       };

       $scope.preparePrint = function ()
       {
           if ($scope.customerInfo === undefined || $scope.customerInfo === null || $scope.customerInfo === undefined || $scope.customerInfo.CustomerId < 1)
           {
               $scope.customerInfo = { UserProfileName: $scope.estimate.CustomerName, CustomerId: $scope.estimate.CustomerId}
           }

           var rec =
           {
               referenceCode: $scope.estimate.EstimateNumber,
               date: $scope.estimate.DateCreatedStr,
               customer: $scope.customerInfo,
               storeAddress: $rootScope.store.StoreAddress,
               cashier: $rootScope.user.Name,
               receiptItems: $scope.estimate.EstimateItemObjects,
               amountDue: $scope.estimate.AmountDue,
               netAmount: $scope.estimate.NetAmount,
               discountAmount: $scope.estimate.DiscountAmount,
               vatAmount: $scope.estimate.VATAmount
           };
       
           $scope.printEstimate(rec);
       };
            
       $scope.getEstimateInvoiceCompleted = function (rec)
       {
           if (rec.SaleId < 1)
           {
               alert("Report information could not be retrieved");
               return;
           }
          

           var reportHtml =
               '<div class="row"><div class="col-md-4"></div><div class="col-md-4">' +
                   '<img src="' + $rootScope.store.StoreLogoPath + '" alt="" style="height: 60px"/>' +
                   '</div><div class="col-md-4"></div></div><div class="row" style="margin-top: 2%"><div class="col-md-12 divlesspadding">' +
                   '<label class="ng-binding">' + $rootScope.store.StoreName + '</label> | <label class="ng-binding">' + $rootScope.store.StoreEmail + '</label>' +
                   '</div></div>' +
                   '<div class="row"><div class="col-md-12 divlesspadding"><div class="row"><div class="col-md-10 divlesspadding"><h5>' + $rootScope.store.StoreAddress + '</h5>' +
                   '</div></div><div class="row"><div class="col-md-12 divlesspadding"><h5>Date: <b>' + rec.DateStr + '</b></h5>' +
                   '</div></div>';

           if (rec.InvoiceNumber !== undefined && rec.InvoiceNumber !== null && rec.InvoiceNumber.length > 0) {
               reportHtml += '<div class="row"><div class="col-md-12 divlesspadding"><h5>Invoice No: <b>' + rec.InvoiceNumber + '</b></h5>' +
                   '</div></div>';
           }

           reportHtml += '<div class="row"><div class="col-md-12 divlesspadding"><h5>Estimate Ref: <b>' + rec.EstimateNumber + '</b></h5>' +
                   '</div></div></div></div><br/>';

           if (rec.CustomerName !== null && rec.CustomerName.length > 0)
           {
               reportHtml += '<div class="row"><div class="col-md-12">Customer : <strong>' + rec.CustomerName + '</strong></div>'
                   + '</div><br/>';
           }
           
           reportHtml += '<div class="col-md-12">' +
            '<table class="table" role="grid" style="width: 100%;font-size:1em">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:1em"><th style="color: #008000;font-size:1em; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:1em; width:20%;">Qty</th><th style="color: #008000;font-size:1em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000;font-size:1em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.StoreItemSoldObjects, function (item, i)
           {

               reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td>' + filterCurrency(item.AmountSold, " ") + '</td></tr>';

               item.QuantityRequested = item.QuantitySold;
               item.ItemPrice = item.Rate;

           });

           reportHtml += '</tbody></table></div>' +
               '<table style="width: 100%;font-size:1em"><tr><td><h5 style="padding: 8px;">Payment method(s):</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Payment details:</h5></td>' +
               '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:1em; float:left">';

           angular.forEach(rec.Transactions, function (l, i) {
               reportHtml += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:1em;">' + l.PaymentMethodName + ':</td>' +
                            '<td><b style="text-align: right">' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
           });

           reportHtml += '</table></td><td>' +
                '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:1em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:1em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td style="text-align: right"><b>' + filterCurrency(rec.AmountDue, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right"><b>' + filterCurrency(rec.DiscountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td style="text-align: right"><b>' + filterCurrency(rec.VATAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td style="text-align: right"><b>' + filterCurrency(rec.NetAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td style="text-align: right"><b>' + filterCurrency(rec.AmountPaid, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.Balance, '') + '</b></td></tr></table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:1.06em">' +
               '<h5>Served by: <b>' + rec.SaleEmployeeName + '</b></h5></div></div>';
           
           angular.element('#invoiceInfo').html('').append(reportHtml);

           $scope.estimate.Header = 'Proforma Invoice';
           $scope.rec = rec;
           $scope.newEdit = false;
           $scope.details = true;
           $scope.rec.Processed = true;

           $scope.rec =
              {
                  referenceCode: rec.InvoiceNumber,
                  estimateRef: rec.EstimateNumber,
                  date: rec.DateStr,
                  customer: rec.CustomerName,
                  time: rec.Time,
                  storeAddress: $rootScope.store.StoreAddress,
                  cashier: $rootScope.user.Name,
                  receiptItems: rec.StoreItemSoldObjects,
                  amountDue: rec.AmountDue,
                  amountReceived: rec.AmountPaid,
                  amountToBalance: rec.Balance,
                  paymentChoices: rec.Transactions,
                  netAmount: rec.NetAmount,
                  discountAmount: rec.DiscountAmount,
                  vatAmount: rec.VATAmount
              };
       };

       $scope.preparePrint2 = function ()
       {
           if ($scope.rec.Processed === true)
           {
               var contents = angular.element('#invoiceInfo').html();
               var html = '<div class="row"><div class="col-md-12"><h4>Estimate Invoice</h4></div></div><br/>' + contents;
               var popupWin = '';
               if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
               {
                   popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
                   popupWin.window.focus();
                   popupWin.document.write('<!DOCTYPE html><html><head>' +
                       '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                       '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + html + '</div></div></html>');
                   popupWin.onbeforeunload = function (event) {
                       popupWin.close();
                       return '.\n';
                   };
                   popupWin.onabort = function (event) {
                       popupWin.document.close();
                       popupWin.close();
                   }
               }
               else {
                   popupWin = window.open('', '_blank', 'width=800,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
                   popupWin.document.open();
                   popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + html + '</div></div></html>');
                   popupWin.document.close();
               }

               popupWin.document.close();
               return false;
           }
           else
           {
               var rec =
                 {
                     referenceCode: $scope.estimate.EstimateNumber,
                     date: $scope.estimate.DateCreatedStr,
                     customer: $scope.customerInfo,
                     storeAddress: $rootScope.store.StoreAddress,
                     cashier: $rootScope.user.Name,
                     receiptItems: $scope.estimate.EstimateItemObjects,
                     amountDue: $scope.estimate.AmountDue,
                     netAmount: $scope.estimate.NetAmount,
                     discountAmount: $scope.estimate.DiscountAmount,
                     vatAmount: $scope.estimate.VATAmount
                 };
               $scope.printEstimate(rec);
           }

       };

       /************************************************  PROCESS ESTIMATE  *************************************************************************/

       $scope.initializeSale = function ()
       {
           $scope.initializeSoldItem();
           $scope.initializeTransaction();

           $scope.posAmount = '';
           $scope.cashAmount = '';

           $scope.discountHolder = 0;
           $scope.vatHolder = 0;

           $scope.lessAmount = '';
           $scope.incompleteAmountDue = false;
       };

       $scope.initializeTransaction = function () {
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

       $scope.addSoldItem = function () {
           if ($scope.soldItem.StoreItemStock.StoreItemStockId < 1) {
               $scope.setError('Please select a product');
               return;
           }

           if ($scope.soldItem.StoreItemStock.PurchaseOrderItemId < 1) {
               $scope.setError('Selected product could not be processed. Please try again.');
               return;
           }

           if ($scope.soldItem.QuantitySold < 1) {
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
               'StoreItemStockObject':
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
               'ReOrderQuantityStr': $scope.soldItem.StoreItemStock.ReOrderQuantityStr,
               'ReOrderLevelStr': $scope.soldItem.StoreItemStock.ReOrderLevelStr,
               'ReorderLevel': $scope.soldItem.StoreItemStock.ReorderLevel,
               'ReorderQuantity': $scope.soldItem.StoreItemStock.ReorderQuantity,
               'SaleId': 0,
               'Sku': $scope.soldItem.StoreItemStock.SKU,
               'Rate': rate,
               'QuantitySold': $scope.soldItem.QuantitySold,
               'QuantityDelivered': $scope.soldItem.QuantitySold,
               'QuantityBalance': 0,
               'AmountSold': price,
               'ImagePath': $scope.soldItem.ImagePath,
               'UoMId': $scope.soldItem.StoreItemStock.UoMId,
               'DateSold': ''
           };

           if ($scope.genericSale.Sale.StoreItemSoldObjects.length > 0) {
               var matchFound = false;
               for (var m = 0; m < $scope.genericSale.Sale.StoreItemSoldObjects.length; m++) {
                   var x = $scope.genericSale.Sale.StoreItemSoldObjects[m];
                   if (x.StoreItemStock.StoreItemStockId === soldItem.StoreItemStock.StoreItemStockId) {
                       x.QuantitySold += soldItem.QuantitySold;
                       x.AmountSold += soldItem.AmountSold;
                       matchFound = true;
                   }
               }

               if (!matchFound) {
                   soldItem.TempId = $scope.genericSale.Sale.StoreItemSoldObjects.length + 1;
                   $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItem);
               }
           }
           else {
               soldItem.TempId = 1;
               $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItem);
           }

           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (t, k) {
               totalAmount += t.AmountSold;
           });

           $scope.genericSale.Sale.AmountDue = totalAmount;

           $scope.initializeSoldItem();
       };

       $scope.addTransaction = function (transaction) {
           if ($scope.genericSale.StoreTransactions.length > 0) {
               var matchfound = false;
               angular.forEach($scope.genericSale.StoreTransactions, function (x, y) {
                   if (x.StorePaymentMethod.StorePaymentMethodId === transaction.StorePaymentMethod.StorePaymentMethodId) {
                       x.TransactionAmount = transaction.TransactionAmount;
                       matchfound = true;
                   }
               });

               if (!matchfound) {
                   transaction.TempId = 1;
                   $scope.genericSale.StoreTransactions.push(transaction);
               }
           }
           else {
               transaction.TempId = 1;
               $scope.genericSale.StoreTransactions.push(transaction);

           }

           $scope.initializeTransaction();
       };

       $scope.initializeSoldItem = function ()
       {
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

       $scope.updateamountpaid = function (amountPaid)
       {
           var x = parseFloat(amountPaid);
           if (x === 0)
           {
               $scope.amountPaid = 0;
               $scope.balance = 0;
               return;
           }

           $scope.amountPaid = x;
           $scope.balance = $scope.genericSale.Sale.NetAmount - x;
           console.log('after ' + $scope.genericSale.Sale.NetAmount);
       };

       $scope.validateSoldItem = function (soldItem) {
           if (soldItem.StoreItemStock.StoreItemStockId == undefined || soldItem.StoreItemStock.StoreItemStockId == null || StoreItemStockId.StoreItemStock.StoreItemStockId < 1) {
               alert("ERROR: Please select a Product. ");
               return false;
           }

           if (soldItem.QuantitySold == undefined || soldItem.QuantitySold == null || soldItem.QuantitySold < 1) {
               alert("ERROR: Please provide Item Quantity. ");
               return false;
           }

           if (soldItem.AmountSold == undefined || soldItem.AmountSold == null || soldItem.AmountSold < 1) {
               alert("ERROR: Please Provide Price. ");
               return false;
           }

           if (parseInt(sale.MinimumQuantity) < 1) {
               alert("ERROR: Please Provide Quantity. ");
               return false;
           }
           return true;
       };

       $scope.validateSale = function (genericSale) {
           if (genericSale.Sale.AmountDue == undefined || genericSale.Sale.AmountDue == null || genericSale.Sale.AmountDue < 1) {
               $scope.setError('An unexpected error was encountered. Please review this Transaction details and try again.');
               return false;
           }

           if (genericSale.StoreTransactions == undefined || genericSale.StoreTransactions == null || genericSale.StoreTransactions < 1) {
               $scope.setError('ERROR: Please complete the transaction payment first.');
               return false;
           }

           if ($scope.genericSale.Sale.StoreItemSoldObjects == undefined || $scope.genericSale.Sale.StoreItemSoldObjects == null || $scope.genericSale.Sale.StoreItemSoldObjects < 1) {
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
               if (testResult > 0) {
                   $scope.lessAmount = 'The Net Amount is still short of: ' + 'N' + testResult;
               }

               if (testResult < 0) {
                   $scope.change = testResult;
               }
               $scope.incompleteAmountDue = true;
               return false;
           }

           $scope.incompleteAmountDue = false;
           return true;
       };

       $scope.processSale = function ()
       {

           if (parseInt($scope.genericSale.Sale.paymentOption.StorePaymentMethodId) < 1)
           {
               alert('Please select payment option!');
               return;
           }

           var transaction =
           {
               'StoreTransactionId': '',
               'TempId': '',
               'StoreTransactionTypeId': 1,
               'StorePaymentMethod': '',
               'EffectedByEmployeeId': '',
               'TransactionAmount': '',
               'TransactionDate': '',
               'Remark': '',
               'StoreOutletId': ''
           };

               transaction.TransactionAmount = $scope.cashAmount;
               transaction.StorePaymentMethod = $scope.genericSale.Sale.paymentOption;
               transaction.StorePaymentMethodId = $scope.genericSale.Sale.paymentOption.StorePaymentMethodId;
           
               $scope.addTransaction(transaction);
           

           if (!$scope.validateSale($scope.genericSale))
           {
               return;
           }

           $scope.processing = true;
           
           $scope.genericSale.Sale.EstimateNumber = $scope.estimate.EstimateNumber;
           $scope.genericSale.SaleProcessEstimate = true;
           $scope.genericSale.Sale.SaleProcessEstimate = true;

           var payload =
               {
                    Sale: $scope.genericSale.Sale,
                    StoreTransactions: $scope.genericSale.StoreTransactions
              }

          

           if ($scope.genericSale.Sale.SaleId < 1)
           {
               estimateServices.addSale(payload, $scope.processSaleCompleted);
           }
           else
           {
               estimateServices.editSale(payload, $scope.processSaleCompleted);
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

               if (data.Outlet != null)
               {
                   $scope.outletName = data.Outlet.OutletName;
                   $scope.outletAddress = data.Outlet.Address + ',' + data.Outlet.CityName;
                   $scope.facebook = data.Outlet.FacebookHandle;
                   $scope.twitter = data.Outlet.TwitterHandle;
               }
            
               $scope.rec =
               {
                   referenceCode: data.ReferenceCode,
                   estimateRef: $scope.estimate.EstimateNumber,
                   date: data.Date,
                   customer: $scope.customerInfo,
                   time: data.Time,
                   storeAddress: $rootScope.store.StoreAddress,
                   cashier: $rootScope.user.Name,
                   change: $scope.extraAmount,
                   receiptItems: $scope.genericSale.Sale.StoreItemSoldObjects,
                   amountDue: $scope.genericSale.Sale.AmountDue,
                   subtotal: $scope.presubtotal,
                   amountReceived: $scope.amountPaid,
                   amountToBalance: $scope.balance,
                   paymentChoices: $scope.genericSale.StoreTransactions,
                   netAmount: $scope.genericSale.Sale.NetAmount,
                   discountAmount: $scope.genericSale.Sale.DiscountAmount,
                   vatAmount: $scope.genericSale.Sale.VATAmount
               }; 

               if ($rootScope.store.DeductStockAtSalesPoint === true)
               {
                   //Recalculate the stock quantities of the sold items
                   angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (u, i)
                   {
                       for (var m = 0; m < $scope.items.length; m++)
                       {
                           var x = $scope.items[m];
                           if (x.StoreItemStockId === u.StoreItemStockId)
                           {
                               //Get the difference in stock quantities
                               var difference = x.QuantityInStock - u.QuantitySold;

                               //if the difference is 0 or less than 0, remove it the stock item from the prefetched stock list
                               if (difference === 0 || difference < 0) {
                                   $scope.items.splice(m, 1);
                               }
                               else {
                                   //else, it's Quantity in stock becomes the difference
                                   x.QuantityInStock = difference;
                               }
                           }
                       }
                   });
               }
              
               $scope.printReceipt2();
           }
           
       };

       $scope.printReceipt2 = function ()
       {
           var rec = $scope.rec;

           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 50%; text-align: center; margin-left: 40%"><b style="border-bottom: 1px solid #000;">SALES INVOICE</h3></b></td></tr>' +
               '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date: ' + rec.date + '</td></tr>';
              
           if (rec.referenceCode !== undefined && rec.referenceCode !== null && rec.referenceCode.length > 0)
           {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>';
           }

           if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
           }

           contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer.UserProfileName + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px; margin-right: 3%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
           + '</td></tr></table>';



           contents += '<div class="col-md-12 divlesspadding" style="border-top: #000 solid 1px;">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

      
           angular.forEach(rec.receiptItems, function (item, i) {
               var img = '';
               if (item.StoreItemStockObject.ImagePath !== undefined && item.StoreItemStockObject.ImagePath !== null && item.StoreItemStockObject.ImagePath.length > 0) {
                   img = '<img src="' + item.StoreItemStockObject.ImagePath + '" style="width:50px;height:40px">';
               }
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + img + '&nbsp;' + item.StoreItemStockObject.StoreItemName + '(' + item.UoMCode + ')' + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td>' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
           });

           contents += '</tbody></table></div>' +
               '<br/><table style="width: 100%;font-size:0.9em"><tr><td><h5 style="float:left; text-align:left">Payment method(s):</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Payment details:</h5></td>' +
               '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #000 solid 1px;"><td style="color: #008000;font-size:0.9em;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td><b style="text-align: right">' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
           });

           contents += '</table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td style="text-align: right"><b>' + filterCurrency(rec.amountDue, '') + '</b></td></tr>';

           if (rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right;">' + filterCurrency(rec.discountAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">' + $rootScope.store.VAT + '% VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.vatAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0 || rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.netAmount, '') + '</td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.amountReceived, '') + '</td></tr>';

           if (rec.outStandingPayment > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Outstanding(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.outStandingPayment, '') + '</td></tr>';
           }

           var balance = rec.netAmount - rec.amountReceived;
           if (balance > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(balance, '') + '</td></tr>';
           }

           contents += '</table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                '<h5>Powered by: www.shopkeeper.ng</b></h5></div></div>';

           //angular.element('#receipt').append(contents);

           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null) {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Proforma Invoice</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function () {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                   },

                   500);
               });
           }
           else {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Proforma Invoice</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function () {
                   window.frames["frame1"].focus();
                   window.frames["frame1"].print();
                   frame1.document.body.innerHTML = '';
               }, 500);
           }
           $scope.ngTable.fnClearTable();
           $scope.initializeModel();
           return false;
       };
            
       $scope.printReceipt3 = function () {
        
           var rec = $scope.rec;

           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h4 style="width: 50%; text-align: center; margin-left: 40%"><b style="border-bottom: 4px solid #000;">PROFORMA INVOICE</h3></b></td></tr>' +
               '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date: ' + rec.date + '</td></tr>';

           if (rec.referenceCode !== undefined && rec.referenceCode !== null && rec.referenceCode.length > 0) {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>';
           }

           if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0)
           {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
           }

           contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px; margin-right: 3%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
           + '</td></tr></table>';


           contents += '<div class="col-md-12 divlesspadding">' +
            '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';


           angular.forEach(rec.receiptItems, function (item, i) {
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.9em"><td>' + item.StoreItemName + "(" + item.UoMCode + ")" + '</td><td>' + item.QuantityRequested + '</td><td>' + filterCurrency(item.ItemPrice, '') + '</td>' +
                   '<td style="text-align: right">' + filterCurrency(item.QuantityRequested * item.ItemPrice, " ") + '</td></tr>';
           });

           

           contents += '</tbody></table></div>' +
            '<table style="width: 100%;font-size:0.9em"><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';


           contents += '<table style="width: 100%;font-size:0.9em"><tr><td></td><td><h5 style="float:right; text-align:right; margin-right:1%">Summary:</h5></td>' +
             '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';


           contents += '</table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td>' +
               '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b>' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
               '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr>';

           if (rec.amountReceived > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td><b style="text-align: right">' + filterCurrency(rec.amountReceived, '') + '</b></td></tr>';
           }

           if (rec.amountToBalance > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b style="text-align: right">' + filterCurrency(rec.amountToBalance, '') + '</b></td></tr>';
           }

           contents += '</table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
           '<h5>Served by: <b>' + rec.cashier + '</b></h5></div></div>';


           //angular.element('#receipt').append(contents);

           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null) {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Proforma Invoice</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function () {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                   },

                   500);
               });
           }
           else {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Proforma Invoice</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function () {
                   window.frames["frame1"].focus();
                   window.frames["frame1"].print();
                   frame1.document.body.innerHTML = '';
               }, 500);
           }
           $scope.initializeModel();
           return false;
       };
    }]);
   
});

