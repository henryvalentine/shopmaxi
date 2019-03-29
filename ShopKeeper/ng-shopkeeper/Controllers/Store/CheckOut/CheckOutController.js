"use strict";

define(['application-configuration', 'welcomeServices', 'ngDialog'], function (app)
{
    app.register.controller('checkOutController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'welcomeServices', '$location',
    function (ngDialog, $scope, $rootScope, $routeParams, welcomeServices, $location)
    {
        $scope.setDefaults = function ()
        {
            $scope.deliveryAddress = { Id: 0, AddressLine1: '', AddressLine2: '', CityId: '', CustomerId: null, CustomerIpAddress: '', PaymentTypeId: 0, Country: { Name: '-- Select Country --', StoreCountryId: '' }, State: { Name: '-- Select State --', StoreStateId: '' }, City: { Name: '-- Select City --', StoreCityId: '' }};

        };
        
        $scope.continue = function ()
        {
            if ($rootScope.ShoppingCart == null || $rootScope.ShoppingCart.ShoppingCartId < 1)
            {
                $scope.setError('Your Cart information is empty. Please try again later.');
                return;
            }

            if ($rootScope.ShoppingCart.ShopingCartItemObjects < 1)
            {
                $scope.setError('Your Cart information is empty. Please try again later.');
                return;
            }

            if ($scope.deliveryAddress.Country == null || $scope.deliveryAddress.Country.StoreCountryId < 1)
            {
                $scope.setError('Please a Country.');
                return;
            }

            if ($scope.deliveryAddress.State == null || $scope.deliveryAddress.State.StoreStateId < 1)
            {
                $scope.setError('Please select a State.');
                return;
            }

            if ($scope.deliveryAddress.City == null || $scope.deliveryAddress.City.StoreCityId < 1)
            {
                $scope.setError('Please select a City.');
                return;
            }

            if ($scope.deliveryAddress.AddressLine1 == null || $scope.deliveryAddress.AddressLine1.trim() < 1)
            {
                $scope.setError('Please provide Address Line 1.');
                return;
            }

            if ($scope.deliveryAddress.ContactEmail == null || $scope.deliveryAddress.ContactEmail.trim() < 1) {
                $scope.setError('Please provide your email address.');
                return;
            }

            if ($rootScope.deliveryAddress.PaymentTypeId < 1)
            {
                $scope.setError('Please select a Payment option.');
                return;
            }

            var deliveryAddress = { Id: 0, AddressLine1: $scope.deliveryAddress.AddressLine1.trim(), AddressLine2: $scope.deliveryAddress.AddressLine2.trim(), CityId: $scope.deliveryAddress.City.StoreCityId, CustomerId: $rootScope.ShoppingCart.CustomerId, CustomerIpAddress: $rootScope.ShoppingCart.CustomerIpAddress, PaymentTypeId: $scope.deliveryAddress.PaymentTypeId, ShoppingCartId: $rootScope.ShoppingCart.ShoppingCartId, CouponCode: $scope.deliveryAddress.CouponCode };
            
            welcomeServices.processCartCheckout(deliveryAddress, $scope.processShoppingCartItemCompleted);
            
        };

        $scope.processCartCheckoutCompleted = function (response)
        {
            if (response.Code < 1)
            {
                $scope.setError(response.Error);
                return;
            }

            $scope.setSuccessFeedback(response.Error);
            setTimeout(function ()
            {
                $location.path('/Store/Welcome/Welcome');
            }, 900);
        };

        $scope.saveAddress = function (address)
        {

            if (address == null || address.StoreCountryId < 1 || address.StoreStateId < 1 || address.StoreCityId < 1 || address.AddressLine1.trim() < 1)
            {
                $scope.setError('The selected address could not be processed. Please try again later.');
                return;
            }
            $scope.deliveryAddress = address;
        };
        
        $scope.setError = function (errorMessage)
        {
            $scope.error = errorMessage;
            $scope.negativefeedback = true;
            $scope.success = '';
            $scope.positivefeedback = false;
        };

        $scope.clearError = function ()
        {
            $scope.error = '';
            $scope.negativefeedback = false;
            $scope.success = '';
            $scope.positivefeedback = false;
        };

        $scope.setSuccessFeedback = function (successMessage)
        {
            $scope.error = '';
            $scope.negativefeedback = false;
            $scope.success = successMessage;
            $scope.positivefeedback = true;
        };

        $scope.getCoupon = function ()
        {
            if ($scope.deliveryAddress.CouponCode || $scope.deliveryAddress.CouponCode < 1)
            {
                return;
            }
            welcomeServices.getCoupon($scope.deliveryAddress.CouponCode, $scope.getCouponCompleted);
        }; 

        $scope.getCouponCompleted = function (response)
        {
            if (response == null || response.CouponId < 1)
            {
                $scope.setError("The coupon could not be found or it's validity has expired.");
                return;
            }

            if ($rootScope.cartItemsCount > response.MinimumOrderAmount)
            {
                var qty = $rootScope.cartItemsCount - response.MinimumOrderAmount;
                $scope.setError("The minimum order quantity for this coupon is not yet met. Add more " + qty + " items to be able to use this coupon.");
                return;
            }

            if ($rootScope.cartTotalAmount > response.MinimumOrderAmount)
            {
                var amount = $rootScope.cartTotalAmount - response.MinimumOrderAmount;
                $scope.setError("Your order value is " + $rootScope.ShoppingCart.ShopingCartItemObjects[0].CurencySymbol + amount + " less the minimum value for this coupon. Add more items to be able to use this coupon.");
                return;
            }

            var actualAmount = (response.PercentageDeduction * $rootScope.cartTotalAmount) / 100;

            $rootScope.cartTotalAmount = actualAmount;
            $scope.setSuccessFeedback("Congratulations! Your coupon has been successfull applied.");
            return;
        };
    }]);

});




