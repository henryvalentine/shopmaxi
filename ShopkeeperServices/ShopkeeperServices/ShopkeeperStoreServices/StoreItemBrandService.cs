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
	public class StoreItemBrandServices
	{
		private readonly StoreItemBrandRepository  _storeItemBrandRepository;
        public StoreItemBrandServices()
		{
            _storeItemBrandRepository = new StoreItemBrandRepository();
		}
        public long AddStoreItemBrand(StoreItemBrandObject storeItemBrand)
		{
			try
			{
                return _storeItemBrandRepository.AddStoreItemBrand(storeItemBrand);
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
                return _storeItemBrandRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreItemBrand(StoreItemBrandObject storeItemBrand)
		{
			try
			{
                return _storeItemBrandRepository.UpdateStoreItemBrand(storeItemBrand);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreItemBrand(long storeItemBrandId)
		{
			try
			{
                return _storeItemBrandRepository.DeleteStoreItemBrand(storeItemBrandId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreItemBrandObject GetStoreItemBrand(long storeItemBrandId)
		{
			try
			{
                return _storeItemBrandRepository.GetStoreItemBrand(storeItemBrandId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemBrandObject();
			}
		}
        public List<StoreItemBrandObject> GetStoreItemBrands()
		{
			try
			{
                var objList = _storeItemBrandRepository.GetStoreItemBrands();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreItemBrandObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemBrandObject>();
			}
		}
        public List<StoreItemBrandObject> GetStoreItemBrandObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemBrandRepository.GetStoreItemBrandObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemBrandObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemBrandObject>();
            }
        }

        public List<StoreItemBrandObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemBrandRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemBrandObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemBrandObject>();
            }
        }
	  }

}
