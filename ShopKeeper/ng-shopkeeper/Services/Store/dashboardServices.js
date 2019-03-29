define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('dashboardServices', ['ajaxService', function (ajaxService)
    {
        this.getReportSnapshots = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Report/GetReportSnapshots", callbackFunction);
        };

    }]);
});
