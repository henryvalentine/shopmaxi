define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('personServices', ['ajaxService', function (ajaxService) {


        this.addUserProfile = function(person, callbackFunction)
        {
            var dxcv = JSON.stringify({ person: person });
            return ajaxService.AjaxPost(dxcv, "/UserProfile/AddUserProfile", callbackFunction);
        };
        
        this.getSalutations = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/UserProfile/GetSalutations", callbackFunction);
        };


        this.getUserProfile = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/UserProfile/GetUserProfile?id=" + id, callbackFunction);
        };
       
        this.editUserProfile = function (person, callbackFunction)
        {
            var dxcv = JSON.stringify({ person: person });
            return ajaxService.AjaxPost(dxcv, "/UserProfile/EditUserProfile", callbackFunction);
        };

        this.deleteUserProfile = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/UserProfile/DeleteUserProfile?id=" + id, callbackFunction);
        };
    }]);
});