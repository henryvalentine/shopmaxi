"use strict";

define(['application-configuration', 'imageViewServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngImageViewTable', function ($compile)
    {
        return function ($scope, ngImageViewTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/ImageView/GetImageViewObjects";
                tableOptions.itemId = 'ImageViewId';
                tableOptions.columnHeaders = ['Name'];
                var ttc = tableManager($scope, $compile, ngImageViewTable, tableOptions, 'New Image View', 'prepareImageViewTemplate', 'getImageView', 'deleteImageView', 139);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });

    app.register.controller('imageViewController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'imageViewServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, imageViewServices, alertsService)
    {
        $rootScope.applicationModule = "ImageView";
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
            $scope.imageView = new Object();
            $scope.imageView.ImageViewId = 0;
            $scope.imageView.Name = '';
            $scope.imageView.Header = 'New Image View';
        };
        
        $scope.prepareImageViewTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ImageViews/ProcessImageViews.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processImageView = function ()
        {
            var imageView = new Object();
            imageView.Name = $scope.imageView.Name;
            if (imageView.Name == undefined || imageView.Name.length < 1)
            {
                alert("ERROR: Please provide Image View Name. ");
                return;
            }

            if ($scope.imageView.ImageViewId < 1)
            {
                imageViewServices.addImageView(imageView, $scope.processImageViewCompleted);
            }
            else
            {
                imageViewServices.editImageView(imageView, $scope.processImageViewCompleted);
            }

        };
        
        $scope.processImageViewCompleted = function (data)
        {
            if (data.ImageViewId < 1)
            {
                   alert(data.Name);

               }
            else
            {

                if ($scope.imageView.ImageViewId < 1)
                {
                       alert("Image View information was successfully added.");
                   } else {
                       alert("Image View information was successfully updated.");
                   }
                   ngDialog.close('/ng-shopkeeper/Views/Store/ImageViews/ProcessImageViews.html', '');
                   $scope.jtable.fnClearTable();
                   $scope.initializeController();
               }
           };

        $scope.getImageView = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            imageViewServices.getImageView(id, $scope.getImageViewCompleted);
        };
        
        $scope.getImageViewCompleted = function (data)
        {
            if (data.ImageViewId < 1)
            {
                alert("ERROR: Image View information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.imageView = data;
                $scope.imageView.Header = 'Update Image View Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/ImageViews/ProcessImageViews.html',
                    className: 'ngdialog-theme-flat',
                    //controller: 'manageImageViewCntroller',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteImageView = function (id)
        {
            if (parseInt(id) > 0) {
                if (!confirm("This Image View information will be deleted permanently. Continue?")) {
                    return;
                }
                imageViewServices.deleteImageView(id, $scope.deleteImageViewCompleted);
            } else {
                alert('Invalid selection.');
            }
        };

        $scope.deleteImageViewCompleted = function (data)
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