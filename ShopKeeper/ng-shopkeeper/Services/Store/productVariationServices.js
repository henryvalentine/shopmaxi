define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('productVariationServices', ['ajaxService', function (ajaxService)
    {
        this.addProductVariation = function (productVariation, callbackFunction)
        {
            return ajaxService.AjaxPost({ productVariation: productVariation }, "/StoreItemVariation/AddStoreItemVariation", callbackFunction);
        };
        
        this.getProductVariation = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemVariation/GetStoreItemVariation?id=" + id, callbackFunction);
        };

        this.editProductVariation = function (productVariation, callbackFunction)
        {
            return ajaxService.AjaxPost({ productVariation: productVariation }, "/StoreItemVariation/EditStoreItemVariation", callbackFunction);
        };

        this.deleteProductVariation = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItemVariation/DeleteStoreItemVariation?id=" + id, callbackFunction);
        };

    }]);
});