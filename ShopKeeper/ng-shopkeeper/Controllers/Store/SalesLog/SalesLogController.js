"use strict";

define(['application-configuration', 'saleServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngSalesLog', function ($compile)
    {
        return function ($scope, ngSalesLog)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/Sales/GetMySaleObjects";
                tableOptions.itemId = 'SaleId';
                tableOptions.columnHeaders = ['DateStr', 'AmountDue', 'NumberSoldItems', 'RegisterName'];
                var ttc = salesLogManager($scope, $compile, ngSalesLog, tableOptions, 'New Sale', 'ngy.html#Store/Sales/Sales', 'getSaleDetails', 95);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.ngTable = ttc;
            }
        };
    });

    app.register.controller('salesLogController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'saleServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, saleServices, alertsService)
    {
       $rootScope.applicationModule = "Sale";
       $scope.getAuthStatus = function () {
           return $rootScope.isAuthenticated;
       };

       $scope.redir = function () {
           $rootScope.redirectUrl = $location.path();
           $location.path = "/ngy.html#signIn";
       };
       $scope.initializeController = function ()
       {
           $scope.negativefeedback = false;
           $scope.error = '';
           $scope.detail = false;
           $scope.success = '';
           $scope.positivefeedback = false;
       };
       
       $scope.getSaleDetails = function (saleId)
       {
           if (saleId < 1)
           {
               $scope.setError('Invalid Selection!');
               return;
           }

           saleServices.getSale(saleId, $scope.getSaleDetailsCompleted);
       };
        
       $scope.getSaleDetailsCompleted = function (data)
       {
           if (data == null || data.SaleId < 1)
           {
               $scope.setError('Sale Information could not be retrieved. Please try agan!');
               return;
           }
           $scope.sale = data;
           $scope.detail = true;
       };

       $scope.setError = function (errorMessage)
       {
           $scope.error = errorMessage;
           $scope.negativefeedback = true;
           $scope.success = '';
           $scope.positivefeedback = false;
       };
        
       $scope.setSuccessFeedback = function (successMessage)
       {
           $scope.error = '';
           $scope.negativefeedback = false;
           $scope.success = successMessage;
           $scope.positivefeedback = true;
       };


       $scope.printInvoice = function () {

           var printContents = document.getElementById('invoice').innerHTML;
           var popupWin = '';
           if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
               popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.window.focus();
               popupWin.document.write('<!DOCTYPE html><html><head>' +
                   '<link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" />' +
                   '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + printContents + '</div></div></html>');
               popupWin.onbeforeunload = function (event) {
                   popupWin.close();
                   return '.\n';
               };
               popupWin.onabort = function (event) {
                   popupWin.document.close();
                   popupWin.close();
               }
           } else {
               popupWin = window.open('', '_blank', 'width=800,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
               popupWin.document.open();
               popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + printContents + '</div></div></html>');
               popupWin.document.close();
           }
           popupWin.document.close();

           return true;
       };

    }]);
   
});

//#27ae60

//positivefeedback success negativefeedback error