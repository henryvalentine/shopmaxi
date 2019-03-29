"use strict";

define(['application-configuration', 'purchaseOrderServices', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{

    app.register.directive('ngPorderReceipts', function ($compile)
    {
        return function ($scope, ngPorderReceipts)
        {
            var tableOptions = {};
            if ($scope.isAdmin === true) {
                tableOptions.sourceUrl = "/Purchaseorder/GetPurchaseOrders";
            }
            else {
                if ($scope.isPurchaser === true) {
                    tableOptions.sourceUrl = "/Purchaseorder/GetMyPurchaseOrders";
                } else {
                    return;
                }
            }

            tableOptions.itemId = 'PurchaseOrderTableId'; 
            tableOptions.columnHeaders = ["PurchaseOrderNumber", "SupplierName", "GeneratedByEmployeeName", "DerivedTotalCostStr", "DateTimePlacedStr", 'ActualDeliveryDateStr', "DeliveryStatus"];
            var ttc = purchaseOrderReceptionTableManager($scope, $compile, ngPorderReceipts, tableOptions, 'getOrderDetails');
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };
    });

  
    app.register.controller('purchaseOrderReceiptsController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'purchaseOrderServices','$upload', '$timeout', 'fileReader',

        function (ngDialog, $scope, $rootScope, $routeParams, purchaseOrderServices, $upload, $timeout, fileReader)
        {
            $scope.isAdmin = $rootScope.isAdmin;
            $scope.isPurchaser = $rootScope.isPurchaser;

            $scope.initializeInvoiceDate = function ()
            {
                //Expiry Date
                setExpiryDate($scope, '', '');
            };
            
            $scope.initializeInvoiceDate = function ()
            {
                var i = new Date();
                var year = i.getFullYear();
                var month = i.getMonth() + 1;
                var day = i.getDate();
                var minInvoiceDate = year + '/' + month + '/' + day;

                //Date Received for invoice
                setControlDate($scope, '', minInvoiceDate);
            };

            $scope.initializeReceiptdate = function ()
            {
                var xcvb = new Date();
                var year = xcvb.getFullYear();
                var month = xcvb.getMonth() + 1;
                var day = xcvb.getDate();
                var minDate = year + '/' + month + '/' + day;

                //Date Received for products
                setControlDate($scope, '', minDate);
            };

            $scope.opendelivery = function ($event, newOrder)
            {
                newOrder.isOpen = true;
            };

            $scope.opendeliveryExp = function ($event, newOrder)
            {
                newOrder.isExpOpen = true;
            };

            $scope.openInvoiceDate = function ($event, invoice)
            {
                invoice.isOpened = true;
            };
          
            $scope.newOrder = function ()
            {
                $scope.initialisePurchaseOrder();
                $scope.newEdit = true;
            };

            $scope.editReceivedOrder = function (d, k)
            {
                if (d === undefined || d == null || k === undefined || k === null)
                {
                    alert('Invalid selection!');
                    return;
                }

                var quantityDelivered = 0;
                angular.forEach(d.PurchaseOrderItemDeliveryObjects, function (s, x)
                {
                    quantityDelivered += s.QuantityDelivered;
                });

                var balance = d.QuantityOrdered - quantityDelivered;

                //alert(d.QuantityOrdered - quantityDelivered);

                $scope.newOrderReceipt =
                {
                    PurchaseOrderId: d.PurchaseOrderId,
                    PurchaseOrderItemId: k.PurchaseOrderItemId,
                    isOpen: false,
                    isExpOpen: false,
                    balance: balance,
                    isClosed: true,
                    QuantityDelivered: k.QuantityDelivered,
                    ReceivedById: k.ReceivedById,
                    ExpiryDate: k.ExpiryDateStr,
                    ExpiryDateStr: d.ExpiryDateStr,
                    StoreItemName: d.StoreItemName,
                    DateDelivered: k.DateDeliveredStr,
                    DateDeliveredStr: k.DateDeliveredStr,
                    PurchaseOrderItemDeliveryId: k.PurchaseOrderItemDeliveryId,
                    ExpectedDeliveryDate: d.ExpectedDeliveryDate,
                    ExpectedDeliveryDateStr: k.ExpectedDeliveryDateStr,
                    TempId: $scope.tempId,
                    QuantityOrdered: d.QuantityOrdered,
                    TotalQuantityDelivered: d.TotalQuantityDelivered,
                    receptionHeader: 'Update GRN'
                };
                    
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/PurchaseOrderReceipts/ProcessPurchaseOrderReceipt.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
              
            };
            
            $scope.initialisePurchaseOrder = function () 
            {
                $scope.newEdit = false;
                $scope.details = false;
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
                    PurchaseOrderItemObjects: []
                };
          };

            function getOrderDetailsCompleted(data)
            {
                if (data == null || data.PurchaseOrderId < 1)
                {
                    alert('Invalid selection!');
                    return;
                }
                
                var tempId = 1;
                
                angular.forEach(data.PurchaseOrderItemObjects, function (k, g)
                {
                    if (k.StatusCode < 3)
                    {
                        k.IsComplete = false;
                        k.TempId = tempId;
                        $scope.complete = false;
                        tempId++;
                    }

                    k.TotalCost = k.QuantityOrdered * k.CostPrice;
                });

                $scope.originalVatAmount = data.VATAmount;
                $scope.originalDiscountAmount = data.DiscountAmount;
                $scope.originalFobAmount = data.FOB;
                
                $scope.purchaseOrder = data;

                $scope.purchaseOrder.LandingCost = (data.DerivedTotalCost + data.VATAmount + data.FOB) - data.DiscountAmount;

                $scope.deliveries = [];
                $scope.initializeInvoiceDate();
                $scope.initializeReceiptdate();

                $scope.newEdit = true;
            };

            $scope.getOrderDetails = function (id)
            {
                if (id < 1)
                {
                    alert('Invalid selection!');
                    return;
                }
                purchaseOrderServices.getPurchaseorderDetails(id, getOrderDetailsCompleted);
            };
            
            $scope.addDelivery = function (newOrder)
            {
                if (newOrder == null)
                {
                    return;
                }

                if (newOrder.DateDelivered == null || newOrder.DateDelivered === undefined || newOrder.DateDelivered.length < 1)
                {
                    return;
                }

                if (newOrder.QuantityDelivered == null || newOrder.QuantityDelivered === undefined || newOrder.QuantityDelivered.length < 1)
                {
                    return;
                }

                var totalQuantityDelivered = newOrder.TotalQuantityDelivered + newOrder.QuantityDelivered;
                if ((totalQuantityDelivered > newOrder.QuantityOrdered))
                {
                    newOrder.QuantityDelivered = '';
                    alert('The quantity received must not be more than quantity ordered.');
                    return;
                }
                
                var dtt = new Date(newOrder.DateDelivered);
                var y = dtt.getFullYear();
                var m = dtt.getMonth();
                var dd = dtt.getDate();
                var date = y + '/' + m + '/' + dd;

                newOrder.DateDeliveredStr = date;

                if (newOrder.ExpiryDate !== undefined && newOrder.ExpiryDate !== null && newOrder.ExpiryDate.length > 0)
                {
                    var dtt2 = new Date(newOrder.ExpiryDate);
                    var y2 = dtt2.getFullYear();
                    var m2 = dtt2.getMonth();
                    var dd2 = dtt2.getDate();
                    var date2 = y2 + '/' + m2 + '/' + dd2;
                    newOrder.ExpiryDateStr = date2;
                }

                if ($scope.deliveries.length > 0)
                {
                    var matchFound = false;
                    angular.forEach($scope.deliveries, function (s, x)
                    {
                        
                        if (s.TempId === newOrder.TempId && newOrder.DateDelivered === s.DateDelivered)
                        {
                            if (newOrder.PurchaseOrderItemDeliveryId > 0 && s.PurchaseOrderItemDeliveryId === newOrder.PurchaseOrderItemDeliveryId)
                            {
                                s.QuantityDelivered += newOrder.QuantityDelivered;
                                s.DateDelivered = newOrder.DateDelivered;
                                s.DateDeliveredStr = newOrder.DateDeliveredStr;
                                s.ExpiryDate = newOrder.ExpiryDate;
                                s.Price = newOrder.Price;
                                s.ItemPriceId = newOrder.ItemPriceId;
                                s.MinimumQuantity = newOrder.MinimumQuantity;
                                s.ExpiryDateStr = newOrder.ExpiryDateStr;
                                matchFound = true;
                            }
                            else
                            {
                                s.QuantityDelivered += newOrder.QuantityDelivered;
                                s.DateDelivered = newOrder.DateDelivered;
                                s.DateDeliveredStr = newOrder.DateDeliveredStr;
                                s.Price = newOrder.Price;
                                s.ItemPriceId = newOrder.ItemPriceId;
                                s.MinimumQuantity = newOrder.MinimumQuantity;
                                s.ExpiryDate = newOrder.ExpiryDate;
                                s.ExpiryDateStr = newOrder.ExpiryDateStr;
                                matchFound = true;
                            }
                            
                        }
                    });

                    if (matchFound === false)
                    {
                       $scope.deliveries.push(newOrder);
                    }
                }
                else
                {
                    $scope.deliveries.push(newOrder);
                }

                angular.forEach($scope.purchaseOrder.PurchaseOrderItemObjects, function (k, g)
                {
                    if (newOrder.PurchaseOrderItemId === k.PurchaseOrderItemId)
                    {
                        k.TotalQuantityDelivered += newOrder.QuantityDelivered;
                        k.Price = newOrder.Price;

                        if (k.TotalQuantityDelivered === newOrder.QuantityOrdered)
                        {
                            k.StatusCode = 3;
                            k.IsComplete = true;
                        }
                   
                        if (k.PurchaseOrderItemDeliveryObjects === null || k.PurchaseOrderItemDeliveryObjects === undefined || k.PurchaseOrderItemDeliveryObjects.length < 1)
                        {
                            k.PurchaseOrderItemDeliveryObjects = [newOrder];
                        }
                        else
                        {
                            var newMatch = false;
                            angular.forEach(k.PurchaseOrderItemDeliveryObjects, function (s, x)
                            {

                                if (s.TempId === newOrder.TempId && newOrder.DateDelivered === s.DateDelivered)
                                {
                                    if (newOrder.PurchaseOrderItemDeliveryId > 0 && s.PurchaseOrderItemDeliveryId === newOrder.PurchaseOrderItemDeliveryId)
                                    {
                                        s.QuantityDelivered += newOrder.QuantityDelivered;
                                        s.DateDelivered = newOrder.DateDelivered;
                                        s.DateDeliveredStr = newOrder.DateDeliveredStr;
                                        s.ExpiryDate = newOrder.ExpiryDate;
                                        s.ExpiryDateStr = newOrder.ExpiryDateStr;
                                        newMatch = true;
                                    }
                                    else
                                    {
                                        s.QuantityDelivered += newOrder.QuantityDelivered;
                                        s.DateDelivered = newOrder.DateDelivered;
                                        s.DateDeliveredStr = newOrder.DateDeliveredStr;
                                        s.ExpiryDate = newOrder.ExpiryDate;
                                        s.ExpiryDateStr = newOrder.ExpiryDateStr;
                                        newMatch = true;
                                    }

                                    
                                }
                               
                            });
                            
                            if(newMatch === false)
                            {
                                k.PurchaseOrderItemDeliveryObjects.push(newOrder);
                            }
                        }
                    }
                
                });

                ngDialog.close('/ng-shopkeeper/Views/Store/PurchaseOrderReceipts/ProcessPurchaseOrderReceipt.html', '');
            };
            
            $scope.addNewOrder = function (pOrder)
            {
                if (pOrder == null || pOrder.PurchaseOrderId < 1)
                {
                    alert('Invalid selection!');
                    return;
                }

                $scope.newOrderReceipt =
                    {
                        PurchaseOrderId: pOrder.PurchaseOrderId,
                        PurchaseOrderItemId: pOrder.PurchaseOrderItemId,
                        isOpen: false,
                        isExpOpen: false,
                        isClosed: true,
                        QuantityDelivered: '',
                        ReceivedById: '',
                        ExpiryDate: '',
                        ExpiryDateStr: '',
                        DateDelivered: '',
                        DateDeliveredStr: '',
                        UoMCode: pOrder.UoMCode,
                        Price: pOrder.Price,
                        MinimumQuantity: pOrder.MinimumQuantity,
                        balance: pOrder.QuantityOrdered - pOrder.TotalQuantityDelivered,
                        StoreItemName: pOrder.StoreItemName,
                        PurchaseOrderItemDeliveryId: 0,
                        ExpectedDeliveryDate: pOrder.ExpectedDeliveryDate,
                        ExpectedDeliveryDateStr: new Date(pOrder.ExpectedDeliveryDate),
                        TempId: pOrder.TempId,
                        QuantityOrdered: pOrder.QuantityOrdered,
                        TotalQuantityDelivered: pOrder.TotalQuantityDelivered,
                        receptionHeader: 'Add GRN'
                    };

                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/PurchaseOrderReceipts/ProcessPurchaseOrderReceipt.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
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
     
           $scope.removeOrderReceiptItem = function (purchaseOrderItem, id)
           {
               if (id < 1)
               {
                   $scope.setError('Invalid selection');
                   return;
               }

               angular.forEach(purchaseOrderItem.PurchaseOrderItemDeliveryObjects, function (x, y)
               {
                   if (x.TempId === id && purchaseOrderItem.StatusCode < 2)
                   {
                       if (!confirm("This Item will be removed from the list. Continue?"))
                       {
                           return;
                       }

                       if (x.PurchaseOrderItemDeliveryId > 0)
                       {
                           var promise = purchaseOrderServices.deleteOrderItemReceipt(x.PurchaseOrderItemDeliveryId);
                           promise.then(function (val)
                           {
                               $rootScope.busy = false;
                               alert(val.Error);
                               if (val.Code < 1)
                               {
                                   return;
                               }
                               purchaseOrderItem.splice(y, 1);
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

           $scope.processPurchaseOrderDelivery = function ()
           {
               if ($scope.deliveries == null || $scope.deliveries.length < 1)
               {
                   $scope.setError('Please provide the required fields and try again.');
                   return;
               }
               
               $scope.processing = true;
               var delivery = {VATAmount: $scope.purchaseOrder.VATAmount, DiscountAmount: $scope.purchaseOrder.DiscountAmount, FOB: $scope.purchaseOrder.FOB, DeliveredItems: $scope.deliveries };
               purchaseOrderServices.processPurchaseorderDelivery(delivery, $scope.processPurchaseOrderCompleted);

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
                   $scope.ngTable.fnClearTable();
                   alert(data.Error);
                   $scope.initialisePurchaseOrder();
               }
            };

           $scope.addInvoice = function() {
                $scope.invoice =
                {
                    InvoiceId: 0,
                    ReferenceCode: '',
                    PurchaseOrderId: 0,
                    StatusCode: 0,
                    DueDate: null,
                    DateSent: null,
                    DateSentStr: '',
                    Attachment: '',
                    isOpened : false,
                    TempSource: '/Content/images/noImage.png'
                };
            };

           $scope.editInvoice = function (i)
            {
                if ( i === undefined || i == null || i.InvoiceId < 1)
                {
                    alert('Invalid selection!.');
                    return;
                }

                i.isOpened = false;
                i.TempSource = i.Attachment;
                $scope.invoice = i;
            };

           $scope.setAttachment = function (e)
            {
                var el = (e.srcElement || e.target);
                if (el.files == null)
                {
                    return;
                }
                var file = el.files[0];
                if (file.size > 1024000) {
                    alert('File size must not exceed 4MB');
                    return;
                }

                $scope.file = file;

            };

           $scope.setDateSent = function (e)
            {
               $scope.file = file;

            };

           $scope.ProcessInvoice = function ()
           {
               if (($scope.file == undefined || $scope.file == null) && ($scope.invoice.Attachment.length < 1))
              {
                  alert('Please select invoice.');
                  return;
              }

               if ($scope.invoice.DateSentStr == undefined || $scope.invoice.DateSentStr == null || $scope.invoice.DateSentStr.length < 1)
              {
                  alert('Please select invoice reception date.');
                  return;
              }
               var drStr = moment($scope.invoice.DateSentStr).format('YYYY/MM/DD');
               var cll = drStr.split('/');
               var daySr = cll[2];
               var xdate2 = cll[0] + '/' + cll[1] + '/' + daySr;

               var t = new Date($scope.purchaseOrder.ExpectedDeliveryDateStr);
               var drStr2 = moment(t).format('YYYY/MM/DD');
               
              var url = "/Purchaseorder/ProcessPurchaseorderInvoice?invoiceId="
                  + $scope.invoice.InvoiceId + '&purchaseOrderId=' + $scope.purchaseOrder.PurchaseOrderId + '&referenceCode=' + $scope.invoice.ReferenceCode
                  + '&statusCode=' + $scope.purchaseOrder.StatusCode + '&dueDate=' + drStr2 + '&dateSent=' + drStr + '&attachment=' + $scope.invoice.Attachment;
           
              $rootScope.busy = true;
               $upload.upload({
                   url: url,
                   method: "POST",
                   data: { file: $scope.file}
               })
              .progress(function (evt)
              {
                  $scope.processing = true;
                 
              }).success(function (data)
              {
                  $rootScope.busy = false;
                  $scope.processing = false;
                  if (data.Code < 1)
                  {
                      $scope.setError(data.Error);
                      return;
                  }
                  if ($scope.invoice.InvoiceId < 1)
                  {
                      $scope.invoice.InvoiceId = data.Code;
                      $scope.invoice.Attachment = data.FilePath;
                      $scope.invoice.DateSentStr = xdate2;
                      $scope.purchaseOrder.InvoiceObjects.push($scope.invoice);
                  }
                  else
                  {
                      angular.forEach($scope.purchaseOrder.InvoiceObjects, function (i, k)
                      {
                          if (i.InvoiceId === $scope.invoice.InvoiceId < 1)
                          {
                              i.Attachment = data.FilePath;
                              i.invoice.DateSentStr = xdate2;
                          }
                      });
                  }
                  $scope.setSuccessFeedback(data.Error);
                  $scope.invoice = false;
              });


           };
            
           $scope.updateVatAmount = function (vatAmount)
           {
               if ($scope.purchaseOrder.LandingCost.length < 1)
               {
                   return;
               }

               if (vatAmount > 0)
               {
                   //purchaseOrder.VATAmount  purchaseOrder.FOB
                   var disc = parseFloat(vatAmount);

                   if ($scope.originalVatAmount < disc)
                   {
                       $scope.originalVatAmount = disc;
                       $scope.purchaseOrder.LandingCost += disc;
                   }
                   else
                   {
                       if ($scope.originalFobAmount > disc)
                       {
                           var reconciliator = $scope.originalVatAmount - disc;
                           $scope.purchaseOrder.LandingCost = (($scope.purchaseOrder.LandingCost - reconciliator) + disc).toFixed(2);
                           $scope.originalVatAmount = disc;
                       }
                   }
               }
               else {
                   if ($scope.purchaseOrder.FOB < 1 && $scope.purchaseOrder.DiscountAmount < 1)
                   {
                       $scope.purchaseOrder.LandingCost = $scope.purchaseOrder.DerivedTotalCost;
                       $scope.originalVatAmount = 0;
                       $scope.purchaseOrder.VATAmount = 0;
                   }
               }
           };

           $scope.updateFobAmount = function (fob)
           {
               if ($scope.purchaseOrder.LandingCost.length < 1)
               {
                   return;
               }

               if (fob > 0)
               {
                   
                   var disc = parseFloat(fob);

                   if ($scope.originalFobAmount < disc)
                   {
                       $scope.originalFobAmount = disc;
                       $scope.purchaseOrder.LandingCost += disc;
                   }
                   else
                   {
                       if ($scope.originalFobAmount > disc)
                       {
                           var reconciliator = $scope.originalFobAmount - disc;
                           $scope.purchaseOrder.LandingCost = (($scope.purchaseOrder.LandingCost - reconciliator) + disc).toFixed(2);
                           $scope.originalFobAmount = disc;
                       }
                   }
               }
               else
               {
                   if ($scope.purchaseOrder.VATAmount < 1 && $scope.purchaseOrder.DiscountAmount < 1)
                   {
                       $scope.purchaseOrder.LandingCost = $scope.purchaseOrder.DerivedTotalCost;
                       $scope.originalFobAmount = 0;
                       $scope.purchaseOrder.FOB = 0;
                   }
               }
           };

           $scope.updateDiscountAmount = function (discountAmount)
           {
               if ($scope.purchaseOrder.LandingCost.length < 1)
               {
                   return;
               }

               if (discountAmount > 0)
               {
                   //purchaseOrder.VATAmount  purchaseOrder.FOB
                   var disc = parseFloat(discountAmount);

                   if ($scope.originalDiscountAmount < disc)
                   {
                       $scope.originalDiscountAmount = disc;
                       $scope.purchaseOrder.LandingCost -= disc;
                   }
                   else
                   {
                       if ($scope.originalDiscountAmount > disc)
                       {
                           var reconciliator = $scope.originalDiscountAmount - disc;
                           $scope.purchaseOrder.LandingCost = (($scope.purchaseOrder.LandingCost + reconciliator) - disc).toFixed(2);
                           $scope.originalDiscountAmount = disc;
                       }
                   }
               }
               else
               {
                   if ($scope.purchaseOrder.VATAmount < 1 && $scope.purchaseOrder.FOB < 1)
                   {
                       $scope.purchaseOrder.LandingCost = $scope.purchaseOrder.DerivedTotalCost;
                       $scope.originalDiscountAmount = 0;
                       $scope.purchaseOrder.DiscountAmount = 0;
                   }
               }

           };
          
    }]);
   
});

