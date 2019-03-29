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
    app.register.controller('purchaseOrderReportController', ['$scope', '$rootScope', '$routeParams', 'reportServices', '$filter', '$locale',
    function ($scope, $rootScope, $routeParams, reportServices, $filter, $locale)
    {
        setControlDate($scope, '', '');
        setEndDate($scope, '', '');
       
        $scope.initializeController = function () 
        {
            $scope.reportOptions =
            [
                { name: 'Employee', typeId: '1' },
                { name: 'Product', typeId: '2' },
                { name: 'Payment Type', typeId: '3' },
                { name: 'Outlet', typeId: '4' },
                { name: 'Customer', typeId: '5' },
                { name: 'Product Brand', typeId: '6' },
                { name: 'Product Category', typed: '7' },
                { name: 'Product Type', typeId: '8'}
            ];

            $scope.details = false;
            $scope.reportChoice = '';
            $scope.option = {};
            $scope.getDefaults();
        };

        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        $scope.getDefaults = function ()
        {
            $scope.items = [];
            reportServices.getDefaults($scope.getDefaultsCompleted);
        };

        $scope.setOption = function (opt)
        {
            if (opt.typeId === undefined || opt.typeId == null || opt.typeId < 1)
            {
                return;
            }
            
            $scope.option = opt;
        };
         
        $scope.getDefaultsCompleted = function (data)
        {
            if (data == null)
            {
                return;
            }

            $scope.categories = data.categories;
            $scope.itemTypes = data.itemTypes;
            $scope.itemBrands = data.itemBrands;
            $scope.employees = data.Employees;
            $scope.paymentMethods = data.PaymentMethods;
            $scope.outlets = data.Outlets;

            if (data.Items != null && data.Items.length > 0)
            {
                $scope.items = data.Items;
                $scope.page = 2;
                $scope.itemsPerPage = 50;
                reportServices.getProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
            }

        };

        $scope.getProductsCompleted = function (products)
        {
            if (products.length < 1)
            {
                return;
            }

            angular.forEach(products, function (x, y)
            {
                $scope.items.push(x);

            });

            $scope.page++;
            reportServices.getProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
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
            }

        };

        $scope.getOutletReports = function (outlet)
        {
            if (outlet === undefined || outlet == null)
            {
                alert('Please select Payment Type');
                return;
            }

            if (outlet.StoreOutletId === undefined || outlet.StoreOutletId == null || outlet.StoreOutletId < 1)
            {
                alert('Please select a Payment Type');
                return;
            }

            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.lenght < 1)
            {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.lenght < 1) {
                alert('Please select date range');
                return;
            }

            var date1 = new Date($scope.startDate);
            var date2 = new Date($scope.endDate);

            if (date1 > date2) {
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
            $scope.outlet = outlet;

            if ($scope.outletTableDir !== undefined && $scope.outletTableDir != null)
            {
                $scope.productReport = false;
                $scope.outletTableDir.fnClearTable();
            }
            else
            {
                $scope.processing = true;
                $scope.outletsReport = true;
                var tableDir = angular.element('#outletRepTbl');
                var tableOptions = {};
                tableOptions.sourceUrl = '/Report/GetOutletSalesReport?outletId=' + outlet.StoreOutletId + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                tableOptions.itemId = 'SaleId';
                tableOptions.columnHeaders = ['StoreItemName', 'AmountDue', 'AmountPaidStr', 'DateStr'];
                var outletTableDir = employeeReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getReport');
                $scope.outletTableDir = outletTableDir;
            }
        }

        $scope.getPaymentTypeReports = function (paymentType)
        {
            console.log(JSON.stringify(paymentType));
            if (paymentType === undefined || paymentType == null)
            {
                alert('Please select Payment Type');
                return;
            }

            if (paymentType.StorePaymentMethodId === undefined || paymentType.StorePaymentMethodId == null || paymentType.StorePaymentMethodId < 1) {
                alert('Please select a Payment Type');
                return;
            }

            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.lenght < 1) {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.lenght < 1) {
                alert('Please select date range');
                return;
            }

            var date1 = new Date($scope.startDate);
            var date2 = new Date($scope.endDate);

            if (date1 > date2) {
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

            $scope.paymentType = paymentType;

            if ($scope.paymentTypeTableDir !== undefined && $scope.paymentTypeTableDir != null) {
                $scope.paymentReport = false;
                $scope.paymentTypeTableDir.fnClearTable();
            } else {
                $scope.processing = true;
                $scope.paymentReport = true;
                var tableDir = angular.element('#paymentTypeRepTbl');
                var tableOptions = {};
                tableOptions.sourceUrl = '/Report/GetPaymentTypeSalesReport?paymentTypeId=' + paymentType.StorePaymentMethodId + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                tableOptions.itemId = 'SaleId';
                tableOptions.columnHeaders = ['StoreItemName', 'AmountDue', 'AmountPaidStr', 'DateStr'];
                var paymentTypeTableDir = employeeReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getReport');
                $scope.paymentTypeTableDir = paymentTypeTableDir;
            }
            
        }

        $rootScope.getProductReports = function (selectedProduct)
        {
            var product = selectedProduct.originalObject;
            if (product === undefined || product == null)
            {
                alert('Please select a Product');
                return;
            }

            if (product.StoreItemId === undefined || product.StoreItemStockId == null || product.StoreItemStockId < 1)
            {
                alert('Please select a Product');
                return;
            }
            
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

            if (date1 > date2) {
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

            $scope.product = product;

            if ($scope.prTableDir !== undefined && $scope.prTableDir != null)
            {
                $scope.productReport = false;
                $scope.prTableDir.fnClearTable();
            } else {
                $scope.processing = true;
                $scope.productReport = true;
                var tableDir = angular.element('#proRepTbl');
                var tableOptions = {};
                tableOptions.sourceUrl = '/Report/GetProductSalesReport?productId=' + product.StoreItemStockId + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                tableOptions.itemId = 'StoreItemStockId';
                tableOptions.columnHeaders = ['QuantitySoldStr', 'RateStr', 'AmountSoldStr', 'QuantityLeftStr', 'DateSoldStr'];
                var prTableDir = productReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getProductSalesReportDetail');
                $scope.prTableDir = prTableDir;
            }
        }
        
        $rootScope.getEmployeeReports = function (selectedEmployee)
        {
            var employee = selectedEmployee.originalObject;
            if (employee === undefined || employee == null)
            {
                alert('Please select an employee');
                return;
            }

            if (employee.EmployeeId === undefined || employee.EmployeeId == null || employee.EmployeeId < 1)
            {
                alert('Please select an employee');
                return;
            }
            
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

            $scope.employee = employee;

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
                tableOptions.sourceUrl = '/Report/GetEmployeeSalesReport?employeeId=' + employee.EmployeeId + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                tableOptions.itemId = 'SaleId';
                tableOptions.columnHeaders = ['StoreItemName', 'AmountDue', 'AmountPaidStr', 'DateStr'];
                var empTableDir = employeeReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getSalesReport');
                $scope.empTableDir = empTableDir;
            }
        }
        
        $scope.getSalesReport = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            reportServices.getProduct(id, $scope.getSalesReportDetailsCompleted);
        };

        $scope.goBack = function ()
        {
            $scope.details = false;
        };

        $scope.getProductSalesReportDetail = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            var productReport = 
            {
                ProductId: id,
                StartDate:  $scope.startDate,
                EndDate: $scope.endDate
            }

            reportServices.getProductSalesReportDetail(productReport, $scope.getProductSalesReportDetailsCompleted);
        };

        $scope.getProductSalesReportDetailsCompleted = function (rec)
        {
            if (rec.length < 1)
            {
                alert("Report information could not be retrieved");
                return;
            }

            var reportHtml =
                '<div class="row modal-header"><div class="col-md-12"><div class="col-md-4"></div><div class="col-md-4"><h3>Product Sales Report Details</h3></div><div class="col-md-4"></div></div><br/><br/>' +
                    '<div class="row"><div class="col-md-12"><h5>Product: <b>' + rec[0].StoreItemName + '</b></h5></div><br/>';

            reportHtml += '<div class="col-md-12">' +
             '<table class="table" role="grid" style="width: 100%;font-size:1em">' +
                '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:1em"><th style="color: #008000;font-size:1em; width:20%">Item</th>' +
                 '<th style="color: #008000;font-size:1em; width:15%;">Qty</th><th style="color: #008000;font-size:1em; width:15%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                  '<th style="color: #008000;font-size:1em; width:15%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="color: #008000;font-size:1em; width:15%;">Date Sold</th><th style="color: #008000;font-size:1em; width:15%;">Qty in stock</th></tr></thead><tbody>';

            angular.forEach(rec.StoreItemSoldObjects, function (item, i)
            {
                reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td style="text-align: right">' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td style="float:right">' + filterCurrency(item.AmountSold, " ") + '</td><td style="float:right">' + item.DateSoldStr + '</td><td style="float:right">' + item.QuantityLeft + '</td></tr>';
            });
            
            reportHtml += '</tbody></table></div>' +
                
            angular.element('#rep').html('').append(reportHtml);
            $scope.details = true;
        };
       
        $scope.getSalesReportDetailsCompleted = function (rec)
        {
            if (rec.SaleId < 1)
            {
                alert("Report information could not be retrieved");
                return;
            } 
            
            var reportHtml =
                  '<div class="row modal-header"><div class="col-md-12"><div class="col-md-4"></div><div class="col-md-4"><h3>Sales Report Details</h3></div><div class="col-md-4"></div></div><br/><br/>' +
                  '<div class="row"><div class="col-md-12"><h5>Outlet: <b>' + rec.OutletName + '</b></h5></div>' +
                  '<div class="row"><div class="col-md-12"><h5>Date: <b>' + rec.DateStr + '</b></h5>' +
                  '</div></div><div class="row"><div class="col-md-12"><h5>Invoice No: <b>' + rec.InvoiceNumber + '</b></h5>' +
                  '</div></div></div></div><div class="row" style="padding-left: 0px"><div class="col-md-12" style="font-size:1em">' +
                 '<h5>Sold by: <b>' + rec.SaleEmployeeName + '</b></h5></div></div><br/>';

            if (rec.CustomerName !== null && rec.CustomerName.length > 0)
            {
                reportHtml += '<div class="row"><div class="col-md-12">Customer : <strong>' + rec.CustomerName + '</strong></div>'
                    + '</div><br/>';
            }

            reportHtml += '<div class="col-md-12">' +
             '<table class="table" role="grid" style="width: 100%;font-size:1em">' +
                '<thead><tr style="text-align: left; border-bottom: 1px solid #ddd;font-size:1em"><th style="color: #008000;font-size:1em; width:35%">Item</th>' +
                 '<th style="color: #008000;font-size:1em; width:20%;">Qty</th><th style="color: #008000;font-size:1em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                  '<th style="color: #008000;font-size:1em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

            angular.forEach(rec.StoreItemSoldObjects, function (item, i)
            {
                reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td style="text-align: right">' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td style="float:right">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            });

            reportHtml += '</tbody></table></div>' +
                '<table style="width: 100%;font-size:1em"><tr><td><h5 style="padding: 8px;">Payment method(s):</h5></td><td><h5 style="float:right; text-align:right; margin-right:11%">Payment details:</h5></td>' +
                '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:1em; float:left">';

            angular.forEach(rec.Transactions, function (l, i)
            {
                reportHtml += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:1em;">' + l.PaymentMethodName + ':</td>' +
                             '<td><b style="text-align: right">' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
            });
            
            reportHtml += '</table></td><td>' +
                 '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:1em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:1em;">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                         '<td style="text-align: right"><b>' + filterCurrency(rec.AmountDue, '') + '</b></td>' +
                     '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right"><b>' + filterCurrency(rec.DiscountAmount, '') + '</b></td>' +
                     '</tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                      '<td style="text-align: right"><b>' + filterCurrency(rec.VATAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                       '<td style="text-align: right"><b>' + filterCurrency(rec.NetAmount, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                         '<td style="text-align: right"><b>' + filterCurrency(rec.AmountPaid, '') + '</b></td></tr><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;">Balance(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                     '<td style="text-align: right"><b>' + filterCurrency(rec.Balance, '') + '</b></td></tr></table></td></tr></table>';
            
            angular.element('#rep').html('').append(reportHtml);
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




