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
    public class StoreItemStockServices
    {
        private readonly StoreItemStockRepository _storeItemStockRepository;
        public StoreItemStockServices()
        {
            _storeItemStockRepository = new StoreItemStockRepository();
        }

        public long AddStoreItemStock(StoreItemStockObject storeItemStock)
        {
            try
            {
                return _storeItemStockRepository.AddStoreItemStock(storeItemStock);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int GetObjectCountByBrand(int brandId)
        {
            try
            {
                return _storeItemStockRepository.GetObjectCountByBrand(brandId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int GetObjectCountByOutlet(int outletId)
        {
            try
            {
                return _storeItemStockRepository.GetObjectCountByOutlet(outletId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int64 UpdateStoreItemStock(StoreItemStockObject storeItemStock)
        {
            return _storeItemStockRepository.UpdateStoreItemStock(storeItemStock);
        }

        public StoreItemSoldObject UpdateStoreItemStock(StoreItemSoldObject itemSold, out double remainingStockVolume)
        {
            return _storeItemStockRepository.UpdateStoreItemStock(itemSold, out remainingStockVolume);
        }

        public bool UpdateSoldItemDelivery(List<StoreItemSoldObject> soldItems)
        {
            return _storeItemStockRepository.UpdateSoldItemDelivery(soldItems);
        }

        public long ReturnItemSold(StoreItemSoldObject itemSold)
        {
            return _storeItemStockRepository.ReturnItemSold(itemSold);
        }

        public bool DeleteStoreItemStock(long storeItemStock)
        {
            try
            {
                return _storeItemStockRepository.DeleteStoreItemStock(storeItemStock);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemStockObject GetStoreItemStock(long storeItemStock)
        {
            try
            {
                return _storeItemStockRepository.GetStoreItemStock(storeItemStock);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemStockObject();
            }
        }

        public StoreItemStockObject GetStoreItemStockDetails(long storeItemStockId)
        {
            try
            {
                return _storeItemStockRepository.GetStoreItemStockDetails(storeItemStockId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemStockObject();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocks()
        {
            try
            {
                var objList = _storeItemStockRepository.GetStoreItemStocks();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }
        public List<StoreItemStockObject> GetStoreItemStockObjects()
        {
            try
            {
                var objList = _storeItemStockRepository.GetStoreItemStockObjects();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public StoreCurrencyObject GetStoreDefaultCurrency()
        {
            return _storeItemStockRepository.GetStoreDefaultCurrency();
        }

        public StoreOutletObject GetStoreDefaultOutlet()
        {
            return _storeItemStockRepository.GetStoreDefaultOutlet();
        }

        public List<StoreItemStockObject> GetInventoriesByCategory(int categoryId, int outletId)
        {
            try
            {
                var objList = _storeItemStockRepository.GetInventoriesByCategory(categoryId, outletId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStockObjects(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return _storeItemStockRepository.GetStoreItemStockObjects(itemsPerPage, pageNumber, out countG);
        }

        public List<StoreItemStockObject> GetInventoryObjects(int outletId)
        {
            try
            {
                var objList = _storeItemStockRepository.GetInventoryObjects(outletId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocksByStoreItem(int? itemsPerPage, int? pageNumber, int storeItemId)
        {
            try
            {
                var objList = _storeItemStockRepository.GetStoreItemStocksByStoreItem(itemsPerPage, pageNumber, storeItemId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }
        public List<StoreItemStockObject> GetStoreItemStocksByStoreOutlet(int? itemsPerPage, int? pageNumber, int storeOutletId)
        {
            try
            {
                var objList = _storeItemStockRepository.GetStoreItemStocksByStoreOutlet(itemsPerPage, pageNumber, storeOutletId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocksByStoreItemVariation(int? itemsPerPage, int? pageNumber, int storeItemVariationId)
        {
            try
            {
                var objList = _storeItemStockRepository.GetStoreItemStocksByStoreItemVariation(itemsPerPage, pageNumber, storeItemVariationId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocksByStoreItemVariationValue(int? itemsPerPage, int? pageNumber, int storeItemVariationValueId)
        {
            try
            {
                var objList = _storeItemStockRepository.GetStoreItemStocksByStoreItemVariationValue(itemsPerPage, pageNumber, storeItemVariationValueId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemStockRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }
    }

}
