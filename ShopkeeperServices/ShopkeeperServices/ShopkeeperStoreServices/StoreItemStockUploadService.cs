using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class StoreItemStockUploadServices
    {
        private readonly StoreItemStockUploadRepository _storeItemRepository;
        public StoreItemStockUploadServices()
        {
            _storeItemRepository = new StoreItemStockUploadRepository();
        }

        public List<GenericValidator> ReadExcelData(string filePath, string sheetName, long currencyId, int outletId)
        {
            return _storeItemRepository.ReadExcelData(filePath, sheetName, currencyId, outletId);
        }

        public List<long> EditExcelData(string filePath, string sheetName, ref List<long> errorList, ref string msg)
        {
            try
            {
                var objList = _storeItemRepository.EditExcelData(filePath, sheetName, ref errorList, ref msg);
                if (objList == null || !objList.Any())
                {
                    return new List<long>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<long>();
            }
        }
    }

}
