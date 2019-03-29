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
	public class ItemPriceServices
	{
        private readonly ItemPriceRepository _itemPriceRepository;
        public ItemPriceServices()
		{
            _itemPriceRepository = new ItemPriceRepository();
		}

        public long AddItemPrice(ItemPriceObject itemPriceAccount)
        {
            try
            {
                return _itemPriceRepository.AddItemPrice(itemPriceAccount);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateItemPrice(ItemPriceObject itemPrice)
        {
            return _itemPriceRepository.UpdateItemPrice(itemPrice);
        }

        public List<ItemPriceObject> GetItemPrices(long stockItemId)
        {
            return _itemPriceRepository.GetItemPrices(stockItemId);
        }

        public bool DeleteItemPrice(long itemPriceAccountId)
        {
            try
            {
                return _itemPriceRepository.DeleteItemPrice(itemPriceAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public List<ItemPriceObject> GetItemPrices(string criteria)
        {
            try
            {
                return _itemPriceRepository.GetItemPrices(criteria);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }

        public ItemPriceObject GetItemPrice(long itemPriceAccountId)
        {
            try
            {
                return _itemPriceRepository.GetItemPrice(itemPriceAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ItemPriceObject();
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _itemPriceRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<ItemPriceObject> GetItemPrices()
        {
            try
            {
                var objList = _itemPriceRepository.GetItemPrices();
                if (objList == null || !objList.Any())
                {
                    return new List<ItemPriceObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }

        public List<ItemPriceObject> GetItemPriceObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _itemPriceRepository.GetItemPriceObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<ItemPriceObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }

        public List<ItemPriceObject> GetItemPriceListByStockItemId(long stockItemId)
        {
            try
            {
                return _itemPriceRepository.GetItemPriceListByStockItemId(stockItemId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }

        public List<StoreItemStockObject> GetItemPriceListByOutlet(int outletId, int page, int itemsPerPage)
        {
            return _itemPriceRepository.GetItemPriceListByOutlet(outletId, page, itemsPerPage);
        }

        public List<StoreItemStockObject> SearchItemPriceListByOutlet(int outletId, string criteria)
        {
            return _itemPriceRepository.SearchItemPriceListByOutlet(outletId, criteria);
        }

        public List<StoreItemStockObject> SearchAllItemPriceListByOutlet(int outletId, string criteria)
        {
            return _itemPriceRepository.SearchAllItemPriceListByOutlet(outletId, criteria);
        }


        public List<StoreItemStockObject> GetItemAllPriceListByOutlet(int outletId, int page, int itemsPerPage)
        {
            return _itemPriceRepository.GetAllItemPriceListByOutlet(outletId, page, itemsPerPage);
        }

        public List<StoreItemStockObject> GetItemPriceListForWeb(int page, int itemsPerPage)
        {
            return _itemPriceRepository.GetItemPriceListForWeb(page, itemsPerPage);
        }
        public List<StoreItemStockObject> GetProducts(int page, int itemsPerPage)
        {
            return _itemPriceRepository.GetProducts(page, itemsPerPage);
        }

        public List<ItemPriceObject> GetItemPriceListByStockItemCategory(int categoryId, int outletId)
        {
            try
            {
                return _itemPriceRepository.GetItemPriceListByStockItemCategory(categoryId, outletId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }

        public List<ItemPriceObject> GetItemPriceObjectBySku(string sku, int outletId)
        {
            try
            {
                return _itemPriceRepository.GetItemPriceObjectBySku(sku, outletId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }

        public ItemPriceObject GetItemPriceByStockItemId(long stockItemId)
        {
            try
            {
                return _itemPriceRepository.GetItemPriceByStockItemId(stockItemId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ItemPriceObject();
            }
        }
        public List<ItemPriceObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _itemPriceRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<ItemPriceObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ItemPriceObject>();
            }
        }
	}

}
