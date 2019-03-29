using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class StoreTransactionTypeRepository
    {
       private readonly IShopkeeperRepository<StoreTransactionType> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreTransactionTypeRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreTransactionType>(_uoWork);
		}
       
        public long AddStoreTransactionType(StoreTransactionTypeObject transactionType)
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
                var transactionTypeEntity = ModelCrossMapper.Map<StoreTransactionTypeObject, StoreTransactionType>(transactionType);
                if (transactionTypeEntity == null || string.IsNullOrEmpty(transactionTypeEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(transactionTypeEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreTransactionTypeId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreTransactionType(StoreTransactionTypeObject transactionType)
        {
            try
            {
                if (transactionType == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == transactionType.Name.Trim().ToLower() && (m.StoreTransactionTypeId != transactionType.StoreTransactionTypeId)) > 0)
                {
                    return -3;
                }
                
                var transactionTypeEntity = ModelCrossMapper.Map<StoreTransactionTypeObject, StoreTransactionType>(transactionType);
                if (transactionTypeEntity == null || transactionTypeEntity.StoreTransactionTypeId < 1)
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

        public bool DeleteStoreTransactionType(long transactionTypeId)
        {
            try
            {
                var returnStatus = _repository.Remove(transactionTypeId);
                _uoWork.SaveChanges();
                return returnStatus.StoreTransactionTypeId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreTransactionTypeObject GetStoreTransactionType(long transactionTypeId)
        {
            try
            {
                var myItem = _repository.GetById(transactionTypeId);
                if (myItem == null || myItem.StoreTransactionTypeId < 1)
                {
                    return new StoreTransactionTypeObject();
                }
                var transactionTypeObject = ModelCrossMapper.Map<StoreTransactionType, StoreTransactionTypeObject>(myItem);
                if (transactionTypeObject == null || transactionTypeObject.StoreTransactionTypeId < 1)
                {
                    return new StoreTransactionTypeObject();
                }
                return transactionTypeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreTransactionTypeObject();
            }
        }

        public List<StoreTransactionTypeObject> GetStoreTransactionTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreTransactionType> transactionTypeEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        transactionTypeEntityList = _repository.GetWithPaging(m => m.StoreTransactionTypeId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        transactionTypeEntityList = _repository.GetAll().ToList();
                    }

                    if (!transactionTypeEntityList.Any())
                    {
                        return new List<StoreTransactionTypeObject>();
                    }
                    var transactionTypeObjList = new List<StoreTransactionTypeObject>();
                    transactionTypeEntityList.ForEach(m =>
                    {
                        var transactionTypeObject = ModelCrossMapper.Map<StoreTransactionType, StoreTransactionTypeObject>(m);
                        if (transactionTypeObject != null && transactionTypeObject.StoreTransactionTypeId > 0)
                        {
                            transactionTypeObjList.Add(transactionTypeObject);
                        }
                    });

                return transactionTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionTypeObject>();
            }
        }

        public List<StoreTransactionTypeObject> GetTransactionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreTransactionType> transactionTypeEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    transactionTypeEntityList = _repository.GetWithPaging(m => m.StoreTransactionTypeId, tpageNumber, tsize).ToList();
                }

                else
                {
                    transactionTypeEntityList = _repository.GetAll().ToList();
                }

                if (!transactionTypeEntityList.Any())
                {
                    return new List<StoreTransactionTypeObject>();
                }
                var transactionTypeObjList = new List<StoreTransactionTypeObject>();
                transactionTypeEntityList.ForEach(m =>
                {
                    var transactionTypeObject = ModelCrossMapper.Map<StoreTransactionType, StoreTransactionTypeObject>(m);
                    if (transactionTypeObject != null && transactionTypeObject.StoreTransactionTypeId > 0)
                    {
                        transactionTypeObjList.Add(transactionTypeObject);
                    }
                });

                return transactionTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionTypeObject>();
            }
        }

        public List<StoreTransactionTypeObject> Search(string searchCriteria)
        {
            try
            {
                var transactionTypeEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!transactionTypeEntityList.Any())
                {
                    return new List<StoreTransactionTypeObject>();
                }
                var transactionTypeObjList = new List<StoreTransactionTypeObject>();
                transactionTypeEntityList.ForEach(m =>
                {
                    var transactionTypeObject = ModelCrossMapper.Map<StoreTransactionType, StoreTransactionTypeObject>(m);
                    if (transactionTypeObject != null && transactionTypeObject.StoreTransactionTypeId > 0)
                    {
                        transactionTypeObjList.Add(transactionTypeObject);
                    }
                });
                return transactionTypeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionTypeObject>();
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

        public List<StoreTransactionTypeObject> GetStoreTransactionTypes()
        {
            try
            {
                var transactionTypeEntityList = _repository.GetAll().ToList();
                if (!transactionTypeEntityList.Any())
                {
                    return new List<StoreTransactionTypeObject>();
                }
                var transactionTypeObjList = new List<StoreTransactionTypeObject>();
                transactionTypeEntityList.ForEach(m =>
                {
                    var transactionTypeObject = ModelCrossMapper.Map<StoreTransactionType, StoreTransactionTypeObject>(m);
                    if (transactionTypeObject != null && transactionTypeObject.StoreTransactionTypeId > 0)
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
