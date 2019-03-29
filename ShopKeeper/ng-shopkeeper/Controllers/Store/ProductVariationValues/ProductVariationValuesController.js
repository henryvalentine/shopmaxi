"use strict";

define(['application-configuration', 'productVariationValueServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngProductVariationValueTable', function ($compile)
    {
        return function ($scope, ngProductVariationValueTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItemVariationValue/GetStoreItemVariationValueObjects";
                tableOptions.itemId = 'ProductVariationValueId';
                tableOptions.columnHeaders = ['Value'];
                var ttc = tableManager($scope, $compile, ngProductVariationValueTable, tableOptions, 'New Product Variation Value', 'prepareProductVariationValueTemplate', 'getProductVariationValue', 'deleteProductVariationValue', 215);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('productVariationValueController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productVariationValueServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, productVariationValueServices, alertsService)
    {
        $rootScope.applicationModule = "productVariationValue";
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
            $scope.productVariationValue = new Object();
            $scope.productVariationValue.ProductVariationValueId = 0;
            $scope.productVariationValue.Value = '';
            $scope.productVariationValue.Header = 'Create New Product Variation Value';
        };
        
        $scope.prepareProductVariationValueTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ProductVariationValues/ProcessProductVariationValues.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processProductVariationValue = function ()
        {
            var productVariationValue = new Object();
            productVariationValue.Value = $scope.productVariationValue.Value;
            productVariationValue.Description = $scope.productVariationValue.Description;
            
            if (productVariationValue.Value == undefined || productVariationValue.Value.length < 1)
            {
                alert("ERROR: Please provide Product Variation Value. ");
                return;
            }

            if ($scope.productVariationValue.ProductVariationValueId < 1)
            {
                productVariationValueServices.addProductVariationValue(productVariationValue, $scope.processProductVariationValueCompleted);
            }
            else
            {
                productVariationValueServices.editProductVariationValue(productVariationValue, $scope.processProductVariationValueCompleted);
            }

        };
        
        $scope.processProductVariationValueCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

            }
            else
            {

                if ($scope.productVariationValue.ProductVariationValueId < 1)
                {
                    alert("Product Variation Value information was successfully added.");
                }
                else
                {
                    alert("Product Variation Value information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/ProductVariationValues/ProcessProductVariationValues.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getProductVariationValue = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productVariationValueServices.getProductVariationValue(id, $scope.getProductVariationValueCompleted);
        };
        
        $scope.getProductVariationValueCompleted = function (data)
        {
            if (data.ProductVariationValueId < 1)
            {
                alert("ERROR: Product Variation Value information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.productVariationValue = data;
                $scope.productVariationValue.Header = 'Update Product Variation Value Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ProductVariationValues/ProcessProductVariationValues.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteProductVariationValue = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Product Variation Value information will be deleted permanently. Continue?"))
                {
                    return;
                }
                productVariationValueServices.deleteProductVariationValue(id, $scope.deleteProductVariationValueCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteProductVariationValueCompleted = function (data)
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