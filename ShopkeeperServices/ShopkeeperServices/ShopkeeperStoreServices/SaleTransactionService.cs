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
    public class SaleTransactionServices
    {
        private readonly SaleTransactionRepository _saleTransactionRepository;
        public SaleTransactionServices()
        {
            _saleTransactionRepository = new SaleTransactionRepository();
        }

        public long AddSaleTransaction(SaleTransactionObject saleTransactionAccount)
        {
            try
            {
                return _saleTransactionRepository.AddSaleTransaction(saleTransactionAccount);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSaleTransaction(SaleTransactionObject saleTransaction)
        {
            try
            {
                return _saleTransactionRepository.UpdateSaleTransaction(saleTransaction);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSaleTransaction(long saleTransactionAccountId)
        {
            try
            {
                return _saleTransactionRepository.DeleteSaleTransaction(saleTransactionAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public bool DeleteSaleTransactionByTransaction(long transactionId)
        {
            try
            {
                return _saleTransactionRepository.DeleteSaleTransactionByTransaction(transactionId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SaleTransactionObject GetSaleTransaction(long saleTransactionAccountId)
        {
            try
            {
                return _saleTransactionRepository.GetSaleTransaction(saleTransactionAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SaleTransactionObject();
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _saleTransactionRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<SaleTransactionObject> GetSaleTransactions()
        {
            try
            {
                var objList = _saleTransactionRepository.GetSaleTransactions();
                if (objList == null || !objList.Any())
                {
                    return new List<SaleTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleTransactionObject>();
            }
        }

        public List<SaleTransactionObject> GetSaleTransactionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _saleTransactionRepository.GetSaleTransactionObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SaleTransactionObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleTransactionObject>();
            }
        }

    }

}
