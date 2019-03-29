using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class CityServices
	{
		private readonly CityRepository  _cityRepository;
        public CityServices()
		{
            _cityRepository = new CityRepository();
		}

        public long AddCity(CityObject cityAccount)
		{
			try
			{
                return _cityRepository.AddCity(cityAccount);
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

        public int UpdateCity(CityObject city)
		{
			try
			{
                return _cityRepository.UpdateCity(city);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteCity(long cityAccountId)
		{
			try
			{
                return _cityRepository.DeleteCity(cityAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public CityObject GetCity(long cityAccountId)
		{
			try
			{
                return _cityRepository.GetCity(cityAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CityObject();
			}
		}

        public List<CityObject> GetCities()
		{
			try
			{
                var objList = _cityRepository.GetCities();
                if (objList == null || !objList.Any())
			    {
                    return new List<CityObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CityObject>();
			}
		}

        public List<CityObject> GetCityObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _cityRepository.GetCityObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<CityObject>();
                }
                return objList;
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
                var objList = _cityRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<CityObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CityObject>();
            }
        }
	}

}
