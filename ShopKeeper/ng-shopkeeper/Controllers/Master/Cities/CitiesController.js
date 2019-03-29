"use strict";

define(['application-configuration', 'cityServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('cityTable', function ($compile)
    {
        return function ($scope, cityTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/City/GetCityObjects";
            tableOptions.itemId = 'CityId';
            tableOptions.columnHeaders = ['Name', 'StateName'];
            var ttc = tableManager($scope, $compile, cityTable, tableOptions, 'New City', 'prepareCityTemplate', 'getCity', 'deleteCity', 93);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });

    app.register.controller('manageCityCntroller', ['ngDialog','$scope', '$rootScope', '$routeParams', 'cityServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, cityServices, alertsService)
    {
       $rootScope.applicationModule = "City";
       $scope.initializeController = function ()
       {
           $scope.States = [];
           cityServices.getStates($scope.getStatesCompleted);
           $scope.selectedCity = new Object();
           $scope.selectedCity.CityId = 0;
           $scope.selectedCity.Name = '';
           $scope.selectedCity.State = new Object();
           $scope.selectedCity.State.StateId = '';
           $scope.selectedCity.State.Name = '';

       };

       $scope.getStatesCompleted = function (data)
       {
           $scope.States = data;
       };

       $scope.prepareCityTemplate = function ()
       {
           $scope.initializeController();
           $scope.selectedCity.Header = 'Create New City';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Master/Cities/ProcessCities.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processCity = function ()
       {
           var city = new Object();
           city.Name = $scope.selectedCity.Name;
           city.StateId = $scope.selectedCity.State.StateId;
           if(city.Name == undefined || city.Name.length < 1)
           {
               alert("ERROR: Please provide City Name. ");
               return;
           }

           if (city.StateId == undefined || city.StateId < 1)
           {
               alert("ERROR: Please select a State. ");
               return;
           }
           if ($scope.selectedCity.CityId < 1)
           {
               cityServices.addCity(city, $scope.processCityCompleted);
           }
           else
           {
               cityServices.editCity(city, $scope.processCityCompleted);
           }

       };

       $scope.processCityCompleted = function (data)
       {
           if (data.CityId < 1)
           {
                alert(data.Name);

           }
           else
           {
               if ($scope.selectedCity.CityId < 1)
               {
                    alert("City information was successfully added.");
               }
               else
               {
                   alert("City information was successfully updated.");
               }
                ngDialog.close('/ng-shopkeeper/Views/Master/Cities/ProcessCities.html', '');
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

           cityServices.getCity(id, $scope.getCityCompleted);
       };

       $scope.getCityCompleted = function (data)
       {
           if (data.CityId < 1)
           {
             alert("ERROR: City information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.selectedCity = data;
                $scope.selectedCity.State = {};
                $scope.selectedCity.State.StateId = data.StateId; 
                $scope.selectedCity.State.Name = '';
                $scope.selectedCity.Header = 'Update City Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Master/Cities/ProcessCities.html',
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
               cityServices.deleteCity(id, $scope.deleteCityCompleted);
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

