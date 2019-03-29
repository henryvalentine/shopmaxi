"use strict";

define(['application-configuration', 'saleServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngRevokedSlaes', function ($compile)
    {
        return function ($scope, ngRevokedSlaes) {
            var tableOptions = {};
            tableOptions.sourceUrl = "/Sales/GetRevokedSales";
            tableOptions.itemId = 'SaleId';
            tableOptions.columnHeaders = ["InvoiceNumber", "CustomerName", "AmountDueStr", "NetAmountStr", 'DateStr', "DateRevokedStr"];
            var ttc = invoiceTableManager($scope, $compile, ngRevokedSlaes, tableOptions, 'New Refund', 'newRefund', 'getRefundDetails', 115);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };
    });
    
    app.register.controller('refundNoteController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'saleServices', '$filter', '$locale',
        function (ngDialog, $scope, $rootScope, $routeParams, saleServices, $filter, $locale)
        {

        $scope.goBack = function ()
        {
            $scope.newRevoke = false;
            $scope.details = false;
        };

        $scope.newRefund = function ()
        {
            $scope.initializeRefund();
            $scope.newRevoke = true;
            $scope.details = false;
        };
       
        $scope.initializeController = function ()
        {
            $scope.newRevoke = false;
            $scope.details = false;
            $scope.revokeSale = false;
            $scope.edit = false;
            $scope.refundProduct = false;
            $scope.getProducts();
        };

        $scope.setChoice = function (choice)
        {
            if (choice === 3)
            {
                $scope.refundProduct = true;
                $scope.revokeSale = false;
                $scope.refundNote.ReturnedProductObjects = [];
                angular.forEach($scope.refundObject.SoldItems, function (r, i)
                {
                    r.QuantityReturned = 0;
                    r.AmountRefunded = 0;
                });

                $scope.refundNote.VATAmount = 0;
                $scope.refundNote.TotalAmountRefunded = 0;

                $scope.refundNote.AmountDue = 0;
                $scope.refundNote.NetAmount = 0;
                $scope.refundNote.IssueTypeId = 0;
                $scope.refundNote.IssueTypeObject = { IssueTypeId: '', Name: '-- Select Reason to revoke transaction --', Description: '' };
                $scope.refundNote.ReturnedProductObjects = [];
            }
            else
            {
                $scope.revokeSale = true;
                $scope.refundProduct = false;

                $scope.refundNote.VATAmount = 0;
                $scope.refundNote.TotalAmountRefunded = 0;

                $scope.refundNote.AmountDue = 0;
                $scope.refundNote.NetAmount = 0;
                $scope.refundNote.IssueTypeId = 0;
                $scope.refundNote.IssueTypeObject = { IssueTypeId: '', Name: '-- Select Reason to revoke transaction --', Description: '' };
                $scope.refundNote.ReturnedProductObjects = [];
              
                angular.forEach($scope.refundObject.SoldItems, function (r, i)
                {
                    //if the sale is completely revoked, the returned Quantities become
                    //equal to the sold Quantities

                    var item =
                        {
                            ReturnedProductId: 0,
                            IssueTypeId: 0,
                            StoreItemStockId: r.StoreItemStockId,
                            RefundNoteId: 0,
                            DateReturned: '',
                            QuantityBought: r.QuantitySold,
                            StoreItemName: r.StoreItemName,
                            QuantityReturned: r.QuantitySold,
                            AmountRefunded: r.AmountSold,
                            ImagePath: r.ImagePath,
                            PurchaseOrderItemId: r.PurchaseOrderItemId,
                            Rate: r.Rate,
                            Reason: ''
                        }

                    r.QuantityReturned = r.QuantitySold;
                    r.AmountRefunded = r.AmountSold;

                    var priceInfo = $scope.getPrice(r);
                    if (priceInfo.rate < 1)
                    {
                        $scope.setError('An error was encountered. Please try again later.');
                        return;
                    }

                    item.AmountRefunded = priceInfo.price;
                    item.Rate = priceInfo.rate;
                    item.refHolder = priceInfo.price;
                    item.rateHolder = priceInfo.rate;
                 
                    $scope.refundNote.AmountDue += item.AmountRefunded;
                    $scope.refundNote.ReturnedProductObjects.push(item);

                    $scope.updateVatDiscountAmount(item);

                    
                });

            }
            $scope.edit = true;
        }

        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        $scope.getInvoiceDetails = function (invoiceNumber)
        {
            if (invoiceNumber === undefined || invoiceNumber === null || invoiceNumber.length < 1)
            {
                alert("ERROR: Please provide a valid Invoice number!");
                return;
            }

            saleServices.getSaleToRefund(invoiceNumber, $scope.getInvoiceDetailsCompleted);
        }; 

        $scope.getInvoiceDetailsCompleted = function (rec)
        {
            if (rec.SaleId < 1)
            {
                alert("No match found!");
                return;
            }

            $scope.initializeRefund();
            
            $scope.fullyRefunded = true;
            var refundcount = rec.StoreItemSoldObjects.length;
            angular.forEach(rec.StoreItemSoldObjects, function (s, i)
            {
                //for record purposes, to get the proportional rate and total 
                //for remaining unreturned product quantity 
                if (s.QuantitySold > s.QuantityReturned && s.QuantityReturned > 0)
                {
                    refundcount -= 1;
                    var quantityHolder = s.QuantitySold - s.QuantityReturned;
                    s.QuantitySold = quantityHolder;
                    s.QuantityReturned = quantityHolder;
                    s.fullyRefunded = false;
                    var priceInfo = $scope.getPrice(s);

                    if (priceInfo.price > 0)
                    {
                        s.AmountSold = priceInfo.price;
                        s.Rate = priceInfo.rate;
                        s.QuantityReturned = '';
                    }
                }
                else
                {
                    if (s.QuantityReturned < s.QuantitySold)
                    {
                        s.fullyRefunded = false;
                        refundcount -= 1;
                    }

                    if (s.QuantityReturned >= s.QuantitySold)
                    {
                        s.fullyRefunded = true;
                    }
                }
                
                if (s.ItemPriceObjects.length > 1)
                {
                    s.ItemPriceObjects.sort(function (a, b)
                    {
                        return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
                    });
                }
            });

            if (refundcount < rec.StoreItemSoldObjects.length)
            {
                $scope.fullyRefunded = false;
            }
            
            $scope.refundObject.Sale = rec;
            $scope.refundObject.SoldItems = rec.StoreItemSoldObjects;
            
            $scope.refundObject.StoreTransactions = rec.Transactions;
            
          $scope.refundNote =
          {
              Id: 0,
              SaleId: rec.SaleId,
              Discount: rec.Discount,
              DiscountAmount: 0,
              VAT: rec.VAT,
              VATAmount: 0,
              TotalAmountRefunded : 0,
              IssueTypeName: '',
              InvoiceNumber: '',
              CustomerName: '',
              EmployeeName: '',
              PaymentMethodId: rec.Transactions[0].StorePaymentMethodId,
              DateReturned: '',
              AmountDue: 0,
              NetAmount: 0,
              EmployeeId: rec.EmployeeId,
              CustomerId: rec.CustomerId,
              IssueTypeObject: { IssueTypeId: '', Name: '-- Select Reason to revoke transaction --', Description: '' },
              ReturnedProductObjects: []
          }
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
            if (rec.length < 1) {
                return;
            }

            $scope.detailRefundNotes = rec;
        };

        $scope.getRefundDetails = function (id)
        {
            if (id < 1)
            {
                alert("ERROR: Invalid selection");
                return;
            }
            $scope.initializeRefund();
            saleServices.getRevokedSalesInfo(id, $scope.getRevokedSalesInfoCompleted);
        };

        $scope.getRevokedSalesInfoCompleted = function (rec)
        {
            if (rec.SaleId < 1)
            {
                alert("No match found!");
                return;
            }
           
            angular.forEach(rec.Transactions, function (s)
            {
                s.StorePaymentMethod = { Name: s.StorePaymentMethod, TransactionAmount: s.TransactionAmount };
            });
            
            $scope.refundObject.Sale = rec;
            $scope.refundObject.SoldItems = rec.ReturnedProductObjects;

            $scope.refundObject.StoreTransactions = rec.Transactions;

            $scope.refundNote.IssueTypeId = null;

            $scope.newRefund = false;
            $scope.details = true;
            angular.element('#refundDetails').html('');

            angular.forEach(rec.ReturnedProductObjects, function (s, i)
            {

                if (s.ItemPriceObjects.length > 1) {
                    s.ItemPriceObjects.sort(function (a, b) {
                        return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
                    });
                }
            });

            saleServices.getSaleRefundNotes(rec.SaleId, $scope.getRefundNoteDetailsCompleted);
        };

        $scope.printRefund = function ()
        {
            if ($scope.sale === undefined || $scope.sale === null || $scope.sale.SaleId < 1)
            {
                alert("An unknown Error was encountered. Please refresh the page and try again.");
                return;
            }

            $scope.rec =
              {
                referenceCode: $scope.sale.InvoiceNumber,
                date: $scope.sale.DateStr,
                customer: $scope.sale.CustomerName,
                time: '',
                dateRevokedStr : $scope.sale.DateRevokedStr,
                storeAddress: $rootScope.store.StoreAddress,
                cashier: $rootScope.user.Name,
                change: $scope.sale.AmountRefunded,
                receiptItems: $scope.sale.StoreItemSoldObjects,
                amountDue: $scope.sale.AmountDue,
                subtotal: 0,
                amountReceived: $scope.sale.AmountPaid,
                amountToBalance: $scope.sale.Balance,
                paymentChoices: $scope.sale.Transactions,
                netAmount: $scope.sale.NetAmount,
                discountAmount: $scope.sale.DiscountAmount,
                vatAmount: $scope.sale.VATAmount
              };

            $scope.determineDetailPrintOption();
        }

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

        $scope.initializeRefund = function ()
        {
            $scope.selected = undefined;
            $scope.refPrintOption = 2;
            $scope.revokeSale = false;
            $scope.refundProduct = false;
            $scope.customer = null;
            $scope.choice = '';
            $scope.refundObject =
            {
                'Sale': { 'SaleId': 0, 'RegisterId': 0, 'CustomerId': 0, 'EmployeeId': 0, 'AmountDue': 0, CustomerName: '', 'Status': '', 'Date': '', Discount: '', VATAmount: 0, DiscountAmount: '', NetAmount: 0, applyVat: false, VAT: 0, EstimateNumber: ''},
                'StoreTransactions': [],
                'SoldItems': []
            };

            $scope.refundNote =
            {
                Id: 0,
                SaleId: 0,
                Discount: 0,
                DiscountAmount: 0,
                VAT: 0,
                VATAmount: 0,
                TotalAmountRefunded : 0,
                IssueTypeName: '',
                InvoiceNumber: '',
                CustomerName: '',
                EmployeeName: '',
                PaymentMethodId: 0,
                DateReturned: '',
                AmountDue: 0,
                NetAmount: 0,
                IssueTypeId: null,
                EmployeeId: 0,
                CustomerId: 0,
                CustomerObject: {UserProfileName: ''},
                IssueTypeObject : {IssueTypeId : '', Name : '-- Select Reason to revoke transaction --', Description : ''},
                ReturnedProductObjects : []
            }
           
        };
            
       function getIssueTypesCompleted(issuTypes)
        {
            $scope.issuTypes = issuTypes;
        }
            
       $scope.getProducts = function ()
        {
            $scope.items = [];
            $scope.page = 0;
            $scope.itemsPerPage = 50;
            saleServices.getIssueTypes(getIssueTypesCompleted);
            //saleServices.getEveryPriceList($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
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
        
       $scope.getPrice = function(item)
       {
       
            var priceResult = { 'rate': 0, 'price': 0 };

            //var results = $scope.items.filter(function (u)
            //{
            //    return u.StoreItemStockId === item.StoreItemStockId;
            //});
           
            //if (results.length < 1)
            //{
            //    $scope.setError('An error was encountered. Please refresh the page and try again');
            //    return priceResult;
            //}

            var x = item.ItemPriceObjects;
           
            if (x.length < 1)
            {
                $scope.setError('An error was encountered. Please refresh the page and try again');
                return priceResult;
            }

            if (x.length === 1)
            {
                priceResult.rate = x[0].Price;
                priceResult.price = x[0].Price * item.QuantityReturned;
                if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1)
                {
                    priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                }

                return priceResult;
            }

            var result = x.filter(function (u)
            {
                return u.MinimumQuantity === item.QuantityReturned;
            });

            if (result.length > 0) {

                priceResult.rate = result[0].Price;
                priceResult.price = result[0].Price * item.QuantityReturned;
                if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                    priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                }
                return priceResult;
            }

            if (item.QuantityReturned >= x[x.length - 1].MinimumQuantity) {
                priceResult.rate = x[x.length - 1].Price;
                priceResult.price = x[x.length - 1].Price * item.QuantityReturned;
                if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                    priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                }
                return priceResult;
            }
            if (item.QuantityReturned <= x[0].MinimumQuantity) {
                priceResult.rate = x[0].Price;
                priceResult.price = x[0].Price * item.QuantityReturned;
                if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                    priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                }
                return priceResult;
            }

            else {
                for (var i = 0; i < x.length; i++) {
                    if (item.QuantityReturned <= x[i].MinimumQuantity) {
                        priceResult.rate = x[i].Price;
                        priceResult.price = x[i].Price * item.QuantityReturned;
                        if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                            priceResult.price = Math.ceil(priceResult.price / 100) * 100;;
                        }
                        return priceResult;
                    }
                    else {
                        if (item.QuantityReturned > x[i].MinimumQuantity && item.QuantityReturned <= x[i + 1].MinimumQuantity) {
                            priceResult.rate = x[i + 1].Price;
                            priceResult.price = x[i + 1].Price * item.QuantityReturned;
                            if (item.StoreItemName.indexOf('LPG COOKING GAS') > -1) {
                                priceResult.price = Math.ceil(priceResult.price / 100) * 100;
                            }
                            return priceResult;
                        }
                    }

                }
                return priceResult;
            }
       };
         
       $scope.updateAmount = function (returnedProduct)
       {
           if (returnedProduct == null || returnedProduct.StoreItemStockId < 1 || returnedProduct.QuantityReturned === null || returnedProduct.QuantityReturned < 1)
           {
               return;
            }

            if (returnedProduct.QuantityReturned === '' ||returnedProduct.QuantityReturned === undefined ||returnedProduct.QuantityReturned === NaN ||returnedProduct.QuantityReturned === 0)
            {
                return;
            }

            if (returnedProduct.QuantityReturned > returnedProduct.QuantitySold)
            {
                alert('Quantity returned cannot be more than quantity sold');
                returnedProduct.QuantityReturned = '';
                return;
            }
           
            var priceInfo = $scope.getPrice(returnedProduct);
            if (priceInfo.rate < 1)
            {
                $scope.setError('An error was encountered. Please try again later.');
                return;
            }
          
            returnedProduct.AmountRefunded = priceInfo.price;
            returnedProduct.Rate = priceInfo.rate;
            returnedProduct.refHolder = priceInfo.price;
            returnedProduct.rateHolder = priceInfo.rate;

            $scope.refundNote.AmountDue += returnedProduct.AmountRefunded;

           $scope.updateVatDiscountAmount(returnedProduct);
       };

       $scope.updateVatDiscountAmount = function (returnedProduct)
       {
           if (returnedProduct.AmountRefunded < 1)
           {
               $scope.setError('Action could not be completed. Please refresh the pag and try again');
               return;
           }

           var dst = parseFloat($scope.refundNote.Discount);

           var discountAmount = 0;

           if (dst !== undefined && dst !== NaN && dst !== null && dst > 0)
           {
               discountAmount = (dst * returnedProduct.refHolder) / 100;
               returnedProduct.refHolder -= discountAmount;
               $scope.refundNote.DiscountAmount += discountAmount;
           }

           var vatAmount = 0;

           if (parseFloat($scope.refundNote.VAT) > 0)
           {
               vatAmount = (returnedProduct.refHolder * $scope.refundNote.VAT) / 100;
               returnedProduct.refHolder += vatAmount;
               $scope.refundNote.VATAmount += vatAmount;
           }
           
           $scope.refundNote.NetAmount += returnedProduct.refHolder;
           $scope.refundNote.TotalAmountRefunded = $scope.refundNote.NetAmount;

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
            
       $scope.processRefundNote = function ()
       {
           var err = false;
            if ($scope.refundProduct === true)
            {
                if ($scope.refundNote.AmountDue == undefined || $scope.refundNote.AmountDue == null || $scope.refundNote.AmountDue < 1) {
                    $scope.setError('An unexpected error was encountered. Please review this Transaction details and try again.');
                    return;
                }

                $scope.refundNote.ReturnedProductObjects = [];
                $scope.refundNote.IssueTypeId = null;

                angular.forEach($scope.refundObject.SoldItems, function (r, i)
                {
                    var st = parseFloat(r.QuantityReturned);
                    if (st !== undefined && st !== NaN && st !== null && st > 0)
                    {
                        if (r.IssueType === undefined || r.IssueType === null || r.IssueType.IssueTypeId < 1)
                        {
                           err = true;
                           return;
                        }
                        else
                        {
                            var item =
                            {
                                ReturnedProductId: 0,
                                IssueTypeId: r.IssueType.IssueTypeId,
                                StoreItemStockId: r.StoreItemStockId,
                                RefundNoteId: 0,
                                StoreItemName: r.StoreItemName,
                                DateReturned: '',
                                QuantityBought: r.QuantitySold,
                                QuantityReturned: r.QuantityReturned,
                                AmountRefunded: r.AmountRefunded,
                                ImagePath: r.ImagePath,
                                PurchaseOrderItemId: r.PurchaseOrderItemId,
                                Rate: r.Rate,
                                Reason: r.IssueType.Name
                            }

                            $scope.refundNote.ReturnedProductObjects.push(item);
                            
                        }
                       
                    }
                   
                });

            }

            if (err === true)
            {
                alert('Please select the reason for returning the selected product(s)');
                return;
            }

            if ($scope.revokeSale === true)
            {

                if ($scope.refundObject.Sale.IssueType === undefined || $scope.refundObject.Sale.IssueType === null || $scope.refundObject.Sale.IssueType.IssueTypeId < 1) {
                    alert('Please select the reason for revoking this transaction');
                    return;
                }
                if ($scope.refundNote.AmountDue == undefined || $scope.refundNote.AmountDue == null || $scope.refundNote.AmountDue < 1) {
                    $scope.setError('An unexpected error was encountered. Please review this Transaction details and try again.');
                    return;
                }

                if ($scope.refundNote.ReturnedProductObjects.length < 1) {
                    $scope.setError('Action could not be completed. Please try again later.');
                    return;
                }
                angular.forEach($scope.refundNote.ReturnedProductObjects, function (r, i)
                {
                    r.IssueTypeId = $scope.refundObject.Sale.IssueType.IssueTypeId;
                    r.Reason = $scope.refundObject.Sale.IssueType.Name;
                });
                
                   $scope.refundNote.IssueTypeId = $scope.refundObject.Sale.IssueType.IssueTypeId;
               
            }
           
           $scope.processing = true;

           saleServices.refundSale($scope.refundNote, $scope.processRefundNoteCompleted);
       };
        
       $scope.processRefundNoteCompleted = function (data)
       {
           $scope.processing = false;
           if (data.Code < 1)
           {
               alert(data.Error);
               return;
           }
          
            $scope.setSuccessFeedback(data.Error);
             
            $scope.rec =
            {
                referenceCode: $scope.refundObject.Sale.InvoiceNumber,
                date: $scope.refundObject.Sale.DateStr,
                customer: $scope.refundObject.Sale.CustomerName,
                refNumber: data.RefundNoteNumber,
                time: data.Time,
                storeAddress : $rootScope.store.StoreAddress,
                cashier: $rootScope.user.Name,
                dateRevokedStr: data.Date,
                change: $scope.refundNote.TotalAmountRefunded,
                receiptItems: $scope.refundNote.ReturnedProductObjects,
                amountDue: $scope.refundNote.AmountDue,
                amountToBalance: $scope.refundNote.TotalAmountRefunded,
                paymentChoice: {},
                netAmount: $scope.refundNote.NetAmount,
                discountAmount: $scope.refundNote.DiscountAmount,
                vatAmount: $scope.refundNote.VATAmount
            }

            var choice = $scope.refundObject.StoreTransactions[0];
            var refundOption = { PaymentMethodName: choice.PaymentMethodName, StorePaymentMethodId: choice.StorePaymentMethodId, TransactionAmount: $scope.rec.amountToBalance };
            $scope.rec.paymentChoice = refundOption;

            $scope.ngTable.fnClearTable();
            $scope.determineRefundPrintOption();
           
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
           $scope.initializeRefund();
           popupWin.document.close();

           return true;
       };
       
       $scope.printReceipt = function (rec)
       {
           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 class="ng-binding" style="width: 100%; font-size:0.9em"><b>' + $rootScope.store.StoreName + '</b></h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website:  ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.7em"><td style="width: 100%">Date Invoiced: ' + rec.date + '</td></tr>'
               + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date Refunded: ' + rec.dateRevokedStr + '</td></tr>'
               + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>'
               + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Refund Note No.: ' + rec.refNumber + '</td></tr>'
               + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + rec.customer + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '">'
               + '</td></tr><tr style="font-size:0.9em;"><td style="width: 43%"></td><td  style="width: 40%;"><h5><b style="border-bottom: 1px solid #000">Sales Refund Note</b></h5></td><td></td></tr></table>';
           
              contents += '<div class="col-md-12 divlesspadding">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty. Returned</th><th style="color: #008000;font-size:0.9em; width:20%;">Amount Refunded(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Refunded Reason</th></tr></thead><tbody>';
                
              angular.forEach(rec.receiptItems, function (item, i)
              {
                  //var img = '';
                  //if (item.ImagePath !== undefined && item.ImagePath !== null && item.ImagePath.length > 0)
                  //{
                  //    img = '<img src="' + item.ImagePath + '" style="width:50px;height:40px">&nbsp;';
                  //}
                  contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + item.StoreItemName + '</td><td>' + item.QuantityReturned + '</td><td>' + filterCurrency(item.AmountRefunded, '') + '</td>' +
                    '<td>' + item.Reason + '</td></tr>';
              });


           contents += '</tbody></table></div>' +
               '<table style="width: 100%;font-size:0.9em"><tr><td><h5 style="float:left; text-align:left">Refund method :</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Refund Details:</h5></td>' +
               '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';

           contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">' + rec.paymentChoice.PaymentMethodName + ':</td>' +
                            '<td><b style="text-align: right">' + filterCurrency(rec.paymentChoice.TransactionAmount, '') + '</b></td></tr>';
          
           contents += '</table></td><td>' +
                '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b>' + filterCurrency(rec.discountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td><b style="text-align: right">' + filterCurrency(rec.vatAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr>' +
                    '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Total Amount Refunded(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td><b style="text-align: right">' + filterCurrency(rec.amountToBalance, '') + '</b></td></tr></table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                '<h5>Served by: <b>' + rec.cashier + '</b></h5></div></div>';
           //angular.element('#receipt').html('').html(contents);

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
                       + '<title>Sales Refund Note</title>');
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
                    + '<title>Sales Refund Note</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function ()
               {
                    window.frames["frame1"].focus();
                    window.frames["frame1"].print();
                    frame1.document.body.innerHTML = '';
                }, 500);
           }

           $scope.goBack();
           return false;
        };

       $scope.determineRefundPrintOption = function ()
       {
           if ($scope.refPrintOption === undefined || $scope.refPrintOption === null || $scope.refPrintOption < 1)
           {
               alert('Please select a print option');
               return;
           }

           if ($scope.refPrintOption === 1)
           {
               $scope.printReceipt($scope.rec);
               return;
           }

           if ($scope.refPrintOption === 2) {
               $scope.printWithLowerDimensions($scope.rec);
               return;
           }

           $scope.initializeRefund();
       };

       $scope.determineDetailPrintOption = function ()
       {
           if ($scope.setRefPrintDetail === undefined || $scope.setRefPrintDetail === null || $scope.setRefPrintDetail < 1) {
               alert('Please select a print option');
               return;
           }
        
           if ($scope.setRefPrintDetail === 5)
           {
               $scope.printReceipt($scope.rec);
               return;
           }

           if ($scope.setRefPrintDetail === 6) {
               $scope.printWithLowerDimensions($scope.rec);
               return;
           }
       };

       $scope.printWithLowerDimensions = function (rec)
       {
           var contents = '<table style="width: 100%; margin-left: 0px; border: none; color: #000; font-weight: bold" class="table"><tr style="border: none">'
                       + '<td style="width: 100%"><img style="height: 60px; float: left; margin-left:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/></td></tr>'
                       + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">' + $rootScope.store.StoreAddress + '</td></tr>'
                       + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Website:  ' + $rootScope.store.Url + '</td></tr>'
                       + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Email: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.8em">'
                       + '<td style="width: 100%;padding: 1px;margin-bottom: 10px; text-align: center">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                       + '<tr style="font-size:1.2em;"><td style="margin-bottom: 2px;"><h5><b style="border-bottom: 1px solid #000; text-align: center; margin-left:20%">SALES REFUND NOTE</b></h5></td></tr>'
                       + '</tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Date Invoiced: ' + rec.date + '</td></tr>'
                       + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%">Date Refunded: ' + rec.dateRevokedStr + '</td></tr>'
                       + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Invoice No.: ' + rec.referenceCode + '</td></tr>'
                       + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Refund Note No.: ' + rec.refNumber + '</td></tr>';
           
           if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
               contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
           }

           contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Customer: ' + rec.customer + '</td></tr>'
            + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table>';


           contents +=
            '<table class="table" role="grid" style="width: 100%; font-weight: bold; margin-left: 0px;border-bottom: 1px solid #000">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em;"><th style="color: #008000; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.93em; width:20%;">Qty</th><th style="color: #008000; width:20%;">Amount Refunded(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000; width:20%;">Refunded Reason</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i)
           {
               //var img = '';
               //if (item.ImagePath !== undefined && item.ImagePath !== null && item.ImagePath.length > 0)
               //{
               //    img = '<img src="' + item.ImagePath + '" style="width:50px;height:40px">&nbsp;';
               //}
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + item.StoreItemName + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + item.QuantityReturned + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.AmountRefunded, '') + '</td>' +
                   '<td style="text-align: right;font-size:0.79em">' + item.Reason + '</td></tr>';
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

           //contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
           //         '<td style="text-align: right"><b>' + filterCurrency(rec.amountReceived, '') + '</td></tr><tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td colspan="2"></td></tr><tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr>';

        
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Total Amount Refunded(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.amountToBalance, '') + '</td></tr>';
           

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"><h5 style="float:left; text-align:left;font-size:0.95em; font-weight: bold;">Payment option(s):</h5></td></tr>';
           
           
            contents += '<tr style="border-top: #ddd solid 1px;font-size:0.89em;"><td style="color: #008000;">' + rec.paymentChoice.PaymentMethodName + ':</td>' +
                            '<td style="text-align: right"><b>' + filterCurrency(rec.paymentChoice.TransactionAmount, '') + '</td></tr>';
           
           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
               '</div><h5>Powered by: www.shopkeeper.ng</h5></div>';
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
                       + '<title>Sales Refund Note</title>');
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
                    + '<title>Sales Refund Note</title>');
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

           return false;
       };
            
       $scope.checkAdmin = function ()
       {
           $scope.delAuth = { email: '', password: '' };
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/RefundNote/verifyAdmin.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       }

       $scope.verifyUser = function ()
       {
           if ($scope.delAuth.email.length < 1) {
               $scope.setError('Please provide your email.');
               return;
           }

           if ($scope.delAuth.password.length < 1) {
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
               alert(data.UserName);
               return;
           }
           
           ngDialog.close('/ng-shopkeeper/Views/Store/RefundNote/verifyAdmin.html', '');
           $scope.processRefundNote();
       };
    }]);
   
});

