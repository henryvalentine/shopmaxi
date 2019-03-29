define(['application-configuration', 'ajaxService'], function (app) {

    app.register.service('couponServices', ['ajaxService', function (ajaxService)
    {
        this.addCoupon = function (coupon, callbackFunction)
        {
            var dxcv = JSON.stringify({ coupon: coupon });
            return ajaxService.AjaxPost(dxcv, "/Coupon/AddCoupon", callbackFunction);
        };
        
        this.getCoupon = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/Coupon/GetCoupon?id=" + id, callbackFunction);
        };

        this.editCoupon = function (coupon, callbackFunction)
        {
            var dxcv = JSON.stringify({ coupon: coupon });
            return ajaxService.AjaxPost(dxcv, "/Coupon/EditCoupon", callbackFunction);
        };

        this.deleteCoupon = function (id, callbackFunction)
        {
            return ajaxService.AjaxDelete("/Coupon/DeleteCoupon?id=" + id, callbackFunction);
        };

    }]);
});