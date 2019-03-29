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
	public class StoreTransactionServices
	{
		private readonly StoreTransactionRepository  _transactionRepository;
        public StoreTransactionServices()
		{
            _transactionRepository = new StoreTransactionRepository();
		}

        public long AddStoreTransaction(StoreTransactionObject transaction)
        {
            try
            {
                return _transactionRepository.AddStoreTransaction(transaction);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long RevokeStoreTransaction(StoreTransactionObject transaction, bool saleRevoked)
        {
            return _transactionRepository.RevokeStoreTransaction(transaction, saleRevoked);
        }

        public int GetObjectCount()
        {
            try
            {
                return _transactionRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreTransaction(StoreTransactionObject transaction)
        {
            try
            {
                return _transactionRepository.UpdateStoreTransaction(transaction);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreTransaction(long transactionId)
        {
            try
            {
                return _transactionRepository.DeleteStoreTransaction(transactionId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreTransactionObject GetStoreTransaction(long transactionId)
        {
            try
            {
                return _transactionRepository.GetStoreTransaction(transactionId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreTransactionObject();
            }
        }

        public List<StoreTransactionObject> GetStoreTransactions()
        {
            try
            {
                var objList = _transactionRepository.GetStoreTransactions();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionObject>();
            }
        }

        public List<StoreTransactionObject> GetStoreTransactionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _transactionRepository.GetStoreTransactionObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionObject>();
            }
        }

        public List<StoreTransactionObject> GetStoreTransactionsByType(int? itemsPerPage, int? pageNumber, int transactionTypeId)
        {
            try
            {
                var objList = _transactionRepository.GetStoreTransactionsByType(itemsPerPage, pageNumber, transactionTypeId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionObject>();
            }
        }
        public List<StoreTransactionObject> GetStoreTransactionsByPaymentMethod(int? itemsPerPage, int? pageNumber, int paymentMethodId)
        {
            try
            {
                var objList = _transactionRepository.GetStoreTransactionsByPaymentMethod(itemsPerPage, pageNumber, paymentMethodId);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionObject>();
            }
        }

        public List<StoreTransactionObject> GetStoreTransactionsByDateRange(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime stopDate)
        {
            try
            {
                var objList = _transactionRepository.GetStoreTransactionsByDateRange(itemsPerPage, pageNumber, startDate, stopDate);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreTransactionObject>();
            }
        }
	}

}
