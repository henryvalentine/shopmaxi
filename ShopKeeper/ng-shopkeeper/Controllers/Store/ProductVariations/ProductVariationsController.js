"use strict";

define(['application-configuration', 'productVariationServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngProductVariationTable', function ($compile)
    {
        return function ($scope, ngProductVariationTable)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreItemVariation/GetStoreItemVariationObjects";
                tableOptions.itemId = 'ProductVariationId';
                tableOptions.columnHeaders = ['VariationProperty'];
                var ttc = tableManager($scope, $compile, ngProductVariationTable, tableOptions, 'New Product Variation', 'prepareProductVariationTemplate', 'getProductVariation', 'deleteProductVariation', 177);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('productVariationController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productVariationServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, productVariationServices, alertsService)
    {
        $rootScope.applicationModule = "productVariation";
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
            $scope.productVariation = new Object();
            $scope.productVariation.ProductVariationId = 0;
            $scope.productVariation.VariationProperty = '';
            $scope.productVariation.Header = 'Create New Product Variation';
        };
        
        $scope.prepareProductVariationTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ProductVariations/ProcessProductVariations.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processProductVariation = function ()
        {
            var productVariation = new Object();
            productVariation.VariationProperty = $scope.productVariation.VariationProperty;
            productVariation.Description = $scope.productVariation.Description;
            
            if (productVariation.VariationProperty == undefined || productVariation.VariationProperty.length < 1)
            {
                alert("ERROR: Please provide Product Variation Property. ");
                return;
            }

            if ($scope.productVariation.ProductVariationId < 1)
            {
                productVariationServices.addProductVariation(productVariation, $scope.processProductVariationCompleted);
            }
            else
            {
                productVariationServices.editProductVariation(productVariation, $scope.processProductVariationCompleted);
            }

        };
        
        $scope.processProductVariationCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {

                if ($scope.productVariation.ProductVariationId < 1)
                {
                    alert("Product Variation information was successfully added.");
                }
                else
                {
                    alert("Product Variation information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/ProductVariations/ProcessProductVariations.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getProductVariation = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            productVariationServices.getProductVariation(id, $scope.getProductVariationCompleted);
        };
        
        $scope.getProductVariationCompleted = function (data)
        {
            if (data.ProductVariationId < 1)
            {
                alert("ERROR: Product Variation information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.productVariation = data;
                $scope.productVariation.Header = 'Update Product Variation Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ProductVariations/ProcessProductVariations.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteProductVariation = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Product Variation information will be deleted permanently. Continue?"))
                {
                    return;
                }
                productVariationServices.deleteProductVariation(id, $scope.deleteProductVariationCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteProductVariationCompleted = function (data)
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