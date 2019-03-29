"use strict";

define(['application-configuration', 'storeDepartmentServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngStoreDepartmentTable', function ($compile)
    {
        return function ($scope, ngStoreDepartmentTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreDepartment/GetStoreDepartmentObjects";
                tableOptions.itemId = 'StoreDepartmentId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableManager($scope, $compile, ngStoreDepartmentTable, tableOptions, 'New Department', 'prepareStoreDepartmentTemplate', 'getStoreDepartment', 'deleteStoreDepartment', 143);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('storeDepartmentController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeDepartmentServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeDepartmentServices, alertsService)
    {
        $rootScope.applicationModule = "department";
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
            $scope.department = new Object();
            $scope.department.StoreDepartmentId = 0;
            $scope.department.Name = '';
            $scope.department.Description = '';
            $scope.department.Header = 'Create New Department';
        };
        
        $scope.prepareStoreDepartmentTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Departments/ProcessDepartments.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processStoreDepartment = function ()
        {
            var department = new Object();
            department.Name = $scope.department.Name;
            department.Description = $scope.department.Description;
            
            if (department.Name == undefined || department.Name.length < 1)
            {
                alert("ERROR: Please provide Department Title. ");
                return;
            }

            if ($scope.department.StoreDepartmentId < 1)
            {
                storeDepartmentServices.addStoreDepartment(department, $scope.processStoreDepartmentCompleted);
            }
            else
            {
                storeDepartmentServices.editStoreDepartment(department, $scope.processStoreDepartmentCompleted);
            }

        };
        
        $scope.processStoreDepartmentCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {

                if ($scope.department.StoreDepartmentId < 1)
                {
                    alert("Department information was successfully added.");
                }
                else
                {
                    alert("Department information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/Departments/ProcessDepartments.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getStoreDepartment = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeDepartmentServices.getStoreDepartment(id, $scope.getStoreDepartmentCompleted);
        };
        
        $scope.getStoreDepartmentCompleted = function (data)
        {
            if (data.StoreDepartmentId < 1)
            {
                alert("ERROR: Department information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.department = data;
                $scope.department.Header = 'Update Department Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Departments/ProcessDepartments.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteStoreDepartment = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Department information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storeDepartmentServices.deleteStoreDepartment(id, $scope.deleteStoreDepartmentCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteStoreDepartmentCompleted = function (data)
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