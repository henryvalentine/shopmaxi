using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreItemStockRepository
    {
        private readonly IShopkeeperRepository<StoreItemStock> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _db;
        public StoreItemStockRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }

            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
            _repository = new ShopkeeperRepository<StoreItemStock>(_uoWork);
        }

        public long AddStoreItemStock(StoreItemStockObject storeItemStock)
        {
            try
            {
                using (var db = _db)
                {
                    if (storeItemStock == null)
                    {
                        return -2;
                    }
                    int duplicates;
                    var storeItem = storeItemStock.StoreItemObject;
                    if (storeItemStock.StoreItemVariationId > 0)
                    {
                        duplicates = db.StoreItemStocks.Count(m => m.StoreItemVariationValueId == storeItemStock.StoreItemVariationValueId && m.StoreItem.Name.Trim().ToLower() == storeItem.Name.Trim().ToLower() && m.StoreItem.StoreItemBrandId == storeItemStock.StoreItemObject.StoreItemBrandId);
                    }
                    else
                    {
                        duplicates = db.StoreItems.Count(m => m.Name.Trim().ToLower() == storeItem.Name.Trim().ToLower() && m.StoreItemBrandId == storeItem.StoreItemBrandId);
                    }

                    if (duplicates > 0)
                    {
                        return -3;
                    }

                    if (storeItemStock.StoreItemObject == null)
                    {
                        return -2;
                    }

                    var storeItemEntity = ModelCrossMapper.Map<StoreItemObject, StoreItem>(storeItem);
                    if (storeItemEntity == null || string.IsNullOrEmpty(storeItemEntity.Name))
                    {
                        return -2;
                    }

                    var processed = db.StoreItems.Add(storeItemEntity);
                    db.SaveChanges();

                    storeItemStock.StoreItemId = processed.StoreItemId;

                    var storeItemStockEntity = ModelCrossMapper.Map<StoreItemStockObject, StoreItemStock>(storeItemStock);
                    if (storeItemStockEntity == null || storeItemStockEntity.StoreItemId < 1)
                    {
                        return -2;
                    }

                    var returnStatus = db.StoreItemStocks.Add(storeItemStockEntity);
                    db.SaveChanges();
                    return returnStatus.StoreItemStockId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int64 UpdateStoreItemStock(StoreItemStockObject storeItemStock)
        {
            try
            {
                if (storeItemStock == null || storeItemStock.StoreItemObject == null)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    int duplicates;
                    var storeItem = storeItemStock.StoreItemObject;
                    if (storeItemStock.StoreItemVariationId > 0)
                    {
                        duplicates = db.StoreItemStocks.Count(m => m.StoreItemVariationValueId == storeItemStock.StoreItemVariationValueId && m.StoreItem.Name.Trim().ToLower() == storeItem.Name.Trim().ToLower() && m.StoreItem.StoreItemBrandId == storeItemStock.StoreItemObject.StoreItemBrandId && m.StoreItemStockId != storeItemStock.StoreItemStockId);
                    }
                    else
                    {
                        duplicates = db.StoreItems.Count(m => m.Name.Trim().ToLower() == storeItem.Name.Trim().ToLower() && m.StoreItemBrandId == storeItem.StoreItemBrandId && m.StoreItemId != storeItem.StoreItemId);
                    }

                    if (duplicates > 0)
                    {
                        return -3;
                    }

                    if (storeItemStock.StoreItemObject == null)
                    {
                        return -2;
                    }

                    var storeItemEntity = ModelCrossMapper.Map<StoreItemObject, StoreItem>(storeItem);
                    if (storeItemEntity == null || string.IsNullOrEmpty(storeItemEntity.Name))
                    {
                        return -2;
                    }

                    db.Entry(storeItemEntity).State = EntityState.Modified;
                    db.SaveChanges();

                    var storeItemStockEntity = ModelCrossMapper.Map<StoreItemStockObject, StoreItemStock>(storeItemStock);
                    if (storeItemStockEntity == null || storeItemStockEntity.StoreItemStockId < 1)
                    {
                        return -2;
                    }

                    db.StoreItemStocks.Attach(storeItemStockEntity);
                    db.Entry(storeItemStockEntity).State = EntityState.Modified;
                    db.SaveChanges();
                    return storeItemStock.StoreItemStockId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public StoreItemSoldObject UpdateStoreItemStock(StoreItemSoldObject itemSold, out double remainingStockVolume)
        {
            try
            {
                if (itemSold == null)
                {
                    remainingStockVolume = 0;
                    return new StoreItemSoldObject();
                }
                using (var db = _db)
                {

                    var itemSoldEntity = ModelCrossMapper.Map<StoreItemSoldObject, StoreItemSold>(itemSold);
                    if (itemSoldEntity == null || itemSoldEntity.StoreItemStockId < 1)
                    {
                        remainingStockVolume = 0;
                        return new StoreItemSoldObject();
                    }

                    var storeSettings = db.StoreSettings.ToList();
                    if (!storeSettings.Any())
                    {
                        remainingStockVolume = 0;
                        return new StoreItemSoldObject();
                    }

                    //todo: retrieve from web config
                    //var serviceCategory = 3;

                    var remainingVolume = 0.0;
                    var setting = storeSettings[0];
                    var myItems = db.StoreItemStocks.Where(i => i.StoreItemStockId == itemSoldEntity.StoreItemStockId).Include("StoreItem").ToList();
                    if (!myItems.Any())
                    {
                        remainingStockVolume = 0;
                        return new StoreItemSoldObject();
                    }

                    var myItem = myItems[0];
                    var storeItem = myItem.StoreItem;

                    if (itemSold.StoreItemCategoryId > 0 && itemSold.StoreItemCategoryId != storeItem.StoreItemCategoryId)
                    {
                        if (setting.DeductStockAtSalesPoint)
                        {
                            itemSoldEntity.QuantityDelivered = itemSoldEntity.QuantitySold;
                            itemSoldEntity.QuantityBalance = 0;

                            var orderedItems = db.PurchaseOrderItems.Where(i => i.PurchaseOrderItemId == itemSold.PurchaseOrderItemId).ToList();
                            if (orderedItems.Any())
                            {
                                var orderedItem = orderedItems[0];
                                orderedItem.QuantityInStock = orderedItem.QuantityInStock - itemSoldEntity.QuantitySold;
                                db.Entry(orderedItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            myItem.QuantityInStock = myItem.QuantityInStock - itemSoldEntity.QuantitySold;

                            myItem.TotalQuantityAlreadySold += itemSoldEntity.QuantitySold;

                            db.Entry(myItem).State = EntityState.Modified;
                            db.SaveChanges();
                            remainingVolume = myItem.QuantityInStock;
                        }
                        else
                        {
                            remainingVolume = myItem.QuantityInStock;
                            itemSoldEntity.QuantityBalance = itemSoldEntity.QuantitySold;
                        }
                    }

                    var soldItem = db.StoreItemSolds.Add(itemSoldEntity);
                    db.SaveChanges();

                    remainingStockVolume = remainingVolume;
                    itemSold.StoreItemSoldId = soldItem.StoreItemSoldId;

                    return itemSold;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                remainingStockVolume = 0;
                return new StoreItemSoldObject();
            }
        }

        public bool UpdateSoldItemDelivery(List<StoreItemSoldObject> soldItems)
        {
            try
            {
                if (!soldItems.Any())
                {
                    return false;
                }

                var successCount = 0;

                using (var db = _db)
                {
                    soldItems.ForEach(x =>
                    {
                        var itemSoldEntities = db.StoreItemSolds.Where(s => s.StoreItemSoldId == x.StoreItemSoldId).ToList();
                        if (itemSoldEntities.Any())
                        {
                            return;
                        }
                        var itemSoldEntity = itemSoldEntities[0];

                        itemSoldEntity.QuantityDelivered += x.QuantityDelivered;

                        itemSoldEntity.QuantityBalance = itemSoldEntity.QuantitySold - itemSoldEntity.QuantityDelivered;

                        db.Entry(itemSoldEntity).State = EntityState.Modified;
                        db.SaveChanges();
                        successCount += 1;
                    });

                    return successCount == soldItems.Count;
                }
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public long ReturnItemSold(StoreItemSoldObject itemSold)
        {
            try
            {
                if (itemSold == null || itemSold.StoreItemStockId < 1)
                {
                    return -2;
                }
                using (var db = _db)
                {
                    var soldItems = db.StoreItemSolds.Where(e => e.StoreItemStockId == itemSold.StoreItemStockId && e.SaleId == itemSold.SaleId).ToList();
                    if (!soldItems.Any())
                    {
                        return -2;
                    }

                    var myItems = db.StoreItemStocks.Where(i => i.StoreItemStockId == itemSold.StoreItemStockId).ToList();
                    if (!myItems.Any())
                    {
                        return -2;
                    }

                    var myItem = myItems[0];

                    var orderedItems = db.PurchaseOrderItems.Where(i => i.PurchaseOrderItemId == itemSold.PurchaseOrderItemId).ToList();
                    if (orderedItems.Any())
                    {
                        var orderedItem = orderedItems[0];
                        orderedItem.QuantityInStock += itemSold.ReturnedQuantity;
                        db.Entry(orderedItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //update stock accordingly
                    myItem.QuantityInStock += itemSold.ReturnedQuantity;
                    myItem.TotalQuantityAlreadySold -= itemSold.ReturnedQuantity;

                    db.Entry(myItem).State = EntityState.Modified;
                    db.SaveChanges();

                    //deduct quantity of sold stock accordingly
                    var x = soldItems[0];
                    x.QuantityReturned += itemSold.ReturnedQuantity;
                    db.Entry(x).State = EntityState.Modified;
                    db.SaveChanges();


                    return itemSold.StoreItemStockId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemStock(long storeItemStockId)
        {
            try
            {
                _repository.Remove(storeItemStockId);
                _uoWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemStockObject GetStoreItemStock(long storeItemStockId)
        {
            try
            {
                var myItem = _repository.GetById(storeItemStockId);
                if (myItem == null || myItem.StoreItemStockId < 1)
                {
                    return new StoreItemStockObject();
                }
                var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(myItem);
                if (storeItemStockObject == null || storeItemStockObject.StoreItemStockId < 1)
                {
                    return new StoreItemStockObject();
                }
                return storeItemStockObject;
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
                using (var db = _db)
                {
                    var myItemList = db.StoreItemStocks.Where(m => m.StoreItemStockId == storeItemStockId)
                        .Include("StoreCurrency")
                        .Include("StoreItem")
                        .Include("StoreItemVariationValue")
                        .Include("StoreItemVariation")
                        .Include("StockUploads")
                        .ToList();

                    if (!myItemList.Any())
                    {
                        return new StoreItemStockObject();
                    }
                    var myItem = myItemList[0];
                    var storeItem = myItem.StoreItem;

                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(myItem);
                    if (storeItemStockObject == null || storeItemStockObject.StoreItemStockId < 1)
                    {
                        return new StoreItemStockObject();
                    }

                    if (storeItemStockObject.StoreItemVariationId != null && storeItemStockObject.StoreItemVariationId > 0)
                    {
                        storeItemStockObject.VariationProperty = myItem.StoreItemVariation.VariationProperty;
                        storeItemStockObject.VariationValue = myItem.StoreItemVariationValue.Value;
                    }

                    var storeItemObject = ModelCrossMapper.Map<StoreItem, StoreItemObject>(storeItem);
                    if (storeItemObject == null || storeItemObject.StoreItemId < 1)
                    {
                        return new StoreItemStockObject();
                    }

                    storeItemStockObject.StoreItemObject = storeItemObject;

                    if (myItem.StockUploads.Any())
                    {
                        storeItemStockObject.StockUploadObjects = new List<StockUploadObject>();
                        var imgList = myItem.StockUploads.ToList();
                        imgList.ForEach(x =>
                        {
                            var imgObject = ModelCrossMapper.Map<StockUpload, StockUploadObject>(x);
                            if (imgObject != null && imgObject.StockUploadId > 0)
                            {
                                imgObject.ImagePath = x.ImagePath.Replace("~", string.Empty);
                                storeItemStockObject.StockUploadObjects.Add(imgObject);
                            }

                        });
                    }

                    if (storeItemStockObject.ExpirationDate != null)
                    {
                        storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("dd/MM/yyyy");
                    }
                    return storeItemStockObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemStockObject();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStockObjects(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var storeItemStockList =
                            (from stk in db.StoreItemStocks
                             join cr in db.StoreCurrencies on stk.StoreCurrencyId equals cr.StoreCurrencyId
                             join st in db.StoreItems.OrderBy(h => h.Name).Skip(tpageNumber).Take(tsize) on stk.StoreItemId equals st.StoreItemId
                             join br in db.StoreItemBrands on st.StoreItemBrandId equals br.StoreItemBrandId
                             join tp in db.StoreItemTypes on st.StoreItemTypeId equals tp.StoreItemTypeId
                             join ct in db.StoreItemCategories on st.StoreItemCategoryId equals ct.StoreItemCategoryId

                             select new StoreItemStockObject
                             {
                                 StoreItemStockId = stk.StoreItemStockId,
                                 StoreItemName = st.Name,
                                 BrandName = br.Name,
                                 TypeName = tp.Name,
                                 SKU = stk.SKU,
                                 StoreItemVariationValueId = stk.StoreItemVariationValueId,
                                 CategoryName = ct.Name,
                                 ExpirationDate = stk.ExpirationDate,
                                 QuantityInStock = stk.QuantityInStock,
                                 TotalQuantityAlreadySold = stk.TotalQuantityAlreadySold

                             }).ToList();

                        if (!storeItemStockList.Any())  //.Where(m => m.StoreOutletId == outletId)
                        {
                            countG = 0;
                            return new List<StoreItemStockObject>();
                        }

                        storeItemStockList.ForEach(m =>
                        {
                            var variations = db.StoreItemVariationValues.Where(v => v.StoreItemVariationValueId == m.StoreItemVariationValueId).ToList();
                            if (variations.Any())
                            {
                                var variation = variations[0];
                                m.StoreItemName = m.StoreItemName + "/" + variation.Value;
                            }

                            m.QuantityInStockStr = m.QuantityInStock.ToString("n0");
                            m.QuantitySoldStr = m.TotalQuantityAlreadySold != null ? ((float)(m.TotalQuantityAlreadySold)).ToString("n0") : string.Empty;

                            if (m.ExpirationDate != null)
                            {
                                m.ExpiryDate = ((DateTime)m.ExpirationDate).ToString("dd/MM/yyyy");
                            }

                        });

                        countG = db.StoreItemStocks.Count();
                        return storeItemStockList.OrderBy(h => h.StoreItemName).ToList();
                    }

                }
                countG = 0;
                return new List<StoreItemStockObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public StoreOutletObject GetStoreDefaultOutlet()
        {
            try
            {
                using (var db = _db)
                {

                    var outlets = db.StoreOutlets.Where(s => s.IsMainOutlet).ToList();
                    if (!outlets.Any())
                    {
                        return new StoreOutletObject();
                    }

                    var outlet = outlets[0];
                    var outletObj = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(outlet);
                    if (outletObj == null || outletObj.StoreOutletId < 1)
                    {
                        return new StoreOutletObject();
                    }
                    return outletObj;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreOutletObject();
            }
        }

        public StoreCurrencyObject GetStoreDefaultCurrency()
        {
            try
            {
                using (var db = _db)
                {

                    var settings = db.StoreCurrencies.Where(s => s.IsDefaultCurrency).ToList();
                    if (!settings.Any())
                    {
                        return new StoreCurrencyObject();
                    }

                    var currency = settings[0];
                    var currencyObj = ModelCrossMapper.Map<StoreCurrency, StoreCurrencyObject>(currency);
                    if (currencyObj == null || currencyObj.StoreCurrencyId < 1)
                    {
                        return new StoreCurrencyObject();
                    }
                    return currencyObj;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCurrencyObject();
            }
        }

        public List<StoreItemStockObject> GetInventoryObjects(int outletId)
        {
            try
            {
                var storeItemStockEntityList = _repository.GetAll(m => m.StoreOutletId == outletId, "StoreItem, StoreItemVariationValue, StoreItemVariation").ToList();

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.StoreItemName = m.StoreItem.Name + "/" + m.StoreItemVariationValue.Value;
                        }

                        if (storeItemStockObject.ExpirationDate != null)
                        {
                            storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("yyyy/MM/dd");
                        }
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList;
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
                List<StoreItemStock> storeItemStockEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeItemStockEntityList = _repository.GetWithPaging(m => m.StoreItemId == storeItemId, m => m.StoreItemId, tpageNumber, tsize, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                else
                {
                    storeItemStockEntityList = _repository.GetAll(m => m.StoreItemId == storeItemId, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId != null && m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.VariationProperty = m.StoreItemVariation.VariationProperty;
                            storeItemStockObject.VariationValue = m.StoreItemVariationValue.Value;
                        }
                        storeItemStockObject.StoreItemName = m.StoreItem.Name;
                        storeItemStockObject.StoreOutletName = m.StoreOutlet.OutletName;
                        storeItemStockObject.CurrencyName = m.StoreCurrency.Name;
                        storeItemStockObject.CurrencySymbol = m.StoreCurrency.Symbol;
                        if (storeItemStockObject.ExpirationDate != null)
                        {
                            storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("dd/MM/yyyy");
                        }
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList.OrderBy(h => h.StoreItemName).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocksByStoreOutlet(int? itemsPerPage, int? pageNumber, long storeOutletId)
        {
            try
            {
                List<StoreItemStock> storeItemStockEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeItemStockEntityList = _repository.GetWithPaging(m => m.StoreOutletId == storeOutletId, m => m.StoreItemId, tpageNumber, tsize, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                else
                {
                    storeItemStockEntityList = _repository.GetAll(m => m.StoreOutletId == storeOutletId, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId != null && m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.VariationProperty = m.StoreItemVariation.VariationProperty;
                            storeItemStockObject.VariationValue = m.StoreItemVariationValue.Value;
                        }
                        storeItemStockObject.StoreItemName = m.StoreItem.Name;
                        storeItemStockObject.StoreOutletName = m.StoreOutlet.OutletName;
                        storeItemStockObject.CurrencyName = m.StoreCurrency.Name;
                        storeItemStockObject.CurrencySymbol = m.StoreCurrency.Symbol;
                        if (storeItemStockObject.ExpirationDate != null)
                        {
                            storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("dd/MM/yyyy");
                        }
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList.OrderBy(h => h.StoreItemName).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocksByStoreItemVariation(int? itemsPerPage, int? pageNumber, long storeItemVariationId)
        {
            try
            {
                List<StoreItemStock> storeItemStockEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeItemStockEntityList = _repository.GetWithPaging(m => m.StoreItemVariationId == storeItemVariationId, m => m.StoreItemStockId, tpageNumber, tsize, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                else
                {
                    storeItemStockEntityList = _repository.GetAll(m => m.StoreItemVariationId == storeItemVariationId, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId != null && m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.VariationProperty = m.StoreItemVariation.VariationProperty;
                            storeItemStockObject.VariationValue = m.StoreItemVariationValue.Value;
                        }
                        storeItemStockObject.StoreItemName = m.StoreItem.Name;
                        storeItemStockObject.StoreOutletName = m.StoreOutlet.OutletName;
                        storeItemStockObject.CurrencyName = m.StoreCurrency.Name;
                        storeItemStockObject.CurrencySymbol = m.StoreCurrency.Symbol;
                        if (storeItemStockObject.ExpirationDate != null)
                        {
                            storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("dd/MM/yyyy");
                        }
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList.OrderBy(h => h.StoreItemName).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetStoreItemStocksByStoreItemVariationValue(int? itemsPerPage, int? pageNumber, long storeItemVariationValueId)
        {
            try
            {
                List<StoreItemStock> storeItemStockEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeItemStockEntityList = _repository.GetWithPaging(m => m.StoreItemVariationId == storeItemVariationValueId, m => m.StoreItemStockId, tpageNumber, tsize, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                else
                {
                    storeItemStockEntityList = _repository.GetAll(m => m.StoreItemVariationId == storeItemVariationValueId, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId != null && m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.VariationProperty = m.StoreItemVariation.VariationProperty;
                            storeItemStockObject.VariationValue = m.StoreItemVariationValue.Value;
                        }
                        storeItemStockObject.StoreItemName = m.StoreItem.Name;
                        storeItemStockObject.StoreOutletName = m.StoreOutlet.OutletName;
                        storeItemStockObject.CurrencyName = m.StoreCurrency.Name;
                        storeItemStockObject.CurrencySymbol = m.StoreCurrency.Symbol;
                        if (storeItemStockObject.ExpirationDate != null)
                        {
                            storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("dd/MM/yyyy");
                        }
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList.OrderBy(h => h.StoreItemName).ToList();
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
                using (var db = _db)
                {
                    var lower = searchCriteria.ToLower();
                    var searchList = (
                                from st in db.StoreItems.Where(t => t.Name.ToLower().Contains(lower) || t.StoreItemBrand.Name.ToLower().Contains(lower) || t.StoreItemType.Name.ToLower().Contains(lower) || t.StoreItemCategory.Name.ToLower().Contains(lower))
                                join stk in db.StoreItemStocks.Where(o => o.SKU.ToLower().Contains(lower) || !o.SKU.ToLower().Contains(lower) || o.StoreItemVariationValue.Value.ToLower().Contains(lower) || !o.StoreItemVariationValue.Value.ToLower().Contains(lower)) on st.StoreItemId equals stk.StoreItemId
                                join sv in db.StoreItemVariationValues on stk.StoreItemVariationValueId equals sv.StoreItemVariationValueId
                                join br in db.StoreItemBrands on st.StoreItemBrandId equals br.StoreItemBrandId
                                join tp in db.StoreItemTypes on st.StoreItemTypeId equals tp.StoreItemTypeId
                                join ct in db.StoreItemCategories on st.StoreItemCategoryId equals ct.StoreItemCategoryId

                                select new StoreItemStockObject
                                {
                                    StoreItemStockId = stk.StoreItemStockId,
                                    StoreItemName = st.Name,
                                    BrandName = br.Name,
                                    SKU = stk.SKU,
                                    TypeName = tp.Name,
                                    StoreItemVariationValueId = stk.StoreItemVariationValueId,
                                    CategoryName = ct.Name,
                                    ExpirationDate = stk.ExpirationDate,
                                    QuantityInStock = stk.QuantityInStock,
                                    TotalQuantityAlreadySold = stk.TotalQuantityAlreadySold

                                }).ToList();

                    if (!searchList.Any())  //.Where(m => m.StoreOutletId == outletId)
                    {
                        return new List<StoreItemStockObject>();
                    }

                    searchList.ForEach(m =>
                    {
                        var variations = db.StoreItemVariationValues.Where(v => v.StoreItemVariationValueId == m.StoreItemVariationValueId).ToList();
                        if (variations.Any())
                        {
                            var variation = variations[0];
                            m.StoreItemName = m.StoreItemName + "/" + variation.Value;
                        }

                        m.QuantityInStockStr = m.QuantityInStock.ToString("n0");
                        m.QuantitySoldStr = m.TotalQuantityAlreadySold != null ? ((float)(m.TotalQuantityAlreadySold)).ToString("n0") : string.Empty;

                        if (m.ExpirationDate != null)
                        {
                            m.ExpiryDate = ((DateTime)m.ExpirationDate).ToString("dd/MM/yyyy");
                        }

                    });

                    return searchList.OrderBy(h => h.StoreItemName).ToList();
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public int GetObjectCountByBrand(int brandId)
        {
            try
            {

                return _repository.Count(m => m.StoreItem.StoreItemBrandId == brandId);
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
                return _repository.Count(m => m.StoreOutletId == outletId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public List<StoreItemStockObject> GetStoreItemStocks()
        {
            try
            {
                var storeItemStockEntityList = _repository.GetAll().ToList();
                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });
                return storeItemStockObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        public List<StoreItemStockObject> GetStoreItemStockObjects()
        {
            try
            {

                var storeItemStockEntityList = _repository.GetAll(m => m.StoreItemId > 0, " StoreItem, StoreItemVariationValue").ToList();

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId != null && m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.StoreItemName = m.StoreItem.Name + "/" + m.StoreItemVariationValue.Value;
                        }
                        else
                        {
                            storeItemStockObject.StoreItemName = m.StoreItem.Name;
                        }

                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetInventoriesByCategory(int? itemsPerPage, int? pageNumber, int storeItemId)
        {
            try
            {
                List<StoreItemStock> storeItemStockEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeItemStockEntityList = _repository.GetWithPaging(m => m.StoreItemId == storeItemId, m => m.StoreItemId, tpageNumber, tsize, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                else
                {
                    storeItemStockEntityList = _repository.GetAll(m => m.StoreItemId == storeItemId, "StoreCurrency, StoreItem, StoreOutlet, StoreItemVariationValue, StoreItemVariation").ToList();
                }

                if (!storeItemStockEntityList.Any())
                {
                    return new List<StoreItemStockObject>();
                }
                var storeItemStockObjList = new List<StoreItemStockObject>();
                storeItemStockEntityList.ForEach(m =>
                {
                    var storeItemStockObject = ModelCrossMapper.Map<StoreItemStock, StoreItemStockObject>(m);
                    if (storeItemStockObject != null && storeItemStockObject.StoreItemStockId > 0)
                    {
                        if (m.StoreItemVariationId != null && m.StoreItemVariationId > 0)
                        {
                            storeItemStockObject.VariationProperty = m.StoreItemVariation.VariationProperty;
                            storeItemStockObject.VariationValue = m.StoreItemVariationValue.Value;
                        }
                        storeItemStockObject.StoreItemName = m.StoreItem.Name;
                        storeItemStockObject.StoreOutletName = m.StoreOutlet.OutletName;
                        storeItemStockObject.CurrencyName = m.StoreCurrency.Name;
                        storeItemStockObject.CurrencySymbol = m.StoreCurrency.Symbol;
                        if (storeItemStockObject.ExpirationDate != null)
                        {
                            storeItemStockObject.ExpiryDate = ((DateTime)storeItemStockObject.ExpirationDate).ToString("dd/MM/yyyy");
                        }
                        storeItemStockObjList.Add(storeItemStockObject);
                    }
                });

                return storeItemStockObjList.OrderBy(h => h.StoreItemName).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetInventoriesByCategory(int categoryId, int outletId)
        {
            try
            {
                using (var db = _db)
                {
                    var date = DateTime.Today;
                    var storeItemStockEntityList = (from sc in db.StoreItems.Where(m => m.StoreItemCategoryId == categoryId)
                                                    join si in db.StoreItemStocks on sc.StoreItemId equals si.StoreItemId
                                                    where si.StoreOutletId == outletId && si.ExpirationDate > date
                                                    join it in db.ItemPrices on si.StoreItemStockId equals it.StoreItemStockId
                                                    where it.MinimumQuantity == 1
                                                    join vv in db.StoreItemVariationValues on si.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                                    select new StoreItemStockObject
                                                    {
                                                        StoreItemStockId = si.StoreItemStockId,
                                                        StoreOutletId = si.StoreOutletId,
                                                        SKU = si.SKU,
                                                        StoreItemVariationId = si.StoreItemVariationId,
                                                        StoreItemVariationValueId = vv.StoreItemVariationValueId,
                                                        StoreItemId = sc.StoreItemId,
                                                        QuantityInStock = si.QuantityInStock,
                                                        CostPrice = si.CostPrice == null ? 0 : (double)si.CostPrice,
                                                        ReorderLevel = si.ReorderLevel,
                                                        ReorderQuantity = si.ReorderQuantity,
                                                        LastUpdated = si.LastUpdated,
                                                        ShelfLocation = si.ShelfLocation,
                                                        ExpirationDate = si.ExpirationDate,
                                                        TotalQuantityAlreadySold = si.TotalQuantityAlreadySold,
                                                        StoreCurrencyId = si.StoreCurrencyId,
                                                        StoreItemName = vv == null ? sc.Name : sc.Name + "/" + vv.Value,
                                                        UnitOfMeasurementId = it.UoMId
                                                    }).ToList();

                    if (!storeItemStockEntityList.Any())
                    {
                        return new List<StoreItemStockObject>();
                    }
                    return storeItemStockEntityList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemStockObject>();
            }
        }
    }
}
