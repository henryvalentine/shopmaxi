define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('documentTypeServices', ['ajaxService', function (ajaxService)
    {
        this.addDocumentType = function (documentType, callbackFunction)
        {
            return ajaxService.AjaxPost({ documentType: documentType }, "/DocumentType/AddDocumentType", callbackFunction);
        };
        
        this.getDocumentType = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/DocumentType/GetDocumentType?id=" + id, callbackFunction);
        };

        this.editDocumentType = function (documentType, callbackFunction)
        {
            return ajaxService.AjaxPost({ documentType: documentType }, "/DocumentType/EditDocumentType", callbackFunction);
        };

        this.deleteDocumentType = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/DocumentType/DeleteDocumentType?id=" + id, callbackFunction);
        };

    }]);
});