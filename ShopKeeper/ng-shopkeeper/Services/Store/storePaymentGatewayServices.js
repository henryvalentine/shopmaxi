define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('storePaymentGatewayServices', ['ajaxService', function (ajaxService)
    {
        this.addStorePaymentGateway = function (paymentGateway, callbackFunction)
        {
            return ajaxService.AjaxPost({ storePaymentGateway: paymentGateway }, "/StorePaymentGateway/AddStorePaymentGateway", callbackFunction);
        };

        this.getStorePaymentGateway = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StorePaymentGateway/GetStorePaymentGateway?id=" + id, callbackFunction);
        };

        this.editStorePaymentGateway = function (paymentGateway, callbackFunction)
        {
            return ajaxService.AjaxPost({ storePaymentGateway: paymentGateway }, "/StorePaymentGateway/EditStorePaymentGateway", callbackFunction);
        };

        this.deleteStorePaymentGateway = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StorePaymentGateway/DeleteStorePaymentGateway?id=" + id, callbackFunction);
        };
    

    }]);
});