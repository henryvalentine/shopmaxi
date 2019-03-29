"use strict";

define(['application-configuration', 'unitsOfMeasurementServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngUomTable', function ($compile)
    {
        return function ($scope, ngUomTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/UnitOfMeasurement/GetUnitOfMeasurementObjects";
                tableOptions.itemId = 'UnitOfMeasurementId';
                tableOptions.columnHeaders = ['UoMCode'];
                var ttc = tableManager($scope, $compile, ngUomTable, tableOptions, 'New Unit of Measurement', 'prepareUnitOfMeasurementTemplate', 'getUnitOfMeasurement', 'deleteUnitOfMeasurement', 200);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('uomController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'unitsOfMeasurementServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, unitsOfMeasurementServices, alertsService)
    {
        $rootScope.applicationModule = "uom";
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
            $scope.uom = new Object();
            $scope.uom.UnitOfMeasurementId = 0;
            $scope.uom.UoMCode = '';
            $scope.uom.UoMDescription = '';
            $scope.uom.Header = 'Create New Unit of Measurement';
        };
        
        $scope.prepareUnitOfMeasurementTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/UnitsOfMeasurement/ProcessUnitsOfMeasurement.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processUoM = function ()
        {
            var uom = new Object();
            uom.UoMCode = $scope.uom.UoMCode;
            uom.UoMDescription = $scope.uom.UoMDescription;
            
            if (uom.UoMCode == undefined || uom.UoMCode.length < 1)
            {
                alert("ERROR: Please provide Product Unit of Measurement UoMCode. ");
                return;
            }

            if ($scope.uom.UnitOfMeasurementId < 1)
            {
                unitsOfMeasurementServices.addUnitOfMeasurement(uom, $scope.processUoMCompleted);
            }
            else
            {
                unitsOfMeasurementServices.editUnitOfMeasurement(uom, $scope.processUoMCompleted);
            }

        };
        
        $scope.processUoMCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
                return;

            }
            
              alert(data.Error);
               ngDialog.close('/ng-shopkeeper/Views/Store/UnitsOfMeasurement/ProcessUnitsOfMeasurement.html', '');
               $scope.jtable.fnClearTable();
               $scope.initializeController();
               
           };

        $scope.getUnitOfMeasurement = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            unitsOfMeasurementServices.getUnitOfMeasurement(id, $scope.getUnitOfMeasurementCompleted);
        };
        
        $scope.getUnitOfMeasurementCompleted = function (data)
        {
            if (data.UnitOfMeasurementId < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.initializeController();
                $scope.uom = data;
                $scope.uom.Header = 'Update Unit of Measurement Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/UnitsOfMeasurement/ProcessUnitsOfMeasurement.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteUnitOfMeasurement = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Unit of Measurement information will be deleted permanently. Continue?"))
                {
                    return;
                }
                unitsOfMeasurementServices.deleteUnitOfMeasurement(id, $scope.deleteUnitOfMeasurementCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteUnitOfMeasurementCompleted = function (data)
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