define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('registerServices', ['ajaxService', function (ajaxService)
    {
        this.addRegister = function (register, callbackFunction)
        {
            var dxcv = JSON.stringify({ register: register });
            return ajaxService.AjaxPost(dxcv, "/Register/AddRegister", callbackFunction);
        };
        
        this.getRegister = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Register/GetRegister?id=" + id, callbackFunction);
        };

        this.getOutlets = function (callbackFunction) {
            return ajaxService.AjaxGet("/Register/GetStoreOutlets", callbackFunction);
        };

        this.editRegister = function (register, callbackFunction)
        {
            var dxcv = JSON.stringify({ register: register });
            return ajaxService.AjaxPost(dxcv, "/Register/EditRegister", callbackFunction);
        };

        this.deleteRegister = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Register/DeleteRegister?id=" + id, callbackFunction);
        };

    }]);
});