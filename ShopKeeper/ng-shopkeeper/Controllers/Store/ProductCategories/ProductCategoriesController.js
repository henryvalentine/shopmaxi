"use strict";

define(['application-configuration', 'productCategoryServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngProductCategoryTable', function ($compile)
    {
        return function ($scope, ngProductCategoryTable)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItemCategory/GetStoreItemCategoryObjects";
                tableOptions.itemId = 'StoreItemCategoryId';
                tableOptions.columnHeaders = ['Name', 'ParentName'];
                var ttc = tableManager($scope, $compile, ngProductCategoryTable, tableOptions, 'New Product Category', 'prepareProductCategoryTemplate', 'getProductCategory', 'deleteProductCategory', 180);
                ttc.removeAttr('width').attr('width', '100%');
                $scope.ttc = ttc;
            }
        };
    });
    app.register.directive("ngProductCatFileSelect", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.file = (e.srcElement || e.target).files[0];
                    $scope.processFile();
                });
            }
        };
    });

    app.register.controller('productCategoryController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productCategoryServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, productCategoryServices, $upload, fileReader, alertsService) {
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        var productCategories = [];
        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.productCategories = [];
            if (productCategories.length < 1)
            {
                productCategoryServices.getProductCategories($scope.getProductCategoriesCompleted);
            }
            else
            {
                $scope.productCategories = productCategories;
            }
            
            $scope.productCategory = new Object();
            var sb = $scope.productCategory;
            sb.ProductCategoryId = 0;
            sb.ParentCategory = new Object();
            sb.ParentCategory.ParentCategoryId = 0;
            sb.ParentCategory.Name = '';
            sb.Name = '';
            sb.Description = '';
            sb.Header = 'Create New Product Category';
            sb.SampleImagePath = '/Content/images/noImage.png';
        };
        
        $scope.prepareProductCategoryTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ProductCategories/ProcessProductCategories.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processProductCategory = function ()
        {
            var productCategory = new Object();
            productCategory.Description = $scope.productCategory.Description;
            productCategory.Name = $scope.productCategory.Name;
            
            if (productCategory.Name == undefined || productCategory.Name.length < 1 || productCategory.Name == null) {
                alert("ERROR: Please provide Product Category Name. ");
                return;
            }

            if ($scope.productCategory.ParentCategory != null)
            {
                productCategory.ParentCategoryId = $scope.productCategory.ParentCategoryId;
            }

            var productCategoryId = parseInt($scope.productCategory.ProductCategoryId);

            if (productCategoryId == undefined || productCategoryId == 0 || productCategoryId < 1)
            {
                productCategoryServices.addProductCategory(productCategory, $scope.processProductCategoryCompleted);
            }
            else
            {
                productCategoryServices.editProductCategory(productCategory, $scope.processProductCategoryCompleted);
            }
        };

        $scope.processProductCategoryCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
            }
            else
            {
                if ($scope.productCategory.ProductCategoryId < 1)
                {
                    alert("Product Category information was successfully added.");
                }
                else
                {
                    alert("Product Category information was successfully updated.");
                }

                ngDialog.close('/ng-shopkeeper/Views/Store/ProductCategories/ProcessProductCategories.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };

        $scope.getProductCategory = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productCategoryServices.getProductCategory(id, $scope.getProductCategoryCompleted);
        };
       
        $scope.getProductCategoryCompleted = function (data)
        {
            if (data.ProductCategoryId < 1)
            {
               alert("ERROR: Product Category information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                $scope.productCategory = data;
                if (data.SampleImagePath != null)
                {
                    $scope.productCategory.SampleImagePath = data.SampleImagePath.replace('~', '');
                } else {
                    $scope.productCategory.SampleImagePath = '/Content/images/noImage.png';
                }
                $scope.productCategory.Header = 'Update Product Category Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ProductCategories/ProcessProductCategories.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.getProductCategoriesCompleted = function (data)
        {
            $scope.productCategories = data;
            productCategories = data;
        };

        $scope.deleteProductCategory = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Product Category information will be deleted permanently. Continue?"))
                {
                    return;
                }
                productCategoryServices.deleteProductCategory(id, $scope.deleteProductCategoryCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteProductCategoryCompleted = function (data)
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
                              $scope.productCategory.SampleImagePath = result;
                          });
            
            $upload.upload({
                url: "/StoreItemCategory/CreateFileSession",
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



