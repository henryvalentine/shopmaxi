"use strict";

define(['application-configuration', 'chartOfAccountServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngChartOfAccountTable', function ($compile)
    {
        return function ($scope, ngChartOfAccountTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/ChartOfAccount/GetChartOfAccountObjects";
                tableOptions.itemId = 'ChartOfAccountId';
                tableOptions.columnHeaders = ['AccountType', 'AccountCode', 'AccountGroupName'];
                var ttc = tableManager($scope, $compile, ngChartOfAccountTable, tableOptions, 'New Chart Of Account', 'prepareChartOfAccountTemplate', 'getChartOfAccount', 'deleteChartOfAccount', 177);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('chartOfAccountController', ['ngDialog','$scope', '$rootScope', '$routeParams', 'chartOfAccountServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, chartOfAccountServices, alertsService)
    {
        $rootScope.applicationModule = "ChartOfAccount";

        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };

        var accountGroups = [];
       $scope.initializeController = function ()
       {
           $scope.AccountGroups = [];
           if (accountGroups.length < 1)
           {
               chartOfAccountServices.getAccountGroups($scope.getAccountGroupCompleted);
           }
           else {
               $scope.accountGroups = accountGroups;
           }
           $scope.chartOfAccount = new Object();
           $scope.chartOfAccount.ChartOfAccountId = 0;
           $scope.chartOfAccount.AccountType = '';
           $scope.chartOfAccount.AccountCode = '';
           $scope.chartOfAccount.AccountGroup = new Object();
           $scope.chartOfAccount.AccountGroup.AccountGroupId = '';
           $scope.chartOfAccount.AccountGroup.Name = '';
           

       };

       $scope.getAccountGroupCompleted = function (data)
       {
           $scope.accountGroups = data;
           accountGroups = data;
       };

       $scope.prepareChartOfAccountTemplate = function ()
       {
           $scope.initializeController();
           $scope.chartOfAccount.Header = 'Create New Chart Of Account';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/ChartsOfAccount/ProcessChartsOfAccount.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processChartOfAccount = function ()
       {
           var chartOfAccount = new Object();
           chartOfAccount.AccountType = $scope.chartOfAccount.AccountType;
           chartOfAccount.AccountCode = $scope.chartOfAccount.AccountCode;
           chartOfAccount.AccountGroupId = $scope.chartOfAccount.AccountGroup.AccountGroupId;
           
           if(chartOfAccount.AccountType == undefined || chartOfAccount.AccountType.length < 1)
           {
               alert("ERROR: Please provide Account Type. ");
               return;
           }
           
           if (chartOfAccount.AccountCode == undefined || chartOfAccount.AccountCode.length < 1) {
               alert("ERROR: Please provide Account Code. ");
               return;
           }

           if (chartOfAccount.AccountGroupId == undefined || chartOfAccount.AccountGroupId < 1)
           {
               alert("ERROR: Please select a valid Account Group. ");
               return;
           }
           if ($scope.chartOfAccount.ChartOfAccountId < 1)
           {
               chartOfAccountServices.addChartOfAccount(chartOfAccount, $scope.processChartOfAccountCompleted);
           }
           else
           {
               chartOfAccountServices.editChartOfAccount(chartOfAccount, $scope.processChartOfAccountCompleted);
           }

       };

       $scope.processChartOfAccountCompleted = function (data)
       {
           if (data.ChartOfAccountId < 1)
           {
                alert(data.AccountType);

           }
           else
           {
               if ($scope.chartOfAccount.ChartOfAccountId < 1)
               {
                   alert(data.Error);
               }
               else
               {
                   alert(data.Error);
               }
                ngDialog.close('/ng-shopkeeper/Views/Store/ChartsOfAccount/ProcessChartsOfAccount.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
            }
        };

       $scope.getChartOfAccount = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           chartOfAccountServices.getChartOfAccount(id, $scope.getChartOfAccountCompleted);
       };

       $scope.getChartOfAccountCompleted = function (data)
       {
           if (data.ChartOfAccountId < 1)
           {
             alert("ERROR: Item information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.chartOfAccount = data;
                $scope.chartOfAccount.AccountGroup = {};
                $scope.chartOfAccount.AccountGroup.AccountGroupId = data.AccountGroupId;
                $scope.chartOfAccount.AccountGroup.Name = '';
                $scope.chartOfAccount.Header = 'Update Chart Of Account Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ChartsOfAccount/ProcessChartsOfAccount.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
       $scope.deleteChartOfAccount = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This Chart Of Account information will be deleted permanently. Continue?"))
               {
                   return;
               }
               chartOfAccountServices.deleteChartOfAccount(id, $scope.deleteChartOfAccountCompleted);
           } else {
               alert('Invalid selection.');
           }
       };

       $scope.deleteChartOfAccountCompleted = function (data)
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

