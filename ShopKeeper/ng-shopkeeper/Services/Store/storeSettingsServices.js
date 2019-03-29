define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('storeSettingsServices', ['ajaxService', function (ajaxService)
    {
        this.getStoreSettings = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreOutlet/GetStoreOutlet?id=" + id, callbackFunction);
        };
        
        this.getCities = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreOutlet/GetCities", callbackFunction);
        };

        this.editStoreOutlet = function (storeOutlet, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeOutlet: storeOutlet }, "/StoreOutlet/EditStoreOutlet", callbackFunction);
        };


    }]);
});