"use strict";

define(['application-configuration', 'billingCycleServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('billingcycleTable', function ($compile)
    {
        return function ($scope, billingcycleTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/BillingCycle/GetBillingCycleObjects";
            tableOptions.itemId = 'BillingCycleId';
            tableOptions.columnHeaders = ['Name', 'Code', 'Duration'];
            var ttc = tableManager($scope, $compile, billingcycleTable, tableOptions, 'New BillingCycle', 'prepareBillingCycleTemplate', 'getBillingCycle', 'deleteBillingCycle', 140);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });

    app.register.controller('manageBillingCycleCntroller', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'billingCycleServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, billingCycleServices, alertsService) {
        
        $rootScope.applicationModule = "BillingCycle";
        
        $scope.alerts = [];
        $scope.initializeController = function ()
        {
            $scope.billingCycle = new Object();
            $scope.billingCycle.BillingCycleId = 0;
            $scope.billingCycle.Name = '';
            $scope.billingCycle.Code = '';
            $scope.billingCycle.Duration = '';
            $scope.billingCycle.Remark = '';
            $scope.billingCycle.Header = 'Create New Billing Cycle';
        };
        $scope.prepareBillingCycleTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Master/BillingCycles/ProcessBillingCycles.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processBillingCycle = function ()
        {
            var billingCycle = new Object();
            billingCycle.Name = $scope.billingCycle.Name;
            billingCycle.Code = $scope.billingCycle.Code;
            billingCycle.Duration = $scope.billingCycle.Duration;
            if ($scope.billingCycle.Remark != undefined || $scope.billingCycle.Remark != null || $scope.billingCycle.length > 0)
            {
                billingCycle.Remark = $scope.billingCycle.Remark;
            }
            
            if (billingCycle.Name == undefined || billingCycle.Name.length < 1)
            {
                alert("ERROR: Please provide Billing Cycle Name. ");
                return;
            }

            if ($scope.billingCycle.BillingCycleId < 1)
            {
                billingCycleServices.addBillingCycle(billingCycle, $scope.processBillingCycleCompleted);
            }
            else {
                billingCycleServices.editBillingCycle(billingCycle, $scope.processBillingCycleCompleted);
            }

        };
        
        $scope.processBillingCycleCompleted = function (data)
        {
            if (data.BillingCycleId < 1)
            {
                   alert(data.Name);

               }
               else {

                if ($scope.billingCycle.BillingCycleId < 1)
                {
                       alert("Billing Cycle information was successfully added.");
                   } else {
                       alert("Billing Cycle information was successfully updated.");
                   }
                   ngDialog.close('/ng-shopkeeper/Views/Master/BillingCycles/ProcessBillingCycles.html', '');
                   $scope.jtable.fnClearTable();
                   $scope.initializeController();
               }
           };

        $scope.getBillingCycle = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            billingCycleServices.getBillingCycle(id, $scope.getBillingCycleCompleted);
        };
        
        $scope.getBillingCycleCompleted = function (data)
        {
            if (data.BillingCycleId < 1)
            {
                alert("ERROR: BillingCycle information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.billingCycle = data;
                $scope.billingCycle.Header = 'Update Billing Cycle Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Master/BillingCycles/ProcessBillingCycles.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteBillingCycle = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Billing Cycle information will be deleted permanently. Continue?"))
                {
                    return;
                }
                billingCycleServices.deleteBillingCycle(id, $scope.deleteBillingCycleCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteBillingCycleCompleted = function (data)
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