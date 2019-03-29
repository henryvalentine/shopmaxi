"use strict";

define(['application-configuration', 'productServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngProductTable', function ($compile)
    {
        return function ($scope, ngProductTable)
        {
            var authStatus = $scope.getAuthStatus();
            if (authStatus === false)
            {
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItem/GetStoreItemObjects";
                tableOptions.itemId = 'StoreItemId';
                tableOptions.columnHeaders = ['Name', 'StoreItemCategoryName', 'StoreItemTypeName', 'StoreItemBrandName', 'ParentItemName', 'ChartOfAccountTypeName'];
                var ttc = tableManager($scope, $compile, ngProductTable, tableOptions, 'New Product ', 'prepareProductTemplate', 'getProduct', 'deleteProduct', 117);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.ttc = ttc;
            }
        };
       
    });
    app.register.directive("ngPrtFileSelect", function ()
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
    
    app.register.controller('productController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productServices', '$upload', 'fileReader','$http', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, productServices, $upload, fileReader,$http, alertsService)
    {
       
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
            $scope.alerts = [];
            
            $scope.genericListObject = genericListObject;
            
            $scope.product = { 'StoreItemId': '', 'Name': '', 'Description': '', 'Header': 'Create New Product '};
            
            $scope.product.ProductBrand = { 'StoreItemBrandId': '', 'Name': '-- Select Product Brand --' };
            
            $scope.product.ParentProduct = { 'ParentItemId': '', 'Name': '-- Select Parent Product --' };

            $scope.product.ProductCategory = { 'StoreItemCategoryId': '', 'Name': '-- Select Product Category --' };

            $scope.product.ChartOfAccount = { 'ChartOfAccountId': '', 'AccountType': '-- Select Chart of Account --' };

            $scope.product.ProductType = { 'StoreItemTypeId': '', 'Name': '-- Select Product Type --' };
        };
        
        $scope.prepareProductTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Products/ProcessProducts.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processProduct = function ()
        {
            var product = new Object();
            product.Description = $scope.product.Description;
            product.Name = $scope.product.Name;
            product.StoreItemBrandId = $scope.product.ProductBrand.StoreItemBrandId;
            product.StoreItemCategoryId = $scope.product.ProductCategory.StoreItemCategoryId;
            product.ChartOfAccountId = $scope.product.ChartOfAccount.ChartOfAccountId;
            product.StoreItemTypeId = $scope.product.ProductType.StoreItemTypeId;
            
            if (product.Name == undefined || product.Name.length < 1 || product.Name == null)
            {
                alert("ERROR: Please provide Product  Name.");
                return;
            }
            
            if ($scope.product.ParentProduct != null)
            {
                product.ParentItemId = $scope.product.ParentProduct.StoreItemId;
            }

            if (parseInt(product.StoreItemBrandId) < 1)
            {
                alert("ERROR: Please Product Brand.");
                return;
            }
            
            if (parseInt(product.StoreItemCategoryId) < 1)
            {
                alert("ERROR: Please Product Category.");
                return;
            }
            
            if (parseInt(product.ChartOfAccountId) < 1)
            {
                alert("ERROR: Please Chart of Account.");
                return;
            }
            
            if (parseInt(product.StoreItemTypeId) < 1)
            {
                alert("ERROR: Please Product Type.");
                return;
            }

            if ($scope.product.StoreItemId == null || $scope.product.StoreItemId == '' || $scope.product.StoreItemId == 0 || $scope.product.StoreItemId < 1 || $scope.product.StoreItemId == undefined)
            {
                productServices.addProduct(product, $scope.processProductCompleted);
            }
            else
            {
                productServices.editProduct(product, $scope.processProductCompleted);
            }
        };

        $scope.processProductCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Code);

            }
            else
            {
                alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/Products/ProcessProducts.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };
        
        $scope.getGenericListCompleted = function (data)
        {
            genericListObject.ItemsLoaded = false;
            genericListObject.parentProducts = [];
            genericListObject.productTypes = [];
            genericListObject.productCategories = [];
            genericListObject.productBrands = [];
            genericListObject.chartsOfAccount = [];

            genericListObject.parentProducts = data.Products;
            genericListObject.productTypes = data.ProductTypes;
            genericListObject.productCategories = data.ProductCategories;
            genericListObject.productBrands = data.ProductBrands;
            genericListObject.chartsOfAccount = data.ChartsOfAccount;
            $scope.genericListObject = genericListObject;
            itemsLoaded = true;
        };

        if (!itemsLoaded)
        {
            productServices.getGenericList($scope.getGenericListCompleted);
        }

        $scope.getProduct = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productServices.getProduct(id, $scope.getProductCompleted);
        };
       
        $scope.getProductCompleted = function (data)
        {
            if (data.StoreItemId < 1)
            {
               alert("ERROR: Product  information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                $scope.product = data;

                $scope.product = { 'StoreItemId': data.StoreItemId, 'Name': data.Name, 'Description': data.Description, 'Header': 'Update Product  Information ' };

                $scope.product.ProductBrand = { 'StoreItemBrandId': data.StoreItemBrandId, 'Name': data.StoreItemBrandName };

                var parentItemName = '-- Select Parent Product --';
                
                if (data.ParentItemId != null)
                {
                    parentItemName = data.ParentItemName;
                }

                $scope.product.ParentProduct = { 'ParentItemId': data.StoreItemId, 'Name': parentItemName };

                $scope.product.ProductCategory = { 'StoreItemCategoryId': data.StoreItemCategoryId, 'Name': data.StoreItemCategoryName };

                $scope.product.ChartOfAccount = { 'ChartOfAccountId': data.ChartOfAccountId, 'AccountType': data.ChartOfAccountTypeName };

                $scope.product.ProductType = { 'StoreItemTypeId': data.StoreItemTypeId, 'Name': data.StoreItemTypeName };
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Products/ProcessProducts.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
         };

        $scope.deleteProduct = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Product  information will be deleted permanently. Continue?"))
                {
                    return;
                }
                productServices.deleteProduct(id, $scope.deleteProductCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteProductCompleted = function (data)
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
                              $scope.product.SampleImagePath = result;
                          });
            
            $upload.upload({
                url: "/Product/CreateFileSession",
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
        
        //$scope.refreshProductBrands = function (searchCriteria)
        //{
        //    alert(searchCriteria);
        //    productServices.getProductBrandSearch(searchCriteria, $scope.productBrandSearchCompleted);
        //};
        
        //$scope.productBrandSearchCompleted = function(response)
        //{
        //    alert('No data foound');
        //    $scope.genericListObject.productBrands = response;
        //};
        
        $scope.refreshProductBrands = function (searchCriteria)
        {
            var params = { searchCriteria: searchCriteria };
            return $http.get('/StoreItem/GetStoreItemBrandObjects', { params: params }).then(function (response)
            {
                $scope.genericListObject.productBrands = response;
            });
            
        };
    }]);
    
});

var genericListObject = {};
var itemsLoaded = false;



