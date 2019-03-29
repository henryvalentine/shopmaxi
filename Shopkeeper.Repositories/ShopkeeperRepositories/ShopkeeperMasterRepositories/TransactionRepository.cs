using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class TransactionRepository
    {
       private readonly IShopkeeperRepository<Transaction> _repository;
       private readonly UnitOfWork _uoWork;

       public TransactionRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<Transaction>(_uoWork);
		}
       
        public long AddTransaction(TransactionObject transaction)
        {
            try
            {
                if (transaction == null)
                {
                    return -2;
                }
               
                var transactionEntity = ModelCrossMapper.Map<TransactionObject, Transaction>(transaction);
                if (transactionEntity == null || transactionEntity.TransactionTypeId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(transactionEntity);
                _uoWork.SaveChanges();
                return returnStatus.TransactionId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateTransaction(TransactionObject transaction)
        {
            try
            {
                if (transaction == null)
                {
                    return -2;
                }
                var transactionEntity = ModelCrossMapper.Map<TransactionObject, Transaction>(transaction);
                if (transactionEntity == null || transactionEntity.TransactionId < 1)
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

        public bool DeleteTransaction(long transactionId)
        {
            try
            {
                var returnStatus = _repository.Remove(transactionId);
                _uoWork.SaveChanges();
                return returnStatus.TransactionId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public TransactionObject GetTransaction(long transactionId)
        {
            try
            {
                var myItem = _repository.Get(m => m.TransactionId == transactionId, "PaymentMethod, TransactionType");
                if (myItem == null || myItem.TransactionId < 1)
                {
                    return new TransactionObject();
                }
                var transactionObject = ModelCrossMapper.Map<Transaction, TransactionObject>(myItem);
                if (transactionObject == null || transactionObject.TransactionId < 1)
                {
                    return new TransactionObject();
                }
                transactionObject.TransactionTypeName = myItem.TransactionType.Name;
                transactionObject.PaymentMethodName = myItem.PaymentMethod.Name;
                return transactionObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new TransactionObject();
            }
        }

        public List<TransactionObject> GetTransactionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<Transaction> transactionEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        transactionEntityList = _repository.GetWithPaging(m => m.TransactionId, tpageNumber, tsize, "PaymentMethod, TransactionType").ToList();
                    }

                    else
                    {
                        transactionEntityList = _repository.GetAll("PaymentMethod, TransactionType").ToList();
                    }

                    if (!transactionEntityList.Any())
                    {
                        return new List<TransactionObject>();
                    }
                    var transactionObjList = new List<TransactionObject>();
                    transactionEntityList.ForEach(m =>
                    {
                        var transactionObject = ModelCrossMapper.Map<Transaction, TransactionObject>(m);
                        if (transactionObject != null && transactionObject.TransactionId > 0)
                        {
                            transactionObject.TransactionTypeName = m.TransactionType.Name;
                            transactionObject.PaymentMethodName = m.PaymentMethod.Name;
                            transactionObjList.Add(transactionObject);
                        }
                    });

                return transactionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }

        public List<TransactionObject> GetTransactionsByType(int? itemsPerPage, int? pageNumber, int transactionTypeId)
        {
            try
            {
                List<Transaction> transactionEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    transactionEntityList = _repository.GetWithPaging(m => m.TransactionTypeId == transactionTypeId, m => m.TransactionId, tpageNumber, tsize, "PaymentMethod, TransactionType").ToList();
                }

                else
                {
                    transactionEntityList = _repository.GetAll(m => m.TransactionTypeId == transactionTypeId, "PaymentMethod, TransactionType").ToList();
                }

                if (!transactionEntityList.Any())
                {
                    return new List<TransactionObject>();
                }
                var transactionObjList = new List<TransactionObject>();
                transactionEntityList.ForEach(m =>
                {
                    var transactionObject = ModelCrossMapper.Map<Transaction, TransactionObject>(m);
                    if (transactionObject != null && transactionObject.TransactionId > 0)
                    {
                        transactionObject.TransactionTypeName = m.TransactionType.Name;
                        transactionObject.PaymentMethodName = m.PaymentMethod.Name;
                        transactionObjList.Add(transactionObject);
                    }
                });

                return transactionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }

        public List<TransactionObject> GetTransactionsByPaymentMethod(int? itemsPerPage, int? pageNumber, int paymentMethodId)
        {
            try
            {
                List<Transaction> transactionEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    transactionEntityList = _repository.GetWithPaging(m => m.PaymentMethodId == paymentMethodId, m => m.TransactionId, tpageNumber, tsize, "PaymentMethod, TransactionType").ToList();
                }

                else
                {
                    transactionEntityList = _repository.GetAll(m => m.PaymentMethodId == paymentMethodId, "PaymentMethod, TransactionType").ToList();
                }

                if (!transactionEntityList.Any())
                {
                    return new List<TransactionObject>();
                }
                var transactionObjList = new List<TransactionObject>();
                transactionEntityList.ForEach(m =>
                {
                    var transactionObject = ModelCrossMapper.Map<Transaction, TransactionObject>(m);
                    if (transactionObject != null && transactionObject.TransactionId > 0)
                    {
                        transactionObject.TransactionTypeName = m.TransactionType.Name;
                        transactionObject.PaymentMethodName = m.PaymentMethod.Name;
                        transactionObjList.Add(transactionObject);
                    }
                });

                return transactionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }

        public List<TransactionObject> GetTransactionsByDateRange(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime stopDate)
        {
            try
            {
                List<Transaction> transactionEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    transactionEntityList = _repository.GetWithPaging(m => m.TransactionDate >= startDate && m.TransactionDate <= stopDate, m => m.TransactionId, tpageNumber, tsize, "PaymentMethod, TransactionType").ToList();
                }

                else
                {
                    transactionEntityList = _repository.GetAll(m => m.TransactionDate >= startDate && m.TransactionDate <= stopDate,"PaymentMethod, TransactionType").ToList();
                }

                if (!transactionEntityList.Any())
                {
                    return new List<TransactionObject>();
                }
                var transactionObjList = new List<TransactionObject>();
                transactionEntityList.ForEach(m =>
                {
                    var transactionObject = ModelCrossMapper.Map<Transaction, TransactionObject>(m);
                    if (transactionObject != null && transactionObject.TransactionId > 0)
                    {
                        transactionObject.TransactionTypeName = m.TransactionType.Name;
                        transactionObject.PaymentMethodName = m.PaymentMethod.Name;
                        transactionObjList.Add(transactionObject);
                    }
                });

                return transactionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
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

        public List<TransactionObject> GetTransactions()
        {
            try
            {
                var transactionEntityList = _repository.GetAll().ToList();
                if (!transactionEntityList.Any())
                {
                    return new List<TransactionObject>();
                }
                var transactionObjList = new List<TransactionObject>();
                transactionEntityList.ForEach(m =>
                {
                    var transactionObject = ModelCrossMapper.Map<Transaction, TransactionObject>(m);
                    if (transactionObject != null && transactionObject.TransactionId > 0)
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
