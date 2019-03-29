"use strict";

define(['application-configuration', 'accountGroupServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngAccountGroupTable', function ($compile)
    {
        return function ($scope, ngAccountGroupTable)
        {
            var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/AccountGroup/GetAccountGroupObjects";
                tableOptions.itemId = 'AccountGroupId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableManager($scope, $compile, ngAccountGroupTable, tableOptions, 'New Account Group', 'prepareAccountGroupsTemplate', 'getAccountGroup', 'deleteAccountGroup', 163);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('accountGroupController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'accountGroupServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, accountGroupServices, alertsService)
    {
        $rootScope.applicationModule = "accountGroup";
       
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
            $scope.accountGroup = new Object();
            $scope.accountGroup.AccountGroupId = 0;
            $scope.accountGroup.Name = '';
            $scope.accountGroup.Header = 'Create New Account Group';
        };
        
        $scope.prepareAccountGroupsTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/AccountGroups/ProcessAccountGroups.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processAccountGroup = function ()
        {
            var accountGroup = new Object();
            accountGroup.Name = $scope.accountGroup.Name;
            
            if (accountGroup.Name == undefined || accountGroup.Name.length < 1)
            {
                alert("ERROR: Please provide Product Account Group Name. ");
                return;
            }

            if ($scope.accountGroup.AccountGroupId < 1)
            {
                accountGroupServices.addAccountGroup(accountGroup, $scope.processAccountGroupCompleted);
            }
            else
            {
                accountGroupServices.editAccountGroup(accountGroup, $scope.processAccountGroupCompleted);
            }

        };
        
        $scope.processAccountGroupCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
                return;

            }
              alert(data.Error);
               ngDialog.close('/ng-shopkeeper/Views/Store/AccountGroups/ProcessAccountGroups.html', '');
               $scope.jtable.fnClearTable();
               $scope.initializeController();
               
           };

        $scope.getAccountGroup = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            accountGroupServices.getAccountGroup(id, $scope.getAccountGroupCompleted);
        };
        
        $scope.getAccountGroupCompleted = function (data)
        {
            if (data.AccountGroupId < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.initializeController();
                $scope.accountGroup = data;
                $scope.accountGroup.Header = 'Update Account Group Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/AccountGroups/ProcessAccountGroups.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteAccountGroup = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Account Group information will be deleted permanently. Continue?"))
                {
                    return;
                }
                accountGroupServices.deleteAccountGroup(id, $scope.deleteAccountGroupCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteAccountGroupCompleted = function (data)
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