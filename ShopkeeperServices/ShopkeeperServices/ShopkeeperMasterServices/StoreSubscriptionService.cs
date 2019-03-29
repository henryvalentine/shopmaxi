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
	public class StoreSubscriptionServices
	{
		private readonly StoreSubscriptionHistoryRepository  _storeSubscriptionRepository;
        public StoreSubscriptionServices()
		{
            _storeSubscriptionRepository = new StoreSubscriptionHistoryRepository();
		}

        public long AddStoreSubscription(StoreSubscriptionHistoryObject storeSubscription)
		{
			try
			{
                return _storeSubscriptionRepository.AddStoreSubscriptionHistory(storeSubscription);
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
                return _storeSubscriptionRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreSubscription(StoreSubscriptionHistoryObject storeSubscription)
		{
			try
			{
                return _storeSubscriptionRepository.UpdateStoreSubscriptionHistory(storeSubscription);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreSubscription(long storeSubscriptionId)
		{
			try
			{
                return _storeSubscriptionRepository.DeleteStoreSubscriptionHistory(storeSubscriptionId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreSubscriptionHistoryObject GetStoreSubscription(long storeSubscriptionId)
		{
			try
			{
                return _storeSubscriptionRepository.GetStoreSubscriptionHistory(storeSubscriptionId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSubscriptionHistoryObject();
			}
		}

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptions()
		{
			try
			{
                var objList = _storeSubscriptionRepository.GetStoreSubscriptions();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreSubscriptionHistoryObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
			}
		}

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeSubscriptionRepository.GetStoreSubscriptionHistoryObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreSubscriptionHistoryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
            }
        }
        
        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptionsBySubscriptionPackage(int? itemsPerPage, int? pageNumber, int subscriptionPackageId)
            
        {
            try
            {
                var objList = _storeSubscriptionRepository.GetStoreSubscriptionsBySubscriptionPackage(itemsPerPage, pageNumber, subscriptionPackageId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreSubscriptionHistoryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
            }
        }

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptionsByStore(int? itemsPerPage, int? pageNumber, int storeId)
        {
            try
            {
                var objList = _storeSubscriptionRepository.GetStoreSubscriptionsByStore(itemsPerPage, pageNumber, storeId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreSubscriptionHistoryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
            }
        }
	}

}
