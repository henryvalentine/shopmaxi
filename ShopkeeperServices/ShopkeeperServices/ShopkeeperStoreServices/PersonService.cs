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
	public class UserProfileServices
	{
		private readonly UserProfileRepository  _personRepository;
        public UserProfileServices()
		{
            _personRepository = new UserProfileRepository();
		}

        public long AddUserProfile(UserProfileObject person)
		{
			try
			{
                return _personRepository.AddUserProfile(person);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        
        public int UpdateUserProfile(UserProfileObject person)
		{
			try
			{
                return _personRepository.UpdateUserProfile(person);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteUserProfile(long personId)
		{
			try
			{
                return _personRepository.DeleteUserProfile(personId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public UserProfileObject GetUserProfile(long personId)
		{
			try
			{
                return _personRepository.GetUserProfile(personId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new UserProfileObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _personRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<UserProfileObject> GetUserProfiles()
		{
			try
			{
                var objList = _personRepository.GetUserProfiles();
                if (objList == null || !objList.Any())
			    {
                    return new List<UserProfileObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UserProfileObject>();
			}
		}

        public List<UserProfileObject> GetUserProfileObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _personRepository.GetUserProfileObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<UserProfileObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UserProfileObject>();
            }
        }

        public List<UserProfileObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _personRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<UserProfileObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UserProfileObject>();
            }
        }
	}

}
