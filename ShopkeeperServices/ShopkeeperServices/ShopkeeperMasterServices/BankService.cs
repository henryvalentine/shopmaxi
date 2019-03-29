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
	public class BankServices
	{
		private readonly BankRepository  _bankAccountRepository;
        public BankServices()
		{
            _bankAccountRepository = new BankRepository();
		}

        public long AddBank(BankObject bankAccount)
		{
			try
			{
                return _bankAccountRepository.AddBank(bankAccount);
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
                return _bankAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateBank(BankObject bankAccount)
		{
			try
			{
                return _bankAccountRepository.UpdateBank(bankAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteBank(long bankAccountId)
		{
			try
			{
                return _bankAccountRepository.DeleteBank(bankAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public BankObject GetBank(long bankAccountId)
		{
			try
			{
                return _bankAccountRepository.GetBank(bankAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BankObject();
			}
		}

        public List<BankObject> GetBanks()
		{
			try
			{
                var objList = _bankAccountRepository.GetBanks();
                if (objList == null || !objList.Any())
			    {
                    return new List<BankObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankObject>();
			}
		}

        public List<BankObject> GetBankObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _bankAccountRepository.GetBankObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<BankObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankObject>();
            }
        }

        public List<BankObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _bankAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<BankObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankObject>();
            }
        }
	}

}
