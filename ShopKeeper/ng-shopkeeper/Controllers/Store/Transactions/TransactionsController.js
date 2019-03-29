"use strict";

define(['application-configuration', 'storeTransactionServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('storeTransactionTable', function ($compile)
    {
        return function ($scope, storeTransactionTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreTransaction/GetStoreTransactionObjects";
                tableOptions.itemId = 'StoreTransactionId';
                tableOptions.columnHeaders = ['PackageTitle', 'FileStorageSpace', 'NumberOfStoreProducts', 'NumberOfOutlets', 'Registers', 'NumberOfUsers', 'MaximumTransactions'];
                var ttc = tableManager($scope, $compile, storeTransactionTable, tableOptions, 'New Package', 'prepareStoreTransactionTemplate', 'getStoreTransaction', 'deleteStoreTransaction', 125);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('storeTransactionController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeTransactionServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeTransactionServices, alertsService)
    {
        $rootScope.applicationModule = "StoreTransaction";
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
            $scope.storeTransaction = new Object();
            $scope.storeTransaction.StoreTransactionId = 0;
            $scope.storeTransaction.PackageTitle = '';
            $scope.storeTransaction.FileStorageSpace = '';
            $scope.storeTransaction.NumberOfStoreProducts = '';
            $scope.storeTransaction.NumberOfOutlets = '';
            $scope.storeTransaction.Registers = '';
            $scope.storeTransaction.NumberOfUsers = '';
            $scope.storeTransaction.UseReportBuilder = '';
            $scope.storeTransaction.GenerateReports = '';
            $scope.storeTransaction.MaximumTransactions = '';
            $scope.storeTransaction.Note = '';
            $scope.storeTransaction.Header = 'Create New Transaction';
        };
        
        $scope.prepareStoreTransactionTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/StoreTransactions/ProcessStoreTransactions.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processStoreTransaction = function ()
        {
            var storeTransaction = new Object();

            storeTransaction.PackageTitle = $scope.storeTransaction.PackageTitle;
            storeTransaction.FileStorageSpace = $scope.storeTransaction.FileStorageSpace;
            storeTransaction.NumberOfStoreProducts = $scope.storeTransaction.NumberOfStoreProducts;
            storeTransaction.NumberOfOutlets = $scope.storeTransaction.NumberOfOutlets;
            storeTransaction.Registers = $scope.storeTransaction.Registers;
            storeTransaction.NumberOfUsers = $scope.storeTransaction.NumberOfUsers;
            storeTransaction.UseReportBuilder = $scope.storeTransaction.UseReportBuilder;
            storeTransaction.GenerateReports = $scope.storeTransaction.GenerateReports;
            storeTransaction.MaximumTransactions = $scope.storeTransaction.MaximumTransactions;
            storeTransaction.Note = $scope.storeTransaction.Note;
            
            if (storeTransaction.PackageTitle == undefined || storeTransaction.PackageTitle == null || storeTransaction.PackageTitle.length < 1)
            {
                alert("ERROR: Please provide Subscription Package Title. ");
                return;
            }
            
            if (storeTransaction.FileStorageSpace == undefined || storeTransaction.FileStorageSpace == null || storeTransaction.FileStorageSpace < 1)
            {
                alert("ERROR: Please provide File Storage Space. ");
                return;
            }

            if (storeTransaction.NumberOfStoreProducts == undefined || storeTransaction.NumberOfStoreProducts == null || storeTransaction.NumberOfStoreProducts < 1) {
                alert("ERROR: Please provide Number Of Store Products. ");
                return;
            }

            if (storeTransaction.NumberOfOutlets == undefined || storeTransaction.NumberOfOutlets == null || storeTransaction.NumberOfOutlets < 1)
            {
                alert("ERROR: Please provide Number Of Outlets. ");
                return;
            }
            
            if (storeTransaction.Registers == undefined || storeTransaction.Registers == null || storeTransaction.Registers < 1) {
                alert("ERROR: Please provide Number of Registers. ");
                return;
            }
            
            if (storeTransaction.NumberOfUsers == undefined || storeTransaction.NumberOfUsers == null || storeTransaction.NumberOfUsers < 1) {
                alert("ERROR: Please provide Number Of Users. ");
                return;
            }

            if (storeTransaction.MaximumTransactions == undefined || storeTransaction.MaximumTransactions == null || parseInt(storeTransaction.MaximumTransactions) < 1) {
                alert("ERROR: Please provide Maximum Transactions. ");
                return;
            }

            if ($scope.storeTransaction.StoreTransactionId < 1)
            {
                storeTransactionServices.addStoreTransaction(storeTransaction, $scope.processStoreTransactionCompleted);
            }
            else
            {
                storeTransactionServices.editStoreTransaction(storeTransaction, $scope.processStoreTransactionCompleted);
            }

        };
        
        $scope.processStoreTransactionCompleted = function (data)
        {
            if (data.Code < 1)
            {
                   alert(data.Error);

            }
            else
            {

                if ($scope.storeTransaction.StoreTransactionId < 1)
                {
                    alert("Subscription Package information was successfully added.");
                }
                else
                {
                    alert("Subscription Package information was successfully updated.");
                }
                   ngDialog.close('/ng-shopkeeper/Views/Store/StoreTransactions/ProcessStoreTransactions.html', '');
                   $scope.jtable.fnClearTable();
                   $scope.initializeController();
           }
        };

        $scope.getStoreTransaction = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeTransactionServices.getStoreTransaction(id, $scope.getStoreTransactionCompleted);
        };
        
        $scope.getStoreTransactionCompleted = function (data)
        {
            if (data.StoreTransactionId < 1)
            {
                alert("ERROR: StoreTransaction information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.storeTransaction = data;
                $scope.storeTransaction.Header = 'Update Subscription Package Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/StoreTransactions/ProcessStoreTransactions.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteStoreTransaction = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Subscription Package information will be deleted permanently. Continue?")) {
                    return;
                }
                storeTransactionServices.deleteStoreTransaction(id, $scope.deleteStoreTransactionCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteStoreTransactionCompleted = function (data)
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