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
    public class StoreCustomerTypeRepository
    {
        private readonly IShopkeeperRepository<StoreCustomerType> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreCustomerTypeRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString); 
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreCustomerType>(_uoWork);
		}
       
        public long AddStoreCustomerType(StoreCustomerTypeObject storeCustomerType)
        {
            try
            {
                if (storeCustomerType == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == storeCustomerType.Name.Trim().ToLower() && (m.StoreCustomerTypeId != storeCustomerType.StoreCustomerTypeId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeCustomerTypeEntity = ModelCrossMapper.Map<StoreCustomerTypeObject, StoreCustomerType>(storeCustomerType);
                if (storeCustomerTypeEntity == null || string.IsNullOrEmpty(storeCustomerTypeEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeCustomerTypeEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreCustomerTypeId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreCustomerType(StoreCustomerTypeObject storeCustomerType)
        {
            try
            {
                if (storeCustomerType == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == storeCustomerType.Name.Trim().ToLower() && (m.StoreCustomerTypeId != storeCustomerType.StoreCustomerTypeId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeCustomerTypeEntity = ModelCrossMapper.Map<StoreCustomerTypeObject, StoreCustomerType>(storeCustomerType);
                if (storeCustomerTypeEntity == null || storeCustomerTypeEntity.StoreCustomerTypeId < 1)
                {
                    return -2;
                }
                _repository.Update(storeCustomerTypeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreCustomerType(long storeCustomerTypeId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeCustomerTypeId);
                _uoWork.SaveChanges();
                return returnStatus.StoreCustomerTypeId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreCustomerTypeObject GetStoreCustomerType(long storeCustomerTypeId)
        {
            try
            {
                var myItem = _repository.GetById(storeCustomerTypeId);
                if (myItem == null || myItem.StoreCustomerTypeId < 1)
                {
                    return new StoreCustomerTypeObject();
                }
                var storeCustomerTypeObject = ModelCrossMapper.Map<StoreCustomerType, StoreCustomerTypeObject>(myItem);
                if (storeCustomerTypeObject == null || storeCustomerTypeObject.StoreCustomerTypeId < 1)
                {
                    return new StoreCustomerTypeObject();
                }
                return storeCustomerTypeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCustomerTypeObject();
            }
        }

        public List<StoreCustomerTypeObject> GetStoreCustomerTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreCustomerType> storeCustomerTypeEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeCustomerTypeEntityList = _repository.GetWithPaging(m => m.StoreCustomerTypeId, tpageNumber, tsize).ToList();
                }

                else
                {
                    storeCustomerTypeEntityList = _repository.GetAll().ToList();
                }

                if (!storeCustomerTypeEntityList.Any())
                {
                    return new List<StoreCustomerTypeObject>();
                }
                var storeCustomerTypeObjList = new List<StoreCustomerTypeObject>();
                storeCustomerTypeEntityList.ForEach(m =>
                {
                    var storeCustomerTypeObject = ModelCrossMapper.Map<StoreCustomerType, StoreCustomerTypeObject>(m);
                    if (storeCustomerTypeObject != null && storeCustomerTypeObject.StoreCustomerTypeId > 0)
                    {
                        storeCustomerTypeObjList.Add(storeCustomerTypeObject);
                    }
                });

                return storeCustomerTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCustomerTypeObject>();
            }
        }

        public List<StoreCustomerTypeObject> Search(string searchCriteria)
        {
            try
            {
               var storeCustomerTypeEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storeCustomerTypeEntityList.Any())
                {
                    return new List<StoreCustomerTypeObject>();
                }
                var storeCustomerTypeObjList = new List<StoreCustomerTypeObject>();
                storeCustomerTypeEntityList.ForEach(m =>
                {
                    var storeCustomerTypeObject = ModelCrossMapper.Map<StoreCustomerType, StoreCustomerTypeObject>(m);
                    if (storeCustomerTypeObject != null && storeCustomerTypeObject.StoreCustomerTypeId > 0)
                    {
                        storeCustomerTypeObjList.Add(storeCustomerTypeObject);
                    }
                });
                return storeCustomerTypeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCustomerTypeObject>();
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

        public int GetObjectCount(Expression<Func<StoreCustomerType, bool>> predicate)
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

        public List<StoreCustomerTypeObject> GetStoreCustomerTypes()
        {
            try
            {
                var storeCustomerTypeEntityList = _repository.GetAll().ToList();
                if (!storeCustomerTypeEntityList.Any())
                {
                    return new List<StoreCustomerTypeObject>();
                }
                var storeCustomerTypeObjList = new List<StoreCustomerTypeObject>();
                storeCustomerTypeEntityList.ForEach(m =>
                {
                    var storeCustomerTypeObject = ModelCrossMapper.Map<StoreCustomerType, StoreCustomerTypeObject>(m);
                    if (storeCustomerTypeObject != null && storeCustomerTypeObject.StoreCustomerTypeId > 0)
                    {
                        storeCustomerTypeObjList.Add(storeCustomerTypeObject);
                    }
                });
                return storeCustomerTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
