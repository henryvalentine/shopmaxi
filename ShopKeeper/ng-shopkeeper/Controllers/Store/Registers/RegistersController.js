"use strict";

define(['application-configuration', 'registerServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngRegisterTable', function ($compile)
    {
        return function ($scope, ngRegisterTable)
        {
            var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/Register/GetRegisterObjects";
                tableOptions.itemId = 'RegisterId';
                tableOptions.columnHeaders = ['Name', 'OutletName'];
                var ttc = tableManager($scope, $compile, ngRegisterTable, tableOptions, 'New Register', 'prepareRegisterTemplate', 'getRegister', 'deleteRegister', 121);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('registerController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'registerServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, registerServices, alertsService)
    {
        $rootScope.applicationModule = "Register";
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
            $scope.selectedRegister = new Object();
            $scope.selectedRegister.RegisterId = 0;
            $scope.selectedRegister.Name = '';
            $scope.selectedRegister.CurrentOutletId = 0;
            $scope.selectedRegister.StoreOutlet = { 'StoreOutletId': '', 'OutletName': '-- Select Outlet --' };
            $scope.selectedRegister.Header = 'Create New Register';
            $scope.getOutlets();
        };
        
        $scope.getOutlets = function ()
        {
            registerServices.getOutlets($scope.getOutletsCompleted);
        };
        
        $scope.getOutletsCompleted = function (data)
        {
            $scope.outlets = data;
        };
        
        $scope.prepareRegisterTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Registers/ProcessRegisters.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processRegister = function () {
            var register = new Object();
            register.Name = $scope.selectedRegister.Name;
            register.CurrentOutletId = $scope.selectedRegister.StoreOutlet.StoreOutletId;

            if (register.Name == undefined || register.Name.length < 1)
            {
                alert("ERROR: Please provide Register Name. ");
                return;
            }
            
            if (register.CurrentOutletId < 1)
            {
                alert("ERROR: Please select an Outlet. ");
                return;
            }

            if ($scope.selectedRegister.RegisterId < 1)
            {
                registerServices.addRegister(register, $scope.processRegisterCompleted);
            }
            else {
                registerServices.editRegister(register, $scope.processRegisterCompleted);
            }

        };
        
        $scope.processRegisterCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/Registers/ProcessRegisters.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
             }
           };

        $scope.getRegister = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            registerServices.getRegister(id, $scope.getRegisterCompleted);
        };
        
        $scope.getRegisterCompleted = function (data)
        {
            if (data.RegisterId < 1)
            {
                alert("ERROR: Register information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.selectedRegister = data;
                $scope.selectedRegister.CurrentOutletId = data.CurrentOutletId;
                $scope.selectedRegister.StoreOutlet = { 'StoreOutletId': data.CurrentOutletId, 'OutletName': data.Name };
                $scope.selectedRegister.Header = 'Update Register Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Registers/ProcessRegisters.html',
                    className: 'ngdialog-theme-flat',
                    controller: 'registerController',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteRegister = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Register information will be deleted permanently. Continue?")) {
                    return;
                }
                registerServices.deleteRegister(id, $scope.deleteRegisterCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteRegisterCompleted = function (data)
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