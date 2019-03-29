"use strict";

define(['application-configuration', 'storeItemStockServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader', 'datePicker'], function (app)
{
    app.register.directive('ngEditInventoryTable', function ($compile)
    {
        return function ($scope, ngEditInventoryTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItemStock/GetStoreItemStockObjects";
                tableOptions.itemId = 'StoreItemStockId';
                tableOptions.columnHeaders = ['StoreItemName', 'SKU', 'QuantityInStock', 'ExpiryDate'];
                var ttc = inventoryManager($scope, $compile, ngEditInventoryTable, tableOptions, 'Bulk Edit', 'handleBulkEdit', '/StoreItemStock/GetInventory', 'getSingleEdit', 'getItemDetails', 'deleteInventory', 93);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.ngTable = ttc;
            }
        };
    });
    
    app.register.directive("ngProductStockBulkSelect", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.file = (e.srcElement || e.target).files[0];
                });
            }
        };
    });
    
    app.register.directive("ngVariantImg", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.productVariant.VariantImage = (e.srcElement || e.target).files[0];
                    $scope.variantImgControl = el;
                    $scope.previewVariantImage();
                });
            }
        };
    });
    
    app.register.directive("ngSessImg", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.sessObj.Img = (e.srcElement || e.target).files[0];
                    $scope.sessImgControl = el;
                    $scope.sessObj.FileObj = $scope.sessObj.Img;
                    $scope.previewSessObjImage();
                });
            }
        };
    });

    app.register.controller('editInventoryController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeItemStockServices', '$upload', 'fileReader', '$timeout', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeItemStockServices, $upload, fileReader, $timeout, alertsService)
    {
        $rootScope.applicationModule = "EditInventory";
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.initializeComponents = function ()
        {
            $scope.details = false;
            $scope.edit = false;
            $scope.moreImages = false;
            $scope.productVariant = {
                'productVariation': { 'StoreItemVariationId': '', 'VariationProperty': '-- Select Option --' }, 'unitofMeasurement': { 'UnitOfMeasurementId': '', 'UoMCode': '-Select Option --' }, 'MinimumQuantity': '',
                'productVariationValue': { 'StoreItemVariationValueId': '', 'Value': '-- Select Option--' }, 'Currency': { 'StoreCurrencyId': '', 'Name': '-- Select Currency --' },
                'ImagePath': '', 'ImageResult': '/Content/images/noImage.png', 'SKU': '', 'StoreItem': { 'StoreItemId': '', 'Name': '-- Select Product --' }, 'QuantityInStock': '', 'Price': '',
                'ExpirationDate': '', 'StockImages': [], 'StockUploadObjects': []
           };

            storeItemStockServices.getGenericList2($scope.getGenericListCompleted2);
            $scope.details = false;
            $scope.edit = false;
            $scope.moreImages = false;
            $scope.initializeSessObj();
        };
        
        $scope.initializeSessObj = function () 
        {
            $scope.sessObj =
            {
                'StockUploadId': '',
                'ImageViewId': '',
                'ImageView': { 'ImageViewId': '', 'Name': '-- Select Image View --' },
                'FileId': '',
                'TempId': '',
                'ImagePath': '/Content/images/noImage.png',
                'FileObj': '',
                'ViewName': '',
                'StoreItemStockId': '',
                'LastUpdated': ''
            };

            $scope.buttonStatus = 1;
            $scope.buttonText = 'Add Image';
        };

        var xcvb = new Date();
        var year = xcvb.getFullYear();
        var month = xcvb.getMonth() + 1;
        var day = xcvb.getDate();
        var miniDate = year + '/' + month + '/' + day;
        
        setControlDate($scope, miniDate, '');
        
        $scope.getGenericListCompleted2 = function (data)
        {
            genericListObject.storeItemVariations = [];
            genericListObject.storeItemVariationValues = [];
            genericListObject.storeCurrencies = [];
            genericListObject.imageViews = [];
            genericListObject.unitsofMeasurement = [];
            
            genericListObject.storeItemVariations = data.StoreItemVariations;
            genericListObject.storeItemVariationValues = data.StoreItemVariationValues;
            genericListObject.storeCurrencies = data.StoreCurrencies;
            genericListObject.unitsofMeasurement = data.UnitsofMeasurement;
            genericListObject.imageViews = data.ImageViews;
            $scope.genericListObject = genericListObject;
        };
       
        $scope.handleBulkEdit = function ()
        {
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/EditInventory/BulkEdit.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
         
        $scope.editInventory = function ()
        {
            var storeItemStock = new Object();
            
            if ($scope.productVariant.productVariation.StoreItemVariationId > 0)
            {
                storeItemStock.StoreItemVariationId = $scope.productVariant.productVariation.StoreItemVariationId;
                if ($scope.productVariant.productVariationValue.StoreItemVariationValueId)
                {
                    storeItemStock.StoreItemVariationValueId = $scope.productVariant.productVariationValue.StoreItemVariationValueId;
                }
                else
                {
                    alert('Please select product Variation Value!');
                    return;
                }
            }
            
            //storeItemStock.UnitOfMeasurementId = $scope.unitofMeasurement.UnitOfMeasurementId;
            //storeItemStock.Price = $scope.productVariant.Price;
            //storeItemStock.Price = $scope.productVariant.Price;
            storeItemStock.QuantityInStock = $scope.productVariant.QuantityInStock;
            storeItemStock.ExpirationDate = $scope.productVariant.ExpirationDate;
            
            //if ($scope.productVariant.Currency.StoreCurrencyId > 0)
            //{
            //    storeItemStock.StoreCurrencyId = $scope.productVariant.Currency.StoreCurrencyId;
            //}

            //if (!$scope.ValidateProps(storeItemStock))
            //{
            //    return;
            //}
            
            if ($scope.productVariant.StockUploadObjects.length > 0)
            {
                storeItemStock.StockUploadObjects = [];
                angular.forEach($scope.productVariant.StockUploadObjects, function (item, index)
                {
                    if (item.TempId > 0)
                    {
                        $upload.upload({
                            url: "/StoreItemStock/CreateFileSession",
                            method: "POST",
                            data: { file: item.FileObj },
                            })
                            .success(function (data)
                            {
                                if (data.Code < 1)
                                {
                                    alert('Inventory Image could not be processed. Please try again later.');
                                }
                                else {
                                    item.FileId = data.Code;
                                    storeItemStock.StockUploadObjects.push(item);
                                    if (storeItemStock.StockUploadObjects.length == $scope.productVariant.StockUploadObjects.length)
                                    {
                                        $scope.processVariant($scope.productVariant, storeItemStock);
                                    }
                                }
                            });
                    }
                    else
                    {
                        storeItemStock.StockUploadObjects.push(item);
                        if (storeItemStock.StockUploadObjects.length == $scope.productVariant.StockUploadObjects.length)
                        {
                            $scope.processVariant($scope.productVariant, storeItemStock);
                        }
                    }
                });
            }
            else
            {
                $scope.processVariant($scope.productVariant, storeItemStock);
                if (storeItemStock.StockUploadObjects.length == $scope.productVariant.StockUploadObjects.length)
                {
                    $scope.processVariant($scope.productVariant, storeItemStock);
                }
            }
            
        };

        $scope.processVariant = function (productVariant, storeItemStock)
        {
            //if (productVariant.VariantImage != null && productVariant.VariantImage.size > 0)
            //{
            //    $upload.upload({
            //        url: "/StoreItemStock/SaveFile",
            //        method: "POST",
            //        data: { file: productVariant.VariantImage },
            //    })
            //    .success(function (data)
            //    {
            //        if (data.code < 1)
            //        {
            //            alert('Product Stock Image could not be processed. Please try again later.');
            //        }
            //        else {
            //            storeItemStock.ImagePath = data.Error;
            //            storeItemStockServices.EditInventory(storeItemStock, $scope.editInventoryCompleted);
            //        }
            //    });
            //}
            //else
            //{
                storeItemStockServices.EditInventory(storeItemStock, $scope.editInventoryCompleted);
            //}
        };

        $scope.ValidateProps = function (storeItemStock)
        {
            if (storeItemStock.UnitOfMeasurementId < 1)
            {
                alert("ERROR: Please select Unit of Measurement.");
                return false;
            }

            if (parseInt(storeItemStock.Price) < 1)
            {
                alert("ERROR: Please providde Price.");
                return false;
            }

            return true;
        };
        
        $scope.editInventoryCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
                return;
            }
            else
            {
                $scope.ngTable.fnClearTable();
                $scope.edit = false;
                //ngDialog.close('/ng-shopkeeper/Views/Store/EditInventory/EditProductStocks.html', '');
                $scope.initializeComponents();
                alert(data.Error);
            }
        };

        $scope.getSingleEdit = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeItemStockServices.getStoreItemStock(id, $scope.getInventoryCompleted);
        };
        
        $scope.getInventoryCompleted = function (data)
        {
            if (data.StoreItemStockId < 1)
            {
                alert("ERROR: Inventory information could not be retrieved! Please try again ");
                return;
            }
            else
            {
                $scope.initializeComponents();
                
                $scope.productVariant.Header = 'Update';
                var imgPath = '';
                if (data.ImagePath != null && data.ImagePath != undefined && data.ImagePath.length > 0)
                {
                    imgPath = data.ImagePath;
                }
                else
                {
                    imgPath = '/Content/images/noImage.png';
                }
               
                $scope.productVariant =
                {
                    'variantId': '', 'productVariation': { 'StoreItemVariationId': data.StoreItemVariationId, 'VariationProperty': data.VariationProperty },
                    'productVariationValue': { 'StoreItemVariationValueId': data.StoreItemVariationValueId, 'Value': data.VariationValue },
                    'ImagePath': imgPath, 'ImageResult': imgPath, 'SKU': data.SKU, 'StoreItem': { 'StoreItemId': data.StoreItemId, 'Name': data.StoreItemName },
                    'StockUploadObjects': [], 'ExpirationDate': data.ExpiryDate, 'QuantityInStock': data.QuantityInStock,
                    'Currency': { 'StoreCurrencyId': data.StoreCurrencyId, 'Name': data.CurrencyName, 'Symbol': data.CurrencySymbol }
                };
                
                // 'Price': data.Price, 'MinimumQuantity': data.MinimumQuantity,
                //'Currency': { 'StoreCurrencyId': data.StoreCurrencyId, 'Name': data.CurrencyName, 'Symbol': data.CurrencySymbol}
                
                if (data.ItemPriceObjects.length > 0)
                {
                    $scope.productVariant.itemPriceList = data.ItemPriceObjects;
                }

                if (data.StockUploadObjects != null && data.StockUploadObjects.length > 0)
                {
                    angular.forEach(data.StockUploadObjects, function (img, i)
                    {
                         var sessObj =
                          {
                              'StockUploadId': img.StockUploadId,
                              'ImageViewId': img.ImageViewId,
                              'ImageView': { 'ImageViewId': img.ImageViewId, 'Name': img.ViewName },
                              'FileId': '',
                              'TempId' : 0,
                              'ImagePath': img.ImagePath,
                              'FileObj': '',
                              'ViewName': img.ViewName,
                              'StoreItemStockId': img.StoreItemStockId,
                              'LastUpdated': img.LastUpdated
                          };

                         $scope.productVariant.StockUploadObjects.push(sessObj);
                    });
                    $scope.sessObj.ImagePath = $scope.productVariant.StockUploadObjects[0].ImagePath;
                }
               
                $scope.sessObj.ImagePath = '/Content/images/noImage.png';
                $scope.edit = true;
            }
        };
        
        $scope.getItemDetails = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeItemStockServices.getStoreItemStock(id, $scope.getItemDetailsCompleted);
        };

        $scope.getItemDetailsCompleted = function (data)
        {
            if (data.StoreItemStockId < 1)
            {
                alert("ERROR: Inventory information could not be retrieved! Please try again ");
                return;
            }
            else {
                $scope.initializeComponents();

                $scope.productVariant.Header = 'Update';
                var imgPath = '/Content/images/noImage.png';
               
                $scope.productVariant =
                {
                    'variantId': '', 'productVariation': { 'StoreItemVariationId': data.StoreItemVariationId, 'VariationProperty': data.VariationProperty },
                    'productVariationValue': { 'StoreItemVariationValueId': data.StoreItemVariationValueId, 'Value': data.VariationValue }, 'CurrencySymbol': data.CurrencySymbol,
                    'ImagePath': imgPath, 'ImageResult': imgPath, 'SKU': data.SKU, 'StoreItem': { 'StoreItemId': data.StoreItemId, 'Name': data.StoreItemName }, 'QuantityInStock': data.QuantityInStock, 'Price': data.Price,
                    'ExpirationDate': data.ExpiryDate, 'ShelfLocation': data.ShelfLocation, 'ReorderQuantity': data.ReorderQuantity, 'ReorderLevel': data.ReorderLevel, 'StockUploadObjects': [],
                    'unitofMeasurement': { 'UnitOfMeasurementId': data.UnitOfMeasurementId, 'UoMCode': data.UoMCode }, 'MinimumQuantity': data.MinimumQuantity,
                }; 
                
                if (data.StoreItemVariationId < 1 || data.StoreItemVariationId == null)
                {
                    $scope.productVariant.productVariationVariationProperty = '';
                    $scope.productVariant.productVariationValue.Value = '';
                }
                
                if (data.StockUploadObjects != null && data.StockUploadObjects.length > 0)
                {
                    angular.forEach(data.StockUploadObjects, function (img, i)
                    {
                        var sessObj =
                         {
                             'StockUploadId': img.StockUploadId,
                             'ImageViewId': img.ImageViewId,
                             'ImageView': { 'ImageViewId': img.ImageViewId, 'Name': img.ViewName },
                             'FileId': '',
                             'ImagePath': img.ImagePath,
                             'FileObj': '',
                             'ViewName': img.ViewName,
                             'StoreItemStockId': img.StoreItemStockId,
                             'LastUpdated': img.LastUpdated
                         };

                        $scope.productVariant.StockUploadObjects.push(sessObj);
                    });
                    
                    $scope.imageViews = true;
                    $scope.preview = {};
                    $scope.preview.ViewName = $scope.productVariant.StockUploadObjects[0].ViewName;
                    if($scope.productVariant.StockUploadObjects.length > 0 )
                    {
                        $scope.preview.ImagePath = $scope.productVariant.StockUploadObjects[0].ImagePath;
                    } else {
                        $scope.preview.ImagePath = '/Content/images/noImage.png';
                    }
                }
                else
                {
                    $scope.imageViews = false;
                }
                $scope.details = true;
            }
        };
        
        $scope.deleteInventory = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Inventory information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storeItemStockServices.deleteStoreItemStock(id, $scope.deleteStoreItemStockCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteStoreItemStockCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                alert(data.Error);
                $scope.ngTable.fnClearTable();
            }
        };
        
        $scope.previewInventoryImage = function ()
        {
            fileReader.readAsDataUrl($scope.productVariant.VariantImage, $scope)
                          .then(function (result)
                          {
                              $scope.productVariant.ImageResult = result;
                              
                          });
        };
        
        $scope.processFile = function (file)
        {
            $upload.upload({
                url: "/StoreItemStock/SaveFile",
                method: "POST",
                data: { file: file },
            })
           .success(function (data)
           {
               if (data.code < 1)
               {
                   alert('Inventory Image could not be processed. Please try again later.');
                   return false;
               }
               else
               {
                   $scope.returnPath = data;
                   return true;
               }
           });
        };
        
        $scope.ProcessBulkEdit = function ()
        {
            if ($scope.file == null)
            {
                alert('Please select bulk Upload file.');
                return;
            }
            
            if ($scope.file.size < 1)
            {
                alert('Please select bulk Upload file.');
                return;
            }
            
            $upload.upload({
                url: "/StoreItemStock/BulkEdit",
                method: "POST",
                data: { file: $scope.file },
            })
           .progress(function (evt)
           {
               $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
           })
           .success(function (data)
           {
               if (data.code < 1)
               {
                   alert('File could not be processed. Please try again later.');
               } else {

                   ngDialog.close('/ng-shopkeeper/Views/Store/EditInventory/BulkEdit.html', '');
                   $scope.ngTable.fnClearTable();
                   alert(data.Error);
               }
           });
        };
        
        $scope.getBulkEditTemplate = function ()
        {
            storeItemStockServices.GetInventory($scope.getBulkEditTemplateCompleted);
        };
        
        $scope.getBulkEditTemplateCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
            }
        };
        
        $scope.previewSessObjImage = function ()
        {
            fileReader.readAsDataUrl($scope.sessObj.Img, $scope)
            .then(function (result)
            {
               $scope.sessObj.ImagePath = result;
            });
        };
        
        $scope.saveSessObj = function ()
        {
            if ($scope.buttonStatus == 1)
            {
                $scope.addSessObj();
            }
            if ($scope.buttonStatus == 2)
            {
                $scope.editSessObj();
            }
        };
        
        $scope.viewSessObj = function (imageViewId)
        {
            if (imageViewId > 0)
            {
                angular.forEach($scope.productVariant.StockUploadObjects, function (item, index)
                {
                    if (item.ImageViewId === parseInt(imageViewId))
                    {
                        $scope.preview = item;
                    }
                });
            }
        };

        $scope.addSessObj = function ()
        {
            if ($scope.sessObj.ImageView.ImageViewId < 1)
            {
                alert('Please select Image View');
                return;
            }
            
            if ($scope.sessObj.FileObj == null || $scope.sessObj.FileObj == undefined)
            {
                alert('Please select an Image');
                return;
            }

            if ($scope.sessObj.FileObj.size < 1)
            {
                alert('Invalid Image');
                return;
            }
            var tempId = 0;
            if ($scope.productVariant.StockUploadObjects.length > 0)
            {
                var tempCol = [];
                tempId = $scope.productVariant.StockUploadObjects.length + 1;
                angular.forEach($scope.productVariant.StockUploadObjects, function (img, i)
                {
                    if (img.ImageViewId == $scope.sessObj.ImageView.ImageViewId)
                    {
                        tempCol.push(img);
                    }
                });

                if (tempCol.length > 0)
                {
                    alert('This Image view has already been added');
                    return;
                }
            } else
            {
                tempId = 1;
            }
             var sessObj =
              {
                  'StockUploadId': '',
                  'ImageViewId': $scope.sessObj.ImageView.ImageViewId,
                  'ImageView': { 'ImageViewId': $scope.sessObj.ImageView.ImageViewId, 'Name': $scope.sessObj.ImageView.Name },
                  'FileId': '',
                  'TempId': tempId,
                  'ImagePath': $scope.sessObj.ImagePath,
                  'FileObj': $scope.sessObj.FileObj,
                  'ViewName': $scope.sessObj.ImageView.Name,
                  'StoreItemStockId': '',
                  'LastUpdated': ''
              };
           
           $scope.productVariant.StockUploadObjects.push(sessObj);
           $scope.initializeSessObj();
           $scope.sessImgControl.val(null);
        };

        $scope.clearSessObjects = function ()
        {
            $scope.productVariant.StockUploadObjects = [];
            $scope.initializeComponents();
            if ($scope.sessImgControl != null)
            {
                $scope.sessImgControl.val(null);
            }
        };
        
        $scope.getSessObj = function (sessObjId)
        {
            if (sessObjId == undefined || sessObjId == NaN || parseInt(sessObjId) < 1)
            {
                alert('Invalid selection');
                return;
            }

            angular.forEach($scope.productVariant.StockUploadObjects, function (item, index)
            {
                if (item.ImageViewId === parseInt(sessObjId))
                {
                    $scope.sessObj = item;
                    $scope.buttonStatus = 2;
                    $scope.buttonText = 'Update Image';
                }
            });
        };

        $scope.editSessObj = function ()
        {
            //if (!$scope.ValidateProps)
            //{
            //    return;
            //}

            angular.forEach($scope.productVariant.StockUploadObjects, function (item, index)
            {
                if (item.ImageViewId == $scope.sessObj.ImageView.ImageViewId)
                {
                    item.ImageViewId = $scope.sessObj.ImageView.ImageViewId;
                    item.ImageView = { 'ImageViewId': $scope.sessObj.ImageView.ImageViewId, 'Name': $scope.sessObj.ImageView.Name };
                    item.Name = $scope.sessObj.ImageView.Name;
                    item.ViewName = $scope.sessObj.ImageView.Name;
                    if ($scope.sessObj.FileObj != null)
                    {
                        item.FileObj = $scope.sessObj.FileObj;
                        item.TempId = $scope.productVariant.StockUploadObjects.length + 1;
                        if ($scope.sessImgControl != null)
                        {
                            $scope.sessImgControl.val(null);
                        }
                        //alert('Temp Image saved');
                    }
                    
                    $scope.initializeSessObj();
                }
            });
        };

        $scope.removeSessObj = function (sessObjId)
        {
            if (sessObjId < 1)
            {
                alert('Invalid selection');
                return;
            }
            angular.forEach($scope.productVariant.StockUploadObjects, function (item, index)
            {
                if (item.ImageViewId === parseInt(sessObjId))
                {
                    if (!confirm("This Image view information will be removed. Continue?"))
                    {
                        return;
                    }
                    
                    if (item.StockUploadId > 0)
                    {
                        storeItemStockServices.deleteStockUpload(item.StockUploadId, $scope.removeSessObjCompleted);
                    }
                    else
                    {
                        $scope.productVariant.StockUploadObjects.splice(index, 1);
                    }
                }
            });
        };

        $scope.removeSessObjCompleted = function(data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
                return;
            }
            angular.forEach($scope.productVariant.StockUploadObjects, function (item, index)
            {
                if (item.ImageViewId === data.Code)
                {
                    $scope.productVariant.StockUploadObjects.splice(index, 1);
                    alert(data.Error);
                }
            });
        };
    }]);
});

var genericListObject = {};

