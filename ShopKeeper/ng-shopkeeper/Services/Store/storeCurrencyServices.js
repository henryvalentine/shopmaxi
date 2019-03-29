define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('storeCurrencyServices', ['ajaxService', function (ajaxService) {


        this.addCurrency = function(currency, callbackFunction)
        {
            var dxcv = JSON.stringify({ storeCurrency: currency });
            return ajaxService.AjaxPost(dxcv, "/StoreCurrency/AddStoreCurrency", callbackFunction);
        };
        
        this.getCurrency = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreCurrency/GetStoreCurrency?id=" + id, callbackFunction);
        };
        
        this.getCountries = function(callbackFunction) 
        {
            return ajaxService.AjaxGet("/StoreCurrency/GetCountries", callbackFunction);
        };

        this.editCurrency = function (currency, callbackFunction)
        {
            var dxcv = JSON.stringify({ storeCurrency: currency });
            return ajaxService.AjaxPost(dxcv, "/StoreCurrency/EditStoreCurrency", callbackFunction);
        };

        this.deleteCurrency = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreCurrency/DeleteStoreCurrency?id=" + id, callbackFunction);
        };
    }]);
});