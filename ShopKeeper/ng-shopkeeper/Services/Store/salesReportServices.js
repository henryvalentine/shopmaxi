define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('salesReportServices', ['ajaxService', function (ajaxService)
    {
        this.getProduct = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Report/GetSalesReportDetails?saleId=" + id, callbackFunction);
        };

        this.getProductSalesReportDetail = function (productReport, callbackFunction)
        {
            return ajaxService.AjaxGetWithData(productReport, '/Report/productReport', callbackFunction);
        };

        this.getCustomers = function (callbackFunction)
        {
            return ajaxService.AjaxGet('/Report/GetCustomersAndSuppliers', callbackFunction);
        };

        this.getDefaults = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Report/GetDefaults", callbackFunction);
        };

        this.getSuppliers = function (callbackFunction) {
            return ajaxService.AjaxGet("/Report/GetSupplliers", callbackFunction);
        };

        this.getProducts = function (page, itemsPerPage, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetAllPriceLists?page=" + page + "&itemsPerPage=" + itemsPerPage, callbackFunction);
        };
        
        this.getReports = function (url, callbackFunction)
        {
            return ajaxService.AjaxGet(url, callbackFunction);
        };

        this.getreportWithPayload = function (data, route, callbackFunction)
        {
            return ajaxService.AjaxPost(data, route, callbackFunction);
        };

        this.getSaleRefundNotes = function (saleId, callbackFunction) {
            return ajaxService.AjaxGet("/Sales/GetRefundedSaleNotes?saleId=" + saleId, callbackFunction);
        };

    }]);
});
