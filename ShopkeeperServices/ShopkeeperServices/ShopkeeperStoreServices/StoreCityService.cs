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
	public class StoreCityServices
	{
		private readonly StoreCityRepository  _cityRepository;
        public StoreCityServices()
		{
            _cityRepository = new StoreCityRepository();
		}

        public long AddStoreCity(StoreCityObject cityAccount)
		{
			try
			{
                return _cityRepository.AddStoreCity(cityAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _cityRepository.GetObjectCount();
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
                return _cityRepository.UpdateStoreCity(city);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreCity(long cityAccountId)
		{
			try
			{
                return _cityRepository.DeleteStoreCity(cityAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreCityObject GetStoreCity(long cityAccountId)
		{
			try
			{
                return _cityRepository.GetStoreCity(cityAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCityObject();
			}
		}

        public List<StoreCityObject> GetCities()
		{
			try
			{
                var objList = _cityRepository.GetCities();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreCityObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCityObject>();
			}
		}

        public List<StoreCityObject> GetStoreCityObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _cityRepository.GetStoreCityObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCityObject>();
                }
                return objList;
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
                var objList = _cityRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCityObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCityObject>();
            }
        }
	}

}
