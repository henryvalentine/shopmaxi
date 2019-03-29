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
    app.register.controller('customerInvoiceReports', ['$scope', '$rootScope', '$routeParams', 'salesReportServices', '$filter', '$locale',
    function ($scope, $rootScope, $routeParams, salesReportServices, $filter, $locale)
    {
        setControlDate($scope, '', '');
        setEndDate($scope, '', '');
       
        $scope.initializeController = function () 
        {
            $scope.showCustomerSearch = true;
            $scope.GetCustomerStatements(null);
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

        $scope.GetCustomerStatements = function (customer)
        {
            var selCustomer =
            {
                CustomerId: 0
            };

            if (customer !== undefined && customer !== null && customer.CustomerId > 0)
            {
                selCustomer = customer;
                $scope.customer = customer;
            }
          
             $scope.customerPayload =
             {
                 customer: selCustomer,
                 itemsPerPage: 100,
                 pageNumber: 0,
                 totalAmountDue: 0,
                 totalVAT: 0,
                 totalDiscount: 0,
                 totalNet: 0,
                 totalPaid: 0,
                 invoiceBalance: 0,
                 sn: 1
             };

             $scope.allCustomerCollection = [];
             $scope.customerHtml = '';
             angular.element('#customerSummary').empty();
             angular.element('#prtCustomerTbl').hide();
             angular.element('#prtSupplierTbl').hide();
            
             angular.element('#customerInvoiceRepTbl tbody').empty();
             $scope.header = '';
             $scope.retrieveCustomersStatement($scope.customerPayload);
        }
         
        $scope.retrieveCustomersStatement = function (payload)
        {
            var url = '/Report/GetAllCustomerStatements?itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
     
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getCustomerStatementCompleted);
        }

        $scope.getCustomerStatementCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.customerPayload.totalPaid > 0)
                {
                    var totalPaid = filterCurrency($scope.customerPayload.totalPaid);
                    var totalDue = filterCurrency($scope.customerPayload.totalAmountDue);
                    var totalVat = filterCurrency($scope.customerPayload.totalVAT);
                    var totalDiscount = filterCurrency($scope.customerPayload.totalDiscount);
                    var invoiceBalance = filterCurrency($scope.customerPayload.invoiceBalance);

                    angular.element('#prtCustomerTbl').show();
                    $scope.customerStatement = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    +'<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';

                     $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Customer Performances</h5></td><td style="width: 20%"></td></tr>';

                    $scope.header += '</table><br/>';

                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div><table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.85em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL AMOUNT DUE</td><td style="width: 20%;">TOTAL VAT' +
                        '</td><td style="width: 20%">TOTAL DISCOUNT</td><td style="width: 20%;">TOTAL AMOUNT PAID</td><td style="width: 20%;">TOTAL INVOICE BALANCE</td></tr>' +
                        '<tr style="font-size:0.79em; font-weight: bold"><td style="width: 20%">' + totalDue + '</td><td style="width: 20%;">' + totalVat + '</td><td style="width: 20%">' + totalDiscount + '</td><td style="width: 20%">' + totalPaid + '</td>' +
                        '<td style="width: 20%;">' + invoiceBalance + '</td></tr></table>';

                    $scope.header += subHeader.replace('0.85em', '0.67em').replace('0.79em', '0.65em');
                    angular.element('#customerSummary').html('').append(subHeader);
                   
                    angular.element('#customerInvoiceRepTbl > tbody:last-child').append($scope.compiler($scope.customerHtml)($scope));
                }
                  
                return;
            }

            $scope.buildCustomerStatement(data);
        };
        
        $scope.buildCustomerStatement = function (data)
        {
            angular.forEach(data, function (r, i)
            {
                $scope.allCustomerCollection.push(r);

                var totalDue = r[4].replace(',', '');
                var totalVat = r[5].replace(',', '');
                var totalDiscount = r[6].replace(',', '');
                var totalPaid = r[7].replace(',', '');
                var invoiceBalance = r[8].replace(',', '');

                $scope.customerPayload.totalAmountDue += parseFloat(totalDue);
                $scope.customerPayload.totalVAT += parseFloat(totalVat);
                $scope.customerPayload.totalDiscount += parseFloat(totalDiscount);
                $scope.customerPayload.totalPaid += parseFloat(totalPaid);
                $scope.customerPayload.invoiceBalance += parseFloat(invoiceBalance);

                //var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                $scope.customerHtml += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.customerPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td></tr>';
                $scope.customerPayload.sn++;
            });
            
            $scope.customerPayload.itemsPerPage = 100;
            $scope.customerPayload.pageNumber += 1;
            $scope.retrieveCustomersStatement($scope.customerPayload);
        };

        /*SALES REPORT TYPES END*/

        $scope.goBack = function ()
        {
            $scope.details = false;
        };
        
        $scope.printCustomerStatement = function (collection)
        {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Customer' +
                '</th><th style="width: 12%; text-align: left">Date Registered</th><th style="width: 12%; text-align: left">' +
                'Outlet</th><th style="width: 12%; text-align: left">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol+')' +
                '</th><th style="width: 12%; text-align: left">Total VAT(' + $rootScope.store.DefaultCurrencySymbol+')</th>' +
                '<th style="width: 12%; text-align: left">Total Discount(' + $rootScope.store.DefaultCurrencySymbol+')' +
                '</th><th style="width: 12%; text-align: left">Total Amount Paid(' + $rootScope.store.DefaultCurrencySymbol+')' +
                '</th><th style="width: 15%; text-align: left">Total Invoice Balance(' + $rootScope.store.DefaultCurrencySymbol+')</th></tr></thead><tbody>';
             var sn = 1;

            angular.forEach(items, function (r, i)
            {
                sr += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;font-size:0.62em"><td  style="width: 3%;">' + sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td></tr>';
                sn++; 
            });
            sr += '</tbody></table><br/>';
            $scope.printReport(sr);
        };

        $scope.printSupplierStatement = function (collection) {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1) {
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Supplier' +
                '</th><th style="width: 12%; text-align: left">Date Joined</th>' +
                '<th style="width: 12%; text-align: left">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + ')' +
                '</th><th style="width: 12%; text-align: left">Total VAT(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                '<th style="width: 12%; text-align: left">Total Discount(' + $rootScope.store.DefaultCurrencySymbol + ')' +
                '</th><th style="width: 12%; text-align: left">Total Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + ')' +
                '</th><th style="width: 15%; text-align: left">Total Invoice Balance(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
            var sn = 1;

            angular.forEach(items, function (r, i) {
                sr += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;font-size:0.62em"><td  style="width: 3%;">' + sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td></tr>';
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




