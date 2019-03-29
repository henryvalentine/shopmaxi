define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('reportServices', ['ajaxService', function (ajaxService)
    {
        this.getProduct = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Report/GetSalesReportDetails?saleId=" + id, callbackFunction);
        };
        
        this.getDefaults = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Report/GetDefaults", callbackFunction);
        };

        this.getProducts = function (page, itemsPerPage, callbackFunction)
        {
            return ajaxService.AjaxGet("/Sales/GetAllPriceLists?page=" + page + "&itemsPerPage=" + itemsPerPage, callbackFunction);
        };
        

    }]);
});