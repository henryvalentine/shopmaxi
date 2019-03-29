"use strict";

define(['application-configuration', 'accountsService', 'alertsService'], function (app)
{
    app.register.directive('ngMain', function () {
        return function($scope, $rootScope, ngMain, attrs)
        {
            $scope.getUserInfo();

            return {
                link: function (element, attr)
                {
                    element.href = element.href.replace('/Account', "");
                }
            };
        };
    });

    app.register.controller('defaultController', ['$scope', '$rootScope','accountsService', 'alertsService', function ($scope, $rootScope,accountsService, alertsService) {

        $scope.getUserInfo = accountsService.getUserName($scope.getUserInfoCompleted);
        
        $scope.getUserInfoCompleted = function (data)
        {
            $rootScope.userName = data;
        };

        //setTimeout(function () {
        //    window.location = "Master.html";
        //}, 10);
    }]);
});


