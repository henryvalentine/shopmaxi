define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('storeTransactionTypeServices', ['ajaxService', function (ajaxService)
    {
        this.addTransactionType = function (transactionType, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeTransactionType: transactionType }, "/StoreTransactionType/AddStoreTransactionType", callbackFunction);
        };

        this.getTransactionType = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreTransactionType/GetStoreTransactionType?id=" + id, callbackFunction);
        };

        this.editTransactionType = function (transactionType, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeTransactionType: transactionType }, "/StoreTransactionType/EditStoreTransactionType", callbackFunction);
        };

        this.deleteTransactionType = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreTransactionType/DeleteStoreTransactionType?id=" + id, callbackFunction);
        };
    

    }]);
});