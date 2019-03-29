"use strict";

define(['application-configuration', 'issueTypeServices', 'ngDialog'], function (app)
{

    app.register.directive('ngIssueTypeTable', function ($compile)
    {
        return function ($scope, ngIssueTypeTable)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/IssueType/GetIssueTypeObjects";
            tableOptions.itemId = 'IssueTypeId';
            tableOptions.columnHeaders = ['Name'];
            var ttc = tableManager($scope, $compile, ngIssueTypeTable, tableOptions, 'New Issue Type', 'prepareIssueTypeTemplate', 'getIssueType', 'deleteIssueType', 139);
            ttc.removeAttr('width').attr('width', 'auto');
            $scope.jtable = ttc;
        };
    });

    app.register.controller('issueTypeController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'issueTypeServices',
    function (ngDialog, $scope, $rootScope, $routeParams, issueTypeServices)
    {
        $scope.initializeController = function ()
        {
            $scope.issueType = new Object();
            $scope.issueType.IssueTypeId = 0;
            $scope.issueType.Name = '';
            $scope.issueType.Header = 'New Issue Type';
        };
        
        $scope.prepareIssueTypeTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/IssueTypes/ProcessIssueTypes.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processIssueType = function ()
        {
            var issueType = new Object();
            issueType.Name = $scope.issueType.Name;
            if (issueType.Name == undefined || issueType.Name.length < 1)
            {
                alert("ERROR: Please provide Issue Type Name. ");
                return;
            }

            if ($scope.issueType.IssueTypeId < 1)
            {
                issueTypeServices.addIssueType(issueType, $scope.processIssueTypeCompleted);
            }
            else
            {
                issueTypeServices.editIssueType(issueType, $scope.processIssueTypeCompleted);
            }

        };
        
        $scope.processIssueTypeCompleted = function (data)
        {
            if (data.IssueTypeId < 1)
            {
                   alert(data.Name);

               }
            else
            {

                if ($scope.issueType.IssueTypeId < 1)
                {
                       alert("Issue Type information was successfully added.");
                   } else {
                       alert("Issue Type information was successfully updated.");
                   }
                   ngDialog.close('/ng-shopkeeper/Views/Store/IssueTypes/ProcessIssueTypes.html', '');
                   $scope.jtable.fnClearTable();
                   $scope.initializeController();
               }
           };

        $scope.getIssueType = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            issueTypeServices.getIssueType(id, $scope.getIssueTypeCompleted);
        };
        
        $scope.getIssueTypeCompleted = function (data)
        {
            if (data.IssueTypeId < 1)
            {
                alert("ERROR: Issue Type information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.issueType = data;
                $scope.issueType.Header = 'Update Issue Type Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/IssueTypes/ProcessIssueTypes.html',
                    className: 'ngdialog-theme-flat',
                    //controller: 'manageIssueTypeCntroller',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteIssueType = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Issue Type information will be deleted permanently. Continue?")) {
                    return;
                }
                issueTypeServices.deleteIssueType(id, $scope.deleteIssueTypeCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteIssueTypeCompleted = function (data)
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