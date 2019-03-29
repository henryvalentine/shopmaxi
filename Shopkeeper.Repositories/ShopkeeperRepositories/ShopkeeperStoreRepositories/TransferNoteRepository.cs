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
    public class TransferNoteRepository
    {
        private readonly IShopkeeperRepository<TransferNote> _repository;
        private readonly UnitOfWork _uoWork;
        private ShopKeeperStoreEntities _db;
       public TransferNoteRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
            _repository = new ShopkeeperRepository<TransferNote>(_uoWork);
		}
        
        public long AddTransferNote(TransferNoteObject transferNote, out string transferNoteNumber)
        {
            long pId = 0;
            try
            {
                if (transferNote == null || transferNote.TransferNoteItemObjects == null || !transferNote.TransferNoteItemObjects.Any())
                {
                    transferNoteNumber = "";
                    return -2;
                }
                
                using (var db = _db)
                {
                    var orderEntity = ModelCrossMapper.Map<TransferNoteObject, TransferNote>(transferNote);
                    if (orderEntity == null || orderEntity.GeneratedByUserId < 1)
                    {
                        transferNoteNumber = "";
                        return -2;
                    }
                    
                    var code = DateTime.Now.Year + DateTime.Now.Month.ToString();
                    var similarBatches = db.TransferNotes.Where(u => u.TransferNoteNumber.Contains(code)).ToList();
                    if (similarBatches.Any())
                    {
                        var tempList = new List<float>();
                        similarBatches.ForEach(x =>
                        {
                            float t;
                            var sprs = float.TryParse(x.TransferNoteNumber, out t);
                            if (sprs && t > 0)
                            {
                                tempList.Add(t);
                            }

                        });

                        if (tempList.Any())
                        {
                            var recent = tempList.OrderByDescending(k => k).ToList()[0];
                            orderEntity.TransferNoteNumber = (recent + 1).ToString(CultureInfo.InvariantCulture);
                        }

                        else
                        {
                            orderEntity.TransferNoteNumber = code + "1";
                        }
                    }

                    else
                    {
                        orderEntity.TransferNoteNumber = code + "1";
                    }

                    transferNoteNumber = orderEntity.TransferNoteNumber;

                    var processeTransferNote = db.TransferNotes.Add(orderEntity);
                    db.SaveChanges();
                    pId = processeTransferNote.Id;

                    transferNote.TransferNoteItemObjects.ToList().ForEach(it =>
                    {
                        it.TransferNoteId = pId;

                        var stocks = db.StoreItemStocks.Where(i => i.StoreItemStockId == it.StoreItemStockId).ToList();
                        if (!stocks.Any())
                        {
                            return;
                        }

                        var stock = stocks[0];

                        var itemEntity = ModelCrossMapper.Map<TransferNoteItemObject, TransferNoteItem>(it);
                        if (itemEntity != null && itemEntity.TransferNoteId > 0)
                        {
                            db.TransferNoteItems.Add(itemEntity);
                            db.SaveChanges();

                            stock.QuantityInStock -= it.TotalQuantityRaised;
                            db.Entry(stock).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    });

                    return pId;
                }
               
            }
           
            catch (Exception e)
            {
               
                if (pId > 0)
                {
                    using (var db = _db)
                    {
                        var list =  db.TransferNoteItems.Where(g => g.TransferNoteId == pId).ToList();
                        if (list.Any())
                        {
                            list.ForEach(d =>
                            {
                                db.TransferNoteItems.Remove(d);
                                db.SaveChanges();
                            });
                        }

                        var items = db.TransferNotes.Where(g => g.Id == pId).ToList();
                        if (items.Any())
                        {
                            var item = items[0];
                            db.TransferNotes.Remove(item);
                            db.SaveChanges();
                           
                        }
                    }
                }
               
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                transferNoteNumber = "";
                return 0;
            }
        }

        public long UpdateTransferNote(TransferNoteObject transferNote)
        {
            try
            {
                if (transferNote == null || transferNote.TransferNoteItemObjects == null || !transferNote.TransferNoteItemObjects.Any())
                {
                    return -2;
                }
                
                using (var db = _db)
                {
                    var ius = db.TransferNoteItems.Where(t => t.TransferNoteId == transferNote.Id).ToList();
                    if (!ius.Any())
                    {
                        return -2;
                    }

                    var orderEntities = db.TransferNotes.Where(t => t.Id == transferNote.Id).ToList();

                    if (!orderEntities.Any())
                    {
                        return -2;
                    }

                    var orderEntity = orderEntities[0];

                    orderEntity.TotalAmount = transferNote.TotalAmount;
                    orderEntity.TargetOutletId = transferNote.TargetOutletId;
                    orderEntity.SourceOutletId = transferNote.SourceOutletId;

                    var items = transferNote.TransferNoteItemObjects.ToList();

                    db.Entry(orderEntity).State = EntityState.Modified;
                    db.SaveChanges();

                    items.ForEach(it =>
                    {
                        if (it.Id < 1)
                        {
                            it.TransferNoteId = transferNote.Id;
                            var itemEntity = ModelCrossMapper.Map<TransferNoteItemObject, TransferNoteItem>(it);
                            if (itemEntity != null && itemEntity.TransferNoteId > 0)
                            {
                                db.TransferNoteItems.Add(itemEntity);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var refItem = ius.Find(f => f.Id == it.Id);
                            if (refItem != null && refItem.Id > 0)
                            {
                                var stocks = db.StoreItemStocks.Where(i => i.StoreItemStockId == it.StoreItemStockId).ToList();
                                if (!stocks.Any())
                                {
                                    return;
                                }

                                var stock = stocks[0];

                                if (refItem.TotalQuantityRaised.Equals(it.TotalQuantityRaised)) return;
                                stock.QuantityInStock += refItem.TotalQuantityRaised;
                                db.Entry(stock).State = EntityState.Modified;
                                db.SaveChanges();

                                refItem.TotalQuantityTransfered = it.TotalQuantityTransfered;
                                refItem.TotalQuantityRaised = it.TotalQuantityRaised;
                                refItem.TotalAmountRaised = it.TotalAmountRaised;
                                refItem.TotalAmountTransfered = it.TotalAmountTransfered;
                                refItem.Rate = it.Rate;
                                    
                                db.Entry(refItem).State = EntityState.Modified;
                                db.SaveChanges();

                                stock.QuantityInStock -= it.TotalQuantityRaised;
                                db.Entry(stock).State = EntityState.Modified;
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

        public long ConvertTransferNoteToInvoice(string transferNoteNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(transferNoteNumber))
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var transferNotes = db.TransferNotes.Where(m => m.TransferNoteNumber == transferNoteNumber).ToList();
                    if (!transferNotes.Any())
                    {
                        return -2;
                    }
                    var transferNote = transferNotes[0];
                    transferNote.Status = (int)TransfereNoteStatus.Completely_Transfered;
                    db.Entry(transferNote).State = EntityState.Modified;
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

        public long DeleteTransferNoteItem(long transferNoteItemId)
        {
            try
            {
                if (transferNoteItemId < 1)
                {
                    return -2;
                }

                var partlyTransfered = (int) TransfereNoteStatus.Partly_Trasnfered;
                var completelyTransfered = (int) TransfereNoteStatus.Completely_Transfered;

                using (var db = _db)
                {
                    var items = db.TransferNoteItems.Where(o => o.Id == transferNoteItemId && o.TransferNote.Status != partlyTransfered && o.TransferNote.Status != completelyTransfered && o.TotalQuantityTransfered < o.TotalQuantityRaised).Include("TransferNote").ToList();
                    if (!items.Any())
                    {
                        return -2;
                    }
                    
                    var item = items[0];
                    var transferNote = item.TransferNote;

                    var stocks = db.StoreItemStocks.Where(i => i.StoreItemStockId == item.StoreItemStockId).ToList();
                    if (!stocks.Any())
                    {
                        return -2;
                    }


                    db.TransferNoteItems.Remove(item); 
                    db.SaveChanges();
                    
                    var stock = stocks[0];
                    stock.QuantityInStock += item.TotalQuantityRaised;
                    db.Entry(stock).State = EntityState.Modified;
                    db.SaveChanges();

                    var refreshedItems = db.TransferNoteItems.Where(h => h.TransferNoteId == item.Id).Include("TransferNote").ToList();
                    if (!refreshedItems.Any())
                    {
                        db.TransferNotes.Remove(transferNote);
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

        public long DeleteTransferNote(long transferNoteId)
        {
            try
            {
                if (transferNoteId < 1)
                {
                    return -2;
                }

                var partlyTransfered = (int) TransfereNoteStatus.Partly_Trasnfered;
                var completelyTransfered = (int)TransfereNoteStatus.Completely_Transfered;

                using (var db = _db)
                {
                    var transferNotes = db.TransferNotes.Where(o => o.Status != partlyTransfered && o.Status != completelyTransfered).Include("TransferNoteItems").ToList();
                    if (!transferNotes.Any())
                    {
                        return -2;
                    }

                    var item = transferNotes[0];
                    var transferNoteItems = item.TransferNoteItems.ToList();
                    if (!transferNoteItems.Any())
                    {
                        return -2;
                    }

                    transferNoteItems.ForEach(e =>
                    {
                        db.TransferNoteItems.Remove(e);
                        db.SaveChanges();
                    });

                    db.TransferNotes.Remove(item);
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

        public TransferNoteObject GetTransferNote(long transferNoteId)
        {
            try
            {
                using (var db = _db)
                {
                    var orders = db.TransferNotes.Where(m => m.Id == transferNoteId).Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1").ToList();
                    if (!orders.Any())
                    {
                        return new TransferNoteObject();   
                    }

                    var order = orders[0];

                    var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                    if (orderObject == null || orderObject.Id < 1)
                    {
                        return new TransferNoteObject();
                    }

                    var sourceOutlet = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(order.StoreOutlet); 
                    if (sourceOutlet == null || sourceOutlet.StoreOutletId < 1)
                    {
                        return new TransferNoteObject();
                    }

                    var targetOutlet = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(order.StoreOutlet1);
                    if (targetOutlet == null || targetOutlet.StoreOutletId < 1)
                    {
                        return new TransferNoteObject();
                    }

                    orderObject.TransferNoteItemObjects = new List<TransferNoteItemObject>();
                    var transferNoteItems =
                        (
                         from p in db.TransferNoteItems.Where(m => m.TransferNoteId == transferNoteId)
                         join si in db.StoreItemStocks on p.StoreItemStockId equals si.StoreItemStockId
                         join sii in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType")
                         on si.StoreItemId equals sii.StoreItemId
                         join uom in db.UnitsOfMeasurements on p.UoMId equals uom.UnitOfMeasurementId
                         join iv in db.StoreItemVariationValues on si.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new TransferNoteItemObject
                         {
                             StoreItemStockId = si.StoreItemStockId,
                             Id = p.Id,
                             TotalQuantityTransfered = p.TotalQuantityTransfered,
                             TotalQuantityRaised = p.TotalQuantityRaised,
                             TotalAmountRaised = p.TotalAmountRaised,
                             Rate = p.Rate,
                             TransferNoteId = p.TransferNoteId,
                             SKU = si.SKU,
                             UoMId = uom.UnitOfMeasurementId,
                             UoMCode = uom.UoMCode,
                             StoreItemName = iv == null ? sii.Name : sii.Name + "/" + iv.Value

                         }).ToList();


                    if (!transferNoteItems.Any())
                    {
                        return new TransferNoteObject();
                    }

                    transferNoteItems.ForEach(x =>
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

                    orderObject.TransferNoteItemObjects = transferNoteItems;
                    orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                    orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                    orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                    orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                    orderObject.DateTransferdStr = orderObject.DateTransferd != null? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                    orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                    orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                    return orderObject;
                }

                

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new TransferNoteObject();
            }
        }

        public TransferNoteObject GetTransferNoteByRef(string refNumber)
        {
            try
            {
                using (var db = _db)
                {
                    var orders = db.TransferNotes.Where(m => m.TransferNoteNumber == refNumber).Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1").ToList();
                    if (!orders.Any())
                    {
                        return new TransferNoteObject();
                    }

                    var order = orders[0];

                    var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                    if (orderObject == null || orderObject.Id < 1)
                    {
                        return new TransferNoteObject();
                    }

                    var sourceOutlet = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(order.StoreOutlet);
                    if (sourceOutlet == null || sourceOutlet.StoreOutletId < 1)
                    {
                        return new TransferNoteObject();
                    }

                    var targetOutlet = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(order.StoreOutlet1);
                    if (targetOutlet == null || targetOutlet.StoreOutletId < 1)
                    {
                        return new TransferNoteObject();
                    }

                    orderObject.TransferNoteItemObjects = new List<TransferNoteItemObject>();
                    var transferNoteItems =
                        (
                         from p in db.TransferNoteItems.Where(m => m.TransferNoteId == order.Id)
                         join si in db.StoreItemStocks on p.StoreItemStockId equals si.StoreItemStockId
                         join sii in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType")
                         on si.StoreItemId equals sii.StoreItemId
                         join uom in db.UnitsOfMeasurements on p.UoMId equals uom.UnitOfMeasurementId
                         join iv in db.StoreItemVariationValues on si.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new TransferNoteItemObject
                         {
                             StoreItemStockId = si.StoreItemStockId,
                             Id = p.Id,
                             TotalQuantityTransfered = p.TotalQuantityTransfered,
                             TotalQuantityRaised = p.TotalQuantityRaised,
                             TotalAmountRaised = p.TotalAmountRaised,
                             Rate = p.Rate,
                             TransferNoteId = p.TransferNoteId,
                             SKU = si.SKU,
                             UoMId = uom.UnitOfMeasurementId,
                             UoMCode = uom.UoMCode,
                             StoreItemName = iv == null ? sii.Name : sii.Name + "/" + iv.Value

                         }).ToList();

                    if (!transferNoteItems.Any())
                    {
                        return new TransferNoteObject();
                    }

                    transferNoteItems.ForEach(x =>
                    {
                        var prices = db.ItemPrices.Where(p => p.StoreItemStockId == x.StoreItemStockId).OrderBy(y => y.MinimumQuantity).ToList();
                        if (prices.Any())
                        {
                            x.BaseSellingPrice = prices[0].Price;
                        }
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

                    orderObject.TransferNoteItemObjects = transferNoteItems;
                    orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                    orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                    orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                    orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                    orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                    orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                    orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                    return orderObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new TransferNoteObject();
            }
        }
        
        public List<TransferNoteObject> GetTransferNotes(int? itemsPerPage, int? pageNumber, out int count)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var transferNotes = db.TransferNotes.OrderByDescending(m => m.Id).Skip(tpageNumber).Take(tsize)
                                .Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1")
                                .ToList();

                        if (transferNotes.Any())
                        {
                            var newList = new List<TransferNoteObject>();
                            transferNotes.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }
                                
                                orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                                orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                                orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                                orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                                orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                                orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                                orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                                newList.Add(orderObject);
                            });

                            count = db.TransferNotes.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<TransferNoteObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<TransferNoteObject>();
            }
        }
        public List<TransferNoteObject> GetTransferNotesByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var transferNotes = db.TransferNotes.Where(o => o.SourceOutletId == outletId || o.TargetOutletId == outletId).OrderByDescending(m => m.Id).Skip(tpageNumber).Take(tsize)
                              .Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1")
                                .ToList();

                        if (transferNotes.Any())
                        {
                            var newList = new List<TransferNoteObject>();
                            transferNotes.ForEach(order =>
                            {
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }

                                orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                                orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                                orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                                orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                                orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                                orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                                orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                                newList.Add(orderObject);

                            });

                            count = db.TransferNotes.Count(o => o.SourceOutletId == outletId || o.TargetOutletId == outletId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<TransferNoteObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<TransferNoteObject>();
            }
        }
        public List<TransferNoteObject> GetTransferNotesByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var transferNotes = db.TransferNotes.Where(o => o.GeneratedByUserId == employeeId).OrderByDescending(m => m.Id).Skip(tpageNumber).Take(tsize)
                               .Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1")
                                .ToList();

                        if (transferNotes.Any())
                        {
                            var newList = new List<TransferNoteObject>();
                            transferNotes.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }



                                orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                                orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                                orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                                orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                                orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                                orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                                orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                                newList.Add(orderObject);
                            });

                            count = db.TransferNotes.Count(o => o.GeneratedByUserId == employeeId);
                            return newList.OrderByDescending(m => m.Id).ToList();
                        }
                    }

                }
                count = 0;
                return new List<TransferNoteObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<TransferNoteObject>();
            }
        }
        public List<TransferNoteObject> SearchTransferNotes(string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    var transferNotes = db.TransferNotes.Where(o => (o.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.TransferNoteNumber.ToLower().Contains(searchCriteria.ToLower()))
                           .Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1")
                            .ToList();

                    if (transferNotes.Any())
                    {
                        var newList = new List<TransferNoteObject>();
                        transferNotes.ForEach(order =>
                        {
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                            orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                            orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                            orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                            orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                            orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                            orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<TransferNoteObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransferNoteObject>();
            }
        }
        public List<TransferNoteObject> SearchOutletTransferNote(string searchCriteria, int outletId)
        {
            try
            {
                using (var db = _db)
                {
                    var transferNotes = db.TransferNotes.Where(o => o.SourceOutletId == outletId || o.TargetOutletId == outletId
                         && (o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.TransferNoteNumber.ToLower().Contains(searchCriteria.ToLower()))
                           .Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1")
                            .ToList();

                    if (transferNotes.Any())
                    {
                        var newList = new List<TransferNoteObject>();
                        transferNotes.ForEach(order =>
                        {

                            var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                            orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                            orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                            orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                            orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                            orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                            orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<TransferNoteObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransferNoteObject>();
            }
        }

        public List<TransferNoteObject> SearchEmployeeTransferNote(string searchCriteria, long employeeId)
        {
            try
            {
                using (var db = _db)
                {
                    var transferNotes = db.TransferNotes.Where(o => o.GeneratedByUserId == employeeId
                        && (o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.TransferNoteNumber.ToLower().Contains(searchCriteria.ToLower()))
                           .Include("UserProfile").Include("StoreOutlet").Include("StoreOutlet1")
                            .ToList();

                    if (transferNotes.Any())
                    {
                        var newList = new List<TransferNoteObject>();
                        transferNotes.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<TransferNote, TransferNoteObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.SourceOutletName = order.StoreOutlet.OutletName;
                            orderObject.TargetOutletName = order.StoreOutlet1.OutletName;
                            orderObject.GeneratedBy = order.UserProfile.LastName + " " + order.UserProfile.OtherNames;
                            orderObject.TotalAmountStr = orderObject.TotalAmount.ToString("n0");
                            orderObject.DateTransferdStr = orderObject.DateTransferd != null ? ((DateTime)orderObject.DateTransferd).ToString("dd/MM/yyyy") : "";
                            orderObject.DateGeneratedStr = orderObject.DateGenerated.ToString("dd/MM/yyyy");
                            orderObject.StatusStr = Enum.GetName(typeof(TransfereNoteStatus), orderObject.Status).Replace("_", " ");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<TransferNoteObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransferNoteObject>();
            }
        }

    }
}
