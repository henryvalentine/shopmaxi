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
    public class StoreItemServices
    {
        private readonly StoreItemRepository _storeItemRepository;
        public StoreItemServices()
        {
            _storeItemRepository = new StoreItemRepository();
        }
        public long AddStoreItem(StoreItemObject storeItem)
        {
            try
            {
                return _storeItemRepository.AddStoreItem(storeItem);
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
                return _storeItemRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreItem(StoreItemObject storeItem)
        {
            try
            {
                return _storeItemRepository.UpdateStoreItem(storeItem);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }
        public bool DeleteStoreItem(long storeItemId)
        {
            try
            {
                return _storeItemRepository.DeleteStoreItem(storeItemId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        public StoreItemObject GetStoreItem(long storeItemId)
        {
            try
            {
                return _storeItemRepository.GetStoreItem(storeItemId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemObject();
            }
        }
        public List<StoreItemObject> GetStoreItems()
        {
            try
            {
                var objList = _storeItemRepository.GetStoreItems();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemObject>();
            }
        }
        public List<StoreItemObject> GetStoreItemObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemRepository.GetStoreItemObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemObject>();
            }
        }

        public List<StoreItemObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemObject>();
            }
        }
    }

}
