"use strict";

define(['application-configuration', 'subscriptionPackageServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('subscriptionpackageTable', function ($compile)
    {
        return function ($scope, subscriptionpackageTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/SubscriptionPackage/GetSubscriptionPackageObjects";
            tableOptions.itemId = 'SubscriptionPackageId';
            tableOptions.columnHeaders = ['PackageTitle', 'DedicatedAccountManager', 'TelephoneSupport', 'EmailSupport', 'LiveChat'];
            var ttc = tableManager($scope, $compile, subscriptionpackageTable, tableOptions, 'New Package', 'prepareSubscriptionPackageTemplate', 'getSubscriptionPackage', 'deleteSubscriptionPackage', 125);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });
    
    app.register.controller('subscriptionPackageController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'subscriptionPackageServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, subscriptionPackageServices, alertsService)
    {
        $rootScope.applicationModule = "SubscriptionPackage";
        
        $scope.alerts = [];
        $scope.initializeController = function ()
        {
            $scope.subscriptionPackage = new Object();
            $scope.subscriptionPackage.SubscriptionPackageId = 0;
            $scope.subscriptionPackage.PackageTitle = '';
            $scope.subscriptionPackage.FileStorageSpace = '';
            $scope.subscriptionPackage.NumberOfStoreProducts = '';
            $scope.subscriptionPackage.NumberOfOutlets = '';
            $scope.subscriptionPackage.Registers = '';
            $scope.subscriptionPackage.NumberOfUsers = '';
            $scope.subscriptionPackage.UseReportBuilder = '';
            $scope.subscriptionPackage.GenerateReports = '';
            $scope.subscriptionPackage.MaximumCustomer = '';
            $scope.subscriptionPackage.TransactionFee = '';
            $scope.subscriptionPackage.LiveChat = '';
            $scope.subscriptionPackage.EmailSupport = '';
            $scope.subscriptionPackage.TelephoneSupport = '';
            $scope.subscriptionPackage.DedicatedAccountManager = '';
            $scope.subscriptionPackage.MaximumTransactions = '';
            $scope.subscriptionPackage.Note = '';
            $scope.subscriptionPackage.Header = 'Create New Subscription Package';
        };
        
        $scope.prepareSubscriptionPackageTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Master/SubscriptionPackages/ProcessSubscriptionPackages.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processSubscriptionPackage = function ()
        {
            var subscriptionPackage = new Object();

            subscriptionPackage.PackageTitle = $scope.subscriptionPackage.PackageTitle;
            subscriptionPackage.FileStorageSpace = $scope.subscriptionPackage.FileStorageSpace;
            subscriptionPackage.NumberOfStoreProducts = $scope.subscriptionPackage.NumberOfStoreProducts;
            subscriptionPackage.NumberOfOutlets = $scope.subscriptionPackage.NumberOfOutlets;
            subscriptionPackage.Registers = $scope.subscriptionPackage.Registers;
            subscriptionPackage.NumberOfUsers = $scope.subscriptionPackage.NumberOfUsers;
            subscriptionPackage.UseReportBuilder = $scope.subscriptionPackage.UseReportBuilder;
            subscriptionPackage.GenerateReports = $scope.subscriptionPackage.GenerateReports;
            subscriptionPackage.MaximumTransactions = $scope.subscriptionPackage.MaximumTransactions;
            subscriptionPackage.Note = $scope.subscriptionPackage.Note;
            subscriptionPackage.MaximumCustomer = $scope.subscriptionPackage.MaximumCustomer;
            subscriptionPackage.TransactionFee = $scope.subscriptionPackage.TransactionFee;
            subscriptionPackage.LiveChat = $scope.subscriptionPackage.LiveChat;
            subscriptionPackage.EmailSupport = $scope.subscriptionPackage.EmailSupport;
            subscriptionPackage.TelephoneSupport = $scope.subscriptionPackage.TelephoneSupport;
            subscriptionPackage.DedicatedAccountManager = $scope.subscriptionPackage.DedicatedAccountManager;
            
            if (subscriptionPackage.PackageTitle == undefined || subscriptionPackage.PackageTitle == null || subscriptionPackage.PackageTitle.length < 1)
            {
                alert("ERROR: Please provide Subscription Package Title. ");
                return;
            }
            
            if (subscriptionPackage.FileStorageSpace == NaN || subscriptionPackage.FileStorageSpace == null)
            {
                alert("ERROR: Please provide a valid value for File Storage Space. ");
                return;
            }

            if (subscriptionPackage.NumberOfStoreProducts == undefined || subscriptionPackage.NumberOfStoreProducts == NaN)
            {
                alert("ERROR: Please provide a valid value for Maximum Number Of Store Products. ");
                return;
            }

            if (subscriptionPackage.NumberOfOutlets == undefined || subscriptionPackage.NumberOfOutlets == null || subscriptionPackage.NumberOfOutlets == NaN)
            {
                alert("ERROR: Please provide a valid value for Maximum Number Of Outlets. ");
                return;
            }
            
            if (subscriptionPackage.Registers == undefined || subscriptionPackage.Registers == null || subscriptionPackage.Registers == NaN)
            {
                alert("ERROR: Please provide a valid value for Number Maximum Number of Registers. ");
                return;
            }
            
            if (subscriptionPackage.NumberOfUsers == undefined || subscriptionPackage.NumberOfUsers == null || subscriptionPackage.NumberOfUsers == NaN)
            {
                alert("ERROR: Please provide a valid value for Maximum Number Of Users.");
                return;
            }

            if (subscriptionPackage.MaximumTransactions == undefined || subscriptionPackage.MaximumTransactions == null || parseInt(subscriptionPackage.MaximumTransactions) == NaN)
            {
                alert("ERROR: Please provide  a valid value for Maximum Transactions. ");
                return;
            }

            if ($scope.subscriptionPackage.SubscriptionPackageId < 1)
            {
                subscriptionPackageServices.addSubscriptionPackage(subscriptionPackage, $scope.processSubscriptionPackageCompleted);
            }
            else
            {
                subscriptionPackageServices.editSubscriptionPackage(subscriptionPackage, $scope.processSubscriptionPackageCompleted);
            }

        };
        
        $scope.processSubscriptionPackageCompleted = function (data)
        {
            if (data.Code < 1)
            {
                   alert(data.Error);

            }
            else
            {

                if ($scope.subscriptionPackage.SubscriptionPackageId < 1)
                {
                    alert("Subscription Package information was successfully added.");
                }
                else
                {
                    alert("Subscription Package information was successfully updated.");
                }
                   ngDialog.close('/ng-shopkeeper/Views/Master/SubscriptionPackages/ProcessSubscriptionPackages.html', '');
                   $scope.jtable.fnClearTable();
                   $scope.initializeController();
           }
        };

        $scope.getSubscriptionPackage = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            subscriptionPackageServices.getSubscriptionPackage(id, $scope.getSubscriptionPackageCompleted);
        };
        
        $scope.getSubscriptionPackageCompleted = function (data)
        {
            if (data.SubscriptionPackageId < 1)
            {
                alert("ERROR: SubscriptionPackage information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.subscriptionPackage = data;
                $scope.subscriptionPackage.NumberOfStoreProducts = data.NumberOfStoreProducts;
                $scope.subscriptionPackage.Header = 'Update Subscription Package Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Master/SubscriptionPackages/ProcessSubscriptionPackages.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteSubscriptionPackage = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Subscription Package information will be deleted permanently. Continue?")) {
                    return;
                }
                subscriptionPackageServices.deleteSubscriptionPackage(id, $scope.deleteSubscriptionPackageCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteSubscriptionPackageCompleted = function (data)
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

})