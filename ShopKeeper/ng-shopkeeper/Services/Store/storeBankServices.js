define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('storeBankServices', ['ajaxService', function (ajaxService) {
        
        this.addBank = function(bank, callbackFunction) {
            var dxcv = JSON.stringify({ bank: bank });
            return ajaxService.AjaxPost(dxcv, "/StoreBank/AddBank", callbackFunction);
        };

        this.getBank = function(id, callbackFunction) {
            return ajaxService.AjaxGet("/StoreBank/GetBank?id=" + id, callbackFunction);
        };

        this.editBank = function(bank, callbackFunction) {
            var dxcv = JSON.stringify({ bank: bank });
            return ajaxService.AjaxPost(dxcv, "/StoreBank/EditBank", callbackFunction);
        };

        this.deleteBank = function(id, callbackFunction) {
            return ajaxService.AjaxDelete("/StoreBank/DeleteBank?id=" + id, callbackFunction);
        };
    

    }]);
});