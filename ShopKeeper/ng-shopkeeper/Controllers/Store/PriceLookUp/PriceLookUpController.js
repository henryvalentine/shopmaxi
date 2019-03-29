"use strict";

define(['application-configuration', 'itemPriceServices', 'ngDialog'], function (app)
{
    app.register.controller('priceLookUpController', ['ngDialog','$scope', '$rootScope', '$routeParams', 'itemPriceServices',
    function (ngDialog, $scope, $rootScope, $routeParams, itemPriceServices)
    {
        $rootScope.applicationModule = "Item Price LookUp";
        
        $scope.getAuthStatus = function ()
        {
            return $rootScope.isAuthenticated === true ? '' : $scope.redir();
        };

        $scope.redir = function ()
        {
           $rootScope.redirectUrl = $location.path();
           $location.path = "/ngy.html#signIn";
        };
        
       $scope.initializeController = function ()
       {
           $scope.getAuthStatus();
           $scope.priceList = [];
       };
    
       $scope.scanSKU = function ()
       {
           $scope.skuControl.focus();
       };

       $scope.getpItemPrices = function ()
       {
           if ($scope.criteria == undefined || $scope.criteria.trim().length < 1)
           {
               return;
           }

           itemPriceServices.getItemPrices($scope.criteria, $scope.getpItemPricesCompleted);
       };

       $scope.getpItemPricesCompleted = function (data)
       {
           if (data == null || data.length < 1)
           {
               alert('No match found!');
               return;
           }

           $scope.priceList = data;
       };
      
    }]);
   
});

