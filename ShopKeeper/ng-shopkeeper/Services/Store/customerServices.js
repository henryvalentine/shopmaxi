define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('customerServices', ['ajaxService', function (ajaxService)
    {
        this.addCustomer = function (customer, callbackFunction)
        {
            var dxcv = JSON.stringify({ customer: customer });
            return ajaxService.AjaxPost(dxcv, "/Customer/AddCustomer", callbackFunction);
        };

        this.editCustomer = function (customer, callbackFunction)
        {
            var dxcv = JSON.stringify({ customer: customer });
            return ajaxService.AjaxPost(dxcv, "/Customer/EditCustomer", callbackFunction);
        };
        
        this.getCustomer = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Customer/GetCustomer?id=" + id, callbackFunction);
        };

        this.getOutlets = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreOutlet/GetOutlets", callbackFunction);
        };
        
        this.getStates = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Customer/GetStates?countryId=" + id, callbackFunction);
        };
        
        this.getSelectables = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Customer/GetListObjects", callbackFunction);
        };
        
        this.deleteCustomer = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Customer/DeleteCustomer?id=" + id, callbackFunction);
        };

    }]);
});