"use strict";

define(['application-configuration', 'stateServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('stateTable', function ($compile)
    {
        return function ($scope, stateTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/State/GetStateObjects";
            tableOptions.itemId = 'StateId';
            tableOptions.columnHeaders = ['Name', 'CountryName'];
            var ttc = tableManager($scope, $compile, stateTable, tableOptions, 'New State', 'prepareStateTemplate', 'getState', 'deleteState', 100);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });

    app.register.controller('manageStateCntroller', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'stateServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, stateServices, alertsService)
    {
        $rootScope.applicationModule = "State";
        var countries = [];
       $scope.initializeController = function ()
       {
           $scope.countries = [];
           
           if (countries.length < 1)
           {
               stateServices.getCountries($scope.getCountriesCompleted);
           } else {
               $scope.countries = countries;
           }
           $scope.selectedState = new Object();
           $scope.selectedState.StateId = 0;
           $scope.selectedState.Name = '';
           $scope.selectedState.Country = new Object();
           $scope.selectedState.Country.CountryId = '';
           $scope.selectedState.Country.Name = '';

       };

       $scope.getCountriesCompleted = function (data)
       {
           $scope.countries = data;
           countries = data;
       };

       $scope.prepareStateTemplate = function ()
       {
           $scope.initializeController();
           $scope.selectedState.Header = 'Create New State';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Master/States/ProcessStates.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processState = function ()
       {
           var state = new Object();
           state.Name = $scope.selectedState.Name;
           state.CountryId = $scope.selectedState.Country.CountryId;
           if(state.Name == undefined || state.Name.length < 1)
           {
               alert("ERROR: Please provide State Name. ");
               return;
           }

           if (state.CountryId == undefined || state.CountryId < 1)
           {
               alert("ERROR: Please select a Country. ");
               return;
           }
           if ($scope.selectedState.StateId < 1)
           {
               stateServices.addState(state, $scope.processStateCompleted);
           }
           else
           {
               stateServices.editState(state, $scope.processStateCompleted);
           }

       };

       $scope.processStateCompleted = function (data)
       {
           if (data.StateId < 1)
           {
                alert(data.Name);

           }
           else
           {
               if ($scope.selectedState.StateId < 1)
               {
                    alert("State information was successfully added.");
               }
               else
               {
                   alert("State information was successfully updated.");
               }
                ngDialog.close('/ng-shopkeeper/Views/Master/States/ProcessStates.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
            }
        };

       $scope.getState = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           stateServices.getState(id, $scope.getStateCompleted);
       };

       $scope.getStateCompleted = function (data)
       {
           if (data.StateId < 1)
           {
             alert("ERROR: State information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.selectedState = data;
                $scope.selectedState.Country = {};
                $scope.selectedState.Country.CountryId = data.CountryId; 
                $scope.selectedState.Country.Name = data.StateName;
                $scope.selectedState.Header = 'Update State Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Master/States/ProcessStates.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
       $scope.deleteState = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This State information will be deleted permanently. Continue?"))
               {
                   return;
               }
               stateServices.deleteState(id, $scope.deleteStateCompleted);
           } else {
               alert('Invalid selection.');
           }
       };

       $scope.deleteStateCompleted = function (data)
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

