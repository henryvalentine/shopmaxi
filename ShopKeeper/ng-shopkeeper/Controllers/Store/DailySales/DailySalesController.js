"use strict";

define(['application-configuration', 'salesReportServices'], function (app)
{
    app.register.directive('ngDefault', function ($compile)
    {
        return function ($scope, ngDefault)
        {
            $scope.compiler = $compile;
        };

    });
    app.register.controller('dailySalesController', ['$scope', '$rootScope', '$routeParams', 'salesReportServices', '$filter', '$locale',
    function ($scope, $rootScope, $routeParams, salesReportServices, $filter, $locale)
    {
        setControlDate($scope, '', '');
        setEndDate($scope, '', '');
       
        $scope.initializeController = function () {
            $scope.details = false;
            $scope.showEmpSearch = true;
            $rootScope.getDailySales();
        };
        
        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        function filterNumber(valueToFilter)
        {
            var number = $filter('number');
            var value = number(valueToFilter);
            return value;
        }
        
        $rootScope.getDailySales = function ()
        {
            $scope.dailyPayload =
            {
                itemsPerPage: 100,
                pageNumber: 0,
                totalAmountDue: 0,
                totalVAT: 0,
                totalDiscount: 0,
                totalNet: 0,
                totalPaid: 0,
                sn: 1
            };

            angular.element('#empRepTbl tbody').empty();
            $scope.cllCollection = [];
            $scope.retrieveDailySales($scope.dailyPayload);
        };

        $scope.retrieveDailySales = function(payload)
        {
            var url = '/Report/GetDailySales?itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getDailySalesCompleted);
        }

        $scope.getDailySalesCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.dailyPayload.totalPaid > 0)
                {
                    var totalPaid = filterCurrency($scope.dailyPayload.totalPaid);
                    var totalNet = filterCurrency($scope.dailyPayload.totalNet);

                    angular.element('#prtEmpTbl').show();
                    $scope.empReport = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    +'<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';

                    
                    $scope.header += "<tr style=\"font-size:0.9em;\"><td style=\"width: 20%\"></td><td  style=\"width: 60%;\"><h5><b style=\"border-bottom: 1px solid #000\">Today's Sales</b></h5></td><td style=\"width: 20%\"></td></tr>";

                    $scope.header += '</table><br/>';

                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div><table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.85em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL NET</td><td style="width: 20%;">TOTAL AMOUNT PAID</td></tr>' +
                        '<tr style="font-size:0.79em; font-weight: bold"><td style="width: 20%">' + totalNet + '</td>' +
                        '<td style="width: 20%;">' + totalPaid + '</td></tr></table>';

                    $scope.header += subHeader.replace('0.85em', '0.7em').replace('0.79em', '0.65em');
                    angular.element('#empSummary').html('').append(subHeader);
                }
                  
                return;
            }
            
            var rows = '';
            
            angular.forEach(data, function (r, i) 
            {
                $scope.cllCollection.push(r);
                
                var totalDue = r[3].replace(',', '');
                var totalVat = r[4].replace(',', '');
                var totalDiscount = r[5].replace(',', '');
                var totalNet = r[6].replace(',', '');
                var totalPaid = r[7].replace(',', '');
                
                $scope.dailyPayload.totalAmountDue += parseFloat(totalDue);
                $scope.dailyPayload.totalVAT += parseFloat(totalVat);
                $scope.dailyPayload.totalDiscount += parseFloat(totalDiscount);
                $scope.dailyPayload.totalNet += parseFloat(totalNet);
                $scope.dailyPayload.totalPaid += parseFloat(totalPaid);

                //var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                //<td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td>
                rows += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.dailyPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[8] + '</td><td>' + r[2] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td></tr>';
                $scope.dailyPayload.sn++;
            });
            
            angular.element('#empRepTbl > tbody:last-child').append(rows);
            $scope.dailyPayload.itemsPerPage = 100;
            $scope.dailyPayload.pageNumber += 1;
            $scope.retrieveDailySales($scope.dailyPayload);
        };
        
        $scope.printEmpReport = function (collection)
        {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }
        
            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Invoice No.</th><th style="width: 12%; text-align: left">Date</th><th style="width: 12%; text-align: left">' +
                'Customer</th><th style="width: 12%; text-align: left">Amount Due(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 8%; text-align: left">' +
                'VAT(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 10%; text-align: left">Discount('+$rootScope.store.DefaultCurrencySymbol+')</th>' +
                '<th style="width: 12%; text-align: left">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 12%; text-align: left">' +
                'Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
            var sn = 1;

            angular.forEach(items, function (r, i)
            {
                sr += '<tr role="row" class="odd" style="border-top:solid 1px #ddd; font-size:0.62em"><td>' + sn + '</td><td>' + r[1] + '</td><td>' + r[8] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td></tr>';
                sn++;
            });
            sr += '</tbody></table><br/>';
            $scope.printReport(sr);
        };

        $scope.printReport = function (html)
        {
            if (html === undefined || html === null || html.length < 1)
            {
                return false;
            }
            var printContents = html;
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
                popupWin.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" /></head><body onload="window.print()"><div class="row" style="width:95%; margin-left:3%; margin-right:2%; margin-top:5%; margin-bottom:2%"><div class="col-md-12">' + printContents + '</div></div></html>');
                popupWin.document.close();
            }
            popupWin.document.close();
            return true;
        };

    }]);
    
});




