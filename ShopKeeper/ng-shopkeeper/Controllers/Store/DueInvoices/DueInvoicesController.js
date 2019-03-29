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

    app.register.controller('dueInvoicesController', ['$scope', '$rootScope', '$routeParams', 'salesReportServices', '$filter', '$locale',
    function ($scope, $rootScope, $routeParams, salesReportServices, $filter, $locale)
    {
        $scope.initializeController = function () {
            $rootScope.getDueInvoices();
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
        
        $rootScope.getDueInvoices = function ()
        {
            $scope.invPayload =
            {
                itemsPerPage: 100,
                pageNumber: 0,
                sn: 1
            };

            angular.element('#invTbl tbody').empty();
            $scope.cllCollection = [];
            $scope.retrieveDueInvoices($scope.invPayload);
        };

        $scope.retrieveDueInvoices = function(payload)
        {
            var url =  '/Sales/GetDueInvoices?itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getDueInvoicesCompleted);
        }

        $scope.getDueInvoicesCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.invPayload.totalPaid > 0)
                {
                    angular.element('#prtEmpTbl').show();
                    $scope.empReport = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    +'<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';
                    
                    $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales by all cashiers for the period <b>' + $scope.invPayload.strDt + '-' + $scope.invPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';
                    
                    $scope.header += '</table><br/>';
                }
                  
                return;
            }
            
            var rows = '';
            
            angular.forEach(data, function (r, i) 
            {
                $scope.cllCollection.push(r);
                
                var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                rows += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.invPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td>' + template + '</tr>';
                $scope.invPayload.sn++;
            });

            angular.element('#invTbl > tbody:last-child').append($scope.compiler(rows)($scope));
            $scope.invPayload.itemsPerPage = 100;
            $scope.invPayload.pageNumber += 1;
            $scope.retrieveDueInvoices($scope.invPayload);
        };

        $scope.getSalesReport = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            salesReportServices.getProduct(id, $scope.getInvoiceDetailsCompleted);
        };

        $scope.getInvoiceDetailsCompleted = function (rec)
        {
            if (rec.SaleId < 1) {
                alert("Report information could not be retrieved");
                return;
            }

            var reportHtml =
                '<table style="width: 100%; margin-left: 20px; border: none"> <tr style="border: none;"><td style="width: 40%; border: none"></td>' +
                    '<td style="width: 30%; border: none"><h3 class="ng-binding" style="width: 100%;">' + $rootScope.store.StoreName + '</h3>' +
                    '</td><td></td></tr><tr style="border: none"><td style="width: 40%; border: none; font-size: 0.9em">' +
                    '<table style="border: none; width: 100%; vertical-align: top"><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Address: ' +
                    $rootScope.store.StoreAddress + '</td></tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Website: <b></b></td>' +
                    '</tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Email Address: <b>' + $rootScope.store.StoreEmail + '</b>' +
                    '</td></tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Phone: <b>' + $rootScope.store.PhoneNumber + '</b></td>' +
                    '</tr><tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Date: <b>' + rec.DateStr + '</b></td>';

            if (rec.EstimateNumber !== undefined && rec.EstimateNumber !== null && rec.EstimateNumber.length > 0) {
                reportHtml += '<tr><td style="width: 40%;font-size:1em"><h5>Estimate Ref: <b>' + rec.EstimateNumber + '</b></h5>' +
                    '</td><td style="width: 30%"></td><td></td></tr>';
            }

            reportHtml += '<tr style="border: none"><td style="border: none; width: 100%;font-size:1em">Invoice No.: <b>' + rec.InvoiceNumber + '</b></td>' +
        '</tr></table></td><td style="width: 30%; border: none"></td><td style ="border: none"><img style="height: 60px; vertical-align: top; float: right; margin-right:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '">' +
        '</td></tr>';

            reportHtml += '<tr style="border: none"><td style="width: 40%;font-size:1em" >' +
            'Customer : <strong>' + rec.CustomerName + '</strong></td><td style="width: 30%"></td><td></td></tr></table><br>';

            reportHtml += '<div class="col-md-12">' +
             '<table class="table" role="grid" style="width: 100%;font-size:1em">' +
                '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:1em"><th style="color: green;font-size:1em; width:35%">Item</th>' +
                 '<th style="color: green;font-size:1em; width:20%;">Qty</th><th style="color: green;font-size:1em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                  '<th style="color: green;font-size:1em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

            angular.forEach(rec.StoreItemSoldObjects, function (item, i) {
                reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td>' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            });

            reportHtml += '</tbody></table></div>' +
                '<table style="width: 100%;font-size:1em"><tr><td><h5 style="padding: 8px;">Payment method(s):</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Payment details:</h5></td>' +
                '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:1em; float:left">';

            angular.forEach(rec.Transactions, function (l, i) {
                reportHtml += '<tr style="border-top: #ddd solid 1px;"><td style="color: green;font-size:1em;">' + l.PaymentMethodName + ':</td>' +
                             '<td><b >' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
            });

            reportHtml += '</table></td><td>' +
                 '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:1em;"><tr style="border-top: #ddd solid 1px;"><td style="color: green;font-size:1em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                         '<td style="text-align: right"><b>' + filterCurrency(rec.AmountDue, '') + '</b></td>' +
                     '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right"><b>' + filterCurrency(rec.DiscountAmount, '') + '</b></td>' +
                     '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td style="text-align: right"><b>' + filterCurrency(rec.VATAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                       '<td style="text-align: right"><b>' + filterCurrency(rec.NetAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                         '<td style="text-align: right"><b>' + filterCurrency(rec.AmountPaid, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: green;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td style="text-align: right"><b>' + filterCurrency(rec.Balance, '') + '</b></td></tr></table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:1.06em">' +
                '<h5>Served by: <b>' + rec.SaleEmployeeName + '</b></h5></div></div>';

            angular.element('#details').html('').append(reportHtml);

            $scope.details = true;
        };

        $scope.goBack = function ()
        {
            $scope.details = false;
        };

        $scope.printInvoice = function ()
        {
            var printContents = document.getElementById('details').innerHTML;
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




