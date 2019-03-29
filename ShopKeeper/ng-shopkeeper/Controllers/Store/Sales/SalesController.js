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

    app.register.directive('ngInvoice', function ($compile)
    {
        return function ($scope, ngInvoice)
        {
            var tableOptions = {};
            if ($scope.isAdmin === true)
            {
                tableOptions.sourceUrl = "/Sales/GetAdminSalesReport";
            }
            else
            {
                if ($scope.isCashier === true)
                {
                    tableOptions.sourceUrl = "/Sales/GetMySalesReport";
                }
                else
                {
                    tableOptions.sourceUrl = "/Sales/GetContactPersonInvoices";
                    
                }
                
            }
            
            tableOptions.itemId = 'SaleId';
            tableOptions.columnHeaders = ["InvoiceNumber", "CustomerName", 'DateStr', "AmountDueStr", "NetAmountStr", "PaymentStatus"];
            var ttc = invoiceTableManager($scope, $compile, ngInvoice, tableOptions, 'New Invoice', 'newInvoice', 'getInvoiceDetails', 115);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };
    });
    
    app.register.controller('saleController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'saleServices', '$filter', '$locale',
        function (ngDialog, $scope, $rootScope, $routeParams, saleServices, $filter, $locale)
        {
            $scope.isAdmin = $rootScope.isAdmin;
            $scope.isCashier = $rootScope.isCashier;
            
            var xcvb = new Date();
            var year = xcvb.getFullYear();
            var month = xcvb.getMonth() + 1;
            var day = xcvb.getDate();
            var maxDate = year + '/' + month + '/' + day;

        setControlDate($scope, '', maxDate);

        $scope.goBack = function ()
        {
            $scope.newTransaction = false;
            $scope.details = false;
        };
       
        $scope.initializeController = function ()
        {
            $scope.newTransaction = false;

            $scope.details = false;

            $scope.printDNote = false;
            $scope.setPrintX = 4;
            $scope.setPrintOption = 2;
            
            $scope.genders = [{ genderId: 1, name: 'Male' }, { genderId: 2, name: 'Female' }, , { genderId: 3, name: 'Corporate' }];
            $scope.getProducts();
        };

        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        $scope.newInvoice = function ()
        {
            $scope.delAuth = { email: '', password: '' };
            $scope.search = { skuName: '' };
            $scope.initializeModel();
            $scope.details = false;
            $scope.newTransaction = true;
        };

        $scope.getInvoiceDetails = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            saleServices.getInvoice(id, $scope.getInvoiceDetailsCompleted);
        };

        $scope.getInvoiceDetailsCompleted = function (rec)
        {
            if (rec.SaleId < 1)
            {
                alert("Report information could not be retrieved");
                return;
            }

            $scope.sale = rec;

            var reportHtml =
                '<table style="width: 100%; margin-left: 20px; border: none"> <tr style="border: none;"><td style="width: 40%; border: none"></td>' +
                    '<td style="width: 30%; border: none"><h3 class="ng-binding" style="width: 100%; text-align: center">' + $rootScope.store.StoreName + '</h3>' +
                    '</td><td></td></tr><tr style="border: none"><td style="width: 40%; border: none; font-size: 0.9em">' +
                    '<table style="border: none; width: 100%; vertical-align: top"><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Address: ' +
                    $rootScope.store.StoreAddress + '</td></tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Website: <b></b></td>' +
                    '</tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Email: <b>' + $rootScope.store.StoreEmail + '</b>' +
                    '</td></tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Phone: <b>' + $rootScope.store.PhoneNumber + '</b></td>' +
                    '</tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Date: <b>' + rec.DateStr + '</b></td>';

                    if (rec.EstimateNumber !== undefined && rec.EstimateNumber !== null && rec.EstimateNumber.length > 0)
                    {
                        reportHtml += '<tr><td style="width: 40%;font-size:1em"><h5>Estimate Ref: <b>' + rec.EstimateNumber + '</b></h5>' +
                            '</td><td style="width: 30%"></td><td></td></tr>';
                    }

                    reportHtml += '<tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Invoice No.: <b>' + rec.InvoiceNumber + '</b></td>' +
                '</tr></table></td><td style="width: 30%; border: none"></td><td style ="border: none"><img style="height: 60px; vertical-align: top; float: right; margin-right:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '">' +
                '</td></tr>';
            
                    reportHtml += '<tr style="border: none"><td style="width: 40%;font-size:1em" >' +
                    'Customer : <strong>' + rec.CustomerName + '</strong></td><td style="width: 30%"></td><td></td></tr></table><br>';

            reportHtml += '<div class="col-md-12">' +
             '<table class="table" role="grid" style="width: 100%;font-size:1em">' +
                '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:1em"><th style="color: green;font-size:1em; width:35%">Item</th>' +
                 '<th style="color: green;font-size:1em; width:20%;">Qty</th><th style="color: green;font-size:1em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                  '<th style="color: green;font-size:1em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

            angular.forEach(rec.StoreItemSoldObjects, function (item, i)
            {
                item.StoreItemStockObject = { ImagePath: item.ImagePath, StoreItemName: item.StoreItemName };
                item.QuantityDelivered = item.QuantitySold;
               
                reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '(' + item.UoMCode + ')' + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td>' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            });

            reportHtml += '</tbody></table></div>' +
                '<table style="width: 100%;font-size:1em"><tr><td><h5 style="padding: 8px;">Payment method(s):</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Payment details:</h5></td>' +
                '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:1em; float:left">';

            angular.forEach(rec.Transactions, function (l, i)
            {
                l.StorePaymentMethod = l.PaymentMethodName;
                l.StorePaymentMethod = { Name: l.PaymentMethodName };
                reportHtml += '<tr style="border-top: #ddd solid 1px;"><td style="color: green;font-size:1em;">' + l.PaymentMethodName + ':</td>' +
                             '<td><b >' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
            });

            reportHtml += '<tr style="border-top: #ddd solid 1px;"><td style="color: green;font-size:1em;"></td>' +
                             '<td><b style="position: absolute;opacity: 0.37;font-size: 2em;margin-top: 6%;margin-left: 25%;text-align: center;z-index: 1000;;transform:rotate(300deg);">REPRINTED COPY</b></td></tr>';
            

            reportHtml += '</table></td><td>' +
                 '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:1em;"><tr style="border-top: #ddd solid 1px;"><td style="color: green;font-size:1em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                         '<td style="text-align: right"><b>' + filterCurrency(rec.AmountDue, '') + '</b></td>' +
                     '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right"><b>' + filterCurrency(rec.DiscountAmount, '') + '</b></td>' +
                     '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td  style="text-align: right"><b>' + filterCurrency(rec.VATAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                       '<td  style="text-align: right"><b>' + filterCurrency(rec.NetAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                         '<td  style="text-align: right"><b>' + filterCurrency(rec.AmountPaid, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td  style="text-align: right"><b>' + filterCurrency(rec.Balance, '') + '</b></td></tr></table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:1.06em">' +
                '<h5>Served by: <b>' + rec.SaleEmployeeName + '</b></h5></div></div><br>';

            angular.element('#details').html('').append(reportHtml); 

            $scope.details = true;

            $scope.rec =
              {
                  referenceCode: rec.InvoiceNumber,
                  date: rec.DateStr,
                  customer: rec.CustomerName,
                  time: '',
                  storeAddress: $rootScope.store.StoreAddress,
                  cashier: rec.SaleEmployeeName,
                  change: 0,
                  receiptItems: rec.StoreItemSoldObjects,
                  amountDue: rec.AmountDue,
                  subtotal: 0,
                  waterMark: '<b style="position: absolute;opacity: 0.47;font-size: 2em;margin-top: 6%;margin-left: 25%;text-align: center;z-index: 1000;transform:rotate(300deg);">REPRINTED COPY</b>',
                  estimateNumber: rec.EstimateNumber,
                  amountReceived: rec.AmountPaid,
                  amountToBalance: rec.Balance,
                  paymentChoices: rec.Transactions,
                  netAmount: rec.NetAmount,
                  discountAmount: rec.DiscountAmount,
                  vatAmount: rec.VATAmount
              };

          $scope.refundExists = false;
          angular.element('#refunds').html('');
          saleServices.getSaleRefundNotes(rec.SaleId, $scope.getSaleRefundNotesCompleted);
        };

        $scope.getSaleRefundNotesCompleted = function (rec)
        {
            if (rec.length < 1)
            {
                return;
            }
            
            $scope.saleRefundNotes = rec;
            $scope.refundExists = true;
        };

        $scope.getRefundNoteDetailsCompleted = function (rec)
        {
            if (rec.length < 1)
            {
                return;
            }

            $scope.detailRefundNotes = rec;
        };

        $scope.preparePrint = function ()
        {
            var contents = angular.element('#details').html();
            var html = '<div class="row"><div class="col-md-12"><h5 style=";font-size:1em">Sales Invoice</h4></div></div>' + contents.replace('font-size:1em', 'font-size:0.8em').replace('font-size: 1em', 'font-size:0.8em');
            var popupWin = '';
            if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
            {
                popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
                popupWin.window.focus();
                popupWin.document.write('<!DOCTYPE html><html><head>' +
                    '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                    '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + html + '</div></div></html>');
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
                popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + html + '</div></div></html>');
                popupWin.document.close();
            }

            popupWin.document.close();
            return false;
        };

        $scope.initializeModel = function ()
        {
            $scope.selected = undefined;
            
            $scope.cash = true;
            $scope.genericSale =
            {
                'Sale': { 'SaleId': 0, 'RegisterId': 0, 'CustomerId': null, 'EmployeeId': 0, 'AmountDue': 0, 'Status': '', 'Date': '', Discount: '', VATAmount: 0, DiscountAmount: '', NetAmount: 0, applyVat: false, VAT: 0, EstimateNumber: '', ProcessEstimate: false, StoreItemSoldObjects : [] },
                'StoreTransactions': [],
                'paymentOption': { 'StorePaymentMethodId': '', 'Name': '-- select payment option --' }
            };

            
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

            
            $scope.customer = null;
            $scope.customerInfo = { 'CustomerId': '', 'UserProfileName': '' };
            $scope.customer = { 'CustomerId': '', 'UserProfileName': '' };
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
            if (opt.StorePaymentMethodId < 1) {
                return;
            }

            $scope.ammountPaid = '';
            $scope.posAmmount = '';
            $scope.cashAmmount = '';
            $scope.genericSale.Sale.paymentOption = opt;
            $scope.cashAmount = $scope.genericSale.Sale.NetAmount;
            $scope.updateamountpaid($scope.cashAmount);
        };

        $scope.getProducts = function ()
        {
            $scope.items = [];
            $scope.page = 0;
            $scope.itemsPerPage = 50;
            $scope.initializeSoldItem();
            $scope.soldItem.StoreItemStockObject = { 'StoreItemStockId': '', 'StoreItemName': '-- Select Product --' };
            saleServices.getGenericList($scope.getGenericListCompleted);
            //saleServices.getAllProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
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

            d.ItemPriceObjects.sort(function (a, b)
            {
                return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
            });
            
            //var minQty = 1;
            //var minPrice = d.ItemPriceObjects[0];

            var minQty = '';
            var minPrice = d.ItemPriceObjects[0];

            //check if this item is still available by evaluating it's available 
            //quantity in relation to the minimum quantity in it's price configuration

            //if (minQty > d.QuantityInStock)
            //{
            //    minQty = d.QuantityInStock;
            //}

            //check if this item has been selected before
            var secondResults = $scope.genericSale.Sale.StoreItemSoldObjects.filter(function (s)
            {
                return (s.StoreItemStockId === d.StoreItemStockId);
            });

            //if already selected, increment the quantity to be sold, then recalculate the total price
            if (secondResults.length > 0) {
                secondResults[0].QuantitySold += minQty;
                $scope.updateAmount(secondResults[0]);
                return;
            } else {
                //if not selected before, add it to the basket
                var tempId = $scope.genericSale.Sale.StoreItemSoldObjects.length + 1;


                var soldItemX =
                {
                    'StoreItemSoldId': '',
                    'TempId': tempId,
                    'GotFromSKU': true,
                    'StoreItemStockId': d.StoreItemStockId,
                    'StoreItemStockObject': d,
                    'ReOrderQuantityStr': d.ReOrderQuantityStr,
                    'ReOrderLevelStr': d.ReOrderLevelStr,
                    'ReorderLevel': d.ReorderLevel,
                    'ReorderQuantity': d.ReorderQuantity,
                    'SaleId': 0,
                    'Sku': d.SKU,
                    'Rate': minPrice.Price,
                    'QuantitySold': minQty,
                    'QuantityBalance': 0,
                    //'AmountSold': minPrice.Price,
                    'AmountSold': 0,
                    'StoreItemName': d.StoreItemName,
                    'ImagePath': d.ImagePath,
                    'UoMId': minPrice.UoMId,
                    'UoMCode': d.ItemPriceObjects[0].UoMCode,
                    'DateSold': '',
                    'QuantityDelivered': minQty
                };

                $scope.clearError();
                $rootScope.skuControl = document.getElementById('stockControl_value');
                $rootScope.skuControl.focus();
                $rootScope.searchStr = "";

                $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItemX);
                //$scope.updateAmount(soldItemX);
            }
           
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
           
           var secondResults = $scope.genericSale.Sale.StoreItemSoldObjects.filter(function (s)
           {
               return (s.SKU === i);
           });

           if (secondResults.length > 0)
           {
               return;
           }

           var d = results[0];

           var tempId = $scope.genericSale.Sale.StoreItemSoldObjects.length + 1;

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
           $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItem);
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
           if ($scope.soldItem.StoreItemStockObject.StoreItemStockId < 1)
           {
               $scope.setError('Please select a product');
               return;
           }

           if ($scope.soldItem.StoreItemStockObject.PurchaseOrderItemId < 1)
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
               'StoreItemStockId': $scope.soldItem.StoreItemStockObject.StoreItemStockId,
               'StoreItemStockObject': $scope.soldItem.StoreItemStockObject,
               'ReOrderQuantityStr': $scope.soldItem.StoreItemStockObject.ReOrderQuantityStr,
               'ReOrderLevelStr': $scope.soldItem.StoreItemStockObject.ReOrderLevelStr,
               'ReorderLevel': $scope.soldItem.StoreItemStockObject.ReorderLevel,
               'ReorderQuantity': $scope.soldItem.StoreItemStockObject.ReorderQuantity,
               'SaleId': 0,
               'Sku': $scope.soldItem.StoreItemStockObject.SKU,
               'Rate' : rate,
               'QuantitySold': $scope.soldItem.QuantitySold,
               'QuantityBalance': 0,
               'AmountSold': price,
               'StoreItemName': $scope.soldItem.StoreItemStockObject.StoreItemName,
               'ImagePath': $scope.soldItem.ImagePath,
               'UoMId': $scope.soldItem.StoreItemStockObject.UoMId,
               'DateSold': '',
               'QuantityDelivered': $scope.soldItem.QuantitySold
           };
           
           if ($scope.genericSale.Sale.StoreItemSoldObjects.length > 0)
           {
               var matchFound = false;
               for (var m = 0; m < $scope.genericSale.Sale.StoreItemSoldObjects.length; m++)
               {
                   var x = $scope.genericSale.Sale.StoreItemSoldObjects[m];
                   if (x.StoreItemStock.StoreItemStockId === soldItem.StoreItemStockObject.StoreItemStockId)
                   {
                       x.QuantitySold += soldItem.QuantitySold;
                        x.AmountSold += soldItem.AmountSold;
                        matchFound = true;
                   }
               }
               
               if (!matchFound)
               {
                 soldItem.TempId = $scope.genericSale.Sale.StoreItemSoldObjects.length + 1;
                 $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItem);
              }
           }
           else
           {
               soldItem.TempId = 1;
               $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItem);
           }
           
           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (t, k)
           {
               totalAmount += t.AmountSold;
           });
           
           $scope.genericSale.Sale.AmountDue = totalAmount;
           
           $scope.initializeSoldItem();
       };
        
      //Get price of items prices with minimum quantity matching or nearest the sold item quantity 
       $scope.getPrice = function(item)
       {
           var priceResult = {'rate' : 0, 'price' : 0, 'UoMId' : 0};
          
           var x = item.StoreItemStockObject.ItemPriceObjects;

           if (x.length < 1)
           {
               alert('not found!');
                $scope.setError('A fatal error was encountered. The requested operation was aborted');
                return priceResult;
           }

           if (x.length === 1)
           {
               priceResult.rate = x[0].Price;
               priceResult.price = x[0].Price * item.QuantitySold;
               priceResult.UoMId = x[0].UoMId;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) 
               {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }
           
           var result = x.filter(function (u)
           {
               return u.MinimumQuantity === item.QuantitySold;
           });

           if (result.length > 0)
           {

               priceResult.rate = result[0].Price;
               priceResult.price = result[0].Price * item.QuantitySold;
               priceResult.UoMId = x[0].UoMId;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }
           
           if (item.QuantitySold >= x[x.length - 1].MinimumQuantity)
           {
               priceResult.rate = x[x.length - 1].Price;
               priceResult.price = x[x.length - 1].Price * item.QuantitySold;
               priceResult.UoMId = x[x.length - 1].UoMId;
               if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                   priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
               }
               return priceResult;
           }

           else
           {
               for (var i = 0; i < x.length; i++)
               {
                   if (item.QuantitySold <= x[i].MinimumQuantity)
                   {
                       priceResult.rate = x[i].Price;
                       priceResult.price = x[i].Price * item.QuantitySold;
                       priceResult.UoMId = x[i].UoMId;
                       if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                           priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                       }
                       return priceResult;
                   }
                   else
                   {
                       if (item.QuantitySold > x[i].MinimumQuantity && item.QuantitySold <= x[i + 1].MinimumQuantity)
                       {
                           priceResult.rate = x[i + 1].Price;
                           priceResult.price = x[i + 1].Price * item.QuantitySold;
                           priceResult.UoMId = x[i + 1].UoMId;

                           if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1)
                           {
                               priceResult.price = Math.ceil(priceResult.price / 100) * 100;
                           }
                           return priceResult;
                       }
                   }
                   
               }
               return priceResult;
           }
           
       }
            
       $scope.checkQuantity = function (soldItem)
       {
           var test = parseFloat(soldItem.QuantitySold);

           if (isNaN(test) || test < 1)
           {
               var confirmDelete = 'Do you want to remove ' + soldItem.StoreItemStockObject.StoreItemName + ' from the list?';
               if (!confirm(confirmDelete))
               {
                   soldItem.QuantitySold = 1;
                   $scope.updateAmount(soldItem);
                   return;
               }

               $scope.confirmDelete(soldItem.TempId);
               //$scope.removeSoldItem2(soldItem.TempId);
               return;
           }
       };

       $scope.updateamountpaid = function (amountPaid)
       {
           if (amountPaid < 1)
           {
               $scope.amountPaid = 0;
              return;
           }

           $scope.amountPaid = amountPaid;
           $scope.balance = $scope.genericSale.Sale.NetAmount - amountPaid;
           if ($scope.genericSale.Sale.paymentOption !== undefined && $scope.genericSale.Sale.paymentOption !== null && $scope.genericSale.Sale.paymentOption.StorePaymentMethodId > 0)
           {
               $scope.cashAmount = $scope.genericSale.Sale.NetAmount;
               
           }
       };
        
       $scope.updateAmount = function (soldItem)
       {
           if (soldItem == null || soldItem.StoreItemStockObject.StoreItemStockId < 1)
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
               
            }

            var d = soldItem;
       
            var ssz = {};

            //if (d.StoreItemStockObject.StoreItemCategoryId !== 28)
           //{

            if (d.QuantitySold > d.StoreItemStockObject.QuantityInStock)
            {
                $scope.setError('Only ' + d.StoreItemStockObject.QuantityInStock + d.StoreItemStockObject.ItemPriceObjects[0].UoMCode + ' of this item is available.');
                d.QuantitySold = d.StoreItemStockObject.QuantityInStock;
            }

            ssz = $scope.getPrice(soldItem);

            if (ssz.rate < 1)
            {
                $scope.setError('An error was encountered. Please try again later.');
                return;
            }

            soldItem.AmountSold = ssz.price;
            soldItem.Rate = ssz.rate;
                
            //} 
            //else
            //{
            //    soldItem.AmountSold = soldItem.QuantitySold * soldItem.Rate;
            //}
            
            
           var totalAmount = 0;
           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (t, k)
           {
               totalAmount += t.AmountSold;
           });

           $scope.balance = 0;
           $scope.amountPaid = 0;
           $scope.cashAmmount = '';

           $scope.genericSale.Sale.AmountDue = totalAmount;
           $scope.genericSale.Sale.NetAmount = totalAmount;
           $scope.updateAmountForDiscount();
            
       };

       $scope.updateServiceamount = function (service)
       {
           var test1 = parseFloat(service.QuantitySold);
           var test2 = parseFloat(service.Rate);

           if (test1 === undefined || test1 === null || test1 === NaN || test1 < 1 || test2 === undefined || test2 === null || test2 === NaN || test2 < 1) {
               return;
           }

           service.AmountSold = test1 * test2;

       }

       $scope.AddService = function (service)
       {
           var test1 = parseFloat(service.QuantitySold);
           var test2 = parseFloat(service.Rate);
           var test3 = parseFloat(service.AmountSold);

           if (test1 === undefined || test1 === null || test1 === NaN || test1 < 1 || test2 === undefined || test2 === null || test2 === NaN || test2 < 1) {
               return;
           }

           if (test3 === undefined || test3 === null || test3 === NaN || test3 < 1)
           {
               alert('An error was encountered. The process was aborted.');
               return;
           }

           //todo: make to tally with live id
           service.UoMId = 4;

           var d = service;

           ngDialog.close('/ng-shopkeeper/Views/Store/RefundNote/verifyAdmin.html', '');
        
           var totalAmount = 0;

           var refItems = $scope.genericSale.Sale.StoreItemSoldObjects.filter(function (s)
           {
               return (s.StoreItemStockId === d.StoreItemStockId);
           });

           if (refItems.length < 1)
           {
               var tempId = $scope.genericSale.Sale.StoreItemSoldObjects.length + 1;

               var soldItemX =
               {
                   'StoreItemSoldId': '',
                   'TempId': tempId,
                   'GotFromSKU': true,
                   'StoreItemStockId': d.StoreItemStockId,
                   'StoreItemStockObject': d,
                   'ReOrderQuantityStr': d.ReOrderQuantityStr,
                   'ReOrderLevelStr': d.ReOrderLevelStr,
                   'ReorderLevel': d.ReorderLevel,
                   'ReorderQuantity': d.ReorderQuantity,
                   'SaleId': 0,
                   'Sku': d.SKU,
                   'Rate': service.Rate,
                   'QuantitySold': service.QuantitySold,
                   'QuantityBalance': 0,
                   'AmountSold': service.AmountSold,
                   'StoreItemName': d.StoreItemName,
                   'ImagePath': d.ImagePath,
                   'UoMId': d.UoMId,
                   'UoMCode': ' ',
                   'DateSold': '',
                   'QuantityDelivered': service.QuantitySold
               };

               $scope.genericSale.Sale.StoreItemSoldObjects.push(soldItemX);
               //console.log(JSON.stringify(soldItemX));
           }
           else
           {
               refItems[0].QuantitySold += d.QuantitySold;
               refItems[0].Rate += d.Rate;
               refItems[0].AmountSold += d.AmountSold;
           }
          
           $scope.clearError();
           
        
           $rootScope.skuControl = document.getElementById('stockControl_value');
           $rootScope.skuControl.focus();
           $rootScope.searchStr = "";

           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (t, k)
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

       $scope.verifyUserCompleted = function (data)
       {
           if (data.Code < 1) {
               alert(data.UserName);
               return;
           }

           ngDialog.close('/ng-shopkeeper/Views/Store/RefundNote/verifyAdmin.html', '');
           $scope.processRefundNote();
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
                   discountAmount = (disc * $scope.genericSale.Sale.AmountDue) / 100;
                   $scope.discountHolder = disc;
                   $scope.genericSale.Sale.DiscountAmount = discountAmount;
                   var amountLessDiscount = $scope.genericSale.Sale.AmountDue - discountAmount;
                   $scope.genericSale.Sale.NetAmount = amountLessDiscount;
               }
               else
               {
                   if ($scope.discountHolder > disc) {
                       var xrem = $scope.discountHolder - disc;

                       discountAmount = (disc * $scope.genericSale.Sale.AmountDue) / 100;
                       $scope.genericSale.Sale.DiscountAmount = discountAmount;
                       var reconciliator = (xrem * $scope.genericSale.Sale.AmountDue) / 100;
                       $scope.genericSale.Sale.NetAmount = (($scope.genericSale.Sale.NetAmount + reconciliator) - discountAmount).toFixed(2);

                       $scope.discountHolder = disc;
                   }
                   else
                   {
                       if ($scope.discountHolder === disc)
                       {
                           discountAmount = (disc * $scope.genericSale.Sale.AmountDue) / 100;
                           $scope.discountHolder = disc;
                           $scope.genericSale.Sale.DiscountAmount = discountAmount;
                           $scope.genericSale.Sale.NetAmount = $scope.genericSale.Sale.AmountDue - discountAmount;
                           
                       }
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
             
           }

           if ($scope.genericSale.Sale.applyVat === true)
           {
               $scope.genericSale.Sale.VAT = $rootScope.store.VAT;
               var vatAmount = ($scope.genericSale.Sale.NetAmount * $rootScope.store.VAT) / 100;
               $scope.genericSale.Sale.VATAmount = vatAmount;
               $scope.genericSale.Sale.NetAmount = ($scope.genericSale.Sale.NetAmount + vatAmount).toFixed(2);
           }

           if ($scope.genericSale.Sale.paymentOption !== undefined && $scope.genericSale.Sale.paymentOption !== null && $scope.genericSale.Sale.paymentOption.StorePaymentMethodId > 0)
           {
               $scope.cashAmount = $scope.genericSale.Sale.NetAmount;
               $scope.updateamountpaid($scope.cashAmount);
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
           saleServices.getCustomerTypes($scope.getCustomerTypesCompleted);

           //if (data.Customers.length > 0)
           //{
           //    $scope.pNum = 1;
           //    $scope.customers = data.Customers;
           //    //saleServices.getMoreCustomers($scope.pNum, 200, $scope.getMoreCustomersCompleted);
           //}
           
           $scope.paymentMethods = data.PaymentMethods;
       };

       $scope.getMoreCustomersCompleted = function (customers)
       {
           saleServices.getCustomerTypes($scope.getCustomerTypesCompleted);

           if (customers.length > 0)
           {
               $scope.pNum += 1;
               saleServices.getMoreCustomers($scope.pNum, 300, $scope.getMoreCustomersCompleted);
               angular.forEach(customers, function (c)
               {
                   $scope.customers.push(c);
               });
           }
           $scope.paymentMethods = data.PaymentMethods;
       };

       $scope.getCustomerTypesCompleted = function (data)
       {
           
           $scope.customerTypes = data;
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

           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (item, index)
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

           $scope.delAuth = { email: '', password: '' };
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Sales/verifyAdmin.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
           
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
           ngDialog.close('/ng-shopkeeper/Views/Store/Sales/verifyAdmin.html', '');
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
           
            angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (x, y)
            {
                if (x.TempId === id)
                {
                    $scope.genericSale.Sale.StoreItemSoldObjects.splice(y, 1);
                    $scope.search.skuName = '';
                }
            });
            
            var totalAmount = 0;
            angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (y, i)
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

       $scope.removeSoldItem2 = function (id)
       {
           if (id < 1)
           {
               $scope.setError('Invalid selection');
               return;
           }

           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (x, y)
           {
               if (x.TempId === id)
               {
                  $scope.genericSale.Sale.StoreItemSoldObjects.splice(y, 1);
               }
           });

           var totalAmount = 0;
           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (y, i)
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

       $scope.validateSoldItem = function (soldItem)
       {
           if (soldItem.StoreItemStockObject.StoreItemStockId == undefined || soldItem.StoreItemStockObject.StoreItemStockId == null || StoreItemStockId.StoreItemStock.StoreItemStockId < 1)
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
           
           if (genericSale.Sale.StoreItemSoldObjects == undefined || genericSale.Sale.StoreItemSoldObjects == null || genericSale.Sale.StoreItemSoldObjects < 1)
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
        
       $scope.initializeCustomer = function ()
       {
           $scope.CustomerObject =
             {
                 CustomerId: 0,
                 ReferredByCustomerId: null,
                 StoreOutletId: '',
                 UserId: 0,
                 FirstPurchaseDate: null,
                 newCustomer: false,
                 LastName: '',
                 OtherNames: '',
                 UserProfileName: '',
                 Gender: '',
                 GenderObject: { genderId: '', name: '-- select --' },
                 BirthDay: null,
                 ContactPersonId: ''
             }
           
       };

       $scope.addNewCustomer = function ()
       {
          
           $scope.initializeCustomer();
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Sales/ProcessCustomers.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processCustomer = function () {
           if ($scope.CustomerObject.MobileNumber == null || $scope.CustomerObject.MobileNumber === undefined || $scope.CustomerObject.MobileNumber.length < 1) {
               alert("ERROR: Please provide customer's Mobile phone number.");
               return;
           }

           if ($scope.CustomerObject.OtherNames == null || $scope.CustomerObject.OtherNames === undefined || $scope.CustomerObject.OtherNames.length < 1) {
               alert("ERROR: Please provide Customer's Other names.");
               return;
           }
           if ($scope.CustomerObject.LastName == null || $scope.CustomerObject.LastName === undefined || $scope.CustomerObject.LastName.length < 1) {
               alert("ERROR: Please Customer's Last Name.");
               return;
           }

           if ($scope.CustomerObject.customerType == null || $scope.CustomerObject.customerType === undefined || $scope.CustomerObject.customerType.StoreCustomerTypeId < 1) {
               alert("ERROR: Please select Customer Type.");
               return;
           }

           if ($scope.CustomerObject.GenderObject == null || $scope.CustomerObject.GenderObject === undefined || $scope.CustomerObject.GenderObject.genderId < 1) {
               alert("ERROR: Please select a gender.");
               return;
           }

           $scope.CustomerObject.Gender = $scope.CustomerObject.GenderObject.name;
           $scope.CustomerObject.StoreCustomerTypeId = $scope.CustomerObject.customerType.StoreCustomerTypeId;
           $scope.CustomerObject.Birthday = $scope.CustomerObject.BirthdayStr;

           $scope.CustomerObject.CreditWorthy = $scope.CustomerObject.customerType.CreditWorthy;
           $scope.CustomerObject.CreditLimit = $scope.CustomerObject.customerType.CreditLimit;

           $scope.customerInfo = $scope.CustomerObject;
           $scope.customerInfo.UserProfileName = $scope.CustomerObject.LastName + " " + $scope.CustomerObject.OtherNames;
           $scope.customerDetail = $scope.customerInfo;
           $scope.customer = $scope.customerInfo;

           $scope.genericSale.Sale.CustomerName = $scope.CustomerObject.LastName + " " + $scope.CustomerObject.OtherNames + ' (' + $scope.CustomerObject.MobileNumber + ')';

           var customer =
            {
                CustomerId: 0,
                ReferredByCustomerId: null,
                StoreOutletId: '',
                MobileNumber: $scope.CustomerObject.MobileNumber,
                ContactEmail: $scope.CustomerObject.ContactEmail,
                UserId: 0,
                FirstPurchaseDate: null,
                newCustomer: true,
                LastName: $scope.CustomerObject.LastName,
                OtherNames: $scope.CustomerObject.OtherNames,
                Gender: $scope.CustomerObject.GenderObject.name,
                BirthDay: $scope.CustomerObject.BirthdayStr,
                ContactPersonId: '',
                StoreCustomerTypeId: $scope.CustomerObject.customerType.StoreCustomerTypeId
            }
           saleServices.processCustomer(customer, $scope.processCustomerCompleted);

       };

       $scope.processCustomerCompleted = function (user)
       {
           if (user.Code < 1)
           {
               alert(user.Error);
               return;
           }
           
           $scope.genericSale.Sale.CustomerId = user.Code;
           $scope.customerInfo.CustomerId = user.Code;
           $scope.customerDetail.CustomerId = user.Code;
           $scope.customer.CustomerId = user.Code;

           $scope.setSuccessFeedback('Customer was successfully processed');
           ngDialog.close('/ng-shopkeeper/Views/Store/Sales/ProcessCustomers.html', '');
       };

       $scope.setCustomer = function (user)
       {
           if (user == null || user.CustomerId < 1)
           {
               return;
           }

           if ($scope.CustomerObject !== undefined && $scope.CustomerObject !== null && $scope.CustomerObject.LastName.length > 0 && $scope.CustomerObject.OtherNames.length > 0) {
               if (!confirm('Do you want to replace the original customer with this new one?')) {
                   return;
               }
               $scope.estimate.CustomerId = user.CustomerId;
               $scope.genericSale.Sale.CustomerId = user.CustomerId;
           }

           if ($scope.estimate.CustomerId > 0 && $scope.estimate.CustomerId !== user.CustomerId) {
               if (!confirm('Do you want to replace the original customer with this new one?')) {
                   return;
               }
               $scope.estimate.CustomerId = user.CustomerId;
               $scope.genericSale.Sale.CustomerId = user.CustomerId;
           }

           if ($scope.estimate.CustomerId === undefined || $scope.estimate.CustomerId === null || $scope.estimate.CustomerId < 1) {
               $scope.estimate.CustomerId = user.CustomerId;
               $scope.genericSale.Sale.CustomerId = user.CustomerId;
           }
           $scope.customerInfo = user;

       };
            
       $rootScope.setCustomer = function (val)
       {
           if (val == null) {
               return;
           }

           var d = val.originalObject;
           if (d.CustomerId < 1) {
               $scope.setError('Customer information could not be retrieved. Please try again later.');
               return;
           }

           $scope.customerInfo = d;
           $scope.customerDetail = d;
           $scope.customer = d;
           $scope.genericSale.Sale.CustomerId = d.CustomerId;
           $scope.genericSale.Sale.CustomerName = d.UserProfileName;
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
       
       $scope.processSale = function ()
       {
           if ($scope.genericSale.Sale.paymentOption  === undefined || $scope.genericSale.Sale.paymentOption === null  || parseInt($scope.genericSale.Sale.paymentOption.StorePaymentMethodId) < 1)
           {
               alert('Please select payment option!');
               return;
           }

           var isErr = 0;

           angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (t, k) 
           {
               var tQty = parseFloat(t.QuantitySold);

               if (isNaN(tQty) || tQty < 1)
               {
                   isErr += 1;
               }
           });

           if (isErr > 0)
           {
               alert('Please review the selected products and try again');
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
              'Remark': $scope.transaction.Remark,
              'StoreOutletId': ''
          };

          if ($scope.customerDetail && $scope.customerDetail.CustomerId > 0)
          {
              if ($scope.customerDetail.CreditWorthy === true)  
              {
                  if ($scope.customerDetail.InvoiceBalance > $scope.customerDetail.CreditLimit)
                  {
                      $scope.setError('This customer has exceeded the permissible credit limit');
                      return;
                  }

                  var cumulativeCredit = $scope.customerDetail.InvoiceBalance + $scope.customerDetail.CreditLimit;

                  if ($scope.balance > cumulativeCredit)
                  {
                      $scope.setError('The transaction balance is more than the permissible credit limit for this customer');
                      return;
                  }
              }
              else
              {
                  if ($scope.balance > 0)
                  {
                      $scope.setError('Please reconcile the Amount due with Amount paid.');
                      return;
                  }
              }
          }
          else
          {
              if ($scope.balance > 0)
              {
                  $scope.setError('Please reconcile the Amount due with Amount paid.');
                  return;
              }
          }
           

          $scope.genericSale.StoreTransactions = [];
          transaction.TransactionAmount = $scope.cashAmount;
          transaction.StorePaymentMethod = $scope.genericSale.Sale.paymentOption;
          transaction.StorePaymentMethodId = $scope.genericSale.Sale.paymentOption.StorePaymentMethodId;
   
          $scope.genericSale.StoreTransactions.push(transaction);
          
           
            if (!$scope.validateSale($scope.genericSale))
            {
                return;
            }

            $scope.processing = true;
           
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
               var oustanding = $scope.genericSale.Sale.NetAmount - $scope.amountPaid;

               $scope.rec =
               {
                   referenceCode: data.ReferenceCode,
                   date: data.Date,
                   customer: $scope.genericSale.Sale.CustomerName,
                   time: data.Time,
                   storeAddress : $rootScope.store.StoreAddress,
                   cashier: $rootScope.user.Name,
                   change: $scope.extraAmount,
                   estimateNumber: '',
                   waterMark : '',
                   outStandingPayment : oustanding,
                   receiptItems: $scope.genericSale.Sale.StoreItemSoldObjects,
                   amountDue: $scope.genericSale.Sale.AmountDue,
                   amountReceived: $scope.amountPaid,
                   amountToBalance: $scope.balance,
                   paymentChoices: $scope.genericSale.StoreTransactions,
                   netAmount: $scope.genericSale.Sale.NetAmount,
                   discountAmount: $scope.genericSale.Sale.DiscountAmount,
                   vatAmount: $scope.genericSale.Sale.VATAmount
               };
             
               //if ($rootScope.store.DeductStockAtSalesPoint === true)
               //{
               //    //Recalculate the stock quantities of the sold items
               //    angular.forEach($scope.genericSale.Sale.StoreItemSoldObjects, function (u, i)
               //    {
               //        for (var m = 0; m < $scope.items.length; m++)
               //        {
               //            var x = $scope.items[m];
               //            if (x.StoreItemStockId === u.StoreItemStockId)
               //            {
               //                //Get the difference in stock quantities
               //                var difference = x.QuantityInStock - u.QuantitySold;

               //                //if the difference is 0 or less than 0, remove it the stock item from the prefetched stock list
               //                if (difference === 0 || difference < 0)
               //                {
               //                    $scope.items.splice(m, 1);
               //                }
               //                else
               //                {
               //                    //else, it's Quantity in stock becomes the difference
               //                    x.QuantityInStock = difference;
               //                }
               //            }
               //        }
               //    });
               //}
               $scope.ngTable.fnClearTable();
               $scope.determinePrintOption();
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
           //alert('Print Receipt');

           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 50%; text-align: center; margin-left: 41%"><b style="border-bottom: 1px solid #000;">SALES INVOICE</h3></b></td></tr>' +
               '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date: ' + rec.date + " " + rec.time + '</td></tr>' +
               '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>';

               if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
                   contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
               }

               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px; margin-right: 3%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
               + '</td></tr></table>';

           contents += '<div class="col-md-12 divlesspadding" style="border-top: #000 solid 1px;">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
                
            angular.forEach(rec.receiptItems, function (item, i) 
            {
                //var img = '';
                //if (item.StoreItemStockObject.ImagePath !== undefined && item.StoreItemStockObject.ImagePath !== null && item.StoreItemStockObject.ImagePath.length > 0)
                //{
                //    img = '<img src="' + item.StoreItemStockObject.ImagePath + '" style="width:50px;height:40px">';
                //}
                //' + img + ' & nbsp;
                contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + item.StoreItemStockObject.StoreItemName + '(' + item.UoMCode + ')' + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td>' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            }); 

           contents += '</tbody></table></div>' +
               '<table style="width: 100%;font-size:0.9em"><tr><td><h5 style="float:left; text-align:left">Payment option(s)</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%"></h5></td>' +
               '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td style="text-align: right"><b>' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
           });

           contents += '<tr><td></td>' +
                             '<td>' + rec.waterMark + '</td></tr>';

           contents += '</table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td style="text-align: right"><b>' + filterCurrency(rec.amountDue, '') + '</b></td></tr>';

           if (rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right;">' + filterCurrency(rec.discountAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">' + $rootScope.store.VAT + '% VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.vatAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0 || rec.discountAmount > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.netAmount, '') + '</td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.amountReceived, '') + '</td></tr>';

           if (rec.outStandingPayment > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Outstanding(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.outStandingPayment, '') + '</td></tr>';
           }

           var balance = rec.amountReceived - rec.netAmount;
           if (balance > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(balance, '') + '</td></tr>';
           }
           
           contents += '</table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                '</div><br/><div class="row"><h5 style="font-style: italic; text-align: center">' + $rootScope.store.Slogan + '</h5></div><br><h5>Powered by: www.shopkeeper.ng</h5></div><br/>';

           //angular.element('#receipt').html('').html(contents);

           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null)
           {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Invoice Receipt</title>');
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
           else
           {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Invoice Receipt</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function () {
                   window.frames["frame1"].focus();
                   window.frames["frame1"].print();
                   //frame.document.body.innerHTML = '';
               }, 500);
           }

           //if ($scope.printDNote === true)
           //{
           //    $scope.printDeliveryNote(rec);
           //}
           //else
           //{
               $scope.initializeModel();
           //}
           
           return false;
        };

       $scope.printDeliveryNote = function (rec)
       {
           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 50%; text-align: center; margin-left: 43%"><b style="border-bottom: 1px solid #000;">DELIVERY NOTE</h3></b></td></tr>' +
               '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date: ' + rec.date + " " + rec.time + '</td></tr>' +
               '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>';

           if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
           }

           contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px; margin-right: 3%" alt="" src="' + $rootScope.store.StoreLogoPath + '">'
               + '</td></tr></table>';


           //+ '<td style="width: 100%;padding: 1px;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
           //             + '<tr style="font-size:1.1em;"><td  style=";margin-bottom: 4px;"><h5><b style="border-bottom: 1px solid #000">SALES INVOICE</b></h5></td><td></td></tr>'

           contents += '<div class="col-md-12 divlesspadding" style="border-top: #000 solid 1px;">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Delivery Qty.</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i) {
               //var img = '';
               //if (item.StoreItemStockObject.ImagePath !== undefined && item.StoreItemStockObject.ImagePath !== null && item.StoreItemStockObject.ImagePath.length > 0) {
               //    img = '<img src="' + item.StoreItemStockObject.ImagePath + '" style="width:50px;height:40px">';
               //}
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + item.StoreItemStockObject.StoreItemName + '(' + item.UoMCode + ')' + '</td><td>' + item.QuantityDelivered + '</td></tr>';
           });
           
           contents += '</tbody></table></div>' +
              '<table style="width: 100%;font-size:0.9em">' +
               '<tr><td><h5 style="float:left; text-align:left">Payment option(s)</h5></td>' +
              '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #ddd solid 1px;color: #008000;font-size:0.9em;"><td>' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td style="text-align: left"><b>' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
           });

           contents += '<tr><td></td>' +
                              '<td>' + rec.waterMark + '</td></tr>';

           contents += '</table></td></tr></table>';

           //angular.element('#receipt').html('').html(contents);
           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null) {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Delivery Note</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function () {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                       angular.element('#receipt').html('');
                   },

                   500);
               });
           }
           else {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Delivery Note</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function () {
                   window.frames["frame1"].focus();
                   window.frames["frame1"].print();
                   frame1.document.body.innerHTML = '';
                   angular.element('#receipt').html('');
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
           if ($scope.criteria === undefined || $scope.criteria === null || $scope.criteria.length < 1)
           {
               return;
           }

           if ($scope.criteria.trim().length < 1)
           {
               return;
           }

           $scope.priceList = [];
           saleServices.getItemsBySearchCriteria($rootScope.homeroot + '/Sales/SearchItemPriceListByOutlet?criteria=' + $scope.criteria, $scope.getpItemPricesCompleted);

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
       
       $scope.determinePrintOption = function ()
       {
           if ($scope.setPrintOption === undefined || $scope.setPrintOption === null || $scope.setPrintOption < 1)
           {
               alert('Please select a print option');
               return;
           }

           if ($scope.setPrintOption === 1)
           {
               $scope.printReceipt($scope.rec);
           }
           else
           {
               $scope.printWithLowerDimensions($scope.rec);
           }
           
       };

       $scope.setDetailsPrintOption = function () {
          
           if ($scope.setPrintX === undefined || $scope.setPrintX === null || $scope.setPrintX < 1)
           {
               alert('Please select a print option');
               return;
           }

           if ($scope.setPrintX === 3)
           {
               $scope.printReceipt($scope.rec);
               return;
           }

           if ($scope.setPrintX === 4)
           {
               $scope.printWithLowerDimensions($scope.rec);
               return;
           }
       };

       $scope.setDetailsNotePrintOption = function () {

           if ($scope.setPrintX === undefined || $scope.setPrintX === null || $scope.setPrintX < 1) {
               alert('Please select a print option');
               return;
           }

           if ($scope.setPrintX === 3) {
               $scope.printDeliveryNote($scope.rec);
               return;
           }

           if ($scope.setPrintX === 4)
           {
               $scope.printDeliveryNoteWithLowerDimensions($scope.rec);
               return;
           }
       };

       $scope.printWithLowerDimensions = function (rec)
       {
           //<img style="width: 60px; height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>

           //<h3 class="ng-binding" style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>

           var contents = '<table style="width: 100%; margin-left: 0px; border: none; color: #000; font-weight: bold" class="table"><tr style="border: none">'
                        + '<td style="width: 100%"><img style="height: 60px; float: left; margin-left:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/></td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">' + $rootScope.store.StoreAddress + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Website:  ' + $rootScope.store.Url + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Email: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.8em">'
                        + '<td style="width: 100%;padding: 1px;margin-bottom: 10px; text-align: center">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                        + '<tr style="font-size:1.3em;"><td style="margin-bottom: 2px;"><h4><b style="border-bottom: 1px solid #000; text-align: center; margin-left:20%">SALES INVOICE</b></h5></td></tr>'
                        + '</tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Date: ' + rec.date + " " + rec.time + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Invoice No.: ' + rec.referenceCode + '</td></tr>';

                       if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
                           contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
                       }

                       contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Customer: ' + rec.customer + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table>';


           contents += 
            '<table class="table" role="grid" style="width: 100%; font-weight: bold; margin-left: 0px;border-bottom: 1px solid #000">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em;"><th style="color: #008000; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.93em; width:20%;">Qty</th><th style="color: #008000; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i)
           {
               contents += '<tr><td style="border-bottom: #ddd solid 1px;font-size:0.7em">' + item.StoreItemStockObject.StoreItemName + '(' + item.UoMCode + ')' + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + item.QuantitySold + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td style="text-align: right;font-size:0.79em">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
           });

           contents += '</tbody></table>' +
               '<table style="width: 100%; font-weight: bold; margin-left: 0px;">';

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;;"><td style="color: #008000">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td style="text-align: right;"><b>' + filterCurrency(rec.amountDue, '') + '</td>' +
               '</tr>';
           if (rec.discountAmount > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right;">' + filterCurrency(rec.discountAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">' + $rootScope.store.VAT + '% VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.vatAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0 || rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.netAmount, '') + '</td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.amountReceived, '') + '</td></tr><tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td colspan="2"></td></tr><tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr>';

           if (rec.outStandingPayment > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Outstanding(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.outStandingPayment, '') + '</td></tr>';
           }

           var balance = rec.amountReceived - rec.netAmount;
           if (balance > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(balance, '') + '</td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"><h5 style="float:left; text-align:left;font-size:0.95em; font-weight: bold;">Payment option(s):</h5></td></tr>';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.89em;"><td style="color: #008000;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td style="text-align: right"><b>' + filterCurrency(l.TransactionAmount, '') + '</td></tr>';
           });
           
           contents += '<tr style="border: none;" colspan="2">' + rec.waterMark.replace('25%', '-9%').replace('6%', '9%')  + '<td></tr>';

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
               '</div><br/><div class="row"><h5 style="font-style: italic; text-align: center">' + $rootScope.store.Slogan + '</h5></div><br><h5>Powered by: www.shopkeeper.ng</h5></div><br>';
           angular.element('#receipt').html('').html(contents);

           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null) {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Invoice Receipt</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function ()
                   {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                       angular.element('#receipt').html('');
                   },500);
               });
           }
           else {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Thanks for patronizing us</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function () {
                   window.frames["frame1"].focus();
                   window.frames["frame1"].print();
                   frame1.document.body.innerHTML = '';
                   angular.element('#receipt').html('');
               }, 500);
           }

           //if ($scope.printDNote === true)
           //{
           //    $scope.printDeliveryNoteWithLowerDimensions(rec);

           //} else {
               $scope.initializeModel();
           //}
          
           //$scope.setPrintX = '';
           return false;
       };

       $scope.printDeliveryNoteWithLowerDimensions = function (rec)
       {
           
           var contents = '<table style="width: 100%; margin-left: 0px; border: none; color: #000; font-weight: bold" class="table"><tr style="border: none">'
                        + '<td style="width: 100%"><img style="height: 60px; margin-left:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/></td><td></td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">' + $rootScope.store.StoreAddress + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Website:  ' + $rootScope.store.Url + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Email: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.8em">'
                        + '<td style="width: 100%;padding: 1px;margin-bottom: 10px; text-align: center">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                        + '<tr style="font-size:1.3em;"><td style=";margin-bottom: 2px;"><h4><b style="border-bottom: 1px solid #000; text-align: center; margin-left:20%">DELIVERY NOTE</b></h5></td></tr>'
                        + '</tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Date: ' + rec.date + " " + rec.time + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Invoice No.: ' + rec.referenceCode + '</td></tr>';

           if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
               contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
           }

           contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Customer: ' + rec.customer + '</td></tr>'
            + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table>';


           contents +=
            '<table class="table" role="grid" style="width: 100%; font-weight: bold; margin-left: 0px;border-bottom: 1px solid #000">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em;"><th style="color: #008000; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.93em; width:20%;">Delivery Qty.</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i)
           {
               contents += '<tr><td style="border-bottom: #ddd solid 1px;font-size:0.7em">' + item.StoreItemStockObject.StoreItemName + '(' + item.UoMCode + ')' + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + item.QuantityDelivered + '</td></tr>';
           });

           contents += '</tbody></table>' +
               '<table style="width: 100%; font-weight: bold; margin-left: 0px;">';

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"><h5 style="float:left; text-align:left;font-size:0.95em; font-weight: bold;">Payment option(s):</h5></td></tr>';

           angular.forEach(rec.paymentChoices, function (l, i) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.89em;"><td style="color: #008000;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td style="text-align: right"><b>' + filterCurrency(l.TransactionAmount, '') + '</td></tr>';
           });

           contents += '<tr style="border: none;" colspan="2">' + rec.waterMark.replace('25%', '-9%').replace('6%', '9%') + '<td></tr>';

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
               '</div><br/><div class="row"><h5 style="font-style: italic; text-align: center">' + $rootScope.store.Slogan + '</h5></div><br><h5>Powered by: www.shopkeeper.ng</h5></div><br>';
           //angular.element('#receipt').html('').html(contents);
           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null) {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Delivery Note</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function () {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                       angular.element('#receipt').html('');
                   },

                   500);
               });
           }
           else {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Delivery Note</title>');
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

