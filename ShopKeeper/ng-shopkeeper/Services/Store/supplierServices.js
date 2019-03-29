define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('supplierServices', ['ajaxService', function (ajaxService)
    {
        this.addSupplier = function (supplier, callbackFunction)
        {
            return ajaxService.AjaxPost({ supplier: supplier }, "/Supplier/AddSupplier", callbackFunction);
        };
        
        this.getSupplier = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Supplier/GetSupplier?id=" + id, callbackFunction);
        };

        this.editSupplier = function (supplier, callbackFunction)
        {
            return ajaxService.AjaxPost({ supplier: supplier }, "/Supplier/EditSupplier", callbackFunction);
        };

        this.deleteSupplier = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Supplier/DeleteSupplier?id=" + id, callbackFunction);
        };

    }]);
});