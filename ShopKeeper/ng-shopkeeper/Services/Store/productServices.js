define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('productServices', ['ajaxService', function (ajaxService)
    {
        this.addProduct = function (product, callbackFunction)
        {
            return ajaxService.AjaxPost({ product: product }, "/StoreItem/AddStoreItem", callbackFunction);
        };

        this.getProduct = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItem/GetStoreItem?id=" + id, callbackFunction);
        };
        
        this.getGenericList = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItem/GetListObjects", callbackFunction);
        };

        this.editProduct = function (product, callbackFunction)
        {
            return ajaxService.AjaxPost({ product: product }, "/StoreItem/EditStoreItem", callbackFunction);
        };

        this.getProductBrandSearch = function (searchCriteria, callbackFunction)
        {
            return ajaxService.AjaxGet({ searchCriteria: searchCriteria }, "/StoreItem/GetStoreItemBrandObjects", callbackFunction);
        };

        this.deleteProduct = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItem/DeleteStoreItem?id=" + id, callbackFunction);
        };
    

    }]);
});