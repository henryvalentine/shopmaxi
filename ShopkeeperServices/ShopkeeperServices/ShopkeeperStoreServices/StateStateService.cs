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
    public class StoreStateServices
    {
        private readonly StoreStateRepository _stateRepository;
        public StoreStateServices()
        {
            _stateRepository = new StoreStateRepository();
        }

        public long AddStoreState(StoreStateObject stateAccount)
        {
            try
            {
                return _stateRepository.AddStoreState(stateAccount);
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
                return _stateRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreState(StoreStateObject state)
        {
            try
            {
                return _stateRepository.UpdateStoreState(state);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreState(long stateAccountId)
        {
            try
            {
                return _stateRepository.DeleteStoreState(stateAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreStateObject GetStoreState(long stateAccountId)
        {
            try
            {
                return _stateRepository.GetStoreState(stateAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreStateObject();
            }
        }

        //public int GetObjectCount(Expression<Func<StoreState, bool>> predicate)
        //{
        //    try
        //    {
        //        return _stateRepository.GetObjectCount(predicate);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return 0;
        //    }
        //}

        public List<StoreStateObject> GetStoreStates()
        {
            try
            {
                var objList = _stateRepository.GetStoreStates();
                if (objList == null || !objList.Any())
                {
                    return new List<StoreStateObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreStateObject>();
            }
        }

        public List<StoreStateObject> GetStoreStateObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _stateRepository.GetStoreStateObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreStateObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreStateObject>();
            }
        }

        public List<StoreStateObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _stateRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreStateObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreStateObject>();
            }
        }
    }

}
