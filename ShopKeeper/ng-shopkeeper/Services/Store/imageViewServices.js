define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('imageViewServices', ['ajaxService', function (ajaxService)
    {
        this.addImageView = function (imageView, callbackFunction)
        {
            var dxcv = JSON.stringify({ imageView: imageView });
            return ajaxService.AjaxPost(dxcv, "/ImageView/AddImageView", callbackFunction);
        };
        
        this.getImageView = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ImageView/GetImageView?id=" + id, callbackFunction);
        };

        this.editImageView = function (imageView, callbackFunction)
        {
            var dxcv = JSON.stringify({ imageView: imageView });
            return ajaxService.AjaxPost(dxcv, "/ImageView/EditImageView", callbackFunction);
        };

        this.deleteImageView = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/ImageView/DeleteImageView?id=" + id, callbackFunction);
        };

    }]);
});