define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('paymentGatewayServices', ['ajaxService', function (ajaxService)
    {
        this.addPaymentGateway = function (paymentGateway, callbackFunction)
        {
            return ajaxService.AjaxPost({ paymentGateway: paymentGateway }, "/PaymentGateway/AddPaymentGateway", callbackFunction);
        };

        this.getPaymentGateway = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/PaymentGateway/GetPaymentGateway?id=" + id, callbackFunction);
        };

        this.editPaymentGateway = function (paymentGateway, callbackFunction)
        {
            return ajaxService.AjaxPost({ paymentGateway: paymentGateway }, "/PaymentGateway/EditPaymentGateway", callbackFunction);
        };

        this.deletePaymentGateway = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/PaymentGateway/DeletePaymentGateway?id=" + id, callbackFunction);
        };
    

    }]);
});