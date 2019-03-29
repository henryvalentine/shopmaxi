"use strict";

define(['application-configuration', 'itemPriceServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngItemPriceTable', function ($compile)
    {
        return function ($scope, ngItemPriceTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/ItemPrice/GetItemPriceObjects";
            tableOptions.itemId = 'ItemPriceId';
            tableOptions.columnHeaders = ['StoreItemStockName', 'Price', 'MinimumQuantity', 'UoMCode'];
            var ttc = tableManager($scope, $compile, ngItemPriceTable, tableOptions, 'New Price', 'prepareItemPriceTemplate', 'getItemPrice', 'deleteItemPrice', 100);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
            
        };
    });

    app.register.controller('itemPriceController', ['ngDialog','$scope', '$rootScope', '$routeParams', 'itemPriceServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, itemPriceServices, alertsService)
    {
        $rootScope.applicationModule = "ItemPrice";
        
        $scope.getAuthStatus = function ()
        {
           return $rootScope.isAuthenticated;
       };

       $scope.redir = function ()
       {
           $rootScope.redirectUrl = $location.path();
           $location.path = "/ngy.html#signIn";
       };
        
       $scope.initializeController = function ()
       {
           $scope.initializeModel();
           $scope.setStatusFromStep();
           $scope.itemPrices = [];
           itemPriceServices.getGenericList($scope.getGenericListCompleted);
       };

       $scope.initializeModel = function ()
       {
            $scope.itemPrice = new Object();
            $scope.itemPrice.Price = '';
            $scope.itemPrice.ItemPriceId = 0;
            $scope.itemPrice.MinimumQuantity = '';
            $scope.itemPrice.Remark = '';
            $scope.buttonStatus = 1;
            $scope.buttonText = 'Add Price';
           
            $scope.itemPrice.StoreItemStock =
            {
                'StoreItemStockId': '',
                'StoreItemName': '-- Select Inventory --'
            };

            $scope.itemPrice.unitofMeasurement =
            {
                'UnitOfMeasurementId': '',
                'UoMCode': '-- Select --'
            };
       };

       $scope.setStatusFromStep = function ()
       {
            $scope.clicked = false;
            $scope.edit = false;
        };

       $scope.saveItemPrice = function ()
       {
           if (!$scope.validatateItem($scope.itemPrice))
           {
               return;
           }
           
           if ($scope.buttonStatus == 1)
           {
               $scope.addItemPrice();
           }
           
           if ($scope.buttonStatus == 2)
           {
               $scope.editItemPrice();
           }
           
       };
        
       $scope.addItemPrice = function ()
       {
           var itemPrice = new Object();
           itemPrice.Price = $scope.itemPrice.Price;
           itemPrice.ItemPriceId = 0;
           itemPrice.MinimumQuantity = $scope.itemPrice.MinimumQuantity;
           itemPrice.Remark = $scope.itemPrice.Remark;

           itemPrice.StoreItemStock =
           {
               'StoreItemStockId': $scope.itemPrice.StoreItemStock.StoreItemStockId,
               'StoreItemName': $scope.itemPrice.StoreItemStock.StoreItemName
           };

           itemPrice.unitofMeasurement =
           {
               'UnitOfMeasurementId': $scope.itemPrice.unitofMeasurement.UnitOfMeasurementId,
               'UoMCode': $scope.itemPrice.unitofMeasurement.UoMCode
           };
           var id = 0;
           if ($scope.itemPrices.length < 1) {
               id = 1;
           }
           else {
               id = $scope.itemPrices.length + 1;
           }

           itemPrice.ItemPriceId = id;
           $scope.itemPrices.push(itemPrice);
           $scope.initializeModel();
       };
        
       $scope.editItemPrice = function ()
       {
           angular.forEach($scope.itemPrices, function (itemPrice, index)
           {
               if (itemPrice.ItemPriceId === $scope.itemPrice.ItemPriceId) {
                   itemPrice.Price = $scope.itemPrice.Price;
                   itemPrice.MinimumQuantity = $scope.itemPrice.MinimumQuantity;
                   itemPrice.Remark = $scope.itemPrice.Remark;

                   itemPrice.StoreItemStock =
                   {
                       'StoreItemStockId': $scope.itemPrice.StoreItemStock.StoreItemStockId,
                       'StoreItemName': $scope.itemPrice.StoreItemStock.StoreItemName
                   };

                   itemPrice.unitofMeasurement =
                   {
                       'UnitOfMeasurementId': $scope.itemPrice.unitofMeasurement.UnitOfMeasurementId,
                       'UoMCode': $scope.itemPrice.unitofMeasurement.UoMCode
                   };
                   $scope.initializeModel();
               }
           });
       };
       
       $scope.getGenericListCompleted = function (data)
       {
           $scope.genericList = {};
           $scope.genericList.items = [];
           $scope.genericList.unitsofMeasurement = [];
           $scope.genericList.items = data.Inventories;
           $scope.genericList.unitsofMeasurement = data.UnitsofMeasurement;
          
       };
        
       $scope.prepareItemPriceTemplate = function ()
       {
           $scope.initializeModel();
           $scope.itemPrices = [];
           $scope.itemPrice.Header = 'New Price(s)';
           $scope.clicked = true;
           $scope.buttonStatus = 1;
       };

       $scope.processItemPrice = function ()
       {
           if ($scope.itemPrices.length > 0)
           {
               var priceList = [];
               angular.forEach($scope.itemPrices, function(x, i) 
               {
                   var price = new Object();
                   price.StoreItemStockId = x.StoreItemStock.StoreItemStockId;
                   price.UoMId = x.unitofMeasurement.UnitOfMeasurementId;
                   price.Price = x.Price;
                   price.MinimumQuantity = x.MinimumQuantity;
                   price.Remark = x.Remark;
                   priceList.push(price);

                   if (priceList.length == $scope.itemPrices.length)
                   {
                      itemPriceServices.addPriceList(priceList, $scope.processItemPriceCompleted);
                   }

               });
           }
           else
           {
               var itemPrice = new Object();
               itemPrice.StoreItemStockId = $scope.itemPrice.StoreItemStock.StoreItemStockId;
               itemPrice.UoMId = $scope.itemPrice.unitofMeasurement.UnitOfMeasurementId;
               itemPrice.Price = $scope.itemPrice.Price;
               itemPrice.MinimumQuantity = $scope.itemPrice.MinimumQuantity;
               itemPrice.Remark = $scope.itemPrice.Remark;

               if (!$scope.validatateItem($scope.itemPrice))
               {
                   return;
               }

               if ($scope.itemPrice.ItemPriceId < 1)
               {
                   itemPriceServices.addItemPrice(itemPrice, $scope.processItemPriceCompleted);
               }
               else
               {
                   itemPriceServices.editItemPrice(itemPrice, $scope.processItemPriceCompleted);
               }
           }
       };
       
       $scope.getTempItemPrice = function (itemId)
       {
           if (itemId == undefined || itemId == NaN || parseInt(itemId) < 1)
           {
               alert('Invalid selection');
               return;
           }

           angular.forEach($scope.itemPrices, function (item, index)
           {
               if (item.ItemPriceId === parseInt(itemId))
               {
                   $scope.initializeModel();
                   $scope.itemPrice = item;
                    $scope.buttonStatus = 2;
                    $scope.buttonText = 'Update Price';
               }
           });
       };

       $scope.removeTempItemPrice = function (id)
        {
            if (id < 1)
            {
                alert('Invalid selection');
                return;
            }
            
            angular.forEach($scope.itemPrices, function (itemPrice, index)
            {
                if (itemPrice.ItemPriceId === $scope.itemPrice.ItemPriceId)
                {
                    if (!confirm("This Price information will be removed from the list. Continue?"))
                    {
                        return;
                    }
                    
                    $scope.itemPrices.splice(index, 1);
                    $scope.initializeModel();
                }
            });
        };

       $scope.validatateItem = function (itemPrice)
       {
           if (itemPrice.StoreItemStock.StoreItemStockId == undefined || itemPrice.StoreItemStock.StoreItemStockId < 1)
           {
                alert("ERROR: Please select an Inventory. ");
                return false;
            }

           if (itemPrice.unitofMeasurement.UnitOfMeasurementId == undefined || itemPrice.unitofMeasurement.UnitOfMeasurementId < 1)
            {
                alert("ERROR: Please select Unit of Measurement. ");
                return false;
            }

           if (parseInt(itemPrice.Price) < 1)
            {
                alert("ERROR: Please Provide Price. ");
                return false;
            }

           if (parseInt(itemPrice.MinimumQuantity) < 1)
            {
                alert("ERROR: Please Provide Quantity. ");
                return false;
            }
            return true;
       };
        
       $scope.validatateUpdateItem = function (itemPrice)
       {

           if (itemPrice.StoreItemStockId == undefined || itemPrice.StoreItemStockId < 1)
           {
               alert("ERROR: Please select an Inventory. ");
               return false;
           }

           if (itemPrice.UoMId == undefined || itemPrice.UoMId < 1) {
               alert("ERROR: Please select Unit of Measurement. ");
               return false;
           }
           
           if (parseInt(itemPrice.Price) < 1)
           {
               alert("ERROR: Please Provide Price. ");
               return false;
           }

           if (parseInt(itemPrice.MinimumQuantity) < 1) {
               alert("ERROR: Please Provide Quantity. ");
               return false;
           }
           return true;
       };

       $scope.processItemPriceCompleted = function (data)
       {
           if (data.ItemPriceId < 1)
           {
                alert(data.Error);
                return;
           }
           else
           {
               alert(data.Error);
               if ($scope.edit)
               {
                   ngDialog.close('/ng-shopkeeper/Views/Store/ItemPrice/EditItemPrice.html', '');
               }
                $scope.jtable.fnClearTable();
                $scope.setStatusFromStep();
                $scope.initializeModel();
            }
        };

       $scope.updateItemPrice = function ()
       {
           var price = new Object();
           price.StoreItemStockId = $scope.editItem.StoreItemStock.StoreItemStockId;
           price.UoMId = $scope.editItem.unitofMeasurement.UnitOfMeasurementId;
           price.Price = $scope.editItem.Price;
           price.MinimumQuantity = $scope.editItem.MinimumQuantity;
           price.Remark = $scope.editItem.Remark;
           price.ItemPriceId = $scope.editItem.ItemPriceId;
           if (!$scope.validatateUpdateItem(price))
           {
               return;
           }

           itemPriceServices.editItemPrice(price, $scope.processItemPriceCompleted);
       };

       $scope.updateItemPriceCompleted = function (data)
       {
           if (data.Code < 1)
           {
               alert(data.Error);
               return;
           }
           else
           {
               alert(data.Error);
               $scope.jtable.fnClearTable();
               ngDialog.close('/ng-shopkeeper/Views/Store/ItemPrice/EditItemPrice.html', '');
               $scope.initializeModel();
           }
       };

       $scope.getItemPrice = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           itemPriceServices.getItemPrice(id, $scope.getItemPriceCompleted);
       };

       $scope.getItemPriceCompleted = function (data)
       {
           if (data.ItemPriceId < 1)
           {
             alert(data.Error);
           }
            else
            {
               $scope.editItem = new Object();
               $scope.clicked = false;
               $scope.editItem.Price = data.Price;
               $scope.editItem.ItemPriceId = data.ItemPriceId;
               $scope.editItem.MinimumQuantity = data.MinimumQuantity;
               $scope.editItem.Remark = data.Remark;
               $scope.editItem.Header = 'Update Price';
               $scope.editItem.StoreItemStock =
               {
                   'StoreItemStockId': data.StoreItemStockId,
                   'StoreItemName': data.StoreItemStockName
               };

               $scope.editItem.unitofMeasurement =
               {
                   'UnitOfMeasurementId': data.UoMId,
                   'UoMCode': data.UoMCode
               };
               ngDialog.open({
                   template: '/ng-shopkeeper/Views/Store/ItemPrice/EditItemPrice.html',
                   className: 'ngdialog-theme-flat',
                   scope: $scope
               });
              
            }
       };
        
       $scope.deleteItemPrice = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This Inventory Price information will be deleted permanently. Continue?"))
               {
                   return;
               }
               itemPriceServices.deleteItemPrice(id, $scope.deleteItemPriceCompleted);
           } else {
               alert('Invalid selection.');
           }
       };

       $scope.deleteItemPriceCompleted = function (data)
       {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.jtable.fnClearTable();
                alert(data.Error);
            }
        };
    }]);
   
});

