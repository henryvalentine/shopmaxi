"use strict";

define(['application-configuration', 'productBrandServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngProductBrandTable', function ($compile)
    {
        return function ($scope, ngProductBrandTable)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus === false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItemBrand/GetStoreItemBrandObjects";
                tableOptions.itemId = 'ProductBrandId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableManager($scope, $compile, ngProductBrandTable, tableOptions, 'New Product Brand', 'prepareProductBrandTemplate', 'getProductBrand', 'deleteProductBrand', 160);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('productBrandController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productBrandServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, productBrandServices, alertsService)
    {
        $rootScope.applicationModule = "productBrand";
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.alerts = [];
        $scope.initializeController = function ()
        {
            $scope.productBrand = new Object();
            $scope.productBrand.ProductBrandId = 0;
            $scope.productBrand.Name = '';
            $scope.productBrand.Description = '';
            $scope.productBrand.Header = 'Create New Product Brand';
        };
        
        $scope.prepareProductBrandTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ProductBrands/ProcessProductBrands.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processProductBrand = function ()
        {
            var productBrand = new Object();
            productBrand.Name = $scope.productBrand.Name;
            productBrand.Description = $scope.productBrand.Description;
            
            if (productBrand.Name == undefined || productBrand.Name.length < 1)
            {
                alert("ERROR: Please provide Product Product Brand Name. ");
                return;
            }

            if ($scope.productBrand.ProductBrandId < 1)
            {
                productBrandServices.addProductBrand(productBrand, $scope.processProductBrandCompleted);
            }
            else
            {
                productBrandServices.editProductBrand(productBrand, $scope.processProductBrandCompleted);
            }

        };
        
        $scope.processProductBrandCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {

                if ($scope.productBrand.ProductBrandId < 1)
                {
                    alert("Product Brand information was successfully added.");
                }
                else
                {
                    alert("Product Brand information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/ProductBrands/ProcessProductBrands.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getProductBrand = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productBrandServices.getProductBrand(id, $scope.getProductBrandCompleted);
        };
        
        $scope.getProductBrandCompleted = function (data)
        {
            if (data.ProductBrandId < 1)
            {
                alert("ERROR: Product Brand information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.productBrand = data;
                $scope.productBrand.Header = 'Update Product Brand Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ProductBrands/ProcessProductBrands.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteProductBrand = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Product Brand information will be deleted permanently. Continue?"))
                {
                    return;
                }
                productBrandServices.deleteProductBrand(id, $scope.deleteProductBrandCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteProductBrandCompleted = function (data)
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