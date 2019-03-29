using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class CountryServices
	{
		private readonly CountryRepository  _countryRepository;
        public CountryServices()
		{
            _countryRepository = new CountryRepository();
		}

        public long AddCountry(StoreCountryObject countryAccount)
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
        
        public int UpdateCountry(StoreCountryObject country)
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

        public StoreCountryObject GetCountry(long countryAccountId)
		{
			try
			{
                return _countryRepository.GetCountry(countryAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCountryObject();
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
        
        //public int GetObjectCount(Expression<Func<Country, bool>> predicate)
        //{
        //    try
        //    {
        //        return _countryRepository.GetObjectCount(predicate);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return 0;
        //    }
        //}

        public List<StoreCountryObject> GetCountries()
		{
			try
			{
                var objList = _countryRepository.GetCountries();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreCountryObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCountryObject>();
			}
		}

        public List<StoreCountryObject> GetCountryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _countryRepository.GetCountryObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCountryObject>();
                }
                return objList;
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
                var objList = _countryRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCountryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCountryObject>();
            }
        }
	}

}
