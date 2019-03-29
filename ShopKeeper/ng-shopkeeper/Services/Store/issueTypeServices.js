define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('issueTypeServices', ['ajaxService', function (ajaxService)
    {
        this.addIssueType = function (issueType, callbackFunction)
        {
            var dxcv = JSON.stringify({ issueType: issueType });
            return ajaxService.AjaxPost(dxcv, "/IssueType/AddIssueType", callbackFunction);
        };
        
        this.getIssueType = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/IssueType/GetIssueType?id=" + id, callbackFunction);
        };

        this.editIssueType = function (issueType, callbackFunction)
        {
            var dxcv = JSON.stringify({ issueType: issueType });
            return ajaxService.AjaxPost(dxcv, "/IssueType/EditIssueType", callbackFunction);
        };

        this.deleteIssueType = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/IssueType/DeleteIssueType?id=" + id, callbackFunction);
        };

    }]);
});