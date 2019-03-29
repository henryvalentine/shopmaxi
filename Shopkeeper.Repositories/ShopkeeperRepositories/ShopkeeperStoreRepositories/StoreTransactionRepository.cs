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
    public class StoreTransactionRepository
    {
       private readonly IShopkeeperRepository<StoreTransaction> _repository;
       private readonly ShopKeeperStoreEntities _db;
       private readonly UnitOfWork _uoWork;

       public StoreTransactionRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
            _repository = new ShopkeeperRepository<StoreTransaction>(_uoWork);
            
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
               var returnStatus = _repository.Add(transactionEntity);
               _uoWork.SaveChanges();
               return returnStatus.StoreTransactionId;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public long RevokeStoreTransaction(StoreTransactionObject transaction, bool saleRevoked)
       {
           try
           {
               if (transaction == null)
               {
                   return -2;
               }
               using (var db = _db)
               {
                   var trs = db.SaleTransactions.Where(s => s.SaleId == transaction.SaleId).Include("StoreTransaction").ToList();
                   if (!trs.Any())
                   {
                       return -2;
                   }

                   var tr = trs[0];

                   tr.StoreTransaction.TransactionAmount = -tr.StoreTransaction.TransactionAmount;
                   tr.StoreTransaction.TransactionDate = DateTime.Now;
                   db.Entry(tr.StoreTransaction).State = EntityState.Modified;
                   db.SaveChanges();
                   return tr.StoreTransactionId;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public Int32 UpdateStoreTransaction(StoreTransactionObject transaction)
       {
           try
           {
               if (transaction == null)
               {
                   return -2;
               }
               var transactionEntity = ModelCrossMapper.Map<StoreTransactionObject, StoreTransaction>(transaction);
               if (transactionEntity == null || transactionEntity.StoreTransactionId < 1)
               {
                   return -2;
               }
               _repository.Update(transactionEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteStoreTransaction(long transactionId)
       {
           try
           {
               var returnStatus = _repository.Remove(transactionId);
               _uoWork.SaveChanges();
               return returnStatus.StoreTransactionId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public StoreTransactionObject GetStoreTransaction(long transactionId)
       {
           try
           {
               var myItem = _repository.Get(m => m.StoreTransactionId == transactionId, "StoreStorePaymentMethod, StoreTransactionType");
               if (myItem == null || myItem.StoreTransactionId < 1)
               {
                   return new StoreTransactionObject();
               }
               var transactionObject = ModelCrossMapper.Map<StoreTransaction, StoreTransactionObject>(myItem);
               if (transactionObject == null || transactionObject.StoreTransactionId < 1)
               {
                   return new StoreTransactionObject();
               }
               transactionObject.StoreTransactionTypeName = myItem.StoreTransactionType.Name;
               transactionObject.PaymentMethodName = myItem.StorePaymentMethod.Name;
               return transactionObject;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new StoreTransactionObject();
           }
       }

       public List<StoreTransactionObject> GetStoreTransactionObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               List<StoreTransaction> transactionEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   transactionEntityList = _repository.GetWithPaging(m => m.StoreTransactionId, tpageNumber, tsize, "StoreStorePaymentMethod, StoreTransactionType").ToList();
               }

               else
               {
                   transactionEntityList = _repository.GetAll("StoreStorePaymentMethod, StoreTransactionType").ToList();
               }

               if (!transactionEntityList.Any())
               {
                   return new List<StoreTransactionObject>();
               }
               var transactionObjList = new List<StoreTransactionObject>();
               transactionEntityList.ForEach(m =>
               {
                   var transactionObject = ModelCrossMapper.Map<StoreTransaction, StoreTransactionObject>(m);
                   if (transactionObject != null && transactionObject.StoreTransactionId > 0)
                   {
                       transactionObject.StoreTransactionTypeName = m.StoreTransactionType.Name;
                       transactionObject.PaymentMethodName = m.StorePaymentMethod.Name;
                       transactionObjList.Add(transactionObject);
                   }
               });

               return transactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreTransactionObject>();
           }
       }

       public List<StoreTransactionObject> GetStoreTransactionsByType(int? itemsPerPage, int? pageNumber, int transactionTypeId)
       {
           try
           {
               List<StoreTransaction> transactionEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   transactionEntityList = _repository.GetWithPaging(m => m.StoreTransactionTypeId == transactionTypeId, m => m.StoreTransactionId, tpageNumber, tsize, "StorePaymentMethod, StoreTransactionType").ToList();
               }

               else
               {
                   transactionEntityList = _repository.GetAll(m => m.StoreTransactionTypeId == transactionTypeId, "StoreStorePaymentMethod, StoreTransactionType").ToList();
               }

               if (!transactionEntityList.Any())
               {
                   return new List<StoreTransactionObject>();
               }
               var transactionObjList = new List<StoreTransactionObject>();
               transactionEntityList.ForEach(m =>
               {
                   var transactionObject = ModelCrossMapper.Map<StoreTransaction, StoreTransactionObject>(m);
                   if (transactionObject != null && transactionObject.StoreTransactionId > 0)
                   {
                       transactionObject.StoreTransactionTypeName = m.StoreTransactionType.Name;
                       transactionObject.PaymentMethodName = m.StorePaymentMethod.Name;
                       transactionObjList.Add(transactionObject);
                   }
               });

               return transactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreTransactionObject>();
           }
       }

       public List<StoreTransactionObject> GetStoreTransactionsByPaymentMethod(int? itemsPerPage, int? pageNumber, int paymentMethodId)
       {
           try
           {
               List<StoreTransaction> transactionEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   transactionEntityList = _repository.GetWithPaging(m => m.StorePaymentMethodId == paymentMethodId, m => m.StoreTransactionId, tpageNumber, tsize, "StorePaymentMethod, StoreTransactionType").ToList();
               }

               else
               {
                   transactionEntityList = _repository.GetAll(m => m.StorePaymentMethodId == paymentMethodId, "StorePaymentMethod, StoreTransactionType").ToList();
               }

               if (!transactionEntityList.Any())
               {
                   return new List<StoreTransactionObject>();
               }
               var transactionObjList = new List<StoreTransactionObject>();
               transactionEntityList.ForEach(m =>
               {
                   var transactionObject = ModelCrossMapper.Map<StoreTransaction, StoreTransactionObject>(m);
                   if (transactionObject != null && transactionObject.StoreTransactionId > 0)
                   {
                       transactionObject.StoreTransactionTypeName = m.StoreTransactionType.Name;
                       transactionObject.PaymentMethodName = m.StorePaymentMethod.Name;
                       transactionObjList.Add(transactionObject);
                   }
               });

               return transactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreTransactionObject>();
           }
       }

       public List<StoreTransactionObject> GetStoreTransactionsByDateRange(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime stopDate)
       {
           try
           {
               List<StoreTransaction> transactionEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   transactionEntityList = _repository.GetWithPaging(m => m.TransactionDate >= startDate && m.TransactionDate <= stopDate, m => m.StoreTransactionId, tpageNumber, tsize, "StorePaymentMethod, StoreTransactionType").ToList();
               }

               else
               {
                   transactionEntityList = _repository.GetAll(m => m.TransactionDate >= startDate && m.TransactionDate <= stopDate, "StorePaymentMethod, StoreTransactionType").ToList();
               }

               if (!transactionEntityList.Any())
               {
                   return new List<StoreTransactionObject>();
               }
               var transactionObjList = new List<StoreTransactionObject>();
               transactionEntityList.ForEach(m =>
               {
                   var transactionObject = ModelCrossMapper.Map<StoreTransaction, StoreTransactionObject>(m);
                   if (transactionObject != null && transactionObject.StoreTransactionId > 0)
                   {
                       transactionObject.StoreTransactionTypeName = m.StoreTransactionType.Name;
                       transactionObject.PaymentMethodName = m.StorePaymentMethod.Name;
                       transactionObjList.Add(transactionObject);
                   }
               });

               return transactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreTransactionObject>();
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

       public List<StoreTransactionObject> GetStoreTransactions()
       {
           try
           {
               var transactionEntityList = _repository.GetAll().ToList();
               if (!transactionEntityList.Any())
               {
                   return new List<StoreTransactionObject>();
               }
               var transactionObjList = new List<StoreTransactionObject>();
               transactionEntityList.ForEach(m =>
               {
                   var transactionObject = ModelCrossMapper.Map<StoreTransaction, StoreTransactionObject>(m);
                   if (transactionObject != null && transactionObject.StoreTransactionId > 0)
                   {
                       transactionObjList.Add(transactionObject);
                   }
               });
               return transactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
    }
}
