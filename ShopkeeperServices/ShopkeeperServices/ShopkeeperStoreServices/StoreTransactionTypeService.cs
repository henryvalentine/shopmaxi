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
	public class StoreTransactionTypeServices
	{
		private readonly StoreTransactionTypeRepository  _transactionTypeRepository;
        public StoreTransactionTypeServices()
		{
            _transactionTypeRepository = new StoreTransactionTypeRepository();
		}
        public long AddStoreTransactionType(StoreTransactionTypeObject transactionType)
		{
			try
			{
                return _transactionTypeRepository.AddStoreTransactionType(transactionType);
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
                return _transactionTypeRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreTransactionType(StoreTransactionTypeObject transactionType)
		{
			try
			{
                return _transactionTypeRepository.UpdateStoreTransactionType(transactionType);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreTransactionType(long transactionTypeId)
		{
			try
			{
                return _transactionTypeRepository.DeleteStoreTransactionType(transactionTypeId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreTransactionTypeObject GetStoreTransactionType(long transactionTypeId)
		{
			try
			{
                return _transactionTypeRepository.GetStoreTransactionType(transactionTypeId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreTransactionTypeObject();
			}
		}
        public List<StoreTransactionTypeObject> GetStoreTransactionTypes()
		{
			try
			{
                var objList = _transactionTypeRepository.GetStoreTransactionTypes();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreTransactionTypeObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionTypeObject>();
			}
		}
        public List<StoreTransactionTypeObject> GetStoreTransactionTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _transactionTypeRepository.GetStoreTransactionTypeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionTypeObject>();
            }
        }

        public List<StoreTransactionTypeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _transactionTypeRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionTypeObject>();
            }
        }
	  }

}
