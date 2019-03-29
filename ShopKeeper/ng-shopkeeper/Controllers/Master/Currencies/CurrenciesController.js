"use strict";

define(['application-configuration', 'currencyServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('currencyTable', function ($compile)
    {
        return function ($scope, currencyTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/Currency/GetCurrencyObjects";
            tableOptions.itemId = 'CurrencyId';
            tableOptions.columnHeaders = ['Name','Symbol', 'CountryName'];
            var ttc = tableManager($scope, $compile, currencyTable, tableOptions, 'New Currency', 'prepareCurrencyTemplate', 'getCurrency', 'deleteCurrency', 125);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });
    
    app.register.controller('manageCurrencyCntroller', ['ngDialog','$scope', '$rootScope', '$routeParams', 'currencyServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, currencyServices, alertsService)
    {
       $rootScope.applicationModule = "Currency";
       var countries = [];
       $scope.initializeController = function ()
       {
           $scope.countries = [];

           if (countries.length < 1) {
               currencyServices.getCountries($scope.getCountriesCompleted);
           } else {
               $scope.countries = countries;
           }
           
           $scope.currency = new Object();
           $scope.currency.CurrencyId = 0;
           $scope.currency.Name = '';
           $scope.currency.Symbol = '';
           $scope.currency.Country = new Object();
           $scope.currency.Country.CountryId = '';
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
               template: '/ng-shopkeeper/Views/Master/Currencies/ProcessCurrencies.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processCurrency = function ()
       {
           var currency = new Object();
           currency.Name = $scope.currency.Name;
           currency.Symbol = $scope.currency.Symbol;
           currency.CountryId = $scope.currency.Country.CountryId;
           if ($scope.currency.Name == undefined || $scope.currency.Name.length < 1)
           {
               alert("ERROR: Please provide Currency Name. ");
               return;
           }

           if (currency.CountryId == undefined || currency.CountryId < 1)
           {
               alert("ERROR: Please select a Country. ");
               return;
           }
           if ($scope.currency.CurrencyId < 1)
           {
               currencyServices.addCurrency(currency, $scope.processCurrencyCompleted);
           }
           else
           {
               currencyServices.editCurrency(currency, $scope.processCurrencyCompleted);
           }

       };

       $scope.processCurrencyCompleted = function (data)
       {
           if (data.CurrencyId < 1)
           {
                alert(data.Name);

           }
           else
           {
               if ($scope.currency.CurrencyId < 1)
               {
                    alert("Currency information was successfully added.");
               }
               else
               {
                   alert("Currency information was successfully updated.");
               }
                ngDialog.close('/ng-shopkeeper/Views/Master/Currencies/ProcessCurrencies.html', '');
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

           currencyServices.getCurrency(id, $scope.getCurrencyCompleted);
       };

       $scope.getCurrencyCompleted = function (data)
       {
           if (data.CurrencyId < 1)
           {
             alert("ERROR: Currency information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.currency = data;
                $scope.currency.Country = {};
                $scope.currency.Country.CountryId = data.CountryId; 
                $scope.currency.Country.Name = data.CountryName;
                $scope.currency.Header = 'Update Currency Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Master/Currencies/ProcessCurrencies.html',
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
               currencyServices.deleteCurrency(id, $scope.deleteCurrencyCompleted);
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
