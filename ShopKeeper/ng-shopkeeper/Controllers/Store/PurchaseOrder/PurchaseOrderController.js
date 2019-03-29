"use strict";

define(['application-configuration', 'purchaseOrderServices', 'ngDialog'], function (app)
{

    app.register.directive('ngPorders', function ($compile)
    {
        return function ($scope, ngPorders)
        {
            var tableOptions = {};

            if ($scope.isAdmin === true) {
                tableOptions.sourceUrl = "/Purchaseorder/GetPurchaseOrders";
            }
            else
            {
                if ($scope.isPurchaser === true)
                {
                    tableOptions.sourceUrl = "/Purchaseorder/GetMyPurchaseOrders";
                } else {
                    return;
                }
            }
            
            tableOptions.itemId = 'PurchaseOrderId';
            tableOptions.columnHeaders = ["PurchaseOrderNumber", "SupplierName", "DerivedTotalCostStr", "DateTimePlacedStr", 'ExpectedDeliveryDateStr', "ActualDeliveryDateStr", "Status"];
            var ttc = employeePurchaseOderTableManager($scope, $compile, ngPorders, tableOptions, 'New Purchase Order', 'newOrder', 'getPurchaseOrder', 'getOrderDetails', 168);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };
        
    });
  
    app.register.controller('purchaseOrderController', ['ngDialog', '$scope', '$rootScope', '$routeParams', '$filter', 'purchaseOrderServices',

        function (ngDialog, $scope, $rootScope, $routeParams, $filter, purchaseOrderServices)
        {

            $scope.isAdmin = $rootScope.isAdmin;
            $scope.isPurchaser = $rootScope.isPurchaser;

            //date picker settings
            var xcvb = new Date();
            var year = xcvb.getFullYear();
            var month = xcvb.getMonth() + 1;
            var day = xcvb.getDate();
            var minDate = year + '/' + month + '/' + day;
            setControlDate($scope, minDate, '');

            $scope.newOrder = function ()
            {
                $scope.initialisePurchaseOrder();
                $scope.newEdit = true;
            };

            function filterCurrency(amount, symbol)
            {
                var currency = $filter('currency');
                var value = currency(amount, symbol);
                return value;
            }

            function getSelectiblesCompleted(data)
            {
                $scope.suppliers = data.Suppliers;
                $scope.chartsofAccount = data.ChartOfAccounts;

                $scope.items = [];
                $scope.page = 0;
                $scope.itemsPerPage = 50;
                //purchaseOrderServices.getProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
            };
       
            $scope.initializeController = function ()
            {
                $scope.initialisePurchaseOrder();
                purchaseOrderServices.getSelectibles(getSelectiblesCompleted);
            };

            $scope.initialisePurchaseOrder = function () 
            {
                $scope.newEdit = false;
                $scope.details = false;
                $scope.header = 'New Purchase Order';
                $scope.purchaseOrder =
                {
                    PurchaseOrderId: '',
                    StoreOutletId: '',
                    AccountId: '',
                    SupplierId: '',
                    StatusCode: '',
                    DerivedTotalCost: '',
                    ExpectedDeliveryDate: '',
                    ExpectedDeliveryDateStr: '',
                    SupplierObject: { SupplierId: '', CompanyName: '-- select supplier --' },
                    ChartOfAccountObject: { ChartOfAccountId: '', AccountCode: '', AccountGroupName: '-- select chart of account --' },
                    PurchaseOrderItemObjects: []
                };
        };
            
            $rootScope.getProductByNameSku = function (val)
            {
                if (val == null)
                {
                    return;
                }

                var d = val.originalObject;
                if (d == null || d.StoreItemId === undefined || d.StoreItemId < 1)
                {
                    $scope.setError('Item product could not be accessed. Please try again later.');
                    return;
                }
            
                //check if this item has been selected before
                var secondResults = $scope.purchaseOrder.PurchaseOrderItemObjects.filter(function (s)
                {
                    return (s.StoreItemId === d.StoreItemId);
                });

                //if already selected, increment the quantity to be ordered
                if (secondResults.length > 0)
                {
                    secondResults[0].QuantityDelivered += 1;
                    return;
                }

                //if not selected before, add it to the order Items
                $scope.addProduct(d);
            };
        
            $scope.getProductsCompleted = function (products)
            {
                if (products.length < 1)
                {
                    return;
                }

                products.forEach(function (p, i)
                {
                    $scope.items.push(p);
                });

                $scope.page++;
                purchaseOrderServices.getProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
            };

            function getPurchaseOrderCompleted(rec)
            {
                if (rec == null || rec.PurchaseOrderId < 1)
                {
                    alert('Invalid selection!');
                    return;
                }
                var tempId = 1;
                angular.forEach(rec.PurchaseOrderItemObjects, function (k, g)
                {
                    k.TotalCost = k.QuantityOrdered * k.CostPrice;
                    k.TempId = tempId;
                    tempId++;
                });
                
                $scope.rec =
                  {
                      referenceCode: rec.PurchaseOrderNumber,
                      dateGenerated: rec.DateTimePlacedStr,
                      expectedDate: rec.ExpectedDeliveryDateStr,
                      supplier: rec.SupplierObject.CompanyName,
                      storeAddress: $rootScope.store.StoreAddress,
                      receiptItems: rec.PurchaseOrderItemObjects,
                      totalCost: rec.DerivedTotalCost
                  };

                //var suppliers = $scope.suppliers.filter(function (s)
                //{
                //    return s.SupplierId === data.SupplierId;
                //});

                //if (suppliers.length > 0)
                //{
                //    data.SupplierObject = suppliers[0];
                //}

                //var accountCharts = $scope.chartsofAccount.filter(function (c)
                //{
                //    return s.ChartOfAccountId === data.AccountId;
                //});
            
                //if (accountCharts.length > 0)
                //{
                //    data.ChartOfAccountObject = accountCharts[0];
                //}

                $scope.purchaseOrder = rec;
                $scope.purchaseOrder.ExpectedDeliveryDate = rec.ExpectedDeliveryDateStr;
                $scope.header = 'Update Purchase Order';
                $scope.newEdit = true;
            };

            $scope.getPurchaseOrder = function (id)
            {
                if (id < 1)
                {
                    alert('Invalid selection!');
                    return;
                }
                purchaseOrderServices.getPurchaseorder(id, getPurchaseOrderCompleted);
            };

            function getOrderDetailsCompleted(rec)
            {
                if (rec == null || rec.PurchaseOrderId < 1)
                {
                    alert('Invalid selection!');
                    return;
                }

                angular.forEach(rec.PurchaseOrderItemObjects, function (k, g)
                {
                    k.TotalCost = k.QuantityOrdered * k.CostPrice;
                });
                
                $scope.rec =
                  {
                      referenceCode: rec.PurchaseOrderNumber,
                      dateGenerated: rec.DateTimePlacedStr,
                      expectedDate: rec.ExpectedDeliveryDateStr,
                      supplier: rec.SupplierObject.CompanyName,
                      storeAddress: $rootScope.store.StoreAddress,
                      receiptItems: rec.PurchaseOrderItemObjects,
                      totalCost: rec.DerivedTotalCost
                  };
               
                $scope.purchaseOrder = rec;
                $scope.purchaseOrder.ExpectedDeliveryDate = rec.ExpectedDeliveryDateStr;
                //StoreItemStockId: item.StoreItemStockId,
                $scope.details = true;
            };

            $scope.getOrderDetails = function (id)
            {
                if (id < 1)
                {
                    alert('Invalid selection!');
                    return;
                }
                purchaseOrderServices.getPurchaseorder(id, getOrderDetailsCompleted);
            };

            $scope.addProduct = function (item)
           {
                if (item.StoreItemStockId < 1)
               {
                   $scope.setError('Please select a product');
                   return;
               }

               var orderItem =
               {
                   TempId: 1,
                   PurchaseOrderItemId: item.PurchaseOrderItemId,
                   PurchaseOrderId: 0,
                   StoreItemStockId: item.StoreItemStockId,
                   SerialNumber: item.SKU,
                   CostPrice: item.CostPrice,
                   QuantityOrdered: 1,
                   TotalCost: item.QuantityOrdered * item.CostPrice,
                   QuantityDelivered: item.QuantityDelivered,
                   ImagePath : item.ImagePath,
                   StoreItemName : item.StoreItemName,
                   StatusCode: 1,
                   DeliveryStatus : 'Pending'
               };
              
               if ($scope.purchaseOrder.PurchaseOrderItemObjects.length > 0)
               {
                   var matchFound = false;
                   angular.forEach($scope.purchaseOrder.PurchaseOrderItemObjects, function(o, k)
                   {
                       if (o.StoreItemStockId === orderItem.StoreItemStockId)
                       {
                           o.QuantityDelivered += orderItem.QuantityDelivered;
                           matchFound = true;
                       }

                   });

                   if (!matchFound)
                   {
                       orderItem.TempId = $scope.purchaseOrder.PurchaseOrderItemObjects.length + 1;
                       $scope.purchaseOrder.PurchaseOrderItemObjects.push(orderItem);
                  }
               }
               else
               {
                   orderItem.TempId = 1;
                   $scope.purchaseOrder.PurchaseOrderItemObjects.push(orderItem);
               }

               $scope.updateAmount();
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
     
           $scope.removeOrderItem = function (id)
           {
               if (id < 1)
               {
                   $scope.setError('Invalid selection');
                   return;
               }

               angular.forEach($scope.purchaseOrder.PurchaseOrderItemObjects, function (x, y)
               {
                   if (x.TempId === id && x.StatusCode < 2)
                   {
                       if (!confirm("This Item will be removed from the list. Continue?"))
                       {
                           return;
                       }

                       if (x.PurchaseOrderItemId > 0)
                       {
                           var promise = purchaseOrderServices.deleteProduct(x.PurchaseOrderItemId);
                           promise.then(function (val)
                           {
                               $rootScope.busy = false;
                               alert(val.Error);
                               if (val.Code < 1)
                               {
                                   return;
                               }
                               $scope.purchaseOrder.PurchaseOrderItemObjects.splice(y, 1);
                               $scope.updateAmount();

                           });
                       }
                       else
                       {
                           $scope.purchaseOrder.PurchaseOrderItemObjects.splice(y, 1);
                           $scope.updateAmount();
                       }
                   }
                  
               });

           };

           $scope.processPurchaseOrder = function ()
           {
               if ($scope.purchaseOrder.SupplierObject == null || $scope.purchaseOrder.SupplierObject.SupplierId < 1)
               {
                   $scope.setError('Please select a supplier.');
                   return;
               }

               if ($scope.purchaseOrder.ChartOfAccountObject == null || $scope.purchaseOrder.ChartOfAccountObject.ChartOfAccountId < 1)
               {
                   $scope.setError('Please select a chart of account.');
                   return;
               }

               if ($scope.purchaseOrder.ExpectedDeliveryDateStr == null || $scope.purchaseOrder.ExpectedDeliveryDateStr.length < 1)
               {
                   $scope.setError('Please provide the expected delivery date for this purchase order.');
                   return;
               }

               if ($scope.purchaseOrder.PurchaseOrderItemObjects == null || $scope.purchaseOrder.ChartOfAccountObject.ChartOfAccountId < 1)
               {
                   $scope.setError('Please select add at least on product.');
                   return;
               }

               $scope.purchaseOrder.AccountId = $scope.purchaseOrder.ChartOfAccountObject.ChartOfAccountId;
               $scope.purchaseOrder.SupplierId = $scope.purchaseOrder.SupplierObject.SupplierId;
               $scope.purchaseOrder.ExpectedDeliveryDate = $scope.purchaseOrder.ExpectedDeliveryDateStr;


               angular.forEach($scope.purchaseOrder.PurchaseOrderItemObjects, function(p, t) 
               {
                   if (p.StoreItemId < 1)
                   {
                       $scope.setError('product information is invalid.');
                       return;
                   }

                   if (p.CostPrice < 1)
                   {
                       $scope.setError('Please product cost price');
                       return;
                   }

                   if (p.QuantityOrdered < 1) 
                   {
                       alert('please provide order quantity for the product(s)');
                   }
               });

                $scope.purchaseOrder.ExpectedDeliveryDate = $scope.purchaseOrder.ExpectedDeliveryDateStr;
           
                var date1 = new Date($scope.purchaseOrder.ExpectedDeliveryDate);
                var year1 = date1.getFullYear();
                var month1 = date1.getMonth() + 1;
                var day1 = date1.getDate();

                var date2 = new Date();
                var year2 = date2.getFullYear();
                var month2 = date2.getMonth() + 1;
                var day2 = date2.getDate();

                var dateGenerated = day2 + '/' + month2 + '/' + year2;
               
                var expectedDeliveryDate = day1 + '/' + month1 + '/' + year1;

                $scope.rec =
                   {
                       referenceCode: '',
                       dateGenerated: dateGenerated,
                       expectedDate: expectedDeliveryDate,
                       supplier: $scope.purchaseOrder.SupplierObject.CompanyName,
                       storeAddress: $rootScope.store.StoreAddress,
                       receiptItems: $scope.purchaseOrder.PurchaseOrderItemObjects,
                       totalCost: $scope.purchaseOrder.DerivedTotalCost
                   };

                $scope.processing = true;

                if ($scope.purchaseOrder.PurchaseOrderId < 1)
                {
                    purchaseOrderServices.addpurchaseOrder($scope.purchaseOrder, $scope.processPurchaseOrderCompleted);
                }
                else
                {
                    purchaseOrderServices.editPurchaseorder($scope.purchaseOrder, $scope.processPurchaseOrderCompleted);
                }
           };
        
           $scope.processPurchaseOrderCompleted = function (data)
           {
               $scope.processing = false;

               if (data.Code < 1)
               {
                  $scope.setError(data.Error);
                   return;
               }
               else
               {
                   $scope.rec.referenceCode = data.PurchaseOrderNumber;
                   $scope.printLpo();
                   $scope.setSuccessFeedback(data.Error);
                   $scope.initialisePurchaseOrder();
                   $scope.ngTable.fnClearTable();
               }
            };
     
           $scope.updateAmount = function ()
           {
               var totalAmount = 0;

               angular.forEach($scope.purchaseOrder.PurchaseOrderItemObjects, function (t, k)
               {
                   if (t.CostPrice < 1 || t.QuantityOrdered < 1)
                   {
                       return;
                   }

                   var qt = parseFloat(t.QuantityOrdered);
                   var pr = parseFloat(t.CostPrice);

                   if (!isNaN(qt) && qt > 0 && !isNaN(pr) && pr > 0) 
                   {
                       t.TotalCost = qt * pr;
                       totalAmount += t.TotalCost;
                   }
               });

               $scope.purchaseOrder.DerivedTotalCost = totalAmount;
           };

           $scope.determinePrintOption = function () {
               if ($scope.setPrintOption === undefined || $scope.setPrintOption === null || $scope.setPrintOption < 1) {
                   alert('Please select a print option');
                   return;
               }

               if ($scope.setPrintOption === 1) {
                   $scope.printLpo($scope.rec);
               }
               else {
                   $scope.printWithLowerDimensions($scope.rec);
               }

           };

           $scope.printLpo = function ()
           {
               var rec = $scope.rec;

               var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
                   + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>'
                   + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
                   + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website: ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
                   + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                   + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="width: 70%; text-align: center; margin-left: 40%"><b style="border-bottom: 1px solid #000;">PURCHASE ORDER</h3></b></td></tr>'
                   + '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date Generated: ' + rec.dateGenerated + '</td></tr>'
                   + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Expected Delivery Date: ' + rec.expectedDate + '</td></tr>'
                   + '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">LPO No.: ' + rec.referenceCode + '</td></tr>';

               contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Supplier: ' + rec.supplier + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;"></td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px; margin-right: 3%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>'
               + '</td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Grand Total Cost (' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right"><b>' + filterCurrency(rec.totalCost, '') + '</b></td></tr></table>';
             
               //+ '<td style="width: 100%;padding: 1px;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               //             + '<tr style="font-size:1.1em;"><td  style=";margin-bottom: 4px;"><h5><b style="border-bottom: 1px solid #000">SALES INVOICE</b></h5></td><td></td></tr>'

               contents += '<div class="col-md-12 divlesspadding" style="border-top: #000 solid 1px;">' +
                   '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                      '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                       '<th style="color: #008000;font-size:0.9em; width:20%;">Qty. Ordered</th><th style="color: #008000;font-size:0.9em; width:20%;">Cost(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                        '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

               angular.forEach(rec.receiptItems, function (item, i)
               {
                 contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + item.StoreItemName + '</td><td>' + item.QuantityOrdered + '</td><td>' + filterCurrency(item.CostPrice, '') + '</td>' +
                       '<td>' + filterCurrency(item.TotalCost, " ") + '</td></tr>';
               });

               contents += '</tbody></table></div>' +
                   '<table style="width: 100%;font-size:0.9em"><tr><td><h5 style="float:left; text-align:left"></h5></td><td><h5 style="float:right; text-align:right; margin-right:11%"></h5></td>' +
                   '</tr><tr><td style="vertical-align: top; "></td><td>' +
                   '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;">';
               

               contents += '</table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                    '<h5>Powered by: www.shopkeeper.ng</b></h5></div></div>';

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
                           + '<title>LPO</title>');
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
                        + '<title>LPO</title>');
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
               $scope.initialisePurchaseOrder();
               return false;
           };

           $scope.printWithLowerDimensions = function (rec) {
               //<img style="width: 60px; height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '"/>

               //<h3 class="ng-binding" style="width: 100%; text-align: center; font-size:1.5em">' + $rootScope.store.StoreName + '</h3>

               var contents = '<table style="width: 100%; margin-left: 0px; border: none; color: #000; font-weight: bold" class="table"><tr style="border: none">'
                            + '<td style="width: 100%"><img style="height: 60px; float: left; margin-left:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/></td></tr>'
                            + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">' + $rootScope.store.StoreAddress + '</td></tr>'
                            + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Website:  ' + $rootScope.store.Url + '</td></tr>'
                            + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Email: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.8em">'
                            + '<td style="width: 100%;padding: 1px;margin-bottom: 10px; text-align: center">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                            + '<tr style="font-size:1.3em;"><td style="margin-bottom: 2px;"><h4><b style="border-bottom: 1px solid #000; text-align: center; margin-left:20%">SALES INVOICE</b></h5></td></tr>'
                            + '</tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Date: ' + rec.dateGenerated + '</td></tr>'
                            + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">LPO No.: ' + rec.referenceCode + + '</td></tr>';

               contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Supplier: ' + rec.supplier + '</td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">Grand Total Cost (' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td style="text-align: right"><b>' + filterCurrency(rec.totalCost, '') + '</b></td></tr></table>';
               
               contents += 
                  '<table class="table" role="grid" style="width: 100%;font-size:0.9em; font-weight: bold; margin-left: 0px;border-bottom: 1px solid #000">' +
                     '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                      '<th style="color: #008000;font-size:0.9em; width:20%;">Qty. Ordered</th><th style="color: #008000;font-size:0.9em; width:20%;">Cost(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                       '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';


               angular.forEach(rec.receiptItems, function (item, i)
               {
                   contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td>' + item.StoreItemName + '</td><td>' + item.QuantityOrdered + '</td><td>' + filterCurrency(item.CostPrice, '') + '</td>' +
                         '<td>' + filterCurrency(item.TotalCost, " ") + '</td></tr>';
               });
               
               contents += '</tbody></table>' +
                   '<table style="width: 100%; font-weight: bold; margin-left: 0px;">';

               contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
                   '</div><h5>Powered by: www.shopkeeper.ng</h5></div>';


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
                           + '<title>LPO</title>');
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
                   });
               }
               else {
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                        + '<title>LPO</title>');
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

               $scope.initialisePurchaseOrder();
               return false;
           };
    }]);
   
});

