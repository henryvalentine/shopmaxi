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
	public class BillingCycleServices
	{
		private readonly BillingCycleRepository  _billingCycleAccountRepository;
        public BillingCycleServices()
		{
            _billingCycleAccountRepository = new BillingCycleRepository();
		}

        public long AddBillingCycle(BillingCycleObject billingCycleAccount)
		{
			try
			{
                return _billingCycleAccountRepository.AddBillingCycle(billingCycleAccount);
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
                return _billingCycleAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateBillingCycle(BillingCycleObject billingCycleAccount)
		{
			try
			{
                return _billingCycleAccountRepository.UpdateBillingCycle(billingCycleAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteBillingCycle(long billingCycleAccountId)
		{
			try
			{
                return _billingCycleAccountRepository.DeleteBillingCycle(billingCycleAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public BillingCycleObject GetBillingCycle(long billingCycleAccountId)
		{
			try
			{
                return _billingCycleAccountRepository.GetBillingCycle(billingCycleAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BillingCycleObject();
			}
		}

        public List<BillingCycleObject> GetBillingCycles()
		{
			try
			{
                var objList = _billingCycleAccountRepository.GetBillingCycles();
                if (objList == null || !objList.Any())
			    {
                    return new List<BillingCycleObject>();
			    }

				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BillingCycleObject>();
			}
		}

        public List<BillingCycleObject> GetBillingCycleObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _billingCycleAccountRepository.GetBillingCycleObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<BillingCycleObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BillingCycleObject>();
            }
        }

        public List<BillingCycleObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _billingCycleAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<BillingCycleObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BillingCycleObject>();
            }
        }
	}

}
