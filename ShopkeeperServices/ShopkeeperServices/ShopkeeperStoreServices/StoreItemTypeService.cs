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
	public class StoreItemTypeServices
	{
		private readonly StoreItemTypeRepository  _storeItemTypeRepository;
        public StoreItemTypeServices()
		{
            _storeItemTypeRepository = new StoreItemTypeRepository();
		}
        public long AddStoreItemType(StoreItemTypeObject storeItemType)
		{
			try
			{
                return _storeItemTypeRepository.AddStoreItemType(storeItemType);
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
                return _storeItemTypeRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreItemType(StoreItemTypeObject storeItemType)
		{
			try
			{
                return _storeItemTypeRepository.UpdateStoreItemType(storeItemType);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreItemType(long storeItemTypeId)
		{
			try
			{
                return _storeItemTypeRepository.DeleteStoreItemType(storeItemTypeId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreItemTypeObject GetStoreItemType(long storeItemTypeId)
		{
			try
			{
                return _storeItemTypeRepository.GetStoreItemType(storeItemTypeId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemTypeObject();
			}
		}
        public List<StoreItemTypeObject> GetStoreItemTypes()
		{
			try
			{
                var objList = _storeItemTypeRepository.GetStoreItemTypes();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreItemTypeObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemTypeObject>();
			}
		}
        public List<StoreItemTypeObject> GetStoreItemTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemTypeRepository.GetStoreItemTypeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemTypeObject>();
            }
        }

        public List<StoreItemTypeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemTypeRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemTypeObject>();
            }
        }
	  }

}
