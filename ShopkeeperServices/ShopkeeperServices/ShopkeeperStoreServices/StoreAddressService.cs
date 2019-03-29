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
    public class StoreAddressServices
    {
        private readonly StoreAddressRepository _cityRepository;
        public StoreAddressServices()
        {
            _cityRepository = new StoreAddressRepository();
        }

        public long AddStoreAddress(StoreAddressObject storeAddress)
        {
            try
            {
                return _cityRepository.AddStoreAddress(storeAddress);
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

        public long UpdateStoreAddress(StoreAddressObject city)
        {
            try
            {
                return _cityRepository.UpdateStoreAddress(city);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreAddress(long storeAddressId)
        {
            try
            {
                return _cityRepository.DeleteStoreAddress(storeAddressId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreAddressObject GetStoreAddress(long storeAddressId)
        {
            try
            {
                return _cityRepository.GetStoreAddress(storeAddressId);
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
                return _cityRepository.GetStoreAddress();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreAddressObject();
            }
        }

        public List<StoreAddressObject> GetCities()
        {
            try
            {
                var objList = _cityRepository.GetCities();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreAddressObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreAddressObject>();
            }
        }

        public List<StoreAddressObject> GetStoreAddressObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _cityRepository.GetStoreAddressObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreAddressObject>();
                }
                return objList;
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
                var objList = _cityRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreAddressObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreAddressObject>();
            }
        }
    }

}
