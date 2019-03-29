"use strict";

define(['application-configuration', 'storeTransactionTypeServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngTransactionTypeTable', function ($compile)
    {
        return function ($scope, ngTransactionTypeTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreTransactionType/GetStoreTransactionTypeObjects";
                tableOptions.itemId = 'TransactionTypeId';
                tableOptions.columnHeaders = ['Name', 'Action'];
                var ttc = tableManager($scope, $compile, ngTransactionTypeTable, tableOptions, 'New Transaction Type', 'prepareTransactionTypeTemplate', 'getTransactionType', 'deleteTransactionType', 178);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.ttc = ttc;
            }
        };
    });
    
    app.register.controller('storeTransactionTypeController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeTransactionTypeServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeTransactionTypeServices, $upload, fileReader, alertsService)
    {
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.transactionType = new Object();
            var sb = $scope.transactionType;
            sb.TransactionTypeId = 0;
            sb.Name = '';
            sb.Action = '';
            sb.Description = '';
            sb.Header = 'Create New Transaction Type';
        };
        
        $scope.prepareTransactionTypeTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/TransactionTypes/ProcessTransactionTypes.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processTransactionType = function ()
        {
            var transactionType = new Object();
            transactionType.Description = $scope.transactionType.Description;
            transactionType.Name = $scope.transactionType.Name;
            transactionType.Action = $scope.transactionType.Action;
            
            if (transactionType.Name == undefined || transactionType.Name.length < 1 || transactionType.Name == null)
            {
                alert("ERROR: Please provide Transaction Type Name. ");
                return;
            }
            
            if (transactionType.Action == undefined || transactionType.Action.length < 1 || transactionType.Action == null)
            {
                alert("ERROR: Please provide the Action for this Transaction. ");
                return;
            }

            if ($scope.transactionType.TransactionTypeId == 0 || $scope.transactionType.TransactionTypeId < 1 || $scope.transactionType.TransactionTypeId == undefined)
            {
                storeTransactionTypeServices.addTransactionType(transactionType, $scope.processTransactionTypeCompleted);
            }
            
            else
            {
                storeTransactionTypeServices.editTransactionType(transactionType, $scope.processTransactionTypeCompleted);
            }
        };

        $scope.processTransactionTypeCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);
            }
            else
            {
                if ($scope.transactionType.TransactionTypeId < 1)
                {
                    alert("Transaction Type information was successfully added.");
                }
                else
                {
                    alert("Transaction Type information was successfully updated.");
                }

                ngDialog.close('/ng-shopkeeper/Views/Store/TransactionTypes/ProcessTransactionTypes.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };

        $scope.getTransactionType = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            storeTransactionTypeServices.getTransactionType(id, $scope.getTransactionTypeCompleted);
        };
       
        $scope.getTransactionTypeCompleted = function (data)
        {
            if (data.TransactionTypeId < 1)
            {
               alert("ERROR: Transaction Type information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                $scope.transactionType = data;
                if (data.SampleImagePath != null)
                {
                    $scope.transactionType.SampleImagePath = data.SampleImagePath.replace('~', '');
                }
                else
                {
                    $scope.transactionType.SampleImagePath = '/Content/images/noImage.png';
                }
                $scope.transactionType.Header = 'Update Transaction Type Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/TransactionTypes/ProcessTransactionTypes.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
         };

        $scope.deleteTransactionType = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Transaction Type information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storeTransactionTypeServices.deleteTransactionType(id, $scope.deleteTransactionTypeCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteTransactionTypeCompleted = function (data)
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
                              $scope.transactionType.SampleImagePath = result;
                          });
            
            $upload.upload({
                url: "/TransactionType/CreateFileSession",
                method: "POST",
                data: { file: $scope.file },
            })
           .progress(function (evt)
           {
               $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
           }).success(function (data)
           {
               if (data.code < 1)
               {
                   alert('File could not be processed. Please try again later.');
               }
               
           });
        };
        
    }]);
    
});



