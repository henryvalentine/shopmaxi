"use strict";

define(['application-configuration', 'storePaymentGatewayServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngPaymentGatewayTable', function ($compile)
    {
        return function ($scope, ngPaymentGatewayTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StorePaymentGateway/GetStorePaymentGatewayObjects";
                tableOptions.itemId = 'StorePaymentGatewayId';
                tableOptions.columnHeaders = ['GatewayGatewayName'];
                var ttc = tableManager($scope, $compile, ngPaymentGatewayTable, tableOptions, 'New Payment Gateway', 'preparePaymentGatewayTemplate', 'getPaymentGateway', 'deletePaymentGateway', 182);
                ttc.removeAttr('width').attr('width', '100%');
                $scope.ttc = ttc;
            }
        };
    });
    app.register.directive("ngPgfileSelect", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.file = (e.srcElement || e.target).files[0];
                    $scope.processFile();
                });
            }
        };
    });

    app.register.controller('storePaymentGatewayController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storePaymentGatewayServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storePaymentGatewayServices, $upload, fileReader, alertsService)
    {
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.paymentGateway = new Object();
            var sb = $scope.paymentGateway;
            sb.StorePaymentGatewayId = 0;
            sb.GatewayName = '';
            sb.Header = 'Create New Payment Gateway';
            sb.LogoPath = '/Content/images/noImage.png';
        };
        
        $scope.preparePaymentGatewayTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/PaymentGateways/ProcessPaymentGateways.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processPaymentGateway = function ()
        {
            var paymentGateway = new Object();
            paymentGateway.GatewayName = $scope.paymentGateway.GatewayName;
            if (paymentGateway.GatewayName == undefined || paymentGateway.GatewayName.length < 1 || paymentGateway.GatewayName == null)
            {
                alert("ERROR: Please provide Payment Gateway Name. ");
                return;
            }
            
            if ($scope.paymentGateway.StorePaymentGatewayId == 0 || $scope.paymentGateway.StorePaymentGatewayId < 1 || $scope.paymentGateway.StorePaymentGatewayId == undefined)
            {
                storePaymentGatewayServices.addStorePaymentGateway(paymentGateway, $scope.processPaymentGatewayCompleted);
            }
            else
            {
                storePaymentGatewayServices.editStorePaymentGateway(paymentGateway, $scope.processPaymentGatewayCompleted);
            }
        };

        $scope.processPaymentGatewayCompleted = function (data)
        {
            if (data.StorePaymentGatewayId < 1)
            {
                alert(data.GatewayName);

            }
            else
            {
                if ($scope.paymentGateway.StorePaymentGatewayId < 1)
                {
                    alert("Payment Gateway information was successfully added.");
                }
                else
                {
                    alert("Payment Gateway information was successfully updated.");
                }

                ngDialog.close('/ng-shopkeeper/Views/Store/paymentGateways/ProcessPaymentGateways.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };

        $scope.getPaymentGateway = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storePaymentGatewayServices.getStorePaymentGateway(id, $scope.getPaymentGatewayCompleted);
        };
       
        $scope.getPaymentGatewayCompleted = function (data)
        {
            if (data.StorePaymentGatewayId < 1)
            {
               alert("ERROR: Payment Gateway information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                $scope.paymentGateway = data;
                if (data.LogoPath != null)
                {
                    $scope.paymentGateway.LogoPath = data.LogoPath.replace('~', '');
                } else {
                    $scope.paymentGateway.LogoPath = '/Content/images/noImage.png';
                }
                $scope.paymentGateway.Header = 'Update Payment Gateway Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/paymentGateways/ProcessPaymentGateways.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
         };

        $scope.deletePaymentGateway = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Payment Gateway information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storePaymentGatewayServices.deleteStorePaymentGateway(id, $scope.deletePaymentGatewayCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deletePaymentGatewayCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.ttc.fnClearTable();
                alert(data.Error);
            }
        };

        $scope.processFile = function ()
        {
            $scope.progress = 0;
            fileReader.readAsDataUrl($scope.file, $scope)
                          .then(function (result)
                          {
                              $scope.paymentGateway.LogoPath = result;
                          });
            
            $upload.upload({
                url: "/StorePaymentGateway/CreateFileSession",
                method: "POST",
                data: { file: $scope.file },
            })
           .progress(function (evt)
           {
               $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
           }).success(function (data) {
               if (data.code < 1) {
                   alert('File could not be processed. Please try again later.');
               }
               
           });
        };
        
    }]);
    
});



