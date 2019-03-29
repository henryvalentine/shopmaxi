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
    app.register.controller('stockReport', ['$scope', '$rootScope', '$routeParams', 'salesReportServices', '$filter', '$locale',
    function ($scope, $rootScope, $routeParams, salesReportServices, $filter, $locale)
    {
        $scope.initializeController = function () 
        {
            $scope.details = false;

            var date1 = new Date();
            var year1 = date1.getFullYear();
            var month1 = date1.getMonth() + 1;
            var day1 = date1.getDate();
            var date = day1 + '/' + month1 + '/' + year1;

            $scope.stockReportPayload =
            {
                itemsPerPage: 100,
                pageNumber: 0,
                date : date,
                totalStockValue: 0,
                totalStockBalance: 0,
                totalCostPrice: 0,
                sn: 1
            };

            angular.element('#stockDate').html('').html('Stock report Generated on ' + date);
            angular.element('#stockRepTbl tbody').empty();
            $scope.stockReportCollection = [];
            $scope.stockReportHtml = '';
            $scope.retrieveStockReport($scope.stockReportPayload);
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
        
        /*STATEMENT TYPES*/
        $scope.retrieveStockReport = function (payload)
        {
            var url = '/Report/GetStockReport?itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getStockReportCompleted);
        }

        $scope.getStockReportCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.stockReportCollection.length > 0)
                {
                    angular.element('#prtStockTbl').show();
                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    +'<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr><tr style="font-size:0.9em;"><td style="width: 20%"></td>' +
                        '<td  style="width: 60%;"><h5><b style="border-bottom: 1px solid #000">Stock report Generated on ' + $scope.stockReportPayload.date + '</b></h5></td><td style="width: 20%"></td></tr>';
                    
                    $scope.header += '</table><br/>';
                    

                    var rows = '';
                    var totalValue = 0;
                    angular.forEach($scope.stockReportCollection, function (r, i)
                    {
                        rows += '<tr role="row" style="border-top:solid 1px #ddd; font-weight: bold;font-size:1.1em; padding-top:5px; padding-bottom:5px"><td  colspan="7">' + r.Name + '</td></tr>';
                        var sn = 1;

                        var totalCategoryValue = 0;

                        angular.forEach(r.StockItems, function (o, i)
                        {
                            rows += '<tr role="row" style="border-top:solid 1px #ddd;"><td  style="width: 3%;font-size:0.9em;">' + sn + '</td><td>' + o.StoreItemName + '</td><td>' + o.SKU + '</td><td>' + o.QuantityInStockStr + '</td><td>' + filterCurrency(o.CostPrice, '') + '</td><td>' + o.StockValueStr + '</td></tr>';
                            sn++;
                            totalCategoryValue += o.StockValue;
                            totalValue += o.StockValue;
                        });
                        rows += '<tr role="row" style="border-top:solid 1px #ddd;"><td  style="width: 3%;font-size:0.9em;"></td><td></td><td></td><td style="border-top:solid 1px #e0e0e0;border-bottom:solid 1px #e0e0e0;"></td><td style="border-top:solid 1px #e0e0e0;border-bottom:solid 1px #e0e0e0;"></td><td style="border-top:solid 1px #e0e0e0;border-bottom:solid 1px #e0e0e0;"><b>' + filterCurrency(totalCategoryValue, '') + '</b></td></tr><br>';
                    
                    });

                    $scope.stockReportPayload.totalStockValue += totalValue;

                    var header = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div>' +
                       '<table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.85em; font-weight: bold;"><td style="width: 20%;">TOTAL STOCK VALUE(' + $rootScope.store.DefaultCurrencySymbol + ')</td></tr>' +
                      '<tr style="font-size:0.79em; font-weight: bold"><td style="width: 20%;">' + filterCurrency($scope.stockReportPayload.totalStockValue, '') + '</td></tr></table>';

                    angular.element('#stockSummary').html('').append(header);

                    header = header.replace('0.85em', '0.7em').replace('0.79em', '0.65em');
                    $scope.header += header;
                    
                    angular.element('#stockRepTbl tbody').empty();
                    angular.element('#stockRepTbl > tbody:last-child').append($scope.compiler(rows)($scope));
                }
                  
                return;
            }
            
            angular.forEach(data, function (r, i)
            {
                var exis = $scope.stockReportCollection.filter(function (c)
                {
                    return c.StoreItemCategoryId === r.StoreItemCategoryId;
                });

                if (exis.length > 0) {
                    angular.forEach(r.StockItems, function (o, i)
                    {
                        exis[0].StockItems.push(o);
                    });
                }
                else
                {
                    $scope.stockReportCollection.push(r);
                }
                
            });

            $scope.stockReportPayload.itemsPerPage = 100;
            $scope.stockReportPayload.pageNumber += 1;
            $scope.retrieveStockReport($scope.stockReportPayload);
        };

        
        /*SALES REPORT TYPES END*/
        $scope.goBack = function ()
        {
            $scope.details = false;
        };

        $scope.printStockReport = function ()
        {
            var items = $scope.stockReportCollection;
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }

            var sr = $scope.header.replace('<br/>', '').replace('<br>', '');

            sr += '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.7em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Product' +
                '</th><th style="width: 10%; text-align: left">SKU</th><th style="width: 12%; text-align: left"> Stock Balance</th><th style="width: 12%; text-align: left">Cost Price(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 12%; text-align: left">Stock Value(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
            
            angular.forEach($scope.stockReportCollection, function (r, i)
            {
                sr += '<tr role="row" style="border-top:solid 1px #ddd; font-weight: bold;font-size:0.68em; padding-top:5px; padding-bottom:5px"><td  colspan="7">' + r.Name + '</td></tr>';
                var sn = 1;

                var totalCategoryValue = 0;

                angular.forEach(r.StockItems, function (o, i)
                {
                    sr += '<tr role="row" style="border-top:solid 1px #ddd;font-size:0.60em;"><td  style="width: 3%;font-size:0.9em;">' + sn + '</td><td>' + o.StoreItemName + '</td><td>' + o.SKU + '</td><td>' + o.QuantityInStockStr + '</td><td>' + filterCurrency(o.CostPrice, '') + '</td><td>' + o.StockValueStr + '</td></tr>';
                    sn++;
                    totalCategoryValue += o.StockValue;
                });
                sr += '<tr role="row" style="border-top:solid 1px #ddd;font-size:0.60em;"><td  style="width: 3%;font-size:0.9em;"></td><td></td><td></td><td style="border-top:solid 1px #e0e0e0;border-bottom:solid 1px #e0e0e0;"></td><td style="border-top:solid 1px #e0e0e0;border-bottom:solid 1px #e0e0e0;"></td><td style="border-top:solid 1px #e0e0e0;border-bottom:solid 1px #e0e0e0;"><b>' + filterCurrency(totalCategoryValue, '') + '</b></td></tr>';
               
                $scope.stockReportPayload.totalStockValue += totalCategoryValue;
            });

            sr += '</tbody></table>';
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
            else
            {
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




