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
	public class SupplierServices
	{
		private readonly SupplierRepository  _supplierAccountRepository;
        public SupplierServices()
		{
            _supplierAccountRepository = new SupplierRepository();
		}

        public long AddSupplier(SupplierObject supplierAccount)
		{
			try
			{
                return _supplierAccountRepository.AddSupplier(supplierAccount);
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
                return _supplierAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSupplier(SupplierObject supplierAccount)
		{
			try
			{
                return _supplierAccountRepository.UpdateSupplier(supplierAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteSupplier(long supplierAccountId)
		{
			try
			{
                return _supplierAccountRepository.DeleteSupplier(supplierAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public SupplierObject GetSupplier(long supplierAccountId)
		{
			try
			{
                return _supplierAccountRepository.GetSupplier(supplierAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SupplierObject();
			}
		}

        public List<SupplierObject> GetSuppliers()
		{
			try
			{
                var objList = _supplierAccountRepository.GetSuppliers();
                if (objList == null || !objList.Any())
			    {
                    return new List<SupplierObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SupplierObject>();
			}
		}

        public List<SupplierObject> GetSupplierObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _supplierAccountRepository.GetSupplierObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SupplierObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SupplierObject>();
            }
        }

        public List<SupplierObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _supplierAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<SupplierObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SupplierObject>();
            }
        }
	}

}
