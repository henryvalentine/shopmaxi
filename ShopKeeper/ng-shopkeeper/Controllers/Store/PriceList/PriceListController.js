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
    app.register.controller('priceListReports', ['$scope', '$rootScope', '$routeParams', 'salesReportServices', '$filter', '$locale',
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

            $scope.priceListPayload =
            {
               itemsPerPage: 100,
               pageNumber: 0,
               date : date,
               sn: 1
            };

            angular.element('#priceSummary').html('').html('Price List report Generated on ' + date);
            angular.element('#priceListRepTbl tbody').empty();
            $scope.priceListCollection = [];
            $scope.priceListReportHtml = '';
            $scope.retrievePriceListReport($scope.priceListPayload);
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
        $scope.retrievePriceListReport = function (payload)
        {
            var url = '/Report/GetPriceList?itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getPriceListCompleted);
        }

        $scope.getPriceListCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.priceListPayload.pageNumber > 0)
                {
                    angular.element('#prtPriceTbl').show();
                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    +'<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';
                    
                    $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Price List report Generated on <b>' + $scope.priceListPayload.date + '</b></h5></td><td style="width: 20%"></td></tr>';
                    
                    $scope.header += '</table><br/>';

                    angular.element('#priceListRepTbl tbody').empty();
                    angular.element('#priceListRepTbl > tbody:last-child').append($scope.priceHtml);
                }
                  
                return;
            }
            
            angular.forEach(data, function (r, i) 
            {
                $scope.priceListCollection.push(r);
                $scope.priceHtml += '<tr role="row" class="odd" style="border-top:solid 1px #ddd; font-size: 0.9em"><td  style="width: 3%;">' + $scope.priceListPayload.sn + '</td><td>' + r.StoreItemName + '</td><td>' + r.SKU + '</td><td>' + r.CategoryName + '</td>';
                if (r.ItemPriceObjects.lenth > 3) 
                {
                    var pss = [];
                    pss.push(r.ItemPriceObjects[0]);
                    pss.push(r.ItemPriceObjects[1]);
                    pss.push(r.ItemPriceObjects[2]);
                    r.ItemPriceObjects = pss;
                }
                angular.forEach(r.ItemPriceObjects, function (p, k) 
                {
                    $scope.priceHtml += '<td>' + p.PriceStr + '</td><td>' + p.MinimumQuantityStr +'</td>';
                });

                $scope.priceHtml += '</tr>';

                $scope.priceListPayload.sn++;
            });
            
            $scope.priceListPayload.itemsPerPage = 100;
            $scope.priceListPayload.pageNumber += 1;
            $scope.retrievePriceListReport($scope.priceListPayload);
        };

        
        /*SALES REPORT TYPES END*/
        $scope.goBack = function ()
        {
            $scope.details = false;
        };

        $scope.printPriceListReport = function (collection)
        {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px"><th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">'
                + 'Product</th><th style="width: 10%; text-align: left">SKU</th>'
                + '<th style="width: 10%; text-align: left">Category</th><th style="width: 10%; text-align: left">Price 1</th>'
                + '<th style="width: 10%; text-align: left">Quantity 1</th><th style="width: 10%; text-align: left">Price 2</th>'
                + '<th style="width: 10%; text-align: left">Quantity 2</th><th style="width: 10%; text-align: left">Price 3'
                + '</th><th style="width: 10%; text-align: left">Quantity 3</th></tr></thead><tbody>';
           
            sr += $scope.priceHtml.replace('font-size: 0.9em', 'font-size: 0.65em');
            
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




