define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('countryServices', ['ajaxService', function (ajaxService)
    {
        this.addCountry = function (country, callbackFunction)
        {
            var dxcv = JSON.stringify({ country: country });
            return ajaxService.AjaxPost(dxcv, "/Country/AddCountry", callbackFunction);
        };
        
        this.getCountry = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Country/GetCountry?id=" + id, callbackFunction);
        };

        this.editCountry = function (country, callbackFunction)
        {
            var dxcv = JSON.stringify({ country: country });
            return ajaxService.AjaxPost(dxcv, "/Country/EditCountry", callbackFunction);
        };

        this.deleteCountry = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Country/DeleteCountry?id=" + id, callbackFunction);
        };

    }]);
});