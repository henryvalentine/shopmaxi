using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class PurchaseOrderRepository
    {
        private readonly IShopkeeperRepository<ParentMenu> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _db;

        public PurchaseOrderRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<ParentMenu>(_uoWork);
		}

        public long AddPurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            long pId = 0;
            try
            {
                if (purchaseOrder == null || purchaseOrder.PurchaseOrderItemObjects == null || !purchaseOrder.PurchaseOrderItemObjects.Any())
                {
                    return -2;
                }
                
                using (var db = _db)
                {
                    var orderEntity = ModelCrossMapper.Map<PurchaseOrderObject, PurchaseOrder>(purchaseOrder);
                    if (orderEntity == null || orderEntity.StoreOutletId < 1)
                    {
                        return -2;
                    }
                    
                    var code = DateTime.Today.ToString("yyyy") + DateTime.Today.ToString("MM");
                    var similarBatches = db.PurchaseOrders.Where(u => u.PurchaseOrderNumber.Contains(code)).ToList();
                    if (similarBatches.Any())
                    {
                        var tempList = new List<float>();
                        similarBatches.ForEach(x =>
                        {
                            float t;
                            var sprs = float.TryParse(x.PurchaseOrderNumber, out t);
                            if (sprs && t > 0)
                            {
                                tempList.Add(t);
                            }

                        });

                        if (tempList.Any())
                        {
                            var recent = tempList.OrderByDescending(k => k).ToList()[0];
                            orderEntity.PurchaseOrderNumber = (recent + 1).ToString(CultureInfo.InvariantCulture);
                        }

                        else
                        {
                            orderEntity.PurchaseOrderNumber = code + "0001";
                        }
                    }
                    else
                    {
                        orderEntity.PurchaseOrderNumber = code + "0001";
                    }

                    var processedOrder = db.PurchaseOrders.Add(orderEntity);
                    db.SaveChanges();
                    pId = processedOrder.PurchaseOrderId;

                    purchaseOrder.PurchaseOrderItemObjects.ToList().ForEach(it =>
                    {
                        it.PurchaseOrderId = pId;

                        var itemEntity = ModelCrossMapper.Map<PurchaseOrderItemObject, PurchaseOrderItem>(it);
                        if(itemEntity != null && itemEntity.StoreItemStockId > 0)
                        {
                            itemEntity.StatusCode = (int)PurchaseOrderDeliveryStatus.Pending;
                            db.PurchaseOrderItems.Add(itemEntity);
                            db.SaveChanges();
                        }
                    });
                    
                    return processedOrder.PurchaseOrderId;
                }
               
            }
           
            catch (Exception e)
            {
               
                if (pId > 0)
                {
                    using (var db = _db)
                    {
                        var list =  db.PurchaseOrderItems.Where(g => g.PurchaseOrderId == pId).ToList();
                        if (list.Any())
                        {
                            list.ForEach(d =>
                            {
                                db.PurchaseOrderItems.Remove(d);
                                db.SaveChanges();
                            });
                        }

                        var items = db.PurchaseOrders.Where(g => g.PurchaseOrderId == pId).ToList();
                        if (items.Any())
                        {
                            var item = items[0];
                            db.PurchaseOrders.Remove(item);
                            db.SaveChanges();
                           
                        }
                    }
                }
               
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return 0;
            }
        }

        public int GetOutlet()
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
                {
                    var storeOutlets = db.StoreOutlets.ToList();
                    if (storeOutlets.Any())
                    {
                        var outletId = storeOutlets.Find(s => s.IsMainOutlet).StoreOutletId;
                        if (outletId < 1)
                        {
                            return storeOutlets[0].StoreOutletId;
                        }

                        return outletId;
                    }

                    return -2;
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
                return -2;
            }
        }

        public long AddPurchaseOrder(PurchaseOrderObject purchaseOrder, out string purchaseOrderNumber)
        {
            long pId = 0;
            try
            {
                if (purchaseOrder == null || purchaseOrder.PurchaseOrderItemObjects == null || !purchaseOrder.PurchaseOrderItemObjects.Any())
                {
                    purchaseOrderNumber = string.Empty;
                    return -2;
                }

                //var outletId = GetOutlet();
                //if (outletId < 1)
                //{
                //    purchaseOrderNumber = string.Empty;
                //    return -2;
                //}

                //purchaseOrder.StoreOutletId = outletId;

                using (var db = _db)
                {
                    var orderEntity = ModelCrossMapper.Map<PurchaseOrderObject, PurchaseOrder>(purchaseOrder);
                    if (orderEntity == null || orderEntity.StoreOutletId < 1)
                    {
                        purchaseOrderNumber = string.Empty;
                        return -2;
                    }

                    var yrStr = DateTime.Now.ToString("yyyy/MM");

                    var code = yrStr.Replace("/", "");
                    var similarBatches = db.PurchaseOrders.Where(u => u.PurchaseOrderNumber.Contains(code)).ToList();
                    if (similarBatches.Any())
                    {
                        var tempList = new List<float>();
                        similarBatches.ForEach(x =>
                        {
                            float t;
                            var sprs = float.TryParse(x.PurchaseOrderNumber, out t);
                            if (sprs && t > 0)
                            {
                                tempList.Add(t);
                            }

                        });

                        if (tempList.Any())
                        {
                            var recent = tempList.OrderByDescending(k => k).ToList()[0];
                            orderEntity.PurchaseOrderNumber = (recent + 1).ToString(CultureInfo.InvariantCulture);
                        }

                        else
                        {
                            orderEntity.PurchaseOrderNumber = code + "1";
                        }
                    }
                    else
                    {
                        orderEntity.PurchaseOrderNumber = code + "1";
                    }

                    var processedOrder = db.PurchaseOrders.Add(orderEntity);
                    db.SaveChanges();
                    pId = processedOrder.PurchaseOrderId;

                    purchaseOrder.PurchaseOrderItemObjects.ToList().ForEach(it =>
                    {
                        it.PurchaseOrderId = pId;

                        var itemEntity = ModelCrossMapper.Map<PurchaseOrderItemObject, PurchaseOrderItem>(it);
                        if (itemEntity != null && itemEntity.StoreItemStockId > 0)
                        {
                            itemEntity.StatusCode = (int)PurchaseOrderDeliveryStatus.Pending;
                            db.PurchaseOrderItems.Add(itemEntity);
                            db.SaveChanges();
                        }
                    });
                    purchaseOrderNumber = orderEntity.PurchaseOrderNumber;
                    return processedOrder.PurchaseOrderId;
                }

            }

            catch (Exception e)
            {

                if (pId > 0)
                {
                    using (var db = _db)
                    {
                        var list = db.PurchaseOrderItems.Where(g => g.PurchaseOrderId == pId).ToList();
                        if (list.Any())
                        {
                            list.ForEach(d =>
                            {
                                db.PurchaseOrderItems.Remove(d);
                                db.SaveChanges();
                            });
                        }

                        var items = db.PurchaseOrders.Where(g => g.PurchaseOrderId == pId).ToList();
                        if (items.Any())
                        {
                            var item = items[0];
                            db.PurchaseOrders.Remove(item);
                            db.SaveChanges();

                        }
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                purchaseOrderNumber = string.Empty;
                return 0;
            }
        }

        public long UpdatePurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            try
            {
                if (purchaseOrder == null || purchaseOrder.PurchaseOrderItemObjects == null || !purchaseOrder.PurchaseOrderItemObjects.Any())
                {
                    return -2;
                }
                
                using (var db = _db)
                {

                    var pOrders = db.PurchaseOrders.Where(t => t.PurchaseOrderId == purchaseOrder.PurchaseOrderId).Include("PurchaseOrderItems").ToList();
                    if (!pOrders.Any())
                    {
                        return -2;
                    }
                    
                    var pOrder = pOrders[0];

                    var ius = pOrder.PurchaseOrderItems.ToList();
                    if (!ius.Any())
                    {
                        return -2;
                    }

                    pOrder.SupplierId =purchaseOrder.SupplierId;
                    pOrder.DerivedTotalCost =purchaseOrder.DerivedTotalCost;
                    pOrder.DiscountAmount =purchaseOrder.DiscountAmount;
                    pOrder.VATAmount =purchaseOrder.VATAmount;
                    pOrder.ExpectedDeliveryDate =purchaseOrder.ExpectedDeliveryDate;
                    pOrder.AccountId = purchaseOrder.AccountId;

                    db.Entry(pOrder).State = EntityState.Modified;
                    db.SaveChanges();

                    var items = purchaseOrder.PurchaseOrderItemObjects.ToList();

                    items.ForEach(it =>
                    {
                        if (it.PurchaseOrderItemId < 1)
                        {
                            it.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                            var itemEntity = ModelCrossMapper.Map<PurchaseOrderItemObject, PurchaseOrderItem>(it);
                            if (itemEntity != null && itemEntity.StoreItemStockId > 0)
                            {
                                db.PurchaseOrderItems.Add(itemEntity);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var refItem = ius.Find(f => f.PurchaseOrderItemId == it.PurchaseOrderItemId && f.StoreItemStockId == it.StoreItemStockId);
                            if (refItem != null && refItem.PurchaseOrderItemId > 0)
                            {
                                refItem.SerialNumber = it.SerialNumber;
                                refItem.QuantityOrdered = it.QuantityOrdered;
                                refItem.QuantityOrdered = it.QuantityOrdered;
                                refItem.StatusCode = it.StatusCode;

                                db.Entry(refItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        
                    });

                    ius.ForEach(it =>
                    {
                        var refItem = items.Find(f => f.PurchaseOrderItemId == it.PurchaseOrderItemId && f.StoreItemStockId == it.StoreItemStockId);
                        
                        if (refItem == null || refItem.PurchaseOrderItemId < 1)
                        {
                            db.PurchaseOrderItems.Remove(it);
                            db.SaveChanges();
                        }
                            
                    });

                    return 5;
                }
               
            }

            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return 0;
            }
            //catch (DbEntityValidationException e)
            //{
            //    var str = "";
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage) + " \n";
            //        }
            //    }
            //    ErrorLogger.LogError(e.StackTrace, e.Source, str);
            //    return 0;
            //}
        }

        public long ProcessPurchaseOrderDeliveries(POrderInfo orderDelivery)
        {
            try
            {
                if (orderDelivery == null || !orderDelivery.DeliveredItems.Any())
                {
                    return -2;
                }
                
                using (var db = _db)
                {
                    var successCount = 0;
                    var statusList = new List<int>();

                    var purchaseOrderId = orderDelivery.DeliveredItems[0].PurchaseOrderId;
                    var purchaseOrders = db.PurchaseOrders.Where(r => r.PurchaseOrderId == purchaseOrderId).ToList();
                    var paymentMethod = db.StorePaymentMethods.ToList()[0];

                    var types = db.StoreTransactionTypes.Where(t => t.Name.ToLower().Trim() == "debit" || t.StoreTransactionTypeId == 2).ToList();

                    if (!purchaseOrders.Any() || paymentMethod == null || paymentMethod.StorePaymentMethodId < 1 || !types.Any())
                    {
                        return -2;
                    }

                    var purchaseOrder = purchaseOrders[0];

                    purchaseOrder.VATAmount = orderDelivery.VATAmount;
                    purchaseOrder.FOB = orderDelivery.FOB;
                    purchaseOrder.DiscountAmount = orderDelivery.DiscountAmount;

                    var transaction = new StoreTransaction
                    {
                        StorePaymentMethodId = paymentMethod.StorePaymentMethodId,
                        StoreTransactionTypeId = types[0].StoreTransactionTypeId,
                        EffectedByEmployeeId = orderDelivery.DeliveredItems[0].ReceivedById,
                        TransactionDate = DateTime.Now,
                        StoreOutletId = purchaseOrder.StoreOutletId
                    };

                    var payment = new PurchaseOrderPayment
                    {
                        PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        DateMade = DateTime.Now,

                    };
                    
                    orderDelivery.DeliveredItems.ForEach(l => 
                    {
                        var purchasedItems = db.PurchaseOrderItems.Where(r => r.PurchaseOrderItemId == l.PurchaseOrderItemId).Include("PurchaseOrderItemDeliveries").ToList();
                        if (!purchasedItems.Any())
                        {
                            return;
                        }

                        var purchasedItem = purchasedItems[0];
                        var stockItems = db.StoreItemStocks.Where(s => s.StoreItemStockId == purchasedItem.StoreItemStockId).ToList();
                        if (!stockItems.Any())
                        {
                            return;
                        }

                        var stockItem = stockItems[0];
                        var deliveredItems = purchasedItem.PurchaseOrderItemDeliveries.ToList();
                        var prices = db.ItemPrices.Where(o => o.MinimumQuantity.Equals(l.MinimumQuantity) && o.ItemPriceId == l.ItemPriceId).ToList();
                        if (prices.Any())
                        {
                            var price = prices[0];
                            price.Price = l.Price;
                            db.Entry(price).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                       
                        //Update Stock Item
                        stockItem.QuantityInStock += l.QuantityDelivered;
                        stockItem.ExpirationDate = l.ExpiryDate;
                        db.Entry(stockItem).State = EntityState.Modified;
                        db.SaveChanges();

                        //Update Purchase order Item and insert payment 
                        
                        purchasedItem.QuantityDelivered += l.QuantityDelivered;
                        purchasedItem.QuantityInStock += l.QuantityDelivered;

                        if (purchasedItem.QuantityDelivered > 0)
                        {
                            if (purchasedItem.QuantityOrdered > purchasedItem.QuantityDelivered)
                            {
                                purchasedItem.StatusCode = (int)PurchaseOrderDeliveryStatus.Partly_Delivered;
                                statusList.Add(2);
                            }

                            if (purchasedItem.QuantityOrdered.Equals(purchasedItem.QuantityDelivered))
                            {
                                purchasedItem.StatusCode = (int)PurchaseOrderDeliveryStatus.Completely_Delivered;
                                statusList.Add(3);
                            }
                        }

                        else
                        {
                            purchasedItem.StatusCode = (int)PurchaseOrderDeliveryStatus.Pending;
                            statusList.Add(1);
                        }

                        if (l.PurchaseOrderItemDeliveryId > 0)
                        {
                            var delivery = deliveredItems.Find(q => q.PurchaseOrderItemDeliveryId == l.PurchaseOrderItemDeliveryId);
                            if (delivery != null)
                            {
                                delivery.QuantityDelivered = l.QuantityDelivered;
                                delivery.DateDelivered = l.DateDelivered;
                                delivery.ExpiryDate = l.ExpiryDate;
                                db.Entry(delivery).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var orderItemDeliveryEntity = ModelCrossMapper.Map<PurchaseOrderItemDeliveryObject, PurchaseOrderItemDelivery>(l);
                            if (orderItemDeliveryEntity != null && orderItemDeliveryEntity.PurchaseOrderItemId > 0)
                            {
                                db.PurchaseOrderItemDeliveries.Add(orderItemDeliveryEntity);
                                db.SaveChanges();
                            }
                        }
                       
                        db.Entry(purchasedItem).State = EntityState.Modified;
                        db.SaveChanges();
                        successCount += 1;

                        transaction.TransactionAmount += l.CostPrice*l.QuantityDelivered;
                        payment.AmountPaid += transaction.TransactionAmount;

                    });

                    if (successCount != orderDelivery.DeliveredItems.Count)
                    {
                        return -2;
                    }
                    
                    if (statusList.Exists(i => i == 1))
                    {
                        purchaseOrder.StatusCode = (int)PurchaseOrderDeliveryStatus.Partly_Delivered;
                    }
                    else
                    {
                        var pendingCount = statusList.Count(i => i == 1);
                        var partlyCount = statusList.Count(i => i == 2);
                        var completedCount = statusList.Count(i => i == 3);

                        if (completedCount == statusList.Count())
                        {
                            purchaseOrder.StatusCode = (int)PurchaseOrderDeliveryStatus.Completely_Delivered;
                            purchaseOrder.ActualDeliveryDate = DateTime.Now;
                        }
                        if (pendingCount == statusList.Count())
                        {
                            purchaseOrder.StatusCode = (int)PurchaseOrderDeliveryStatus.Pending;
                        }
                        if (partlyCount > 0)
                        {
                            purchaseOrder.StatusCode = (int)PurchaseOrderDeliveryStatus.Partly_Delivered;
                        }
                    }

                    var procesedTransaction = db.StoreTransactions.Add(transaction);
                    db.SaveChanges();

                    payment.StoreTransactionId = procesedTransaction.StoreTransactionId;
                    db.PurchaseOrderPayments.Add(payment);
                    db.SaveChanges();

                    db.Entry(purchaseOrder).State = EntityState.Modified;
                    db.SaveChanges();

                    return purchaseOrder.PurchaseOrderId;
                }

            }

            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return 0;
            }
        }

        public long ProcessPurchaseOrderInvoice(InvoiceJson invoice)
        {
            try
            {
                if (invoice == null)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    if (invoice.InvoiceId < 1)
                    {
                        var invoiceEntity = ModelCrossMapper.Map<InvoiceJson, Invoice>(invoice);
                        if (invoiceEntity == null || invoiceEntity.PurchaseOrderId < 1)
                        {
                            return -2;
                        }

                        var processeInvoice = db.Invoices.Add(invoiceEntity);
                        db.SaveChanges();
                        invoice.InvoiceId = processeInvoice.InvoiceId;
                    }
                    else
                    {
                        var invoices = db.Invoices.Where(i => i.InvoiceId == invoice.InvoiceId).ToList();
                        if (!invoices.Any())
                        {
                            return -2;
                        }
                        var entityInvoice = invoices[0];
                        entityInvoice.Attachment = !string.IsNullOrEmpty(invoice.Attachment)? invoice.Attachment : entityInvoice.Attachment;
                        entityInvoice.DateSent = invoice.DateSent;
                        db.Entry(entityInvoice).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return invoice.InvoiceId;
                }

            }

            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return 0;
            }
        }
        public long DeleteOrderItem(long orderItemId)
        {
            try
            {
                if (orderItemId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var items = db.PurchaseOrderItems.Where(o => o.PurchaseOrderItemId == orderItemId && o.StatusCode < 2).Include("PurchaseOrder").ToList();
                    if (!items.Any())
                    {
                        return -2;
                    }
                    
                    var item = items[0];
                    var purchaseOrder = item.PurchaseOrder;
                    var count = purchaseOrder.PurchaseOrderItems.Count();
                    if (count < 2)
                    {
                        return -2;
                    }

                    db.PurchaseOrderItems.Remove(item); 
                    db.SaveChanges();

                    var totalCost = db.PurchaseOrderItems.Where(o => o.PurchaseOrderId == item.PurchaseOrderId).Sum(s => s.CostPrice * s.QuantityOrdered);
                    purchaseOrder.DerivedTotalCost = totalCost;
                    db.Entry(purchaseOrder).State = EntityState.Modified;
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
        public long DeletePurchaseOrderItemDelivery(long purchaseOrderItemDeliveryId)
        {
            try
            {
                if (purchaseOrderItemDeliveryId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var items = db.PurchaseOrderItemDeliveries.Where(o => o.PurchaseOrderItemDeliveryId == purchaseOrderItemDeliveryId && o.PurchaseOrderItem.StatusCode < 2).Include("PurchaseOrderItem").ToList();
                    if (!items.Any())
                    {
                        return -2;
                    }

                    var item = items[0];

                    var orderItem = item.PurchaseOrderItem;
                    if (orderItem.QuantityDelivered > 0 && orderItem.QuantityOrdered > item.QuantityDelivered)
                    {
                        orderItem.StatusCode = (int) PurchaseOrderDeliveryStatus.Partly_Delivered;
                    }

                    if (orderItem.QuantityOrdered.Equals(item.QuantityDelivered))
                    {
                        orderItem.StatusCode = (int) PurchaseOrderDeliveryStatus.Completely_Delivered;
                    }

                    db.Entry(orderItem).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    db.PurchaseOrderItemDeliveries.Remove(item);
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
        public PurchaseOrderObject GetPurchaseOrder(long purchaseOrderId)
        {
            try
            {
                using (var db = _db)
                {
                    var orders = db.PurchaseOrders.Where(m => m.PurchaseOrderId == purchaseOrderId).Include("ChartOfAccount").Include("PurchaseOrderItems").Include("Employee").Include("Supplier").Include("StoreOutlet").Include("PurchaseOrderPayments").Include("Invoices").ToList();
                    if (!orders.Any())
                    {
                        return new PurchaseOrderObject();           
                    }

                    var order = orders[0];
                    var chart = order.ChartOfAccount;
                    var supplier = order.Supplier;
                    var employee = order.Employee;
                    var outlet = order.StoreOutlet;
                    var invoices = order.Invoices;

                    var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                    if (orderObject == null || orderObject.PurchaseOrderId < 1)
                    {
                        return new PurchaseOrderObject();
                    }

                    var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                    if (delStatus != null)
                    {
                        orderObject.DeliveryStatus = delStatus.Replace("_", " "); 
                    }

                    orderObject.ExpectedDeliveryDateStr = orderObject.ExpectedDeliveryDate.ToString("dd/MM/yyyy");
                    orderObject.DateTimePlacedStr = orderObject.DateTimePlaced.ToString("dd/MM/yyyy");
                    var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                    if (groups.Any())
                    {
                        orderObject.ChartOfAccountObject = new ChartOfAccountObject
                        {
                            ChartOfAccountId = orderObject.AccountId,
                            AccountGroupId = chart.AccountGroupId,
                            AccountType = chart.AccountType,
                            AccountCode = chart.AccountCode,
                            AccountGroupName = groups[0].Name
                        }; 
                    }

                    orderObject.SupplierObject = new SupplierObject
                    {
                        SupplierId = supplier.SupplierId,
                        DateJoined = supplier.DateJoined,
                        LastSupplyDate = supplier.LastSupplyDate,
                        Note = supplier.Note,
                        CompanyName = supplier.CompanyName,
                        TIN = supplier.TIN
                    };

                    if (employee != null && employee.EmployeeId > 0)
                    {
                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                    }

                    if (outlet != null && outlet.StoreOutletId > 0)
                    {
                        orderObject.OutletName = outlet.OutletName;
                    }

                    orderObject.PurchaseOrderItemObjects = new List<PurchaseOrderItemObject>();
                    var purchaseItems =
                        (
                         from p in db.PurchaseOrderItems.Where(m => m.PurchaseOrderId == purchaseOrderId)
                         join sc in db.StoreItemStocks on p.StoreItemStockId equals sc.StoreItemStockId
                         join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType")
                         on sc.StoreItemId equals si.StoreItemId
                         
                         join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                         select new PurchaseOrderItemObject
                         {
                             StoreItemStockId = sc.StoreItemStockId,
                             QuantityOrdered = p.QuantityOrdered,
                             QuantityDelivered = p.QuantityDelivered,
                             CostPrice = p.CostPrice,
                             StatusCode = p.StatusCode,
                             SerialNumber = sc.SKU,
                             StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value
                         }).ToList();

                    if (!purchaseItems.Any())
                    {
                        return new PurchaseOrderObject();
                    }

                    purchaseItems.ForEach(x =>
                    {
                        if (orderObject.PurchaseOrderItemObjects.Any(d => d.StoreItemStockId == x.StoreItemStockId))
                        {
                            return;
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

                        var name = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), x.StatusCode);
                        if (name != null)
                        {
                            x.DeliveryStatus = name.Replace("_", " ");
                        }
                        x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                    });

                    orderObject.PurchaseOrderItemObjects = purchaseItems;

                    orderObject.InvoiceObjects = new List<InvoiceObject>();
                    if (invoices.Any())
                    {
                        invoices.ToList().ForEach(n =>
                        {
                            var invObj = ModelCrossMapper.Map<Invoice, InvoiceObject>(n);
                            if (invObj != null && invObj.InvoiceId > 0)
                            {
                                orderObject.InvoiceObjects.Add(invObj);
                            }
                        });
                    }

                    return orderObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PurchaseOrderObject();
            }
        }
        public PurchaseOrderObject GetPurchaseOrderDetails(long purchaseOrderId)
        {
            try
            {
                using (var db = _db) 
                {
                    var purchaseOrders = db.PurchaseOrders.Where(p => p.PurchaseOrderId == purchaseOrderId)
                        .Include("ChartOfAccount").Include("PurchaseOrderItems").Include("Employee").Include("Supplier").Include("StoreOutlet").Include("PurchaseOrderPayments").Include("Invoices")
                        .ToList();

                    if (!purchaseOrders.Any())
                    {
                        return new PurchaseOrderObject();
                    }

                    var order = purchaseOrders[0];
                    var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                    if (orderObject == null || orderObject.PurchaseOrderId < 1)
                    {
                        return new PurchaseOrderObject();
                    }
                    
                    var supplier = order.Supplier;
                    var chartOfAccount = order.ChartOfAccount;
                    var storeOutlet = order.StoreOutlet;
                    var employee = order.Employee;
                    var user = db.UserProfiles.Where(u => u.Id == employee.UserId).ToList();
                    var accountGroup = db.AccountGroups.Where(a => a.AccountGroupId == chartOfAccount.AccountGroupId).ToList();
                    var invoices = db.Invoices.Where(m => m.PurchaseOrderId == purchaseOrderId).ToList();
                    var purchasePayments = db.PurchaseOrderPayments.Where(m => m.PurchaseOrderId == purchaseOrderId).ToList();
                    var purchaseItems = db.PurchaseOrderItems.Where(m => m.PurchaseOrderId == purchaseOrderId).ToList();

                    if (!purchaseItems.Any())
                    {
                        return new PurchaseOrderObject();
                    }

                    var purchaseItemObjects = new List<PurchaseOrderItemObject>();

                    purchaseItems.ForEach(p =>
                    {
                        var orderItems = (from sc in db.StoreItemStocks.Where(s => s.StoreItemStockId == p.StoreItemStockId) 
                                          join si in db.StoreItems.Include("StoreItemBrand").Include("StoreItemCategory").Include("StoreItemType") on sc.StoreItemId equals si.StoreItemId 
                                          join iv in db.StoreItemVariationValues on sc.StoreItemVariationValueId equals iv.StoreItemVariationValueId

                          select new PurchaseOrderItemObject
                          {
                              StoreItemStockId = sc.StoreItemStockId,
                              SerialNumber = sc.SKU,
                              QuantityDelivered = p.QuantityDelivered,
                              PurchaseOrderItemId = p.PurchaseOrderItemId,
                              PurchaseOrderId = orderObject.PurchaseOrderId,
                              StatusCode = p.StatusCode,
                              QuantityOrdered = p.QuantityOrdered,
                              StoreItemName = iv == null ? si.Name : si.Name + "/" + iv.Value

                          }).ToList();

                        if (purchaseItems.Any())
                        {
                            var x = orderItems[0];
                            var uoms = (from it in db.ItemPrices.Where(i => i.StoreItemStockId == x.StoreItemStockId).OrderByDescending(a => a.MinimumQuantity).Take(1).Include("UnitsOfMeasurement")
                                        select new ItemPriceObject
                                        {
                                            UoMCode = it.UnitsOfMeasurement.UoMCode,
                                            Price = it.Price,
                                            MinimumQuantity = it.MinimumQuantity,
                                            ItemPriceId = it.ItemPriceId

                                        }).ToList();

                            if (uoms.Any())
                            {
                                x.UoMCode = uoms[0].UoMCode;
                                x.Price = uoms[0].Price;
                                x.ItemPriceId = uoms[0].ItemPriceId;
                                x.MinimumQuantity = uoms[0].MinimumQuantity;
                            }

                            x.PurchaseOrderItemDeliveryObjects = new List<PurchaseOrderItemDeliveryObject>();
                            var porderDeliveryItems = db.PurchaseOrderItemDeliveries.Where(o => o.PurchaseOrderItemId == x.PurchaseOrderItemId).ToList();
                            if (porderDeliveryItems.Any())
                            {
                                x.TotalQuantityDelivered = porderDeliveryItems.Sum(r => r.QuantityDelivered);
                                porderDeliveryItems.ForEach(d =>
                                {
                                    var employees = db.UserProfiles.Where(f => f.Employees.Any(e => e.EmployeeId == d.ReceivedById)).ToList();
                                    if (!employees.Any())
                                    {
                                        return;
                                    }
                                    x.PurchaseOrderItemDeliveryObjects.Add(new PurchaseOrderItemDeliveryObject
                                    {
                                        DateDeliveredStr = d.DateDelivered.ToString("dd/MM/yyyy"),
                                        PurchaseOrderItemDeliveryId = d.PurchaseOrderItemDeliveryId,
                                        PurchaseOrderItemId = d.PurchaseOrderItemId,
                                        PurchaseOrderId = x.PurchaseOrderId,
                                        CostPrice = p.CostPrice,
                                        TotalCost = p.QuantityOrdered * p.CostPrice,
                                        QuantityOrdered = p.QuantityOrdered,
                                        QuantityDelivered = d.QuantityDelivered,
                                        DateDelivered = d.DateDelivered,
                                        ExpiryDate = d.ExpiryDate,
                                        ExpiryDateStr = d.ExpiryDate != null ? ((DateTime)d.ExpiryDate).ToString("dd/MM/yyyy") : "",
                                        ReceivedById = d.ReceivedById,
                                        EmployeeName = employees[0].LastName + " " + employees[0].OtherNames
                                    });
                                    
                                });
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

                            var name = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), x.StatusCode);
                            if (name != null)
                            {
                                x.DeliveryStatus = name.Replace("_", " ");
                            }

                            x.ImagePath = string.IsNullOrEmpty(img.ImagePath) ? "/Content/images/noImage.png" : img.ImagePath.Replace("~", "");

                            purchaseItemObjects.Add(x);
                        }
                    });

                    if (!purchaseItemObjects.Any())
                    {
                        return new PurchaseOrderObject();
                    }
                    
                    orderObject.PurchaseOrderItemObjects = purchaseItemObjects;

                    orderObject.GeneratedByEmployeeName = user[0].LastName + " " + user[0].OtherNames;

                    var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                    if (delStatus != null)
                    {
                        orderObject.DeliveryStatus = delStatus.Replace("_", " "); 
                    }
                    
                    orderObject.OutletName = storeOutlet.OutletName;
                    
                    orderObject.SupplierName = supplier.CompanyName;
                    orderObject.AccountGroupName = accountGroup[0].Name;
 
                    orderObject.PurchaseOrderPaymentObjects = new List<PurchaseOrderPaymentObject>();
                    if (purchasePayments.Any())
                    {
                        purchasePayments.ToList().ForEach(i =>
                        {
                            var p = ModelCrossMapper.Map<PurchaseOrderPayment, PurchaseOrderPaymentObject>(i);
                            if (p == null || p.PurchaseOrderPaymentId < 1)
                            {
                                return;
                            }

                            orderObject.PurchaseOrderPaymentObjects.Add(p);
                        });
                    }

                    orderObject.InvoiceObjects = new List<InvoiceObject>();

                    if (invoices.Any())
                    {
                        invoices.ToList().ForEach(i =>
                        {
                            orderObject.InvoiceObjects.Add(new InvoiceObject
                            {
                                InvoiceId = i.InvoiceId,
                                PurchaseOrderId = i.PurchaseOrderId,
                                ReferenceCode = i.ReferenceCode,
                                StatusCode = i.StatusCode,
                                DueDate = i.DueDate,
                                DateSent = i.DateSent,
                                DateSentStr = i.DateSent.ToString("dd/MM/yyyy"),
                                Attachment = i.Attachment
                            });
                        });
                    }
                    
                    orderObject.DateTimePlacedStr = orderObject.DateTimePlaced.ToString("dd/MM/yyyy");
                    orderObject.ExpectedDeliveryDateStr = orderObject.ExpectedDeliveryDate.ToString("dd/MM/yyyy");
                    return orderObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PurchaseOrderObject();
            }
        }
        public List<PurchaseOrderObject> GetPurchaseOrders(int? itemsPerPage, int? pageNumber, out int count)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount").Include("PurchaseOrderItems").Include("Employee").Include("Supplier").Include("StoreOutlet").Include("PurchaseOrderPayments").Include("Invoices")
                                .ToList();

                        if (purchaseOrders.Any()){
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }

                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }

                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> GetPurchaseOrdersByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.Where(o => o.StoreOutletId == outletId).OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount")
                                .Include("Employee")
                                .Include("StoreOutlet")
                                .Include("Supplier")
                                .ToList();

                        if (purchaseOrders.Any())
                        {
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }

                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }
                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }
                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count(o => o.StoreOutletId == outletId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> GetPurchaseOrdersByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.Where(o => o.GeneratedById == employeeId).OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount")
                                .Include("Employee")
                                .Include("StoreOutlet")
                                .Include("Supplier")
                                .ToList();

                        if (purchaseOrders.Any())
                        {
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.ActualDeliveryDate != null ? ((DateTime)order.ActualDeliveryDate).ToString("dd/MM/yyyy") : "";
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }
                                    
                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }

                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }

                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count(o => o.GeneratedById == employeeId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> SearchPurchaseOrders(string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o =>  
                        (o.PurchaseOrderNumber.ToLower().Contains(searchCriteria.ToLower())
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower())))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> SearchOutletPurchaseOrder(string searchCriteria, int outletId)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o => o.StoreOutletId == outletId && 
                        (o.PurchaseOrderNumber.ToLower().Contains(searchCriteria.ToLower())
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower())))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> SearchEmployeePurchaseOrder(string searchCriteria, long employeeId)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o => o.GeneratedById == employeeId && 
                        (o.PurchaseOrderNumber.ToLower().Contains(searchCriteria.ToLower())) 
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower()))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        public PurchaseOrderSelectable GetSelectables()
        {
            try
            {
                using (var db = _db)
                {
                    var chartsOfAccounts = db.ChartOfAccounts.ToList();
                    var suppliers = db.Suppliers.ToList();
                    //var outlets = db.StoreOutlets.ToList();
                    var pSelectable = new PurchaseOrderSelectable
                    {
                        ChartOfAccounts = new List<ChartOfAccountObject>(),
                        Suppliers = new List<SupplierObject>(),
                        StoreOutlets = new List<StoreOutletObject>()
                    };
                    
                    if (chartsOfAccounts.Any())
                    {
                        chartsOfAccounts.ForEach(c =>
                        {
                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == c.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                var accObj = ModelCrossMapper.Map<ChartOfAccount, ChartOfAccountObject>(c);
                                if (accObj != null && accObj.ChartOfAccountId > 0)
                                {
                                    accObj.AccountGroupName = accObj.AccountCode + "(" + groups[0].Name + ")";
                                    pSelectable.ChartOfAccounts.Add(accObj);
                                }
                            }
                            
                        });
                        
                    }

                    if (suppliers.Any())
                    {
                        suppliers.ForEach(c =>
                        {
                            var supObj = ModelCrossMapper.Map<Supplier, SupplierObject>(c);
                            if (supObj != null && supObj.SupplierId > 0)
                            {
                                pSelectable.Suppliers.Add(supObj);
                            }
                        });

                    }

                    //if (outlets.Any())
                    //{
                    //    outlets.ForEach(c =>
                    //    {
                    //        var outletObj = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(c);
                    //        if (outletObj != null && outletObj.StoreOutletId > 0)
                    //        {
                    //            pSelectable.StoreOutlets.Add(outletObj);
                    //        }
                    //    });

                    //}

                    return pSelectable;
                }

            }
            catch (DbEntityValidationException e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return new PurchaseOrderSelectable();
            }
        }

    }
}
