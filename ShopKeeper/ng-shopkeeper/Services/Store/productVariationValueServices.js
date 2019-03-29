define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('productVariationValueServices', ['ajaxService', function (ajaxService)
    {
        this.addProductVariationValue = function (productVariationValue, callbackFunction)
        {
            return ajaxService.AjaxPost({ productVariationValue: productVariationValue }, "/StoreItemVariationValue/AddStoreItemVariationValue", callbackFunction);
        };
        
        this.getProductVariationValue = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemVariationValue/GetStoreItemVariationValue?id=" + id, callbackFunction);
        };

        this.editProductVariationValue = function (productVariationValue, callbackFunction)
        {
            return ajaxService.AjaxPost({ productVariationValue: productVariationValue }, "/StoreItemVariationValue/EditStoreItemVariationValue", callbackFunction);
        };

        this.deleteProductVariationValue = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItemVariationValue/DeleteStoreItemVariationValue?id=" + id, callbackFunction);
        };

    }]);
});