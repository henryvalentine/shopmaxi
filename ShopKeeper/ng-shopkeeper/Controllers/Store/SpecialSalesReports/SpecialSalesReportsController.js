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
    app.register.controller('salesReportController', ['$scope', '$rootScope', '$routeParams', 'salesReportServices', '$filter', '$locale',
    function ($scope, $rootScope, $routeParams, salesReportServices, $filter, $locale)
    {
        setControlDate($scope, '', '');
        setEndDate($scope, '', '');
       
        $scope.initializeController = function () 
        {
            $scope.reportOptions =
            [
                { name: 'Cashier', typeId: 1 },
                { name: 'Product', typeId: 2 },
                { name: 'Payment Method', typeId: 3 },
                { name: 'Outlet', typeId: 4 },
                { name: 'Customer', typeId: 5 }
                //{ name: 'Product Activity', typeId: 6 }
                //{ name: 'Product Category', typed: '8' },
                //{ name: 'Product Type', typeId: '9'}
            ];

            $scope.details = false;
            $scope.endDate = '';
            $scope.startDate = '';
            $scope.reportChoice = '';
            $scope.option = {};
            salesReportServices.getCustomers($scope.getCustomersCompleted);
            $scope.getDefaults();
        };
        
        $scope.getCustomersCompleted = function (data)
        {
            $scope.customers = data;
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

        $scope.getDefaults = function ()
        {
            $scope.items = [];
            salesReportServices.getDefaults($scope.getDefaultsCompleted);
        };

        $scope.setOption = function (opt)
        {
            if (opt.typeId === undefined || opt.typeId == null || opt.typeId < 1)
            {
                return;
            }
            
            $scope.option = opt;
            $scope.validateDates();
        };
        
        $scope.checkReportType = function ()
        {
            var reportType = parseInt($scope.option.typeId);

            var date1 = new Date($scope.startDate).getDate();
            var date2 = new Date($scope.endDate).getDate();

            if (reportType === 3 && date2 > 0 && date1 > 0)
            {
                $scope.getPaymentTypeReports();
            }

            if (reportType === 6 && date2 > 0 && date1 > 0)
            {
                $scope.GetCustomerInvoiceReports();
            }

            if (reportType === 1 && date2 > 0 && date1 > 0)
            {
                $scope.showEmpSearch = true;
                $rootScope.getEmployeeReports(null);
            }

            if (reportType === 2 && date2 > 0 && date1 > 0)
            {
                $scope.showPrSearch = true;
                $rootScope.getProductReports(null);
            }
            
            if (reportType === 4 && date2 > 0 && date1 > 0)
            {
                $scope.showoutletReport = true;
                $rootScope.getOutletReports(null);
            }

            if (reportType === 5 && date2 > 0 && date1 > 0)
            {
                $scope.showCustomerReport = true;
                $rootScope.GetCustomerSalesReport(null);
            }
        };
         
        $scope.getDefaultsCompleted = function (data)
        {
            salesReportServices.getReports('StoreOutlet/GetOutlets', $scope.getoutletsCompleted);
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
                salesReportServices.getProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
            }

        };

        $scope.getoutletsCompleted = function (data)
        {
            $rootScope.outlets = data;
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
            salesReportServices.getProducts($scope.page, $scope.itemsPerPage, $scope.getProductsCompleted);
        };

        $scope.validateDates = function ()
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1)
            {
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1)
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
            
            $scope.checkReportType();

        };

        /*SALES REPORT TYPES*/

        $rootScope.getOutletReports = function (outlet)
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1)
            {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1) {
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
           
            var strDt = day1 + '/' + month1 + '/' + year1;
            var endDt = day2 + '/' + month2 + '/' + year2;

            var storeOutlet = {StoreOutletId : 0};

            if (outlet === undefined || outlet == null)
            {
                storeOutlet = outlet;
                $scope.outlet = outlet;
            }

            $scope.outletPayload =
            {
                outlet: storeOutlet,
                startDate: startDate,
                endDate: endDate,
                strDt: strDt,
                endDt: endDt,
                itemsPerPage: 100,
                pageNumber: 0,
                totalAmountDue: 0,
                totalVAT: 0,
                totalDiscount: 0,
                totalNet: 0,
                totalPaid: 0,
                sn: 1
            };
            angular.element('#outletRepTbl tbody').empty();
            $scope.outletCollection = [];
            
            $scope.retrieveOutletReport($scope.outletPayload);
        }

        $scope.retrieveOutletReport = function (payload)
        {
            var url = '';

            if (payload.outlet === undefined || payload.outlet == null || payload.outlet.StoreOutletId < 1)
            {
                url = '/Report/GetAllOutletsSalesReports?startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            else
            {
                $scope.outlet = payload.outlet;

                url = '/Report/GetSingleOutletSalesReports?outletId=' + payload.outlet.StoreOutletId + '&startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }

            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getOutletReportsCompleted);
        }

        $scope.getOutletReportsCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.outletPayload.totalPaid > 0)
                {
                    var totalPaid = filterCurrency($scope.outletPayload.totalPaid);
                    var totalDue = filterCurrency($scope.outletPayload.totalAmountDue);
                    var totalVat = filterCurrency($scope.outletPayload.totalVAT);
                    var totalDiscount = filterCurrency($scope.outletPayload.totalDiscount);
                    var totalNet = filterCurrency($scope.outletPayload.totalNet);

                    angular.element('#prtOutletTbl').show();
                    $scope.outletsReport = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    + '<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';
                    
                    if ($scope.outletPayload.outlet !== undefined && $scope.outletPayload.outlet !== null && $scope.outletPayload.outlet.StoreOutletId > 0)
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales by <b>' + $scope.outletPayload.outlet.OutletName + '</b> for the period <b>' + $scope.outletPayload.strDt + '-' + $scope.outletPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';
                    }
                    else
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales by all cashiers for the period <b>' + $scope.outletPayload.strDt + '-' + $scope.outletPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';

                    }

                    $scope.header += '</table><br/>';

                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div><table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.85em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL AMOUNT DUE</td><td style="width: 20%;">TOTAL VAT' +
                        '</td><td style="width: 20%">TOTAL DISCOUNT</td><td style="width: 20%">' +
                        'TOTAL NET</td><td style="width: 20%;">TOTAL AMOUNT PAID</td></tr>' +
                        '<tr style="font-size:0.79em; font-weight: bold"><td style="width: 20%">' + totalDue + '</td><td style="width: 20%;">' + totalVat + '</td><td style="width: 20%">' + totalDiscount + '</td><td style="width: 20%">' + totalNet + '</td>' +
                        '<td style="width: 20%;">' + totalPaid + '</td></tr></table>';

                    $scope.header += subHeader.replace('0.85em', '0.7em').replace('0.79em', '0.65em');
                    angular.element('#outletSummary').html('').append(subHeader);
                }

                return;
            }

            var rows = '';

            angular.forEach(data, function (r, i)
            {
                $scope.outletCollection.push(r);

                var totalDue = r[3].replace(',', '');
                var totalVat = r[4].replace(',', '');
                var totalDiscount = r[5].replace(',', '');
                var totalNet = r[6].replace(',', '');
                var totalPaid = r[7].replace(',', '');

                $scope.outletPayload.totalAmountDue += parseFloat(totalDue);
                $scope.outletPayload.totalVAT += parseFloat(totalVat);
                $scope.outletPayload.totalDiscount += parseFloat(totalDiscount);
                $scope.outletPayload.totalNet += parseFloat(totalNet);
                $scope.outletPayload.totalPaid += parseFloat(totalPaid);

                var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                rows += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.outletPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[8] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td>' + template + '</tr>';
                $scope.outletPayload.sn++;
            });
            
            angular.element('#outletRepTbl > tbody:last-child').append($scope.compiler(rows)($scope));
            $scope.outletPayload.itemsPerPage = 100;
            $scope.outletPayload.pageNumber += 1;
            $scope.retrieveOutletReport($scope.outletPayload);
        };

        $rootScope.getPaymentTypeReports = function (selectedPaymentType)
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1)
            {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1) 
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
            
            var strDt = day1 + '/' + month1 + '/' + year1;
            var endDt = day2 + '/' + month2 + '/' + year2;

            var paymentType =
            {
                StorePaymentMethodId: 0
            };

            if (selectedPaymentType !== undefined && selectedPaymentType !== null)
            {
                paymentType = selectedPaymentType;
                $scope.pyT = selectedPaymentType;
            }

            $scope.ptyPayload =
            {
                paymentType: paymentType,
                startDate: startDate,
                totalcount : 0,
                endDate: endDate,
                strDt: strDt,
                endDt: endDt,
                itemsPerPage: 100,
                pageNumber: 0,
                totalAmount: 0,
                sn: 1
            };
            angular.element('#paymentTypeRepTbl tbody').empty();
            $scope.ptyCollection = [];
            $scope.retrievePaymentTypeReport($scope.ptyPayload);
        } 

        $scope.retrievePaymentTypeReport = function (payload)
        {
            var url = '';

            if (payload.paymentType.StorePaymentMethodId === undefined || payload.paymentType.StorePaymentMethodId == null || payload.paymentType.StorePaymentMethodId < 1) {
                url = '/Report/GetAllPaymentTypeReports?startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            else {
                $scope.paymentType = payload.paymentType;
                url = '/Report/GetSinglePaymentTypeReports?paymentMethodTypeId=' + payload.paymentType.StorePaymentMethodId + '&startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }

            $scope.paymentReport = true;
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getPaymetReportsCompleted);
        }

        $scope.getPaymetReportsCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.ptyPayload.totalAmount > 0)
                {
                    var totalAmount = filterCurrency($scope.ptyPayload.totalAmount);
                   
                    angular.element('#prtPyTbl').show();
                    $scope.paymentReport = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    + '<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';

                    if ($scope.ptyPayload.paymentType.StorePaymentMethodId > 0)
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Transaction Payments made using <b>' + $scope.ptyPayload.paymentType.Name + '</b> for the period <b>' + $scope.ptyPayload.strDt + '-' + $scope.ptyPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';
                    }
                    else
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Transactions for all Payment Methods within the period <b>' + $scope.ptyPayload.strDt + '-' + $scope.ptyPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';

                    }

                    $scope.header += '</table><br/>';

                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div>' +
                        '<table style="width: 100%; margin-bottom: 2%"><tr style="font-size:0.9em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL TRANSACTIONS</td><td style="width: 20%;">TOTAL TRANSACTIONS VALUE: </td></tr><tr style="font-size:0.85em; font-weight: bold;">' +
                        '<td style="width: 20%;">' + $scope.ptyPayload.totalcount + '</td>'
                        +'<td style="width: 20%;">' + totalAmount + '</td></tr></table>';
                    
                    $scope.header += subHeader.replace('0.9em', '0.7em').replace('0.85em', '0.65em');
                    angular.element('#pyTypeSummary').html('').append(subHeader);
                    
                    var rows = '';

                    angular.forEach($scope.ptyCollection, function (r, i)
                    {
                        rows += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">'
                            + $scope.ptyPayload.sn + '</td><td>' +
                            r.Name + '</td><td>' + r.Description + '</td><td>' +
                            r.TotalTransactions + '</td><td>' + r.TotalTransactionValue + '</td></tr>';
                        $scope.ptyPayload.sn++;
                    });
                     
                    $('#paymentTypeRepTbl > tbody:last-child').append($scope.compiler(rows)($scope));
                }

                return;
            }

            angular.forEach(data, function (r, i)
            {
                var tt = $scope.ptyCollection.filter(function (t)
                {
                    return t.StorePaymentMethodId === r.StorePaymentMethodId;
                });
                if (tt.length > 0)
                {
                    var f = tt[0];
                    f.TotalTransactions += r.TotalTransactions;
                    f.TotalTransactionValue += r.TotalTransactionValue;
                }
                else
                {
                    $scope.ptyCollection.push(r);
                }
                $scope.ptyPayload.totalcount += r.TotalTransactions;
                $scope.ptyPayload.totalAmount += r.TotalTransactionValue;
            });
            
            $scope.ptyPayload.itemsPerPage = 100;
            $scope.ptyPayload.pageNumber += 1;
            $scope.retrievePaymentTypeReport($scope.ptyPayload);
        };

        $rootScope.getProductReports = function (selectedProduct)
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1)
            {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1)
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
            var strDt = day1 + '/' + month1 + '/' + year1;
            var endDt = day2 + '/' + month2 + '/' + year2;

            var product =
            {
               StoreItemStockId: 0
            };

            if (selectedProduct !== undefined && selectedProduct !== null)
            {
                product = selectedProduct.originalObject;
            }

            $scope.prPayload =
            {
                product: product,
                startDate: startDate,
                endDate: endDate,
                strDt: strDt,
                endDt: endDt,
                itemsPerPage: 100,
                pageNumber: 0,
                totalQuantitySold: 0,
                totalAmountSold: 0,
                totalStockQuantity: 0,
                totalStockValue: 0,
                sn: 1
            };
            angular.element('#proRepTbl tbody').empty();
            $scope.prCollection = [];
            $scope.retrieveProductReport($scope.prPayload);
        }

        $scope.retrieveProductReport = function(payload)
        {
            var url = '';
              
            if (payload.product.StoreItemId === undefined || payload.product.StoreItemStockId == null || payload.product.StoreItemStockId < 1)
            {
                url = '/Report/GetAllProductSalesReport?startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            else
            {
                $scope.product = payload.product;
                url = '/Report/GetSingleProductReport?itemId=' + payload.product.StoreItemStockId + '&startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }

            $scope.productReport = true;
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getProductReportsCompleted);
        }
        
        $scope.getProductReportsCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.prPayload.totalAmountSold > 0)
                {
                    var totalAmountSold = filterCurrency($scope.prPayload.totalAmountSold);
                    var totalQuantitySold = filterNumber($scope.prPayload.totalQuantitySold);
                    var totalStockQuantity = filterNumber($scope.prPayload.totalStockQuantity);
                    var totalStockValue = filterCurrency($scope.prPayload.totalStockValue);
                  
                    angular.element('#prtPrTbl').show();
                    $scope.productReport = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    + '<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';
                    
                    if ($scope.prPayload.product.StoreItemStockId > 0)
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales of <b>' + $scope.prPayload.product.Name + '</b> for the period <b>' + $scope.prPayload.strDt + '-' + $scope.prPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';
                    }
                    else
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales of all products for the period <b>' + $scope.prPayload.strDt + '-' + $scope.prPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';

                    }

                    $scope.header += '</table><br/>';

                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div><table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.9em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL QTY. SOLD</td><td style="width: 20%;">TOTAL AMOUNT SOLD' +
                        '</td><td style="width: 20%">TOTAL QTY. IN STOCK</td><td style="width: 20%">' +
                        'TOTAL STOCK VALUE</td></tr>' +
                        '<tr style="font-size:0.85em; font-weight: bold"><td style="width: 20%">' + totalQuantitySold + '</td><td style="width: 20%;">' + totalAmountSold + '</td><td style="width: 20%">' + totalStockQuantity + '</td><td style="width: 20%">' + totalStockValue + '</td>' +
                        '</tr></table>';

                    $scope.header += subHeader.replace('0.9em', '0.7em').replace('0.85em', '0.65em');
                    angular.element('#prSummary').html('').append(subHeader);
                }

                return;
            }

            var rows = '';

            angular.forEach(data, function (r, i)
            {
                $scope.prCollection.push(r);

                var totalAmountSold = r[7].replace(',', '');
                var totalQuantitySold = r[5].replace(',', '');
                var totalStockQuantity = r[8].replace(',', '');
                var totalStockValue = r[9].replace(',', '');

                $scope.prPayload.totalAmountSold += parseFloat(totalAmountSold);
                $scope.prPayload.totalQuantitySold += parseFloat(totalQuantitySold);
                $scope.prPayload.totalStockQuantity += parseFloat(totalStockQuantity);
                $scope.prPayload.totalStockValue += parseFloat(totalStockValue);

                //var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getProductSalesReportDetail(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                rows += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.prPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td><td>' + r[9] + '</td></tr>';
                $scope.prPayload.sn++;
            });
            
            $('#proRepTbl > tbody:last-child').append($scope.compiler(rows)($scope));
            $scope.prPayload.itemsPerPage = 100;
            $scope.prPayload.pageNumber += 1;
            $scope.retrieveProductReport($scope.prPayload);
        };

        $rootScope.getEmployeeReports = function (selectedEmployee)
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1) {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1) {
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
            var strDt = day1 + '/' + month1 + '/' + year1;
            var endDt = day2 + '/' + month2 + '/' + year2;

            var employee =
            {
                EmployeeId : 0
            };

            if (selectedEmployee !== undefined && selectedEmployee !== null)
            {
                employee = selectedEmployee.originalObject;
            }

            $scope.empPayload =
            {
                employee: employee,
                startDate: startDate,
                endDate : endDate,
                strDt : strDt,
                endDt: endDt,
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
            $scope.retrieveEmployeeReport($scope.empPayload);
        };

        $scope.retrieveEmployeeReport = function(payload)
        {
            var url = '';
            
            if (payload.employee === undefined || payload.employee == null || payload.employee.EmployeeId < 1)
            {
                url = '/Report/GetAllEmployeeSalesReport?startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            else
            {
                $rootScope.employee = payload.employee;

                url = '/Report/GetSingleEmployeeSalesReport?employeeId=' + payload.employee.EmployeeId + '&startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getEmployeeReportsCompleted);
        }

        $scope.getEmployeeReportsCompleted = function (data)
        {
            $scope.processing = false;
            if (data.length < 1)
            {
                if ($scope.empPayload.totalPaid > 0)
                {
                    var totalPaid = filterCurrency($scope.empPayload.totalPaid);
                    var totalDue = filterCurrency($scope.empPayload.totalAmountDue);
                    var totalVat = filterCurrency($scope.empPayload.totalVAT);
                    var totalDiscount = filterCurrency($scope.empPayload.totalDiscount);
                    var totalNet = filterCurrency($scope.empPayload.totalNet);

                    angular.element('#prtEmpTbl').show();
                    $scope.empReport = true;

                    $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                                    +'<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                                    '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';

                    

                    if ($scope.empPayload.employee.EmployeeId > 0)
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales by <b>' + $scope.empPayload.employee.Name + '</b> for the period <b>' + $scope.empPayload.strDt + '-' + $scope.empPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';
                    }
                    else
                    {
                        $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales by all cashiers for the period <b>' + $scope.empPayload.strDt + '-' + $scope.empPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';

                    }

                    $scope.header += '</table><br/>';

                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div><table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.85em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL AMOUNT DUE</td><td style="width: 20%;">TOTAL VAT' +
                        '</td><td style="width: 20%">TOTAL DISCOUNT</td><td style="width: 20%">' +
                        'TOTAL NET</td><td style="width: 20%;">TOTAL AMOUNT PAID</td></tr>' +
                        '<tr style="font-size:0.79em; font-weight: bold"><td style="width: 20%">' + totalDue + '</td><td style="width: 20%;">' + totalVat + '</td><td style="width: 20%">' + totalDiscount + '</td><td style="width: 20%">' + totalNet + '</td>' +
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
                
                $scope.empPayload.totalAmountDue += parseFloat(totalDue);
                $scope.empPayload.totalVAT += parseFloat(totalVat);
                $scope.empPayload.totalDiscount += parseFloat(totalDiscount);
                $scope.empPayload.totalNet += parseFloat(totalNet);
                $scope.empPayload.totalPaid += parseFloat(totalPaid);

                var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                rows += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.empPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[8] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td>' + template + '</tr>';
                $scope.empPayload.sn++;
            });
            
            angular.element('#empRepTbl > tbody:last-child').append($scope.compiler(rows)($scope));
            $scope.empPayload.itemsPerPage = 100;
            $scope.empPayload.pageNumber += 1;
            $scope.retrieveEmployeeReport($scope.empPayload);
        };

        $rootScope.GetCustomerInvoiceReports = function ()
         {
             if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1) {
                 alert('Please select date range');
                 return;
             }

             if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1) {
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


             if ($scope.invoiceTableDir !== undefined && $scope.invoiceTableDir != null)
             {
                 $scope.invoiceReport = false;
                 $scope.invoiceTableDir.fnClearTable();
             }
             else
             {
                 $scope.processing = true;
                 $scope.invoiceReport = true;
                 var tableDir = angular.element('#invoiceRepTbl');
                 var tableOptions = {};
                 tableOptions.sourceUrl = '/Report/GetCustomerInvoiceReports?' + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                 tableOptions.itemId = 'Id';
                 tableOptions.columnHeaders = ['CustomerName', 'TotalAmountDueStr', 'TotalVATAmountStr', 'TotalDiscountAmountStr', 'TotalAmountPaidStr', 'InvoiceBalanceStr', 'DateProfiledStr'];
                 var invoiceTableDir = customerInvoiceReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getInvoiceReport');
                 $scope.invoiceTableDir = invoiceTableDir;
             }
        }
        
        $rootScope.GetCustomerSalesReport = function (customer)
        {
            if ($scope.startDate === undefined || $scope.startDate == null || $scope.startDate.length < 1) {
                alert('Please select date range');
                return;
            }

            if ($scope.endDate === undefined || $scope.endDate == null || $scope.endDate.length < 1) {
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
            var strDt = day1 + '/' + month1 + '/' + year1;
            var endDt = day2 + '/' + month2 + '/' + year2;

            var selCustomer =
            {
                CustomerId : 0
            };

            if (customer !== undefined && customer !== null && customer.CustomerId > 0)
            {
                selCustomer = customer;
            }

            $scope.customerPayload =
            {
                customer: selCustomer,
                startDate: startDate,
                endDate : endDate,
                strDt : strDt,
                endDt: endDt,
                itemsPerPage: 100,
                pageNumber: 0,
                totalAmountDue: 0,
                totalVAT: 0,
                totalDiscount: 0,
                totalNet: 0,
                totalPaid: 0,
                sn: 1
            };
            $scope.allCustomerCollection = [];
            $scope.singleCustomerCollection = [];

            $scope.header = '<table style="width:100%"><tr style="font-size:0.9em;"><td style="width: 20%"></td><td style="width: 60%;"><h4>' + $rootScope.store.StoreName + '</h4></td><td style="width: 20%"></td></tr>'
                          + '<tr style="font-size:0.9em"><td style="width: 20%;"></td><td  style="width: 60%"><h5>' + $rootScope.store.StoreAddress + '</h5></td><td style="width: 20%"></td></tr>' +
                          '<tr><td colspan="3"></td></tr><tr><td colspan="3"></td></tr>';

            if (selCustomer === undefined || selCustomer == null || selCustomer.CustomerId < 1)
            {
                $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales Transactions with all Customers for the period <b>' + $scope.customerPayload.strDt + '-' + $scope.customerPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';

                angular.element('#singleCustomerTbl').fadeOut('fast');
                angular.element('#allCustomerTbl tbody').empty();
                angular.element('#allCustomerTbl').show();
            }
            else
            {
                $scope.header += '<tr style="font-size:0.9em;"><td style="width: 20%"></td><td  style="width: 60%;"><h5 style="border-bottom: 1px solid #000">Sales Transactions with Customer: <b>' + selCustomer.UserProfileName + '</b> for the period <b>' + $scope.customerPayload.strDt + '-' + $scope.customerPayload.endDt + '</b></h5></td><td style="width: 20%"></td></tr>';
                angular.element('#singleCustomerTbl').fadeIn();

                $scope.customer = selCustomer;

                angular.element('#allCustomerTbl').fadeOut('fast');
                angular.element('#singleCustomerTbl tbody').empty();
                angular.element('#singleCustomerTbl').show();
            }
            $scope.header += '</table><br/>';
            angular.element('#prtCustomerTbl').show();
            
            $scope.retrieveCustomerReport($scope.customerPayload);
        };

        $scope.retrieveCustomerReport = function (payload)
        {
            var url = '';
            
            if (payload.customer === undefined || payload.customer == null || payload.customer.CustomerId < 1)
            {
                url = '/Report/GetAllCustomersSalesReport?startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            else
            {
                $scope.repCustomer = payload.customer;
                url = '/Report/GetSingleCustomerSalesReport?customerId=' + payload.customer.CustomerId + '&startDateStr=' + payload.startDate + '&endDateStr=' + payload.endDate + '&itemsPerPage=' + payload.itemsPerPage + '&pageNumber=' + payload.pageNumber;
            }
            
            $scope.processing = true;
            salesReportServices.getReports(url, $scope.getCustomerReportsCompleted); 
        }

        $scope.getCustomerReportsCompleted = function (data)
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
                    var totalNet = filterCurrency($scope.customerPayload.totalNet);
                    var subHeader = '<div class="row" style=" margin-top: 2%"><b>SUMMARY:</b></div><table style="border: solid 1px #ddd; width: 100%; margin-bottom: 2%"><tr style="border-bottom: solid 1px #ddd; font-size:0.85em; font-weight: bold;"><td style="width: 20%">' +
                        'TOTAL AMOUNT DUE</td><td style="width: 20%;">TOTAL VAT' +
                        '</td><td style="width: 20%">TOTAL DISCOUNT</td><td style="width: 20%">' +
                        'TOTAL NET</td><td style="width: 20%;">TOTAL AMOUNT PAID</td></tr>' +
                        '<tr style="font-size:0.79em; font-weight: bold"><td style="width: 20%">' + totalDue + '</td><td style="width: 20%;">' + totalVat + '</td><td style="width: 20%">' + totalDiscount + '</td><td style="width: 20%">' + totalNet + '</td>' +
                        '<td style="width: 20%;">' + totalPaid + '</td></tr></table>';
                    $scope.customerReport = true;
                    $scope.header += subHeader.replace('0.85em', '0.7em').replace('0.79em', '0.65em');
                    angular.element('#customerSummary').html('').append(subHeader);
                }
                  
                return;
            }

            if ($scope.customerPayload.customer !== undefined && $scope.customerPayload.customer !== null && $scope.customerPayload.customer.CustomerId > 0)
            {
                $scope.buildSingleCustomerRepTable(data);
            }
            else {
                
                $scope.buildMultipleCustomerRepTable(data);
            }
            
        };
        
        $scope.buildSingleCustomerRepTable = function (data)
        {
            angular.forEach(data, function (r, i)
            {
                $scope.singleCustomerCollection.push(r);

                var totalDue = r[5].replace(',', '');
                var totalVat = r[6].replace(',', '');
                var totalDiscount = r[7].replace(',', '');
                var totalNet = r[8].replace(',', '');
                var totalPaid = r[9].replace(',', '');

                $scope.customerPayload.totalAmountDue += parseFloat(totalDue);
                $scope.customerPayload.totalVAT += parseFloat(totalVat);
                $scope.customerPayload.totalDiscount += parseFloat(totalDiscount);
                $scope.customerPayload.totalNet += parseFloat(totalNet);
                $scope.customerPayload.totalPaid += parseFloat(totalPaid);

                var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                var row = '<tr role="row" class="odd" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.customerPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td>' + template + '</tr>';
                angular.element('#singleCustomerTbl > tbody:last-child').append($scope.compiler(row)($scope));
                $scope.customerPayload.sn++;
            });
            
            $scope.customerPayload.itemsPerPage = 100;
            $scope.customerPayload.pageNumber += 1;
            $scope.retrieveCustomerReport($scope.customerPayload);
        };

        $scope.buildMultipleCustomerRepTable = function (data)
        {
            angular.forEach(data, function (r, i)
            {
                $scope.allCustomerCollection.push(r);

                var totalDue = r[5].replace(',', '');
                var totalVat = r[6].replace(',', '');
                var totalDiscount = r[7].replace(',', '');
                var totalNet = r[8].replace(',', '');
                var totalPaid = r[9].replace(',', '');

                $scope.customerPayload.totalAmountDue += parseFloat(totalDue);
                $scope.customerPayload.totalVAT += parseFloat(totalVat);
                $scope.customerPayload.totalDiscount += parseFloat(totalDiscount);
                $scope.customerPayload.totalNet += parseFloat(totalNet);
                $scope.customerPayload.totalPaid += parseFloat(totalPaid);

                var template = '<td style="width: 5%; text-align:center"><a title="Get Report Details" id="' + r[0] + '" style="cursor: pointer" ng-click = "' + 'getSalesReport(' + r[0] + ')"><img src="/Content/images/details.png" /></a></td>';
                var row = '<tr role="row" style="border-top:solid 1px #ddd;"><td  style="width: 3%;">' + $scope.customerPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td><td>' + r[9] + '</td>' + template + '</tr>';
                angular.element('#allCustomerTbl > tbody:last-child').append($scope.compiler(row)($scope));
                $scope.customerPayload.sn++;
            });

            $scope.customerPayload.itemsPerPage = 100;
            $scope.customerPayload.pageNumber += 1;
            $scope.retrieveCustomerReport($scope.customerPayload);
        };

        /*SALES REPORT TYPES END*/

        $scope.getSalesReport = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            salesReportServices.getProduct(id, $scope.getSalesReportDetailsCompleted);
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

            salesReportServices.getProductSalesReportDetail(productReport, $scope.getProductSalesReportDetailsCompleted);
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
                reportHtml += '<tr style="border-bottom: #ddd solid 1px;font-size:1em"><td>' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td >' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
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

        $scope.printOutletReport = function (collection)
        {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Invoice No.</th><th style="width: 12%; text-align: left">Date</th><th style="width: 12%; text-align: left">' +
                'Customer</th><th style="width: 12%; text-align: left">Amount Due(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 8%; text-align: left">' +
                'VAT(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 10%; text-align: left">Discount(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                '<th style="width: 12%; text-align: left">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 12%; text-align: left">' +
                'Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
            var sn = 1;

            angular.forEach(items, function (r, i) {
                sr += '<tr role="row" class="odd" style="border-top:solid 1px #ddd; font-size:0.62em"><td>' + sn + '</td><td>' + r[1] + '</td><td>' + r[8] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td></tr>';
                sn++;
            });
            sr += '</tbody></table><br/>';
            $scope.printReport(sr);
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

        $scope.printProductReport = function (collection)
        {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1)
            {
                console.log('collection is empty');
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.62em;height:32px;"><th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">' +
                'Invoice No.</th><th style="width: 12%; text-align: left">Date</th><th style="width: 12%; text-align: left">SKU</th><th style="width: 12%; text-align: left">' +
                'Item</th><th style="width: 8%; text-align: left">Qty. Sold</th><th style="width: 10%; text-align: left">' +
                'Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 12%; text-align: left">' +
                'Amount(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 10%; text-align: left">Qty. in Stock' +
                '</th><th style="width: 10%; text-align: left">Stock Value</th></tr></thead><tbody>';

            var sn = 1;
            angular.forEach(items, function (r, i)
            {
                sr += '<tr style="border-top:solid 1px #ddd; font-size:0.6em;"><td  style="width: 3%;">' + $scope.prPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td style="width: 12%; text-align: center">' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td><td>' + r[9] + '</td></tr>';
                sn++;
            });

            sr += '</tbody></table><br/>';
            $scope.printReport(sr);
        };

        $scope.printPaymentTypeReport = function (collection)
        {
            var items = $scope[collection];
            if (items === undefined || items === null || items.length < 1)
            {
                console.log('collection is empty');
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr  style="color: #000; border-top: solid 1px #ddd; font-size:0.62em;height:32px;"><th style="width: 3%; text-align: left">S/N</th><th style="width: 15%; text-align: left">' +
            'Payment Method</th><th style="width: 15%; text-align: left">Transaction Date</th><th style="width: 16%; text-align: left">' +
            'Amount(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 13%; text-align: left">Invoice Number</th><th style="width: 20%">' +
            'Customer</th></tr></thead><tbody>';

            var sn = 1;

            angular.forEach(items, function (r, i) {
                sr += '<tr role="row" class="odd" style="border-top:solid 1px #ddd; font-size:0.6em;"><td  style="width: 3%;">'
                    + $scope.ptyPayload.sn + '</td><td>' +
                    r.Name + '</td><td>' + r.Description + '</td><td>' +
                    r.TotalTransactions + '</td><td>' + r.TotalTransactionValue + '</td></tr>';
                sn++;
            });
            sr += '</tbody></table><br/>';
            $scope.printReport(sr);
        };

        $scope.printCustomerReport = function ()
        {
            if ($scope.customerPayload.customer !== undefined && $scope.customerPayload.customer !== null && $scope.customerPayload.customer.CustomerId > 0)
            {
                var singleItems = $scope.singleCustomerCollection;
                if (singleItems === undefined || singleItems === null || singleItems.length < 1)
                {
                    return;
                }

                $scope.printSingleCustomerReport(singleItems);
            }
            else
            {
                var items = $scope.allCustomerCollection;
                if (items === undefined || items === null || items.length < 1)
                {
                    return;
                }

                $scope.printAllCustomersReport(items);

            }
        };

        $scope.printAllCustomersReport = function (items)
        {
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Invoice No.</th><th style="width: 12%; text-align: left">Date</th><th style="width: 12%; text-align: left">' +
                'Customer</th><th style="width: 12%; text-align: left">' +
                'Outlet</th><th style="width: 12%; text-align: left">Amount Due(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 8%; text-align: left">' +
                'VAT(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 10%; text-align: left">Discount(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                '<th style="width: 12%; text-align: left">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 12%; text-align: left">' +
                'Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
            var sn = 1;

            angular.forEach(items, function (r, i)
            {
                sr += '<tr role="row" class="odd" style="border-top:solid 1px #ddd;font-size:0.62em"><td  style="width: 3%;">' + $scope.customerPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[3] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td><td>' + r[9] + '</td></tr>';
                sn++; 
            });
            sr += '</tbody></table><br/>';
            $scope.printReport(sr);
        };

        $scope.printSingleCustomerReport = function (items)
        {
            if (items === undefined || items === null || items.length < 1)
            {
                return;
            }

            var sr = $scope.header + '<table style="border: solid 1px #ddd; width:100%"><thead><tr style="color: #000; border-top: solid 1px #ddd; font-size:0.65em;height:32px">' +
                '<th style="width: 3%; text-align: left">S/N</th><th style="width: 12%; text-align: left">Invoice No.</th><th style="width: 12%; text-align: left">Date</th><th style="width: 12%; text-align: left">' +
                'Outlet</th><th style="width: 12%; text-align: left">Amount Due(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 8%; text-align: left">' +
                'VAT(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 10%; text-align: left">Discount(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                '<th style="width: 12%; text-align: left">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + ')</th><th style="width: 12%; text-align: left">' +
                'Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
            var sn = 1;

            angular.forEach(items, function (r, i)
            {
                sr += '<tr style="border-top:solid 1px #ddd;font-size:0.62em"><td  style="width: 3%;">' + $scope.customerPayload.sn + '</td><td>' + r[1] + '</td><td>' + r[2] + '</td><td>' + r[4] + '</td><td>' + r[5] + '</td><td>' + r[6] + '</td><td>' + r[7] + '</td><td>' + r[8] + '</td><td>' + r[9] + '</td></tr>';
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




