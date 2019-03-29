using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class StateServices
	{
		private readonly StateRepository  _stateRepository;
        public StateServices()
		{
            _stateRepository = new StateRepository();
		}

        public long AddState(StateObject stateAccount)
		{
			try
			{
                return _stateRepository.AddState(stateAccount);
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

        public int UpdateState(StateObject state)
		{
			try
			{
                return _stateRepository.UpdateState(state);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteState(long stateAccountId)
		{
			try
			{
                return _stateRepository.DeleteState(stateAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StateObject GetState(long stateAccountId)
		{
			try
			{
                return _stateRepository.GetState(stateAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StateObject();
			}
		}
        
        public int GetObjectCount(Expression<Func<State, bool>> predicate)
        {
            try
            {
                return _stateRepository.GetObjectCount(predicate);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<StateObject> GetStates()
		{
			try
			{
                var objList = _stateRepository.GetStates();
                if (objList == null || !objList.Any())
			    {
                    return new List<StateObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StateObject>();
			}
		}

        public List<StateObject> GetStateObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _stateRepository.GetStateObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StateObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StateObject>();
            }
        }

        public List<StateObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _stateRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StateObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StateObject>();
            }
        }
	}

}
