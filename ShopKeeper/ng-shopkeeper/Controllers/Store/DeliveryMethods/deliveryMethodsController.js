"use strict";

define(['application-configuration', 'deliveryMethodServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngDeliveryMethodTable', function ($compile)
    {
        return function ($scope, deliveryMethodTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/DeliveryMethod/GetDeliveryMethodObjects";
                tableOptions.itemId = 'DeliveryMethodId';
                tableOptions.columnHeaders = ['MethodTitle'];
                var ttc = tableManager($scope, $compile, deliveryMethodTable, tableOptions, 'New Delivery Method', 'prepareDeliveryMethodTemplate', 'getDeliveryMethod', 'deleteDeliveryMethod', 170);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('deliveryMethodController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'deliveryMethodServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, deliveryMethodServices, alertsService)
    {
        $rootScope.applicationModule = "deliveryMethod";
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
            $scope.deliveryMethod = new Object();
            $scope.deliveryMethod.DeliveryMethodId = 0;
            $scope.deliveryMethod.MethodTitle = '';
            $scope.deliveryMethod.Description = '';
            $scope.deliveryMethod.Header = 'Create New Delivery Method';
        };
        
        $scope.prepareDeliveryMethodTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/DeliveryMethods/ProcessDeliveryMethods.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processDeliveryMethod = function ()
        {
            var deliveryMethod = new Object();
            deliveryMethod.MethodTitle = $scope.deliveryMethod.MethodTitle;
            deliveryMethod.Description = $scope.deliveryMethod.Description;
            
            if (deliveryMethod.MethodTitle == undefined || deliveryMethod.MethodTitle.length < 1)
            {
                alert("ERROR: Please provide Delivery Method Title. ");
                return;
            }

            if ($scope.deliveryMethod.DeliveryMethodId < 1)
            {
                deliveryMethodServices.addDeliveryMethod(deliveryMethod, $scope.processDeliveryMethodCompleted);
            }
            else
            {
                deliveryMethodServices.editDeliveryMethod(deliveryMethod, $scope.processDeliveryMethodCompleted);
            }

        };
        
        $scope.processDeliveryMethodCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {

                if ($scope.deliveryMethod.DeliveryMethodId < 1)
                {
                    alert("Delivery Method information was successfully added.");
                }
                else
                {
                    alert("Delivery Method information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/DeliveryMethods/ProcessDeliveryMethods.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getDeliveryMethod = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            deliveryMethodServices.getDeliveryMethod(id, $scope.getDeliveryMethodCompleted);
        };
        
        $scope.getDeliveryMethodCompleted = function (data)
        {
            if (data.DeliveryMethodId < 1)
            {
                alert("ERROR: Delivery Method information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.deliveryMethod = data;
                $scope.deliveryMethod.Header = 'Update Delivery Method Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/DeliveryMethods/ProcessDeliveryMethods.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteDeliveryMethod = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Delivery Method information will be deleted permanently. Continue?"))
                {
                    return;
                }
                deliveryMethodServices.deleteDeliveryMethod(id, $scope.deleteDeliveryMethodCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteDeliveryMethodCompleted = function (data)
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