"use strict";

define(['application-configuration', 'storeOutletServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngStoreOutletTable', function ($compile)
    {
        return function ($scope, ngStoreOutletTable)
        {
            var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreOutlet/GetStoreOutletObjects";
                tableOptions.itemId = 'StoreOutletId';
                tableOptions.columnHeaders = ['OutletName', 'IsMainOutlet', 'DefaultTax', 'IsOperational', 'DateCreated'];
                var ttc = tableManager($scope, $compile, ngStoreOutletTable, tableOptions, 'New Store Outlet', 'prepareStoreOutletTemplate', 'getStoreOutlet', 'deleteStoreOutlet', 143);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });
    
    app.register.controller('storeOutletController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeOutletServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeOutletServices, alertsService)
    {
        $rootScope.applicationModule = "StoreOutlet";
        
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        
        $scope.alerts = [];
        var cities = [];
        $scope.initializeController = function ()
        {
            $scope.cities = [];
            if (cities.length < 1)
            {
                storeOutletServices.getCities($scope.getCitiesCompleted);
            }
            else
            {
                $scope.cities = cities;
            }
            $scope.storeOutlet = new Object();
            $scope.storeOutlet.StoreOutletId = 0;
            $scope.storeOutlet.City = new Object();
            $scope.storeOutlet.City.StoreCityId = 0;
            $scope.storeOutlet.City.Name = '';
            $scope.storeOutlet.OutletName = '';
            $scope.storeOutlet.StoreAddress = '';
            $scope.storeOutlet.DefaultTax = '';
            $scope.storeOutlet.FacebookHandle = '';
            $scope.storeOutlet.TwitterHandle = '';
            $scope.storeOutlet.IsOperational = true;
            $scope.storeOutlet.IsMainOutlet = false;
            $scope.storeOutlet.Header = 'Create New Outlet';
        };
        
        $scope.prepareStoreOutletTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Outlets/ProcessOutlets.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processStoreOutlet = function ()
        {
            var storeOutlet = new Object();

            storeOutlet.StoreOutletId = $scope.storeOutlet.StoreOutletId;
            storeOutlet.OutletName = $scope.storeOutlet.OutletName;
            storeOutlet.StoreAddressObject = { 'StreetNo': $scope.storeOutlet.StoreAddress, 'StoreCityId': $scope.storeOutlet.City.StoreCityId };
            storeOutlet.DefaultTax = $scope.storeOutlet.DefaultTax;
            if ($scope.storeOutlet.IsOperational)
            {
                storeOutlet.IsOperational = true;
            }
            else
            {
                storeOutlet.IsOperational = false;
            }
            
            if ($scope.storeOutlet.IsMainOutlet)
            {
                storeOutlet.IsMainOutlet = true;
            }
            else
            {
                storeOutlet.IsMainOutlet = false;
            }

            storeOutlet.FacebookHandle = $scope.storeOutlet.FacebookHandle;
            storeOutlet.TwitterHandle = $scope.storeOutlet.TwitterHandle;
            
            if (parseInt(storeOutlet.StoreCityId) < 1)
            {
                alert("ERROR: Please select a City. ");
                return;
            }
            
            if (storeOutlet.OutletName == undefined || storeOutlet.OutletName == null || storeOutlet.OutletName < 1)
            {
                alert("ERROR: Please provide Outlet Name. ");
                return;
            }
            if (storeOutlet.StoreAddressObject.StreetNo == undefined || storeOutlet.StoreAddressObject.StreetNo == null || storeOutlet.StoreAddressObject.StreetNo < 1) {
                alert("ERROR: Please provide Outlet Address. ");
                return;
            }

            if ($scope.storeOutlet.StoreOutletId < 1)
            {
                storeOutletServices.addStoreOutlet(storeOutlet, $scope.processStoreOutletCompleted);
            }
            else
            {
                storeOutletServices.editStoreOutlet(storeOutlet, $scope.processStoreOutletCompleted);
            }

        };
        
        $scope.getCitiesCompleted = function (data)
        {
            $scope.cities = data;
            cities = data;
        };

        $scope.processStoreOutletCompleted = function (data)
        {
            if (data.Code < 1)
            {
                   alert(data.Error);

            }
            else
            {
                alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/Outlets/ProcessOutlets.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
           }
        };

        $scope.getStoreOutlet = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeOutletServices.getStoreOutlet(id, $scope.getStoreOutletCompleted);
        };
        
        $scope.getStoreOutletCompleted = function (data)
        {
            if (data.StoreOutletId < 1)
            {
                alert("ERROR: Outlet information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.storeOutlet = new Object();
                $scope.storeOutlet = data;
                $scope.StoreOutletId = $scope.storeOutlet.StoreOutletId;
                $scope.storeOutlet.City = new Object();
                $scope.storeOutlet.City.StoreCityId = data.StoreCityId;
                $scope.storeOutlet.City.Name = data.CityName;
                $scope.storeOutlet.StoreAddress = data.Address;

                $scope.storeOutlet.Header = 'Update Outlet Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Outlets/ProcessOutlets.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteStoreOutlet = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Outlet information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storeOutletServices.deleteStoreOutlet(id, $scope.deleteStoreOutletCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteStoreOutletCompleted = function (data)
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