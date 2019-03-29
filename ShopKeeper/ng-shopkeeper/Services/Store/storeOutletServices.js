define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('storeOutletServices', ['ajaxService', function (ajaxService)
    {
        this.addStoreOutlet = function (storeOutlet, callbackFunction) 
        {
            return ajaxService.AjaxPost({ storeOutlet: storeOutlet }, "/StoreOutlet/AddStoreOutlet", callbackFunction);
        };
        
        this.getStoreOutlet = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreOutlet/GetMainOutlet", callbackFunction);
        };
        
        this.getCities = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreOutlet/GetCities", callbackFunction);
        };

        this.editStoreOutlet = function (storeOutlet, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeOutlet: storeOutlet }, "/StoreOutlet/EditStoreOutlet", callbackFunction);
        };

        this.deleteStoreOutlet = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreOutlet/DeleteStoreOutlet?id=" + id, callbackFunction);
        };

        this.getCurrencies = function (callbackFunction) {
            return ajaxService.AjaxGet("/StoreOutlet/GetCurrencies", callbackFunction);
        };
    }]);
});