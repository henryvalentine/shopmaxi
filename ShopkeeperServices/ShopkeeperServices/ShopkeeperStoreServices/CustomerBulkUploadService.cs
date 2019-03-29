using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class CustomerBulkUploadServices
	{
        private readonly BulkCustomerRepository _customerBulkRepository;
        public CustomerBulkUploadServices()
		{
            _customerBulkRepository = new BulkCustomerRepository();
		}

        public List<GenericValidator> ReadExcelData(string filePath, string sheetName)
        {
            return _customerBulkRepository.ReadExcelData(filePath, sheetName);
        }

        public List<GenericValidator> EditExcelData(string filePath, string sheetName, ref List<long> errorList, ref string msg)
        {
            try
            {
                var objList = _customerBulkRepository.ReadExcelData(filePath, sheetName);
                if (objList == null || !objList.Any())
                {
                    return new List<GenericValidator>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<GenericValidator>();
            }
        }
	  }

}
