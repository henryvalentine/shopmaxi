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
    public class StockUploadServices
    {
        private readonly StockUploadRepository _stockUploadRepository;
        public StockUploadServices()
        {
            _stockUploadRepository = new StockUploadRepository();
        }

        public long AddStockUpload(StockUploadObject stockUploadAccount)
        {
            try
            {
                return _stockUploadRepository.AddStockUpload(stockUploadAccount);
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
                return _stockUploadRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStockUpload(StockUploadObject stockUpload)
        {
            try
            {
                return _stockUploadRepository.UpdateStockUpload(stockUpload);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStockUpload(long stockUploadAccountId)
        {
            try
            {
                return _stockUploadRepository.DeleteStockUpload(stockUploadAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public bool DeleteStockUploadByStorItemId(long storeItemStockId, out List<string> filePathList)
        {
            try
            {
                return _stockUploadRepository.DeleteStockUploadByStorItemId(storeItemStockId, out filePathList);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                filePathList = new List<string>();
                return false;
            }
        }
        public StockUploadObject GetStockUpload(long stockUploadAccountId)
        {
            try
            {
                return _stockUploadRepository.GetStockUpload(stockUploadAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StockUploadObject();
            }
        }

        public List<StockUploadObject> GetStockUploads()
        {
            try
            {
                var objList = _stockUploadRepository.GetStockUploads();
                if (objList == null || !objList.Any())
                {
                    return new List<StockUploadObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StockUploadObject>();
            }
        }

        public List<StockUploadObject> GetStockUploadsByStockItem(long storeItemId)
        {
            try
            {
                var objList = _stockUploadRepository.GetStockUploadsByStockItem(storeItemId);
                if (objList == null || !objList.Any())
                {
                    return new List<StockUploadObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StockUploadObject>();
            }
        }

    }

}
