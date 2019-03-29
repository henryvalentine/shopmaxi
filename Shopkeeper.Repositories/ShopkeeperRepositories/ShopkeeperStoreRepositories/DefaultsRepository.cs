using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class DefaultsRepository
    {
        private readonly IShopkeeperRepository<Sale> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _db;

        public DefaultsRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<Sale>(_uoWork);
		}
       
        public OnlineStoreObject GetDefaults()
        {
            try
            {
                using (var db = _db)
                {
                    var categories = (from ct in db.StoreItemCategories
                                       select new StoreItemCategoryObject
                                       {
                                           StoreItemCategoryId = ct.StoreItemCategoryId,
                                           Name = ct.Name,
                                           ImagePath = string.IsNullOrEmpty(ct.ImagePath) ? "/Content/images/noImage.png" : ct.ImagePath.Replace("~", "") 
                                       }).ToList();

                    var itemTypes = (from tp in db.StoreItemTypes select new StoreItemTypeObject
                    {
                        StoreItemTypeId = tp.StoreItemTypeId,
                        Name = tp.Name,
                        SampleImagePath = tp.SampleImagePath
                    }).ToList();

                    var itemBrands = (from bd in db.StoreItemBrands select new StoreItemBrandObject
                    {
                        StoreItemBrandId = bd.StoreItemBrandId,
                        Name = bd.Name
                    }).ToList();

                    var items = (from st in db.StoreItems.OrderBy(m => m.StoreItemId).Include("StoreItemCategory").Include("StoreItemBrand").Include("StoreItemType").Take(15)
                                 join stk in db.StoreItemStocks.Where(d => d.QuantityInStock > 0).Include("StoreCurrency") on st.StoreItemId equals stk.StoreItemId
                                 
                                 select new StoreItemObject
                                 {
                                     StoreItemId = st.StoreItemId,
                                     CurrencySymbol = stk.StoreCurrency.Symbol,
                                     StoreItemBrandName = st.StoreItemBrand.Name,
                                     StoreItemTypeName = st.StoreItemType.Name,
                                     StoreItemCategoryName = st.StoreItemCategory.Name,
                                     StoreItemStockId = stk.StoreItemStockId,
                                     StoreItemBrandId = st.StoreItemBrandId,
                                     StoreItemTypeId = st.StoreItemTypeId,
                                     StoreItemCategoryId = st.StoreItemCategoryId,
                                     ChartOfAccountId = st.ChartOfAccountId,
                                     Name = st.Name,
                                     Description = st.Description,
                                     ParentItemId = st.ParentItemId
                                    
                                 }).ToList();
                    //.Where(x => x.ImageView.Name.Contains("Front"))
                    var obj = new OnlineStoreObject
                    {
                        ItemCategories = categories,
                        ItemTypes = itemTypes,
                        ItemBrands = itemBrands,
                        Items = items
                    };

                    if (items.Any())
                    {
                       items.ForEach(i =>
                       {
                           var itemPrices = db.ItemPrices.Where(p => p.StoreItemStockId == i.StoreItemStockId).OrderBy(p => p.MinimumQuantity).Take(1).ToList();
                           if (itemPrices.Any())
                           {
                               i.Price = itemPrices[0].Price;
                           }

                           var images = db.StockUploads.Where(m => m.StoreItemStockId == i.StoreItemStockId).Include("ImageView").ToList();
                           if (images.Any())
                           {
                               StockUpload img;
                               var front = images.Find(f => f.ImageView.Name.Contains("Front"));
                               if (front != null && front.StockUploadId > 0)
                               {
                                   img = front;
                               }
                               else
                               {
                                   img = images[0];
                               }

                               i.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "") ;
                           }

                       }); 
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new OnlineStoreObject();
            }
        }

        public StoreItemStockObject GetProductDetails(long id)
        {
            try
            {
                using (var db = _db)
                {
                    var items = (from it in db.StoreItemStocks.Where(d => d.StoreItemStockId == id && d.QuantityInStock > 0)
                                  join st in  db.StoreItems on it.StoreItemId equals st.StoreItemId
                                  join ct in  db.StoreItemCategories on st.StoreItemCategoryId equals ct.StoreItemCategoryId
                                  join stt in  db.StoreItemTypes on st.StoreItemTypeId equals stt.StoreItemTypeId
                                  join b in  db.StoreItemBrands on st.StoreItemBrandId equals b.StoreItemBrandId
                                  join c in  db.StoreCurrencies on it.StoreCurrencyId equals c.StoreCurrencyId
                                  join v in db.StoreItemVariations on it.StoreItemVariationId equals v.StoreItemVariationId
                                  join p in db.StoreItemVariationValues on it.StoreItemVariationValueId equals p.StoreItemVariationValueId
                                  join pr in db.ItemPrices.OrderBy(p => p.MinimumQuantity) on it.StoreItemStockId equals pr.StoreItemStockId
                                  join uom in db.UnitsOfMeasurements on pr.UoMId equals uom.UnitOfMeasurementId
                                 select new StoreItemStockObject
                                 {
                                    StoreItemStockId = it.StoreItemStockId,
                                    StoreOutletId = it.StoreOutletId,
                                    SKU = it.SKU,
                                    StoreItemVariationId = it.StoreItemVariationId,
                                    StoreItemVariationValueId = it.StoreItemVariationValueId,
                                    StoreItemBrandId = b.StoreItemBrandId,
                                    StoreItemTypeId = stt.StoreItemTypeId,
                                    StoreItemCategoryId = ct.StoreItemCategoryId,
                                    StoreItemId = it.StoreItemId,
                                    QuantityInStock = it.QuantityInStock,
                                    ReorderLevel = it.ReorderLevel,
                                    ReorderQuantity = it.ReorderQuantity,
                                    LastUpdated = it.LastUpdated,
                                    ShelfLocation = it.ShelfLocation,
                                    ExpirationDate = it.ExpirationDate,
                                    TotalQuantityAlreadySold = it.TotalQuantityAlreadySold,
                                    StoreCurrencyId = it.StoreCurrencyId,
                                    Price = pr.Price,
                                    CurrencySymbol = c.Symbol,
                                    VariationProperty = v.VariationProperty,
                                    VariationValue = p.Value,
                                    UoMCode = uom.UoMCode,
                                    UnitOfMeasurementId = uom.UnitOfMeasurementId
                                 }).ToList();
                    if (!items.Any())
                    {
                        return new StoreItemStockObject();
                    }
                    
                    var item = items[0];

                    var itemUploads = (from stk in db.StockUploads.Where(d => d.StoreItemStockId == item.StoreItemStockId).Include("ImageView") 
                        select new StockUploadObject
                        {
                            StockUploadId = stk.StockUploadId,
                            StoreItemStockId = stk.StoreItemStockId,
                            ImageViewId = stk.ImageViewId,
                            ViewName = stk.ImageView.Name,
                            ImagePath = string.IsNullOrEmpty(stk.ImagePath) ? "/Content/images/noImage.png" : stk.ImagePath.Replace("~", "")
                        }).ToList();


                    if (!itemUploads.Any())
                    {
                        return new StoreItemStockObject();
                    }

                    item.StockUploadObjects = new List<StockUploadObject>();
                    item.StockUploadObjects = itemUploads;

                    var similarItems = (from st in db.StoreItems.Where(z => z.StoreItemTypeId == item.StoreItemTypeId).OrderBy(m => m.StoreItemId).Include("StoreItemCategory").Include("StoreItemBrand").Include("StoreItemType").Take(5)
                                        join stk in db.StoreItemStocks.Where(d => d.QuantityInStock > 0 && d.StoreItemStockId != item.StoreItemStockId).Include("StoreCurrency") on st.StoreItemId equals stk.StoreItemId
                                        join pr in db.ItemPrices.OrderBy(p => p.MinimumQuantity) on stk.StoreItemStockId equals pr.StoreItemStockId
                                 join img in db.StockUploads on stk.StoreItemStockId equals img.StoreItemStockId
                                 select new StoreItemObject
                                 {
                                     StoreItemId = st.StoreItemId,
                                     Price = pr.Price,
                                     CurrencySymbol = stk.StoreCurrency.Symbol,
                                     StoreItemBrandName = st.StoreItemBrand.Name,
                                     StoreItemTypeName = st.StoreItemType.Name,
                                     StoreItemCategoryName = st.StoreItemCategory.Name,
                                     StoreItemStockId = stk.StoreItemStockId,
                                     StoreItemBrandId = st.StoreItemBrandId,
                                     StoreItemTypeId = st.StoreItemTypeId,
                                     StoreItemCategoryId = st.StoreItemCategoryId,
                                     ChartOfAccountId = st.ChartOfAccountId,
                                     Name = st.Name,
                                     Description = st.Description,
                                     ParentItemId = st.ParentItemId,
                                     ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "")
                                 }).ToList();

                    item.SimilarItems = similarItems;
                    return item;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemStockObject();
            }
        }

        public List<VariationObject> GetItemVariations(long storeItemId)
        {
            try
            {
                using (var db = _db)
                {
                    
                    var itemVariations = (from st in db.StoreItemStocks.Where(k => k.StoreItemId == storeItemId)
                                          join iv in db.StoreItemVariations on st.StoreItemVariationId equals iv.StoreItemVariationId
                                          join ivv in db.StoreItemVariationValues on st.StoreItemVariationValueId equals ivv.StoreItemVariationValueId
                                          select new ItemVariationObject
                                          {
                                              StoreItemVariationName = iv.VariationProperty,
                                              StoreItemVariationValueName = ivv.Value,
                                              StoreItemVariationId = iv.StoreItemVariationId,
                                              StoreItemVariationValueId = ivv.StoreItemVariationValueId
                                          }).ToList();

                    var variations = new List<VariationObject>();
                    if (itemVariations.Any())
                    {
                        itemVariations.ForEach(v =>
                        {
                            var existing = variations.Find(j => j.StoreItemVariation == v.StoreItemVariationName);
                            if (existing == null)
                            {
                                var variation = new VariationObject
                                {
                                    StoreItemVariation = v.StoreItemVariationName,
                                    StoreItemVariationId = v.StoreItemVariationId,
                                    StoreItemVariationValueObjects = new List<StoreItemVariationValueObject>()
                                };
                                variation.StoreItemVariationValueObjects.Add(new StoreItemVariationValueObject
                                {
                                    StoreItemVariationValueId = v.StoreItemVariationValueId,
                                    Value = v.StoreItemVariationValueName
                                });

                                variations.Add(variation);
                            }
                            else
                            {
                                var variationValue = existing.StoreItemVariationValueObjects.Find(m => m.StoreItemVariationValueId ==  v.StoreItemVariationValueId);
                                if (variationValue == null)
                                {
                                    existing.StoreItemVariationValueObjects.Add(new StoreItemVariationValueObject
                                    {
                                        StoreItemVariationValueId = v.StoreItemVariationValueId,
                                        Value = v.StoreItemVariationValueName
                                    });
                                }
                            }
                        });
                    }

                    return variations;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<VariationObject>();
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

        public ShoppingCartObject GetShoppingCart(long customerId)
        {
            try
            {
                using (var db = _db)
                {
                    var shoppingCarts = (from s in db.ShoppingCarts.Where(s => s.DeliveryStatus == 1 && s.CustomerId == customerId)
                                         select new ShoppingCartObject
                    {
                        ShoppingCartId = s.ShoppingCartId,
                        CustomerId = s.CustomerId,
                        DeliveryStatus = s.DeliveryStatus
                    }).ToList();

                    if (!shoppingCarts.Any())
                    {
                        return new ShoppingCartObject();
                    }
                    
                    var shoppingCart = shoppingCarts[0];
                    var cartItems = (from sci in db.ShopingCartItems.Where(i => i.ShopingCartId == shoppingCart.ShoppingCartId) 
                                     join stk in db.StoreItemStocks.Where(d => d.QuantityInStock > 0).Include("StoreCurrency") on sci.StoreItemStockId equals stk.StoreItemStockId
                                     join st in db.StoreItems.Include("StoreItemCategory").Include("StoreItemBrand").Include("StoreItemType") on stk.StoreItemId equals st.StoreItemId
                                     join ui in db.UnitsOfMeasurements on sci.UoMId equals ui.UnitOfMeasurementId
                                     join br in db.StoreItemBrands on st.StoreItemBrandId equals br.StoreItemBrandId
                                     join tp in db.StoreItemTypes on st.StoreItemTypeId equals tp.StoreItemTypeId
                                     join ct in db.StoreItemCategories on st.StoreItemCategoryId equals ct.StoreItemCategoryId
                                     join vv in db.StoreItemVariationValues on stk.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     join cr in db.StoreCurrencies on stk.StoreCurrencyId equals cr.StoreCurrencyId
                                     join img in db.StockUploads.OrderBy(g => g.ImageView.Name).Take(1) on stk.StoreItemStockId equals img.StoreItemStockId
                                     join v in db.StoreItemVariations on stk.StoreItemVariationId equals v.StoreItemVariationId
                                     join p in db.StoreItemVariationValues on stk.StoreItemVariationValueId equals p.StoreItemVariationValueId
                                     select new ShopingCartItemObject
                                     {
                                        ShopingCartItemId = sci.ShopingCartItemId,
                                        ShopingCartId = sci.ShopingCartId,
                                        StoreItemStockId = sci.StoreItemStockId,
                                        UnitPrice = sci.UnitPrice,
                                        QuantityOrdered = sci.QuantityOrdered,
                                        UoMId = sci.UoMId,
                                        SKU = stk.SKU,
                                        UoMCode = ui.UoMCode,
                                        StoreItemBrandName = st.StoreItemBrand.Name,
                                        StoreItemTypeName = st.StoreItemType.Name,
                                        StoreItemCategoryName = st.StoreItemCategory.Name,
                                        Name = st.Name,
                                        Description = st.Description,
                                        StoreItemBrandId = br.StoreItemBrandId,
                                        StoreItemTypeId = tp.StoreItemTypeId,
                                        StoreItemCategoryId = ct.StoreItemCategoryId,
                                        CurencySymbol = stk.StoreCurrency.Symbol,
                                        Discount = sci.Discount,
                                        VariationProperty = v.VariationProperty,
                                        VariationValue = p.Value,
                                        ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "") 
                                     }).ToList();

                    if (!cartItems.Any())
                    {
                        return new ShoppingCartObject();
                    }

                    shoppingCart.ShopingCartItemObjects = new List<ShopingCartItemObject>();
                    shoppingCart.ShopingCartItemObjects = cartItems;
                    return shoppingCart;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ShoppingCartObject();
            }
        }

        public ShoppingCartObject GetShoppingCart(string customerIpAddress)
        {
            try
            {
                using (var db = _db)
                {
                    var shoppingCarts = (from s in db.ShoppingCarts.Where(s => s.DeliveryStatus == 1 && s.CustomerIpAddress == customerIpAddress)
                                         select new ShoppingCartObject
                                         {
                                             ShoppingCartId = s.ShoppingCartId,
                                             CustomerId = s.CustomerId,
                                             DeliveryStatus = s.DeliveryStatus
                                         }).ToList();

                    if (!shoppingCarts.Any())
                    {
                        return new ShoppingCartObject();
                    }

                    var shoppingCart = shoppingCarts[0];
                    var cartItems = (from sci in db.ShopingCartItems.Where(i => i.ShopingCartId == shoppingCart.ShoppingCartId)
                                     join sk in db.StoreItemStocks on sci.StoreItemStockId equals sk.StoreItemStockId
                                     join st in db.StoreItems on sk.StoreItemId equals st.StoreItemId
                                     join br in db.StoreItemBrands on st.StoreItemBrandId equals br.StoreItemBrandId
                                     join tp in db.StoreItemTypes on st.StoreItemTypeId equals tp.StoreItemTypeId
                                     join ct in db.StoreItemCategories on st.StoreItemCategoryId equals ct.StoreItemCategoryId
                                     join vv in db.StoreItemVariationValues on sk.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     join cr in db.StoreCurrencies on sk.StoreCurrencyId equals cr.StoreCurrencyId
                                     join stu in db.StockUploads.OrderBy(g => g.ImageView.Name).Take(1) on sk.StoreItemStockId equals stu.StoreItemStockId
                                     join ui in db.UnitsOfMeasurements on sci.UoMId equals ui.UnitOfMeasurementId

                                     select new ShopingCartItemObject
                                     {
                                         ShopingCartItemId = sci.ShopingCartItemId,
                                         ShopingCartId = sci.ShopingCartId,
                                         StoreItemStockId = sci.StoreItemStockId,
                                         UnitPrice = sci.UnitPrice,
                                         QuantityOrdered = sci.QuantityOrdered,
                                         UoMId = sci.UoMId,
                                         UoMCode = ui.UoMCode,
                                         Discount = sci.Discount,
                                         StoreItemBrandName = br.Name,
                                         StoreItemTypeName = tp.Name,
                                         StoreItemCategoryName = ct.Name,
                                         Name = st.Name,
                                         Description = st.Description,
                                         StoreItemBrandId = br.StoreItemBrandId,
                                         StoreItemTypeId = tp.StoreItemTypeId,
                                         StoreItemCategoryId = ct.StoreItemCategoryId,
                                         ImagePath = string.IsNullOrEmpty(stu.ImagePath)? "/Content/images/noImage.png" : stu.ImagePath.Replace("~", ""),
                                         SKU = sk.SKU,
                                         VariationProperty = vv.Value,
                                         CurencySymbol = cr.Symbol

                                     }).ToList();

                    if (!cartItems.Any())
                    {
                        return new ShoppingCartObject();
                    }

                    shoppingCart.ShopingCartItemObjects = new List<ShopingCartItemObject>();
                    shoppingCart.ShopingCartItemObjects = cartItems;
                    return shoppingCart;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ShoppingCartObject();
            }
        }
        
        public List<DeliveryAddressObject> GetCustomerPreviousAddresses(string customerIpAddress)
        {
            try
            {
                using (var db = _db)
                {
                    var addresses = (from s in db.DeliveryAddresses.Where(s => s.CustomerIpAddress == customerIpAddress)
                                         join ct in db.StoreCities on s.CityId equals ct.StoreCityId
                                         join st in db.StoreStates on ct.StoreStateId equals st.StoreStateId
                                         join ctr in db.StoreCountries on st.StoreCountryId equals ctr.StoreCountryId
                                         select new DeliveryAddressObject
                                         {
                                            AddressLine1 = s.AddressLine1,
                                            AddressLine2 = s.AddressLine2,
                                            CityId = s.CityId,
                                            StateId = st.StoreStateId,
                                            CountryId = ctr.StoreCountryId,
                                            CustomerId = s.CustomerId,
                                            CustomerIpAddress = s.CustomerIpAddress,
                                            CityName = ct.Name,
                                            StateName  = st.Name,
                                            CountryName = st.Name
                                         }).ToList();

                    if (!addresses.Any())
                    {
                        return new List<DeliveryAddressObject>();
                    }

                    return addresses;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DeliveryAddressObject>();
            }
        }

        public List<DeliveryAddressObject> GetCustomerPreviousAddresses(long customerId)
        {
            try
            {
                using (var db = _db)
                {
                    var addresses = (from s in db.DeliveryAddresses.Where(s => s.CustomerId == customerId)
                                     join ct in db.StoreCities on s.CityId equals ct.StoreCityId
                                     join st in db.StoreStates on ct.StoreStateId equals st.StoreStateId
                                     join ctr in db.StoreCountries on st.StoreCountryId equals ctr.StoreCountryId
                                     select new DeliveryAddressObject
                                     {
                                         AddressLine1 = s.AddressLine1,
                                         AddressLine2 = s.AddressLine2,
                                         CityId = s.CityId,
                                         StateId = st.StoreStateId,
                                         CountryId = ctr.StoreCountryId,
                                         CustomerId = s.CustomerId,
                                         CustomerIpAddress = s.CustomerIpAddress,
                                         CityName = ct.Name,
                                         StateName = st.Name,
                                         CountryName = st.Name
                                     }).ToList();

                    if (!addresses.Any())
                    {
                        return new List<DeliveryAddressObject>();
                    }

                    return addresses;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DeliveryAddressObject>();
            }
        }
        
        public ShoppingCartObject ProcessShoppingCart(ShoppingCartObject cart)
        {
            try
            {
                using (var db = _db)
                {
                    List<ShoppingCart> shoppingCarts;

                    if (cart.CustomerId != null && cart.CustomerId > 0)
                    {
                        shoppingCarts = db.ShoppingCarts.Where(s => s.DeliveryStatus == 1 && s.CustomerId == cart.CustomerId).ToList();
                    }
                    else
                    {
                        shoppingCarts = db.ShoppingCarts.Where(s => s.DeliveryStatus == 1 && s.CustomerIpAddress == cart.CustomerIpAddress).ToList();
                    }

                    var shoppingCart = new ShoppingCartObject();
                    long cartId;
                    if (!shoppingCarts.Any())
                    {
                        var cartEntity = ModelCrossMapper.Map<ShoppingCartObject, ShoppingCart>(cart);
                        if (cartEntity == null || (cartEntity.CustomerId == null && string.IsNullOrEmpty(cartEntity.CustomerIpAddress)))
                        {
                            return new ShoppingCartObject();
                        }

                        var processedCart = db.ShoppingCarts.Add(cartEntity);
                        db.SaveChanges();
                        cartId = processedCart.ShoppingCartId;
                        cart.ShoppingCartId = cartId;
                    }
                    else
                    {
                        cartId = shoppingCarts[0].ShoppingCartId; 
                    }

                    if (cartId < 1)
                    {
                        return new ShoppingCartObject();
                    }

                    var cartItems = db.ShopingCartItems.Where(i => i.ShopingCartId == shoppingCart.ShoppingCartId).ToList();

                    var successCount = 0;
                    
                    if (!cartItems.Any())
                    {
                        cart.ShopingCartItemObjects.ForEach(i =>
                        {
                            var cartItemEntity = ModelCrossMapper.Map<ShopingCartItemObject, ShopingCartItem>(i);
                            if (cartItemEntity == null || cartItemEntity.QuantityOrdered < 1)
                            {
                                return;
                            }

                            cartItemEntity.ShopingCartId = cartId;
                            var item = db.ShopingCartItems.Add(cartItemEntity);
                            db.SaveChanges();
                            i.ShopingCartItemId = item.ShopingCartItemId;
                            successCount += 1;

                            var uploads = db.StockUploads.Where(k => k.StoreItemStockId == i.StoreItemStockId).ToList();
                            if (uploads.Any())
                            {
                                var img = uploads[0];
                                i.ImagePath = string.IsNullOrEmpty(img.ImagePath)? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");
                            }
                            else
                            {
                                i.ImagePath = "/Content/images/noImage.png";
                            }
                        });
                    }
                    else
                    {
                        cart.ShopingCartItemObjects.ForEach(i =>
                        {
                            var existingItem = cartItems.Find(u => u.StoreItemStockId == i.StoreItemStockId);
                            if (existingItem != null && existingItem.StoreItemStockId > 0)
                            {
                                existingItem.QuantityOrdered += i.QuantityOrdered;
                                db.Entry(existingItem).State = EntityState.Modified;
                                db.SaveChanges();
                                successCount += 1;
                                i.ShopingCartItemId = existingItem.ShopingCartItemId;
                            }
                            else
                            {
                                var cartItemEntity = ModelCrossMapper.Map<ShopingCartItemObject, ShopingCartItem>(i);
                                if (cartItemEntity == null || cartItemEntity.QuantityOrdered < 1)
                                {
                                    return;
                                }

                                cartItemEntity.ShopingCartId = cartId;
                                var item = db.ShopingCartItems.Add(cartItemEntity);
                                db.SaveChanges();
                                i.ShopingCartItemId = item.ShopingCartItemId;
                                successCount += 1;

                                var uploads = db.StockUploads.Where(k => k.StoreItemStockId == i.StoreItemStockId).ToList();
                                if (uploads.Any())
                                {
                                    var img = uploads[0];
                                    i.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");
                                }
                                else
                                {
                                    i.ImagePath = "/Content/images/noImage.png";
                                }
                            }
                            
                        });

                       cartItems.ForEach(i =>
                        {
                            var existingItem = cart.ShopingCartItemObjects.Find(u => u.StoreItemStockId == i.StoreItemStockId);
                            if (existingItem == null)
                            {
                                db.ShopingCartItems.Remove(i);
                                db.SaveChanges();
                            }

                        });
                    }

                    return successCount != cart.ShopingCartItemObjects.Count ? new ShoppingCartObject() : cart;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ShoppingCartObject();
            }
        }

        public ShopingCartItemObject ProcessShoppingCartItem(ShopingCartItemObject cartItem)
        {
            try
            {
                using (var db = _db)
                {
                    var cartItems = db.ShopingCartItems.Where(i => i.ShopingCartId == cartItem.ShopingCartId).ToList();

                    if (!cartItems.Any())
                    {
                        return new ShopingCartItemObject();
                    }

                    var existing = new ShopingCartItem();

                    if (cartItem.ShopingCartItemId > 0)
                    {
                        var existingItem = cartItems.Find(u => u.ShopingCartItemId == cartItem.ShopingCartItemId);
                        if (existingItem != null && existingItem.StoreItemStockId > 0)
                        {
                            existing.QuantityOrdered += cartItem.QuantityOrdered;
                            db.Entry(existing).State = EntityState.Modified;
                            db.SaveChanges();
                            cartItem.ShopingCartItemId = existing.ShopingCartItemId;
                            var uploads = db.StockUploads.Where(k => k.StoreItemStockId == cartItem.StoreItemStockId).ToList();
                            if (uploads.Any())
                            {
                                var img = uploads[0];
                                cartItem.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");
                            }
                            else
                            {
                                cartItem.ImagePath = "/Content/images/noImage.png";
                            }
                            return cartItem;
                        }

                        return new ShopingCartItemObject();
                    }

                    var cartItemEntity = ModelCrossMapper.Map<ShopingCartItemObject, ShopingCartItem>(cartItem);
                    if (cartItemEntity == null || cartItemEntity.QuantityOrdered < 1)
                    {
                        return new ShopingCartItemObject();
                    }
                    var item = db.ShopingCartItems.Add(cartItemEntity);
                    db.SaveChanges();
                    cartItem.ShopingCartItemId = item.ShopingCartItemId;
                    return cartItem;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ShopingCartItemObject();
            }
        }

        public long UpdateShoppingCartItem(CartItemObject cartItemObject)
        {
            try
            {
                using (var db = _db)
                {

                    var cartItems = db.ShopingCartItems.Where(i => i.ShopingCartItemId == cartItemObject.ShoppinCartItemId).ToList();
                    if (!cartItems.Any())
                    {
                        return 0;
                    }
                    var cartItem = cartItems[0];
                    cartItem.QuantityOrdered = cartItemObject.QuantityOrdered;
                    db.Entry(cartItem).State = EntityState.Modified;
                    db.SaveChanges();
                    return 5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long RemoveShoppingCartItem(long cartItemId)
        {
            try
            {
                using (var db = _db)
                {

                    var cartItems = db.ShopingCartItems.Where(i => i.ShopingCartItemId == cartItemId).ToList();
                    if (!cartItems.Any())
                    {
                        return 0;
                    }
                    var cartItem = cartItems[0];
                    db.ShopingCartItems.Remove(cartItem);
                    db.SaveChanges();

                    if (db.ShopingCartItems.Count(i => i.ShopingCartId == cartItem.ShopingCartId) < 1)
                    {
                        var carts = db.ShoppingCarts.Where(k => k.ShoppingCartId == cartItem.ShopingCartId).ToList();
                        if (!carts.Any())
                        {
                            return 0;
                        }

                        var cart = carts[0];
                        db.ShoppingCarts.Remove(cart);
                        db.SaveChanges();
                        return 10;
                    }
                    return 5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long DeleteShoppingCart(long cartId)
        {
            try
            {
                using (var db = _db)
                {


                    var shoppingCarts = db.ShoppingCarts.Where(s => s.ShoppingCartId == cartId).ToList();

                    if (!shoppingCarts.Any())
                    {
                        return 0;
                    }
                    var cart = shoppingCarts[0];
                    var cartItems = db.ShopingCartItems.Where(i => i.ShopingCartId == cart.ShoppingCartId).ToList();
                    if (!cartItems.Any())
                    {
                        return 0;
                    }

                    cartItems.ForEach(i =>
                    {
                        db.ShopingCartItems.Remove(i);
                        db.SaveChanges();

                    });

                    db.ShoppingCarts.Remove(cart);
                    db.SaveChanges();
                    return  5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<StoreCountryObject> GetCountries()
        {
            try
            {
                using (var db = _db)
                {
                    var countries = db.StoreCountries.OrderBy(i => i.Name).ToList();

                    if (!countries.Any())
                    {
                        return new List<StoreCountryObject>();
                    }
                    
                    var objList = new List<StoreCountryObject>();
                    countries.ForEach(m =>
                    {
                        var countryObject = ModelCrossMapper.Map<StoreCountry, StoreCountryObject>(m);
                        if (countryObject != null && countryObject.StoreCountryId > 0)
                        {
                            objList.Add(countryObject);
                        }
                    });

                    return objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCountryObject>();
            }
        }

        public List<StoreStateObject> GetCountryStates(long countryId)
        {
            try
            {
                using (var db = _db)
                {
                    var states = db.StoreStates.Where(i => i.StoreCountryId == countryId).ToList();
                    if (!states.Any())
                    {
                        return new List<StoreStateObject>();
                    }

                    var objList = new List<StoreStateObject>();
                    states.ForEach(m =>
                    {
                        var stateObject = ModelCrossMapper.Map<StoreState, StoreStateObject>(m);
                        if (stateObject != null && stateObject.StoreCountryId > 0)
                        {
                            objList.Add(stateObject);
                        }
                    });

                    return objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreStateObject>();
            }
        }

        public List<StoreStateObject> GetStates()
        {
            try
            {
                using (var db = _db)
                {
                    var states = db.StoreStates.OrderBy(i => i.Name).ToList();
                    if (!states.Any())
                    {
                        return new List<StoreStateObject>();
                    }

                    var objList = new List<StoreStateObject>();
                    states.ForEach(m =>
                    {
                        var stateObject = ModelCrossMapper.Map<StoreState, StoreStateObject>(m);
                        if (stateObject != null && stateObject.StoreCountryId > 0)
                        {
                            objList.Add(stateObject);
                        }
                    });

                    return objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreStateObject>();
            }
        }

        public List<StoreCityObject> GetStateCities(long stateId)
        {
            try
            {
                using (var db = _db)
                {
                    var cities = db.StoreCities.Where(i => i.StoreStateId == stateId).ToList();
                    if (!cities.Any())
                    {
                        return new List<StoreCityObject>();
                    }

                    var objList = new List<StoreCityObject>();
                    cities.ForEach(m =>
                    {
                        var cityObject = ModelCrossMapper.Map<StoreCity, StoreCityObject>(m);
                        if (cityObject != null && cityObject.StoreCityId > 0)
                        {
                            objList.Add(cityObject);
                        }
                    });

                    return objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCityObject>();
            }
        }

        public List<StoreCityObject> GetCities()
        {
            try
            {
                using (var db = _db)
                {
                    var cities = db.StoreCities.OrderBy(i => i.Name).ToList();
                    if (!cities.Any())
                    {
                        return new List<StoreCityObject>();
                    }

                    var objList = new List<StoreCityObject>();
                    cities.ForEach(m =>
                    {
                        var cityObject = ModelCrossMapper.Map<StoreCity, StoreCityObject>(m);
                        if (cityObject != null && cityObject.StoreCityId > 0)
                        {
                            objList.Add(cityObject);
                        }
                    });

                    return objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCityObject>();
            }
        }

        public long ProcessCartCheckout(DeliveryAddressObject deliveryAddress)
        {
            try
            {
                using (var db = _db)
                {
                   var shoppingCarts = db.ShoppingCarts.Where(s => s.DeliveryStatus == 1 && s.ShoppingCartId == deliveryAddress.ShoppingCartId).ToList();
                    if (!shoppingCarts.Any())
                    {
                        return -2;
                    }

                    var cart = shoppingCarts[0];
                    if (!string.IsNullOrEmpty(deliveryAddress.CouponCode))
                    {
                        var coupons = db.Coupons.Where(i => i.Code == deliveryAddress.CouponCode).ToList();
                        if (!coupons.Any())
                        {
                            return 0;
                        }

                        var coupon = coupons[0];
                        cart.CouponId = coupon.CouponId;
                    }

                    var cartItems = db.ShopingCartItems.Where(c => c.ShopingCartId == cart.ShoppingCartId).ToList();
                     if (!cartItems.Any())
                    {
                        return -2;
                    }

                    var cartAddress = new DeliveryAddress
                    {
                        AddressLine1 = deliveryAddress.AddressLine1,
                        AddressLine2 = deliveryAddress.AddressLine2,
                        ContactEmail = deliveryAddress.ContactEmail,
                        CityId = deliveryAddress.CityId,
                        CustomerId = deliveryAddress.CustomerId,
                        CustomerIpAddress = deliveryAddress.CustomerIpAddress,
                    };

                    var processedDeliveryAddress = db.DeliveryAddresses.Add(cartAddress);
                    db.SaveChanges();
                    
                    cart.DeliveryAddressId = processedDeliveryAddress.Id;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();

                    var totalAmountDue = cartItems.Sum(v => v.QuantityOrdered*v.UnitPrice);
                    var invoice = new ShoppingCartInvoice
                    {
                        ShoppingCartId = cart.ShoppingCartId,
                        PaymentStatus = 1,
                        PaymentReference = string.Empty,
                        AmountDue = totalAmountDue,
                        PaymentTypeId = deliveryAddress.PaymentTypeId,
                        DateGenerated = DateTime.Now
                    };

                    db.ShoppingCartInvoices.Add(invoice);
                    db.SaveChanges();
                    
                    return 5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public CouponObject GetCouponInfo(string couponCode)
        {
            try
            {
                using (var db = _db)
                {
                    var coupons = db.Coupons.Where(i => i.Code == couponCode).ToList();
                    if (!coupons.Any())
                    {
                        return new CouponObject();
                    }

                    var coupon = coupons[0];

                    if (coupon.ValidFrom > DateTime.Now || coupon.ValidTo < DateTime.Now)
                    {
                        return new CouponObject();
                    }
                    var couponObject = ModelCrossMapper.Map<Coupon, CouponObject>(coupon);
                    if (couponObject == null || couponObject.CouponId < 1)
                    {
                        return new CouponObject();
                    }
                    return couponObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CouponObject();
            }
        }

        public UserProfileObject GetUserProfile(string aspnetId)
        {
            try
            {
                using (var db = _db)
                {
                    var userProfiles =( from asp in db.AspNetUsers.Where(i => i.Id == aspnetId)
                                   join pr in db.UserProfiles.Where(p => p.IsActive) on asp.UserInfo_Id equals pr.Id
                                   select new UserProfileObject
                                   {
                                        Id = pr.Id,
                                        LastName = pr.LastName,
                                        OtherNames = pr.OtherNames,
                                        Gender = pr.Gender,
                                        Birthday = pr.Birthday,
                                        PhotofilePath = pr.PhotofilePath,
                                        IsActive = pr.IsActive
                                   }).ToList();

                    if (!userProfiles.Any())
                    {
                        return new UserProfileObject();
                    }
                    
                    return userProfiles[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new UserProfileObject();
            }
        }
    }
}


