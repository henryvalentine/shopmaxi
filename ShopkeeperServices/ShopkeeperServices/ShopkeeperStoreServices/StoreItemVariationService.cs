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
	public class StoreItemVariationServices
	{
		private readonly StoreItemVariationRepository  _storeItemVariationRepository;
        public StoreItemVariationServices()
		{
            _storeItemVariationRepository = new StoreItemVariationRepository();
		}
        public long AddStoreItemVariation(StoreItemVariationObject storeItemVariation)
		{
			try
			{
                return _storeItemVariationRepository.AddStoreItemVariation(storeItemVariation);
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
                return _storeItemVariationRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreItemVariation(StoreItemVariationObject storeItemVariation)
		{
			try
			{
                return _storeItemVariationRepository.UpdateStoreItemVariation(storeItemVariation);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreItemVariation(long storeItemVariationId)
		{
			try
			{
                return _storeItemVariationRepository.DeleteStoreItemVariation(storeItemVariationId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreItemVariationObject GetStoreItemVariation(long storeItemVariationId)
		{
			try
			{
                return _storeItemVariationRepository.GetStoreItemVariation(storeItemVariationId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemVariationObject();
			}
		}
        public List<StoreItemVariationObject> GetStoreItemVariations()
		{
			try
			{
                var objList = _storeItemVariationRepository.GetStoreItemVariations();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreItemVariationObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationObject>();
			}
		}
        public List<StoreItemVariationObject> GetStoreItemVariationObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemVariationRepository.GetStoreItemVariationObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemVariationObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationObject>();
            }
        }

        public List<StoreItemVariationObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemVariationRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemVariationObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationObject>();
            }
        }
	  }

}
