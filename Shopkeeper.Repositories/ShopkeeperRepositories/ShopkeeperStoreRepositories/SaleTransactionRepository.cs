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
    public class SaleTransactionRepository
    {
        private readonly IShopkeeperRepository<SaleTransaction> _repository;
       private readonly UnitOfWork _uoWork;
       private ShopKeeperStoreEntities _db = new ShopKeeperStoreEntities(); 
       public SaleTransactionRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<SaleTransaction>(_uoWork);
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
               var returnStatus = _repository.Add(saleTransactionEntity);
               _uoWork.SaveChanges();
               return returnStatus.SaleTransactionId;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public int UpdateSaleTransaction(SaleTransactionObject saleTransaction)
       {
           try
           {
               if (saleTransaction == null)
               {
                   return -2;
               }

               var saleTransactionEntity = ModelCrossMapper.Map<SaleTransactionObject, SaleTransaction>(saleTransaction);
               if (saleTransactionEntity == null || saleTransactionEntity.SaleTransactionId < 1)
               {
                   return -2;
               }
               _repository.Update(saleTransactionEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteSaleTransaction(long saleTransactionId)
       {
           try
           {
               var returnStatus = _repository.Remove(saleTransactionId);
               _uoWork.SaveChanges();
               return returnStatus.SaleTransactionId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public bool DeleteSaleTransactionByTransaction(long transactionId)
       {
           try
           {
               using (var db = _db)
               {
                   var r = db.SaleTransactions.Where(m => m.SaleTransactionId == transactionId).ToList();
                   if (r.Any())
                   {
                       db.SaleTransactions.Remove(r[0]);
                       db.SaveChanges();
                       return true;
                   }
                   return false;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public SaleTransactionObject GetSaleTransaction(long saleTransactionId)
       {
           try
           {
               var myItem = _repository.GetById(saleTransactionId);
               if (myItem == null || myItem.SaleTransactionId < 1)
               {
                   return new SaleTransactionObject();
               }
               var saleTransactionObject = ModelCrossMapper.Map<SaleTransaction, SaleTransactionObject>(myItem);
               if (saleTransactionObject == null || saleTransactionObject.SaleTransactionId < 1)
               {
                   return new SaleTransactionObject();
               }
               return saleTransactionObject;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new SaleTransactionObject();
           }
       }

       public List<SaleTransactionObject> GetSaleTransactionObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               List<SaleTransaction> saleTransactionEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   saleTransactionEntityList = _repository.GetWithPaging(m => m.SaleTransactionId, tpageNumber, tsize).ToList();
               }

               else
               {
                   saleTransactionEntityList = _repository.GetAll().ToList();
               }

               if (!saleTransactionEntityList.Any())
               {
                   return new List<SaleTransactionObject>();
               }
               var saleTransactionObjList = new List<SaleTransactionObject>();
               saleTransactionEntityList.ForEach(m =>
               {
                   var saleTransactionObject = ModelCrossMapper.Map<SaleTransaction, SaleTransactionObject>(m);
                   if (saleTransactionObject != null && saleTransactionObject.SaleTransactionId > 0)
                   {
                       saleTransactionObjList.Add(saleTransactionObject);
                   }
               });

               return saleTransactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<SaleTransactionObject>();
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

       public int GetObjectCount(Expression<Func<SaleTransaction, bool>> predicate)
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

       public List<SaleTransactionObject> GetSaleTransactions()
       {
           try
           {
               var saleTransactionEntityList = _repository.GetAll().ToList();
               if (!saleTransactionEntityList.Any())
               {
                   return new List<SaleTransactionObject>();
               }
               var saleTransactionObjList = new List<SaleTransactionObject>();
               saleTransactionEntityList.ForEach(m =>
               {
                   var saleTransactionObject = ModelCrossMapper.Map<SaleTransaction, SaleTransactionObject>(m);
                   if (saleTransactionObject != null && saleTransactionObject.SaleTransactionId > 0)
                   {
                       saleTransactionObjList.Add(saleTransactionObject);
                   }
               });
               return saleTransactionObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
    }
}
