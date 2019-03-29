using System;
using System.Collections.Generic;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;


namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class BankAccountService
	{
		private readonly BankAccountRepository  _bankAccountRepository;
        public BankAccountService()
		{
            _bankAccountRepository = new BankAccountRepository();
		}

        public long AddBankAccount(BankAccountObject bankAccount)
		{
			try
			{
                return _bankAccountRepository.AddBankAccount(bankAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int UpdateBankAccount(BankAccountObject bankAccount)
		{
			try
			{
                return _bankAccountRepository.UpdateBankAccount(bankAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public bool DeleteBankAccount(long bankAccountId)
		{
			try
			{
                return _bankAccountRepository.DeleteBankAccount(bankAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public BankAccountObject GetBankAccount(long bankAccountId)
		{
			try
			{
                return _bankAccountRepository.GetBankAccount(bankAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BankAccountObject();
			}
		}

        public BankAccountObject GetBankAccountByCustomerId(long customerId)
        {
            try
            {
                return _bankAccountRepository.GetBankAccountByCustomerId(customerId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BankAccountObject();
            }
        }

        public List<BankAccountObject> GetBankAccounts()
		{
			try
			{
                var objList = _bankAccountRepository.GetBankAccounts();
			    if (objList == null)
			    {
                    return new List<BankAccountObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankAccountObject>();
			}
		}

	}

}
