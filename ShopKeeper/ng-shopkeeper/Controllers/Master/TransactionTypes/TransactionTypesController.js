"use strict";

define(['application-configuration', 'transactionTypeServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngTransactionTypeTable', function ($compile)
    {
        return function ($scope, ngTransactionTypeTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/TransactionType/GetTransactionTypeObjects";
            tableOptions.itemId = 'TransactionTypeId';
            tableOptions.columnHeaders = ['Name', 'Action'];
            var ttc = tableManager($scope, $compile, ngTransactionTypeTable, tableOptions, 'New Transaction Type', 'prepareTransactionTypeTemplate', 'getTransactionType', 'deleteTransactionType', 178);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.ttc = ttc;
        };
    });
    
    app.register.controller('transactionTypeController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'transactionTypeServices', '$upload', 'fileReader', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, transactionTypeServices, $upload, fileReader, alertsService)
    {
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
                template: '/ng-shopkeeper/Views/Master/TransactionTypes/ProcessTransactionTypes.html',
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
                transactionTypeServices.addTransactionType(transactionType, $scope.processTransactionTypeCompleted);
            }
            
            else
            {
                transactionTypeServices.editTransactionType(transactionType, $scope.processTransactionTypeCompleted);
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

                ngDialog.close('/ng-shopkeeper/Views/Master/TransactionTypes/ProcessTransactionTypes.html', '');
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

            transactionTypeServices.getTransactionType(id, $scope.getTransactionTypeCompleted);
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
                    template: '/ng-shopkeeper/Views/Master/TransactionTypes/ProcessTransactionTypes.html',
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
                transactionTypeServices.deleteTransactionType(id, $scope.deleteTransactionTypeCompleted);
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



