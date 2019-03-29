define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('parentMenuServices', ['ajaxService', function (ajaxService)
    {
        this.addParentMenu = function (menu, callbackFunction)
        {
            return ajaxService.AjaxPost({ parentMenu: menu }, "/ParentMenu/AddSingleParentMenu", callbackFunction);
        };

        this.updateParentMenu = function (menu, callbackFunction)
        {
            return ajaxService.AjaxPost({ parentMenu: menu }, "/ParentMenu/EditParentMenu", callbackFunction);
        };
        
        this.addChildMenu = function (menu, callbackFunction)
        {
            return ajaxService.AjaxPost({childMenu: menu }, "/ParentMenu/AddChildMenu", callbackFunction);
        };
        
        this.updateChildMenu = function (menu, callbackFunction)
        {
            return ajaxService.AjaxPost({childMenu: menu }, "/ParentMenu/EditChildMenu", callbackFunction);
        };
        
        this.getParentMenu = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ParentMenu/GetParentMenu?menuId=" + id, callbackFunction);
        };

        this.getParentMenuChildren = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ParentMenu/GetCascades?menuId=" + id, callbackFunction);
        };
        
        this.getRoles = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Account/GetRoles", callbackFunction);
        };
        

    }]);
});