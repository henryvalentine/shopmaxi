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
    public class CountryRepository
    {
        private readonly IShopkeeperRepository<StoreCountry> _repository;
       private readonly UnitOfWork _uoWork;

       public CountryRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreCountry>(_uoWork);
		}
       
        public long AddCountry(StoreCountryObject country)
        {
            try
            {
                if (country == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == country.Name.Trim().ToLower() && (m.StoreCountryId != country.StoreCountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var countryEntity = ModelCrossMapper.Map<StoreCountryObject, StoreCountry>(country);
                if (countryEntity == null || string.IsNullOrEmpty(countryEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(countryEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreCountryId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateCountry(StoreCountryObject country)
        {
            try
            {
                if (country == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == country.Name.Trim().ToLower() && (m.StoreCountryId != country.StoreCountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var countryEntity = ModelCrossMapper.Map<StoreCountryObject, StoreCountry>(country);
                if (countryEntity == null || countryEntity.StoreCountryId < 1)
                {
                    return -2;
                }
                _repository.Update(countryEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteCountry(long countryId)
        {
            try
            {
                var returnStatus = _repository.Remove(countryId);
                _uoWork.SaveChanges();
                return returnStatus.StoreCountryId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreCountryObject GetCountry(long countryId)
        {
            try
            {
                var myItem = _repository.GetById(countryId);
                if (myItem == null || myItem.StoreCountryId < 1)
                {
                    return new StoreCountryObject();
                }
                var countryObject = ModelCrossMapper.Map<StoreCountry, StoreCountryObject>(myItem);
                if (countryObject == null || countryObject.StoreCountryId < 1)
                {
                    return new StoreCountryObject();
                }
                return countryObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCountryObject();
            }
        }

        public List<StoreCountryObject> GetCountryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreCountry> countryEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    countryEntityList = _repository.GetWithPaging(m => m.StoreCountryId, tpageNumber, tsize).ToList();
                }

                else
                {
                    countryEntityList = _repository.GetAll().ToList();
                }

                if (!countryEntityList.Any())
                {
                    return new List<StoreCountryObject>();
                }
                var countryObjList = new List<StoreCountryObject>();
                countryEntityList.ForEach(m =>
                {
                    var countryObject = ModelCrossMapper.Map<StoreCountry, StoreCountryObject>(m);
                    if (countryObject != null && countryObject.StoreCountryId > 0)
                    {
                        countryObjList.Add(countryObject);
                    }
                });

                return countryObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCountryObject>();
            }
        }

        public List<StoreCountryObject> Search(string searchCriteria)
        {
            try
            {
               var countryEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!countryEntityList.Any())
                {
                    return new List<StoreCountryObject>();
                }
                var countryObjList = new List<StoreCountryObject>();
                countryEntityList.ForEach(m =>
                {
                    var countryObject = ModelCrossMapper.Map<StoreCountry, StoreCountryObject>(m);
                    if (countryObject != null && countryObject.StoreCountryId > 0)
                    {
                        countryObjList.Add(countryObject);
                    }
                });
                return countryObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCountryObject>();
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

        public int GetObjectCount(Expression<Func<StoreCountry, bool>> predicate)
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

        public List<StoreCountryObject> GetCountries()
        {
            try
            {
                var countryEntityList = _repository.GetAll().ToList();
                if (!countryEntityList.Any())
                {
                    return new List<StoreCountryObject>();
                }
                var countryObjList = new List<StoreCountryObject>();
                countryEntityList.ForEach(m =>
                {
                    var countryObject = ModelCrossMapper.Map<StoreCountry, StoreCountryObject>(m);
                    if (countryObject != null && countryObject.StoreCountryId > 0)
                    {
                        countryObjList.Add(countryObject);
                    }
                });
                return countryObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
