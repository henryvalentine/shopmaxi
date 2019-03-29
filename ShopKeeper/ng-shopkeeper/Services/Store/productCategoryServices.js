define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('productCategoryServices', ['ajaxService', function (ajaxService)
    {
        this.addProductCategory = function (productCategory, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemCategory: productCategory }, "/StoreItemCategory/AddStoreItemCategory", callbackFunction);
        };

        this.getProductCategory = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemCategory/GetStoreItemCategory?id=" + id, callbackFunction);
        };
        
        this.getProductCategories = function (callbackFunction) {
            return ajaxService.AjaxGet("/StoreItemCategory/GetStoreItemCategories", callbackFunction);
        };

        this.editProductCategory = function (productCategory, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemCategory: productCategory }, "/StoreItemCategory/EditStoreItemCategory", callbackFunction);
        };

        this.deleteProductCategory = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItemCategory/DeleteStoreItemCategory?id=" + id, callbackFunction);
        };
    

    }]);
});