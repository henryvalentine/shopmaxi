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
    public class SalePaymentRepository
    {
       private readonly IShopkeeperRepository<SalePayment> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _db;

       public SalePaymentRepository()
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

       public long AddSalePayment(SalePaymentObject salePayment)
       {
           try
           {
               if (salePayment == null)
               {
                   return -2;
               }
               using (var db = _db)
               {
                   var salePaymentEntity = ModelCrossMapper.Map<SalePaymentObject, SalePayment>(salePayment);
                   if (salePaymentEntity == null || salePaymentEntity.AmountPaid < 1)
                   {
                       return -2;
                   }

                   var returnStatus = db.SalePayments.Add(salePaymentEntity);
                   db.SaveChanges();
                   return returnStatus.SalePaymentId;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public long RefundSalePayment(SalePaymentObject salePayment, bool saleRevoked)
       {
           try
           {
               if (salePayment == null)
               {
                   return -2;
               }
               using (var db = _db)
               {
                   var pps = db.SalePayments.Where(o => o.SaleId == salePayment.SaleId).ToList();
                   if (!pps.Any())
                   {
                       return -2;
                   }
                   var p = pps[0];
                   p.AmountPaid = !saleRevoked ? p.AmountPaid : 0;
                   p.DatePaid = DateTime.Now;
                   db.Entry(p).State = EntityState.Modified;
                   db.SaveChanges();
                   return p.SalePaymentId;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public long ProcessCustomerInvoice(CustomerInvoiceObject customerInvoice)
       {
           try
           {
               if (customerInvoice == null)
               {
                   return -2;
               }

               using (var db = _db)
               {
                   var customerInvoices = db.CustomerInvoices.Where(p => p.CustomerId == customerInvoice.CustomerId).ToList();
                   var invoice = customerInvoices.Any() ? customerInvoices[0] : new CustomerInvoice();

                   if (invoice.Id < 1)
                   {

                       var totalVat = customerInvoice.TotalVATAmount + invoice.TotalVATAmount;
                       var discount = customerInvoice.TotalDiscountAmount + invoice.TotalDiscountAmount;
                       var totalDue = customerInvoice.AmountDue + invoice.TotalAmountDue;
                       var totalPaid = invoice.TotalAmountPaid + customerInvoice.AmountPaid;

                       invoice.InvoiceBalance = ((totalDue - discount) + totalVat) - totalPaid;

                       invoice.CustomerId = customerInvoice.CustomerId;
                       db.CustomerInvoices.Add(invoice);
                       db.SaveChanges();
                   }
                   else
                   {
                       invoice.TotalAmountPaid += customerInvoice.AmountPaid;
                       invoice.TotalVATAmount += customerInvoice.TotalVATAmount;
                       invoice.TotalDiscountAmount += customerInvoice.TotalDiscountAmount;
                       invoice.TotalAmountDue += customerInvoice.AmountDue;


                       invoice.InvoiceBalance = ((invoice.TotalAmountDue - invoice.TotalDiscountAmount) + invoice.TotalVATAmount) - invoice.TotalAmountPaid;

                       db.Entry(invoice).State = EntityState.Modified;
                       db.SaveChanges();
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

       public long BalanceCustomerInvoice(CustomerInvoiceObject customerInvoice)
       {
           try
           {
               if (customerInvoice == null)
               {
                   return -2;
               }

               using (var db = _db)
               {
                   var customerInvoices = db.CustomerInvoices.Where(p => p.CustomerId == customerInvoice.CustomerId).ToList();

                   if (!customerInvoices.Any())
                   {
                       return -2;
                   }

                   var invoice = customerInvoices[0];

                   invoice.TotalAmountPaid -= customerInvoice.AmountPaid;
                   invoice.TotalAmountDue -= customerInvoice.AmountDue;
                   invoice.TotalVATAmount -= customerInvoice.TotalVATAmount;
                   invoice.TotalDiscountAmount -= customerInvoice.TotalDiscountAmount;


                   invoice.InvoiceBalance = ((invoice.TotalAmountDue - invoice.TotalDiscountAmount) + invoice.TotalVATAmount) - invoice.TotalAmountPaid;

                   //if (customerInvoice.TotalAmountDue > customerInvoice.TotalAmountPaid)
                   //{
                   //    invoice.InvoiceBalance -= customerInvoice.TotalAmountDue - customerInvoice.TotalAmountPaid;
                   //}

                   db.Entry(invoice).State = EntityState.Modified;
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

       public int UpdateSalePayment(SalePaymentObject salePayment)
       {
           try
           {
               if (salePayment == null)
               {
                   return -2;
               }

               var salePaymentEntity = ModelCrossMapper.Map<SalePaymentObject, SalePayment>(salePayment);
               if (salePaymentEntity == null || salePaymentEntity.SalePaymentId < 1)
               {
                   return -2;
               }
               _repository.Update(salePaymentEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteSalePayment(long salePaymentId)
       {
           try
           {
               var returnStatus = _repository.Remove(salePaymentId);
               _uoWork.SaveChanges();
               return returnStatus.SalePaymentId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public SalePaymentObject GetSalePayment(long salePaymentId)
       {
           try
           {
               var myItem = _repository.GetById(salePaymentId);
               if (myItem == null || myItem.SalePaymentId < 1)
               {
                   return new SalePaymentObject();
               }
               var salePaymentObject = ModelCrossMapper.Map<SalePayment, SalePaymentObject>(myItem);
               if (salePaymentObject == null || salePaymentObject.SalePaymentId < 1)
               {
                   return new SalePaymentObject();
               }
               return salePaymentObject;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new SalePaymentObject();
           }
       }

       public List<SalePaymentObject> GetSalePaymentObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               List<SalePayment> salePaymentEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   salePaymentEntityList = _repository.GetWithPaging(m => m.SalePaymentId, tpageNumber, tsize).ToList();
               }

               else
               {
                   salePaymentEntityList = _repository.GetAll().ToList();
               }

               if (!salePaymentEntityList.Any())
               {
                   return new List<SalePaymentObject>();
               }
               var salePaymentObjList = new List<SalePaymentObject>();
               salePaymentEntityList.ForEach(m =>
               {
                   var salePaymentObject = ModelCrossMapper.Map<SalePayment, SalePaymentObject>(m);
                   if (salePaymentObject != null && salePaymentObject.SalePaymentId > 0)
                   {
                       salePaymentObjList.Add(salePaymentObject);
                   }
               });

               return salePaymentObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<SalePaymentObject>();
           }
       }

       //public List<SalePaymentObject> Search(string searchCriteria)
       //{
       //    try
       //    {
       //         var salePaymentEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "StoreState").ToList();

       //        if (!salePaymentEntityList.Any())
       //        {
       //            return new List<SalePaymentObject>();
       //        }
       //        var salePaymentObjList = new List<SalePaymentObject>();
       //        salePaymentEntityList.ForEach(m =>
       //        {
       //            var salePaymentObject = ModelCrossMapper.Map<SalePayment, SalePaymentObject>(m);
       //            if (salePaymentObject != null && salePaymentObject.SalePaymentId > 0)
       //            {
       //                salePaymentObject.StateName = m.StoreState.Name;
       //                salePaymentObjList.Add(salePaymentObject);
       //            }
       //        });
       //        return salePaymentObjList;
       //    }

       //    catch (Exception ex)
       //    {
       //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
       //        return new List<SalePaymentObject>();
       //    }
       //}

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

       public int GetObjectCount(Expression<Func<SalePayment, bool>> predicate)
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
       public List<SalePaymentObject> GetCities()
       {
           try
           {
               var salePaymentEntityList = _repository.GetAll().ToList();
               if (!salePaymentEntityList.Any())
               {
                   return new List<SalePaymentObject>();
               }
               var salePaymentObjList = new List<SalePaymentObject>();
               salePaymentEntityList.ForEach(m =>
               {
                   var salePaymentObject = ModelCrossMapper.Map<SalePayment, SalePaymentObject>(m);
                   if (salePaymentObject != null && salePaymentObject.SalePaymentId > 0)
                   {
                       salePaymentObjList.Add(salePaymentObject);
                   }
               });
               return salePaymentObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
    }
}
