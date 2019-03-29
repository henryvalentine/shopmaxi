define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('productBrandServices', ['ajaxService', function (ajaxService)
    {
        this.addProductBrand = function (productBrand, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemBrand: productBrand }, "/StoreItemBrand/AddStoreItemBrand", callbackFunction);
        };
        
        this.getProductBrand = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemBrand/GetStoreItemBrand?id=" + id, callbackFunction);
        };

        this.editProductBrand = function (productBrand, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemBrand: productBrand }, "/StoreItemBrand/EditStoreItemBrand", callbackFunction);
        };

        this.deleteProductBrand = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItemBrand/DeleteStoreItemBrand?id=" + id, callbackFunction);
        };

    }]);
});