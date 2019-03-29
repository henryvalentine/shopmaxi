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
    public class StoreItemVariationRepository
    {
       private readonly IShopkeeperRepository<StoreItemVariation> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreItemVariationRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreItemVariation>(_uoWork);
		}
       
        public long AddStoreItemVariation(StoreItemVariationObject productVariation)
        {
            try
            {
                if (productVariation == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.VariationProperty.Trim().ToLower() == productVariation.VariationProperty.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var productVariationEntity = ModelCrossMapper.Map<StoreItemVariationObject, StoreItemVariation>(productVariation);
                if (productVariationEntity == null || string.IsNullOrEmpty(productVariationEntity.VariationProperty))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(productVariationEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemVariationId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreItemVariation(StoreItemVariationObject productVariation)
        {
            try
            {
                if (productVariation == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.VariationProperty.Trim().ToLower() == productVariation.VariationProperty.Trim().ToLower() && (m.StoreItemVariationId != productVariation.StoreItemVariationId)) > 0)
                {
                    return -3;
                }
                
                var productVariationEntity = ModelCrossMapper.Map<StoreItemVariationObject, StoreItemVariation>(productVariation);
                if (productVariationEntity == null || productVariationEntity.StoreItemVariationId < 1)
                {
                    return -2;
                }
                _repository.Update(productVariationEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemVariation(long productVariationId)
        {
            try
            {
                var returnStatus = _repository.Remove(productVariationId);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemVariationId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemVariationObject GetStoreItemVariation(long productVariationId)
        {
            try
            {
                var myItem = _repository.GetById(productVariationId);
                if (myItem == null || myItem.StoreItemVariationId < 1)
                {
                    return new StoreItemVariationObject();
                }
                var productVariationObject = ModelCrossMapper.Map<StoreItemVariation, StoreItemVariationObject>(myItem);
                if (productVariationObject == null || productVariationObject.StoreItemVariationId < 1)
                {
                    return new StoreItemVariationObject();
                }
                return productVariationObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemVariationObject();
            }
        }

        public List<StoreItemVariationObject> GetStoreItemVariationObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreItemVariation> productVariationEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        productVariationEntityList = _repository.GetWithPaging(m => m.StoreItemVariationId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        productVariationEntityList = _repository.GetAll().ToList();
                    }

                    if (!productVariationEntityList.Any())
                    {
                        return new List<StoreItemVariationObject>();
                    }
                    var productVariationObjList = new List<StoreItemVariationObject>();
                    productVariationEntityList.ForEach(m =>
                    {
                        var productVariationObject = ModelCrossMapper.Map<StoreItemVariation, StoreItemVariationObject>(m);
                        if (productVariationObject != null && productVariationObject.StoreItemVariationId > 0)
                        {
                            productVariationObjList.Add(productVariationObject);
                        }
                    });

                return productVariationObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationObject>();
            }
        }

        public List<StoreItemVariationObject> Search(string searchCriteria)
        {
            try
            {
                var productVariationEntityList = _repository.GetAll(m => m.VariationProperty.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!productVariationEntityList.Any())
                {
                    return new List<StoreItemVariationObject>();
                }
                var productVariationObjList = new List<StoreItemVariationObject>();
                productVariationEntityList.ForEach(m =>
                {
                    var productVariationObject = ModelCrossMapper.Map<StoreItemVariation, StoreItemVariationObject>(m);
                    if (productVariationObject != null && productVariationObject.StoreItemVariationId > 0)
                    {
                        productVariationObjList.Add(productVariationObject);
                    }
                });
                return productVariationObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationObject>();
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

        public List<StoreItemVariationObject> GetStoreItemVariations()
        {
            try
            {
                var productVariationEntityList = _repository.GetAll().ToList();
                if (!productVariationEntityList.Any())
                {
                    return new List<StoreItemVariationObject>();
                }
                var productVariationObjList = new List<StoreItemVariationObject>();
                productVariationEntityList.ForEach(m =>
                {
                    var productVariationObject = ModelCrossMapper.Map<StoreItemVariation, StoreItemVariationObject>(m);
                    if (productVariationObject != null && productVariationObject.StoreItemVariationId > 0)
                    {
                        productVariationObjList.Add(productVariationObject);
                    }
                });
                return productVariationObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
