define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('storeCityServices', ['ajaxService', function (ajaxService) {


        this.addCity = function(city, callbackFunction)
        {
            var dxcv = JSON.stringify({ city: city });
            return ajaxService.AjaxPost(dxcv, "/StoreCity/AddCity", callbackFunction);
        };
        
        this.getCity = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreCity/GetCity?id=" + id, callbackFunction);
        };
        
        this.getStates = function(callbackFunction) 
        {
            return ajaxService.AjaxGet("/StoreCity/GetStates", callbackFunction);
        };

        this.editCity = function (city, callbackFunction)
        {
            var dxcv = JSON.stringify({ city: city });
            return ajaxService.AjaxPost(dxcv, "/StoreCity/EditCity", callbackFunction);
        };

        this.deleteCity = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreCity/DeleteCity?id=" + id, callbackFunction);
        };
    }]);
});