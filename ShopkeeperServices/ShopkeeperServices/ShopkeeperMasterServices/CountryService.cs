using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class CountryServices
	{
		private readonly CountryRepository  _countryRepository;
        public CountryServices()
		{
            _countryRepository = new CountryRepository();
		}

        public long AddCountry(CountryObject countryAccount)
		{
			try
			{
                return _countryRepository.AddCountry(countryAccount);
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
                return _countryRepository.UpdateCountry(country);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteCountry(long countryAccountId)
		{
			try
			{
                return _countryRepository.DeleteCountry(countryAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public CountryObject GetCountry(long countryAccountId)
		{
			try
			{
                return _countryRepository.GetCountry(countryAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CountryObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _countryRepository.GetObjectCount();
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
                return _countryRepository.GetObjectCount(predicate);
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
                var objList = _countryRepository.GetCountries();
                if (objList == null || !objList.Any())
			    {
                    return new List<CountryObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CountryObject>();
			}
		}

        public List<CountryObject> GetCountryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _countryRepository.GetCountryObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<CountryObject>();
                }
                return objList;
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
                var objList = _countryRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<CountryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CountryObject>();
            }
        }
	}

}
