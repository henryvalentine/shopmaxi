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
	public class AccountGroupServices
	{
		private readonly AccountGroupRepository  _accountGroupRepository;
        public AccountGroupServices()
		{
            _accountGroupRepository = new AccountGroupRepository();
		}
        public long AddAccountGroup(AccountGroupObject accountGroup)
		{
			try
			{
                return _accountGroupRepository.AddAccountGroup(accountGroup);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        public AccountGroupObject GetAccountGroup(long accountGroupId)
        {
            try
            {
                return _accountGroupRepository.GetAccountGroup(accountGroupId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new AccountGroupObject();
            }
        }
        public int GetObjectCount()
        {
            try
            {
                return _accountGroupRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateAccountGroup(AccountGroupObject accountGroup)
		{
			try
			{
                return _accountGroupRepository.UpdateAccountGroup(accountGroup);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteAccountGroup(long accountGroupId)
		{
			try
			{
                return _accountGroupRepository.DeleteAccountGroup(accountGroupId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        //public AccountGroupObject GetAccountGroup(long accountGroupId)
        //{
        //    try
        //    {
        //        return _accountGroupRepository.GetAccountGroup(accountGroupId);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return new AccountGroupObject();
        //    }
        //}
        public List<AccountGroupObject> GetAccountGroupObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _accountGroupRepository.GetAccountGroupObject(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<AccountGroupObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AccountGroupObject>();
            }
        }
        public List<AccountGroupObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _accountGroupRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<AccountGroupObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AccountGroupObject>();
            }
        }

        public List<AccountGroupObject> GetAccountGroups()
        {
            try
            {
                var objList = _accountGroupRepository.GetAccountGroups();
                if (objList == null || !objList.Any())
                {
                    return new List<AccountGroupObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AccountGroupObject>();
            }
        }
	  }

}
