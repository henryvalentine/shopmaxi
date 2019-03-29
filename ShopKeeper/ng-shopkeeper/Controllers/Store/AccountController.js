"use strict";

define(['application-configuration', 'accountService'], function (app)
{
    
    app.register.controller('accountController', ['$scope', '$rootScope', '$routeParams', 'accountService',
    function ($scope, $rootScope, $routeParams, accountService)
    {
        $rootScope.applicationModule = "Account";
        
        
    }]);

});