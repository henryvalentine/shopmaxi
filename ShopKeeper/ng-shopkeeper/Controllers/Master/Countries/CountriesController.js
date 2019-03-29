"use strict";

define(['application-configuration', 'countryServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('countryTable', function ($compile)
    {
        return function ($scope, countryTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/Country/GetCountryObjects";
            tableOptions.itemId = 'CountryId';
            tableOptions.columnHeaders = ['Name'];
            var ttc = tableManager($scope, $compile, countryTable, tableOptions, 'New Country', 'prepareCountryTemplate', 'getCountry', 'deleteCountry', 118);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });

    app.register.controller('manageCountryCntroller', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'countryServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, countryServices, alertsService)
    {
        $rootScope.applicationModule = "Country";
       
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
                template: '/ng-shopkeeper/Views/Master/Countries/ProcessCountries.html',
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
                countryServices.addCountry(country, $scope.processCountryCompleted);
            }
            else {
                countryServices.editCountry(country, $scope.processCountryCompleted);
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
                   ngDialog.close('/ng-shopkeeper/Views/Master/Countries/ProcessCountries.html', '');
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

            countryServices.getCountry(id, $scope.getCountryCompleted);
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
                    template: '/ng-shopkeeper/Views/Master/Countries/ProcessCountries.html',
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
                countryServices.deleteCountry(id, $scope.deleteCountryCompleted);
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