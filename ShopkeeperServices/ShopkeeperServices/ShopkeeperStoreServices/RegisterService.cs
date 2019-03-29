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
	public class RegisterServices
	{
		private readonly RegisterRepository  _registerRepository;
        public RegisterServices()
		{
            _registerRepository = new RegisterRepository();
		}

        public long AddRegister(RegisterObject register)
		{
			try
			{
                return _registerRepository.AddRegister(register);
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
                return _registerRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateRegister(RegisterObject register)
		{
			try
			{
                return _registerRepository.UpdateRegister(register);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteRegister(long registerId)
		{
			try
			{
                return _registerRepository.DeleteRegister(registerId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public RegisterObject GetRegister(long registerId)
		{
			try
			{
                return _registerRepository.GetRegister(registerId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new RegisterObject();
			}
		}

        public List<RegisterObject> GetCities()
		{
			try
			{
                var objList = _registerRepository.GetRegisters();
                if (objList == null || !objList.Any())
			    {
                    return new List<RegisterObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<RegisterObject>();
			}
		}

        public List<RegisterObject> GetRegisterObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _registerRepository.GetRegisterObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<RegisterObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<RegisterObject>();
            }
        }

        public List<RegisterObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _registerRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<RegisterObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<RegisterObject>();
            }
        }
	}

}
