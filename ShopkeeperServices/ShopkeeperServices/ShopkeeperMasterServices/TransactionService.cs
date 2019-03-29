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
	public class TransactionServices
	{
		private readonly TransactionRepository  _transactionRepository;
        public TransactionServices()
		{
            _transactionRepository = new TransactionRepository();
		}

        public long AddTransaction(TransactionObject transaction)
		{
			try
			{
                return _transactionRepository.AddTransaction(transaction);
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
                return _transactionRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateTransaction(TransactionObject transaction)
		{
			try
			{
                return _transactionRepository.UpdateTransaction(transaction);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteTransaction(long transactionId)
		{
			try
			{
                return _transactionRepository.DeleteTransaction(transactionId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public TransactionObject GetTransaction(long transactionId)
		{
			try
			{
                return _transactionRepository.GetTransaction(transactionId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new TransactionObject();
			}
		}

        public List<TransactionObject> GetTransactions()
		{
			try
			{
                var objList = _transactionRepository.GetTransactions();
                if (objList == null || !objList.Any())
			    {
                    return new List<TransactionObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
			}
		}

        public List<TransactionObject> GetTransactionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _transactionRepository.GetTransactionObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<TransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }

        public List<TransactionObject> GetTransactionsByType(int? itemsPerPage, int? pageNumber, int transactionTypeId)
        {
            try
            {
                var objList = _transactionRepository.GetTransactionsByType(itemsPerPage, pageNumber, transactionTypeId);
                if (objList == null || !objList.Any())
                {
                    return new List<TransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }
        public List<TransactionObject> GetTransactionsByPaymentMethod(int? itemsPerPage, int? pageNumber, int paymentMethodId)
        {
            try
            {
                var objList = _transactionRepository.GetTransactionsByPaymentMethod(itemsPerPage, pageNumber, paymentMethodId);
                if (objList == null || !objList.Any())
                {
                    return new List<TransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }

        public List<TransactionObject> GetTransactionsByDateRange(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime stopDate)
        {
            try
            {
                var objList = _transactionRepository.GetTransactionsByDateRange(itemsPerPage, pageNumber, startDate, stopDate);
                if (objList == null || !objList.Any())
                {
                    return new List<TransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<TransactionObject>();
            }
        }
        
	}

}
