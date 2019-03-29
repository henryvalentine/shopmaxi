"use strict";

define(['application-configuration', 'documentTypeServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngDocumentTypeTable', function ($compile)
    {
        return function ($scope, ngDocumentTypeTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/DocumentType/GetDocumentTypeObjects";
                tableOptions.itemId = 'DocumentTypeId';
                tableOptions.columnHeaders = ['TypeName'];
                var ttc = tableManager($scope, $compile, ngDocumentTypeTable, tableOptions, 'New Document Type', 'prepareDocumentTypeTemplate', 'getDocumentType', 'deleteDocumentType', 167);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('documentTypeController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'documentTypeServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, documentTypeServices, alertsService)
    {
        $rootScope.applicationModule = "documentType";
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.alerts = [];
        $scope.initializeController = function ()
        {
            $scope.documentType = new Object();
            $scope.documentType.DocumentTypeId = 0;
            $scope.documentType.TypeName = '';
            $scope.documentType.Description = '';
            $scope.documentType.Header = 'Create New Document Type';
        };
        
        $scope.prepareDocumentTypeTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/DocumentTypes/ProcessDocumentTypes.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processDocumentType = function ()
        {
            var documentType = new Object();
            documentType.TypeName = $scope.documentType.TypeName;
            documentType.Description = $scope.documentType.Description;
            
            if (documentType.TypeName == undefined || documentType.TypeName.length < 1)
            {
                alert("ERROR: Please provide Document Type Name. ");
                return;
            }

            if ($scope.documentType.DocumentTypeId < 1)
            {
                documentTypeServices.addDocumentType(documentType, $scope.processDocumentTypeCompleted);
            }
            else
            {
                documentTypeServices.editDocumentType(documentType, $scope.processDocumentTypeCompleted);
            }

        };
        
        $scope.processDocumentTypeCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);

             }
            else
            {

                if ($scope.documentType.DocumentTypeId < 1)
                {
                    alert("Document Type information was successfully added.");
                }
                else
                {
                    alert("Document Type information was successfully updated.");
                }
                
                ngDialog.close('/ng-shopkeeper/Views/Store/DocumentTypes/ProcessDocumentTypes.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
               }
           };

        $scope.getDocumentType = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            documentTypeServices.getDocumentType(id, $scope.getDocumentTypeCompleted);
        };
        
        $scope.getDocumentTypeCompleted = function (data)
        {
            if (data.DocumentTypeId < 1)
            {
                alert("ERROR: Document Type information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.documentType = data;
                $scope.documentType.Header = 'Update Document Type Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/DocumentTypes/ProcessDocumentTypes.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteDocumentType = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Document Type information will be deleted permanently. Continue?"))
                {
                    return;
                }
                documentTypeServices.deleteDocumentType(id, $scope.deleteDocumentTypeCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteDocumentTypeCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.jtable.fnClearTable();
                alert(data.Error);
            }
        };
    }]);

});