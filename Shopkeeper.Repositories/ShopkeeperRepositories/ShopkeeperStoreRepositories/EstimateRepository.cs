using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class EstimateRepository
    {
       private readonly IShopkeeperRepository<SalePayment> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _db;

       public EstimateRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<SalePayment>(_uoWork);
		}

        public long AddEstimate(EstimateObject estimate, out string estimateNumber, out long customerId)
        {
            long pId = 0;
            try
            {
                if (estimate == null || estimate.EstimateItemObjects == null || !estimate.EstimateItemObjects.Any())
                {
                    estimateNumber = "";
                    customerId = 0;
                    return -2;
                }
                
                using (var db = _db)
                {
                    var orderEntity = ModelCrossMapper.Map<EstimateObject, Estimate>(estimate);
                    if (orderEntity == null || orderEntity.CreatedById < 1)
                    {
                        estimateNumber = "";
                        customerId = 0;
                        return -2;
                    }

                    long csId = 0;

                    if (estimate.CustomerObject != null && estimate.CustomerObject.CustomerId < 1 &&
                        estimate.CustomerId < 1 && !string.IsNullOrEmpty(estimate.CustomerObject.LastName) &&
                        !string.IsNullOrEmpty(estimate.CustomerObject.OtherNames) && !string.IsNullOrEmpty(estimate.CustomerObject.MobileNumber))
                    {
                        var duplicateNumbers = db.UserProfiles.Count(d => d.MobileNumber.Trim() == estimate.CustomerObject.MobileNumber.Trim());
                        if (duplicateNumbers > 0)
                        {
                            estimateNumber = "";
                            customerId = 0;
                            return -7;
                        }

                        var cObj = estimate.CustomerObject;
                        var profileEntity = new UserProfile
                        {
                            Id = 0,
                            LastName = cObj.LastName,
                            OtherNames = cObj.OtherNames,
                            Gender = cObj.Gender,
                            Birthday = cObj.Birthday,
                            PhotofilePath = null,
                            IsActive = true,
                            ContactEmail = cObj.ContactEmail,
                            MobileNumber = cObj.MobileNumber,
                            OfficeLine = cObj.OfficeLine
                        };

                        var profile = db.UserProfiles.Add(profileEntity);
                        db.SaveChanges();

                        var customerEntity = ModelCrossMapper.Map<CustomerObject, Customer>(cObj);
                        if (customerEntity != null && !string.IsNullOrEmpty(estimate.CustomerObject.LastName))
                        {
                            customerEntity.DateProfiled = DateTime.Now;
                            customerEntity.UserId = profile.Id;
                            customerEntity.ContactPersonId = estimate.ContactPersonId;
                            customerEntity.StoreOutletId = estimate.StoreOutletId;
                            var customer = db.Customers.Add(customerEntity);
                            db.SaveChanges();
                            orderEntity.CustomerId = customer.CustomerId;
                            csId = customer.CustomerId;
                        }

                    }
                    else
                    {
                        csId = estimate.CustomerId;
                    }

                    var yrStr = DateTime.Now.ToString("yyyy/MM");

                    var code = yrStr.Replace("/", "");

                    var similarBatches = db.Estimates.Where(u => u.EstimateNumber.Contains(code)).ToList();
                    if (similarBatches.Any())
                    {
                        var tempList = new List<float>();
                        similarBatches.ForEach(x =>
                        {
                            float t;
                            var sprs = float.TryParse(x.EstimateNumber, out t);
                            if (sprs && t > 0)
                            {
                                tempList.Add(t);
                            }

                        });

                        if (tempList.Any())
                        {
                            var recent = tempList.OrderByDescending(k => k).ToList()[0];
                            orderEntity.EstimateNumber = (recent + 1).ToString(CultureInfo.InvariantCulture);
                        }

                        else
                        {
                            orderEntity.EstimateNumber = code + "1";
                        }
                    }

                    else
                    {
                        orderEntity.EstimateNumber = code + "1";
                    }

                    var processeEstimate = db.Estimates.Add(orderEntity);
                    db.SaveChanges();
                    pId = processeEstimate.Id;

                    estimate.EstimateItemObjects.ToList().ForEach(it =>
                    {
                        it.EstimateId = pId;

                        var itemEntity = ModelCrossMapper.Map<EstimateItemObject, EstimateItem>(it);
                        if (itemEntity != null && itemEntity.EstimateId > 0)
                        {
                            db.EstimateItems.Add(itemEntity);
                            db.SaveChanges();
                        }
                    });

                    estimateNumber = InvoiceNumberGenerator.GenerateNumber(pId);

                    processeEstimate.EstimateNumber = estimateNumber;
                    db.Entry(processeEstimate).State = EntityState.Modified;
                    db.SaveChanges();
                    customerId = csId;
                    return pId;
                }
               
            }
           
            catch (Exception e)
            {
               
                if (pId > 0)
                {
                    using (var db = _db)
                    {
                        var list =  db.EstimateItems.Where(g => g.EstimateId == pId).ToList();
                        if (list.Any())
                        {
                            list.ForEach(d =>
                            {
                                db.EstimateItems.Remove(d);
                                db.SaveChanges();
                            });
                        }

                        var items = db.Estimates.Where(g => g.Id == pId).ToList();
                        if (items.Any())
                        {
                            var item = items[0];
                            db.Estimates.Remove(item);
                            db.SaveChanges();
                           
                        }
                    }
                }
               
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                estimateNumber = "";
                customerId = 0;
                return 0;
            }
        }
        public long UpdateEstimate(EstimateObject estimate)
        {
            try
            {
                if (estimate == null || estimate.EstimateItemObjects == null || !estimate.EstimateItemObjects.Any())
                {
                    return -2;
                }
                
                using (var db = _db)
                {
                    var ius = db.EstimateItems.Where(t => t.EstimateId == estimate.Id).ToList();
                    if (!ius.Any())
                    {
                        return -2;
                    }

                    var orderEntity = ModelCrossMapper.Map<EstimateObject, Estimate>(estimate);
                    if (orderEntity == null || orderEntity.Id < 1)
                    {
                        return -2;
                    }

                    var items = estimate.EstimateItemObjects.ToList();

                    if (estimate.CustomerObject != null && estimate.CustomerObject.CustomerId < 1 && estimate.CustomerId < 1 && !string.IsNullOrEmpty(estimate.CustomerObject.LastName) && !string.IsNullOrEmpty(estimate.CustomerObject.OtherNames) && !string.IsNullOrEmpty(estimate.CustomerObject.MobileNumber))
                    {
                        var duplicateNumbers = db.UserProfiles.Count(d => d.MobileNumber.Trim() == estimate.CustomerObject.MobileNumber.Trim());
                        if (duplicateNumbers > 0)
                        {
                            return -7;
                        }
                        var cObj = estimate.CustomerObject;
                        var profileEntity = new UserProfile
                        {
                            Id = 0,
                            LastName = cObj.LastName,
                            OtherNames = cObj.OtherNames,
                            Gender = cObj.Gender,
                            Birthday = cObj.Birthday,
                            PhotofilePath = null,
                            IsActive = true,
                            ContactEmail = cObj.ContactEmail,
                            MobileNumber = cObj.MobileNumber,
                            OfficeLine = cObj.OfficeLine
                        };

                        var profile = db.UserProfiles.Add(profileEntity);
                        db.SaveChanges();

                        var customerEntity = ModelCrossMapper.Map<CustomerObject, Customer>(cObj);
                        if (customerEntity != null && !string.IsNullOrEmpty(estimate.CustomerObject.LastName))
                        {

                            customerEntity.UserId = profile.Id;
                            customerEntity.ContactPersonId = estimate.ContactPersonId;
                            customerEntity.StoreOutletId = estimate.StoreOutletId;
                            var customer = db.Customers.Add(customerEntity);
                            db.SaveChanges();
                            orderEntity.CustomerId = customer.CustomerId;
                        }

                    }

                    db.Entry(orderEntity).State = EntityState.Modified;
                    db.SaveChanges();

                    items.ForEach(it =>
                    {
                        if (it.Id < 1)
                        {
                            it.EstimateId = estimate.Id;
                            var itemEntity = ModelCrossMapper.Map<EstimateItemObject, EstimateItem>(it);
                            if (itemEntity != null && itemEntity.EstimateId > 0)
                            {
                                db.EstimateItems.Add(itemEntity);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var refItem = ius.Find(f => f.Id == it.Id);
                            if (refItem != null && refItem.Id > 0)
                            {
                                refItem.StoreItemStockId = it.StoreItemStockId;
                                refItem.QuantityRequested = it.QuantityRequested;
                                refItem.ItemPrice = it.ItemPrice;

                                refItem.EstimateId = it.EstimateId;

                                db.Entry(refItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            
                        }
                        
                    });

                    return orderEntity.Id;
                }
               
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }
                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return 0;
            }
        }

        public long ConvertEstimateToInvoice(string estimateNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(estimateNumber))
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(m => m.EstimateNumber == estimateNumber).ToList();
                    if (!estimates.Any())
                    {
                        return -2;
                    }
                    var estimate = estimates[0];
                    estimate.ConvertedToInvoice = true;
                    db.Entry(estimate).State = EntityState.Modified;
                    db.SaveChanges();
                    return 5;
                }

            }
            catch (Exception e)
            {
                
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return 0;
            }
        }

        public long UpdateStoreItemStock(StoreItemSoldObject itemSold)
        {
            try
            {
                if (itemSold == null)
                {
                    return -2;
                }
                using (var db = _db)
                {

                    var itemSoldEntity = ModelCrossMapper.Map<StoreItemSoldObject, StoreItemSold>(itemSold);
                    if (itemSoldEntity == null || itemSoldEntity.StoreItemStockId < 1)
                    {
                        return -2;
                    }

                    itemSoldEntity.QuantityDelivered = itemSoldEntity.QuantitySold;
                    itemSoldEntity.QuantityBalance = 0;

                    var soldItem = db.StoreItemSolds.Add(itemSoldEntity);
                    db.SaveChanges();
                    return soldItem.StoreItemSoldId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public long AddStoreTransaction(StoreTransactionObject transaction)
        {
            try
            {
                if (transaction == null)
                {
                    return -2;
                }

                var transactionEntity = ModelCrossMapper.Map<StoreTransactionObject, StoreTransaction>(transaction);
                if (transactionEntity == null || transactionEntity.StoreTransactionTypeId < 1)
                {
                    return -2;
                }
                using (var db = _db)
                {
                    var returnStatus = db.StoreTransactions.Add(transactionEntity);
                    db.SaveChanges();
                    return returnStatus.StoreTransactionId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public long AddSaleTransaction(SaleTransactionObject saleTransaction)
        {
            try
            {
                if (saleTransaction == null)
                {
                    return -2;
                }

                var saleTransactionEntity = ModelCrossMapper.Map<SaleTransactionObject, SaleTransaction>(saleTransaction);
                if (saleTransactionEntity == null || saleTransactionEntity.StoreTransactionId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var returnStatus = db.SaleTransactions.Add(saleTransactionEntity);
                    db.SaveChanges();
                    return returnStatus.SaleTransactionId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long DeleteEstimateItem(long estimateItemId)
        {
            try
            {
                if (estimateItemId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var items = db.EstimateItems.Where(o => o.Id == estimateItemId && o.Estimate.ConvertedToInvoice == false).Include("Estimate").ToList();
                    if (!items.Any())
                    {
                        return -2;
                    }
                    
                    var item = items[0];
                    var estimate = item.Estimate;
                    var count = estimate.EstimateItems.Count();
                    if (count < 2)
                    {
                        return -2;
                    }

                    db.EstimateItems.Remove(item); 
                    db.SaveChanges();

                    var refreshedItems = db.EstimateItems.Where(h => h.EstimateId == item.Id).ToList();
                    if (!refreshedItems.Any())
                    {
                        db.Estimates.Remove(estimate);
                        db.SaveChanges(); 
                    }

                    return 5;
                }

            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }
                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return 0;
            }
        }

        public long DeleteEstimate(long estimateId)
        {
            try
            {
                if (estimateId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => o.Id == estimateId && o.ConvertedToInvoice == false).Include("EstimateItems").ToList();
                    if (!estimates.Any())
                    {
                        return -2;
                    }

                    var item = estimates[0];
                    var estimateItems = item.EstimateItems.ToList();
                    if (!estimateItems.Any())
                    {
                        return -2;
                    }

                    estimateItems.ForEach(e =>
                    {
                        db.EstimateItems.Remove(e);
                        db.SaveChanges();
                    });

                    db.Estimates.Remove(item);
                    db.SaveChanges();

                    return 5;
                }

            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }
                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return 0;
            }
        }

        public EstimateObject GetEstimate(long estimateId)
        {
            try
            {
                using (var db = _db)
                {
                    var orders = db.Estimates.Where(m => m.Id == estimateId).Include("Customer").Include("UserProfile").Include("StoreOutlet").ToList();
                    if (!orders.Any())
                    {
                        return new EstimateObject();           
                    }

                    var order = orders[0];
                    var customers = db.Customers.Where(i => i.CustomerId == order.CustomerId).Include("UserProfile").ToList();
                    if (!customers.Any())
                    {
                        return new EstimateObject();
                    }
                    var customer = customers[0];

                    var employee = order.UserProfile;

                    var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                    if (orderObject == null || orderObject.Id < 1)
                    {
                        return new EstimateObject();
                    }

                    orderObject.CustomerObject = new CustomerObject
                    {
                        CustomerId = customer.CustomerId,
                        ReferredByCustomerId = customer.ReferredByCustomerId,
                        StoreCustomerTypeId = customer.StoreCustomerTypeId,
                        StoreOutletId = customer.StoreOutletId,
                        UserId = customer.UserId,
                        FirstPurchaseDate = customer.FirstPurchaseDate
                    };
                    orderObject.CustomerName = customer.UserProfile.LastName + " " + customer.UserProfile.OtherNames + "(" + customer.UserProfile.MobileNumber + ")";

                    if (employee != null && employee.Id > 0)
                    {
                        orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                    }
                    
                    orderObject.EstimateItemObjects = new List<EstimateItemObject>();
                    var estimateItems =
                        (
                         from p in db.EstimateItems.Where(m => m.EstimateId == estimateId)
                         join si in db.StoreItemStocks on p.StoreItemStockId equals si.StoreItemStockId
                         join sii in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType")
                         on si.StoreItemId equals sii.StoreItemId
                         join iv in db.StoreItemVariationValues on si.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new EstimateItemObject
                         {
                             StoreItemStockId = si.StoreItemStockId,
                             Id = p.Id,
                             QuantityRequested = p.QuantityRequested,
                             ItemPrice = p.ItemPrice,
                             EstimateId = p.EstimateId,
                             SerialNumber = si.SKU,
                             StoreItemName = iv == null ? sii.Name : sii.Name + "/" + iv.Value

                         }).ToList();

                    if (!estimateItems.Any())
                    {
                        return new EstimateObject();
                    }

                    estimateItems.ForEach(x =>
                    {
                        var imgs = (from stu in db.StockUploads.Where(q => q.StoreItemStockId == x.StoreItemStockId).ToList()
                         join imgV in db.ImageViews.Where(f => f.Name == "Front View" || f.Name.ToLower().Contains("front")) on stu.ImageViewId equals imgV.ImageViewId
                         select new StockUpload
                         {
                             ImagePath = stu.ImagePath

                         }).ToList();
                        if (imgs.Any())
                        {

                            x.ImagePath = imgs[0].ImagePath;
                        }
                        else
                        {
                            x.ImagePath = "/Content/images/noImage.png";
                        }

                        var uoms = (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                                    select new ItemPriceObject
                                    {
                                        UoMCode = it.UnitsOfMeasurement.UoMCode

                                    }).ToList();

                        if (uoms.Any())
                        {
                            x.UoMCode = uoms[0].UoMCode;
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

                    orderObject.EstimateItemObjects = estimateItems;
                    orderObject.OutletName = order.StoreOutlet.OutletName;
                    orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                    orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                    orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                    orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                    orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                    orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                    orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                    return orderObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new EstimateObject();
            }
        }

        public EstimateObject GetEstimateByRef(string refNumber)
        {
            try
            {
                using (var db = _db)
                {
                    var orders = db.Estimates.Where(m => m.EstimateNumber == refNumber).Include("UserProfile").Include("StoreOutlet").ToList();
                    if (!orders.Any())
                    {
                        return new EstimateObject();
                    }

                    var order = orders[0];
                    var customers = db.Customers.Where(i => i.CustomerId == order.CustomerId).Include("UserProfile").ToList();
                    if (!customers.Any())
                    {
                        return new EstimateObject();
                    }
                    var customer = customers[0];

                    var employee = order.UserProfile;

                    var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                    if (orderObject == null || orderObject.Id < 1)
                    {
                        return new EstimateObject();
                    }
                    
                    orderObject.CustomerObject = new CustomerObject
                    {
                        CustomerId = customer.CustomerId,
                        ReferredByCustomerId = customer.ReferredByCustomerId,
                        StoreCustomerTypeId = customer.StoreCustomerTypeId,
                        StoreOutletId = customer.StoreOutletId,
                        UserId = customer.UserId,
                        FirstPurchaseDate = customer.FirstPurchaseDate
                    };
                    orderObject.CustomerName = customer.UserProfile.LastName + " " + customer.UserProfile.OtherNames + "(" + customer.UserProfile.MobileNumber + ")";

                    if (employee != null && employee.Id > 0)
                    {
                        orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                    }

                    orderObject.EstimateItemObjects = new List<EstimateItemObject>();
                    var estimateItems =
                        (
                         from p in db.EstimateItems.Where(m => m.EstimateId == orderObject.Id)
                         join si in db.StoreItemStocks on p.StoreItemStockId equals si.StoreItemStockId
                         join sii in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType")
                         on si.StoreItemId equals sii.StoreItemId
                         join iv in db.StoreItemVariationValues on si.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new EstimateItemObject
                         {
                             StoreItemStockId = si.StoreItemStockId,
                             Id = p.Id,
                             QuantityRequested = p.QuantityRequested,
                             ItemPrice = p.ItemPrice,
                             EstimateId = p.EstimateId,
                             SerialNumber = si.SKU,
                             StoreItemName = iv == null ? sii.Name : sii.Name + "/" + iv.Value

                         }).ToList();

                    if (!estimateItems.Any())
                    {
                        return new EstimateObject();
                    }

                    estimateItems.ForEach(x =>
                    {
                        var imgs = (from stu in db.StockUploads.Where(q => q.StoreItemStockId == x.StoreItemStockId).ToList()
                                    join imgV in db.ImageViews.Where(f => f.Name == "Front View" || f.Name.ToLower().Contains("front")) on stu.ImageViewId equals imgV.ImageViewId
                                    select new StockUpload
                                    {
                                        ImagePath = stu.ImagePath

                                    }).ToList();
                        if (imgs.Any())
                        {

                            x.ImagePath = imgs[0].ImagePath;
                        }
                        else
                        {
                            x.ImagePath = "/Content/images/noImage.png";
                        }

                        var uoms = (from it in db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).Include("UnitsOfMeasurement")
                                    select new ItemPriceObject
                                    {
                                        UoMCode = it.UnitsOfMeasurement.UoMCode

                                    }).ToList();

                        if (uoms.Any())
                        {
                            x.UoMCode = uoms[0].UoMCode;
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

                    orderObject.EstimateItemObjects = estimateItems;
                    orderObject.OutletName = order.StoreOutlet.OutletName;
                    orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                    orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                    orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                    orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");

                    orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                    orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                    orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                    return orderObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new EstimateObject();
            }
        }
        
        public SaleObject GetSalesDetails(string estimateNumber)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(l => l.EstimateNumber != null && l.EstimateNumber == estimateNumber)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                 join ps in db.UserProfiles on em.UserId equals ps.Id
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     NetAmount = sa.NetAmount,
                                     VAT = sa.VAT,
                                     SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                     Discount = sa.Discount,
                                     EstimateNumber = sa.EstimateNumber,
                                     VATAmount = sa.VATAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Status = sa.Status,
                                     CustomerId = sa.CustomerId,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId
                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new SaleObject();
                    }

                    var s = sales[0];

                    var customers = db.Customers.Where(i => i.CustomerId == s.CustomerId).Include("UserProfile").ToList();
                    if (!customers.Any())
                    {
                        return new SaleObject();
                    }
                    var customer = customers[0];

                    s.CustomerName = customer.UserProfile.LastName + " " + customer.UserProfile.OtherNames + "(" + customer.UserProfile.MobileNumber + ")";


                    s.StoreItemSoldObjects = new List<StoreItemSoldObject>();
                    s.EmployeeObject = new EmployeeObject();
                    s.SalePaymentObjects = new List<SalePaymentObject>();
                    s.Transactions = new List<StoreTransactionObject>();

                    var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                     join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                     join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                     join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                         QuantitySold = sts.QuantitySold,
                                         AmountSold = sts.AmountSold,
                                         UoMCode = ui.UoMCode,
                                         Rate = sts.Rate
                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new SaleObject();
                    }

                    soldItems.ForEach(q => {});

                    s.StoreItemSoldObjects = soldItems;

                    var transactions = (from t in db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).Include("StorePaymentMethod")
                                        join em in db.Employees on t.EffectedByEmployeeId equals em.EmployeeId
                                        join ps in db.UserProfiles on em.UserId equals ps.Id
                                        join ot in db.StoreOutlets on t.StoreOutletId equals ot.StoreOutletId
                                        select new StoreTransactionObject
                                        {
                                            StoreTransactionId = t.StoreTransactionId,
                                            StoreTransactionTypeId = t.StoreTransactionTypeId,
                                            StorePaymentMethodId = t.StorePaymentMethodId,
                                            EffectedByEmployeeId = t.EffectedByEmployeeId,
                                            TransactionAmount = t.TransactionAmount,
                                            PaymentMethodName = t.StorePaymentMethod.Name,
                                            TransactionDate = t.TransactionDate,
                                            Remark = t.Remark,
                                            StoreOutletId = t.StoreOutletId,
                                            TransactionEmployeeName = ps.LastName + " " + ps.OtherNames,
                                            OutletName = ot.OutletName

                                        }).ToList();

                    if (!transactions.Any())
                    {
                        return new SaleObject();
                    }

                    s.OutletName = transactions[0].OutletName;
                    s.Transactions = transactions;

                    var amountPaid = 0.0;
                    transactions.ForEach(n =>
                    {
                        amountPaid += n.TransactionAmount;
                    });

                    s.AmountPaid = amountPaid;
                    s.AmountPaidStr = amountPaid.ToString("n0");
                    s.DateStr = s.Date.ToString("dd/MM/yyyy hh:mm tt");
                    s.Balance = s.NetAmount - s.AmountPaid;
                    return s;

                }
            }
            catch (Exception ex)
            {
                return new SaleObject();
            }
        }

        public List<EstimateObject> GetEstimates(int? itemsPerPage, int? pageNumber, out int count)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.OrderByDescending(m => m.Id).Skip(tpageNumber).Take(tsize)
                                .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }

                                

                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);
                            });

                            count = db.Estimates.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> GetEstimatesByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.Where(o => o.OutletId == outletId).OrderByDescending(m => m.Id).Skip(tpageNumber).Take(tsize)
                              .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }

                                

                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");  
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);

                            });

                            count = db.Estimates.Count(o => o.OutletId == outletId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> GetEstimatesByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.Where(o => o.CreatedById == employeeId).OrderByDescending(m => m.Id).Skip(tpageNumber).Take(tsize)
                               .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }
                                
                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);
                            });

                            count = db.Estimates.Count(o => o.CreatedById == employeeId);
                            return newList.OrderByDescending(m => m.Id).ToList();
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> SearchEstimates(string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => (o.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower()))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> SearchOutletEstimate(string searchCriteria, int outletId)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => o.OutletId == outletId
                         && (o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower()))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> SearchEmployeeEstimate(string searchCriteria, long employeeId)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => o.CreatedById == employeeId
                        && (o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                         || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower()))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }

    }
}
