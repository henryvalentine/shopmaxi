"use strict";

define(['application-configuration', 'paymentGatewayServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngPaymentGatewayTable', function ($compile)
    {
        return function ($scope, ngPaymentGatewayTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/PaymentGateway/GetPaymentGatewayObjects";
            tableOptions.itemId = 'PaymentGatewayId';
            tableOptions.columnHeaders = ['GatewayGatewayName'];
            var ttc = tableManager($scope, $compile, ngPaymentGatewayTable, tableOptions, 'New Payment Gateway', 'preparePaymentGatewayTemplate', 'getPaymentGateway', 'deletePaymentGateway', 182);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.ttc = ttc;
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

    app.register.controller('paymentGatewayController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'paymentGatewayServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, paymentGatewayServices, $upload, fileReader, alertsService)
    {
        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.paymentGateway = new Object();
            var sb = $scope.paymentGateway;
            sb.PaymentGatewayId = 0;
            sb.GatewayName = '';
            sb.Header = 'Create New Payment Gateway';
            sb.LogoPath = '/Content/images/noImage.png';
        };
        
        $scope.preparePaymentGatewayTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Master/PaymentGateways/ProcessPaymentGateways.html',
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
            
            if ($scope.paymentGateway.PaymentGatewayId == 0 || $scope.paymentGateway.PaymentGatewayId < 1 || $scope.paymentGateway.PaymentGatewayId == undefined)
            {
                paymentGatewayServices.addPaymentGateway(paymentGateway, $scope.processPaymentGatewayCompleted);
            }
            else
            {
                paymentGatewayServices.editPaymentGateway(paymentGateway, $scope.processPaymentGatewayCompleted);
            }
        };

        $scope.processPaymentGatewayCompleted = function (data)
        {
            if (data.PaymentGatewayId < 1)
            {
                alert(data.GatewayName);

            }
            else
            {
                if ($scope.paymentGateway.PaymentGatewayId < 1)
                {
                    alert("Payment Gateway information was successfully added.");
                }
                else
                {
                    alert("Payment Gateway information was successfully updated.");
                }

                ngDialog.close('/ng-shopkeeper/Views/Master/paymentGateways/ProcessPaymentGateways.html', '');
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

            paymentGatewayServices.getPaymentGateway(id, $scope.getPaymentGatewayCompleted);
        };
       
        $scope.getPaymentGatewayCompleted = function (data)
        {
            if (data.PaymentGatewayId < 1)
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
                    template: '/ng-shopkeeper/Views/Master/paymentGateways/ProcessPaymentGateways.html',
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
                paymentGatewayServices.deletePaymentGateway(id, $scope.deletePaymentGatewayCompleted);
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
                url: "/PaymentGateway/CreateFileSession",
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



