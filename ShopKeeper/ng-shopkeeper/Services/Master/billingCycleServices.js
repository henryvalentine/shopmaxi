define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('billingCycleServices', ['ajaxService', function (ajaxService)
    {
        this.addBillingCycle = function (billingCycle, callbackFunction)
        {
            var dxcv = JSON.stringify({ billingCycle: billingCycle });
            return ajaxService.AjaxPost(dxcv, "/BillingCycle/AddBillingCycle", callbackFunction);
        };
        
        this.getBillingCycle = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/BillingCycle/GetBillingCycle?id=" + id, callbackFunction);
        };

        this.editBillingCycle = function (billingCycle, callbackFunction)
        {
            var dxcv = JSON.stringify({ billingCycle: billingCycle });
            return ajaxService.AjaxPost(dxcv, "/BillingCycle/EditBillingCycle", callbackFunction);
        };

        this.deleteBillingCycle = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/BillingCycle/DeleteBillingCycle?id=" + id, callbackFunction);
        };

    }]);
});