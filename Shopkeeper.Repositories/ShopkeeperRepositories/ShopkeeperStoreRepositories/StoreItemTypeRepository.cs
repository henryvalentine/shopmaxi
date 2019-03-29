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
    public class StoreItemTypeRepository
    {
       private readonly IShopkeeperRepository<StoreItemType> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreItemTypeRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreItemType>(_uoWork);
		}
       
        public long AddStoreItemType(StoreItemTypeObject productType)
        {
            try
            {
                if (productType == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == productType.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var productTypeEntity = ModelCrossMapper.Map<StoreItemTypeObject, StoreItemType>(productType);
                if (productTypeEntity == null || string.IsNullOrEmpty(productTypeEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(productTypeEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemTypeId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreItemType(StoreItemTypeObject productType)
        {
            try
            {
                if (productType == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == productType.Name.Trim().ToLower() && (m.StoreItemTypeId != productType.StoreItemTypeId)) > 0)
                {
                    return -3;
                }
                
                var productTypeEntity = ModelCrossMapper.Map<StoreItemTypeObject, StoreItemType>(productType);
                if (productTypeEntity == null || productTypeEntity.StoreItemTypeId < 1)
                {
                    return -2;
                }
                _repository.Update(productTypeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemType(long productTypeId)
        {
            try
            {
                var returnStatus = _repository.Remove(productTypeId);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemTypeId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemTypeObject GetStoreItemType(long productTypeId)
        {
            try
            {
                var myItem = _repository.GetById(productTypeId);
                if (myItem == null || myItem.StoreItemTypeId < 1)
                {
                    return new StoreItemTypeObject();
                }
                var productTypeObject = ModelCrossMapper.Map<StoreItemType, StoreItemTypeObject>(myItem);
                if (productTypeObject == null || productTypeObject.StoreItemTypeId < 1)
                {
                    return new StoreItemTypeObject();
                }
                return productTypeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemTypeObject();
            }
        }

        public List<StoreItemTypeObject> GetStoreItemTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreItemType> productTypeEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        productTypeEntityList = _repository.GetWithPaging(m => m.StoreItemTypeId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        productTypeEntityList = _repository.GetAll().ToList();
                    }

                    if (!productTypeEntityList.Any())
                    {
                        return new List<StoreItemTypeObject>();
                    }
                    var productTypeObjList = new List<StoreItemTypeObject>();
                    productTypeEntityList.ForEach(m =>
                    {
                        var productTypeObject = ModelCrossMapper.Map<StoreItemType, StoreItemTypeObject>(m);
                        if (productTypeObject != null && productTypeObject.StoreItemTypeId > 0)
                        {
                            productTypeObjList.Add(productTypeObject);
                        }
                    });

                return productTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemTypeObject>();
            }
        }

        public List<StoreItemTypeObject> Search(string searchCriteria)
        {
            try
            {
                var productTypeEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!productTypeEntityList.Any())
                {
                    return new List<StoreItemTypeObject>();
                }
                var productTypeObjList = new List<StoreItemTypeObject>();
                productTypeEntityList.ForEach(m =>
                {
                    var productTypeObject = ModelCrossMapper.Map<StoreItemType, StoreItemTypeObject>(m);
                    if (productTypeObject != null && productTypeObject.StoreItemTypeId > 0)
                    {
                        productTypeObjList.Add(productTypeObject);
                    }
                });
                return productTypeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemTypeObject>();
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

        public List<StoreItemTypeObject> GetStoreItemTypes()
        {
            try
            {
                var productTypeEntityList = _repository.GetAll().ToList();
                if (!productTypeEntityList.Any())
                {
                    return new List<StoreItemTypeObject>();
                }
                var productTypeObjList = new List<StoreItemTypeObject>();
                productTypeEntityList.ForEach(m =>
                {
                    var productTypeObject = ModelCrossMapper.Map<StoreItemType, StoreItemTypeObject>(m);
                    if (productTypeObject != null && productTypeObject.StoreItemTypeId > 0)
                    {
                        productTypeObjList.Add(productTypeObject);
                    }
                });
                return productTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
