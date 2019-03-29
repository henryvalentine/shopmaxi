using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class CityRepository
    {
       private readonly IShopkeeperRepository<City> _repository;
       private readonly UnitOfWork _uoWork;

       public CityRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<City>(_uoWork);
		}
       
        public long AddCity(CityObject city)
        {
            try
            {
                if (city == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(city.Name.Trim().ToLower()) && city.StateId.Equals(m.StateId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var cityEntity = ModelCrossMapper.Map<CityObject, City>(city);
                if (cityEntity == null || string.IsNullOrEmpty(cityEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(cityEntity);
                _uoWork.SaveChanges();
                return returnStatus.CityId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateCity(CityObject city)
        {
            try
            {
                if (city == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(city.Name.Trim().ToLower()) && city.StateId.Equals(m.StateId) && (m.CityId != city.CityId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var cityEntity = ModelCrossMapper.Map<CityObject, City>(city);
                if (cityEntity == null || cityEntity.CityId < 1)
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

        public bool DeleteCity(long cityId)
        {
            try
            {
                var returnStatus = _repository.Remove(cityId);
                _uoWork.SaveChanges();
                return returnStatus.CityId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public CityObject GetCity(long cityId)
        {
            try
            {
                var myItem = _repository.GetById(cityId);
                if (myItem == null || myItem.CityId < 1)
                {
                    return new CityObject();
                }
                var cityObject = ModelCrossMapper.Map<City, CityObject>(myItem);
                if (cityObject == null || cityObject.CityId < 1)
                {
                    return new CityObject();
                }
                return cityObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CityObject();
            }
        }

        public List<CityObject> GetCityObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<City> cityEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    cityEntityList = _repository.GetWithPaging(m => m.CityId, tpageNumber, tsize, "State").ToList();
                }

                else
                {
                    cityEntityList = _repository.GetAll("State").ToList();
                }

                if (!cityEntityList.Any())
                {
                    return new List<CityObject>();
                }
                var cityObjList = new List<CityObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<City, CityObject>(m);
                    if (cityObject != null && cityObject.CityId > 0)
                    {
                        cityObject.StateName = m.State.Name;
                        cityObjList.Add(cityObject);
                    }
                });

                return cityObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CityObject>();
            }
        }

        public List<CityObject> Search(string searchCriteria)
        {
            try
            {
               var cityEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "State").ToList();

                if (!cityEntityList.Any())
                {
                    return new List<CityObject>();
                }
                var cityObjList = new List<CityObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<City, CityObject>(m);
                    if (cityObject != null && cityObject.CityId > 0)
                    {
                        cityObject.StateName = m.State.Name;
                        cityObjList.Add(cityObject);
                    }
                });
                return cityObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CityObject>();
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

        public int GetObjectCount(Expression<Func<City, bool>> predicate)
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
        public List<CityObject> GetCities()
        {
            try
            {
                var cityEntityList = _repository.GetAll().ToList();
                if (!cityEntityList.Any())
                {
                    return new List<CityObject>();
                }
                var cityObjList = new List<CityObject>();
                cityEntityList.ForEach(m =>
                {
                    var cityObject = ModelCrossMapper.Map<City, CityObject>(m);
                    if (cityObject != null && cityObject.CityId > 0)
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
