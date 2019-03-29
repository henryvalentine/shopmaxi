define(['application-configuration', 'ajaxService'], function (app)
{
    app.register.service('welcomeServices', ['ajaxService', function (ajaxService)
    {
        this.addSupplier = function (supplier, callbackFunction)
        {
            return ajaxService.AjaxPost({ supplier: supplier }, "/Supplier/AddSupplier", callbackFunction);
        };
        
        this.getItems = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/StoreItem/GetStoreItems", callbackFunction);
        };

        this.getDefaults = function (callbackFunction)
        {
            return ajaxService.AjaxGet("/Account/GetLinks", callbackFunction);
        };

        this.getItemDetails = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ShoppingCart/GetItemDetails?id=" + id, callbackFunction);
        };

        this.getItemVariations = function (id, callbackFunction)
        {
            return ajaxService.AjaxGet("/ShoppingCart/GetItemVariations?storeItemId=" + id, callbackFunction);
        };

        this.processShoppingCart = function (cart, callbackFunction)
        {
            return ajaxService.AjaxPost({ cart: cart }, "/ShoppingCart/ProcessShoppingCart", callbackFunction);
        };

        this.processShoppingCartItem = function (cartItem, callbackFunction)
        {
            return ajaxService.AjaxPost({ cartItem: cartItem }, "/ShoppingCart/ProcessShoppingCartItem", callbackFunction);
        };

        this.updateCartItem = function (cartItem, callbackFunction)
        {
            return ajaxService.AjaxPost({ cartItemObject: cartItem }, "/ShoppingCart/UpdateShoppingCartItem", callbackFunction);
        };
        
        this.removeCartItem = function (id, callbackFunction)
        {
            return ajaxService.AjaxPost({ cartItemId: id }, "/ShoppingCart/RemoveShoppingCartItem", callbackFunction);
        };

        this.getStates = function (countryId, callbackFunction)
        {
            return ajaxService.AjaxGet("/ShoppingCart/GetCountryStates?countryId=" + countryId, callbackFunction);
        };

        this.getCities = function (stateId, callbackFunction)
        {
            return ajaxService.AjaxGet("/ShoppingCart/GetStateCities?stateId=" + stateId, callbackFunction);
        };

        this.processCartCheckout = function (deliveryAddress, callbackFunction)
        {
            return ajaxService.AjaxPost({deliveryAddress: deliveryAddress }, "/ShoppingCart/ProcessCartCheckout", callbackFunction);
        };

        this.getCoupon = function (couponCode, callbackFunction)
        {
            return ajaxService.AjaxGet("/ShoppingCart/GetCouponInfo?couponCode=" + couponCode, callbackFunction);
        };

    }]);
});