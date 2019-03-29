"use strict";

define(['application-configuration', 'subscriptionServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive("ngStoreLogo", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.file = (e.srcElement || e.target).files[0];
                    $scope.processFile();
                });
            }
        };
    });
    
    app.register.directive('ngInvoice', function ($compile)
    {
        return function ($scope, ngInvoice)
        {
            $scope.invoice = ngInvoice;
        };
    });
    
    app.register.directive('ngSubscribe', function ($compile) {
        return function ($scope, ngSubscribe) {
            $scope.processButton = ngSubscribe;
        };
    });

    app.register.controller('subscriptionController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'subscriptionServices', '$upload', 'fileReader', '$location', '$http',
    function (ngDialog, $scope, $rootScope, $routeParams, subscriptionServices, $upload, fileReader, $location,$sce, $http)
    {
        $rootScope.applicationModule = "SubscriptionPackage";
        
        $scope.alerts = [];
        
        $scope.explicitHtml = function (val)
        {
           return $sce.trustAsHtml(val);
        };

        $scope.initializeController = function () {
            $scope.store =
            {
                'StoreId': '',
                'StoreName': '',
                'TotalOutlets': 1,
                'StoreAlias': '',
                'CompanyName': '',
                'CustomerEmail': '',
                'SubscriptionStatus': 2,
                'BillingCycleCode': '',
                'StoreLogoPath': '/Content/images/noImage.png',
                'CurrencySymbol': '&#8358;',
                'DateCreated': '',
                'LastUpdated': '',
                'Currency': { 'Name': 'Naira', 'CurrencyId': 1, 'Symbol': '#' },
                'BillingCycle': { 'BillingCycleId': 1, 'Name': 'Monthly', 'Code': 'MNT' },
                'PaymentMethod': { 'Name': '-- Payment Method --', 'PaymentMethodId': '' },
                'SubscriptionPackageId': 0,
                'Amount': '',
                'IsTrial': false
               
            };
            
            $scope.registerModel =
            {
                'UserName': '',
                'Password': '',
                'Email': '',
                'PhoneNumber': ''
            };

            $scope.trial = false;
            $scope.inprogress = false;
            $scope.subSel = false;
            $scope.creatLogin = false;
            $scope.clearErrors();
            $scope.package = {};
        };
        
        $scope.processSubscription = function ()
        {

           var store =
           {
               'StoreId': '',
               'StoreName': $scope.store.StoreName,
               'TotalOutlets': 0,
               'StoreAlias': '',
               'CompanyName':'',
               'CustomerEmail': $scope.store.CustomerEmail,
               'StoreLogoPath': '',
               'DefaultCurrency': $scope.store.Currency.Symbol,
               'DateCreated': '',
               'LastUpdated': '',
               'CurrencyName': $scope.store.Currency.Name,
               'SubscriptionPackageId': $scope.package.SubscriptionPackageId,
               'PackageName' : $scope.package.PackageTitle,
               'StorageSize': $scope.package.FileStorageSpace,
               'Duration': 0,
               'BillingCycleId': 0,
               'IsBankOption' : true,
              'PaymentOption' : ''
           };
           
            if ($scope.trial)
            {
                store.IsTrial = true;
                store.BillingCycleCode = $scope.notAppBillC.Code;
                store.BillingCycleId = $scope.notAppBillC.BillingCycleId;
                store.SubscriptionStatus = 1;
                store.Duration = 14;
                store.PaymentMethodId = $scope.notAppMeth.PaymentMethodId;
                store.IsBankOption = false;
                store.Amount = 0;
            }
            else
            {
                store.BillingCycleCode = $scope.package.PackagePricing.BillingCycleCode;
                store.BillingCycleId = $scope.package.PackagePricing.BillingCycleId; //$scope.store.BillingCycle.BillingCycleId;
                store.PaymentMethodId = $scope.store.PaymentMethod.PaymentMethodId;
                store.Amount = $scope.package.PackagePricing.Price;
                
                store.SubscriptionStatus = 2;
                store.IsTrial = false;
                store.Duration = $scope.package.PackagePricing.Duration + 14;
                
                if (store.BillingCycleId == undefined || store.BillingCycleId == NaN || store.BillingCycleId < 1)
                {
                    $scope.setError('Please select a Subscription Duration');
                    return;
                }

                if (store.PaymentMethodId == undefined || store.PaymentMethodId == null || store.PaymentMethodId == NaN)
                {
                    $scope.setError("ERROR: Please select a Payment method.");
                    return;
                }

                store.PaymentOption = $scope.store.PaymentMethod.Name;
               
                if (store.PaymentMethodId == 1)
                {
                    store.IsBankOption = true;
                } else {
                    store.IsBankOption = false;
                }
            }
            
            if (store.StoreName == undefined || store.StoreName == null || store.StoreName.length < 1)
            {
                $scope.setError('Please provide your Store Name');
                return;
            }
            
            if ($scope.store.CompanyName == undefined || $scope.store.CompanyName == null || $scope.store.CompanyName.length < 1)
            {
                store.CompanyName = store.StoreName;
            }
            else
            {
                store.CompanyName = $scope.store.CompanyName;
            }
            
            if ($scope.store.StoreAlias == NaN || $scope.store.StoreAlias == null || $scope.store.StoreAlias.length < 1)
            {
                var arr = store.StoreName.split(' ');
                var alias = arr[0].trim();
                store.StoreAlias = alias;
            } else {
                store.StoreAlias = $scope.store.StoreAlias;
            }
            
            if ($scope.registerModel.Password == undefined || $scope.registerModel.Password == null || $scope.registerModel.Password.length < 1) {
                $scope.setError('Please provide your account Password');
                return;
            }

            if ($scope.registerModel.Password.length < 8)
            {
                $scope.setError('Please provide a Password of at least eight character lenght');
                return;
            }

            if ($scope.registerModel.Email == undefined || $scope.registerModel.Email == null || $scope.registerModel.Email.length < 1) {
                $scope.setError('Please provide your Email');
                return;
            }

            if ($scope.registerModel.Email.indexOf('@') === -1)
            {
                $scope.setError('Please provide a valid Email');
                return;
            }
            var splitter = $scope.registerModel.Email.split('@')[1].trim();
            if (splitter.indexOf('.') === -1)
            {
                $scope.setError('Please provide a valid Email');
                return;
            }
            
            $scope.registerModel.UserName = $scope.registerModel.Email;
            store.CustomerEmail = $scope.registerModel.Email;

            $scope.inprogress = true;
            subscriptionServices.subscribe(store, $scope.processSubscriptionCompleted);
            
        };
        
        $scope.processSubscriptionCompleted = function (data)
        {
            if (data.Code < 1)
            {
                $scope.setError(data.Error);
                $scope.inprogress = false;
                return;
            }

            var result =
            {
                'storeName' : data.StoreName,
                'storeAddress' :data.StoreAddress,
                'companyName' : data.CompanyName,
                'referenceCode' : data.ReferenceCode,
                'amountToPay' : data.Magnitude,
                'duration' : data.Duration,
                'currencySymbol' : data.CurrencyCode,
                'alias' : data.StoreAlias,
                'successMessage': data.Error,
                'packageName': data.PackageName,
                'isTrial': data.IsTrial,
                'paymentOption': data.PaymentOption
            };
           
            
            $scope.result = result;
            
            var registerModel =
            {
                'UserName': $scope.registerModel.UserName,
                'Password': $scope.registerModel.Password,
                'Email': $scope.registerModel.Email,
                'PhoneNumber': $scope.registerModel.PhoneNumber,
                'AmountDue': data.Magnitude,
                'PackageName': data.PackageName,
                'StoreName': data.StoreName,
                'Duration': data.Duration,
                'CompanyName': data.CompanyName,
                'ReferenceCode': data.ReferenceCode,
                'IsTrial': data.IsTrial,
                'Gx': data.Gx,
                'IsBankOption': data.IsBankOption,
                'PaymentOption': data.PaymentOption
            };
            
            subscriptionServices.regAccount(registerModel, $scope.processSubscriptionAccountCompleted);
                
        };
        
        $scope.processSubscriptionAccountCompleted = function (data)
        {
            $scope.result.successMessage = data.Error;
            subscriptionServices.setresult($scope.result);
            $scope.inprogress = false;
            $scope.initializeController();
            ngDialog.close('/ng-shopkeeper/Views/Master/Subscriptions/ProcessSubscription.html', '');
            $location.path('/Master/SubscriptionSuccess/SubscriptionSuccess');
            
        };
        
        $scope.computeHash = function ()
        {
            subscriptionServices.computeHash($scope.computeHashCompleted);

        };

        $scope.computeHashCompleted = function (data)
        {
            if (data == null || data.length < 1)
            {
                alert('Hash could not be coumputed');
                return;
            }
            
            $scope.hash = data;
            alert('Hash succesfully coumputed!');
        };

        $scope.setPackage = function (subpackage)
        {
            $scope.trial = false;
            if (subpackage == undefined || subpackage.SubscriptionPackageId == NaN || subpackage.SubscriptionPackageId < 1)
            {
                return;
            }
            
            if (subpackage.SubscriptionPackageId == 1 || subpackage.PackageTitle == 'Trial')
            {
                $scope.setTrialPackage(subpackage);
            } else {
                $scope.initializeController();
                $scope.setPackageCompleted(subpackage);
            }
           
        };
        
        $scope.setBCycle = function (billingcycle)
        {
            if (billingcycle == undefined || billingcycle.BillingCycleId < 1)
            {
                return;
            }

            $scope.store.BillingCycle = billingcycle;
        };

        $scope.setMethod = function (pMethod) {
            if (pMethod == undefined || pMethod.PaymentMethodId < 1)
            {
                return;
            }

            $scope.store.PaymentMethod = pMethod;
        };
        
        $scope.setNewPackage = function (subpackage)
        {
            $scope.trial = false;
            if (subpackage == undefined || subpackage.SubscriptionPackageId == NaN || subpackage.SubscriptionPackageId < 1)
            {
                return;
            }
            
            $scope.initializeController();
            
            if (subpackage.SubscriptionPackageId == 1 || subpackage.PackageTitle == 'Trial')
            {
                $scope.trialPackage = subpackage;
                $scope.trial = true;
            }
            else
            {
                $scope.trial = false;
            }
            
            $scope.setPackageCompleted(subpackage);
        };
        
        $scope.setPackageCompleted = function (subpackage)
        {
            if (subpackage == undefined || subpackage.SubscriptionPackageId == NaN || subpackage.SubscriptionPackageId < 1)
            {
               $scope.trial = false;
               $scope.setError('Please select a valid Plan.');
            }
            else
            {
                $scope.package = subpackage;
                $scope.header = subpackage.PackageTitle;
                $scope.subSel = true;
                $scope.package.PackagePricings2 = $scope.package.PackagePricings;
            }
        };
        
        $scope.setTrialPackage = function ()
        {
            if ($scope.trialPackage == undefined || $scope.trialPackage.SubscriptionPackageId == NaN || $scope.trialPackage.SubscriptionPackageId < 1)
            {
                $scope.trial = false;
                return;
            }
            
            $scope.initializeController();
            $scope.trial = true;
            $scope.setPackageCompleted($scope.trialPackage);
        };

        $scope.getSubscriptionPackages = function ()
        {
            $scope.initializeController();
            subscriptionServices.getGetPackages($scope.getSubscriptionPackagesCompleted);
        };

        $scope.getSubscriptionPackagesCompleted = function (data)
        {
            if (data.length < 1)
            {
                $scope.setError('SERVER ERROR: Subscription Packages could not be retrieved. Please try again.');
                return;
            }
            else
            {
                $scope.allPackages = data;
                angular.forEach($scope.allPackages, function (pack, c)
                {
                    //angular.forEach(pack.PackagePricings, function (o, q)
                    //{
                    //    o.BillingCycleName = '&#8358;' + o.Price + ' ' + o.BillingCycleName;
                    //});
                    
                    if (c.SubscriptionPackageId == 1 || c.PackageTitle == 'Trial')
                    {
                        $scope.trialPackage = c;
                    }
                    
                });
                
                subscriptionServices.getGenericList($scope.getGenericListCompleted);
            }
        };
        
        $scope.getGenericListCompleted = function (data)
        {
            $scope.genericList = {};
            $scope.genericList.currencies = data.Currencies;
            
            angular.forEach(data.PaymentMethods, function (v, c)
            {
                if (v.PaymentMethodId == 5 || v.Name == 'Not Applicable')
                {
                    $scope.notAppMeth = v;
                    data.PaymentMethods.splice(c, 1);
                }
            });
            
            $scope.genericList.paymentMethods = data.PaymentMethods;
            
            angular.forEach(data.BillingCycles, function (l, p)
            {
                if (l.BillingCycleId == 5 || l.Code == 'NA')
                {
                    $scope.notAppBillC = l;
                    data.BillingCycles.splice(p, 1);
                }
            });

            $scope.genericList.billingCycles = data.BillingCycles;
        };
        
        $scope.getCurrenciesCompleted = function (data)
        {
            $scope.genericList = {};
            $scope.genericList.currencies = data.Currencies;
        };

        $scope.processFile = function ()
        {
            $scope.progress = 0;
            fileReader.readAsDataUrl($scope.file, $scope)
            .then(function (result)
            {
                $scope.store.StoreLogoPath = result;
            });
            
            $upload.upload({
                url: "/Store/CreateFileSession",
                method: "POST",
                data: { file: $scope.file },
            }).success(function (data)
            {
                if (data.code < 1)
                {
                   alert('File could not be processed. Please try again later.');
               }
               
           });
        };
        
        $scope.updatealias = function (storeName)
        {
            $scope.store.StoreAlias = storeName.replace(/\s+/g, '');
        };

        $scope.setError = function (errorMessage)
        {
            $scope.error = errorMessage;
            $scope.negativefeedback = true;
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

        $scope.clearErrors = function ()
        {
            $scope.error = '';
            $scope.success = '';
            $scope.negativefeedback = false;
            $scope.positivefeedback = false;
        };

        $scope.printInvoice = function ()
        {
            var contents = $scope.invoice.html();
            var popupWin = '';
            if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)
            {
                popupWin = window.open('', '_blank', 'width=600,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
                popupWin.window.focus();
                popupWin.document.write('<!DOCTYPE html><html><head>' +
                    '<link rel="stylesheet" type="text/css" href="/Content/feedbackmessage.css" /><link href="/Content/bootstrap.css" rel="stylesheet" />' +
                    '</head><body onload="window.print()"><div class="reward-body">' + contents + '</div></html>');
                popupWin.onbeforeunload = function (event)
                {
                    popupWin.close();
                    return '.\n';
                };
                popupWin.onabort = function (event)
                {
                    popupWin.document.close();
                    popupWin.close();
                };
            }
            else
            {
                popupWin = window.open('', '_blank', 'width=800,height=600');
                popupWin.document.open();
                popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body onload="window.print()">' + contents + '</html>');
                popupWin.document.close();
            }
            popupWin.document.close();

            return true;

        };
        
    }]);

    
})