using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreAddressRepository
    {
       private readonly IShopkeeperRepository<StoreAddress> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreAddressRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreAddress>(_uoWork);
		}
       
        public long AddStoreAddress(StoreAddressObject city)
        {
            try
            {
                if (city == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StreetNo.Trim().ToLower().Equals(city.StreetNo.Trim().ToLower()) && city.StoreCityId.Equals(m.StoreCityId));
                if (duplicates > 0)
                {
                    var ads = _repository.GetAll(m => m.StreetNo.Trim().ToLower().Equals(city.StreetNo.Trim().ToLower()) && city.StoreCityId.Equals(m.StoreCityId)).ToList();
                    if (ads.Any())
                    {
                        return ads[0].StoreAddressId;
                    }
                }
                var cityEntity = ModelCrossMapper.Map<StoreAddressObject, StoreAddress>(city);
                if (cityEntity == null || string.IsNullOrEmpty(cityEntity.StreetNo))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(cityEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreAddressId;
            }

            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }
                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return 0;
            }
        }

        public long UpdateStoreAddress(StoreAddressObject city)
        {
            try
            {
                if (city == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StreetNo.Trim().ToLower().Equals(city.StreetNo.Trim().ToLower()) && city.StoreCityId.Equals(m.StoreCityId) && (m.StoreAddressId != city.StoreAddressId));
                if (duplicates > 0)
                {
                    var ads = _repository.GetAll(m => m.StreetNo.Trim().ToLower().Equals(city.StreetNo.Trim().ToLower()) && city.StoreCityId.Equals(m.StoreCityId)).ToList();
                    if (ads.Any())
                    {
                        return ads[0].StoreAddressId;
                    }
                }
                var cityEntity = ModelCrossMapper.Map<StoreAddressObject, StoreAddress>(city);
                if (cityEntity == null || cityEntity.StoreAddressId < 1)
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

        public bool DeleteStoreAddress(long cityId)
        {
            try
            {
                var returnStatus = _repository.Remove(cityId);
                _uoWork.SaveChanges();
                return returnStatus.StoreAddressId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreAddressObject GetStoreAddress(long cityId)
        {
            try
            {
                var myItem = _repository.GetById(cityId);
                if (myItem == null || myItem.StoreAddressId < 1)
                {
                    return new StoreAddressObject();
                }
                var cityObject = ModelCrossMapper.Map<StoreAddress, StoreAddressObject>(myItem);
                if (cityObject == null || cityObject.StoreAddressId < 1)
                {
                    return new StoreAddressObject();
                }
                return cityObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreAddressObject();
            }
        }

        public StoreAddressObject GetStoreAddress()
        {
            try
            {
                var myItem = _repository.GetAll("StoreCity").ToList()[0];
                if (myItem == null || myItem.StoreAddressId < 1)
                {
                    return new StoreAddressObject();
                }
                var cityObject = ModelCrossMapper.Map<StoreAddress, StoreAddressObject>(myItem);
                if (cityObject == null || cityObject.StoreAddressId < 1)
                {
                    return new StoreAddressObject();
                }
                cityObject.StreetNo = myItem.StreetNo + ", " + myItem.StoreCity.Name;
                return cityObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreAddressObject();
            }
        }

        public List<StoreAddressObject> GetStoreAddressObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreAddress> cityEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    cityEntityList = _repository.GetWithPaging(m => m.StoreAddressId, tpageNumber, tsize, "StoreCity").ToList();
                }

                else
                {
                    cityEntityList = _repository.GetAll("StoreCity").ToList();
                }

                if (!cityEntityList.Any())
                {
                    return new List<StoreAddressObject>();
                }
                var cityObjList = new List<StoreAddressObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<StoreAddress, StoreAddressObject>(m);
                    if (cityObject != null && cityObject.StoreAddressId > 0)
                    {
                        cityObject.CityName = m.StoreCity.Name;
                        cityObjList.Add(cityObject);
                    }
                });

                return cityObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreAddressObject>();
            }
        }

        public List<StoreAddressObject> Search(string searchCriteria)
        {
            try
            {
               var cityEntityList = _repository.GetAll(m => m.StreetNo.ToLower().Contains(searchCriteria.ToLower()), "StoreCity").ToList();

                if (!cityEntityList.Any())
                {
                    return new List<StoreAddressObject>();
                }
                var cityObjList = new List<StoreAddressObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<StoreAddress, StoreAddressObject>(m);
                    if (cityObject != null && cityObject.StoreAddressId > 0)
                    {
                        cityObject.CityName = m.StoreCity.Name;
                        cityObjList.Add(cityObject);
                    }
                });
                return cityObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreAddressObject>();
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

        public int GetObjectCount(Expression<Func<StoreAddress, bool>> predicate)
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
        public List<StoreAddressObject> GetCities()
        {
            try
            {
                var cityEntityList = _repository.GetAll().ToList();
                if (!cityEntityList.Any())
                {
                    return new List<StoreAddressObject>();
                }
                var cityObjList = new List<StoreAddressObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<StoreAddress, StoreAddressObject>(m);
                    if (cityObject != null && cityObject.StoreAddressId > 0)
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
