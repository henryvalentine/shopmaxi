using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class StoreOutletServices
	{
		private readonly StoreOutletRepository  _storeOutletRepository;
        public StoreOutletServices()
		{
            _storeOutletRepository = new StoreOutletRepository();
		}

        public long AddStoreOutlet(StoreOutletObject storeOutlet)
		{
			try
			{
                return _storeOutletRepository.AddStoreOutlet(storeOutlet);
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
                return _storeOutletRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreOutlet(StoreOutletObject storeOutlet)
		{
			try
			{
                return _storeOutletRepository.UpdateStoreOutlet(storeOutlet);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreOutlet(long storeOutletId)
		{
			try
			{
                return _storeOutletRepository.DeleteStoreOutlet(storeOutletId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreOutletObject GetStoreOutlet(long storeOutletId)
		{
			try
			{
                return _storeOutletRepository.GetStoreOutlet(storeOutletId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreOutletObject();
			}
		}

        public List<StoreOutletObject> GetStoreOutlets()
		{
			try
			{
                var objList = _storeOutletRepository.GetStoreOutlets();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreOutletObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreOutletObject>();
			}
		}

        public List<StoreOutletObject> GetStoreOutletObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeOutletRepository.GetStoreOutletObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreOutletObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreOutletObject>();
            }
        }
        
	}

}
