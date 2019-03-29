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
	public class StoreCustomerTypeServices
	{
		private readonly StoreCustomerTypeRepository  _storeCustomerTypeAccountRepository;
        public StoreCustomerTypeServices()
		{
            _storeCustomerTypeAccountRepository = new StoreCustomerTypeRepository();
		}

        public long AddStoreCustomerType(StoreCustomerTypeObject storeCustomerTypeAccount)
		{
			try
			{
                return _storeCustomerTypeAccountRepository.AddStoreCustomerType(storeCustomerTypeAccount);
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
                return _storeCustomerTypeAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreCustomerType(StoreCustomerTypeObject storeCustomerTypeAccount)
		{
			try
			{
                return _storeCustomerTypeAccountRepository.UpdateStoreCustomerType(storeCustomerTypeAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreCustomerType(long storeCustomerTypeAccountId)
		{
			try
			{
                return _storeCustomerTypeAccountRepository.DeleteStoreCustomerType(storeCustomerTypeAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreCustomerTypeObject GetStoreCustomerType(long storeCustomerTypeAccountId)
		{
			try
			{
                return _storeCustomerTypeAccountRepository.GetStoreCustomerType(storeCustomerTypeAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCustomerTypeObject();
			}
		}

        public List<StoreCustomerTypeObject> GetStoreCustomerTypes()
		{
			try
			{
                var objList = _storeCustomerTypeAccountRepository.GetStoreCustomerTypes();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreCustomerTypeObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCustomerTypeObject>();
			}
		}

        public List<StoreCustomerTypeObject> GetStoreCustomerTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeCustomerTypeAccountRepository.GetStoreCustomerTypeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCustomerTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCustomerTypeObject>();
            }
        }

        public List<StoreCustomerTypeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeCustomerTypeAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCustomerTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCustomerTypeObject>();
            }
        }
	}

}
