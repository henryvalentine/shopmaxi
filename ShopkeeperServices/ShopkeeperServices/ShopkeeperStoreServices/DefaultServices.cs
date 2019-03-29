using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class DefaultServices
	{
        private readonly DefaultsRepository _defaultsRepository;
        public DefaultServices()
		{
            _defaultsRepository = new DefaultsRepository();
		}

        public OnlineStoreObject GetDefaults()
        {
          return _defaultsRepository.GetDefaults();
        }

        public StoreItemStockObject GetProductDetails(long id)
        {
            return _defaultsRepository.GetProductDetails(id);
        }

        public List<VariationObject> GetItemVariations(long storeItemId)
        {
            return _defaultsRepository.GetItemVariations(storeItemId);
        }
        
        public ShoppingCartObject GetShoppingCart(string customerIpAddress)
        {
            return _defaultsRepository.GetShoppingCart(customerIpAddress);
        }

        public ShoppingCartObject GetShoppingCart(long customerId)
        {
            return _defaultsRepository.GetShoppingCart(customerId);
        }

        public ShoppingCartObject ProcessShoppingCart(ShoppingCartObject cart)
        {
            return _defaultsRepository.ProcessShoppingCart(cart);
        }

        public List<DeliveryAddressObject> GetCustomerPreviousAddresses(long customerId)
        {
            return _defaultsRepository.GetCustomerPreviousAddresses(customerId);
        }
        public List<DeliveryAddressObject> GetCustomerPreviousAddresses(string customerIpAddress)
        {
            return _defaultsRepository.GetCustomerPreviousAddresses(customerIpAddress);
        }

        public ShopingCartItemObject ProcessShoppingCartItem(ShopingCartItemObject cartItem)
        {
            return _defaultsRepository.ProcessShoppingCartItem(cartItem);
        }
        public long RemoveShoppingCartItem(long cartItemId)
        {
            return _defaultsRepository.RemoveShoppingCartItem(cartItemId);
        }
        public long DeleteShoppingCart(long cartId)
        {
            return _defaultsRepository.DeleteShoppingCart(cartId);
        }

        public long UpdateShoppingCartItem(CartItemObject cartItemObject)
        {
            return _defaultsRepository.UpdateShoppingCartItem(cartItemObject);
        }

        public List<StoreCountryObject> GetCountries()
        {
            return _defaultsRepository.GetCountries();
        }
        public List<StoreStateObject> GetCountryStates(long countryId)
        {
            return _defaultsRepository.GetCountryStates(countryId);
        }
        public List<StoreCityObject> GetStateCities(long stateId)
        {
            return _defaultsRepository.GetStateCities(stateId);
        }

        public long ProcessCartCheckout(DeliveryAddressObject deliveryAddress)
        {
            return _defaultsRepository.ProcessCartCheckout(deliveryAddress);
        }

        public UserProfileObject GetUserProfile(string aspnetId)
        {
            return _defaultsRepository.GetUserProfile(aspnetId);
        }

        public CouponObject GetCouponInfo(string couponCode)
        {
            return _defaultsRepository.GetCouponInfo(couponCode);
        }
	}

}


