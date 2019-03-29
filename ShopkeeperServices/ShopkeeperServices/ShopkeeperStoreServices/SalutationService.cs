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
	public class SalutationServices
	{
		private readonly SalutationRepository  _salutationRepository;
        public SalutationServices()
		{
            _salutationRepository = new SalutationRepository();
		}

        public long AddSalutation(SalutationObject salutationAccount)
		{
			try
			{
                return _salutationRepository.AddSalutation(salutationAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        
        public int UpdateSalutation(SalutationObject salutation)
		{
			try
			{
                return _salutationRepository.UpdateSalutation(salutation);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteSalutation(long salutationAccountId)
		{
			try
			{
                return _salutationRepository.DeleteSalutation(salutationAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public SalutationObject GetSalutation(long salutationAccountId)
		{
			try
			{
                return _salutationRepository.GetSalutation(salutationAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SalutationObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _salutationRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<SalutationObject> GetSalutations()
		{
			try
			{
                var objList = _salutationRepository.GetSalutations();
                if (objList == null || !objList.Any())
			    {
                    return new List<SalutationObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalutationObject>();
			}
		}

        public List<SalutationObject> GetSalutationObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _salutationRepository.GetSalutationObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SalutationObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalutationObject>();
            }
        }

        public List<SalutationObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _salutationRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<SalutationObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalutationObject>();
            }
        }
	}

}
