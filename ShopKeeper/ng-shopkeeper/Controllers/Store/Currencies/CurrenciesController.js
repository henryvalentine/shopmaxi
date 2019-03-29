"use strict";

define(['application-configuration', 'storeCurrencyServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('currencyTable', function ($compile)
    {
        return function ($scope, currencyTable)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreCurrency/GetStoreCurrencyObjects";
                tableOptions.itemId = 'StoreCurrencyId';
                tableOptions.columnHeaders = ['Name', 'Symbol', 'CountryName'];
                var ttc = tableManager($scope, $compile, currencyTable, tableOptions, 'New Currency', 'prepareCurrencyTemplate', 'getCurrency', 'deleteCurrency', 125);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });
   
    app.register.controller('storeCurrencyController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeCurrencyServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeCurrencyServices, alertsService)
    {
        $scope.getAuthStatus = function ()
        {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
       var countries = [];
       $scope.initializeController = function ()
       {
           $scope.countries = [];

           if (countries.length < 1) {
               storeCurrencyServices.getCountries($scope.getCountriesCompleted);
           } else {
               $scope.countries = countries;
           }
           
           $scope.currency = new Object();
           $scope.currency.StoreCurrencyId = 0;
           $scope.currency.Name = '';
           $scope.currency.IsDefaultCurrency = false;
           $scope.currency.Symbol = '';
           $scope.currency.Country = new Object();
           $scope.currency.Country.StoreCountryId = '';
           $scope.currency.Country.Name = '';
       };

       $scope.getCountriesCompleted = function (data)
       {
           $scope.countries = data;
           countries = data;
       };

       $scope.prepareCurrencyTemplate = function ()
       {
           $scope.initializeController();
           $scope.currency.Header = 'Create New Currency';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Currencies/ProcessCurrencies.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processCurrency = function ()
       {
           var currency = new Object();
           currency.Name = $scope.currency.Name;
           currency.Symbol = $scope.currency.Symbol;
           currency.IsDefaultCurrency = $scope.currency.IsDefaultCurrency;
           currency.StoreCountryId = $scope.currency.Country.StoreCountryId;
           if ($scope.currency.Name == undefined || $scope.currency.Name.length < 1)
           {
               alert("ERROR: Please provide Currency Name. ");
               return;
           }

           if (currency.StoreCountryId == undefined || currency.StoreCountryId < 1)
           {
               alert("ERROR: Please select a Country. ");
               return;
           }
           if ($scope.currency.StoreCurrencyId < 1)
           {
               storeCurrencyServices.addCurrency(currency, $scope.processCurrencyCompleted);
           }
           else
           {
               storeCurrencyServices.editCurrency(currency, $scope.processCurrencyCompleted);
           }

       };

       $scope.processCurrencyCompleted = function (data)
       {
           if (data.StoreCurrencyId < 1)
           {
                alert(data.Name);

           }
           else
           {
               if ($scope.currency.StoreCurrencyId < 1)
               {
                    alert("Currency information was successfully added.");
               }
               else
               {
                   alert("Currency information was successfully updated.");
               }
                ngDialog.close('/ng-shopkeeper/Views/Store/Currencies/ProcessCurrencies.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
            }
        };

       $scope.getCurrency = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           storeCurrencyServices.getCurrency(id, $scope.getCurrencyCompleted);
       };

       $scope.getCurrencyCompleted = function (data)
       {
           if (data.StoreCurrencyId < 1)
           {
             alert("ERROR: Currency information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.currency = data;
                $scope.currency.Country = {};
                $scope.currency.Country.StoreCountryId = data.StoreCountryId; 
                $scope.currency.Header = 'Update Currency Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Currencies/ProcessCurrencies.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
       $scope.deleteCurrency = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This Currency information will be deleted permanently. Continue?"))
               {
                   return;
               }
               storeCurrencyServices.deleteCurrency(id, $scope.deleteCurrencyCompleted);
           } else
           {
               alert('Invalid selection.');
           }
       };

       $scope.deleteCurrencyCompleted = function (data)
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
