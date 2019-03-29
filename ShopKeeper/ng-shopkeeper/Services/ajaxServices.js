
define(['application-configuration'], function (app) {

    app.register.service('ajaxService', ['$http', '$location', '$rootScope', function ($http, $location, $rootScope)
    {

        function setUIBusy()
        {
            angular.element('.sowBusy').fadeIn('fast');
        };

        function stopUIBusy()
        {
            angular.element('.sowBusy').fadeOut('fast');
        };

        this.AjaxPost = function (data, route, callbackFunction)
        {
            setUIBusy();
            setTimeout(function ()
            {
                $http.post(route, data).success(function (response)
                {
                    stopUIBusy();
                    callbackFunction(response, status);
                });
            }, 1000);
        };
        
        this.AjaxDelete = function (route, callbackFunction)
        {
            setUIBusy();
            setTimeout(function ()
            {
                $http.post(route).success(function (response)
                {
                    stopUIBusy();
                    callbackFunction(response, status);
                });
            }, 1000);
            
        };

        this.AjaxDeleteWithPromise = function (route)
        {
            setUIBusy();
            setTimeout(function ()
            {
                return $http.post(route, data);

            }, 1000);

        };

        this.AjaxPostWithNoAuthenication = function(data, route, callbackFunction, errorFunction) {
            blockUI.start();
            setTimeout(function() {
                $http.post(route, data).success(function(response, status, headers, config) {
                    blockUI.stop();
                    callbackFunction(response, status);
                }).error(function(response) {
                    blockUI.stop();
                    errorFunction(response);
                });
            }, 1000);

        };
        
        this.AjaxGetWithoutAuthentication = function (route, callbackFunction)
        {
            setTimeout(function () {
                $http({ method: 'GET', url: route }).success(function (response)
                {
                    callbackFunction(response);
                });
            }, 1000);
        };
        
        this.AjaxGet = function (route, callbackFunction)
        {
            setTimeout(function () {
                setUIBusy();
                $http({ method: 'GET', url: route }).success(function (response) {
                    stopUIBusy();
                    callbackFunction(response);
                });
            }, 1000);
        };

        this.checkAuthentication = function(route, callbackFunction)
        {
            $http({ method: 'GET', url: "/Account/GetSignedOnUser" }).
                success(function (data)
                {
                    if (data.IsAuthenticated == false) {
                        alert('Your session has timed out!');
                        $rootScope.redirectUrl = $location.path();
                        $location.path = "/ngy.html#signIn";
                    } else {
                        setTimeout(function ()
                        {
                            $http({ method: 'GET', url: route }).success(function (response)
                            {
                                callbackFunction(response);
                            });
                        }, 1000);
                    }
            });
        };
        
        this.checkAuthenticationOnly = function ()
        {
           return $http({ method: 'GET', url: "/Account/GetSignedOnUser" }).
                success(function (data)
                {
                    if (data.IsAuthenticated == false) {
                        return false;
                    } else {
                        return true;
                    }
                });
        };

        
        this.AjaxDownload = function (route)
        {
            setTimeout(function ()
            {
                $http({ method: 'GET', url: route });
            }, 1000);

        };

        this.AjaxGetWithData = function (data, route, callbackFunction)
        {
            setTimeout(function ()
            {
                $http({ method: 'GET', url: route, params: data }).success(function (response, status, headers, config)
                {
                    callbackFunction(response, status);
                });
            }, 1000);

        };


        this.AjaxGetWithNoBlock = function(data, route, callbackFunction, errorFunction) {
            setTimeout(function() {
                $http({ method: 'GET', url: route, params: data }).success(function(response, status, headers, config) {
                    callbackFunction(response, status);
                }).error(function(response) {
                    ;
                    if (response.IsAuthenicated == false) {
                        window.location = "/index.html";
                    }
                    errorFunction(response);
                });
            }, 0);

        };

    }]);
});


