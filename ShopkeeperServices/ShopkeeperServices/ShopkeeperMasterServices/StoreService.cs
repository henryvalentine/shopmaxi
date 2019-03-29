using System;
using System.Collections.Generic;
using System.Linq;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using Shopkeeper.Datacontracts.Helpers;
using StoreSettingObject = Shopkeeper.DataObjects.DataObjects.Master.StoreSettingObject;
using UserProfileObject = Shopkeeper.DataObjects.DataObjects.Master.UserProfileObject;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class StoreServices
	{
		private readonly StoreRepository  _storeRepository;
        public StoreServices()
		{
            _storeRepository = new StoreRepository();
		}

        public long AddStore(StoreObject store)
		{
			try
			{
                return _storeRepository.AddStore(store);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int GetObjectCount()
        {
           return _storeRepository.GetObjectCount();
        }

        public int UpdateStore(StoreObject store)
		{
			try
			{
                return _storeRepository.UpdateStore(store);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public Int32 UpdateStore(long storeId, string logoPath)
        {
            try
            {
                return _storeRepository.UpdateStore(storeId, logoPath);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStore(long storeId)
		{
			try
			{
                return _storeRepository.DeleteStore(storeId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public long VerifyToken(string code, string userId)
        {
            try
            {
                return _storeRepository.VerifyToken(code, userId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public StoreObject GetStore(long storeId)
		{
			return _storeRepository.GetStore(storeId);
		}

        public UserProfileObject GetAdminUserProfile(string aspnetUserId)
        {
            return _storeRepository.GetAdminUserProfile(aspnetUserId);
        }

        public StoreSettingObject GetStoreSetting(string storeAlias)
        {
            try
            {
                return _storeRepository.GetStoreSetting(storeAlias);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
            }
        }

        public List<StoreObject> GetStores()
		{
			try
			{
                var objList = _storeRepository.GetStores();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreObject>();
			}
		}

        public List<StoreObject> GetStoreObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storeRepository.GetStoreObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreObject>();
            }
        }

        public List<StoreObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storeRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreObject>();
            }
        }

        //public List<StoreObject> GetStoresByCompany(int? itemsPerPage, int? pageNumber, int companyId)
        //{
        //    try
        //    {
        //        var objList = _storeRepository.GetStoresByCompany(itemsPerPage, pageNumber, companyId);
        //        if (objList == null || !objList.Any())
        //        {
        //            return new List<StoreObject>();
        //        }
        //        return objList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<StoreObject>();
        //    }
        //}

        //public List<StoreObject> GetStoresByCurrency(int? itemsPerPage, int? pageNumber, int currencyId)
        //{
        //    try
        //    {
        //        var objList = _storeRepository.GetStoresByCurrency(itemsPerPage, pageNumber, currencyId);
        //        if (objList == null || !objList.Any())
        //        {
        //            return new List<StoreObject>();
        //        }
        //        return objList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<StoreObject>();
        //    }
        //}
	}

}
