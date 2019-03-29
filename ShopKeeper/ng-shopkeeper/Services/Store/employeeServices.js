define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('employeeServices', ['ajaxService', function (ajaxService)
    {
        this.addEmployee = function (employee, callbackFunction)
        {
            return ajaxService.AjaxPost({ employee: employee }, "/Employee/AddEmployee", callbackFunction);
        };

        this.editEmployee = function (employee, callbackFunction)
        {
            return ajaxService.AjaxPost({ employee: employee }, "/Employee/EditEmployee", callbackFunction);
        };
        
        this.editMyProfile = function (employee, callbackFunction)
        {
            return ajaxService.AjaxPost({ employee: employee }, "/Employee/EditProfile", callbackFunction);
        };

        this.editAdminProfle = function (employee, callbackFunction)
        {
            return ajaxService.AjaxPost({ employee: employee }, "/Employee/EditAdminProfile", callbackFunction);
        };

       
        this.checkAuthStatus = function ()
        {
            return ajaxService.checkAuthenticationOnly();
        };

        this.editEmployeeVariants = function (employeeList, callbackFunction)
        {
            return ajaxService.AjaxPost({ employeeVariantList: employeeList }, "/Employee/EditEmployeeVariants", callbackFunction);
        };

        this.getEmployee = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Employee/GetEmployee?id=" + id, callbackFunction);
        };

        this.getMyProfile = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Employee/GetMyProfile", callbackFunction);
        };
        
        this.getGenericList = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Employee/GetListObjects", callbackFunction);
        };

        this.getRoles = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Account/GetRoles", callbackFunction);
        };
        
        
        this.deleteEmployee = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Employee/DeleteEmployee?id=" + id, callbackFunction);
        };

    }]);
});