using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
    public class ItemPriceRepository
    {
        private readonly IShopkeeperRepository<ItemPrice> _repository;
        private readonly UnitOfWork _uoWork;
        private ShopKeeperStoreEntities _db;

       public ItemPriceRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<ItemPrice>(_uoWork);
		}

       public long AddItemPrice(ItemPriceObject itemPrice)
       {
           try
           {
               if (itemPrice == null)
               {
                   return -2;
               }
               if (_repository.Count(m => m.Price.Equals(itemPrice.Price) && m.StoreItemStockId == itemPrice.StoreItemStockId && m.MinimumQuantity.Equals(itemPrice.MinimumQuantity)) > 0)
               {
                   return -3;
               }
               var itemPriceEntity = ModelCrossMapper.Map<ItemPriceObject, ItemPrice>(itemPrice);
               if (itemPriceEntity == null || itemPriceEntity.StoreItemStockId < 1)
               {
                   return -2;
               }
               var returnStatus = _repository.Add(itemPriceEntity);
               _uoWork.SaveChanges();
               return returnStatus.ItemPriceId;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public int UpdateItemPrice(ItemPriceObject itemPrice)
       {
           try
           {
               if (itemPrice == null)
               {
                   return -2;
               }
               if (_repository.Count(m => m.Price.Equals(itemPrice.Price) && m.StoreItemStockId == itemPrice.StoreItemStockId && m.MinimumQuantity.Equals(itemPrice.MinimumQuantity) && m.ItemPriceId != itemPrice.ItemPriceId) > 0)
               {
                   return -3;
               }
               var itemPriceEntity = ModelCrossMapper.Map<ItemPriceObject, ItemPrice>(itemPrice);
               if (itemPriceEntity == null || itemPriceEntity.ItemPriceId < 1)
               {
                   return -2;
               }
               _repository.Update(itemPriceEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteItemPrice(long itemPriceId)
       {
           try
           {
               var returnStatus = _repository.Remove(itemPriceId);
               _uoWork.SaveChanges();
               return returnStatus.ItemPriceId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public List<ItemPriceObject> GetItemPrices(long stockItemId)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                       (from it in db.ItemPrices.Where(m => m.StoreItemStockId == stockItemId).Include("UnitsOfMeasurement")
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            Price = it.Price,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = it.UnitsOfMeasurement.UoMCode
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new List<ItemPriceObject>();
                   }

                   return myItems;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<ItemPriceObject>();
           }
       }


       public ItemPriceObject GetItemPrice(long itemPriceId)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                       (from it in _db.ItemPrices.Where(m => m.ItemPriceId == itemPriceId)
                        join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            Price = it.Price,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = um.UoMCode,
                            StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                            CurrencySymbol = cr.Symbol
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new ItemPriceObject();
                   }
                   return myItems[0];
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new ItemPriceObject();
           }
       }


       public List<StoreItemStockObject> SearchItemPriceListByOutlet(int outletId, string criteria)
       {
           try
           {
               //.Skip(page).Take(itemsPerPage)
               var date = DateTime.Today;
               var search = criteria.ToLower();
               using (var db = _db)
               {
                   var itemStocks =
                       (from sc in db.StoreItemStocks.Where(t => (t.StoreItem.Name.ToLower().Contains(search) || t.SKU.ToLower().Contains(search)) && t.ItemPrices.Any() && t.QuantityInStock > 0 && (t.ExpirationDate == null || t.ExpirationDate > date) && t.Discontinued == false && t.PublishOnline == false)
                        join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues.Where(j => j.Value.ToLower().Contains(search) || !j.Value.ToLower().Contains(search)) on sc.StoreItemVariationValueId equals
                           iv.StoreItemVariationValueId

                        select new StoreItemStockObject
                        {
                            StoreItemStockId = sc.StoreItemStockId,
                            StoreItemId = sc.StoreItemId,
                            BrandName = si.StoreItemBrand.Name,
                            PublishOnline = sc.PublishOnline,
                            Discontinued = sc.Discontinued,
                            ReorderLevel = sc.ReorderLevel,
                            ReorderQuantity = sc.ReorderQuantity,
                            TypeName = si.StoreItemType.Name,
                            CategoryName = si.StoreItemCategory.Name,
                            StoreItemCategoryId = si.StoreItemCategoryId,
                            SKU = sc.SKU,
                            QuantityInStock = sc.QuantityInStock,
                            TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                            Description = si.Description,
                            StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!itemStocks.Any())
                   {
                       return new List<StoreItemStockObject>();
                   }

                   itemStocks.ForEach(x =>
                   {
                       x.ReOrderQuantityStr = x.ReorderQuantity.ToString("n0");
                       x.ReOrderLevelStr = x.ReorderLevel.ToString("n0");

                       x.ItemPriceObjects = new List<ItemPriceObject>();
                       var prices =
                           (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                            select new ItemPriceObject
                            {
                                ItemPriceId = it.ItemPriceId,
                                StoreItemStockId = it.StoreItemStockId,
                                Price = it.Price,
                                Remark = it.Remark,
                                UoMId = it.UoMId,
                                MinimumQuantity = it.MinimumQuantity,
                                UoMCode = it.UnitsOfMeasurement.UoMCode

                            }).ToList();

                       if (prices.Any())
                       {
                           prices.ForEach(p => x.ItemPriceObjects.Add(p));
                       }

                       var images = db.StockUploads.Where(m => m.StoreItemStockId == x.StoreItemStockId).Include("ImageView").ToList();
                       var img = new StockUpload();
                       if (images.Any())
                       {
                           var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                           if (front != null && front.StockUploadId > 0)
                           {
                               img = front;
                           }
                           else
                           {
                               img = images[0];
                           }
                       }
                       x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                   });

                   return itemStocks.OrderBy(o => o.StoreItemName).ToList();
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemStockObject>();
           }
       }

       public List<StoreItemStockObject> SearchAllItemPriceListByOutlet(int outletId, string criteria)
       {
           try
           {
               var search = criteria.ToLower();
               using (var db = _db)
               {
                   var itemStocks =
                       (from sc in db.StoreItemStocks.Where(t => (t.StoreItem.Name.ToLower().Contains(search) || t.SKU.ToLower().Contains(search)))
                        join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues.Where(j => j.Value.ToLower().Contains(search) || !j.Value.ToLower().Contains(search)) on sc.StoreItemVariationValueId equals
                           iv.StoreItemVariationValueId

                        select new StoreItemStockObject
                        {
                            StoreItemStockId = sc.StoreItemStockId,
                            StoreItemId = sc.StoreItemId,
                            BrandName = si.StoreItemBrand.Name,
                            CostPrice = sc.CostPrice,
                            PublishOnline = sc.PublishOnline,
                            Discontinued = sc.Discontinued,
                            ReorderLevel = sc.ReorderLevel,
                            ReorderQuantity = sc.ReorderQuantity,
                            TypeName = si.StoreItemType.Name,
                            CategoryName = si.StoreItemCategory.Name,
                            StoreItemCategoryId = si.StoreItemCategoryId,
                            SKU = sc.SKU,
                            QuantityInStock = sc.QuantityInStock,
                            TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                            Description = si.Description,
                            StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!itemStocks.Any())
                   {
                       return new List<StoreItemStockObject>();
                   }

                   itemStocks.ForEach(x =>
                   {
                       x.ReOrderQuantityStr = x.ReorderQuantity.ToString("n0");
                       x.ReOrderLevelStr = x.ReorderLevel.ToString("n0");

                       x.ItemPriceObjects = new List<ItemPriceObject>();
                       var prices =
                           (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                            select new ItemPriceObject
                            {
                                ItemPriceId = it.ItemPriceId,
                                StoreItemStockId = it.StoreItemStockId,
                                Price = it.Price,
                                Remark = it.Remark,
                                UoMId = it.UoMId,
                                MinimumQuantity = it.MinimumQuantity,
                                UoMCode = it.UnitsOfMeasurement.UoMCode

                            }).ToList();

                       if (prices.Any())
                       {
                           prices.ForEach(p => x.ItemPriceObjects.Add(p));
                       }

                       var images = db.StockUploads.Where(m => m.StoreItemStockId == x.StoreItemStockId).Include("ImageView").ToList();
                       var img = new StockUpload();
                       if (images.Any())
                       {
                           var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                           if (front != null && front.StockUploadId > 0)
                           {
                               img = front;
                           }
                           else
                           {
                               img = images[0];
                           }
                       }
                       x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                   });

                   return itemStocks.OrderBy(o => o.StoreItemName).ToList();
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemStockObject>();
           }
       }

       public List<ItemPriceObject> GetItemPrices(string criteria)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                         (from sc in db.StoreItemStocks.Where(j => j.SKU == criteria.Trim())
                          join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                          join it in _db.ItemPrices on sc.StoreItemStockId equals it.StoreItemStockId
                          join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                          join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                          join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId

                          select new ItemPriceObject
                          {
                              ItemPriceId = it.ItemPriceId,
                              StoreItemStockId = it.StoreItemStockId,
                              Price = it.Price,
                              Description = si.Description,
                              QuantityInStock = sc.QuantityInStock,
                              MinimumQuantity = it.MinimumQuantity,
                              Remark = it.Remark,
                              UoMId = it.UoMId,
                              UoMCode = um.UoMCode,
                              StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                              CurrencySymbol = cr.Symbol

                          }).ToList();


                   if (!myItems.Any())
                   {
                       myItems =
                       (
                        from si in db.StoreItems.Where(s => s.Name.ToLower().Contains(criteria.ToLower()))
                        join sc in db.StoreItemStocks on si.StoreItemId equals sc.StoreItemId
                        join it in _db.ItemPrices on sc.StoreItemStockId equals it.StoreItemStockId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            Price = it.Price,
                            Description = si.Description,
                            QuantityInStock = sc.QuantityInStock,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = um.UoMCode,
                            StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                            CurrencySymbol = cr.Symbol

                        }).ToList();
                   }

                   return !myItems.Any() ? new List<ItemPriceObject>() : myItems;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<ItemPriceObject>();
           }
       }

       public List<StoreItemStockObject> GetItemPriceListByOutlet(int outletId, int page, int itemsPerPage)
       {
           try
           {
               //.Skip(page).Take(itemsPerPage)
               var date = DateTime.Today;
               using (var db = _db)
               {
                   //var services = 28;
                   var orderItems =
                        (from itd in db.PurchaseOrderItemDeliveries.Where(it => it.ExpiryDate == null || it.ExpiryDate > date).OrderBy(k => k.ExpiryDate).Skip((page) * itemsPerPage).Take(itemsPerPage)
                         join pi in db.PurchaseOrderItems.Where(it => it.QuantityInStock > 0) on itd.PurchaseOrderItemId equals pi.PurchaseOrderItemId
                         join sc in db.StoreItemStocks.Where(t => t.ItemPrices.Any() && t.QuantityInStock > 0 && (t.ExpirationDate == null || t.ExpirationDate > date) && t.Discontinued == false && t.PublishOnline == false).OrderBy(s => s.ExpirationDate).ThenBy(o => o.StoreItem.Name) on pi.StoreItemStockId equals sc.StoreItemStockId
                         join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                         join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new StoreItemStockObject
                         {
                             StoreItemStockId = sc.StoreItemStockId,
                             StoreItemId = sc.StoreItemId,
                             BrandName = si.StoreItemBrand.Name,
                             PublishOnline = sc.PublishOnline,
                             ReorderLevel = sc.ReorderLevel,
                             ReorderQuantity = sc.ReorderQuantity,
                             Discontinued = sc.Discontinued,
                             TypeName = si.StoreItemType.Name,
                             CategoryName = si.StoreItemCategory.Name,
                             StoreItemCategoryId = si.StoreItemCategoryId,
                             SKU = sc.SKU,
                             QuantityInStock = sc.QuantityInStock,
                             TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                             Description = si.Description,
                             StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                         }).ToList();

                   List<StoreItemStockObject> list;

                   if (!orderItems.Any())
                   {
                       var itemStocks =
                           (from sc in db.StoreItemStocks.Where(t => t.ItemPrices.Any() && t.QuantityInStock > 0 && (t.ExpirationDate == null || t.ExpirationDate > date) && t.Discontinued == false && t.PublishOnline == false)
                                   .OrderBy(s => s.ExpirationDate).ThenBy(o => o.StoreItem.Name).Skip((page) * itemsPerPage).Take(itemsPerPage)
                            join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                            join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals
                            iv.StoreItemVariationValueId

                            select new StoreItemStockObject
                            {
                                StoreItemStockId = sc.StoreItemStockId,
                                StoreItemId = sc.StoreItemId,
                                BrandName = si.StoreItemBrand.Name,
                                PublishOnline = sc.PublishOnline,
                                Discontinued = sc.Discontinued,
                                ReorderLevel = sc.ReorderLevel,
                                ReorderQuantity = sc.ReorderQuantity,
                                TypeName = si.StoreItemType.Name,
                                CategoryName = si.StoreItemCategory.Name,
                                StoreItemCategoryId = si.StoreItemCategoryId,
                                SKU = sc.SKU,
                                QuantityInStock = sc.QuantityInStock,
                                TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                                Description = si.Description,
                                StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                            }).ToList();

                       if (!itemStocks.Any())
                       {
                           return new List<StoreItemStockObject>();
                       }

                       list = itemStocks;
                   }
                   else
                   {
                       list = orderItems;
                   }

                   if (!list.Any())
                   {
                       return new List<StoreItemStockObject>();
                   }

                   //var serviceItems = (from sc in db.StoreItemStocks.Where(t => t.StoreItem.StoreItemCategoryId == services).OrderBy(s => s.StoreItem.Name).Skip((page) * itemsPerPage).Take(itemsPerPage)
                   //                    join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                   //                    join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                   //                    select new StoreItemStockObject
                   //                    {
                   //                        StoreItemStockId = sc.StoreItemStockId,
                   //                        StoreItemId = sc.StoreItemId,
                   //                        BrandName = si.StoreItemBrand.Name,
                   //                        PublishOnline = sc.PublishOnline,
                   //                        ReorderLevel = sc.ReorderLevel,
                   //                        ReorderQuantity = sc.ReorderQuantity,
                   //                        Discontinued = sc.Discontinued,
                   //                        TypeName = si.StoreItemType.Name,
                   //                        CategoryName = si.StoreItemCategory.Name,
                   //                        StoreItemCategoryId = si.StoreItemCategoryId,
                   //                        SKU = sc.SKU,
                   //                        QuantityInStock = sc.QuantityInStock,
                   //                        TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                   //                        Description = si.Description,
                   //                        StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                   //                    }).ToList();

                   //if (serviceItems.Any())
                   //{
                   //    list.AddRange(serviceItems);
                   //}

                   list.ForEach(x =>
                   {
                       //var pOrderItems = db.PurchaseOrderItems.Where(g => g.StoreItemStockId == x.StoreItemStockId).ToList();
                       //if (pOrderItems.Any())
                       //{
                       //    x.PurchaseOrderItemId = pOrderItems[0].PurchaseOrderItemId;
                       //}

                       x.ReOrderQuantityStr = x.ReorderQuantity.ToString("n0");
                       x.ReOrderLevelStr = x.ReorderLevel.ToString("n0");

                       x.ItemPriceObjects = new List<ItemPriceObject>();
                       var prices =
                           (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                            select new ItemPriceObject
                            {
                                ItemPriceId = it.ItemPriceId,
                                StoreItemStockId = it.StoreItemStockId,
                                Price = it.Price,
                                Remark = it.Remark,
                                UoMId = it.UoMId,
                                MinimumQuantity = it.MinimumQuantity,
                                UoMCode = it.UnitsOfMeasurement.UoMCode

                            }).ToList();

                       if (prices.Any())
                       {
                           prices.ForEach(p => x.ItemPriceObjects.Add(p));
                       }

                       var images = db.StockUploads.Where(m => m.StoreItemStockId == x.StoreItemStockId).Include("ImageView").ToList();
                       var img = new StockUpload();
                       if (images.Any())
                       {
                           var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                           if (front != null && front.StockUploadId > 0)
                           {
                               img = front;
                           }
                           else
                           {
                               img = images[0];
                           }
                       }
                       x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                   });

                   return !list.Any() ? new List<StoreItemStockObject>() : list;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemStockObject>();
           }
       }

       public List<StoreItemStockObject> GetAllItemPriceListByOutlet(int outletId, int page, int itemsPerPage)
       {
           try
           {
               //.Skip(page).Take(itemsPerPage)
               var date = DateTime.Today;
               using (var db = _db)
               {
                   var orderItems =
                        (from itd in db.PurchaseOrderItemDeliveries.OrderBy(k => k.ExpiryDate).Skip((page) * itemsPerPage).Take(itemsPerPage)
                         join pi in db.PurchaseOrderItems on itd.PurchaseOrderItemId equals pi.PurchaseOrderItemId
                         join sc in db.StoreItemStocks.Where(t => t.ItemPrices.Any()).OrderBy(s => s.StoreItem.Name) on pi.StoreItemStockId equals sc.StoreItemStockId
                         join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                         join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new StoreItemStockObject
                         {
                             StoreItemStockId = sc.StoreItemStockId,
                             StoreItemId = sc.StoreItemId,
                             BrandName = si.StoreItemBrand.Name,
                             PublishOnline = sc.PublishOnline,
                             ReorderLevel = sc.ReorderLevel,
                             ReorderQuantity = sc.ReorderQuantity,
                             Discontinued = sc.Discontinued,
                             TypeName = si.StoreItemType.Name,
                             CategoryName = si.StoreItemCategory.Name,
                             StoreItemCategoryId = si.StoreItemCategoryId,
                             SKU = sc.SKU,
                             QuantityInStock = sc.QuantityInStock,
                             TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                             Description = si.Description,
                             StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                         }).ToList();

                   List<StoreItemStockObject> list;
                   if (!orderItems.Any())
                   {
                       var itemStocks =
                           (from sc in
                                db.StoreItemStocks.Where(
                                    t =>  t.ItemPrices.Any() && t.QuantityInStock > 0 &&
                                        (t.ExpirationDate == null || t.ExpirationDate > date) && t.Discontinued == false && t.PublishOnline == false)
                                    .OrderBy(s => s.ExpirationDate)
                                    .ThenBy(o => o.StoreItem.Name)
                                    .Skip((page) * itemsPerPage)
                                    .Take(itemsPerPage)
                            join si in
                                db.StoreItems.Include("StoreItemBrand")
                                    .Include("StoreItemCategory")
                                    .Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                            join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals
                                iv.StoreItemVariationValueId

                            select new StoreItemStockObject
                            {
                                StoreItemStockId = sc.StoreItemStockId,
                                StoreItemId = sc.StoreItemId,
                                BrandName = si.StoreItemBrand.Name,
                                PublishOnline = sc.PublishOnline,
                                Discontinued = sc.Discontinued,
                                ReorderLevel = sc.ReorderLevel,
                                ReorderQuantity = sc.ReorderQuantity,
                                TypeName = si.StoreItemType.Name,
                                CategoryName = si.StoreItemCategory.Name,
                                StoreItemCategoryId = si.StoreItemCategoryId,
                                SKU = sc.SKU,
                                QuantityInStock = sc.QuantityInStock,
                                TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                                Description = si.Description,
                                StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                            }).ToList();

                       if (!itemStocks.Any())
                       {
                           return new List<StoreItemStockObject>();
                       }

                       list = itemStocks;
                   }
                   else
                   {
                       list = orderItems;
                   }

                   if (!list.Any())
                   {
                       return new List<StoreItemStockObject>();
                   }

                   list.ForEach(x =>
                   {
                       //var pOrderItems = db.PurchaseOrderItems.Where(g => g.StoreItemStockId == x.StoreItemStockId).ToList();
                       //if (pOrderItems.Any())
                       //{
                       //    x.PurchaseOrderItemId = pOrderItems[0].PurchaseOrderItemId;
                       //}

                       x.ReOrderQuantityStr = x.ReorderQuantity.ToString("n0");
                       x.ReOrderLevelStr = x.ReorderLevel.ToString("n0");

                       x.ItemPriceObjects = new List<ItemPriceObject>();
                       var prices =
                           (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                            select new ItemPriceObject
                            {
                                ItemPriceId = it.ItemPriceId,
                                StoreItemStockId = it.StoreItemStockId,
                                Price = it.Price,
                                Remark = it.Remark,
                                UoMId = it.UoMId,
                                MinimumQuantity = it.MinimumQuantity,
                                UoMCode = it.UnitsOfMeasurement.UoMCode

                            }).ToList();

                       if (prices.Any())
                       {
                           prices.ForEach(p => x.ItemPriceObjects.Add(p));
                       }

                       var images = db.StockUploads.Where(m => m.StoreItemStockId == x.StoreItemStockId).Include("ImageView").ToList();
                       var img = new StockUpload();
                       if (images.Any())
                       {
                           var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                           if (front != null && front.StockUploadId > 0)
                           {
                               img = front;
                           }
                           else
                           {
                               img = images[0];
                           }
                       }
                       x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                   });

                   return !list.Any() ? new List<StoreItemStockObject>() : list;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemStockObject>();
           }
       }

       public List<StoreItemStockObject> GetItemPriceListForWeb(int page, int itemsPerPage)
       {
           try
           {
               //.Skip(page).Take(itemsPerPage)
               var list = new List<StoreItemStockObject>();
               var date = DateTime.Today;
               using (var db = _db)
               {
                   var orderItems =
                        (from itd in db.PurchaseOrderItemDeliveries.Where(it => it.ExpiryDate == null || it.ExpiryDate > date).OrderBy(k => k.ExpiryDate).Skip((page) * itemsPerPage).Take(itemsPerPage)
                         join pi in db.PurchaseOrderItems.Where(it => it.QuantityInStock > 0) on itd.PurchaseOrderItemId equals pi.PurchaseOrderItemId
                         join sc in db.StoreItemStocks.Where(t => t.ItemPrices.Any() && t.QuantityInStock > 0 && (t.ExpirationDate == null || t.ExpirationDate > date) && t.Discontinued == false && t.PublishOnline == true).OrderBy(s => s.ExpirationDate).ThenBy(o => o.StoreItem.Name) on pi.StoreItemStockId equals sc.StoreItemStockId
                         join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                         join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new StoreItemStockObject
                         {
                             StoreItemStockId = sc.StoreItemStockId,
                             StoreItemId = sc.StoreItemId,
                             BrandName = si.StoreItemBrand.Name,
                             PublishOnline = sc.PublishOnline,
                             ReorderLevel = sc.ReorderLevel,
                             ReorderQuantity = sc.ReorderQuantity,
                             CostPrice = sc.CostPrice,
                             Discontinued = sc.Discontinued,
                             TypeName = si.StoreItemType.Name,
                             CategoryName = si.StoreItemCategory.Name,
                             StoreItemCategoryId = si.StoreItemCategoryId,
                             SKU = sc.SKU,
                             QuantityInStock = sc.QuantityInStock,
                             TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                             Description = si.Description,
                             StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                         }).ToList();

                   if (!orderItems.Any())
                   {
                       var itemStocks =
                           (from sc in
                                db.StoreItemStocks.Where(
                                    t =>
                                        t.ItemPrices.Any() && t.QuantityInStock > 0 &&
                                        (t.ExpirationDate == null || t.ExpirationDate > date) && t.Discontinued == false && t.PublishOnline == true)
                                    .OrderBy(s => s.ExpirationDate)
                                    .ThenBy(o => o.StoreItem.Name)
                                    .Skip((page) * itemsPerPage)
                                    .Take(itemsPerPage)
                            join si in
                                db.StoreItems.Include("StoreItemBrand")
                                    .Include("StoreItemCategory")
                                    .Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                            join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals
                                iv.StoreItemVariationValueId

                            select new StoreItemStockObject
                            {
                                StoreItemStockId = sc.StoreItemStockId,
                                StoreItemId = sc.StoreItemId,
                                BrandName = si.StoreItemBrand.Name,
                                PublishOnline = sc.PublishOnline,
                                ReorderLevel = sc.ReorderLevel,
                                ReorderQuantity = sc.ReorderQuantity,
                                Discontinued = sc.Discontinued,
                                CostPrice = sc.CostPrice,
                                TypeName = si.StoreItemType.Name,
                                CategoryName = si.StoreItemCategory.Name,
                                StoreItemCategoryId = si.StoreItemCategoryId,
                                SKU = sc.SKU,
                                QuantityInStock = sc.QuantityInStock,
                                TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                                Description = si.Description,
                                StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                            }).ToList();

                       if (!itemStocks.Any())
                       {
                           return new List<StoreItemStockObject>();
                       }
                       list = itemStocks;
                   }
                   else
                   {
                       list = orderItems;
                   }

                   if (!list.Any())
                   {
                       return new List<StoreItemStockObject>();
                   }

                   list.ForEach(x =>
                   {
                       //var pOrderItems = db.PurchaseOrderItems.Where(g => g.StoreItemStockId == x.StoreItemStockId).ToList();
                       //if (pOrderItems.Any())
                       //{
                       //    x.PurchaseOrderItemId = pOrderItems[0].PurchaseOrderItemId;
                       //}

                       x.ReOrderQuantityStr = x.ReorderQuantity.ToString("n0");
                       x.ReOrderLevelStr = x.ReorderLevel.ToString("n0");

                       x.ItemPriceObjects = new List<ItemPriceObject>();
                       var prices =
                           (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                            select new ItemPriceObject
                            {
                                ItemPriceId = it.ItemPriceId,
                                StoreItemStockId = it.StoreItemStockId,
                                Price = it.Price,
                                Remark = it.Remark,
                                UoMId = it.UoMId,
                                MinimumQuantity = it.MinimumQuantity,
                                UoMCode = it.UnitsOfMeasurement.UoMCode

                            }).ToList();

                       if (prices.Any())
                       {
                           prices.ForEach(p => x.ItemPriceObjects.Add(p));
                       }

                       var images = db.StockUploads.Where(m => m.StoreItemStockId == x.StoreItemStockId).Include("ImageView").ToList();
                       var img = new StockUpload();
                       if (images.Any())
                       {
                           var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                           if (front != null && front.StockUploadId > 0)
                           {
                               img = front;
                           }
                           else
                           {
                               img = images[0];
                           }
                       }
                       x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                   });

                   return !list.Any() ? new List<StoreItemStockObject>() : list;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemStockObject>();
           }
       }

       public List<StoreItemStockObject> GetProducts(int page, int itemsPerPage)
       {
           try
           {
               using (var db = _db)
               {
                   var itemStocks =
                        (
                         from sc in db.StoreItemStocks
                         .OrderBy(o => o.StoreItem.Name).Skip((page) * itemsPerPage).Take(itemsPerPage)
                         join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId
                         join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new StoreItemStockObject
                         {
                             StoreItemStockId = sc.StoreItemStockId,
                             StoreItemId = sc.StoreItemId,
                             BrandName = si.StoreItemBrand.Name,
                             TypeName = si.StoreItemType.Name,
                             CategoryName = si.StoreItemCategory.Name,
                             CostPrice = sc.CostPrice,
                             ReorderLevel = sc.ReorderLevel,
                             ReorderQuantity = sc.ReorderQuantity,
                             StoreItemCategoryId = si.StoreItemCategoryId,
                             SKU = sc.SKU,
                             QuantityInStock = sc.QuantityInStock,
                             TotalQuantityAlreadySold = sc.TotalQuantityAlreadySold,
                             Description = si.Description,
                             StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                         }).ToList();

                   if (!itemStocks.Any())
                   {
                       return new List<StoreItemStockObject>();
                   }

                   itemStocks.ForEach(x =>
                   {

                       var uoms = (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                                   select new ItemPriceObject
                                   {
                                       UoMCode = it.UnitsOfMeasurement.UoMCode

                                   }).ToList();

                       if (uoms.Any())
                       {
                           x.UoMCode = uoms[0].UoMCode;
                       }

                       x.ReOrderQuantityStr = x.ReorderQuantity.ToString("n0");
                       x.ReOrderLevelStr = x.ReorderLevel.ToString("n0");

                       var images = db.StockUploads.Where(m => m.StoreItemStockId == x.StoreItemStockId).Include("ImageView").ToList();
                       var img = new StockUpload();
                       if (images.Any())
                       {
                           var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                           if (front != null && front.StockUploadId > 0)
                           {
                               img = front;
                           }
                           else
                           {
                               img = images[0];
                           }
                       }
                       x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                   });

                   return !itemStocks.Any() ? new List<StoreItemStockObject>() : itemStocks;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreItemStockObject>();
           }
       }

       public List<ItemPriceObject> GetItemPriceListByStockItemId(long stockItemId)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                       (from it in _db.ItemPrices.Where(m => m.StoreItemStockId == stockItemId)
                        join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            Price = it.Price,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = um.UoMCode,
                            StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new List<ItemPriceObject>();
                   }
                   return myItems;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<ItemPriceObject>();
           }
       }

       public List<ItemPriceObject> GetItemPriceListByStockItemCategory(int categoryId, int outletId)
       {
           try
           {
               using (var db = _db)
               {
                   var date = DateTime.Today;
                   var myItems =
                       (from si in db.StoreItems.Where(m => m.StoreItemCategoryId == categoryId)
                        join sc in db.StoreItemStocks on si.StoreItemId equals sc.StoreItemId
                        join ct in db.StoreItemCategories on si.StoreItemCategoryId equals ct.StoreItemCategoryId
                        join su in db.StockUploads on sc.StoreItemStockId equals su.StoreItemStockId
                        join it in db.ItemPrices on sc.StoreItemStockId equals it.StoreItemStockId
                        where sc.StoreOutletId == outletId && (sc.ExpirationDate == null || sc.ExpirationDate > date)
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            StoreItemId = sc.StoreItemId,
                            StoreItemCategoryId = ct.StoreItemCategoryId,
                            Name = ct.Name,
                            SKU = sc.SKU,
                            ImagePath = su.ImagePath,
                            Price = it.Price,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = um.UoMCode,
                            StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();
                   myItems.ForEach(x =>
                   {
                       x.ImagePath = x.ImagePath == null || string.IsNullOrEmpty(x.ImagePath)
                           ? "/Content/images/noImage.png"
                           : PhysicalToVirtualPathMapper.MapPath(x.ImagePath);

                   });

                   if (!myItems.Any())
                   {
                       return new List<ItemPriceObject>();
                   }
                   return myItems;
               }
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
               using (var db = _db)
               {
                   var date = DateTime.Today;
                   var myItems =
                       (from sc in db.StoreItemStocks.Where(m => m.SKU.Trim() == sku.Trim())
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join su in db.StockUploads on sc.StoreItemStockId equals su.StoreItemStockId
                        join ct in db.StoreItemCategories on si.StoreItemCategoryId equals ct.StoreItemCategoryId
                        join it in db.ItemPrices on sc.StoreItemStockId equals it.StoreItemStockId
                        where sc.StoreOutletId == outletId && (sc.ExpirationDate == null || sc.ExpirationDate > date)
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            StoreItemId = sc.StoreItemId,
                            StoreItemCategoryId = ct.StoreItemCategoryId,
                            Name = ct.Name,
                            SKU = sc.SKU,
                            ImagePath = su == null || su.StockUploadId < 1 ? "/Content/images/noImage.png" : PhysicalToVirtualPathMapper.MapPath(su.ImagePath),
                            Price = it.Price,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = um.UoMCode,
                            StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new List<ItemPriceObject>();
                   }
                   return myItems;
               }
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
               using (var db = _db)
               {
                   var myItems =
                       (from it in _db.ItemPrices.Where(m => m.StoreItemStockId == stockItemId)
                        join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                        join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                        join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                        select new ItemPriceObject
                        {
                            ItemPriceId = it.ItemPriceId,
                            StoreItemStockId = it.StoreItemStockId,
                            Price = it.Price,
                            MinimumQuantity = it.MinimumQuantity,
                            Remark = it.Remark,
                            UoMId = it.UoMId,
                            UoMCode = um.UoMCode,
                            StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                            CurrencySymbol = cr.Symbol
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new ItemPriceObject();
                   }
                   return myItems[0];
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new ItemPriceObject();
           }
       }

       public List<ItemPriceObject> GetItemPriceObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               using (var db = _db)
               {
                   List<ItemPriceObject> itemPriceObjectList;
                   if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                   {
                       var tpageNumber = (int)pageNumber;
                       var tsize = (int)itemsPerPage;
                       itemPriceObjectList = (from it in
                                                  db.ItemPrices.OrderBy(m => m.StoreItemStockId).Skip((tpageNumber) * tsize).Take(tsize)
                                              join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                                              join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                                              join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                                              join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                                              join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                                              select new ItemPriceObject
                                              {
                                                  ItemPriceId = it.ItemPriceId,
                                                  StoreItemStockId = it.StoreItemStockId,
                                                  Price = it.Price,
                                                  MinimumQuantity = it.MinimumQuantity,
                                                  Remark = it.Remark,
                                                  UoMId = it.UoMId,
                                                  UoMCode = um.UoMCode,
                                                  StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                                                  CurrencySymbol = cr.Symbol
                                              }).ToList();

                   }

                   else
                   {
                       itemPriceObjectList = (from it in db.ItemPrices.OrderBy(m => m.StoreItemStockId)
                                              join sc in db.StoreItemStocks on it.StoreItemStockId equals sc.StoreItemStockId
                                              join si in db.StoreItems on sc.StoreItemId equals si.StoreItemId
                                              join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                                              join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                                              join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                                              select new ItemPriceObject
                                              {
                                                  ItemPriceId = it.ItemPriceId,
                                                  StoreItemStockId = it.StoreItemStockId,
                                                  Price = it.Price,
                                                  MinimumQuantity = it.MinimumQuantity,
                                                  Remark = it.Remark,
                                                  UoMId = it.UoMId,
                                                  UoMCode = um.UoMCode,
                                                  StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                                                  CurrencySymbol = cr.Symbol
                                              }).ToList();
                   }

                   if (!itemPriceObjectList.Any())
                   {
                       return new List<ItemPriceObject>();
                   }

                   return itemPriceObjectList;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<ItemPriceObject>();
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

       public int GetObjectCount(Expression<Func<ItemPrice, bool>> predicate)
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

       public List<ItemPriceObject> Search(string searchCriteria)
       {
           try
           {
               using (var db = _db)
               {
                   var searchList = (from si in db.StoreItems
                                     where si.Name.ToLower().Contains(searchCriteria)
                                     join sc in db.StoreItemStocks on si.StoreItemId equals sc.StoreItemId
                                     join it in db.ItemPrices on sc.StoreItemStockId equals it.StoreItemStockId
                                     join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId
                                     join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                                     join cr in db.StoreCurrencies on sc.StoreCurrencyId equals cr.StoreCurrencyId
                                     select new ItemPriceObject
                                     {
                                         ItemPriceId = it.ItemPriceId,
                                         StoreItemStockId = it.StoreItemStockId,
                                         Price = it.Price,
                                         MinimumQuantity = it.MinimumQuantity,
                                         Remark = it.Remark,
                                         UoMId = it.UoMId,
                                         UoMCode = um.UoMCode,
                                         StoreItemStockName = iv == null ? si.Name : si.Name + "/" + iv.Value,
                                         CurrencySymbol = cr.Symbol
                                     }).ToList();

                   return !searchList.Any() ? new List<ItemPriceObject>() : searchList;
               }

           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<ItemPriceObject>();
           }
       }

       public List<ItemPriceObject> GetItemPrices()
       {
           try
           {
               var itemPriceEntityList = _repository.GetAll().ToList();
               if (!itemPriceEntityList.Any())
               {
                   return new List<ItemPriceObject>();
               }
               var itemPriceObjList = new List<ItemPriceObject>();
               itemPriceEntityList.ForEach(m =>
               {
                   var itemPriceObject = ModelCrossMapper.Map<ItemPrice, ItemPriceObject>(m);
                   if (itemPriceObject != null && itemPriceObject.ItemPriceId > 0)
                   {
                       itemPriceObjList.Add(itemPriceObject);
                   }
               });
               return itemPriceObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
       
    }
}
