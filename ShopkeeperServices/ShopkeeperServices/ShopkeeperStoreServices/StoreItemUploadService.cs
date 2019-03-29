using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class StoreItemUploadServices
	{
		private readonly StoreItemUploadRepository  _storeItemRepository;
        public StoreItemUploadServices()
		{
            _storeItemRepository = new StoreItemUploadRepository();
		}

        public List<long> ReadItemsExcelData(string filePath, string sheetName, ref List<long> errorList, ref string msg)
        {
            try
            {
                var objList = _storeItemRepository.ReadExcelData(filePath, sheetName, ref errorList, ref msg);
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
