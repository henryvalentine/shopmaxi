"use strict";

define(['application-configuration', 'customerTypeServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngCustomerTypeTable', function ($compile)
    {
        return function ($scope, ngCustomerTypeTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreCustomerType/GetStoreCustomerTypeObjects";
                tableOptions.itemId = 'CustomerTypeId';
                tableOptions.columnHeaders = ['Name', 'Code', 'CreditWorthy', 'CreditLimitStr'];
                var ttc = tableManager($scope, $compile, ngCustomerTypeTable, tableOptions, 'New Customer Type', 'prepareCustomerTypeTemplate', 'getCustomerType', 'deleteCustomerType', 163);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        }; 
    });

    app.register.controller('customerTypeController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'customerTypeServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, customerTypeServices, alertsService)
    {
        $rootScope.applicationModule = "customerType";
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.alerts = [];
        $scope.initializeController = function () {
            $scope.customerType = { CustomerTypeId: 0, Name: '', Code: '', Description: '', CreditWorthy: false, CreditLimit: '' };
            $scope.customerType.Header = 'Create New Customer Type';
        };
        
        $scope.prepareCustomerTypeTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/CustomerTypes/ProcessCustomerTypes.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processCustomerType = function ()
        {
            if ($scope.customerType.Name == undefined || $scope.customerType.Name.length < 1)
            {
                alert("ERROR: Please provide Product Customer Type Name. ");
                return;
            }
            
            if ($scope.customerType.Code == undefined || $scope.customerType.Code.length < 1)
            {
                alert("ERROR: Please provide Product Customer Type Code. ");
                return;
            }

            if ($scope.customerType.CreditWorthy !== undefined && $scope.customerType.CreditWorthy === true) {
                if ($scope.customerType.CreditLimit === undefined || $scope.customerType.CreditLimit === null || $scope.customerType.CreditLimit < 1)
                {
                    alert("ERROR: Please provide credit limit");
                    return;
                }
            }
            else
            {
                $scope.customerType.CreditLimit = 0;
            }

            if ($scope.customerType.CustomerTypeId < 1)
            {
                customerTypeServices.addCustomerType($scope.customerType, $scope.processCustomerTypeCompleted);
            }
            else
            {
                customerTypeServices.editCustomerType($scope.customerType, $scope.processCustomerTypeCompleted);
            }

        };
        
        $scope.processCustomerTypeCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {

                if ($scope.customerType.CustomerTypeId < 1)
                {
                    alert("Customer Type information was successfully added.");
                }
                else
                {
                    alert("Customer Type information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/CustomerTypes/ProcessCustomerTypes.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getCustomerType = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            customerTypeServices.getCustomerType(id, $scope.getCustomerTypeCompleted);
        };
        
        $scope.getCustomerTypeCompleted = function (data)
        {
            if (data.CustomerTypeId < 1)
            {
                alert("ERROR: Customer Type information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.customerType = data;
                $scope.customerType.Header = 'Update Customer Type Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/CustomerTypes/ProcessCustomerTypes.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteCustomerType = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Customer Type information will be deleted permanently. Continue?"))
                {
                    return;
                }
                customerTypeServices.deleteCustomerType(id, $scope.deleteCustomerTypeCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteCustomerTypeCompleted = function (data)
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