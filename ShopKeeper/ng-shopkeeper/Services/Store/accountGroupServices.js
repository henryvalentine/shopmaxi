define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('accountGroupServices', ['ajaxService', function (ajaxService)
    {
        this.addAccountGroup = function (accountGroup, callbackFunction)
        {
            return ajaxService.AjaxPost({ accountGroup: accountGroup }, "/AccountGroup/AddAccountGroup", callbackFunction);
        };
        
        this.getAccountGroup = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/AccountGroup/GetAccountGroup?id=" + id, callbackFunction);
        };
        
        this.editAccountGroup = function (accountGroup, callbackFunction)
        {
            return ajaxService.AjaxPost({ accountGroup: accountGroup }, "/AccountGroup/EditAccountGroup", callbackFunction);
        };

        this.deleteAccountGroup = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/AccountGroup/DeleteAccountGroup?id=" + id, callbackFunction);
        };

    }]);
});