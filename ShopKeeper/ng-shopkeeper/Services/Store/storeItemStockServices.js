define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('storeItemStockServices', ['ajaxService', function (ajaxService)
    {
        this.addStoreItemStock = function (product, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemStock: product }, "/StoreItemStock/AddStoreItemStock", callbackFunction);
        };

        this.addStoreItemStockVariants = function (productList, callbackFunction)
        {
            return ajaxService.AjaxPost({ productVariantList: productList }, "/StoreItemStock/AddStoreItemStockVariants", callbackFunction);
        };
        
        this.editStoreItemStock = function (product, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemStock: product }, "/StoreItemStock/EditStoreItemStock", callbackFunction);
        };
        
        this.EditInventory = function (product, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemStock: product }, "/StoreItemStock/EditInventory", callbackFunction);
        };

        this.getOutlets = function (callbackFunction) {
            return ajaxService.AjaxGet("/StoreOutlet/GetOutlets", callbackFunction);
        };

        this.GetInventory = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemStock/GetInventory", callbackFunction);
        };

        this.getPrices = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ItemPrice/GetItemPriceList?stockItemId=" + id, callbackFunction);
        };
       
        this.editStoreItemStockVariants = function (productList, callbackFunction)
        {
            return ajaxService.AjaxPost({ productVariantList: productList }, "/StoreItemStock/EditStoreItemStockVariants", callbackFunction);
        };

        this.getStoreItemStock = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemStock/GetStoreItemStock?id=" + id, callbackFunction);
        };
        
        this.getDbx = function (brandId, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemStock/GetDbCount?brandId=" + brandId, callbackFunction);
        };
        
        this.getGenericList = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemStock/GetListObjects", callbackFunction);
        };

        this.getProductListObjects = function (callbackFunction) {
            return ajaxService.AjaxGet("/StoreItem/GetListObjects", callbackFunction);
        };

        this.getDefaultList = function (callbackFunction) {
            return ajaxService.AjaxGet("/Employee/GetListObjects", callbackFunction);
        };
        
        this.getGenericList2 = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItemStock/GetListObjects2", callbackFunction);
        };
        
        this.getStockTemplate = function (path)
        {
            return ajaxService.AjaxDownload("/StoreItemStock/DownloadContentFromFolder?path=" + path);
        };

        this.deleteStoreItemStock = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreItemStock/DeleteStoreItemStock?id=" + id, callbackFunction);
        };
    
        this.addItemPrice = function (itemPrice, callbackFunction)
        {
            return ajaxService.AjaxPost({itemPrice: itemPrice }, "/ItemPrice/AddItemPrice", callbackFunction);
        };

        this.editItemPrice = function (itemPrice, callbackFunction) {
            return ajaxService.AjaxPost({ itemPrice: itemPrice }, "/ItemPrice/EditPrice", callbackFunction);
        };

        this.deleteStockUpload = function (stockUpload, callbackFunction)
        {
            return ajaxService.AjaxPost({ stockUpload: stockUpload }, "/StoreItemStock/DeleteStockUpload", callbackFunction);
        };
        
        this.deleteItemPrice = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/ItemPrice/DeleteItemPrice?id=" + id, callbackFunction);
        };

    }]);
});