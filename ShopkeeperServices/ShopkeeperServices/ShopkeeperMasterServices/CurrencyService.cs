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
	public class CurrencyServices
	{
		private readonly CurrencyRepository  _currencyAccountRepository;
        public CurrencyServices()
		{
            _currencyAccountRepository = new CurrencyRepository();
		}

        public long AddCurrency(CurrencyObject currencyAccount)
		{
			try
			{
                return _currencyAccountRepository.AddCurrency(currencyAccount);
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
                return _currencyAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateCurrency(CurrencyObject currencyAccount)
		{
			try
			{
                return _currencyAccountRepository.UpdateCurrency(currencyAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteCurrency(long currencyAccountId)
		{
			try
			{
                return _currencyAccountRepository.DeleteCurrency(currencyAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public CurrencyObject GetCurrency(long currencyAccountId)
		{
			try
			{
                return _currencyAccountRepository.GetCurrency(currencyAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CurrencyObject();
			}
		}

        public List<CurrencyObject> GetCurrencies()
		{
			try
			{
                var objList = _currencyAccountRepository.GetCurrencies();
                if (objList == null || !objList.Any())
			    {
                    return new List<CurrencyObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CurrencyObject>();
			}
		}

        public List<CurrencyObject> GetCurrencyObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _currencyAccountRepository.GetCurrencyObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<CurrencyObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CurrencyObject>();
            }
        }

        public List<CurrencyObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _currencyAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<CurrencyObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CurrencyObject>();
            }
        }
	}

}
