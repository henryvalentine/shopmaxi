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
	public class AlaCarteServices
	{
		private readonly AlaCarteRepository  _alaCarteAccountRepository;
        public AlaCarteServices()
		{
            _alaCarteAccountRepository = new AlaCarteRepository();
		}

        public long AddAlaCarte(AlaCarteObject alaCarteAccount)
		{
			try
			{
                return _alaCarteAccountRepository.AddAlaCarte(alaCarteAccount);
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
                return _alaCarteAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateAlaCarte(AlaCarteObject alaCarteAccount)
		{
			try
			{
                return _alaCarteAccountRepository.UpdateAlaCarte(alaCarteAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteAlaCarte(long alaCarteAccountId)
		{
			try
			{
                return _alaCarteAccountRepository.DeleteAlaCarte(alaCarteAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public AlaCarteObject GetAlaCarte(long alaCarteAccountId)
		{
			try
			{
                return _alaCarteAccountRepository.GetAlaCarte(alaCarteAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new AlaCarteObject();
			}
		}

        public List<AlaCarteObject> GetAlaCartes()
		{
			try
			{
                var objList = _alaCarteAccountRepository.GetAlaCartes();
                if (objList == null || !objList.Any())
			    {
                    return new List<AlaCarteObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AlaCarteObject>();
			}
		}

        public List<AlaCarteObject> GetAlaCarteObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _alaCarteAccountRepository.GetAlaCarteObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<AlaCarteObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AlaCarteObject>();
            }
        }

        public List<AlaCarteObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _alaCarteAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<AlaCarteObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AlaCarteObject>();
            }
        }
	}

}
