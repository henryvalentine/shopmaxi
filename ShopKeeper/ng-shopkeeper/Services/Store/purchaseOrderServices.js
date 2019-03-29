define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('purchaseOrderServices', ['ajaxService', function (ajaxService)
    {
        this.addpurchaseOrder = function (purchaseOder, callbackFunction)
        {
            return ajaxService.AjaxPost({ purchaseOrder: purchaseOder }, "/Purchaseorder/AddPurchaseOrder", callbackFunction);
        };
        
        this.editPurchaseorder = function (purchaseOder, callbackFunction)
        {
            return ajaxService.AjaxPost({ purchaseOrder: purchaseOder }, "/Purchaseorder/EditPurchaseOrder", callbackFunction);
        };
        
         this.addpurchaseOrderDelivery = function (purchaseOder, callbackFunction)
         {
             return ajaxService.AjaxPost({ deliveryObject: purchaseOder }, "/Purchaseorder/AddPurchaseOrderDelivery", callbackFunction);
         };

         this.processPurchaseorderDelivery = function (delivery, callbackFunction)
         {
             return ajaxService.AjaxPost({ delivery: delivery }, "/Purchaseorder/ProcessPurchaseorderDelivery", callbackFunction);
         };
       
         this.editPurchaseorderDelivery = function (purchaseOder, callbackFunction)
         {
             return ajaxService.AjaxPost({purchaseOder: purchaseOder }, "/Purchaseorder/EditPurchaseOrderDelivery", callbackFunction);
         };
         
        this.getPurchaseorder = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Purchaseorder/GetPurchaseOrder?id=" + id, callbackFunction);
        };

        this.getPurchaseorderDetails = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Purchaseorder/GetPurchaseOrderDetails?id=" + id, callbackFunction);
        };

        this.deleteProduct = function (id)
        {
            return ajaxService.AjaxDeleteWithPromise({ orderItemId: id }, "/Purchaseorder/DeleteOrderItem");
        };

        this.deleteOrderReceipt = function (id)
        {
            return ajaxService.AjaxDeleteWithPromise({ orderItemId: id }, "/Purchaseorder/DeleteOrderItem");
        };

        this.deleteOrderItemReceipt = function (id)
        {
            return ajaxService.AjaxDeleteWithPromise({id: id }, "/Purchaseorder/DeletePurchaseOrderItemDelivery");
        };
        
        this.getProducts = function (page, itemsPerPage, callbackFunction)
        {
            return ajaxService.AjaxGet("/Purchaseorder/GetProducts?page=" + page + "&itemsPerPage=" + itemsPerPage, callbackFunction);
        };
        
        this.getSelectibles = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Purchaseorder/GetSelectables", callbackFunction);
        };
    
    }]);
});