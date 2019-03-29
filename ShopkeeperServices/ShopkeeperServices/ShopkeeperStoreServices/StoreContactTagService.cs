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
	public class ContactTagServices
	{
		private readonly ContactTagRepository  _contactTagAccountRepository;
        public ContactTagServices()
		{
            _contactTagAccountRepository = new ContactTagRepository();
		}

        public long AddContactTag(StoreContactTagObject contactTagAccount)
		{
			try
			{
                return _contactTagAccountRepository.AddContactTag(contactTagAccount);
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
                return _contactTagAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateContactTag(StoreContactTagObject contactTagAccount)
		{
			try
			{
                return _contactTagAccountRepository.UpdateContactTag(contactTagAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteContactTag(long contactTagAccountId)
		{
			try
			{
                return _contactTagAccountRepository.DeleteContactTag(contactTagAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StoreContactTagObject GetContactTag(long contactTagAccountId)
		{
			try
			{
                return _contactTagAccountRepository.GetContactTag(contactTagAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreContactTagObject();
			}
		}

        public List<StoreContactTagObject> GetContactTags()
		{
			try
			{
                var objList = _contactTagAccountRepository.GetContactTags();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreContactTagObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactTagObject>();
			}
		}

        public List<StoreContactTagObject> GetContactTagObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _contactTagAccountRepository.GetContactTagObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreContactTagObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactTagObject>();
            }
        }

        public List<StoreContactTagObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _contactTagAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreContactTagObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactTagObject>();
            }
        }
	}

}
