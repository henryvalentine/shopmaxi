define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('chartOfAccountServices', ['ajaxService', function (ajaxService) {


        this.addChartOfAccount = function(chartOfAccount, callbackFunction)
        {
            var dxcv = JSON.stringify({ chartOfAccount: chartOfAccount });
            return ajaxService.AjaxPost(dxcv, "/ChartOfAccount/AddChartOfAccount", callbackFunction);
        };
        
        this.getChartOfAccount = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ChartOfAccount/GetChartOfAccount?id=" + id, callbackFunction);
        };
        
        this.getAccountGroups = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/ChartOfAccount/GetAccountGroups", callbackFunction);
        };
        this.editChartOfAccount = function (chartOfAccount, callbackFunction)
        {
            var dxcv = JSON.stringify({ chartOfAccount: chartOfAccount });
            return ajaxService.AjaxPost(dxcv, "/ChartOfAccount/EditChartOfAccount", callbackFunction);
        };

        this.deleteChartOfAccount = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/ChartOfAccount/DeleteChartOfAccount?id=" + id, callbackFunction);
        };
    }]);
});