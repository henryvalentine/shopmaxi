using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class SaleRepository
    {
        private readonly IShopkeeperRepository<Sale> _repository;
        private readonly UnitOfWork _uoWork;
        private ShopKeeperStoreEntities _db;

       public SaleRepository()
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
       public long AddSale(SaleObject sale, out string invoiceRef)
       {
           try
           {
               if (sale == null)
               {
                   invoiceRef = "";
                   return -2;
               }

               var saleEntity = ModelCrossMapper.Map<SaleObject, Sale>(sale);
               if (saleEntity == null || saleEntity.AmountDue < 1)
               {
                   invoiceRef = "";
                   return -2;
               }

               string estimateNumber = null;
               using (var db = _db)
               {
                   var estimateInfo = new Estimate();
                   if (sale.ProcessEstimate && !string.IsNullOrEmpty(sale.EstimateNumber))
                   {
                       var estimates = db.Estimates.Where(m => m.EstimateNumber == sale.EstimateNumber).ToList();
                       if (!estimates.Any())
                       {
                           db.SaveChanges();
                           invoiceRef = "";
                           return -2;
                       }

                       var estimate = estimates[0];
                       estimate.ConvertedToInvoice = true;
                       db.Entry(estimate).State = EntityState.Modified;
                       db.SaveChanges();
                       estimateNumber = sale.EstimateNumber;
                       estimateInfo = estimate;
                   }

                   if (sale.CustomerObject != null && sale.CustomerObject.CustomerId < 1 && !string.IsNullOrEmpty(sale.CustomerObject.LastName) && sale.CustomerId < 1 && !string.IsNullOrEmpty(sale.CustomerObject.OtherNames) && !string.IsNullOrEmpty(sale.CustomerObject.MobileNumber))
                   {
                       var duplicateNumbers = db.UserProfiles.Count(d => d.MobileNumber.Trim() == sale.CustomerObject.MobileNumber.Trim());
                       if (duplicateNumbers > 0)
                       {
                           invoiceRef = "";
                           return -7;
                       }
                       var cObj = sale.CustomerObject;
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
                       if (customerEntity != null && !string.IsNullOrEmpty(sale.CustomerObject.LastName))
                       {
                           customerEntity.DateProfiled = DateTime.Now;
                           customerEntity.UserId = profile.Id;
                           customerEntity.ContactPersonId = sale.EmployeeId;
                           customerEntity.StoreOutletId = sale.OutletId;
                           var customer = db.Customers.Add(customerEntity);
                           db.SaveChanges();
                           saleEntity.CustomerId = customer.CustomerId;
                           if (estimateInfo.Id > 0)
                           {
                               estimateInfo.CustomerId = customer.CustomerId;
                               db.Entry(estimateInfo).State = EntityState.Modified;
                               db.SaveChanges();
                           }
                       }
                   }

                   var count = db.Sales.Count() + 1;
                   var invoiceNumber = InvoiceNumberGenerator.GenerateNumber(count);
                   saleEntity.InvoiceNumber = invoiceNumber;
                   saleEntity.EstimateNumber = estimateNumber;

                   var processedSale = db.Sales.Add(saleEntity);
                   db.SaveChanges();

                   invoiceRef = invoiceNumber;
                   return processedSale.SaleId;
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
               invoiceRef = "";
               return 0;
           }
       }

       public long UpdateSalePayment(SaleObject sale, UserProfileObject userProfile)
       {
           try
           {
               if (sale == null)
               {
                   return -2;
               }

               using (var db = _db)
               {
                   var transactions = sale.Transactions.Where(t => t.StoreTransactionId < 1).ToList();
                   if (!transactions.Any())
                   {
                       return -2;
                   }

                   var saleEntities = db.Sales.Where(s => s.SaleId == sale.SaleId).Include("SalePayments").Include("SaleTransactions").ToList();
                   if (!saleEntities.Any())
                   {
                       return -2;
                   }

                   var saleEntity = saleEntities[0];

                   if (!saleEntity.SalePayments.Any() || !saleEntity.SaleTransactions.Any())
                   {
                       return -2;
                   }

                   var amountPaid = transactions.Sum(m => m.TransactionAmount);

                   if (saleEntity.CustomerId != null && saleEntity.CustomerId > 0)
                   {
                       var customerInvoices = db.CustomerInvoices.Where(c => c.CustomerId == saleEntity.CustomerId).ToList();
                       if (!customerInvoices.Any())
                       {
                           return -2;
                       }

                       var customerInvoice = customerInvoices[0];
                       customerInvoice.TotalAmountPaid += amountPaid;
                       customerInvoice.InvoiceBalance = ((customerInvoice.TotalAmountDue + customerInvoice.TotalVATAmount) - customerInvoice.TotalDiscountAmount) - customerInvoice.TotalAmountPaid;
                       db.Entry(customerInvoice).State = EntityState.Modified;
                       db.SaveChanges();
                   }


                   var totalAmountPaid = amountPaid;
                   var transHolder = new List<StoreTransaction>();

                   var saleTransactions = saleEntity.SaleTransactions.ToList();

                   saleTransactions.ForEach(s =>
                   {
                       var transX = db.StoreTransactions.Where(r => r.StoreTransactionId == s.StoreTransactionId).ToList();
                       if (!transX.Any())
                       {
                           return;
                       }
                       totalAmountPaid += transX[0].TransactionAmount;
                       transHolder.Add(transX[0]);
                   });

                   transactions.ForEach(m =>
                   {
                       var payment = new SalePayment
                       {
                           AmountPaid = amountPaid,
                           DatePaid = DateTime.Now,
                           SaleId = saleEntity.SaleId
                       };

                       db.SalePayments.Add(payment);
                       db.SaveChanges();

                       var exis = transHolder.Find(t => t.StorePaymentMethodId == m.StorePaymentMethodId);
                       if (exis == null || exis.StoreTransactionId < 1)
                       {
                           m.EffectedByEmployeeId = userProfile.EmployeeId;
                           m.TransactionDate = DateTime.Now;
                           m.StoreOutletId = userProfile.StoreOutletId;
                           db.SaveChanges();
                       }
                       else
                       {
                           exis.TransactionAmount += m.TransactionAmount;
                       }
                   });

                   saleEntity.Status = sale.NetAmount.Equals(totalAmountPaid) ? (int)SaleStatus.Completed : (int)SaleStatus.Partly_Paid;
                   db.Entry(saleEntity).State = EntityState.Modified;
                   db.SaveChanges();

                   return sale.SaleId;
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

       public long RefundSale(RefundNoteObject refundNote, out string refundNoteNumber)
       {
           try
           {
               if (refundNote == null)
               {
                   refundNoteNumber = "";
                   return -2;
               }
               var successCount = 0;
               using (var db = _db)
               {
                   var sales = db.Sales.Where(d => d.SaleId == refundNote.SaleId).ToList();
                   if (!sales.Any())
                   {
                       refundNoteNumber = "";
                       return -2;
                   }

                   var refundNoteEntity = ModelCrossMapper.Map<RefundNoteObject, RefundNote>(refundNote);
                   if (refundNoteEntity == null || refundNoteEntity.SaleId < 1)
                   {
                       refundNoteNumber = "";
                       return -2;
                   }

                   var transaction = new StoreTransaction
                   {
                       StoreTransactionTypeId = (int)TransactionTypeEnum.Debit,
                       StorePaymentMethodId = refundNote.PaymentMethodId,
                       EffectedByEmployeeId = refundNote.EmployeeId,
                       TransactionAmount = refundNote.NetAmount,
                       TransactionDate = refundNote.DateReturned,
                       StoreOutletId = refundNote.OutletId
                   };

                   var processedTransaction = db.StoreTransactions.Add(transaction);
                   db.SaveChanges();

                   refundNoteEntity.TransactionId = processedTransaction.StoreTransactionId;
                   var k = db.RefundNotes.Count() + 1;
                   var refNoteNumber = InvoiceNumberGenerator.GenerateNumber(k);

                   refundNoteEntity.RefundNoteNumber = refNoteNumber;

                   var processedRefundNote = db.RefundNotes.Add(refundNoteEntity);
                   db.SaveChanges();

                   refundNoteNumber = refNoteNumber;

                   if (refundNote.CustomerId != null && refundNote.CustomerId > 0)
                   {
                       var customerInvoices = db.CustomerInvoices.Where(p => p.CustomerId == refundNote.CustomerId).ToList();

                       if (customerInvoices.Any())
                       {
                           var invoice = customerInvoices[0];

                           invoice.TotalAmountPaid -= refundNote.TotalAmountRefunded;
                           invoice.TotalAmountDue -= refundNote.AmountDue;
                           invoice.TotalVATAmount -= refundNote.VATAmount;
                           invoice.TotalDiscountAmount -= refundNote.DiscountAmount;

                           invoice.InvoiceBalance = ((invoice.TotalAmountDue - invoice.TotalDiscountAmount) + invoice.TotalVATAmount) - invoice.TotalAmountPaid;

                           db.Entry(invoice).State = EntityState.Modified;
                           db.SaveChanges();
                       }

                   }

                   var serviceCategory = System.Configuration.ConfigurationManager.AppSettings["ServiceCategoryId"];

                   var categoryId = 0;

                   if (!string.IsNullOrEmpty(serviceCategory))
                   {
                       int.TryParse(serviceCategory, out categoryId);
                   }

                   refundNote.ReturnedProductObjects.ForEach(u =>
                   {
                       var soldItems = db.StoreItemSolds.Where(e => e.StoreItemStockId == u.StoreItemStockId && e.SaleId == refundNote.SaleId).ToList();
                       if (!soldItems.Any())
                       {
                           return;
                       }

                       var myItems = db.StoreItemStocks.Where(i => i.StoreItemStockId == u.StoreItemStockId).Include("StoreItem").ToList();
                       if (!myItems.Any())
                       {
                           return;
                       }

                       var myItem = myItems[0];
                       var storeItem = myItem.StoreItem;

                       if (storeItem.StoreItemCategoryId != categoryId)
                       {
                           var orderedItems = db.PurchaseOrderItems.Where(i => i.PurchaseOrderItemId == u.PurchaseOrderItemId).ToList();
                           if (orderedItems.Any())
                           {
                               var orderedItem = orderedItems[0];
                               orderedItem.QuantityInStock += u.QuantityReturned;
                               db.Entry(orderedItem).State = EntityState.Modified;
                               db.SaveChanges();
                           }

                           //update stock accordingly
                           myItem.QuantityInStock += u.QuantityReturned;
                           myItem.TotalQuantityAlreadySold -= u.QuantityReturned;
                           db.Entry(myItem).State = EntityState.Modified;
                           db.SaveChanges();

                           //update returned quantity of sold item to keep track of what is left after refund 
                           //but don't deduct the original quantity sold
                           var x = soldItems[0];
                           x.QuantityReturned += u.QuantityReturned;
                           db.Entry(x).State = EntityState.Modified;
                           db.SaveChanges();
                       }

                       u.DateReturned = DateTime.Now;
                       u.RefundNoteId = processedRefundNote.Id;

                       var refundItemEntity = ModelCrossMapper.Map<ReturnedProductObject, ReturnedProduct>(u);
                       if (refundItemEntity == null || refundItemEntity.RefundNoteId < 1)
                       {
                           return;
                       }

                       db.ReturnedProducts.Add(refundItemEntity);
                       db.SaveChanges();
                       successCount += 1;

                   });

                   var sl = sales[0];

                   sl.Status = (int)SaleStatus.Refund_Note_Issued;
                   db.Entry(sl).State = EntityState.Modified;

                   db.SaveChanges();

                   return successCount == refundNote.ReturnedProductObjects.Count() ? processedRefundNote.Id : -8;
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
               refundNoteNumber = "";
               return 0;
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

       public int UpdateSale(SaleObject sale)
       {
           try
           {
               if (sale == null)
               {
                   return -2;
               }

               var saleEntity = ModelCrossMapper.Map<SaleObject, Sale>(sale);
               if (saleEntity == null || saleEntity.SaleId < 1)
               {
                   return -2;
               }
               _repository.Update(saleEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteSale(long saleId)
       {
           try
           {
               var returnStatus = _repository.Remove(saleId);
               _uoWork.SaveChanges();
               return returnStatus.SaleId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public List<CustomerObject> GetCustomers()
       {
           try
           {
               using (var db = _db)
               {
                   var customers = (from sa in db.Customers.OrderBy(m => m.UserProfile.LastName).Include("UserProfile").Include("StoreCustomerType")

                                    select new CustomerObject
                                    {
                                        CustomerId = sa.CustomerId,
                                        UserId = sa.UserId,
                                        UserProfileName = sa.UserProfile.LastName + " " + sa.UserProfile.OtherNames + "(" + sa.UserProfile.MobileNumber + ")",
                                        StoreOutletId = sa.StoreOutletId,
                                        FirstPurchaseDate = sa.FirstPurchaseDate,
                                        ContactEmail = sa.UserProfile.ContactEmail,
                                        MobileNumber = sa.UserProfile.MobileNumber,
                                        CreditLimit = sa.StoreCustomerType.CreditLimit,
                                        CreditWorthy = sa.StoreCustomerType.CreditWorthy,
                                        OfficeLine = sa.UserProfile.OfficeLine
                                    }).ToList();

                   if (!customers.Any())
                   {
                       return new List<CustomerObject>();
                   }

                   customers.ForEach(c =>
                   {
                       var tt = db.CustomerInvoices.Where(t => t.CustomerId == c.CustomerId).ToList();
                       if (tt.Any())
                       {
                           c.TotalAmountPaid = tt[0].TotalAmountPaid;
                           c.TotalAmountDue = tt[0].TotalAmountDue;
                           c.InvoiceBalance = tt[0].InvoiceBalance;
                           c.TotalVATAmount = tt[0].TotalVATAmount;
                           c.TotalDiscountAmount = tt[0].TotalDiscountAmount;
                       }
                   });

                   return customers;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<CustomerObject>();
           }
       }

       public List<CustomerObject> GetCustomers(int pageNumber, int itemsPerPage)
       {
           try
           {
               using (var db = _db)
               {
                   var customers = (from sa in db.Customers.OrderBy(m => m.UserProfile.LastName).Include("UserProfile").Include("StoreCustomerType").Skip((pageNumber) * itemsPerPage).Take(itemsPerPage)

                                    select new CustomerObject
                                    {
                                        CustomerId = sa.CustomerId,
                                        UserId = sa.UserId,
                                        UserProfileName = sa.UserProfile.LastName + " " + sa.UserProfile.OtherNames + "(" + sa.UserProfile.MobileNumber + ")",
                                        StoreOutletId = sa.StoreOutletId,
                                        FirstPurchaseDate = sa.FirstPurchaseDate,
                                        ContactEmail = sa.UserProfile.ContactEmail,
                                        MobileNumber = sa.UserProfile.MobileNumber,
                                        CreditLimit = sa.StoreCustomerType.CreditLimit,
                                        CreditWorthy = sa.StoreCustomerType.CreditWorthy,
                                        OfficeLine = sa.UserProfile.OfficeLine
                                    }).ToList();

                   if (!customers.Any())
                   {
                       return new List<CustomerObject>();
                   }

                   customers.ForEach(c =>
                   {
                       var tt = db.CustomerInvoices.Where(t => t.CustomerId == c.CustomerId).ToList();
                       if (tt.Any())
                       {
                           c.TotalAmountPaid = tt[0].TotalAmountPaid;
                           c.TotalAmountDue = tt[0].TotalAmountDue;
                           c.InvoiceBalance = tt[0].InvoiceBalance;
                           c.TotalVATAmount = tt[0].TotalVATAmount;
                           c.TotalDiscountAmount = tt[0].TotalDiscountAmount;
                       }
                   });

                   return customers;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<CustomerObject>();
           }
       }


       public StateReportDefault GetCustomersAndSuppliers()
       {
           try
           {
               using (var db = _db)
               {
                   var suppliers = (from s in db.Suppliers.OrderBy(m => m.CompanyName)
                                    select new SupplierObject
                                    {
                                        SupplierId = s.SupplierId,
                                        DateJoined = s.DateJoined,
                                        LastSupplyDate = s.LastSupplyDate,
                                        Note = s.Note,
                                        CompanyName = s.CompanyName,
                                        TIN = s.TIN

                                    }).ToList();

                   var categories = (from s in db.StoreItemCategories.OrderBy(m => m.Name)
                                     select new StoreItemCategoryObject
                                     {
                                         StoreItemCategoryId = s.StoreItemCategoryId,
                                         Name = s.Name

                                     }).ToList();

                   var statementDefault = new StateReportDefault
                   {
                       Categories = new List<StoreItemCategoryObject>(),
                       Suppliers = new List<SupplierObject>()
                   };

                   statementDefault.Categories = categories;
                   statementDefault.Suppliers = suppliers;
                   return statementDefault;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new StateReportDefault();
           }
       }

       public List<StoreStateObject> GetStates(long countryId)
       {
           try
           {
               using (var db = _db)
               {
                   var states = db.StoreStates.Where(s => s.StoreCountryId == countryId).OrderBy(m => m.Name).Include("StoreCities").ToList();

                   if (!states.Any())
                   {
                       return new List<StoreStateObject>();
                   }

                   var stateObjects = new List<StoreStateObject>();
                   states.ForEach(f =>
                   {
                       var stateObj = ModelCrossMapper.Map<StoreState, StoreStateObject>(f);
                       if (stateObj == null || stateObj.StoreStateId < 1)
                       {
                           return;
                       }

                       stateObj.StoreCityObjects = new List<StoreCityObject>();
                       f.StoreCities.ToList().ForEach(c =>
                       {
                           stateObj.StoreCityObjects.Add(new StoreCityObject
                           {
                               StoreCityId = c.StoreCityId,
                               Name = c.Name,
                               StoreStateId = c.StoreStateId
                           });
                       });

                       stateObjects.Add(stateObj);
                   });

                   return stateObjects;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreStateObject>();
           }
       }

       public List<StoreCountryObject> GetCountries()
       {
           try
           {
               using (var db = _db)
               {
                   var countries = (from c in db.StoreCountries.OrderBy(m => m.Name)
                                    select new StoreCountryObject
                                    {
                                        StoreCountryId = c.StoreCountryId,
                                        Name = c.Name
                                    }).ToList();

                   if (!countries.Any())
                   {
                       return new List<StoreCountryObject>();
                   }
                   return countries;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreCountryObject>();
           }
       }

       public SaleObject GetSale(long saleId)
       {
           try
           {
               using (var db = _db)
               {
                   var myItems =
                       (from sa in db.Sales.Where(m => m.SaleId == saleId)
                        join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                        join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                        join ps in db.UserProfiles on em.UserId equals ps.Id
                        select new SaleObject
                        {
                            SaleId = sa.SaleId,
                            RegisterId = sa.RegisterId,
                            EmployeeId = em.EmployeeId,
                            AmountDue = sa.AmountDue,
                            CustomerName = ps.LastName + " " + ps.OtherNames + "(" + ps.MobileNumber + ")",
                            Status = sa.Status,
                            Date = sa.Date,
                            DiscountAmount = sa.DiscountAmount,
                            VATAmount = sa.VATAmount,
                            RegisterName = rs.Name,
                            SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                            Discount = sa.Discount,
                            VAT = sa.VAT,
                            InvoiceNumber = sa.InvoiceNumber,
                            NetAmount = sa.NetAmount
                        }).ToList();

                   if (!myItems.Any())
                   {
                       return new SaleObject();
                   }

                   var target = myItems[0];

                   target.Transactions = new List<StoreTransactionObject>();
                   target.SoldItems = new List<StoreItemSoldObject>();

                   target.Transactions = (from sta in db.SaleTransactions.Where(m => m.SaleId == target.SaleId)
                                          join st in db.StoreTransactions on sta.StoreTransactionId equals st.StoreTransactionId
                                          join ef in db.Employees on st.EffectedByEmployeeId equals ef.EmployeeId
                                          join ep in db.UserProfiles on ef.UserId equals ep.Id
                                          join pm in db.StorePaymentMethods on st.StorePaymentMethodId equals pm.StorePaymentMethodId
                                          join ty in db.StoreTransactionTypes on st.StoreTransactionTypeId equals ty.StoreTransactionTypeId
                                          join so in db.StoreOutlets on st.StoreOutletId equals so.StoreOutletId
                                          select new StoreTransactionObject
                                          {
                                              StoreTransactionId = st.StoreTransactionId,
                                              StoreTransactionTypeId = ty.StoreTransactionTypeId,
                                              StorePaymentMethodId = pm.StorePaymentMethodId,
                                              EffectedByEmployeeId = ef.EmployeeId,
                                              TransactionEmployeeName = ep.LastName + " " + ep.OtherNames,
                                              TransactionAmount = st.TransactionAmount,
                                              TransactionDate = st.TransactionDate,
                                              Remark = st.Remark,
                                              StoreOutletId = so.StoreOutletId,
                                              OutletName = so.OutletName,
                                              StoreTransactionTypeName = ty.Name,
                                              PaymentMethodName = pm.Name
                                          }).ToList();

                   //StoreName StoreAddress CustomerName CustomerAddress InvoiceNumber UnitPrice

                   var soldItems = (
                   from iss in db.StoreItemSolds.Where(m => m.SaleId == saleId)
                   join sts in db.StoreItemStocks on iss.StoreItemStockId equals sts.StoreItemStockId
                   //join su in db.StockUploads on sts.StoreItemStockId equals su.StoreItemStockId
                   join vv in db.StoreItemVariationValues on sts.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                   join it in db.ItemPrices on sts.StoreItemStockId equals it.StoreItemStockId
                   join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                   join si in db.StoreItems on sts.StoreItemId equals si.StoreItemId
                   select new StoreItemSoldObject
                   {
                       ItemSoldName = vv == null ? si.Name : si.Name + "/" + vv.Value,
                       Sku = sts.SKU,
                       QuantitySold = iss.QuantitySold,
                       UnitPrice = it.Price,
                       AmountSold = iss.AmountSold,
                       UoMCode = um.UoMCode,
                       DateSold = iss.DateSold,
                       QuantityBalance = iss.QuantityBalance
                   }).ToList();

                   soldItems.ForEach(m =>
                   {
                       //m.ImagePath = m.ImagePath == null || string.IsNullOrEmpty(m.ImagePath) ? "/Content/images/noImage.png" : m.ImagePath.Replace("~", "");
                       if (!target.SoldItems.Exists(s => s.StoreItemSoldId == m.StoreItemSoldId))
                       {
                           m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                           target.SoldItems.Add(m);
                       }
                   });

                   target.Transactions.ForEach(m =>
                   {
                       target.AmountPaid += m.TransactionAmount;
                       m.TransactionDateStr = m.TransactionDate.ToString("dd/MM/yyyy");
                   });

                   var storeInfoList = db.StoreAddresses.Include("StoreCity").ToList();
                   if (storeInfoList.Any())
                   {
                       target.StoreAddress = storeInfoList[0].StreetNo + " " + storeInfoList[0].StoreCity.Name;
                   }
                   //StoreName StoreAddress CustomerName CustomerAddress InvoiceNumber 
                   target.DateStr = target.Date.ToString("dd/MM/yyyy");
                   return target;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new SaleObject();
           }
       }

       public SaleObject GetInvoice(long saleId)
       {
           try
           {
               using (var db = _db)
               {
                   var sales = (from sa in db.Sales.Where(l => l.SaleId == saleId)
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
                                    DiscountAmount = sa.DiscountAmount,
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
                   s.CustomerObject = new CustomerObject();
                   if (s.CustomerId != null && s.CustomerId > 0)
                   {
                       var customers = (from cu in db.Customers.Where(c => c.CustomerId == s.CustomerId).Include("StoreCustomerType")
                                        join inv in db.CustomerInvoices on cu.CustomerId equals inv.CustomerId
                                        join sp in db.UserProfiles on cu.UserId equals sp.Id
                                        select new CustomerObject
                                        {
                                            UserProfileName = sp.LastName + "(" + sp.MobileNumber + ")" + sp.OtherNames,
                                            CustomerId = cu.CustomerId,
                                            InvoiceBalance = inv.InvoiceBalance,
                                            CreditLimit = cu.StoreCustomerType.CreditLimit,
                                            CreditWorthy = cu.StoreCustomerType.CreditWorthy
                                        }).ToList();
                       if (customers.Any())
                       {
                           s.CustomerName = customers[0].UserProfileName;
                       }
                       else
                       {
                           s.CustomerName = "N/A";
                       }

                       s.CustomerObject = customers[0];
                   }

                   s.StoreItemSoldObjects = new List<StoreItemSoldObject>();
                   s.EmployeeObject = new EmployeeObject();
                   s.SalePaymentObjects = new List<SalePaymentObject>();
                   s.Transactions = new List<StoreTransactionObject>();

                   var tempItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                    join sti in db.StoreItemStocks.Include("StoreItemVariationValue") on sts.StoreItemStockId equals sti.StoreItemStockId
                                    join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                    join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                    select new StoreItemSoldObject
                                    {
                                        StoreItemName = sti.StoreItemVariationValue == null ? sto.Name : sto.Name + "/" + sti.StoreItemVariationValue.Value,
                                        QuantitySold = sts.QuantitySold,
                                        Sku = sti.SKU,
                                        QuantityBalance = sts.QuantityBalance,
                                        AmountSold = sts.AmountSold,
                                        StoreItemVariationValueId = sti.StoreItemVariationValueId,
                                        StoreItemStockId = sts.StoreItemStockId,
                                        UoMCode = ui.UoMCode,
                                        Rate = sts.Rate
                                    }).ToList();

                   if (!tempItems.Any())
                   {
                       return new SaleObject();
                   }

                   tempItems.ForEach(sts =>
                   {

                       var images = db.StockUploads.Where(i => i.StoreItemStockId == sts.StoreItemStockId).Include("ImageView").ToList();
                       if (images.Any())
                       {
                           var frontView = images.Find(o => o.ImageView.Name.ToLower() == "front view");
                           if (frontView == null || frontView.StockUploadId < 1)
                           {
                               frontView = images[0];
                           }

                           sts.ImagePath = frontView.ImagePath;
                       }

                       sts.ImagePath = string.IsNullOrEmpty(sts.ImagePath)
                           ? "/Content/images/noImage.png" : PhysicalToVirtualPathMapper.MapPath(sts.ImagePath);
                       s.StoreItemSoldObjects.Add(sts);
                   });

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
                   s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
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

       public SaleObject GetInvoiceForRevoke(string invoiceNumber)
       {
           try
           {
               using (var db = _db)
               {
                   var sales = (from sa in db.Sales.Where(l => l.InvoiceNumber == invoiceNumber)
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
                                    DiscountAmount = sa.DiscountAmount,
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

                   var customerNames = (from cu in db.Customers.Where(c => c.CustomerId == s.CustomerId)
                                        join sp in db.UserProfiles on cu.UserId equals sp.Id
                                        select sp.LastName + " " + sp.OtherNames + "(" + sp.MobileNumber + ")").ToList();

                   if (customerNames.Any())
                   {
                       s.CustomerName = customerNames[0];
                   }
                   else
                   {
                       s.CustomerName = "N/A";
                   }

                   s.StoreItemSoldObjects = new List<StoreItemSoldObject>();
                   s.EmployeeObject = new EmployeeObject();
                   s.SalePaymentObjects = new List<SalePaymentObject>();
                   s.Transactions = new List<StoreTransactionObject>();

                   var tempItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                    join sti in db.StoreItemStocks.Include("StoreItemVariationValue") on sts.StoreItemStockId equals sti.StoreItemStockId
                                    join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                    join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                    select new StoreItemSoldObject
                                    {
                                        StoreItemName = sti.StoreItemVariationValue == null ? sto.Name : sto.Name + "/" + sti.StoreItemVariationValue.Value,
                                        QuantitySold = sts.QuantitySold,
                                        Sku = sti.SKU,
                                        AmountSold = sts.AmountSold,
                                        StoreItemVariationValueId = sti.StoreItemVariationValueId,
                                        StoreItemStockId = sts.StoreItemStockId,
                                        UoMCode = ui.UoMCode,
                                        Rate = sts.Rate,
                                        QuantityReturned = sts.QuantityReturned,
                                        QuantityBalance = sts.QuantityBalance
                                    }).ToList();

                   if (!tempItems.Any())
                   {
                       return new SaleObject();
                   }

                   tempItems.ForEach(sts =>
                   {
                       sts.ItemPriceObjects = new List<ItemPriceObject>();
                       var prices = db.ItemPrices.Where(y => y.StoreItemStockId == sts.StoreItemStockId).ToList();
                       if (prices.Any())
                       {
                           prices.ForEach(v =>
                           {
                               var price = ModelCrossMapper.Map<ItemPrice, ItemPriceObject>(v);
                               if (price != null && price.ItemPriceId > 0)
                               {
                                   sts.ItemPriceObjects.Add(price);
                               }
                           });

                       }
                       //
                       var images = db.StockUploads.Where(i => i.StoreItemStockId == sts.StoreItemStockId).Include("ImageView").ToList();
                       if (images.Any())
                       {
                           var frontView = images.Find(o => o.ImageView.Name.ToLower() == "front view");
                           if (frontView == null || frontView.StockUploadId < 1)
                           {
                               frontView = images[0];
                           }

                           sts.ImagePath = frontView.ImagePath;
                       }

                       sts.ImagePath = string.IsNullOrEmpty(sts.ImagePath) ? "/Content/images/noImage.png" : PhysicalToVirtualPathMapper.MapPath(sts.ImagePath);
                       s.StoreItemSoldObjects.Add(sts);
                   });

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
                   s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                   s.DateStr = s.Date.ToString("dd/MM/yyyy");
                   s.Balance = s.NetAmount - s.AmountPaid;
                   return s;

               }
           }
           catch (Exception ex)
           {
               return new SaleObject();
           }
       }

       public List<RefundNoteObject> GetRefundedSaleNotes(long saleId)
       {
           try
           {
               using (var db = _db)
               {
                   var refundNotes = (from sa in db.RefundNotes.Where(l => l.SaleId == saleId).Include("Sale")
                                      join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                      join ps in db.UserProfiles on em.UserId equals ps.Id

                                      select new RefundNoteObject
                                      {
                                          Id = sa.Id,
                                          SaleId = sa.SaleId,
                                          AmountDue = sa.AmountDue,
                                          RefundNoteNumber = sa.RefundNoteNumber,
                                          PaymentMethodId = sa.PaymentMethodId,
                                          VATAmount = sa.VATAmount,
                                          NetAmount = sa.NetAmount,
                                          InvoiceNumber = sa.Sale.InvoiceNumber,
                                          Discount = sa.Discount,
                                          DiscountAmount = sa.DiscountAmount,
                                          CustomerId = sa.CustomerId,
                                          DateReturned = sa.DateReturned,

                                      }).ToList();

                   if (!refundNotes.Any())
                   {
                       return new List<RefundNoteObject>();
                   }

                   refundNotes.ForEach(s =>
                   {
                       if (s.IssueTypeId != null && s.IssueTypeId > 0)
                       {
                           var issues = db.IssueTypes.Where(d => d.IssueTypeId == s.IssueTypeId).ToList();
                           if (issues.Any())
                           {
                               s.Reason = issues[0].Name;
                           }
                       }
                       s.ReturnedProductObjects = new List<ReturnedProductObject>();

                       var returnedItems = (from rt in db.ReturnedProducts.Where(d => d.RefundNoteId == s.Id)
                                            join sti in db.StoreItemStocks.Include("StoreItemVariationValue") on rt.StoreItemStockId equals sti.StoreItemStockId
                                            join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                            join iss in db.IssueTypes on rt.IssueTypeId equals iss.IssueTypeId
                                            select new ReturnedProductObject
                                            {
                                                ReturnedProductId = rt.ReturnedProductId,
                                                IssueTypeId = rt.IssueTypeId,
                                                StoreItemStockId = rt.StoreItemStockId,
                                                RefundNoteId = rt.RefundNoteId,
                                                StoreItemName = sto.Name,
                                                Sku = sti.SKU,
                                                Reason = iss.Name,
                                                DateReturned = rt.DateReturned,
                                                AmountRefunded = rt.AmountRefunded,
                                                QuantityBought = rt.QuantityBought,
                                                QuantityReturned = rt.QuantityReturned
                                            }).ToList();

                       if (!returnedItems.Any())
                       {
                           return;
                       }

                       returnedItems.ForEach(sts =>
                       {
                           sts.ItemPriceObjects = new List<ItemPriceObject>();

                           var prices = db.ItemPrices.Where(y => y.StoreItemStockId == sts.StoreItemStockId).ToList();
                           if (prices.Any())
                           {
                               prices.ForEach(v =>
                               {
                                   var price = ModelCrossMapper.Map<ItemPrice, ItemPriceObject>(v);
                                   if (price != null && price.ItemPriceId > 0)
                                   {
                                       sts.ItemPriceObjects.Add(price);
                                   }
                               });

                           }

                           var images = db.StockUploads.Where(i => i.StoreItemStockId == sts.StoreItemStockId).Include("ImageView").ToList();
                           if (images.Any())
                           {
                               var frontView = images.Find(o => o.ImageView.Name.ToLower() == "front view");
                               if (frontView == null || frontView.StockUploadId < 1)
                               {
                                   frontView = images[0];
                               }

                               sts.ImagePath = frontView.ImagePath;
                           }

                           sts.ImagePath = string.IsNullOrEmpty(sts.ImagePath) ? "/Content/images/noImage.png" : PhysicalToVirtualPathMapper.MapPath(sts.ImagePath);

                       });
                       s.ReturnedProductObjects = returnedItems;
                       s.TotalAmountRefunded = s.NetAmount;
                       s.AmountDueStr = s.AmountDue.ToString("n0");
                       s.VATAmountStr = s.VAT.ToString("n0");
                       s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                       s.DateReturnedStr = s.DateReturned.ToString("dd/MM/yyyy");
                       s.RefundNoteNumber = InvoiceNumberGenerator.GenerateNumber(s.Id);
                   });

                   return refundNotes;
               }
           }
           catch (Exception ex)
           {
               return new List<RefundNoteObject>();
           }
       }

       public List<SaleObject> GetSalesByOutlet(long outletId, int? itemsPerPage, int? pageNumber)
       {
           try
           {
               using (var db = _db)
               {
                   if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                   {
                       var tpageNumber = (int)pageNumber;
                       var tsize = (int)itemsPerPage;
                       var myItems =
                           (from st in _db.StoreTransactions.Where(m => m.StoreOutletId == outletId).OrderByDescending(m => m.StoreTransactionId).Skip((tpageNumber) * tsize).Take(tsize)
                            join stt in db.SaleTransactions on st.StoreTransactionId equals stt.StoreTransactionId
                            join sa in db.Sales on stt.SaleId equals sa.SaleId
                            join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                            select new SaleObject
                            {
                                SaleId = sa.SaleId,
                                RegisterId = sa.RegisterId,
                                AmountDue = sa.AmountDue,
                                Status = sa.Status,
                                Date = sa.Date,
                                RegisterName = rs.Name,
                                Discount = sa.Discount,
                                VAT = sa.VAT,
                                NetAmount = sa.NetAmount
                            }).ToList();

                       if (!myItems.Any())
                       {
                           return new List<SaleObject>();
                       }
                       myItems.ForEach(m =>
                       {
                           m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                           m.DateStr = m.Date.ToString("dd/MM/yyyy");
                       });
                       return myItems;
                   }
                   return new List<SaleObject>();
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<SaleObject>();
           }
       }

       public List<SaleObject> GetSaleObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               using (var db = _db)
               {
                   if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                   {
                       var tpageNumber = (int)pageNumber;
                       var tsize = (int)itemsPerPage;

                       var saleObjectList = (from sa in db.Sales.OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                             join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                             join ps in db.UserProfiles on em.UserId equals ps.Id
                                             join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                             select new SaleObject
                                             {
                                                 SaleId = sa.SaleId,
                                                 RegisterId = sa.RegisterId,
                                                 EmployeeId = em.EmployeeId,
                                                 AmountDue = sa.AmountDue,
                                                 Status = sa.Status,
                                                 Date = sa.Date,
                                                 RegisterName = rs.Name,
                                                 SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                                 Discount = sa.Discount,
                                                 VAT = sa.VAT,
                                                 NetAmount = sa.NetAmount
                                             }).ToList();

                       if (!saleObjectList.Any())
                       {
                           return new List<SaleObject>();
                       }
                       saleObjectList.ForEach(m =>
                       {
                           m.DateStr = m.Date.ToString("dd/MM/yyyy");
                       });
                       return saleObjectList;
                   }

                   return new List<SaleObject>();
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<SaleObject>();
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

       public int GetObjectCount(Expression<Func<Sale, bool>> predicate)
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

       public List<SaleObject> Search(int outletId, string searchCriteria)
       {
           try
           {
               using (var db = _db)
               {
                   List<SaleObject> searchList;
                   DateTime date;
                   var result = DateTime.TryParse(searchCriteria, out date);
                   if (result)
                   {
                       searchList = (from sa in db.Sales.Where(m => m.Date == date)
                                     join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join st in db.StoreTransactions on stt.StoreTransactionId equals st.StoreTransactionId
                                     where st.StoreOutletId == outletId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         RegisterName = rs.Name,
                                         Discount = sa.Discount,
                                         VAT = sa.VAT,
                                         NetAmount = sa.NetAmount
                                     }).ToList();
                   }
                   else
                   {
                       searchList = (from st in db.StoreTransactions.Where(m => m.StoreOutletId == outletId).OrderBy(m => m.TransactionDate)
                                     join stt in db.SaleTransactions on st.StoreTransactionId equals stt.StoreTransactionId
                                     join sa in db.Sales on stt.SaleId equals sa.SaleId
                                     join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         RegisterName = rs.Name,
                                         Discount = sa.Discount,
                                         VAT = sa.VAT,
                                         NetAmount = sa.NetAmount
                                     }).ToList();
                   }

                   if (!searchList.Any())
                   {
                       return new List<SaleObject>();
                   }
                   searchList.ForEach(m =>
                   {
                       var customerNames = (from cu in db.Customers.Where(c => c.CustomerId == m.CustomerId)
                                            join sp in db.UserProfiles on cu.UserId equals sp.Id
                                            select sp.LastName + " " + sp.OtherNames + "(" + sp.MobileNumber + ")").ToList();

                       if (customerNames.Any())
                       {
                           m.CustomerName = customerNames[0];
                       }
                       else
                       {
                           m.CustomerName = "N/A";
                       }
                       m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                       m.DateStr = m.Date.ToString("dd/MM/yyyy");
                   });
                   return searchList;
               }

           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<SaleObject>();
           }
       }

       public List<SaleObject> Search(string searchCriteria)
       {
           try
           {
               using (var db = _db)
               {
                   List<SaleObject> searchList;
                   DateTime date;
                   var result = DateTime.TryParse(searchCriteria, out date);
                   if (result)
                   {
                       searchList = (from sa in db.Sales.Where(m => m.Date == date)
                                     join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                     join ps in db.UserProfiles on em.UserId equals ps.Id
                                     join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         RegisterName = rs.Name,
                                         SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                         Discount = sa.Discount,
                                         VAT = sa.VAT,
                                         NetAmount = sa.NetAmount
                                     }).ToList();
                   }
                   else
                   {
                       searchList = (from ps in db.UserProfiles.Where(m => m.LastName.Contains(searchCriteria) || m.OtherNames.Contains(searchCriteria))
                                     join em in db.Employees on ps.Id equals em.UserId
                                     join sa in db.Sales on em.EmployeeId equals sa.EmployeeId
                                     join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         EmployeeId = em.EmployeeId,
                                         AmountDue = sa.AmountDue,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         RegisterName = rs.Name,
                                         SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                         Discount = sa.Discount,
                                         VAT = sa.VAT,
                                         NetAmount = sa.NetAmount
                                     }).ToList();
                   }

                   if (!searchList.Any())
                   {
                       return new List<SaleObject>();
                   }
                   searchList.ForEach(m =>
                   {
                       var customerNames = (from cu in db.Customers.Where(c => c.CustomerId == m.CustomerId)
                                            join sp in db.UserProfiles on cu.UserId equals sp.Id
                                            select sp.LastName + " " + sp.OtherNames + "(" + sp.MobileNumber + ")").ToList();

                       if (customerNames.Any())
                       {
                           m.CustomerName = customerNames[0];
                       }

                       else
                       {
                           m.CustomerName = "N/A";
                       }

                       m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                       m.DateStr = m.Date.ToString("dd/MM/yyyy");
                   });
                   return searchList;
               }

           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<SaleObject>();
           }
       }

       public List<SaleObject> GetSales()
       {
           try
           {
               var saleEntityList = _repository.GetAll().ToList();
               if (!saleEntityList.Any())
               {
                   return new List<SaleObject>();
               }
               var saleObjList = new List<SaleObject>();
               saleEntityList.ForEach(m =>
               {
                   var saleObject = ModelCrossMapper.Map<Sale, SaleObject>(m);
                   if (saleObject != null && saleObject.SaleId > 0)
                   {
                       saleObjList.Add(saleObject);
                   }
               });
               return saleObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
    }
}
