define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('saleServices', ['ajaxService', function (ajaxService)
    {
        this.addSale = function (sale, callbackFunction)
        {
            return ajaxService.AjaxPost({ genericSale: sale }, "/Sales/AddSale", callbackFunction);
        };

        this.addSaleList = function (saleList, callbackFunction)
        {
            return ajaxService.AjaxPost({ saleList: saleList }, "/Sales/AddSaleList", callbackFunction);
        };

        this.editSale = function (sale, callbackFunction)
        {
            return ajaxService.AjaxPost({ sale: sale }, "/Sales/EditSale", callbackFunction);
        };

        this.processCustomer = function (customer, callbackFunction)
        {
            return ajaxService.AjaxPost({ customer: customer }, "/Sales/AddSalesCustomer", callbackFunction);
        };

        this.getCustomerTypes = function (callbackFunction) {
            return ajaxService.AjaxGet("/Customer/GetCustomerTypes", callbackFunction);
        };

        this.updateSalePayment = function (sale, callbackFunction) {
            return ajaxService.AjaxPost({ sale: sale }, "/Sales/UpdateSalePayment", callbackFunction);
        };

        this.refundSale = function (refundNote, callbackFunction)
        {
            return ajaxService.AjaxPost({ refundNote: refundNote }, "/Sales/RefundSale", callbackFunction);
        };
        
        this.getSale = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetSale?id=" + id, callbackFunction);
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
            return ajaxService.AjaxGet("/Sales/GetAllPriceLists?page=" + page + "&itemsPerPage=" + itemsPerPage, callbackFunction);
        };

        this.getEveryPriceList = function (page, itemsPerPage, callbackFunction) {
            return ajaxService.AjaxGet("/Sales/GetAllItemPriceList?page=" + page + "&itemsPerPage=" + itemsPerPage, callbackFunction);
        };

        this.getIssueTypes = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/IssueType/GetIssueTypes", callbackFunction);
        };

        this.getProduct = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Report/GetSalesReportDetails?saleId=" + id, callbackFunction);
        };

        this.getInvoice = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetInvoice?id=" + id, callbackFunction);
        };

        this.getSalesInfo = function (invoiceNumber, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetSalesReportDetails?invoiceNumber=" + invoiceNumber, callbackFunction);
        };

        this.getSaleToRefund = function (invoiceNumber, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetInvoiceForRevoke?invoiceNumber=" + invoiceNumber, callbackFunction);
        };

        this.getSaleRefundNotes = function (saleId, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetRefundedSaleNotes?saleId=" + saleId, callbackFunction);
        };
        
        this.getRevokedSalesInfo = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetRevokedDetails?id=" + id, callbackFunction);
        };
       
        this.getItemPrices = function (criteria, callbackFunction)
        {
            return ajaxService.AjaxGet("/ItemPrice/GetItemPrices?criteria=" + criteria, callbackFunction);
        };

        this.getItemsBySearchCriteria = function (url, callbackFunction) {
            return ajaxService.AjaxGet(url, callbackFunction);
        };

        this.getProductsBySKU = function (sku, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetProduct?sku=" + sku, callbackFunction);
        };

        this.getMoreCustomers = function (pageNumber, itemsPerPage, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetMoreCustomers?pageNumber=" + pageNumber + '&itemsPerPage=' + itemsPerPage, callbackFunction);
        };
        
        this.getUSBDevices = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetUSBDevices", callbackFunction);
        };

        this.getCustomers = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetCustomers", callbackFunction);
        };

        this.searchCustomers = function (criteria, callbackFunction)
        {
            return ajaxService.AjaxGet("/Customer/SearchCustomer?criteria=" + criteria, callbackFunction);
        };

        this.getGenericList = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetListObjects", callbackFunction);
        };

        this.getPaymenthod = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetPaymenthod", callbackFunction);
        };

        this.verifyDelete = function (model, callbackFunction)
        {
            return ajaxService.AjaxPost({ model: model }, "/Account/VerifyDelete", callbackFunction);
        };

        
        this.deleteSale = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Sales/DeleteSale?id=" + id, callbackFunction);
        };
    
    }]);
});