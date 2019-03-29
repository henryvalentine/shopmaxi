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
    public class ItemSoldServices
    {
        private readonly StoreItemSoldRepository _itemSoldRepository;
        public ItemSoldServices()
        {
            _itemSoldRepository = new StoreItemSoldRepository();
        }

        public long AddStoreItemSold(StoreItemSoldObject itemSoldAccount)
        {
            try
            {
                return _itemSoldRepository.AddStoreItemSold(itemSoldAccount);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreItemSold(StoreItemSoldObject itemSold)
        {
            try
            {
                return _itemSoldRepository.UpdateStoreItemSold(itemSold);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemSold(long itemSoldAccountId)
        {
            try
            {
                return _itemSoldRepository.DeleteStoreItemSold(itemSoldAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemSoldObject GetStoreItemSold(long itemSoldAccountId)
        {
            try
            {
                return _itemSoldRepository.GetStoreItemSold(itemSoldAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemSoldObject();
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _itemSoldRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<StoreItemSoldObject> GetStoreItemSolds()
        {
            try
            {
                var objList = _itemSoldRepository.GetStoreItemSolds();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemSoldObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemSoldObject> GetStoreItemSoldObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _itemSoldRepository.GetStoreItemSoldObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemSoldObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemSoldObject> GetStoreItemSoldListByStockItemId(long stockItemId)
        {
            try
            {
                return _itemSoldRepository.GetStoreItemSoldListByStockItemId(stockItemId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemSoldObject> GetStoreItemSoldListByStockItemCategory(int categoryId)
        {
            try
            {
                return _itemSoldRepository.GetStoreItemSoldListByStockItemCategory(categoryId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemSoldObject>();
            }
        }

        public StoreItemSoldObject GetStoreItemSoldByStockItemId(long stockItemId)
        {
            try
            {
                return _itemSoldRepository.GetStoreItemSoldByStockItemId(stockItemId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemSoldObject();
            }
        }
        public List<StoreItemSoldObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _itemSoldRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemSoldObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemSoldObject>();
            }
        }
    }

}
