define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('storeCountryServices', ['ajaxService', function (ajaxService)
    {
        this.addCountry = function (country, callbackFunction)
        {
            var dxcv = JSON.stringify({ country: country });
            return ajaxService.AjaxPost(dxcv, "/StoreCountry/AddCountry", callbackFunction);
        };
        
        this.getCountry = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreCountry/GetCountry?id=" + id, callbackFunction);
        };

        this.editCountry = function (country, callbackFunction)
        {
            var dxcv = JSON.stringify({ country: country });
            return ajaxService.AjaxPost(dxcv, "/StoreCountry/EditCountry", callbackFunction);
        };

        this.deleteCountry = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreCountry/DeleteCountry?id=" + id, callbackFunction);
        };

    }]);
});