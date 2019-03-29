define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('productTypeServices', ['ajaxService', function (ajaxService)
    {
        this.addStoreItemType = function (productType, callbackFunction)
        {
            return ajaxService.AjaxPost({ productType: productType }, "/StoreItemType/AddStoreItemType", callbackFunction);
        };

        this.getStoreItemType = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemType/GetStoreItemType?id=" + id, callbackFunction);
        };

        this.getItemTypeChildren = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemType/GetItemTypeChildren?id=" + id, callbackFunction);
        };

        this.editStoreItemType = function (productType, callbackFunction)
        {
            return ajaxService.AjaxPost({ productType: productType }, "/StoreItemType/EditStoreItemType", callbackFunction);
        };

        this.deleteStoreItemType = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItemType/DeleteStoreItemType?id=" + id, callbackFunction);
        };
    

    }]);
});