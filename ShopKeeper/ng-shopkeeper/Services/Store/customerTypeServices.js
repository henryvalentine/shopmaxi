define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('customerTypeServices', ['ajaxService', function (ajaxService)
    {
        this.addCustomerType = function (customerType, callbackFunction)
        {
            return ajaxService.AjaxPost({ customerType: customerType }, "/StoreCustomerType/AddStoreCustomerType", callbackFunction);
        };
        
        this.getCustomerType = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreCustomerType/GetStoreCustomerType?id=" + id, callbackFunction);
        };

        this.editCustomerType = function (customerType, callbackFunction)
        {
            return ajaxService.AjaxPost({ customerType: customerType }, "/StoreCustomerType/EditStoreCustomerType", callbackFunction);
        };

        this.deleteCustomerType = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreCustomerType/DeleteStoreCustomerType?id=" + id, callbackFunction);
        };

    }]);
});