define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('currencyServices', ['ajaxService', function (ajaxService) {


        this.addCurrency = function(currency, callbackFunction)
        {
            var dxcv = JSON.stringify({ currency: currency });
            return ajaxService.AjaxPost(dxcv, "/Currency/AddCurrency", callbackFunction);
        };
        
        this.getCurrency = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Currency/GetCurrency?id=" + id, callbackFunction);
        };
        
        this.getCountries = function(callbackFunction) 
        {
            return ajaxService.AjaxGet("/Currency/GetCountries", callbackFunction);
        };

        this.editCurrency = function (currency, callbackFunction)
        {
            var dxcv = JSON.stringify({ currency: currency });
            return ajaxService.AjaxPost(dxcv, "/Currency/EditCurrency", callbackFunction);
        };

        this.deleteCurrency = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Currency/DeleteCurrency?id=" + id, callbackFunction);
        };
    }]);
});