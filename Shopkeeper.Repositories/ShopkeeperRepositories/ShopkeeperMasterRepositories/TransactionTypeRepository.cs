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
    public class TransactionTypeRepository
    {
       private readonly IShopkeeperRepository<TransactionType> _repository;
       private readonly UnitOfWork _uoWork;

       public TransactionTypeRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<TransactionType>(_uoWork);
		}
       
        public long AddTransactionType(TransactionTypeObject transactionType)
        {
            try
            {
                if (transactionType == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == transactionType.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var transactionTypeEntity = ModelCrossMapper.Map<TransactionTypeObject, TransactionType>(transactionType);
                if (transactionTypeEntity == null || string.IsNullOrEmpty(transactionTypeEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(transactionTypeEntity);
                _uoWork.SaveChanges();
                return returnStatus.TransactionTypeId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateTransactionType(TransactionTypeObject transactionType)
        {
            try
            {
                if (transactionType == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == transactionType.Name.Trim().ToLower() && (m.TransactionTypeId != transactionType.TransactionTypeId)) > 0)
                {
                    return -3;
                }
                
                var transactionTypeEntity = ModelCrossMapper.Map<TransactionTypeObject, TransactionType>(transactionType);
                if (transactionTypeEntity == null || transactionTypeEntity.TransactionTypeId < 1)
                {
                    return -2;
                }
                _repository.Update(transactionTypeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteTransactionType(long transactionTypeId)
        {
            try
            {
                var returnStatus = _repository.Remove(transactionTypeId);
                _uoWork.SaveChanges();
                return returnStatus.TransactionTypeId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public TransactionTypeObject GetTransactionType(long transactionTypeId)
        {
            try
            {
                var myItem = _repository.GetById(transactionTypeId);
                if (myItem == null || myItem.TransactionTypeId < 1)
                {
                    return new TransactionTypeObject();
                }
                var transactionTypeObject = ModelCrossMapper.Map<TransactionType, TransactionTypeObject>(myItem);
                if (transactionTypeObject == null || transactionTypeObject.TransactionTypeId < 1)
                {
                    return new TransactionTypeObject();
                }
                return transactionTypeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new TransactionTypeObject();
            }
        }

        public List<TransactionTypeObject> GetTransactionTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<TransactionType> transactionTypeEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        transactionTypeEntityList = _repository.GetWithPaging(m => m.TransactionTypeId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        transactionTypeEntityList = _repository.GetAll().ToList();
                    }

                    if (!transactionTypeEntityList.Any())
                    {
                        return new List<TransactionTypeObject>();
                    }
                    var transactionTypeObjList = new List<TransactionTypeObject>();
                    transactionTypeEntityList.ForEach(m =>
                    {
                        var transactionTypeObject = ModelCrossMapper.Map<TransactionType, TransactionTypeObject>(m);
                        if (transactionTypeObject != null && transactionTypeObject.TransactionTypeId > 0)
                        {
                            transactionTypeObjList.Add(transactionTypeObject);
                        }
                    });

                return transactionTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionTypeObject>();
            }
        }

        public List<TransactionTypeObject> GetTransactionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<TransactionType> transactionTypeEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    transactionTypeEntityList = _repository.GetWithPaging(m => m.TransactionTypeId, tpageNumber, tsize).ToList();
                }

                else
                {
                    transactionTypeEntityList = _repository.GetAll().ToList();
                }

                if (!transactionTypeEntityList.Any())
                {
                    return new List<TransactionTypeObject>();
                }
                var transactionTypeObjList = new List<TransactionTypeObject>();
                transactionTypeEntityList.ForEach(m =>
                {
                    var transactionTypeObject = ModelCrossMapper.Map<TransactionType, TransactionTypeObject>(m);
                    if (transactionTypeObject != null && transactionTypeObject.TransactionTypeId > 0)
                    {
                        transactionTypeObjList.Add(transactionTypeObject);
                    }
                });

                return transactionTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionTypeObject>();
            }
        }

        public List<TransactionTypeObject> Search(string searchCriteria)
        {
            try
            {
                var transactionTypeEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!transactionTypeEntityList.Any())
                {
                    return new List<TransactionTypeObject>();
                }
                var transactionTypeObjList = new List<TransactionTypeObject>();
                transactionTypeEntityList.ForEach(m =>
                {
                    var transactionTypeObject = ModelCrossMapper.Map<TransactionType, TransactionTypeObject>(m);
                    if (transactionTypeObject != null && transactionTypeObject.TransactionTypeId > 0)
                    {
                        transactionTypeObjList.Add(transactionTypeObject);
                    }
                });
                return transactionTypeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionTypeObject>();
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

        public List<TransactionTypeObject> GetTransactionTypes()
        {
            try
            {
                var transactionTypeEntityList = _repository.GetAll().ToList();
                if (!transactionTypeEntityList.Any())
                {
                    return new List<TransactionTypeObject>();
                }
                var transactionTypeObjList = new List<TransactionTypeObject>();
                transactionTypeEntityList.ForEach(m =>
                {
                    var transactionTypeObject = ModelCrossMapper.Map<TransactionType, TransactionTypeObject>(m);
                    if (transactionTypeObject != null && transactionTypeObject.TransactionTypeId > 0)
                    {
                        transactionTypeObjList.Add(transactionTypeObject);
                    }
                });
                return transactionTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
