define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('subscriptionServices', ['ajaxService', function (ajaxService)
    {
        this.subscribe = function (subscription, callbackFunction)
        {
            return ajaxService.AjaxPost({ store: subscription }, "/Store/Subscribe", callbackFunction);
        };
        
        this.regAccount = function (regSubAccount, callbackFunction)
        {
            return ajaxService.AjaxPost({ registerModel: regSubAccount }, "/Account/RegisterSubscriptionAccount", callbackFunction);
        };
        
        this.computeHash = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Account/ComputeHash", callbackFunction);
        };
        
        var res = {};
        
        this.getresult = function ()
        {
            return res;
        };

        this.setresult = function (result)
        {
            return res = result;
        };

        this.getGetPackages = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Store/GetPackages", callbackFunction);
        };
        
        this.getGenericList = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Store/GetListObjects", callbackFunction);
        };
        
        this.getPackage = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Store/GetPackage?id=" + id, callbackFunction);
        };

        this.getCurrencies = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Store/Currencies()", callbackFunction);
        };

        this.modifySubscription = function (subscription, callbackFunction)
        {
            return ajaxService.AjaxPost({ subscriptionPackage: subscriptionPackage }, "/Store/EditSubscriptionPackage", callbackFunction);
        };
        
    }]);
});