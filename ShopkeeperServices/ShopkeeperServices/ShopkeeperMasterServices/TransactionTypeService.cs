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
	public class TransactionTypeServices
	{
		private readonly TransactionTypeRepository  _transactionTypeRepository;
        public TransactionTypeServices()
		{
            _transactionTypeRepository = new TransactionTypeRepository();
		}
        public long AddTransactionType(TransactionTypeObject transactionType)
		{
			try
			{
                return _transactionTypeRepository.AddTransactionType(transactionType);
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
        public int UpdateTransactionType(TransactionTypeObject transactionType)
		{
			try
			{
                return _transactionTypeRepository.UpdateTransactionType(transactionType);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteTransactionType(long transactionTypeId)
		{
			try
			{
                return _transactionTypeRepository.DeleteTransactionType(transactionTypeId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public TransactionTypeObject GetTransactionType(long transactionTypeId)
		{
			try
			{
                return _transactionTypeRepository.GetTransactionType(transactionTypeId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new TransactionTypeObject();
			}
		}
        public List<TransactionTypeObject> GetTransactionTypes()
		{
			try
			{
                var objList = _transactionTypeRepository.GetTransactionTypes();
                if (objList == null || !objList.Any())
			    {
                    return new List<TransactionTypeObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionTypeObject>();
			}
		}
        public List<TransactionTypeObject> GetTransactionTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _transactionTypeRepository.GetTransactionTypeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<TransactionTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionTypeObject>();
            }
        }

        public List<TransactionTypeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _transactionTypeRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<TransactionTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionTypeObject>();
            }
        }
	  }

}
