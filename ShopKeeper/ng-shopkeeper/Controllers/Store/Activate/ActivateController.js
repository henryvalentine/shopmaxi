"use strict";

define(['application-configuration', 'welcomeServices', 'ngDialog'], function (app)
{
    app.register.controller('activateController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'welcomeServices',
    function (ngDialog, $scope, $rootScope, $routeParams, welcomeServices)
    {
        $scope.getDefaults = function () {
            $rootScope.busy = true;
            welcomeServices.getDefaults($scope.getDefaultsComplete);
        };

        $scope.getDefaultsComplete = function (response)
        {
            if (response == null)
            {
                $location.path('/Home/Index');
            }
            else
            {
                $rootScope.categories = response.ItemCategories;
                $rootScope.itemTypes = response.ItemTypes;
                $rootScope.itemBrands = response.ItemBrands;
                $rootScope.items = response.Items;
                $rootScope.store = response.StoreInfo;
                $rootScope.busy = false;
            }
        };
       
    }]);

});