using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class PackagePricingServices
	{
		private readonly PackagePricingRepository  _packagePricingRepository;
        public PackagePricingServices()
		{
            _packagePricingRepository = new PackagePricingRepository();
		}

        public long AddPackagePricing(PackagePricingObject packagePricing)
		{
			try
			{
                return _packagePricingRepository.AddPackagePricing(packagePricing);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _packagePricingRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdatePackagePricing(PackagePricingObject packagePricing)
		{
			try
			{
                return _packagePricingRepository.UpdatePackagePricing(packagePricing);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeletePackagePricing(long packagePricingId)
		{
			try
			{
                return _packagePricingRepository.DeletePackagePricing(packagePricingId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public PackagePricingObject GetPackagePricing(long packagePricingId)
		{
			try
			{
                return _packagePricingRepository.GetPackagePricing(packagePricingId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PackagePricingObject();
			}
		}

        public PackagePricingObject GetPackagePricing(long packagePricingId, long billingCycleId)
        {
            try
            {
                return _packagePricingRepository.GetPackagePricing(packagePricingId, billingCycleId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PackagePricingObject();
            }
        }

        public List<PackagePricingObject> GetPackagePricings()
		{
			try
			{
                var objList = _packagePricingRepository.GetPackagePricings();
                if (objList == null || !objList.Any())
			    {
                    return new List<PackagePricingObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
			}
		}

        public List<PackagePricingObject> GetPackagePricingObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _packagePricingRepository.GetPackagePricingObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
            }
        }

        public List<PackagePricingObject> GetPackagePricingBillingCycle(int? itemsPerPage, int? pageNumber, long billingcycleId)
        {
            try
            {
                var objList = _packagePricingRepository.GetPackagePricingBillingCycle(itemsPerPage, pageNumber, billingcycleId);
                if (objList == null || !objList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
            }
       }
        public List<PackagePricingObject> GetPackagePricingSubscriptionPackage(int? itemsPerPage, int? pageNumber, long subscriptionPackageId)
        {
            try
            {
                var objList = _packagePricingRepository.GetPackagePricingSubscriptionPackage(itemsPerPage, pageNumber, subscriptionPackageId);
                if (objList == null || !objList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
            }
         }
	  }

}
