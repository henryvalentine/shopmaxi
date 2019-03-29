define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('cityServices', ['ajaxService', function (ajaxService) {


        this.addCity = function(city, callbackFunction)
        {
            var dxcv = JSON.stringify({ city: city });
            return ajaxService.AjaxPost(dxcv, "/City/AddCity", callbackFunction);
        };
        
        this.getCity = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/City/GetCity?id=" + id, callbackFunction);
        };
        
        this.getStates = function(callbackFunction) 
        {
            return ajaxService.AjaxGet("/City/GetStates", callbackFunction);
        };

        this.editCity = function (city, callbackFunction)
        {
            var dxcv = JSON.stringify({ city: city });
            return ajaxService.AjaxPost(dxcv, "/City/EditCity", callbackFunction);
        };

        this.deleteCity = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/City/DeleteCity?id=" + id, callbackFunction);
        };
    }]);
});