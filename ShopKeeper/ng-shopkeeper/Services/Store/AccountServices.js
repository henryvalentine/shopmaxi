define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('accountService', ['ajaxService', function (ajaxService)
    {
        this.registerUser = function (user, successFunction, errorFunction)
        {
            ajaxService.AjaxPostWithNoAuthenication(user, "/api/accounts/RegisterUser", successFunction, errorFunction);
        };

        this.login = function (user, successFunction, errorFunction)
        {
            ajaxService.AjaxPostWithNoAuthenication(user, "/api/accounts/Login", successFunction, errorFunction);
        };

        this.getStoreSetting = function (successFunction)
        {
            ajaxService.AjaxGetWithoutAuthentication("/Account/GetStoreSetting", successFunction);
        };
        
        this.getSignedOnUser = function (successFunction)
        {
            ajaxService.AjaxGet("/Account/GetSignedOnUser", successFunction);
        };
        
        this.authenticateUser = function (successFunction)
        {
            ajaxService.AjaxGet("/Account/AuthenticateUser", successFunction);
        };

        this.updateUser = function (user, successFunction, errorFunction)
        {
            ajaxService.AjaxPost(user, "/api/accounts/UpdateUser", successFunction, errorFunction);
        };

    }]);
});
