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
	public class StoreItemCategoryServices
	{
		private readonly StoreItemCategoryRepository  _storeItemCategoryRepository;
        public StoreItemCategoryServices()
		{
            _storeItemCategoryRepository = new StoreItemCategoryRepository();
		}
        public long AddStoreItemCategory(StoreItemCategoryObject storeItemCategory)
		{
			try
			{
                return _storeItemCategoryRepository.AddStoreItemCategory(storeItemCategory);
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
                return _storeItemCategoryRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreItemCategory(StoreItemCategoryObject storeItemCategory)
		{
			try
			{
                return _storeItemCategoryRepository.UpdateStoreItemCategory(storeItemCategory);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreItemCategory(long storeItemCategoryId)
		{
			try
			{
                return _storeItemCategoryRepository.DeleteStoreItemCategory(storeItemCategoryId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreItemCategoryObject GetStoreItemCategory(long storeItemCategoryId)
		{
			try
			{
                return _storeItemCategoryRepository.GetStoreItemCategory(storeItemCategoryId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemCategoryObject();
			}
		}
        public List<StoreItemCategoryObject> GetStoreItemCategories()
		{
			try
			{
                var objList = _storeItemCategoryRepository.GetStoreItemCategories();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreItemCategoryObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
			}
		}
        public List<StoreItemCategoryObject> GetStoreItemCategoryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemCategoryRepository.GetStoreItemCategoryObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemCategoryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
            }
        }

        public List<StoreItemCategoryObject> GetStoreItemCategoryObjectsWithParents(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemCategoryRepository.GetStoreItemCategoryObjectsWithParents(itemsPerPage, pageNumber);

                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemCategoryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
            }
        }

        public List<StoreItemCategoryObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemCategoryRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemCategoryObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
            }
        }
	  }

}
