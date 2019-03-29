"use strict";

define(['application-configuration', 'productTypeServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('producttypeTable', function ($compile)
    {
        return function ($scope, producttypeTable)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItemType/GetStoreItemTypeObjects";
                tableOptions.itemId = 'StoreItemTypeId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableViewManager($scope, $compile, producttypeTable, tableOptions, 'New Product Type', 'prepareStoreItemTypeTemplate', 'getProductType', 'getItemDetails', 'deleteProductType', 153);
                ttc.removeAttr('width').attr('width', '100%');
                $scope.ttc = ttc;
            }
        };
    });
    app.register.directive("ngPtfileSelect", function ()
    {
        return {
            link: function($scope, el) {
                el.bind("change", function (e)
                {
                    $scope.file = (e.srcElement || e.target).files[0];
                    $scope.processFile();
                });
            }
        };
    });

    app.register.controller('productTypeController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productTypeServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, productTypeServices, $upload, fileReader, alertsService)
    {
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.productType = new Object();
            var sb = $scope.productType;
            sb.StoreItemTypeId = 0;
            sb.Name = '';
            sb.Description = '';
            sb.Header = 'Create New Product Type';
            sb.SampleImagePath = '/Content/images/noImage.png';
        };
        
        $scope.prepareStoreItemTypeTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ProductTypes/ProcessProductTypes.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processProductType = function ()
        {
            var productType = new Object();
            productType.Description = $scope.productType.Description;
            productType.Name = $scope.productType.Name;
            //alert($scope.productType.StoreItemTypeId);
            if (productType.Name == undefined || productType.Name.length < 1 || productType.Name == null)
            {
                alert("ERROR: Please provide Product Type Name. ");
                return;
            }
            
            if ($scope.productType.StoreItemTypeId == 0 || $scope.productType.StoreItemTypeId < 1 || $scope.productType.StoreItemTypeId == undefined)
            {
                productTypeServices.addStoreItemType(productType, $scope.processProductTypeCompleted);
            }
            else
            {
                productTypeServices.editStoreItemType(productType, $scope.processProductTypeCompleted);
            }
        };

        $scope.processProductTypeCompleted = function (data)
        {
            if (data.StoreItemTypeId < 1)
            {
                alert(data.Name);

            }
            else
            {
                if ($scope.productType.StoreItemTypeId < 1)
                {
                    alert("Product Type information was successfully added.");
                }
                else
                {
                    alert("Product Type information was successfully updated.");
                }

                ngDialog.close('/ng-shopkeeper/Views/Store/ProductTypes/ProcessProductTypes.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };

        $scope.getProductType = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productTypeServices.getStoreItemType(id, $scope.getProductTypeCompleted);
        };
       
        $scope.getProductTypeCompleted = function (data)
        {
            if (data.StoreItemTypeId < 1)
            {
                alert("ERROR: Product Type information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                $scope.productType = data;
                if (data.SampleImagePath != null)
                {
                    $scope.productType.SampleImagePath = data.SampleImagePath.replace('~', '');
                } else {
                    $scope.productType.SampleImagePath = '/Content/images/noImage.png';
                }
                $scope.productType.Header = 'Update Product Type Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ProductTypes/ProcessProductTypes.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
         };

        $scope.getItemDetails = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productTypeServices.getItemTypeChildren(id, $scope.getItemDetailsCompleted);
        };

        $scope.getItemDetailsCompleted = function (data)
        {
            if (data.length < 1)
            {
                alert("No record found! ");
            }
            else
            {
                $scope.initializeController();
                $scope.productType = data;
                
                $scope.productType.Header = 'Update Product Type Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ProductTypes/ProcessProductTypes.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };

        $scope.deleteProductType = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Product Type information will be deleted permanently. Continue?"))
                {
                    return;
                }
                productTypeServices.deleteStoreItemType(id, $scope.deleteProductTypeCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteProductTypeCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.ttc.fnClearTable();
                alert(data.Error);
            }
        };

        $scope.processFile = function ()
        {
            $scope.progress = 0;
            fileReader.readAsDataUrl($scope.file, $scope)
                          .then(function (result)
                          {
                              $scope.productType.SampleImagePath = result;
                          });
            
            $upload.upload({
                url: "/StoreItemType/CreateFileSession",
                method: "POST",
                data: { file: $scope.file },
            })
           .progress(function (evt)
           {
               $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
           }).success(function (data) {
               if (data.code < 1) {
                   alert('File could not be processed. Please try again later.');
               }
               
           });
        };
        
    }]);
    
});



