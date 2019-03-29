define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('itemPriceServices', ['ajaxService', function (ajaxService)
    {
        this.addItemPrice = function (itemPrice, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeItemStock: itemPrice }, "/ItemPrice/AddItemPrice", callbackFunction);
        };

        this.addPriceList = function (priceList, callbackFunction)
        {
            return ajaxService.AjaxPost({ priceList: priceList }, "/ItemPrice/AddPriceList", callbackFunction);
        };

        this.editItemPrice = function (itemPrice, callbackFunction)
        {
            return ajaxService.AjaxPost({ itemPrice: itemPrice }, "/ItemPrice/EditItemPrice", callbackFunction);
        };
        
        this.getItemPrice = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ItemPrice/GetItemPrice?id=" + id, callbackFunction);
        };

        this.getItemPrices = function (criteria, callbackFunction)
        {
            return ajaxService.AjaxGet("/ItemPrice/GetItemPrices?criteria=" + criteria, callbackFunction);
        };

        this.getGenericList = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/ItemPrice/GetListObjects", callbackFunction);
        };
        
        this.deleteItemPrice = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/ItemPrice/DeleteItemPrice?id=" + id, callbackFunction);
        };
    
    }]);
});