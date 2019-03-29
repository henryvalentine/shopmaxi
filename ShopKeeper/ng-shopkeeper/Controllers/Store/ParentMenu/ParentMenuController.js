"use strict";

define(['application-configuration', 'parentMenuServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngParentMenuTable', function ($compile)
    {
        return function ($scope, ngCustomerTypeTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else
            {
                var tableOptions = {};
                tableOptions.sourceUrl = "/ParentMenu/GetParentMenuObjects";
                tableOptions.itemId = 'CustomerTypeId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableManager($scope, $compile, ngCustomerTypeTable, tableOptions, 'New Customer Type', 'prepareCustomerTypeTemplate', 'getCustomerType', 'deleteCustomerType', 163);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('parentMenuController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'parentMenuServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, parentMenuServices, alertsService)
    {
        $rootScope.applicationModule = "parentMenu";
        
        $scope.getAuthStatus = function ()
        {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function ()
        {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
       
        $scope.getRoles = function ()
        {
            parentMenuServices.getRoles($scope.getRolesCompleted());
        };
        
        $scope.getRolesCompleted = function (roles)
        {
            $scope.roles = roles;

        };

        $scope.initializeController = function ()
        {
           $scope.initializeMenu();
           $scope.getRoles();
           $scope.initialize();
        };
        
        $scope.initialize = function ()
        {
            $scope.menuList = [];
        };

        $scope.initializeMenu = function ()
        {
            $scope.parentMenu = { 'Value': '', 'Href': '', 'MenuOrder': '', 'Roles': [] };
            $scope.Header = 'New Parent Menu';
            $scope.selectedRoles = [];
            $scope.process = false;
        };

        $scope.prepareMenuTemplate = function ()
        {
            $scope.initializeMenu();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ParentMenu/ProcessParentMenu.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.addMenu = function ()
        {
           
            var parentMenu = { 'Value': $scope.parentMenu.Value, 'Href': $scope.parentMenu.Href, 'MenuOrder': $scope.parentMenu.MenuOrder, 'Roles': $scope.selectedRoles };
            
            if (parentMenu.Value == undefined || parentMenu.Value.length < 1)
            {
                alert("ERROR: Please provide Menu display Text. ");
                return;
            }
            
            if (parentMenu.Href == undefined || parentMenu.Href.length < 1) {
                alert("ERROR: Please provide Menu Link. ");
                return;
            }
            if (parentMenu.MenuOrder == undefined || parentMenu.MenuOrder < 1)
            {
                alert("ERROR: Please provide Menu Order. ");
                return;
            }
            
            if (parentMenu.Roles == undefined || parentMenu.Roles.length < 1)
            {
                alert("ERROR: Please provide at least one role to access the Menu. ");
                return;
            }
            
            $scope.menuList.push(parentMenu);
            ngDialog.close('/ng-shopkeeper/Views/Store/ParentMenu/ProcessParentMenu.html', '');
            $scope.initializeMenu();
        };
        
        $scope.addRole = function (status, role)
        {
             if (!status) {
                 if ($scope.selectedRoles != null && $scope.selectedRoles.length > 0)
                 {
                     angular.forEach($scope.selectedRoles, function(x, n) {
                         if (x == role) {
                             $scope.selectedRoles.splice(n, 1);
                         }
                     });
                 }
             } else {
                 $scope.selectedRoles.push(role);
             }
        };
        $scope.processParentMenuList = function ()
        {
            if ($scope.menuList == undefined || $scope.menuList.length < 1)
            {
                alert("ERROR: Please provide at least one Menu item. ");
                return;
            }

            $scope.menuList.push(parentMenu);
            parentMenuServices.addParentMenuList($scope.menuList, $scope.processParentMenuListCompleted);

        };
        
        $scope.processParentMenuListCompleted = function (data)
        {
            if (data.Code == null || data.Code < 1)
            {
               alert(data.Error);
            }
            else
            {
                alert("The process was successfully completed.");
                $scope.jtable.fnClearTable();
                $scope.initialize();
                $scope.initializeMenu();
             }
           };

        $scope.getParentMenu = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            parentMenuServices.getParentMenu(id, $scope.getParentMenuCompleted);
        };
        
        $scope.getParentMenuCompleted = function (data)
        {
            if (data.ParentMenuId < 1)
            {
                alert("ERROR: Menu information could not be retrieved! ");

            }
            else
            {
                $scope.initializeMenu();
                $scope.parentMenu = data;
                $scope.Header = 'Update Menu Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ParentMenu/ProcessParentMenu.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteParentMenu = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Menu information will be deleted permanently. Continue?"))
                {
                    return;
                }
                parentMenuServices.deleteParentMenu(id, $scope.deleteParentMenuCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteParentMenuCompleted = function (data)
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