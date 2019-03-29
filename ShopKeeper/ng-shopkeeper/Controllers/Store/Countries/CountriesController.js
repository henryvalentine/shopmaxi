"use strict";

define(['application-configuration', 'storeCountryServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngCountryTable', function ($compile)
    {
        return function ($scope, ngCountryTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreCountry/GetCountryObjects";
                tableOptions.itemId = 'CountryId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableManager($scope, $compile, ngCountryTable, tableOptions, 'New Country', 'prepareCountryTemplate', 'getCountry', 'deleteCountry', 118);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('storeCountryController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeCountryServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeCountryServices, alertsService)
    {
        $rootScope.applicationModule = "Country";
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
            $scope.selectedCountry = new Object();
            $scope.selectedCountry.CountryId = 0;
            $scope.selectedCountry.Name = '';
            $scope.selectedCountry.Header = 'Create New Country';
        };
        
        $scope.prepareCountryTemplate = function () {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Countries/ProcessCountries.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processCountry = function () {
            var country = new Object();
            country.Name = $scope.selectedCountry.Name;
            if (country.Name == undefined || country.Name.length < 1) {
                alert("ERROR: Please provide Country Name. ");
                return;
            }

            if ($scope.selectedCountry.CountryId < 1) {
                storeCountryServices.addCountry(country, $scope.processCountryCompleted);
            }
            else {
                storeCountryServices.editCountry(country, $scope.processCountryCompleted);
            }

        };
        
        $scope.processCountryCompleted = function (data)
        {
            if (data.CountryId < 1)
            {
                   alert(data.Name);

               }
               else {

                   if ($scope.selectedCountry.CountryId < 1) {
                       alert("Country information was successfully added.");
                   } else {
                       alert("Country information was successfully updated.");
                   }
                   ngDialog.close('/ng-shopkeeper/Views/Store/Countries/ProcessCountries.html', '');
                   $scope.jtable.fnClearTable();
                   $scope.initializeController();
               }
           };

        $scope.getCountry = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeCountryServices.getCountry(id, $scope.getCountryCompleted);
        };
        
        $scope.getCountryCompleted = function (data)
        {
            if (data.CountryId < 1)
            {
                alert("ERROR: Country information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.selectedCountry = data;
                $scope.selectedCountry.Header = 'Update Country Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Countries/ProcessCountries.html',
                    className: 'ngdialog-theme-flat',
                    //controller: 'manageCountryCntroller',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteCountry = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Country information will be deleted permanently. Continue?")) {
                    return;
                }
                storeCountryServices.deleteCountry(id, $scope.deleteCountryCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteCountryCompleted = function (data)
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