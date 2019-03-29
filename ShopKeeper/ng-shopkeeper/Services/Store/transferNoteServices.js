define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('transferNoteServices', ['ajaxService', function (ajaxService)
    {
        this.addTransferNote = function (transferNote, callbackFunction)
        {
            return ajaxService.AjaxPost({ transferNote: transferNote }, "/TransferNote/AddTransferNote", callbackFunction);
        };
         
        this.editTransferNote = function (transferNote, callbackFunction)
        {
            return ajaxService.AjaxPost({ transferNote: transferNote }, "/TransferNote/EditTransferNote", callbackFunction);
        };
         
        this.getTransferNote = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/TransferNote/GetTransferNote?id=" + id, callbackFunction);
        };

        this.getTransferNoteByRef = function (refNumber, callbackFunction) {
            return ajaxService.AjaxGet("/TransferNote/GetTransferNoteByRef?refNumber=" + refNumber, callbackFunction);
        };
        
        this.getOutlets = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreOutlet/GetOutlets", callbackFunction);
        };
        
        this.deleteTransferNoteItem = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/TransferNote/DeleteTransferNoteItem?id=" + id, callbackFunction);
        };

        this.getCurrencies = function (callbackFunction) {
            return ajaxService.AjaxGet("/TransferNote/GetCurrencies", callbackFunction);
        };
    }]);
});