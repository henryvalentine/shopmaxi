define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('stateServices', ['ajaxService', function (ajaxService) {


        this.addState = function(state, callbackFunction)
        {
            var dxcv = JSON.stringify({ state: state });
            return ajaxService.AjaxPost(dxcv, "/State/AddState", callbackFunction);
        };
        
        this.getState = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/State/GetState?id=" + id, callbackFunction);
        };
        
        this.getCountries = function(callbackFunction) 
        {
            return ajaxService.AjaxGet("/State/GetCountries", callbackFunction);
        };

        this.editState = function (state, callbackFunction)
        {
            var dxcv = JSON.stringify({ state: state });
            return ajaxService.AjaxPost(dxcv, "/State/EditState", callbackFunction);
        };

        this.deleteState = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/State/DeleteState?id=" + id, callbackFunction);
        };
    }]);
});