using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreItemSoldRepository
    {
        private readonly IShopkeeperRepository<StoreItemSold> _repository;
        private readonly UnitOfWork _uoWork;
        private ShopKeeperStoreEntities _db = new ShopKeeperStoreEntities();

       public StoreItemSoldRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<StoreItemSold>(_uoWork);
		}

       public long AddStoreItemSold(StoreItemSoldObject itemSold)
       {
           try
           {
               if (itemSold == null)
               {
                   return -2;
               }

               var itemSoldEntity = ModelCrossMapper.Map<StoreItemSoldObject, StoreItemSold>(itemSold);
               if (itemSoldEntity == null || itemSoldEntity.StoreItemStockId < 1)
               {
                   return -2;
               }
               var returnStatus = _repository.Add(itemSoldEntity);
               _uoWork.SaveChanges();
               return returnStatus.StoreItemSoldId;
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
               if (itemSold == null)
               {
                   return -2;
               }
               var itemSoldEntity = ModelCrossMapper.Map<StoreItemSoldObject, StoreItemSold>(itemSold);
               if (itemSoldEntity == null || itemSoldEntity.StoreItemSoldId < 1)
               {
                   return -2;
               }
               _repository.Update(itemSoldEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteStoreItemSold(long itemSoldId)
       {
           try
           {
               var returnStatus = _repository.Remove(itemSoldId);
               _uoWork.SaveChanges();
               return returnStatus.StoreItemSoldId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public StoreItemSoldObject GetStoreItemSold(long itemSoldId)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                       (from it in _db.StoreItemSolds.Where(m => m.StoreItemSoldId == itemSoldId)
                        join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                        select new StoreItemSoldObject
                        {
                            StoreItemSoldId = it.StoreItemSoldId,
                            DateSold = it.DateSold,
                            SaleId = it.SaleId,
                            StoreItemStockId = it.StoreItemStockId,
                            AmountSold = it.AmountSold,
                            QuantitySold = it.QuantitySold,
                            UoMCode = um.UoMCode,
                            ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new StoreItemSoldObject();
                   }
                   var item = myItems[0];
                   item.DateSoldStr = item.DateSold.ToString("dd/MM/yyyy");
                   return myItems[0];
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new StoreItemSoldObject();
           }
       }

       public List<StoreItemSoldObject> GetStoreItemSoldListByStockItemId(long stockItemId)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                       (from it in _db.StoreItemSolds.Where(m => m.StoreItemStockId == stockItemId)
                        join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        select new StoreItemSoldObject
                        {
                            StoreItemSoldId = it.StoreItemSoldId,
                            DateSold = it.DateSold,
                            SaleId = it.SaleId,
                            StoreItemStockId = it.StoreItemStockId,
                            AmountSold = it.AmountSold,
                            QuantitySold = it.QuantitySold,
                            UoMCode = um.UoMCode,
                            ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new List<StoreItemSoldObject>();
                   }
                   myItems.ForEach(m =>
                   {
                       m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                   });
                   return myItems;
               }
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
               using (var db = _db)
               {
                   var myItems =
                       (from si in db.StoreItems.Where(m => m.StoreItemCategoryId == categoryId)
                        join sc in db.StoreItemStocks on si.StoreItemId equals sc.StoreItemId
                        join it in _db.StoreItemSolds on sc.StoreItemStockId equals it.StoreItemStockId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        select new StoreItemSoldObject
                        {
                            StoreItemSoldId = it.StoreItemSoldId,
                            DateSold = it.DateSold,
                            SaleId = it.SaleId,
                            StoreItemStockId = it.StoreItemStockId,
                            AmountSold = it.AmountSold,
                            QuantitySold = it.QuantitySold,
                            UoMCode = um.UoMCode,
                            ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new List<StoreItemSoldObject>();
                   }
                   myItems.ForEach(m =>
                   {
                       m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                   });
                   return myItems;
               }
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
               using (var db = _db)
               {
                   var myItems =
                       (from it in _db.StoreItemSolds.Where(m => m.StoreItemStockId == stockItemId)
                        join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                        select new StoreItemSoldObject
                        {
                            StoreItemSoldId = it.StoreItemSoldId,
                            DateSold = it.DateSold,
                            SaleId = it.SaleId,
                            StoreItemStockId = it.StoreItemStockId,
                            AmountSold = it.AmountSold,
                            QuantitySold = it.QuantitySold,
                            UoMCode = um.UoMCode,
                            ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new StoreItemSoldObject();
                   }
                   myItems.ForEach(m =>
                   {
                       m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                   });
                   return myItems[0];
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new StoreItemSoldObject();
           }
       }

       public List<StoreItemSoldObject> GetStoreItemSoldObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               using (var db = _db)
               {
                   List<StoreItemSoldObject> itemSoldObjectList;
                   if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                   {
                       var tpageNumber = (int)pageNumber;
                       var tsize = (int)itemsPerPage;
                       itemSoldObjectList = (from it in
                                                 db.StoreItemSolds.OrderBy(m => m.StoreItemStockId).Skip((tpageNumber) * tsize).Take(tsize)
                                             join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                                             join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                                             join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                                             join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                                             join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                                             select new StoreItemSoldObject
                                             {
                                                 StoreItemSoldId = it.StoreItemSoldId,
                                                 DateSold = it.DateSold,
                                                 SaleId = it.SaleId,
                                                 StoreItemStockId = it.StoreItemStockId,
                                                 AmountSold = it.AmountSold,
                                                 QuantitySold = it.QuantitySold,
                                                 UoMCode = um.UoMCode,
                                                 ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                                             }).ToList();

                   }

                   else
                   {
                       itemSoldObjectList = (from it in db.StoreItemSolds.OrderBy(m => m.StoreItemStockId)
                                             join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                                             join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                                             join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                                             join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                                             join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                                             select new StoreItemSoldObject
                                             {
                                                 StoreItemSoldId = it.StoreItemSoldId,
                                                 DateSold = it.DateSold,
                                                 SaleId = it.SaleId,
                                                 StoreItemStockId = it.StoreItemStockId,
                                                 AmountSold = it.AmountSold,
                                                 QuantitySold = it.QuantitySold,
                                                 UoMCode = um.UoMCode,
                                                 ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                                             }).ToList();
                   }

                   if (!itemSoldObjectList.Any())
                   {
                       return new List<StoreItemSoldObject>();
                   }
                   itemSoldObjectList.ForEach(m =>
                   {
                       m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                   });
                   return itemSoldObjectList;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemSoldObject>();
           }
       }

       public int GetObjectCount()
       {
           try
           {
               return _repository.Count();
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public int GetObjectCount(Expression<Func<StoreItemSold, bool>> predicate)
       {
           try
           {
               return _repository.Count(predicate);
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public List<StoreItemSoldObject> Search(string searchCriteria)
       {
           try
           {
               using (var db = _db)
               {
                   var searchList = (from si in db.StoreItems
                                     where si.Name.ToLower().Contains(searchCriteria)
                                     join sc in db.StoreItemStocks on si.StoreItemId equals sc.StoreItemId
                                     join it in db.StoreItemSolds on sc.StoreItemStockId equals it.StoreItemStockId
                                     join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                                     join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                                     join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemSoldId = it.StoreItemSoldId,
                                         DateSold = it.DateSold,
                                         SaleId = it.SaleId,
                                         StoreItemStockId = it.StoreItemStockId,
                                         AmountSold = it.AmountSold,
                                         QuantitySold = it.QuantitySold,
                                         UoMCode = um.UoMCode,
                                         ItemSoldName = iv == null ? si.Name : si.Name + "/" + iv.Value
                                     }).ToList();

                   if (!searchList.Any())
                   {
                       return new List<StoreItemSoldObject>();
                   }

                   searchList.ForEach(m =>
                   {
                       m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                   });
                   return searchList;
               }

           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemSoldObject>();
           }
       }

       public List<StoreItemSoldObject> GetStoreItemSolds()
       {
           try
           {
               var itemSoldEntityList = _repository.GetAll().ToList();
               if (!itemSoldEntityList.Any())
               {
                   return new List<StoreItemSoldObject>();
               }
               var itemSoldObjList = new List<StoreItemSoldObject>();
               itemSoldEntityList.ForEach(m =>
               {
                   var itemSoldObject = ModelCrossMapper.Map<StoreItemSold, StoreItemSoldObject>(m);
                   if (itemSoldObject != null && itemSoldObject.StoreItemSoldId > 0)
                   {
                       itemSoldObjList.Add(itemSoldObject);
                   }
               });
               return itemSoldObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
    }
}
