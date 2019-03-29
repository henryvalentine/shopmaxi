using System;
using System.Collections.Generic;
using System.Linq;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using Shopkeeper.Repositories.Utilities;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class SubscriptionPricingServices
	{
		private readonly SubscriptionPricingRepository  _subscriptionPricingRepository;
        public SubscriptionPricingServices()
		{
            _subscriptionPricingRepository = new SubscriptionPricingRepository();
		}

        public long AddSubscriptionPricing(SubscriptionPricingObject subscriptionPricing)
		{
			try
			{
                return _subscriptionPricingRepository.AddSubscriptionPricing(subscriptionPricing);
			}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _subscriptionPricingRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSubscriptionPricing(SubscriptionPricingObject subscriptionPricing)
		{
			try
			{
                return _subscriptionPricingRepository.UpdateSubscriptionPricing(subscriptionPricing);
            }
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteSubscriptionPricing(long subscriptionPricingId)
		{
			try
			{
                return _subscriptionPricingRepository.DeleteSubscriptionPricing(subscriptionPricingId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public SubscriptionPricingObject GetSubscriptionPricing(long subscriptionPricingId)
		{
			try
			{
                return _subscriptionPricingRepository.GetSubscriptionPricing(subscriptionPricingId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new SubscriptionPricingObject();
			}
		}

        public List<SubscriptionPricingObject> GetSubscriptionPricings()
		{
			try
			{
                var objList = _subscriptionPricingRepository.GetSubscriptionPricings();
                if (objList == null || !objList.Any())
			    {
                    return new List<SubscriptionPricingObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
			}
		}

        public List<SubscriptionPricingObject> GetSubscriptionPricingObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _subscriptionPricingRepository.GetSubscriptionPricingObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
            }
        }
        
        public List<SubscriptionPricingObject> GetSubscriptionPricingBillingCycle(int? itemsPerPage, int? pageNumber, long billingcycleId)
        {
            try
            {
                var objList = _subscriptionPricingRepository.GetSubscriptionPricingBillingCycle(itemsPerPage, pageNumber, billingcycleId);
                if (objList == null || !objList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
            }
        }
        public List<SubscriptionPricingObject> GetSubscriptionPricingSubscriptionPackage(int? itemsPerPage, int? pageNumber, long subscriptionPackageId)
        {
            try
            {
                var objList = _subscriptionPricingRepository.GetSubscriptionPricingSubscriptionPackage(itemsPerPage, pageNumber, subscriptionPackageId);
                if (objList == null || !objList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
            }
        }
	}

}
