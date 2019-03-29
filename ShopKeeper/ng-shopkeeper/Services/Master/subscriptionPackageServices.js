define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('subscriptionPackageServices', ['ajaxService', function (ajaxService)
    {
        this.addSubscriptionPackage = function (subscriptionPackage, callbackFunction)
        {
            return ajaxService.AjaxPost({ subscriptionPackage: subscriptionPackage }, "/SubscriptionPackage/AddSubscriptionPackage", callbackFunction);
        };
        
        this.getSubscriptionPackage = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/SubscriptionPackage/GetSubscriptionPackage?id=" + id, callbackFunction);
        };

        this.editSubscriptionPackage = function (subscriptionPackage, callbackFunction)
        {
            return ajaxService.AjaxPost({ subscriptionPackage: subscriptionPackage }, "/SubscriptionPackage/EditSubscriptionPackage", callbackFunction);
        };

        this.deleteSubscriptionPackage = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/SubscriptionPackage/DeleteSubscriptionPackage?id=" + id, callbackFunction);
        };

    }]);
});