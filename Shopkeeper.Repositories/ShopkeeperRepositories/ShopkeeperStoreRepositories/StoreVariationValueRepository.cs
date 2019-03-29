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
    public class StoreItemVariationValueRepository
    {
       private readonly IShopkeeperRepository<StoreItemVariationValue> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreItemVariationValueRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreItemVariationValue>(_uoWork);
		}
       
        public long AddStoreItemVariationValue(StoreItemVariationValueObject productVariationValue)
        {
            try
            {
                if (productVariationValue == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Value.Trim().ToLower() == productVariationValue.Value.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var productVariationValueEntity = ModelCrossMapper.Map<StoreItemVariationValueObject, StoreItemVariationValue>(productVariationValue);
                if (productVariationValueEntity == null || string.IsNullOrEmpty(productVariationValueEntity.Value))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(productVariationValueEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemVariationValueId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreItemVariationValue(StoreItemVariationValueObject productVariationValue)
        {
            try
            {
                if (productVariationValue == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Value.Trim().ToLower() == productVariationValue.Value.Trim().ToLower() && (m.StoreItemVariationValueId != productVariationValue.StoreItemVariationValueId)) > 0)
                {
                    return -3;
                }
                
                var productVariationValueEntity = ModelCrossMapper.Map<StoreItemVariationValueObject, StoreItemVariationValue>(productVariationValue);
                if (productVariationValueEntity == null || productVariationValueEntity.StoreItemVariationValueId < 1)
                {
                    return -2;
                }
                _repository.Update(productVariationValueEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemVariationValue(long productVariationValueId)
        {
            try
            {
                var returnStatus = _repository.Remove(productVariationValueId);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemVariationValueId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemVariationValueObject GetStoreItemVariationValue(long productVariationValueId)
        {
            try
            {
                var myItem = _repository.GetById(productVariationValueId);
                if (myItem == null || myItem.StoreItemVariationValueId < 1)
                {
                    return new StoreItemVariationValueObject();
                }
                var productVariationValueObject = ModelCrossMapper.Map<StoreItemVariationValue, StoreItemVariationValueObject>(myItem);
                if (productVariationValueObject == null || productVariationValueObject.StoreItemVariationValueId < 1)
                {
                    return new StoreItemVariationValueObject();
                }
                return productVariationValueObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemVariationValueObject();
            }
        }

        public List<StoreItemVariationValueObject> GetStoreItemVariationValueObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreItemVariationValue> productVariationValueEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        productVariationValueEntityList = _repository.GetWithPaging(m => m.StoreItemVariationValueId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        productVariationValueEntityList = _repository.GetAll().ToList();
                    }

                    if (!productVariationValueEntityList.Any())
                    {
                        return new List<StoreItemVariationValueObject>();
                    }
                    var productVariationValueObjList = new List<StoreItemVariationValueObject>();
                    productVariationValueEntityList.ForEach(m =>
                    {
                        var productVariationValueObject = ModelCrossMapper.Map<StoreItemVariationValue, StoreItemVariationValueObject>(m);
                        if (productVariationValueObject != null && productVariationValueObject.StoreItemVariationValueId > 0)
                        {
                            productVariationValueObjList.Add(productVariationValueObject);
                        }
                    });

                return productVariationValueObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationValueObject>();
            }
        }

        public List<StoreItemVariationValueObject> Search(string searchCriteria)
        {
            try
            {
                var productVariationValueEntityList = _repository.GetAll(m => m.Value.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!productVariationValueEntityList.Any())
                {
                    return new List<StoreItemVariationValueObject>();
                }
                var productVariationValueObjList = new List<StoreItemVariationValueObject>();
                productVariationValueEntityList.ForEach(m =>
                {
                    var productVariationValueObject = ModelCrossMapper.Map<StoreItemVariationValue, StoreItemVariationValueObject>(m);
                    if (productVariationValueObject != null && productVariationValueObject.StoreItemVariationValueId > 0)
                    {
                        productVariationValueObjList.Add(productVariationValueObject);
                    }
                });
                return productVariationValueObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationValueObject>();
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

        public List<StoreItemVariationValueObject> GetStoreItemVariationValues()
        {
            try
            {
                var productVariationValueEntityList = _repository.GetAll().ToList();
                if (!productVariationValueEntityList.Any())
                {
                    return new List<StoreItemVariationValueObject>();
                }
                var productVariationValueObjList = new List<StoreItemVariationValueObject>();
                productVariationValueEntityList.ForEach(m =>
                {
                    var productVariationValueObject = ModelCrossMapper.Map<StoreItemVariationValue, StoreItemVariationValueObject>(m);
                    if (productVariationValueObject != null && productVariationValueObject.StoreItemVariationValueId > 0)
                    {
                        productVariationValueObjList.Add(productVariationValueObject);
                    }
                });
                return productVariationValueObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
