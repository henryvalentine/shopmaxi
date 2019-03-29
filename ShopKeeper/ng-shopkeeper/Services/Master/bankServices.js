define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('bankServices', ['ajaxService', function (ajaxService)
    {
        
        this.addBank = function(bank, callbackFunction) {
            var dxcv = JSON.stringify({ bank: bank });
            return ajaxService.AjaxPost(dxcv, "/Bank/AddBank", callbackFunction);
        };

        this.getBank = function(id, callbackFunction) {
            return ajaxService.AjaxGet("/Bank/GetBank?id=" + id, callbackFunction);
        };

        this.editBank = function(bank, callbackFunction) {
            var dxcv = JSON.stringify({ bank: bank });
            return ajaxService.AjaxPost(dxcv, "/Bank/EditBank", callbackFunction);
        };

        this.deleteBank = function(id, callbackFunction) {
            return ajaxService.AjaxDelete("/Bank/DeleteBank?id=" + id, callbackFunction);
        };
    

    }]);
});