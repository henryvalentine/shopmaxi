define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('estimateServices', ['ajaxService', function (ajaxService)
    {
        this.addEstimate = function (estimate, callbackFunction)
        {
            return ajaxService.AjaxPost({ estimate: estimate }, "/Estimate/AddEstimate", callbackFunction);
        };

        this.editEstimate = function (estimate, callbackFunction)
        {
            return ajaxService.AjaxPost({ estimate: estimate }, "/Estimate/EditEstimate", callbackFunction);
        };
        
        this.getEstimate = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Estimate/GetEstimate?id=" + id, callbackFunction);
        };

        this.getEstimateByRef = function (refNumber, callbackFunction) {
            return ajaxService.AjaxGet("/Estimate/GetEstimateByRef?refNumber=" + refNumber, callbackFunction);
        };

        this.getEstimateInvoice = function (estimateNumber, callbackFunction)
        {
            return ajaxService.AjaxGet("/Estimate/GetEstimateInvoice?estimateNumber=" + estimateNumber, callbackFunction);
        };
        
        this.deleteEstimateItem = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Estimate/DeleteEstimateItem?id=" + id, callbackFunction);
        };
       
        this.deleteEstimate = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Estimate/DeleteEstimate?id=" + id, callbackFunction);
        };
        
        this.getProducts = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetPriceLists?categoryId=" + id, callbackFunction);
        };

        this.getCustomerInfo = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Customer/GetCustomer?id=" + id, callbackFunction);
        };
        
        this.getAllProducts = function (page, itemsPerPage, callbackFunction)
        {
            return ajaxService.AjaxGet("/Purchaseorder/GetProducts?page=" + page + "&itemsPerPage=" + itemsPerPage, callbackFunction);
        };

        this.getItemPrices = function (criteria, callbackFunction)
        {
            return ajaxService.AjaxGet("/ItemPrice/GetItemPrices?criteria=" + criteria, callbackFunction);
        };

        this.getProductsBySKU = function (sku, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetProduct?sku=" + sku, callbackFunction);
        };
      
        this.getCustomers = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Estimate/GetCustomers", callbackFunction);
        };

        this.getList = function (callbackFunction) {
            return ajaxService.AjaxGet("/Estimate/GetListObjects", callbackFunction);
        };

        this.deleteSale = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Estimate/DeleteEstimate?id=" + id, callbackFunction);
        };

        /*****************************************  PROCESS ESTIMATES  **********************************************/
    
        this.addSale = function (sale, callbackFunction) {
            return ajaxService.AjaxPost({ genericSale: sale }, "/Sales/AddSale", callbackFunction);
        };

        this.addSaleList = function (saleList, callbackFunction) {
            return ajaxService.AjaxPost({ saleList: saleList }, "/Sales/AddSaleList", callbackFunction);
        };

        this.editSale = function (sale, callbackFunction) {
            return ajaxService.AjaxPost({ sale: sale }, "/Sales/EditSale", callbackFunction);
        };

        this.getSale = function (id, callbackFunction) {
            return ajaxService.AjaxGet("/Sales/GetSale?id=" + id, callbackFunction);
        };

        this.deleteSale = function (estimateNumber, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Estimate/ConvertEstimateToInvoice?estimateNumber=" + estimateNumber, callbackFunction);
        };
    }]);
});