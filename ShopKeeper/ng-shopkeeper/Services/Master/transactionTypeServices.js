define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('transactionTypeServices', ['ajaxService', function (ajaxService)
    {
        this.addTransactionType = function (transactionType, callbackFunction)
        {
            return ajaxService.AjaxPost({ transactionType: transactionType }, "/TransactionType/AddTransactionType", callbackFunction);
        };

        this.getTransactionType = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/TransactionType/GetTransactionType?id=" + id, callbackFunction);
        };

        this.editTransactionType = function (transactionType, callbackFunction)
        {
            return ajaxService.AjaxPost({ transactionType: transactionType }, "/TransactionType/EditTransactionType", callbackFunction);
        };

        this.deleteTransactionType = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/TransactionType/DeleteTransactionType?id=" + id, callbackFunction);
        };
    

    }]);
});