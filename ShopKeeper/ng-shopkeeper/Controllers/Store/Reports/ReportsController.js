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
    app.register.controller('reportController', ['$scope', '$rootScope', '$routeParams', 'reportServices', '$http',
    function ($scope, $rootScope, $routeParams, reportServices)
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

        $scope.getDefaults = function ()
        {
            $scope.items = [];
            reportServices.getDefaults($scope.getDefaultsCompleted);
        };

        $scope.setOption = function (opt) {
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

            if (data.Items != null && data.length > 0)
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

            if (product.StoreItemId === undefined || product.StoreItemId == null || product.StoreItemId < 1)
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

            if ($scope.prTableDir !== undefined && $scope.prTableDir != null) {
                $scope.productReport = false;
                $scope.prTableDir.fnClearTable();
            } else {
                $scope.processing = true;
                $scope.productReport = true;
                var tableDir = angular.element('#proRepTbl');
                var tableOptions = {};
                tableOptions.sourceUrl = '/Report/GetProductSalesReport?productId=' + product.StoreItemId + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
                tableOptions.itemId = 'SaleId';
                tableOptions.columnHeaders = ['StoreItemName', 'AmountDue', 'AmountPaidStr', 'DateStr'];
                var prTableDir = employeeReportTableManager($scope, $scope.compiler, tableDir, tableOptions, 'getReport');
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

            if (employee.Id === undefined || employee.Id == null || employee.Id < 1)
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
                tableOptions.sourceUrl = '/Report/GetEmployeeSalesReport?employeeId=' + employee.Id + '&startDateStr=' + startDate + '&endDateStr=' + endDate;
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




