define(['application-configuration', 'ajaxService'], function (app)
{

    app.register.service('storeTransactionServices', ['ajaxService', function (ajaxService)
    {
        this.addStoreTransaction = function (storeTransaction, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeTransaction: storeTransaction }, "/StoreTransaction/AddStoreTransaction", callbackFunction);
        };
        
        this.getStoreTransaction = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreTransaction/GetStoreTransaction?id=" + id, callbackFunction);
        };

        this.editStoreTransaction = function (storeTransaction, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeTransaction: storeTransaction }, "/StoreTransaction/EditStoreTransaction", callbackFunction);
        };

        this.deleteStoreTransaction = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreTransaction/DeleteStoreTransaction?id=" + id, callbackFunction);
        };

    }]);
});