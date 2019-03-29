define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('deliveryMethodServices', ['ajaxService', function (ajaxService)
    {
        this.addDeliveryMethod = function (deliveryMethod, callbackFunction)
        {
            return ajaxService.AjaxPost({ deliveryMethod: deliveryMethod }, "/DeliveryMethod/AddDeliveryMethod", callbackFunction);
        };
        
        this.getDeliveryMethod = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/DeliveryMethod/GetDeliveryMethod?id=" + id, callbackFunction);
        };

        this.editDeliveryMethod = function (deliveryMethod, callbackFunction)
        {
            return ajaxService.AjaxPost({ deliveryMethod: deliveryMethod }, "/DeliveryMethod/EditDeliveryMethod", callbackFunction);
        };

        this.deleteDeliveryMethod = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/DeliveryMethod/DeleteDeliveryMethod?id=" + id, callbackFunction);
        };

    }]);
});