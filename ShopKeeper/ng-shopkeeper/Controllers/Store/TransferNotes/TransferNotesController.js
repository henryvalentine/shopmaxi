"use strict";

define(['application-configuration', 'transferNoteServices', 'alertsService', 'ngDialog'], function (app)
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
                    $scope.getpRates();
                }
            });
            $scope.skupControl = ngPSku;
            $scope.skupControl.focus();
        };
    });

    app.register.directive('ngTransferNotes', function ($compile)
    {
        return function ($scope, ngTransferNotes)
        {
            var tableOptions = { sourceUrl: "/TransferNote/GetTransferNotes", itemId: 'Id' };

            tableOptions.columnHeaders = ["TransferNoteNumber", "TotalAmountStr", "DateGeneratedStr", "SourceOutletName", "TargetOutletName", 'GeneratedBy', 'StatusStr'];
            var ttc = transferNotesTableManager($scope, $compile, ngTransferNotes, tableOptions, 'New Tranfer Note', 'newTransferNote', 'getTransferNote', 'getTransferNoteDetails', 123);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };

    });

    app.register.controller('transferNoteController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'transferNoteServices', '$filter', '$locale',
        function (ngDialog, $scope, $rootScope, $routeParams, transferNoteServices, $filter, $locale)
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
            
        }

        $scope.initializeController = function ()
        {
            $scope.newEdit = false;
            $scope.details = false;
            
            $scope.getOutlets();
        };

        $scope.initializeTransferNote = function ()
        {
            $scope.transferNote =
            {
                Id: 0,
                SourceOutletId: 0,
                TargetOutletId: 0,
                GeneratedByUserId: 0,
                TotalAmount: 0,
                Status: '',
                Date: '',
                sourceOutlet: { OutletName: '-- Select Source Outlet --', StoreOutletId: '' },
                targetOutlet: { OutletName: '-- Select Target Outlet --', StoreOutletId: '' },
                TransferNoteNumber: '',
                TransferNoteItemObjects: [],
                Header : 'Generate Transfer Note'
            };
            $scope.setPrintOption = 2;
            $scope.selected = undefined;

        };  
        
        $scope.prepareInvoice = function ()
        {
            if ($scope.transferNote.Id > 0 && $scope.transferNote.ConvertedToInvoice === false)
            {
                $scope.initializeTransferNote();

                angular.forEach($scope.transferNote.TransferNoteItemObjects, function (d, i)
                {
                    var tempId = $scope.transferNote.TransferNoteItemObjects.length + 1;

                    var stockList = $scope.items.filter(function (item)
                    {
                        return item.StoreItemStockId === d.StoreItemStockId;
                    });
                    
                    if (stockList.length > 0)
                    {
                        var pricesList = stockList[0].ItemPriceObjects.filter(function (p)
                        {
                            return p.Price === d.Rate;
                        });

                        if (pricesList.length > 0)
                        {
                            var soldItem =
                            {
                                'StoreItemSoldId': '',
                                'TempId': tempId,
                                'StoreItemStockId': d.StoreItemStockId,
                                'StoreItemStockObject': { 'StoreItemStockId': d.StoreItemStockId, 'StoreItemId': 0, 'StoreItemName': d.StoreItemName, 'Price': d.Rate, 'MinimumQuantity': d.TotalQuantityRaised, 'SKU': d.SerialNumber, 'ImagePath': d.ImagePath },
                                'Id': 0,
                                'SKU': d.SerialNumber,
                                'GotFromSKU': true,
                                'Rate': d.Rate,
                                'ImagePath': d.ImagePath,
                                'QuantitySold': d.TotalQuantityRaised,
                                'TotalAmountRaised': d.Rate,
                                'UoMId': pricesList[0].UoMId,
                                'UoMCode': pricesList[0].UoMCode,
                                'DateSold': ''
                            };

                            $scope.transferNote.TransferNoteItemObjects.push(soldItem);
                        } 
                    }

                });

                $scope.CustomerObject = $scope.transferNote.CustomerObject;
                $scope.CustomerObject.UserProfileName = $scope.transferNote.CustomerName;
                
                if ($scope.transferNote.TransferNoteItemObjects.length > 0 && $scope.transferNote.TransferNoteItemObjects.length === $scope.transferNote.TransferNoteItemObjects.length)
                {
                    $scope.processInvoice = true;
                }
                else
                {
                    alert('Action failed. Please try again later');
                }
            }
            
        };
            
        $scope.updateAmount = function (item)
        {
            if (item == null || item.StoreItemStockId < 1)
            {
                $scope.setError('Invalid Operation');
                return;
            }
            
            if ($scope.transferNote === undefined || $scope.transferNote === null)
            {
                $scope.initializeTransferNote();
            }

            var test1 = parseFloat(item.TotalQuantityRaised);

            var test2 = parseFloat(item.Rate);

            if (isNaN(test1) || test1 < 1 || isNaN(test2) || test2 < 1)
            {
                $scope.transferNote.TotalAmount = $scope.transferNote.TotalAmount - item.TotalAmountRaised;
                $scope.transferNote.TotalAmount = $scope.transferNote.TotalAmount;

                $scope.transferNote.NetAmount = $scope.transferNote.NetAmount - item.TotalAmountRaised;
                $scope.transferNote.NetAmount = $scope.transferNote.NetAmount;
                item.TotalAmountRaised = 0;
                //$scope.updateAmountForDiscount();
            }
            else
            {
                if (test1 > item.StoreItemStockObject.QuantityInStock)
                {
                    $scope.setError('Only ' + item.StoreItemStockObject.QuantityInStock + item.StoreItemStockObject.ItemPriceObjects[0].UoMCode + ' of this item is available.');
                    item.TotalQuantityRaised = item.StoreItemStockObject.QuantityInStock;
                    test1 = parseFloat(item.TotalQuantityRaised);
                }

                item.TotalAmountRaised = test1 * test2;

                var totalAmount = 0;

                angular.forEach($scope.transferNote.TransferNoteItemObjects, function (t, k)
                {
                    totalAmount += t.TotalAmountRaised;
                });

                $scope.transferNote.TotalAmount = totalAmount;

                $scope.transferNote.NetAmount = totalAmount;

                //$scope.updateAmountForDiscount();

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

        $scope.newTransferNote = function ()
        {
            $scope.newEdit = true;            
            $scope.initializeTransferNote();
        };

        $scope.initializeTransferNoteItem = function ()
        {
            $scope.transferNoteItem =
            {
                StoreItemStockId: 0,
                TempId: 0,
                Id: '',
                TotalQuantityRaised: 0,
                Rate: 0,
                GotFromSKU: true,
                TransferNoteId: 0,
                SerialNumber: '',
                StoreItemName: '',
                TotalAmountRaised: 0,
                ImagePath: '/Content/images/noImage.png'
            }
            
        };

        $scope.getOutlets = function ()
        {
            $scope.initializeTransferNoteItem();
            transferNoteServices.getOutlets($scope.getOutletsCompleted);
        };

        $scope.getOutletsCompleted = function (outlets)
        {
            $scope.outlets = outlets;
            $scope.sourceOutlets = outlets;
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

            d.ItemPriceObjects.sort(function (a, b)
            {
                return (a['MinimumQuantity'] > b['MinimumQuantity']) ? 1 : ((a['MinimumQuantity'] < b['MinimumQuantity']) ? -1 : 0);
            });
            
            var tempId = $scope.transferNote.TransferNoteItemObjects.length + 1;
            var transferNoteItem =
            {
                StoreItemStockId: d.StoreItemStockId,
                TempId: tempId,
                Id: '',
                TotalQuantityRaised: '',
                BaseSellingPrice: d.ItemPriceObjects[0].Price,
                Rate: d.CostPrice,
                StoreItemStockObject: d,
                TransferNoteId: $scope.transferNote.Id,
                ImagePath: d.ImagePath,
                SerialNumber: d.SKU,
                StoreItemName: d.StoreItemName,
                UomCode: d.ItemPriceObjects[0].UoMCode,
                TotalAmountRaised: '',
                UoMId: d.ItemPriceObjects[0].UoMId
            }
           
            var matches = [];

            if ($scope.transferNote.TransferNoteItemObjects.length > 0)
            {
                 matches = $scope.transferNote.TransferNoteItemObjects.filter(function(x) {
                     return x.StoreItemStockId === transferNoteItem.StoreItemStockId;
                 });               
            }

            if (matches.length < 1)
            {
                $scope.transferNote.TransferNoteItemObjects.push(transferNoteItem);          
            }

            $scope.clearError();
            $rootScope.skuControl = document.getElementById('stockControl_value');
            $rootScope.skuControl.focus();
            $rootScope.searchStr = "";
        };

       $scope.filterTargetOutlet = function (sourceOutlet)
       {
            var targetOutlets = $scope.outlets.filter(function (f)
            {
                return f.StoreOutletId !== sourceOutlet.StoreOutletId;
            });

            $scope.targetOutlets = targetOutlets;
          
       };

       $scope.filterSourceOutlet = function (targetOutlet)
       {
           if ($scope.transferNote.sourceOutlet == undefined || $scope.transferNote.sourceOutlet == null || $scope.transferNote.sourceOutlet.StoreOutletId < 1)
           {
               var sourceOutlets = $scope.outlets.filter(function (f)
               {
                   return f.StoreOutletId !== targetOutlet.StoreOutletId;
               });

               $scope.sourceOutlets = sourceOutlets;
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

       $scope.removeTransferNoteItem = function (id)
       {
           index = 0;
           $scope.deleteItem = false;

            if (id < 1)
            {
                $scope.setError('Invalid selection');
                return;
            }
            var matchedItem = {};
            
            angular.forEach($scope.transferNote.TransferNoteItemObjects, function (x, y)
            {
                if (x.StoreItemStockId === id)
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
                transferNoteServices.deleteTransferNoteItem(matchedItem.Id, $scope.deleteTransferNoteItemCompleted);  
            }

            else
            {
                if (!confirm("This Item will be removed from the list. Continue?"))
                {
                    return;
                }
                $scope.transferNote.TransferNoteItemObjects.splice(index, 1);
                $scope.completeDeleteProcess();
            }
          
       };
     
       $scope.deleteTransferNoteItemCompleted = function (data)
       {
           alert(data.Error);

           if (data.Code < 1)
           {
               return;
           }
           $scope.transferNote.TransferNoteItemObjects.splice(index, 1);
           $scope.search.skuName = '';
           $scope.completeDeleteProcess();
       };

       $scope.completeDeleteProcess = function ()
       {
           
           if ($scope.transferNote === undefined || $scope.transferNote === null)
           {
               $scope.initializeTransferNote();
           }

           var totalAmount = 0;
           angular.forEach($scope.transferNote.TransferNoteItemObjects, function (y, i)
           {
               totalAmount += y.TotalAmountRaised;
           });

           if (totalAmount < 1)
           {
               $scope.transferNote.TotalAmount = 0;
           }

           $scope.transferNote.TotalAmount = totalAmount;

           $scope.deleteItem = false;
           $scope.initializeTransferNoteItem();
       };

       $scope.validateTransferNoteItem = function (transferNoteItem)
       {
           if (transferNoteItem.StoreItemStockId == undefined || transferNoteItem.StoreItemStockId == null || StoreItemStockId.StoreItemStockId < 1)
           {
                alert("ERROR: Please select a Product. ");
                return false;
            }

           if (transferNoteItem.TotalQuantityRaised == undefined || transferNoteItem.TotalQuantityRaised == null || transferNoteItem.TotalQuantityRaised < 1)
           {
                alert("ERROR: Please provide Item Quantity. ");
                return false;
           }

           if (transferNoteItem.Rate == undefined || transferNoteItem.Rate == null || transferNoteItem.Rate < 1)
           {
               alert("ERROR: Please Provide Price. ");
               return false;
           }

            return true;
       };
        
       $scope.validateTransferNote = function (transferNote)
       {
           if (transferNote.TotalAmount == undefined || transferNote.TotalAmount == null || transferNote.TotalAmount < 1)
           {
               $scope.setError('An unexpected error was encountered. Please review this Transaction details and try again.');
               return false;
           }
           
           if (transferNote.TransferNoteItemObjects == undefined || transferNote.TransferNoteItemObjects == null || transferNote.TransferNoteItemObjects < 1)
           {
               $scope.setError('ERROR: Please add at least one product for this transaction.');
               return false;
           }

           return true;
       };
            
       $scope.hideDelete = function ()
       {
           $scope.deleteItem = false;
       };
       
       $scope.processTransferNote = function ()
       {
            if (!$scope.validateTransferNote($scope.transferNote))
            {
               return;
            }

            if ($scope.transferNote.sourceOutlet == undefined || $scope.transferNote.sourceOutlet == null)
            {
                $scope.setError('ERROR: Please select source outlet');
                return;
            }

            if ($scope.transferNote.targetOutlet == undefined || $scope.transferNote.targetOutlet == null)
            {
                $scope.setError('ERROR: Please select target outlet');
                return;
            }

           var emptyObjects = $scope.transferNote.TransferNoteItemObjects.filter(function(o) {
               return o.Rate === undefined || o.Rate === null || o.Rate < 1 || o.TotalQuantityRaised === undefined || o.TotalQuantityRaised === null || o.TotalQuantityRaised < 1;
           } );

           if (emptyObjects.length > 0)
            {
               $scope.setError('ERROR: Please review the selected products and try again!');
               return;
            }
                

            $scope.transferNote.SourceOutletId = $scope.transferNote.sourceOutlet.StoreOutletId;
            $scope.transferNote.TargetOutletId = $scope.transferNote.targetOutlet.StoreOutletId;

            $scope.transferNote.SourceOutletName = $scope.transferNote.sourceOutlet.OutletName;
            $scope.transferNote.TargetOutletName = $scope.transferNote.targetOutlet.OutletName;
           

            $scope.processing = true;

            if ($scope.transferNote.Id < 1) 
            {
                transferNoteServices.addTransferNote($scope.transferNote, $scope.processTransferNoteCompleted);
            }
            else
            {
                transferNoteServices.editTransferNote($scope.transferNote, $scope.processTransferNoteCompleted); 
            }
       };
        
       $scope.processTransferNoteCompleted = function (data)
       {
           $scope.processing = false;
           if (data.Code < 1)
           {
              $scope.setError(data.Error);
               return;
           }
           else
           {
               $scope.transferNote.Id = data.Code;
               $scope.transferNote.TransferNoteNumber = data.ReferenceCode;

               $scope.setSuccessFeedback(data.Error);
            
               $scope.rec =
               {
                   referenceCode: data.ReferenceCode,
                   date: data.Date,
                   time: data.Time,
                   sourceOutlet: $scope.transferNote.SourceOutletName,
                   targetOutlet: $scope.transferNote.TargetOutletName,
                   storeAddress : $rootScope.store.StoreAddress,
                   cashier: $rootScope.user.Name,
                   receiptItems: $scope.transferNote.TransferNoteItemObjects,
                   totalAmount: $scope.transferNote.TotalAmount
               };
         
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
            
       $scope.printTransferNote2 = function (rec)
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
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:1.06em"><td><img src="' + item.ImagePath + '" style="width:50px;height:40px">&nbsp;' + item.StoreItemName + '</td><td>' + item.TotalQuantityRaised + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td style="text-align: right">' + filterCurrency(item.TotalAmountRaised, " ") + '</td></tr>';
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
       
       function getTransferNoteDetailsCompleted(rec)
       {
           if (rec == null || rec.Id < 1)
           {
               alert('Item could not be retrieved. Please try again later.');
               return;
           }

           $scope.initializeTransferNote();

           rec.Header = 'TransferNote Details';

           $scope.transferNote = rec;

             $scope.rec =
               {
                   referenceCode: rec.TransferNoteNumber,
                   date: rec.DateGeneratedStr,
                   sourceOutlet: rec.SourceOutletName,
                   targetOutlet: rec.TargetOutletName,
                   storeAddress: $rootScope.store.StoreAddress,
                   cashier: rec.GeneratedBy,
                   receiptItems: rec.TransferNoteItemObjects,
                   totalAmount: rec.TotalAmount
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
                  + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 70%; text-align: center; margin-left: 40%"><b style="border-bottom: 1px solid #000;">TRANSFER NOTE</h3></b></td></tr>'
                  + '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date Generated: ' + rec.date + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Ref No.: ' + rec.referenceCode + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Source Outlet: ' + rec.sourceOutlet + '</td></tr>'
                   + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Target Outlet: ' + rec.targetOutlet + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%"><h5>Generated by: <b>' + rec.cashier + '</b></h5></td></tr>'
                  + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Total Amount(' + $rootScope.store.DefaultCurrencySymbol + '):  <b>' + filterCurrency(rec.totalAmount, " ") + ' </b></td></tr>'
                  + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;"></td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
                  + '</td></tr></table>';
           

           contents += '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                 '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                  '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="color: #008000; width:20%;">Base S.P(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
           
           angular.forEach(rec.receiptItems, function (item, i)
           {
               contents += '<tr><td style="border-bottom: #ddd solid 1px;font-size:0.7em">' + item.StoreItemName + '(' + item.UoMCode + ')' + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.TotalQuantityRaised, " ") + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td style="font-size:0.79em">' + filterCurrency(item.TotalAmountRaised, " ") + '</td>' +
                   '<td style="font-size:0.79em">' + filterCurrency(item.BaseSellingPrice, " ") + '</td></tr>';
           });

           contents += '</tbody></table>' +
              '<div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
               '</div><br/><div class="row"><h5 style="font-style: italic; text-align: center">' + $rootScope.store.Slogan + '</h5></div><br><h5>Powered by: www.shopkeeper.ng</h5></div><br>';

           angular.element('#invoiceInfo').html('').append(contents);

           $scope.transferNote.Header = 'Transfer Note';
           $scope.rec = rec;
           $scope.newEdit = false;
           $scope.details = true;
           $scope.rec.Processed = true;

       }

       $scope.getTransferNoteDetails = function (transferNoteNumber, status)
       {
           $scope.rec = {Processed : false};
           if (transferNoteNumber.length < 1)
           {
               alert('Invalid selection!');
               return;
           }

           transferNoteServices.getTransferNoteByRef(transferNoteNumber, getTransferNoteDetailsCompleted);
           
       };

       function getTransferNoteCompleted(data)
       {
           if (data == null || data.Id < 1)
           {
               alert('Item could not be retrieved. Please try again later.');
               return;
           }

           if ($scope.transferNote === undefined || $scope.transferNote === null || $scope.transferNote === undefined || $scope.transferNote === null) {
               $scope.initializeTransferNote();
           }
           
           angular.forEach($scope.sourceOutlets, function (c, i)
           {
               if (c.StoreOutletId === data.SourceOutletId)
               {
                   data.sourceOutlet = c;
               }
           });

           var targetOutlets = $scope.outlets.filter(function (f)
           {
               return f.StoreOutletId !== data.SourceOutletId;
           });

           $scope.targetOutlets = targetOutlets;

           angular.forEach($scope.targetOutlets, function (c, i)
           {
               if (c.StoreOutletId === data.TargetOutletId)
               {
                   data.targetOutlet = c;
               }
           });
           
          
           $scope.initializeTransferNote();
           
           data.Header = 'Update Transfer Note';
           
           $scope.transferNote = data;
           $scope.newEdit = true;

           $scope.rec =
            {
                referenceCode: data.TransferNoteNumber,
                date: data.DateGeneratedStr,
                sourceOutlet: data.sourceOutlet.OutletName,
                targetOutlet: data.sourceOutlet.OutletName,
                storeAddress: $rootScope.store.StoreAddress,
                cashier: data.GeneratedByEmployee,
                receiptItems: data.TransferNoteItemObjects,
                totalAmount: data.TotalAmount
            };
       };

       $scope.getTransferNote = function (id)
       {
           if (id < 1)
           {
               alert('Invalid selection!');
               return;
           }
           transferNoteServices.getTransferNote(id, getTransferNoteCompleted);
       };
      
       $scope.preparePrint = function ()
       {
           if ($scope.customerInfo === undefined || $scope.customerInfo === null || $scope.customerInfo === undefined || $scope.customerInfo.CustomerId < 1)
           {
               $scope.customerInfo = { UserProfileName: $scope.transferNote.CustomerName, CustomerId: $scope.transferNote.CustomerId}
           }

           var rec =
           {
               referenceCode: $scope.transferNote.TransferNoteNumber,
               date: $scope.transferNote.DateCreatedStr,
               customer: $scope.customerInfo,
               storeAddress: $rootScope.store.StoreAddress,
               cashier: $rootScope.user.Name,
               receiptItems: $scope.transferNote.TransferNoteItemObjects,
               amountDue: $scope.transferNote.TotalAmount,
               netAmount: $scope.transferNote.NetAmount,
               discountAmount: $scope.transferNote.DiscountAmount,
               vatAmount: $scope.transferNote.VATAmount
           };
       
           $scope.printTransferNote(rec);
       };
            
       $scope.getTransferNoteInvoiceCompleted = function (rec)
       {
           if (rec.Id < 1)
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

           reportHtml += '<div class="row"><div class="col-md-12 divlesspadding"><h5>TransferNote Ref: <b>' + rec.TransferNoteNumber + '</b></h5>' +
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

           angular.forEach(rec.TransferNoteItemObjects, function (item, i)
           {

               reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td>' + filterCurrency(item.TotalAmountRaised, " ") + '</td></tr>';

               item.TotalQuantityRaised = item.QuantitySold;
               item.Rate = item.Rate;

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
                        '<td style="text-align: right"><b>' + filterCurrency(rec.TotalAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right"><b>' + filterCurrency(rec.DiscountAmount, '') + '</b></td>' +
                    '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td style="text-align: right"><b>' + filterCurrency(rec.VATAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td style="text-align: right"><b>' + filterCurrency(rec.NetAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                        '<td style="text-align: right"><b>' + filterCurrency(rec.AmountPaid, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.Balance, '') + '</b></td></tr></table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:1.06em">' +
               '<h5>Served by: <b>' + recEmployeeName + '</b></h5></div></div>';
           
           angular.element('#invoiceInfo').html('').append(reportHtml);

           $scope.transferNote.Header = 'Proforma Invoice';
           $scope.rec = rec;
           $scope.newEdit = false;
           $scope.details = true;
           $scope.rec.Processed = true;

           $scope.rec =
              {
                  referenceCode: rec.InvoiceNumber,
                  transferNoteRef: rec.TransferNoteNumber,
                  date: rec.DateStr,
                  customer: rec.CustomerName,
                  time: rec.Time,
                  storeAddress: $rootScope.store.StoreAddress,
                  cashier: $rootScope.user.Name,
                  receiptItems: rec.TransferNoteItemObjects,
                  amountDue: rec.TotalAmount,
                  amountReceived: rec.AmountPaid,
                  amountToBalance: rec.Balance,
                  paymentChoices: rec.Transactions,
                  netAmount: rec.NetAmount,
                  discountAmount: rec.DiscountAmount,
                  vatAmount: rec.VATAmount
              };
       };
      
       $scope.determinePrintOption = function () {
           if ($scope.setPrintOption === undefined || $scope.setPrintOption === null || $scope.setPrintOption < 1) {
               alert('Please select a print option');
               return;
           }

           if ($scope.setPrintOption === 1)
           {
               $scope.printWithA4($scope.rec);
           }
           else
           {
               $scope.printWithTermal($scope.rec);
           }

       };

       $scope.printWithTermal = function (rec) {
           var contents = '<table style="width: 100%; margin-left: 0px; border: none; color: #000; font-weight: bold" class="table"><tr style="border: none">'
                        + '<td style="width: 100%"><img style="height: 60px; float: left; margin-left:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/></td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">' + $rootScope.store.StoreAddress + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Website:  ' + $rootScope.store.Url + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Email: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.8em">'
                        + '<td style="width: 100%;padding: 1px;margin-bottom: 10px; text-align: center">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                        + '<tr style="font-size:1.3em;"><td style="margin-bottom: 2px;"><h4><b style="border-bottom: 1px solid #000; text-align: center; margin-left:20%">TRANSFER NOTE</b></h5></td></tr>'
                        + '</tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Date: ' + rec.date + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Invoice No.: ' + rec.referenceCode + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Source Outlet.: ' + rec.sourceOutlet + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Target Outlet: ' + rec.targetOutlet + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Generated by: ' + rec.cashier + '</td></tr></table>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Total Amount(' + $rootScope.store.DefaultCurrencySymbol + '): ' + filterCurrency(rec.totalAmount, " ") + '</td></tr></table>';
           
           contents +=
            '<table class="table" role="grid" style="width: 100%; font-weight: bold; margin-left: 0px;border-bottom: 1px solid #000">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em;"><th style="color: #008000; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.93em; width:20%;">Qty</th><th style="color: #008000; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                '<th style="color: #008000; width:20%;">Base S.P(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i) {
               contents += '<tr><td style="border-bottom: #ddd solid 1px;font-size:0.7em">' + item.StoreItemStockObject.StoreItemName + '(' + item.UomCode + ')' + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.TotalQuantityRaised, " ") + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td style="text-align: right;font-size:0.79em">' + filterCurrency(item.TotalAmountRaised, " ") + '</td>' +
                   '<td style="text-align: right;font-size:0.79em">' + filterCurrency(item.BaseSellingPrice, " ") + '</td></tr>';
           });

           contents += '</tbody></table>' +
              '<div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
               '</div><br/><div class="row"><h5 style="font-style: italic; text-align: center">' + $rootScope.store.Slogan + '</h5></div><br><h5>Powered by: www.shopkeeper.ng</h5></div><br>';

           angular.element('#receipt').html('').html(contents);

           var popupWin = '';
           if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
               popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.window.focus();
               popupWin.document.write('<!DOCTYPE html><html><head>' +
                   '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                   '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
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
               popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
               popupWin.document.close();
           }

           popupWin.document.close();
           $scope.initializeTransferNote();
           return false;
       };

       $scope.printWithA4 = function (rec)
       {
           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
                  + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
                  + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
                  + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
                  + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                  + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 70%; text-align: center; margin-left: 40%"><b style="border-bottom: 1px solid #000;">TRANSFER NOTE</h3></b></td></tr>'
                  + '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date Generated: ' + rec.date + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Ref No.: ' + rec.referenceCode + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Source Outlet: ' + rec.sourceOutlet + '</td></tr>'
                   + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Target Outlet: ' + rec.targetOutlet + '</td></tr>'
                  + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%"><h5>Generated by: <b>' + rec.cashier + '</b></h5></td></tr>'
                  + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Total Amount(' + $rootScope.store.DefaultCurrencySymbol + '):  <b>' + filterCurrency(rec.totalAmount, " ") + ' </b></td></tr>'
                  + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;"></td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
                  + '</td></tr></table>';

           contents += '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="color: #008000; width:20%;">Base S.P(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i) {
             
               contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.9em"><td>' + item.StoreItemName + '</td><td>' + filterCurrency(item.TotalQuantityRaised, " ") + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                 '<td style="text-align: right">' + filterCurrency(item.TotalAmountRaised, " ") + '</td>' +
                   '<td style="text-align: right">' + filterCurrency(item.BaseSellingPrice, " ") + '</td></tr>';
           });

           contents += '</tbody></table></div></table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;">' +
               '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Total Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                '<td><b style="text-align: right">' + filterCurrency(rec.totalAmount, '') + '</b></td></tr></table>' +
               '<div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
               '</div></div>';

           var popupWin = '';
           if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
               popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.window.focus();
               popupWin.document.write('<!DOCTYPE html><html><head>' +
                   '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                   '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
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
               popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + contents + '</div></div></html>');
               popupWin.document.close();
           }

           popupWin.document.close();
           $scope.initializeTransferNote();
           return false;
       };

    }]);
   
});

