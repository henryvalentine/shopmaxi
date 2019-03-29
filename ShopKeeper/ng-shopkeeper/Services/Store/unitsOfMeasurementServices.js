define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('unitsOfMeasurementServices', ['ajaxService', function (ajaxService)
    {
        this.addUnitOfMeasurement = function (unitOfMeasurement, callbackFunction)
        {
            return ajaxService.AjaxPost({ unitofMeasurement: unitOfMeasurement }, "/UnitOfMeasurement/AddUnitOfMeasurement", callbackFunction);
        };
        
        this.getUnitOfMeasurement = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/UnitOfMeasurement/GetUnitOfMeasurement?id=" + id, callbackFunction);
        };

        this.editUnitOfMeasurement = function (unitOfMeasurement, callbackFunction)
        {
            return ajaxService.AjaxPost({ unitofMeasurement: unitOfMeasurement }, "/UnitOfMeasurement/EditUnitofMeasurement", callbackFunction);
        };

        this.deleteUnitOfMeasurement = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/UnitOfMeasurement/DeleteUnitOfMeasurement?id=" + id, callbackFunction);
        };

    }]);
});