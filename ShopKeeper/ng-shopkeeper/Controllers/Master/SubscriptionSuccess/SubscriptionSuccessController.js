"use strict";

define(['application-configuration', 'subscriptionServices', 'alertsService'], function (app)
{
    app.register.directive('ngInvoice', function ($compile)
    {
        return function ($scope, ngInvoice)
        {
            $scope.invoice = ngInvoice;
        };
    });
  
    app.register.controller('subscriptionSuccessController', [ '$scope', '$rootScope', '$routeParams', 'subscriptionServices', '$location', '$http',
    function ($scope, $rootScope, $routeParams, subscriptionServices, $location, $http)
    {
        $scope.result = subscriptionServices.getresult();

        $scope.subsuccessmsg = true;

        $scope.printInvoice = function ()
        {
            var contents = $scope.invoice.html();
            var popupWin = '';
            if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
            {
                popupWin = window.open('', '_blank', 'width=600,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
                popupWin.window.focus();
                popupWin.document.write('<!DOCTYPE html><html><head>' +
                    '<link rel="stylesheet" type="text/css" href="/Content/feedbackmessage.css" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                    '</head><body onload="window.print()"><div class="row" style="background: whitesmoke; border: 2px solid #27ae60;">' + contents + '</div></html>');
                popupWin.onbeforeunload = function (event)
                {
                    popupWin.close();
                    return '.\n';
                };
                popupWin.onabort = function (event)
                {
                    popupWin.document.close();
                    popupWin.close();
                };
            }
            else
            {
                popupWin = window.open('', '_blank', 'width=800,height=600');
                popupWin.document.open();
                popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Content/feedbackmessage.css" /><link href="/Content/bootstrap.css" rel="stylesheet" /></head><body onload="window.print()"><div class="row" style="background: whitesmoke; border: 2px solid #27ae60;">'
                    + contents + '</div></html>');
                popupWin.document.close();
            }
            popupWin.document.close();

            return true;

        };
        
    }]);

})