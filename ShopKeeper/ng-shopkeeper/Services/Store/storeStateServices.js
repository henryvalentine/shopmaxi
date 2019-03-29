define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('storeStateServices', ['ajaxService', function (ajaxService)
    {
        this.addState = function(state, callbackFunction)
        {
            var dxcv = JSON.stringify({ state: state });
            return ajaxService.AjaxPost(dxcv, "/StoreState/AddState", callbackFunction);
        };
        
        this.getState = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreState/GetState?id=" + id, callbackFunction);
        };
        
        this.getCountries = function(callbackFunction) 
        {
            return ajaxService.AjaxGet("/StoreState/GetCountries", callbackFunction);
        };

        this.editState = function (state, callbackFunction)
        {
            var dxcv = JSON.stringify({ state: state });
            return ajaxService.AjaxPost(dxcv, "/StoreState/EditState", callbackFunction);
        };

        this.deleteState = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreState/DeleteState?id=" + id, callbackFunction);
        };
    }]);
});