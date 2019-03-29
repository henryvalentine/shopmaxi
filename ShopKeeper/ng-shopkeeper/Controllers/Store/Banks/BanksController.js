"use strict";

define(['application-configuration', 'storeBankServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('bankTable', function ($compile)
    {
        return function ($scope, bankTable) {
            var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreBank/GetBankObjects";
                tableOptions.itemId = 'BankId';
                tableOptions.columnHeaders = ['FullName', 'ShortName', 'SortCode'];
                var ttc = tableManager($scope, $compile, bankTable, tableOptions, 'New Bank', 'prepareBankTemplate', 'getBank', 'deleteBank', 100);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.ttc = ttc;
            }
        };
    });
    app.register.directive("ngBankImgSelect", function ()
    {

        return {
            link: function($scope, el) {
                el.bind("change", function (e)
                {
                    $scope.file = (e.srcElement || e.target).files[0];
                    $scope.processFile();
                });
            }
        };
    });

    app.register.controller('storeBankController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeBankServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeBankServices, $upload, fileReader, alertsService)
    {
        //alert($scope);
        //$rootScope.applicationModule = "Bank";
        

        $scope.getAuthStatus = function ()
        {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };


        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.selectedBank = new Object();
            var sb = $scope.selectedBank;
            sb.BankId = 0;
            sb.SortCode = '';
            sb.ShortName = '';
            sb.FullName = '';
            sb.Header = 'Create New Bank';
            sb.filePath = '/Content/images/noImage.png';
        };
        
        $scope.prepareBankTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Banks/ProcessBanks.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processBank = function ()
        {
            var bank = new Object();
            bank.SortCode = $scope.selectedBank.SortCode;
            bank.ShortName = $scope.selectedBank.ShortName;
            bank.FullName = $scope.selectedBank.FullName;
            if (bank.FullName == undefined || bank.FullName.length < 1)
            {
                alert("ERROR: Please provide Bank full name. ");
                return;
            }
            if (bank.SortCode == undefined || bank.SortCode.length < 1)
            {
                alert("ERROR: Please provide Bank full name. ");
                return;
            }
            
            if ($scope.selectedBank.BankId < 1)
            {
                storeBankServices.addBank(bank, $scope.processBankCompleted);
            }
            else
            {
                storeBankServices.editBank(bank, $scope.processBankCompleted);
            }
        };

        $scope.processBankCompleted = function (data)
        {
            if (data.BankId < 1)
            {
                alert(data.FullName);

            }
            else
            {
                if ($scope.selectedBank.BankId < 1)
                {
                    alert("Bank information was successfully added.");
                }
                else
                {
                    alert("Bank information was successfully updated.");
                }

                ngDialog.close('/ng-shopkeeper/Views/Store/Banks/ProcessBanks.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };

        $scope.getBank = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeBankServices.getBank(id, $scope.getBankCompleted);
        };
       
        $scope.getBankCompleted = function (data)
        {
            if (data.BankId < 1)
            {
               alert("ERROR: Bank information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                $scope.selectedBank = data;
                if (data.LogoPath != null)
                {
                    $scope.selectedBank.filePath = data.LogoPath.replace('~', '');
                } else {
                    $scope.selectedBank.filePath = '/Content/images/noImage.png';
                }
                $scope.selectedBank.Header = 'Update Bank Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Banks/ProcessBanks.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
         };

        $scope.deleteBank = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Bank information will be deleted permanently. Continue?")) {
                    return;
                }
                storeBankServices.deleteBank(id, $scope.deleteBankCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteBankCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.ttc.fnClearTable();
                alert(data.Error);
            }
        };

        $scope.processFile = function ()
        {
            $scope.progress = 0;
            fileReader.readAsDataUrl($scope.file, $scope)
                          .then(function (result)
                          {
                              $scope.selectedBank.filePath = result;
                          });
            
            $upload.upload({
                url: "/StoreBank/CreateFileSession",
                method: "POST",
                data: { file: $scope.file },
            })
           .progress(function (evt) {
               $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
           }).success(function (data) {
               if (data.code < 1) {
                   alert('File could not be processed. Please try again later.');
               }
               //else {
               //    $scope.filePath = data.Error.replace('~', '');
               //}
           });
        };

        //$scope.$on("progress", function (e, progress)
        //{
        //    $scope.progress = progress.loaded/progress.total;
        //});


    }]);
    
});



