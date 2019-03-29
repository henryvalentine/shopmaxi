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
    public class StoreCityRepository
    {
       private readonly IShopkeeperRepository<StoreCity> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreCityRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreCity>(_uoWork);
		}
       
        public long AddStoreCity(StoreCityObject city)
        {
            try
            {
                if (city == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(city.Name.Trim().ToLower()) && city.StoreStateId.Equals(m.StoreStateId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var cityEntity = ModelCrossMapper.Map<StoreCityObject, StoreCity>(city);
                if (cityEntity == null || string.IsNullOrEmpty(cityEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(cityEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreCityId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreCity(StoreCityObject city)
        {
            try
            {
                if (city == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(city.Name.Trim().ToLower()) && city.StoreStateId.Equals(m.StoreStateId) && (m.StoreCityId != city.StoreCityId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var cityEntity = ModelCrossMapper.Map<StoreCityObject, StoreCity>(city);
                if (cityEntity == null || cityEntity.StoreCityId < 1)
                {
                    return -2;
                }
                _repository.Update(cityEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreCity(long cityId)
        {
            try
            {
                var returnStatus = _repository.Remove(cityId);
                _uoWork.SaveChanges();
                return returnStatus.StoreCityId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreCityObject GetStoreCity(long cityId)
        {
            try
            {
                var myItem = _repository.GetById(cityId);
                if (myItem == null || myItem.StoreCityId < 1)
                {
                    return new StoreCityObject();
                }
                var cityObject = ModelCrossMapper.Map<StoreCity, StoreCityObject>(myItem);
                if (cityObject == null || cityObject.StoreCityId < 1)
                {
                    return new StoreCityObject();
                }
                return cityObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCityObject();
            }
        }

        public List<StoreCityObject> GetStoreCityObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreCity> cityEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    cityEntityList = _repository.GetWithPaging(m => m.StoreCityId, tpageNumber, tsize, "StoreState").ToList();
                }

                else
                {
                    cityEntityList = _repository.GetAll("StoreState").ToList();
                }

                if (!cityEntityList.Any())
                {
                    return new List<StoreCityObject>();
                }
                var cityObjList = new List<StoreCityObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<StoreCity, StoreCityObject>(m);
                    if (cityObject != null && cityObject.StoreCityId > 0)
                    {
                        cityObject.StateName = m.StoreState.Name;
                        cityObjList.Add(cityObject);
                    }
                });

                return cityObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCityObject>();
            }
        }

        public List<StoreCityObject> Search(string searchCriteria)
        {
            try
            {
               var cityEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "StoreState").ToList();

                if (!cityEntityList.Any())
                {
                    return new List<StoreCityObject>();
                }
                var cityObjList = new List<StoreCityObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<StoreCity, StoreCityObject>(m);
                    if (cityObject != null && cityObject.StoreCityId > 0)
                    {
                        cityObject.StateName = m.StoreState.Name;
                        cityObjList.Add(cityObject);
                    }
                });
                return cityObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCityObject>();
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

        public int GetObjectCount(Expression<Func<StoreCity, bool>> predicate)
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
        public List<StoreCityObject> GetCities()
        {
            try
            {
                var cityEntityList = _repository.GetAll().ToList();
                if (!cityEntityList.Any())
                {
                    return new List<StoreCityObject>();
                }
                var cityObjList = new List<StoreCityObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<StoreCity, StoreCityObject>(m);
                    if (cityObject != null && cityObject.StoreCityId > 0)
                    {
                        cityObjList.Add(cityObject);
                    }
                });
                return cityObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
