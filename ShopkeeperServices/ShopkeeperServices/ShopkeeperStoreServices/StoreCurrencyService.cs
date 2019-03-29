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
	public class StoreCurrencyServices
	{
		private readonly StoreCurrencyRepository  _storeCurrencyAccountRepository;
        public StoreCurrencyServices()
		{
            _storeCurrencyAccountRepository = new StoreCurrencyRepository();
		}

        public long AddStoreCurrency(StoreCurrencyObject storeCurrencyAccount)
		{
			try
			{
                return _storeCurrencyAccountRepository.AddStoreCurrency(storeCurrencyAccount);
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
                return _storeCurrencyAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreCurrency(StoreCurrencyObject storeCurrencyAccount)
		{
			try
			{
                return _storeCurrencyAccountRepository.UpdateStoreCurrency(storeCurrencyAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreCurrency(long storeCurrencyAccountId)
		{
			try
			{
                return _storeCurrencyAccountRepository.DeleteStoreCurrency(storeCurrencyAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreCurrencyObject GetStoreCurrency(long storeCurrencyAccountId)
		{
			try
			{
                return _storeCurrencyAccountRepository.GetStoreCurrency(storeCurrencyAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCurrencyObject();
			}
		}

        public StoreCurrencyObject GetStoreCurrency()
        {
            try
            {
                return _storeCurrencyAccountRepository.GetStoreCurrency();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCurrencyObject();
            }
        }

        public List<StoreCurrencyObject> GetCurrencies()
		{
			try
			{
                var objList = _storeCurrencyAccountRepository.GetCurrencies();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreCurrencyObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCurrencyObject>();
			}
		}

        public List<StoreCurrencyObject> GetStoreCurrencyObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeCurrencyAccountRepository.GetStoreCurrencyObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCurrencyObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCurrencyObject>();
            }
        }

        public List<StoreCurrencyObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeCurrencyAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreCurrencyObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCurrencyObject>();
            }
        }
	}

}
