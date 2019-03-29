define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('storeDepartmentServices', ['ajaxService', function (ajaxService)
    {
        this.addStoreDepartment = function (department, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeDepartment: department }, "/StoreDepartment/AddStoreDepartment", callbackFunction);
        };
        
        this.getStoreDepartment = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreDepartment/GetStoreDepartment?id=" + id, callbackFunction);
        };

        this.editStoreDepartment = function (department, callbackFunction)
        {
            return ajaxService.AjaxPost({ storeDepartment: department }, "/StoreDepartment/EditStoreDepartment", callbackFunction);
        };

        this.deleteStoreDepartment = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/StoreDepartment/DeleteStoreDepartment?id=" + id, callbackFunction);
        };

    }]);
});