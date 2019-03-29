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
	public class StoreItemVariationValueServices
	{
		private readonly StoreItemVariationValueRepository  _storeItemVariationValueRepository;
        public StoreItemVariationValueServices()
		{
            _storeItemVariationValueRepository = new StoreItemVariationValueRepository();
		}
        public long AddStoreItemVariationValue(StoreItemVariationValueObject storeItemVariationValue)
		{
			try
			{
                return _storeItemVariationValueRepository.AddStoreItemVariationValue(storeItemVariationValue);
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
                return _storeItemVariationValueRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreItemVariationValue(StoreItemVariationValueObject storeItemVariationValue)
		{
			try
			{
                return _storeItemVariationValueRepository.UpdateStoreItemVariationValue(storeItemVariationValue);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreItemVariationValue(long storeItemVariationValueId)
		{
			try
			{
                return _storeItemVariationValueRepository.DeleteStoreItemVariationValue(storeItemVariationValueId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreItemVariationValueObject GetStoreItemVariationValue(long storeItemVariationValueId)
		{
			try
			{
                return _storeItemVariationValueRepository.GetStoreItemVariationValue(storeItemVariationValueId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemVariationValueObject();
			}
		}
        public List<StoreItemVariationValueObject> GetStoreItemVariationValues()
		{
			try
			{
                var objList = _storeItemVariationValueRepository.GetStoreItemVariationValues();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreItemVariationValueObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationValueObject>();
			}
		}
        public List<StoreItemVariationValueObject> GetStoreItemVariationValueObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeItemVariationValueRepository.GetStoreItemVariationValueObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemVariationValueObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationValueObject>();
            }
        }

        public List<StoreItemVariationValueObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeItemVariationValueRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreItemVariationValueObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemVariationValueObject>();
            }
        }
	  }

}
