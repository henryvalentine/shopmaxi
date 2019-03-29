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
	public class StoreDepartmentServices
	{
		private readonly StoreDepartmentRepository  _storeDepartmentAccountRepository;
        public StoreDepartmentServices()
		{
            _storeDepartmentAccountRepository = new StoreDepartmentRepository();
		}

        public long AddStoreDepartment(StoreDepartmentObject storeDepartmentAccount)
		{
			try
			{
                return _storeDepartmentAccountRepository.AddStoreDepartment(storeDepartmentAccount);
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
                return _storeDepartmentAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreDepartment(StoreDepartmentObject storeDepartmentAccount)
		{
			try
			{
                return _storeDepartmentAccountRepository.UpdateStoreDepartment(storeDepartmentAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStoreDepartment(long storeDepartmentAccountId)
		{
			try
			{
                return _storeDepartmentAccountRepository.DeleteStoreDepartment(storeDepartmentAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreDepartmentObject GetStoreDepartment(long storeDepartmentAccountId)
		{
			try
			{
                return _storeDepartmentAccountRepository.GetStoreDepartment(storeDepartmentAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreDepartmentObject();
			}
		}

        public List<StoreDepartmentObject> GetStoreDepartments()
		{
			try
			{
                var objList = _storeDepartmentAccountRepository.GetStoreDepartments();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreDepartmentObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreDepartmentObject>();
			}
		}

        public List<StoreDepartmentObject> GetStoreDepartmentObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeDepartmentAccountRepository.GetStoreDepartmentObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreDepartmentObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreDepartmentObject>();
            }
        }

        public List<StoreDepartmentObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeDepartmentAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreDepartmentObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreDepartmentObject>();
            }
        }
	}

}
