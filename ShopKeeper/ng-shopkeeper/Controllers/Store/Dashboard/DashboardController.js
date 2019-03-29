"use strict";

define(['application-configuration', 'dashboardServices'], function (app)
{

    app.register.directive('ngDashboard', function ($compile)
    {
       
    });

    app.register.controller('dashboardController', ['$scope', '$rootScope', '$routeParams', '$timeout', 'dashboardServices',
    function ($scope, $rootScope, $routeParams, $timeout, dashboardServices)
    {
        var timeOut = function ()
        {
            if ($rootScope.isAdmin !== undefined || $rootScope.isAdmin !== null)
            {
                if ($rootScope.isAdmin === true)
                {
                    dashboardServices.getReportSnapshots($rootScope.getReportSnapshotsCompleted);
                    $timeout.cancel(timeOut);
                }
                else
                {
                    if ($rootScope.isMarketer === true)
                    {
                        dashboardServices.getReportSnapshots($rootScope.getReportSnapshotsCompleted);
                        $timeout.cancel(timeOut);
                    }
                    else
                    {
                        $timeout.cancel(timeOut);
                    }
                    
                }
            }

        };
            
        $timeout(timeOut, 2000);
        

    }]);

});