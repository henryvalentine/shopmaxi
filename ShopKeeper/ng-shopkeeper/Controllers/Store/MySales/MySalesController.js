"use strict";

define(['application-configuration', 'reportServices'], function (app)
{
    app.register.directive('ngDefault', function ($compile)
    {
        return function ($scope, ngDefault)
        {
            $scope.compiler = $compile;
        };

    });
    app.register.controller('mySalesController', ['$scope', '$rootScope', '$routeParams', 'reportServices', '$http',
    function ($scope, $rootScope, $routeParams, reportServices)
    {
        setControlDate($scope, '', '');
        setEndDate($scope, '', '');
       
        $scope.initializeController = function () 
        {
            $scope.details = false;
            $scope.reportChoice = '';
        };

        $scope.validateDates = function ()
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.lenght < 1)
            {
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.lenght < 1)
            {
                return;
            }

            var date1 = new Date($scope.startDate);
            var date2 = new Date($scope.endDate);

            if (date1 > date2)
            {
                alert('The start date must not be later than the end date.');
                $scope.startDate = null;
                $scope.endDate = null;
                return;
            } else {
                $rootScope.getEmployeeReports();
            }
        };

        $rootScope.getEmployeeReports = function ()
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.lenght < 1) 
            {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.lenght < 1)
            {
                alert('Please select date range');
                return;
            }

            var date1 = new Date($scope.startDate);
            var date2 = new Date($scope.endDate);

            if (date1 > date2)
            {
                alert('The start date must not be later than the end date.');
                $scope.startDate = null;
                $scope.endDate = null;
                return;
            }

            var year1 = date1.getFullYear();
            var month1 = date1.getMonth() + 1;
            var day1 = date1.getDate();

            var year2 = date2.getFullYear();
            var month2 = date2.getMonth() + 1;
            var day2 = date2.getDate();

            var startDate = year1 + '/' + month1 + '/' + day1;
            var endDate = year2 + '/' + month2 + '/' + day2;
            
            if ($scope.empTableDir !== undefined && $scope.empTableDir != null)
            {
                $scope.empReport = false;
                $scope.empTableDir.fnClearTable();
            }
            else
            {
                $scope.processing = true;
                $scope.empReport = true;
                var tableDir = angular.element('#empRepTbl');
                var tableOptions = {};
                tableOptions.sourceUrl = '/Report/GetMySalesReport' + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                tableOptions.itemId = 'SaleId';
                tableOptions.columnHeaders = ['StoreItemName', 'AmountDue', 'AmountPaidStr', 'DateStr'];
                var empTableDir = employeeReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getReport');
                $scope.empTableDir = empTableDir;
            }
        }
        
        $scope.getReport = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            reportServices.getProduct(id, $scope.getProductCompleted);
        };
       
        $scope.getProductCompleted = function (data)
        {
            if (data.SaleId < 1)
            {
                alert("Report information could not be retrieved");
                return;
            }
            $scope.sale = data;
            $scope.details = true;
        };
      
        $scope.printReport = function () {
            var printContents = document.getElementById('rep').innerHTML;
            var popupWin = '';
            if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
            {
                popupWin = window.open('', '_blank', 'width=500,height=700,scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,titlebar=yes');
                popupWin.window.focus();
                popupWin.document.write('<!DOCTYPE html><html><head>' +
                    '<link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />' +
                    '</head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + printContents + '</div></div></html>');
                popupWin.onbeforeunload = function (event)
                {
                    popupWin.close();
                    return '.\n';
                };
                popupWin.onabort = function (event)
                {
                    popupWin.document.close();
                    popupWin.close();
                }
            }
            else {
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




