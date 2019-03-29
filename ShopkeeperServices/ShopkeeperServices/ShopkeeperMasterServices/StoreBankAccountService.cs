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
	public class StoreBankAccountServices
	{
		private readonly StoreStoreBankAccountRepository  _storeStoreBankAccountRepository;
        public StoreBankAccountServices()
		{
            _storeStoreBankAccountRepository = new StoreStoreBankAccountRepository();
		}

        public long AddStoreBankAccount(StoreBankAccountObject storeBankAccount)
		{
			try
			{
                return _storeStoreBankAccountRepository.AddStoreBankAccount(storeBankAccount);
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
                return _storeStoreBankAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreBankAccount(StoreBankAccountObject storeBankAccount)
		{
			try
			{
                return _storeStoreBankAccountRepository.UpdateStoreBankAccount(storeBankAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreBankAccount(long storeBankAccountId)
		{
			try
			{
                return _storeStoreBankAccountRepository.DeleteStoreBankAccount(storeBankAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreBankAccountObject GetStoreBankAccount(long storeBankAccountId)
		{
			try
			{
                return _storeStoreBankAccountRepository.GetStoreBankAccount(storeBankAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreBankAccountObject();
			}
		}

        public List<StoreBankAccountObject> GetStoreBankAccounts()
		{
			try
			{
                var objList = _storeStoreBankAccountRepository.GetStoreBankAccounts();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreBankAccountObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
			}
		}

        public List<StoreBankAccountObject> GetStoreBankAccountObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeStoreBankAccountRepository.GetStoreBankAccountObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }

        public List<StoreBankAccountObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeStoreBankAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }

        public List<StoreBankAccountObject> GetStoreBankAccountsByBank(int? itemsPerPage, int? pageNumber, long bankId)
        {
            try
            {
                var objList = _storeStoreBankAccountRepository.GetStoreBankAccountsByBank(itemsPerPage, pageNumber, bankId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }

        public List<StoreBankAccountObject> GetStoreBankAccountsByStore(int? itemsPerPage, int? pageNumber, long storeId)
        {
            try
            {
                var objList = _storeStoreBankAccountRepository.GetStoreBankAccountsByStore(itemsPerPage, pageNumber, storeId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }
	}

}
