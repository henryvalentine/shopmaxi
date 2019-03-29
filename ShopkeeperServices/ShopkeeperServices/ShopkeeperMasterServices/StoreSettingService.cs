using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class StoreSettingServices
	{
		private readonly StoreSettingRepository  _storeSettingRepository;
        public StoreSettingServices()
		{
            _storeSettingRepository = new StoreSettingRepository();
		}
        public long AddStoreSetting(StoreSettingObject storeSetting)
		{
			try
			{
                return _storeSettingRepository.AddStoreSetting(storeSetting);
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
                return _storeSettingRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStoreSetting(StoreSettingObject storeSetting)
		{
			try
			{
                return _storeSettingRepository.UpdateStoreSetting(storeSetting);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStoreSetting(long storeSettingId)
		{
			try
			{
                return _storeSettingRepository.DeleteStoreSetting(storeSettingId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StoreSettingObject GetStoreSetting(long storeSettingId)
		{
			try
			{
                return _storeSettingRepository.GetStoreSetting(storeSettingId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
			}
		}

        public StoreSettingObject GetStoreSettingByAlias(string storeAlias)
        {
            try
            {
                return _storeSettingRepository.GetStoreSettingByAlias(storeAlias);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
            }
        }
        
        public List<StoreSettingObject> GetStoreSettings()
		{
			try
			{
                var objList = _storeSettingRepository.GetStoreSettings();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreSettingObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSettingObject>();
			}
		}
        
	  }

}
