"use strict";

define(['application-configuration', 'saleServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngPSku', function ()
    {
        return function ($scope, ngPSku)
        {
            ngPSku.bind("keydown keypress", function (event)
            {
                if (event.which === 13)
                {
                    $scope.criteria = event.target.value;
                    $scope.getpItemPrices();
                }
            });
            $scope.skupControl = ngPSku;
            $scope.skupControl.focus();
        };
    });
    //ngUpdateSale
    app.register.directive('ngUpdateSale', function ($compile)
    {
        return function ($scope, ngUpdateSale)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/Sales/GetUncompletedTransaction";
            tableOptions.itemId = 'SaleId';
            tableOptions.columnHeaders = ["InvoiceNumber", "CustomerName", 'DateStr', "NetAmountStr", 'AmountPaidStr', "BalanceStr"];
            var ttc = invoiceUpdateTableManager($scope, $compile, ngUpdateSale, tableOptions, 'getInvoiceDetails');
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        }; 
    });
    
    app.register.controller('updateSalesController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'saleServices', '$filter', '$locale',
        function (ngDialog, $scope, $rootScope, $routeParams, saleServices, $filter, $locale)
        {

        $scope.goBack = function ()
        {
            $scope.details = false;
        };
       
        $scope.initializeController = function ()
        {
            $scope.newTransaction = false;
            $scope.details = false;
            saleServices.getPaymenthod($scope.getPaymenthodCompleted);
        };

        $scope.getPaymenthodCompleted = function (data)
        {
            $scope.paymentMethods = data.PaymentMethods;
        };

        function filterCurrency(amount, symbol)
        {
            var currency = $filter('currency');
            var value = currency(amount, symbol);
            return value;
        }

        $scope.getInvoiceDetails = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            saleServices.getInvoice(id, $scope.getInvoiceDetailsCompleted);
        };

        $scope.getInvoiceDetailsCompleted = function (rec)
        {
            if (rec.SaleId < 1)
            {
                alert("Report information could not be retrieved");
                return;
            }

            $scope.initializeModel();

            $scope.sale = rec;
            $scope.sale.outstanding = $scope.sale.Balance;
            $scope.sale.tempBalance = $scope.sale.Balance;
            $scope.sale.tempAmountPaid = $scope.sale.AmountPaid;
            $scope.customerDetail = rec.CustomerObject;
            $scope.cashAmount = 0;
            $scope.cash = true;
            $scope.details = true;

            $scope.rec =
              {
                  referenceCode: rec.InvoiceNumber,
                  date: rec.DateStr,
                  customer: rec.CustomerName,
                  time: '',
                  storeAddress: $rootScope.store.StoreAddress,
                  cashier: rec.SaleEmployeeName,
                  change: 0,
                  receiptItems: rec.StoreItemSoldObjects,
                  amountDue: rec.AmountDue,
                  subtotal: 0,
                  estimateNumber: rec.EstimateNumber,
                  amountReceived: 0,
                  totalAmountPaid: rec.AmountPaid,
                  outstanding: rec.Balance,
                  paymentChoices: rec.Transactions,
                  netAmount: rec.NetAmount,
                  discountAmount: rec.DiscountAmount,
                  vatAmount: rec.VATAmount
              };
        };

        $scope.initializeModel = function ()
        {
            $scope.selected = undefined;
            $scope.printChoice = 2;
            $scope.sale =
            {
                'SaleId': 0,
                'RegisterId': 0,
                'CustomerId': 0,
                'EmployeeId': 0,
                'AmountDue': 0,
                'Status': '',
                'Date': '',
                Discount: '',
                VATAmount: 0,
                DiscountAmount: '',
                TotalAmountPaid: 0,
                NetAmount: 0,
                VAT: 0,
                EstimateNumber: '',
                Transactions: [],
                'paymentOption': { 'StorePaymentMethodId': '', 'Name': '-- select payment option --' }
            };
            
            $scope.posAmount = ''; 
            $scope.sale.currentAmountPaid = 0;
            $scope.sale.Balance = 0;
            $scope.cashAmount = 0;
            $scope.cash = false;
            $scope.pos = false;
            $scope.splitOption = false;
        };

        $scope.setPaymentOption = function(opt) 
        {
            $scope.sale.paymentOption = opt;

            if (opt.StorePaymentMethodId === 1 || opt.Name.toLowerCase() === 'cash')
            {
                $scope.cashAmount = $scope.sale.outstanding;
                $scope.updateamountpaid($scope.cashAmount);
            }
           
        };
            
       $scope.initializeTransaction = function ()
        {
            $scope.transaction =
            {
                'StoreTransactionId': '',
                'TempId': '',
                'StoreTransactionTypeId': '',
                'StorePaymentMethod': { 'StorePaymentMethodId': '', 'Name': '-- Select Option --' },
                'EffectedByEmployeeId': '',
                'TransactionAmount': 0,
                'TransactionDate': '',
                'Remark': '',
                'StoreOutletId': ''
            };
           
            $scope.lessAmount = '';
            $scope.incompleteAmountDue = false;
        };
        
       $scope.updateamountpaid = function (amountPaid)
       {
           var tesValue = parseFloat(amountPaid);
           
           if (tesValue !== undefined && tesValue !== null && tesValue !== NaN && tesValue > 0)
           {
               if (tesValue === $scope.sale.Balance)
               {
                   $scope.rec.amountToBalance = 0;
                   $scope.sale.currentAmountPaid = amountPaid;
                   $scope.sale.outstanding = 0;
                   $scope.rec.amountReceived += $scope.sale.Balance;

                   return;
               }
               else
               {
                   if (amountPaid > $scope.sale.Balance)
                   {
                       $scope.rec.amountToBalance = $scope.sale.Balance - amountPaid;
                       $scope.sale.outstanding = $scope.sale.Balance - amountPaid;

                       $scope.rec.amountReceived = $scope.sale.Balance;

                   }
                   if (amountPaid < $scope.sale.Balance)
                   {
                       $scope.sale.currentAmountPaid = amountPaid;
                       $scope.sale.outstanding = $scope.sale.tempBalance - amountPaid;
                       $scope.rec.outstanding = $scope.sale.outstanding;
                       $scope.rec.amountReceived += amountPaid;
                   }
                   
               }
           }
           else
           {
               $scope.rec.amountReceived = 0;
               $scope.sale.currentAmountPaid = 0;
               $scope.sale.outstanding = $scope.sale.Balance;
               $scope.rec.outstanding = $scope.sale.outstanding;
               $scope.rec.amountReceived = $scope.sale.AmountPaid;
               $scope.rec.amountToBalance = 0;
              
           }

           $scope.sale.tempBalance = $scope.sale.Balance;

       };
        
       $scope.isValue = function (val)
       {
           return !(val === null || !angular.isDefined(val) || (angular.isNumber(val) && !isFinite(val)));
       };

       $scope.setError = function (errorMessage)
       {
           $scope.error = errorMessage;
           $scope.negativefeedback = true;
           $scope.success = '';
           $scope.positivefeedback = false;
       };

       $scope.clearError = function ()
       {
           $scope.error = '';
           $scope.negativefeedback = false;
           $scope.success = '';
           $scope.positivefeedback = false;
       };
        
       $scope.setSuccessFeedback = function (successMessage)
       {
           $scope.error = '';
           $scope.negativefeedback = false;
           $scope.success = successMessage;
           $scope.positivefeedback = true;
       };

       $scope.addTransaction = function (transaction)
       {
           if ($scope.sale.Transactions.length > 0)
           {
               var matchfound = false;
               angular.forEach($scope.sale.Transactions, function (x, y)
               {
                   if (x.StorePaymentMethodId === transaction.StorePaymentMethod.StorePaymentMethodId)
                   {
                       x.TransactionAmount += transaction.TransactionAmount;
                       matchfound = true;
                   }
               });
               
               if (!matchfound)
               {
                  transaction.TempId = 1;
                  $scope.sale.Transactions.push(transaction);
               }
           }
           else
           {
               transaction.TempId = 1;
               $scope.sale.Transactions.push(transaction);
               
           }

           $scope.rec.paymentChoices = $scope.sale.Transactions;
           $scope.initializeTransaction();
       };
     
       $scope.getGenericListCompleted = function (data)
       {
           $scope.customers = data.Customers;
           $scope.paymentMethods = data.PaymentMethods;
       };
        
       $scope.prepareSaleTemplate = function ()
       {
           $scope.initializeModel();
           $scope.sale.Header = 'Complete transaction payment';
           $scope.clicked = true;
           $scope.buttonStatus = 1;
       };

       $scope.validateSale = function (sale)
       {
           if (sale.Transactions == undefined || sale.Transactions == null || sale.Transactions.length < 1)
           {
               $scope.setError('ERROR: Please complete the transaction payment first.');
               return false;
           }
           
           return true;
       };
        
       $scope.hideDelete = function ()
       {
           $scope.deleteItem = false;
       };
       
       $scope.updateSalePayment = function ()
       {
           if (parseInt($scope.sale.paymentOption.StorePaymentMethodId) < 1)
           {
               alert('Please select payment option!');
               return;
           }

           var transaction =
           {
               'StoreTransactionId': '',
               'TempId': '',
               'StoreTransactionTypeId': 1,
               'StorePaymentMethod': '',
               'StorePaymentMethodId': 0,
              'EffectedByEmployeeId': '',
              'TransactionAmount': '',
              'TransactionDate': '',
              'StoreOutletId': ''
          };
           $scope.sale.Transactions = [];
          transaction.TransactionAmount = $scope.cashAmount;
          transaction.StorePaymentMethod = $scope.sale.paymentOption;
          transaction.StorePaymentMethodId = $scope.sale.paymentOption.StorePaymentMethodId;
          //$scope.addTransaction(transaction);
          $scope.sale.Transactions.push(transaction);
            if (!$scope.validateSale($scope.sale))
            {
               return;
            }

            $scope.processing = true;
            saleServices.updateSalePayment($scope.sale, $scope.updateSalePaymentCompleted);
       };
        
       $scope.updateSalePaymentCompleted = function (data)
       {
           $scope.processing = false;
           if (data.Code < 1)
           {
              alert(data.Error);
               return;
           }
           else
           {
               $scope.setSuccessFeedback(data.Error);

               $scope.rec.referenceCode = data.ReferenceCode;
               $scope.rec.date = data.Date;
               $scope.rec.customer = $scope.customerInfo;
               $scope.rec.time = data.Time;
               $scope.rec.estimateNumber = $scope.sale.EstimateNumber;
               $scope.rec.amountReceived = $scope.sale.currentAmountPaid;
               $scope.rec.paymentChoices = $scope.sale.Transactions;
               $scope.rec.totalAmountPaid += $scope.sale.currentAmountPaid;

               $scope.determinePrintOption();
               $scope.ngTable.fnClearTable();
           }
        };
    
       $scope.printReceipt = function (rec) {

           var customerName = '';

           if ($scope.customerDetail !== undefined && $scope.customerDetail !== null && $scope.customerDetail.CustomerId > 0)
           {
               customerName = $scope.customerDetail.UserProfileName;
           }
           var contents = '<table style="width: 100%; padding-left: 10px; border: none;" class="table"><tr style="border: none">'
               + '<td style="width: 20%"></td><td style="width: 50%"><h3 style="width: 100%; text-align: center; font-size:0.9em">' + $rootScope.store.StoreName + '</h3>'
               + '</td><td></td></tr><tr style="border: none"><td style="width: 20%;" colspan="2"><table style="width: 100%;"><tr style="font-size: 0.7em">'
               + '<td style="width: 100%;">' + $rootScope.store.StoreAddress + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">Website:  ' + $rootScope.store.Url + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;">'
               + 'Email Address: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.7em"><td style="width: 100%;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
               + '</tr><tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="border-bottom: 1px solid #000;  width: 50%; text-align: center; margin-left: 40%">PAYMENT RECEIPT</h3></td></tr>' +
               '<tr><td style="width: 100%"></td></tr><tr><td style="width: 100%"></td></tr><tr style="border: none;font-size: 0.75em"><td style="width: 100%">Date: ' + rec.date + " " + rec.time + '</td></tr>' +
               '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Invoice No.: ' + rec.referenceCode + '</td></tr>';

               if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
                   contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
               }

           contents += '<tr style="border: none;font-size: 0.75em"><td style="width: 100%">Customer: ' + customerName + '</td></tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table></td><td style="width: 50%"></td><td><img style="height: 60px" alt="" src="' + $rootScope.store.StoreLogoPath + '">'
               + '</td></tr></table>';

           
           //+ '<td style="width: 100%;padding: 1px;margin-bottom: 10px;">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
           //             + '<tr style="font-size:1.1em;"><td  style=";margin-bottom: 4px;"><h5><b style="border-bottom: 1px solid #000">SALES INVOICE</b></h5></td><td></td></tr>'
           
           contents += '<div class="col-md-12 divlesspadding" style="border-top: #000 solid 1px;">' +
               '<table class="table" role="grid" style="width: 100%;font-size:0.9em">' +
                  '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em"><th style="color: #008000;font-size:0.9em; width:35%">Item</th>' +
                   '<th style="color: #008000;font-size:0.9em; width:20%;">Qty</th><th style="color: #008000;font-size:0.9em; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                    '<th style="color: #008000;font-size:0.9em; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';
                
            angular.forEach(rec.receiptItems, function (item, i) 
            {
                contents += '<tr style="border-bottom: #ddd solid 1px;font-size:0.7em"><td><img src="' + item.ImagePath + '" style="width:50px;height:40px">&nbsp;' + item.StoreItemName + '</td><td>' + item.QuantitySold + '</td><td>' + filterCurrency(item.Rate, '') + '</td>' +
                    '<td style="text-align: right">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
            });

           contents += '</tbody></table></div>' +
               '<table style="width: 100%;font-size:0.9em"><tr><td><h5 style="float:left; text-align:left"></h5></td><td><h5 style="float:right; text-align:right; margin-right:11%"></h5></td>' +
               '</tr><tr><td style="vertical-align: top; "><table class="table" role="grid" style="width:70%; padding-left: 0px;font-size:0.9em; float:left">';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.9em;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td><b style="text-align: right">' + filterCurrency(l.TransactionAmount, '') + '</b></td></tr>';
           });

           contents += '</table></td><td>' +
               '<table class="table" role="grid" style="width: auto; float: right; vertical-align:top;font-size:0.9em;"><tr style="border-top: #ddd solid 1px;"><td style="color: #008000;font-size:0.87em">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td><b style="text-align: right">' + filterCurrency(rec.amountDue, '') + '</b></td></tr>';

           if (rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td><b style="text-align: right">' + filterCurrency(rec.discountAmount, '') + '</b></td></tr>';
           }

           if (rec.vatAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">' + $rootScope.store.VAT + '% VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.vatAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0 || rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b style="text-align: right">' + filterCurrency(rec.netAmount, '') + '</b></td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b style="text-align: right">' + filterCurrency(rec.amountReceived, '') + '</b></td></tr>';

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Total Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td ><b style="text-align: right">' + filterCurrency(rec.totalAmountPaid, '') + '</b></td></tr>';

           var outStanding = $scope.sale.Balance - rec.amountReceived;
          
           if (outStanding > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Outstanding(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td><b style="text-align: right">' + filterCurrency(outStanding, '') + '</td></tr>';
           }
           
           contents += '</table></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.9em">' +
                '<h5>Powered by: www.shopkeeper.ng</b></h5></div></div>';

           //angular.element('#receipt').html('').html(contents);
           
           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null)
           {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function ()
               {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Payment Receipt</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function ()
                   {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                   },

                   500);
               });
           }
           else
           {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Payment Receipt</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function ()
               {
                    window.frames["frame1"].focus();
                    window.frames["frame1"].print();
                    frame1.document.body.innerHTML = '';
                }, 500);
           }
           $scope.initializeModel();
           return false;
        };

       $scope.determinePrintOption = function ()
       {
           if ($scope.printChoice === undefined || $scope.printChoice === null || $scope.printChoice < 1)
           {
               alert('Please select a print option');
               return;
           }

           if ($scope.printChoice === 1)
           {
               $scope.printReceipt($scope.rec);
               return;
           }

           if ($scope.printChoice === 2)
           {
               $scope.printWithLowerDimensions($scope.rec);
               return;
           }
       };

       $scope.printWithLowerDimensions = function (rec) {
          
           var customerName = '';

           if ($scope.customerDetail !== undefined && $scope.customerDetail !== null && $scope.customerDetail.CustomerId > 0)
           {
               customerName = $scope.customerDetail.UserProfileName;
           }

           var contents = '<table style="width: 100%; margin-left: 0px; border: none; color: #000; font-weight: bold" class="table"><tr style="border: none">'
                        + '<td style="width: 100%"><img style="height: 60px; float: left; margin-left:20%" alt="" src="' + $rootScope.store.StoreLogoPath + '"/></td></tr><tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">' + $rootScope.store.StoreAddress + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Website:  ' + $rootScope.store.Url + '</td></tr>'
                        + '<tr style="font-size: 0.8em"><td style="width: 100%;padding: 1px; text-align: center">Email: ' + $rootScope.store.StoreEmail + '</td></tr><tr style="font-size: 0.8em">'
                        + '<td style="width: 100%;padding: 1px;margin-bottom: 10px; text-align: center">Phone: ' + $rootScope.store.PhoneNumber + '</td>'
                        + '<tr style="font-size:1.3em;"><td  style=";margin-bottom: 2px;"><h3 style="border-bottom: 1px solid #000; text-align: center">PAYMENT RECEIPT</h3></td><td></td></tr>'
                        + '</tr><tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Date: ' + rec.date + " " + rec.time + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Invoice No.: ' + rec.referenceCode + '</td></tr>';

                       if (rec.estimateNumber !== undefined && rec.estimateNumber !== null && rec.estimateNumber.length > 0) {
                           contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Estimate No.: ' + rec.estimateNumber + '</td></tr>';
                       }

                       contents += '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Customer: ' + customerName + '</td></tr>'
                        + '<tr style="border: none;font-size: 0.87em"><td style="width: 100%;padding: 1px;">Served by: ' + rec.cashier + '</td></tr></table>';


           contents += 
            '<table class="table" role="grid" style="width: 100%; font-weight: bold; margin-left: 0px;border-bottom: 1px solid #000">' +
               '<thead><tr style="text-align: left; border-bottom: 1px solid #000;font-size:0.9em;"><th style="color: #008000; width:35%">Item</th>' +
                '<th style="color: #008000;font-size:0.93em; width:20%;">Qty</th><th style="color: #008000; width:20%;">Rate(' + $rootScope.store.DefaultCurrencySymbol + ')</th>' +
                 '<th style="color: #008000; width:20%;">Total(' + $rootScope.store.DefaultCurrencySymbol + ')</th></tr></thead><tbody>';

           angular.forEach(rec.receiptItems, function (item, i)
           {
               contents += '<tr><td style="border-bottom: #ddd solid 1px;font-size:0.7em">' + item.StoreItemName + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + item.QuantitySold + '</td><td style="border-bottom: #ddd solid 1px;font-size:0.79em">' + filterCurrency(item.Rate, '') + '</td>' +
                   '<td style="font-size:0.79em">' + filterCurrency(item.AmountSold, " ") + '</td></tr>';
           });

           contents += '</tbody></table>' +
               '<table style="width: 100%; font-weight: bold; margin-left: 0px;">';

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;;"><td style="color: #008000">Total Amount Due(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
               '<td style="text-align: right;"><b>' + filterCurrency(rec.amountDue, '') + '</td>' +
               '</tr>';
           if (rec.discountAmount > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Discount(' + $rootScope.store.DefaultCurrencySymbol + '):</td><td style="text-align: right"><b>' + filterCurrency(rec.discountAmount, '') + '</b></td></tr>';
           }

           if (rec.vatAmount > 0)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">' + $rootScope.store.VAT + '% VAT(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.vatAmount, '') + '</td></tr>';
           }

           if (rec.vatAmount > 0 || rec.discountAmount > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Net Amount(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right;"><b>' + filterCurrency(rec.netAmount, '') + '</td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.amountReceived, '') + '</td></tr><tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td colspan="2"></td></tr><tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr>';

           contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000">Total Amount Paid(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                    '<td style="text-align: right"><b>' + filterCurrency(rec.totalAmountPaid, '') + '</td></tr><tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td colspan="2"></td></tr><tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr>';


           var outStanding = $scope.sale.Balance - rec.amountReceived;

           if (outStanding > 0) {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.87em;"><td style="color: #008000;">Outstanding(' + $rootScope.store.DefaultCurrencySymbol + '):</td>' +
                   '<td style="text-align: right"><b>' + filterCurrency(outStanding, '') + '</td></tr>';
           }

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"><h5 style="float:left; text-align:left;font-size:0.95em; font-weight: bold;">Payment option(s):</h5></td></tr>';

           angular.forEach(rec.paymentChoices, function (l, i)
           {
               contents += '<tr style="border-top: #ddd solid 1px;font-size:0.89em;"><td style="color: #008000;">' + l.StorePaymentMethod.Name + ':</td>' +
                            '<td style="text-align: right"><b>' + filterCurrency(l.TransactionAmount, '') + '</td></tr>';
           });

           contents += '<tr style="border-top: #ddd solid 1px;"><td colspan="2"></td></tr></table><div class="row" style="padding-left: 0px"><div class="col-md-12" style="padding-left: 0px;font-size:0.85em; font-weight: bold;margin-top:1%">' +
               '</div><h5>Powered by: www.shopkeeper.ng</h5></div>';

           //angular.element('#receipt').html('').html(contents);

           var frame1 = frames["frame1"];
           if (frame1 === undefined || frame1 == null) {
               frame1 = angular.element('<iframe style="top: 100px; left: 100px;" name="frame1"></iframe>');
               angular.element('#receipt').append(frame1);

               frame1.load(function () {
                   frame1 = frames["frame1"];
                   frame1.document.open();
                   frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                       '<link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                       + '<title>Payment Receipt</title>');
                   frame1.document.write('</head><body style= "width: 100%">');
                   frame1.document.write('</body></html>');
                   frame1.document.body.innerHTML = contents;
                   frame1.document.close();
                   setTimeout(function () {
                       window.frames["frame1"].focus();
                       window.frames["frame1"].print();
                       frame1.document.body.innerHTML = '';
                   },

                   500);
               });
           }
           else {
               frame1.document.open();
               frame1.document.write('<html><head><link href="/Content/site.css" rel="stylesheet" /><link href="/Content/bootstrap.css" rel="stylesheet" /><link href="/Content/feedbackmessage.css" rel="stylesheet" /><link href="/Content/formControls.css" rel="stylesheet" />'
                    + '<title>Payment Receipt</title>');
               frame1.document.write('</head><body style= "width: 100%">');
               frame1.document.write('</body></html>');
               frame1.document.body.innerHTML = contents;
               frame1.document.close();
               setTimeout(function ()
               {
                   window.frames["frame1"].focus();
                   window.frames["frame1"].print();
                   frame1.document.body.innerHTML = '';
               }, 500);
           }
           $scope.initializeModel();
           return false;
       };

    }]);
   
});

