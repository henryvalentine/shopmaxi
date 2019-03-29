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
    public class StoreItemBrandRepository
    {
       private readonly IShopkeeperRepository<StoreItemBrand> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreItemBrandRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreItemBrand>(_uoWork);
		}
       
        public long AddStoreItemBrand(StoreItemBrandObject storeItemBrand)
        {
            try
            {
                if (storeItemBrand == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == storeItemBrand.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var storeItemBrandEntity = ModelCrossMapper.Map<StoreItemBrandObject, StoreItemBrand>(storeItemBrand);
                if (storeItemBrandEntity == null || string.IsNullOrEmpty(storeItemBrandEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeItemBrandEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemBrandId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreItemBrand(StoreItemBrandObject storeItemBrand)
        {
            try
            {
                if (storeItemBrand == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == storeItemBrand.Name.Trim().ToLower() && (m.StoreItemBrandId != storeItemBrand.StoreItemBrandId)) > 0)
                {
                    return -3;
                }
                
                var storeItemBrandEntity = ModelCrossMapper.Map<StoreItemBrandObject, StoreItemBrand>(storeItemBrand);
                if (storeItemBrandEntity == null || storeItemBrandEntity.StoreItemBrandId < 1)
                {
                    return -2;
                }
                _repository.Update(storeItemBrandEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemBrand(long storeItemBrandId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeItemBrandId);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemBrandId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemBrandObject GetStoreItemBrand(long storeItemBrandId)
        {
            try
            {
                var myItem = _repository.GetById(storeItemBrandId);
                if (myItem == null || myItem.StoreItemBrandId < 1)
                {
                    return new StoreItemBrandObject();
                }
                var storeItemBrandObject = ModelCrossMapper.Map<StoreItemBrand, StoreItemBrandObject>(myItem);
                if (storeItemBrandObject == null || storeItemBrandObject.StoreItemBrandId < 1)
                {
                    return new StoreItemBrandObject();
                }
                return storeItemBrandObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemBrandObject();
            }
        }

        public List<StoreItemBrandObject> GetStoreItemBrandObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreItemBrand> storeItemBrandEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeItemBrandEntityList = _repository.GetWithPaging(m => m.StoreItemBrandId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        storeItemBrandEntityList = _repository.GetAll().ToList();
                    }

                    if (!storeItemBrandEntityList.Any())
                    {
                        return new List<StoreItemBrandObject>();
                    }
                    var storeItemBrandObjList = new List<StoreItemBrandObject>();
                    storeItemBrandEntityList.ForEach(m =>
                    {
                        var storeItemBrandObject = ModelCrossMapper.Map<StoreItemBrand, StoreItemBrandObject>(m);
                        if (storeItemBrandObject != null && storeItemBrandObject.StoreItemBrandId > 0)
                        {
                            storeItemBrandObjList.Add(storeItemBrandObject);
                        }
                    });

                return storeItemBrandObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemBrandObject>();
            }
        }

        public List<StoreItemBrandObject> Search(string searchCriteria)
        {
            try
            {
                var storeItemBrandEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storeItemBrandEntityList.Any())
                {
                    return new List<StoreItemBrandObject>();
                }
                var storeItemBrandObjList = new List<StoreItemBrandObject>();
                storeItemBrandEntityList.ForEach(m =>
                {
                    var storeItemBrandObject = ModelCrossMapper.Map<StoreItemBrand, StoreItemBrandObject>(m);
                    if (storeItemBrandObject != null && storeItemBrandObject.StoreItemBrandId > 0)
                    {
                        storeItemBrandObjList.Add(storeItemBrandObject);
                    }
                });
                return storeItemBrandObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemBrandObject>();
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

        public List<StoreItemBrandObject> GetStoreItemBrands()
        {
            try
            {
                var storeItemBrandEntityList = _repository.GetAll().ToList();
                if (!storeItemBrandEntityList.Any())
                {
                    return new List<StoreItemBrandObject>();
                }
                var storeItemBrandObjList = new List<StoreItemBrandObject>();
                storeItemBrandEntityList.ForEach(m =>
                {
                    var storeItemBrandObject = ModelCrossMapper.Map<StoreItemBrand, StoreItemBrandObject>(m);
                    if (storeItemBrandObject != null && storeItemBrandObject.StoreItemBrandId > 0)
                    {
                        storeItemBrandObjList.Add(storeItemBrandObject);
                    }
                });
                return storeItemBrandObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
