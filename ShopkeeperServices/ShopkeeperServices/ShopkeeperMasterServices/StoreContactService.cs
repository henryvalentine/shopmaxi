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
	public class StoreContactServices
	{
		private readonly StoreContactRepository  _storeContactAccountRepository;
        public StoreContactServices()
		{
            _storeContactAccountRepository = new StoreContactRepository();
		}

        public long AddStoreContact(StoreContactObject storeContactAccount)
		{
			try
			{
                return _storeContactAccountRepository.AddStoreContact(storeContactAccount);
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
                return _storeContactAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreContact(StoreContactObject storeContactAccount)
		{
			try
			{
                return _storeContactAccountRepository.UpdateStoreContact(storeContactAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreContact(long storeContactAccountId)
		{
			try
			{
                return _storeContactAccountRepository.DeleteStoreContact(storeContactAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreContactObject GetStoreContact(long storeContactAccountId)
		{
			try
			{
                return _storeContactAccountRepository.GetStoreContact(storeContactAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreContactObject();
			}
		}

        public List<StoreContactObject> GetStoreContacts()
		{
			try
			{
                var objList = _storeContactAccountRepository.GetStoreContacts();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreContactObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactObject>();
			}
		}

        public List<StoreContactObject> GetStoreContactObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeContactAccountRepository.GetStoreContactObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreContactObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactObject>();
            }
        }
        
	}

}
