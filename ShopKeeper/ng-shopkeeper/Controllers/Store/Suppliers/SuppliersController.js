"use strict";

define(['application-configuration', 'supplierServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngSupplierTable', function ($compile)
    {
        return function ($scope, ngSupplierTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/Supplier/GetSupplierObjects";
                tableOptions.itemId = 'SupplierId';
                tableOptions.columnHeaders = ['CompanyName', 'TIN', 'DateRegistered'];
                var ttc = tableManager($scope, $compile, ngSupplierTable, tableOptions, 'New Supplier', 'prepareSupplierTemplate', 'getSupplier', 'deleteSupplier', 120);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('supplierController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'supplierServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, supplierServices, alertsService)
    {
        $rootScope.applicationModule = "supplier";
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
            var i = new Date();
            var year = i.getFullYear();
            var month = i.getMonth() + 1;
            var day = i.getDate();
            var minDate = year + '/' + month + '/' + day;

            //Date Received for invoice
            setEndDate($scope, '', minDate);

            $scope.supplier = new Object();
            $scope.supplier.SupplierId = 0;
            $scope.supplier.CompanyName = '';
            $scope.supplier.DateRegistered = '';
            $scope.supplier.TIN = '';
            $scope.supplier.Note = '';
            $scope.supplier.Header = 'Create New Supplier';
        };
        
        $scope.prepareSupplierTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Suppliers/ProcessSuppliers.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processSupplier = function ()
        {
            var supplier = new Object();
            supplier.CompanyName = $scope.supplier.CompanyName;
            supplier.Note = $scope.supplier.Note;
            supplier.TIN = $scope.supplier.TIN;
            supplier.DateRegistered = $scope.supplier.DateRegistered;
            if (supplier.CompanyName == undefined || supplier.CompanyName.length < 1)
            {
                alert("ERROR: Please provide Product Supplier Name. ");
                return;
            }
 
            if (parseInt($scope.supplier.SupplierId) < 1)
            {
                supplierServices.addSupplier(supplier, $scope.processSupplierCompleted);
            }
            else
            {
                supplierServices.editSupplier(supplier, $scope.processSupplierCompleted);
            }

        };
        
        $scope.processSupplierCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {
                if ($scope.supplier.SupplierId < 1)
                {
                    alert("Supplier information was successfully added.");
                }
                else
                {
                    alert("Supplier information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/Suppliers/ProcessSuppliers.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getSupplier = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            supplierServices.getSupplier(id, $scope.getSupplierCompleted);
        };
        
        $scope.getSupplierCompleted = function (data)
        {
            if (data.SupplierId < 1)
            {
                alert("ERROR: Supplier information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.supplier = data;
                $scope.supplier.Header = 'Update Supplier Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Suppliers/ProcessSuppliers.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteSupplier = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Supplier information will be deleted permanently. Continue?"))
                {
                    return;
                }
                supplierServices.deleteSupplier(id, $scope.deleteSupplierCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteSupplierCompleted = function (data)
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