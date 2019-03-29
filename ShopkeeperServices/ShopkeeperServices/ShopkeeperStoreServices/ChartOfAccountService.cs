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
	public class ChartOfAccountServices
	{
		private readonly ChartOfAccountRepository  _chartOfAccountRepository;
        public ChartOfAccountServices()
		{
            _chartOfAccountRepository = new ChartOfAccountRepository();
		}

        public long AddChartOfAccount(ChartOfAccountObject chartOfAccount)
		{
			try
			{
                return _chartOfAccountRepository.AddChartOfAccount(chartOfAccount);
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
                return _chartOfAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateChartOfAccount(ChartOfAccountObject chartOfAccount)
		{
			try
			{
                return _chartOfAccountRepository.UpdateChartOfAccount(chartOfAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteChartOfAccount(long chartOfAccountId)
		{
			try
			{
                return _chartOfAccountRepository.DeleteChartOfAccount(chartOfAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public ChartOfAccountObject GetChartOfAccount(long chartOfAccountId)
		{
			try
			{
                return _chartOfAccountRepository.GetChartOfAccount(chartOfAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ChartOfAccountObject();
			}
		}

        public List<ChartOfAccountObject> GetChartsOfAccount()
		{
			try
			{
                var objList = _chartOfAccountRepository.GetChartsOfAccount();
                if (objList == null || !objList.Any())
			    {
                    return new List<ChartOfAccountObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChartOfAccountObject>();
			}
		}

        public List<ChartOfAccountObject> GetChartOfAccountObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _chartOfAccountRepository.GetChartOfAccountObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<ChartOfAccountObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChartOfAccountObject>();
            }
        }

        public List<ChartOfAccountObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _chartOfAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<ChartOfAccountObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChartOfAccountObject>();
            }
        }
	}

}
