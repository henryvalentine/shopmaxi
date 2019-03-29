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
	public class SubscriptionPackageServices
	{
		private readonly SubscriptionPackageRepository  _subscriptionPackageRepository;
        public SubscriptionPackageServices()
		{
            _subscriptionPackageRepository = new SubscriptionPackageRepository();
		}

        public long AddSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
		{
			try
			{
                return _subscriptionPackageRepository.AddSubscriptionPackage(subscriptionPackage);
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
                return _subscriptionPackageRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
		{
			try
			{
                return _subscriptionPackageRepository.UpdateSubscriptionPackage(subscriptionPackage);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteSubscriptionPackage(long subscriptionPackageId)
		{
			try
			{
                return _subscriptionPackageRepository.DeleteSubscriptionPackage(subscriptionPackageId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public SubscriptionPackageObject GetSubscriptionPackage(long subscriptionPackageId)
		{
			try
			{
                return _subscriptionPackageRepository.GetSubscriptionPackage(subscriptionPackageId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SubscriptionPackageObject();
			}
		}

        public List<SubscriptionPackageObject> GetSubscriptionPackages()
		{
			try
			{
                var objList = _subscriptionPackageRepository.GetSubscriptionPackages();
                if (objList == null || !objList.Any())
			    {
                    return new List<SubscriptionPackageObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPackageObject>();
			}
		}

        public List<SubscriptionPackageObject> GetSubscriptionPackageObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _subscriptionPackageRepository.GetSubscriptionPackageObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SubscriptionPackageObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPackageObject>();
            }
        }
        
        public List<SubscriptionPackageObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _subscriptionPackageRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<SubscriptionPackageObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPackageObject>();
            }
        }
	}

}
