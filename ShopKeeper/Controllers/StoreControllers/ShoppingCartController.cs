using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{ 
    public class ShoppingCartController : Controller
	{
        public ShoppingCartController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
       
        //[HttpGet]
        //public ActionResult GetShoppingCartObjects(JQueryDataTableParamModel param)
        //{
           
        //}

        public ActionResult GetItemDetails(long id, string subdomain)
        {
            if (id < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var stockItemObj = new DefaultServices().GetProductDetails(id);

            return Json(stockItemObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClientCartByClientIp(string ip, string subdomain)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ip = ClientIpHelper.GetClientIpAddress(Request);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new ShoppingCartObject(), JsonRequestBehavior.AllowGet);
            }

            var shoppingCartObject = new DefaultServices().GetShoppingCart(ip);

            return Json(shoppingCartObject, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClientCartByIp(string subdomain)
        {
            var ip = ClientIpHelper.GetClientIpAddress(Request);
            if (string.IsNullOrEmpty(ip))
            {
                return Json(new ShoppingCartObject(), JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new ShoppingCartObject(), JsonRequestBehavior.AllowGet);
            }

            var shoppingCartObject = new DefaultServices().GetShoppingCart(ip);

            return Json(shoppingCartObject, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetItemVariations(int storeItemId, string subdomain)
        {
            if (storeItemId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }
            //var ipAddress = ClientIpHelper.GetClientIpAddress(Request);
            var variations = new DefaultServices().GetItemVariations(storeItemId);

            return Json(variations, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult ProcessShoppingCart(ShoppingCartObject cart, string subdomain)
        {
            try
            {
                if (cart == null || !cart.ShopingCartItemObjects.Any())
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

                var store = new SessionHelpers().GetStoreInfo(subdomain);
                if (store == null || store.StoreId < 1)
                {
                    return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(cart.CustomerIpAddress))
                {
                    cart.CustomerIpAddress = ClientIpHelper.GetClientIpAddress(Request);
                }

                cart.DateInitiated = DateTime.Now;
                var processStatus = new DefaultServices().ProcessShoppingCart(cart);

                return Json(processStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProcessShoppingCartItem(ShopingCartItemObject cartItem, string subdomain)
        {
            try
            {
                if (cartItem == null || cartItem.QuantityOrdered < 1)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

                var store = new SessionHelpers().GetStoreInfo(subdomain);
                if (store == null || store.StoreId < 1)
                {
                    return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
                }

                var processStatus = new DefaultServices().ProcessShoppingCartItem(cartItem);

                return Json(processStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RemoveShoppingCartItem(long cartItemId, string subdomain)
        {
            try
            {
                if (cartItemId < 1)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

                var store = new SessionHelpers().GetStoreInfo(subdomain);
                if (store == null || store.StoreId < 1)
                {
                    return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
                }

                var processStatus = new DefaultServices().RemoveShoppingCartItem(cartItemId);

                return Json(processStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteShoppingCart(long cartId, string subdomain)
        {
            if (cartId < 1)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var processStatus = new DefaultServices().DeleteShoppingCart(cartId);

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateShoppingCartItem(CartItemObject cartItemObject, string subdomain)
        {
            try
            {
                if (cartItemObject.ShoppinCartItemId < 1 || cartItemObject.QuantityOrdered < 1)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

                var store = new SessionHelpers().GetStoreInfo(subdomain);
                if (store == null || store.StoreId < 1)
                {
                    return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
                }

                var processStatus = new DefaultServices().UpdateShoppingCartItem(cartItemObject);

                return Json(processStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult GetCountries(string subdomain)
        {
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var processStatus = new DefaultServices().GetCountries();

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCountryStates(long countryId, string subdomain)
        {
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var processStatus = new DefaultServices().GetCountryStates(countryId);

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStateCities(long stateId, string subdomain)
        {
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }

            var processStatus = new DefaultServices().GetStateCities(stateId);

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProcessCartCheckout(DeliveryAddressObject deliveryAddress, string subdomain)
        {
            var gVal = new GenericValidator();
            try
            {
                var store = new SessionHelpers().GetStoreInfo(subdomain);
                if (store == null || store.StoreId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be completed. Please try again later.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var validationStatus = ValidateCartCheckout(deliveryAddress);
                if (validationStatus.Code < 1)
                {
                    return Json(validationStatus, JsonRequestBehavior.AllowGet);
                }

                var processStatus = new DefaultServices().ProcessCartCheckout(deliveryAddress);
                if (processStatus < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be completed. Please try again later.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = 5;
                gVal.Error = "Your request was successfully completed.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                gVal.Code = -1;
                gVal.Error = "Your request could not be completed. Please try again later.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAddressesByIp(string ip, string subdomain)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ip = ClientIpHelper.GetClientIpAddress(Request);
            }

            if (string.IsNullOrEmpty(ip))
            {
                return Json(new List<DeliveryAddressObject>(), JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new List<DeliveryAddressObject>(), JsonRequestBehavior.AllowGet);
            }

            var addresses = new DefaultServices().GetCustomerPreviousAddresses(ip);
            return Json(addresses, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCouponInfo(string couponCode, string subdomain)
        {

            if (string.IsNullOrEmpty(couponCode))
            {
                return Json(new CouponObject(), JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new CouponObject(), JsonRequestBehavior.AllowGet);
            }

            var couponObject = new DefaultServices().GetCouponInfo(couponCode);
            return Json(couponObject, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAddressesById(long customerId, string subdomain)
        {
            if (customerId < 1)
            {
                return Json(new List<DeliveryAddressObject>(), JsonRequestBehavior.AllowGet);
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new List<DeliveryAddressObject>(), JsonRequestBehavior.AllowGet);
            }

            var addresses = new DefaultServices().GetCustomerPreviousAddresses(customerId);
            return Json(addresses, JsonRequestBehavior.AllowGet);
        }

        #endregion
       

        #region Helpers
        private GenericValidator ValidateCartCheckout(DeliveryAddressObject deliveryAddress)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(deliveryAddress.AddressLine1.Trim()))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Address Line 1.";
                    return gVal;
                }

                if (string.IsNullOrEmpty(deliveryAddress.ContactEmail.Trim()))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide your email address.";
                    return gVal;
                }

                if (deliveryAddress.ShoppingCartId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be completed. Please try again later.";
                    return gVal;
                }

                if (deliveryAddress.PaymentTypeId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a Payment option.";
                    return gVal;
                }

                if (deliveryAddress.CityId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a City.";
                    return gVal;
                }

                if (string.IsNullOrEmpty(deliveryAddress.CustomerIpAddress.Trim()) && deliveryAddress.CustomerId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be completed. Please try again later.";
                    return gVal;
                }

                gVal.Code = 5;
                gVal.Error = "Validation succeeded.";
                return gVal;
            }
            catch (Exception)
            {
                gVal.Code = -1;
                gVal.Error = "Your request could not be completed. Please try again later.";
                return gVal;
            }
        }
        #endregion

    }
}

