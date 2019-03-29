"use strict";

define(['application-configuration', 'welcomeServices', 'ngDialog'], function (app)
{
    app.register.controller('itemInfoController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'welcomeServices', '$location',
    function (ngDialog, $scope, $rootScope, $routeParams, welcomeServices, $location)
    {
        $scope.getDefaults = function ()
        {
            var id = parseInt($routeParams.id);
            if (id == null || id < 1)
            {
                $location.path('/Store/Welcome/Welcome');
            }
            else
            {
                $scope.selectedItem =
                {
                    Name: $routeParams.nm,
                    StoreItemBrandName: $routeParams.bn, StoreItemCategoryName: $routeParams.cn,
                    StoreItemTypeName: $routeParams.tn, Description: $routeParams.ds, StoreItemBrandId: $routeParams.bid, StoreItemTypeId: $routeParams.sid,
                    StoreItemCategoryId: $routeParams.scid
                };
                welcomeServices.getItemDetails(id, $scope.getItemDetailsCompleted);
            }
        };

        $scope.getItemDetailsCompleted = function (data)
        {
            if (data.StoreItemStockId < 1)
            {
                return;
            }

            $scope.stockItem = data;
            $scope.stockItem.Name = $scope.selectedItem.Name;
            $scope.stockItem.QuantityOrdered = 1;
            $scope.stockItem.StoreItemBrandId = $scope.selectedItem.StoreItemBrandId;
            $scope.stockItem.StoreItemTypeId = $scope.selectedItem.StoreItemTypeId;
            $scope.stockItem.StoreItemCategoryId = $scope.selectedItem.StoreItemCategoryId;

            $scope.stockItem.Description = $scope.selectedItem.Description;
            $scope.stockItem.StoreItemBrandName = $scope.selectedItem.StoreItemBrandName;

            $scope.stockItem.StoreItemTypeName = $scope.selectedItem.StoreItemTypeName;
            $scope.stockItem.StoreItemCategoryName = $scope.selectedItem.StoreItemCategoryName;
            $scope.stockItem.ItemVariationObjects = [];
            $scope.details = true;
            welcomeServices.getItemVariations(data.StoreItemId, $scope.getItemVariationsCompleted);
            
        };

        $scope.getItemVariationsCompleted = function (data)
        {
            $scope.variations = data;
        };
        
        $scope.setItemVariant = function (variant, variantValue)
        {
            if (variant == null || variantValue == null || variantValue.StoreItemVariationValueId < 1)
            {
                angular.forEach($scope.stockItem.ItemVariationObjects, function (v, i)
                {
                    if (v.StoreItemVariationId === variant.StoreItemVariationId)
                    {
                        $scope.stockItem.ItemVariationObjects.splice(i, 1);
                    }
                });
                return;
            }

            var existing = $scope.stockItem.ItemVariationObjects.filter(function (item)
            {
                return item.StoreItemVariationId === variant.StoreItemVariationId && item.StoreItemVariationValueId === variantValue.StoreItemVariationValueId;
            });
            
            if (existing === null || existing === undefined || existing.length < 1)
            {
                $scope.stockItem.ItemVariationObjects.push({ StoreItemVariationName: variant.StoreItemVariationName, StoreItemVariationValueName: variantValue.StoreItemVariationValueName, StoreItemVariationId: variant.StoreItemVariationId, StoreItemVariationValueId: variantValue.StoreItemVariationValueId });
            }
           
        };
        
        $scope.addToCart = function ()
        {
            if ($scope.stockItem == null || $scope.stockItem.StoreItemStockId < 1)
            {
                return;
            }

            if ($scope.stockItem.ItemVariationObjects == null || $scope.stockItem.ItemVariationObjects.length < 1)
            {
                var msg = '';
                angular.forEach($scope.variations, function (v, i)
                {
                    msg += ', ' + v.StoreItemVariation;
                });
                $scope.setError('Please select' + msg);
                return;
            }

            var unselectedVariants = [];
            angular.forEach($scope.variations, function (v, i)
            {
                var existings = $scope.stockItem.ItemVariationObjects.filter(function (item)
                {
                    return item.StoreItemVariationId === v.StoreItemVariationId;
                });

                if (existings.length < 1)
                {
                    unselectedVariants.push(v);
                }
            });

            if (unselectedVariants.length > 0)
            {
                var mssg = '';
                angular.forEach(unselectedVariants, function (h, i)
                {
                    msg += ', ' + h.StoreItemVariation;
                });
                $scope.setError('Please select' + mssg);
                return;
            }

            if ($scope.stockItem.QuantitySold < 1)
            {
                $scope.setError('Please provide a valid Quantity');
                return;
            }

            var cart = {};

            if ($rootScope.ShoppingCart === undefined || $rootScope.ShoppingCart == null)
            {
                var ip = $rootScope.ip.length > 0 ? $rootScope.ip : '';
                cart = $rootScope.ShoppingCart = { ShoppingCartId: 0, CustomerId: null, CustomerIpAddress: ip, DeliveryStatus: 1, DeliveryAddressId : null, ShopingCartItemObjects: [] };
            } 
            else
            {
                cart = $rootScope.ShoppingCart;
            }

            var cartItem =
                        {
                            ShopingCartItemId: 0, ShopingCartId: cart.ShoppingCartId, StoreItemStockId: $scope.stockItem.StoreItemStockId,
                            UnitPrice: $scope.stockItem.Price, QuantityOrdered: $scope.stockItem.QuantityOrdered, UoMId: $scope.stockItem.UnitOfMeasurementId,
                            Discount: $scope.stockItem.Discount, UoMCode: $scope.stockItem.UoMCode
                        };
            
            
            if (cart.ShopingCartItemObjects.length > 0)
            {
                var matchId = 0;
                var matchFound = false;
                angular.forEach(cart.ShopingCartItemObjects, function (x, i)
                {
                    if (x.StoreItemStockId === $scope.stockItem.StoreItemStockId)
                    {
                        x.QuantityOrdered += parseFloat($scope.stockItem.QuantityOrdered);
                        matchFound = true;
                        matchId = x.ShopingCartItemId;
                    }
                });

                if (!matchFound) 
                {
                    cart.ShopingCartItemObjects.push(cartItem);
                }
            }
            else 
            {
                cart.ShopingCartItemObjects.push(cartItem);
            }
            
            $scope.processing = false;
            if ($rootScope.ShoppingCart.ShoppingCartId < 1)
            {
                welcomeServices.processShoppingCart(cart, $scope.processShoppingCartCompleted);
            }
            else
            {
                welcomeServices.processShoppingCartItem(cartItem, $scope.processShoppingCartItemCompleted);
            }
        };

        $scope.processShoppingCartCompleted = function (response)
        {
            if (response == null || response.ShoppingCartId < 1 || response.ShopingCartItemObjects.length < 1)
            {
                $scope.setError('Your cart information could not be processed.');
                return;
            }

            var cartItemsCount = 0;
            var cartAmount = 0;
            $rootScope.ShoppingCart = response;
            angular.forEach(response.ShopingCartItemObjects, function (t, k)
            {
                cartItemsCount += t.QuantityOrdered;
                cartAmount += t.UnitPrice * t.QuantityOrdered;
            });

            //Animate Add to cart
            var cartObj = angular.element('.shopping-cart');
            var imgtodrag = angular.element('#zoomer');
            if (imgtodrag) {
                var imgclone = imgtodrag.clone()
                    .offset({
                        top: imgtodrag.offset().top,
                        left: imgtodrag.offset().left
                    })
                    .css({
                        'opacity': '0.5',
                        'position': 'absolute',
                        'height': '150px',
                        'width': '150px',
                        'z-index': '100'
                    })
                    .appendTo(angular.element('body'))
                    .animate({
                        'top': cartObj.offset().top + 10,
                        'left': cartObj.offset().left + 10,
                        'width': 75,
                        'height': 75
                    }, 1000, 'easeInOutExpo');

                setTimeout(function () {
                    cartObj.effect("shake", {
                        times: 2
                    }, 200);
                }, 1500);

                imgclone.animate({
                    'width': 0,
                    'height': 0
                }, function () {
                    angular.element('.add-to-cart').detach();
                });
            }

            $rootScope.cartTotalAmount = cartAmount;
            $rootScope.cartItemsCount = cartItemsCount;
            $scope.processing = false;
        };
        
        $scope.processShoppingCartItemCompleted = function (response)
        {
            if (response < 1)
            {
                $scope.setError('Your cart information could not be processed.');
                return;
            }
            
            var matchFound = false;
            angular.forEach($rootScope.ShoppingCart.ShopingCartItemObjects, function (x, i)
            {
                if (x.StoreItemStockId === $scope.stockItem.StoreItemStockId)
                {
                    x.QuantityOrdered += response.QuantityOrdered;
                    matchFound = true;
                }
            });

            if (!matchFound)
            {
                $rootScope.ShoppingCart.ShopingCartItemObjects.push(response);
            }
            
            var cartItemsCount = 0;
            var cartAmount = 0;
            angular.forEach(response.ShopingCartItemObjects, function (t, k)
            {
                cartItemsCount += t.QuantityOrdered;
                cartAmount += t.UnitPrice * t.QuantityOrdered;
            });

            $rootScope.cartTotalAmount = cartAmount;
            $rootScope.cartItemsCount = cartItemsCount;
        };
        
        $scope.getPrice = function (item)
        {
            var filteredList = [];
            var priceResult = { 'presubtotal': 0, 'rate': 0, 'price': 0 };
            var hd = false;
            angular.forEach($scope.items, function (i, k)
            {
                if (i.StoreItemStockId === item.StoreItemStockId)
                {
                    filteredList.push(i);
                }
            });

            if (filteredList.length < 1)
            {
                $scope.setError('A fatal error was encountered. The requested operation was aborted');
                return priceResult;
            }

            $scope.sortCollection(filteredList);

            angular.forEach(filteredList, function (u, m)
            {
                if (u.MinimumQuantity == item.QuantitySold)
                {
                    priceResult.rate = u.Price;
                    priceResult.presubtotal += priceResult.rate;
                    priceResult.price = u.Price * item.QuantitySold;
                    hd = true;
                }
            });

            if (!hd)
            {
                priceResult.rate = filteredList[filteredList.length - 1].Price;
                priceResult.presubtotal += priceResult.rate;
                priceResult.price = filteredList[filteredList.length - 1].Price * item.QuantitySold;

            }
            return priceResult;
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
        
    }]);

});




