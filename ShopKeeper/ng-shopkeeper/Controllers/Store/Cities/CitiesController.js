"use strict";

define(['application-configuration', 'storeCityServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('cityTable', function ($compile)
    {
        return function ($scope, cityTable)
        {
            var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreCity/GetCityObjects";
                tableOptions.itemId = 'StoreCityId';
                tableOptions.columnHeaders = ['Name', 'StateName'];
                var ttc = tableManager($scope, $compile, cityTable, tableOptions, 'New City', 'prepareCityTemplate', 'getCity', 'deleteCity', 93);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('manageCityCntroller', ['ngDialog','$scope', '$rootScope', '$routeParams', 'storeCityServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeCityServices, alertsService)
    {
        $rootScope.applicationModule = "City";
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
       $scope.initializeController = function ()
       {
           $scope.States = [];
           storeCityServices.getStates($scope.getStatesCompleted);
           $scope.city = new Object();
           $scope.city.StoreCityId = 0;
           $scope.city.Name = '';
           $scope.city.State = new Object();
           $scope.city.State.StoreStateId = '';
           $scope.city.State.Name = '';

       };

       $scope.getStatesCompleted = function (data)
       {
           $scope.States = data;
       };

       $scope.prepareCityTemplate = function ()
       {
           $scope.initializeController();
           $scope.city.Header = 'Create New City';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Cities/ProcessCities.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processCity = function ()
       {
           var city = new Object();
           city.Name = $scope.city.Name;
           city.StoreStateId = $scope.city.State.StoreStateId;
           if(city.Name == undefined || city.Name.length < 1)
           {
               alert("ERROR: Please provide City Name. ");
               return;
           }

           if (city.StoreStateId == undefined || city.StoreStateId < 1)
           {
               alert("ERROR: Please select a State. ");
               return;
           }
           if ($scope.city.StoreCityId < 1)
           {
               storeCityServices.addCity(city, $scope.processCityCompleted);
           }
           else
           {
               storeCityServices.editCity(city, $scope.processCityCompleted);
           }

       };

       $scope.processCityCompleted = function (data)
       {
           if (data.StoreCityId < 1)
           {
                alert(data.Name);

           }
           else
           {
               if ($scope.city.StoreCityId < 1)
               {
                    alert("City information was successfully added.");
               }
               else
               {
                   alert("City information was successfully updated.");
               }
                ngDialog.close('/ng-shopkeeper/Views/Store/Cities/ProcessCities.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
            }
        };

       $scope.getCity = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           storeCityServices.getCity(id, $scope.getCityCompleted);
       };

       $scope.getCityCompleted = function (data)
       {
           if (data.StoreCityId < 1)
           {
             alert("ERROR: City information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.city = data;
                $scope.city.State = {};
                $scope.city.State.StoreStateId = data.StoreStateId; 
                $scope.city.State.Name = '';
                $scope.city.Header = 'Update City Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Cities/ProcessCities.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
       $scope.deleteCity = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This City information will be deleted permanently. Continue?"))
               {
                   return;
               }
               storeCityServices.deleteCity(id, $scope.deleteCityCompleted);
           } else {
               alert('Invalid selection.');
           }
       };

       $scope.deleteCityCompleted = function (data)
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

