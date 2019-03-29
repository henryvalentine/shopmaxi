"use strict";

define(['application-configuration', 'storeStateServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('stateTable', function ($compile)
    {
        return function ($scope, stateTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreState/GetStateObjects";
                tableOptions.itemId = 'StateId';
                tableOptions.columnHeaders = ['Name', 'CountryName'];
                var ttc = tableManager($scope, $compile, stateTable, tableOptions, 'New State', 'prepareStateTemplate', 'getState', 'deleteState', 100);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('storeStateController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeStateServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeStateServices, alertsService)
    {
        $rootScope.applicationModule = "State";
        $scope.getAuthStatus = function () {
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
           
           if (countries.length < 1)
           {
               storeStateServices.getCountries($scope.getCountriesCompleted);
           }
           else
           {
              $scope.countries = countries;
           }
           
           $scope.state = { 'StoreStateId': '', 'Name': '', 'StoreCountryId' : '' };
           $scope.state.country = { 'StoreCountryId': '', 'Name': '' };
           
           $scope.state.Header = 'Create New State';
       };

       $scope.getCountriesCompleted = function (data)
       {
           $scope.countries = data;
           countries = data;
       };

       $scope.prepareStateTemplate = function ()
       {
           $scope.initializeController();
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/States/ProcessStates.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processStateInfo = function ()
       {
           var state = new Object();
           state.Name = $scope.state.Name;
           state.StoreCountryId = $scope.state.country.StoreCountryId;
           if(state.Name == undefined || state.Name.length < 1)
           {
               alert("ERROR: Please provide State Name. ");
               return;
           }

           if (state.StoreCountryId == undefined || state.StoreCountryId < 1)
           {
               alert("ERROR: Please select a Country. ");
               return;
           }
           if ($scope.state.StoreStateId < 1)
           {
               storeStateServices.addState(state, $scope.processStateCompleted);
           }
           else
           {
               storeStateServices.editState(state, $scope.processStateCompleted);
           }

       };

       $scope.processStateCompleted = function (data)
       {
           if (data.Code < 1)
           {
                alert(data.Error);

           }
           else
           {
               alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/States/ProcessStates.html', '');
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

           storeStateServices.getState(id, $scope.getStateCompleted);
       };

       $scope.getStateCompleted = function (data)
       {
           if (data.StoreStateId < 1)
           {
             alert("ERROR: State information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.state = data;
                $scope.state.country = {};
                $scope.state.country.StoreCountryId = data.StoreCountryId;
                $scope.state.country.Name = '';
                $scope.state.Header = 'Update State Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/States/ProcessStates.html',
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
               storeStateServices.deleteState(id, $scope.deleteStateCompleted);
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

