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
    public class CountryRepository
    {
        private readonly IShopkeeperRepository<Country> _repository;
       private readonly UnitOfWork _uoWork;

       public CountryRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<Country>(_uoWork);
		}
       
        public long AddCountry(CountryObject country)
        {
            try
            {
                if (country == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == country.Name.Trim().ToLower() && (m.CountryId != country.CountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var countryEntity = ModelCrossMapper.Map<CountryObject, Country>(country);
                if (countryEntity == null || string.IsNullOrEmpty(countryEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(countryEntity);
                _uoWork.SaveChanges();
                return returnStatus.CountryId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateCountry(CountryObject country)
        {
            try
            {
                if (country == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == country.Name.Trim().ToLower() && (m.CountryId != country.CountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var countryEntity = ModelCrossMapper.Map<CountryObject, Country>(country);
                if (countryEntity == null || countryEntity.CountryId < 1)
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
                return returnStatus.CountryId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public CountryObject GetCountry(long countryId)
        {
            try
            {
                var myItem = _repository.GetById(countryId);
                if (myItem == null || myItem.CountryId < 1)
                {
                    return new CountryObject();
                }
                var countryObject = ModelCrossMapper.Map<Country, CountryObject>(myItem);
                if (countryObject == null || countryObject.CountryId < 1)
                {
                    return new CountryObject();
                }
                return countryObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CountryObject();
            }
        }

        public List<CountryObject> GetCountryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<Country> countryEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    countryEntityList = _repository.GetWithPaging(m => m.CountryId, tpageNumber, tsize).ToList();
                }

                else
                {
                    countryEntityList = _repository.GetAll().ToList();
                }

                if (!countryEntityList.Any())
                {
                    return new List<CountryObject>();
                }
                var countryObjList = new List<CountryObject>();
                countryEntityList.ForEach(m =>
                {
                    var countryObject = ModelCrossMapper.Map<Country, CountryObject>(m);
                    if (countryObject != null && countryObject.CountryId > 0)
                    {
                        countryObjList.Add(countryObject);
                    }
                });

                return countryObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CountryObject>();
            }
        }

        public List<CountryObject> Search(string searchCriteria)
        {
            try
            {
               var countryEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!countryEntityList.Any())
                {
                    return new List<CountryObject>();
                }
                var countryObjList = new List<CountryObject>();
                countryEntityList.ForEach(m =>
                {
                    var countryObject = ModelCrossMapper.Map<Country, CountryObject>(m);
                    if (countryObject != null && countryObject.CountryId > 0)
                    {
                        countryObjList.Add(countryObject);
                    }
                });
                return countryObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CountryObject>();
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

        public int GetObjectCount(Expression<Func<Country, bool>> predicate)
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

        public List<CountryObject> GetCountries()
        {
            try
            {
                var countryEntityList = _repository.GetAll().ToList();
                if (!countryEntityList.Any())
                {
                    return new List<CountryObject>();
                }
                var countryObjList = new List<CountryObject>();
                countryEntityList.ForEach(m =>
                {
                    var countryObject = ModelCrossMapper.Map<Country, CountryObject>(m);
                    if (countryObject != null && countryObject.CountryId > 0)
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
