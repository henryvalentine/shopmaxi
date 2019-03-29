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
	public class ChildMenuServices
	{
		private readonly ChildMenuRepository  _childMenuRepository;
        public ChildMenuServices()
		{
            _childMenuRepository = new ChildMenuRepository();
		}

        public long AddChildMenu(ChildMenuObject childMenu)
		{
			try
			{
                return _childMenuRepository.AddChildMenu(childMenu);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        
        public int UpdateChildMenu(ChildMenuObject childMenu)
		{
			try
			{
                return _childMenuRepository.UpdateChildMenu(childMenu);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteChildMenu(long childMenuId)
		{
			try
			{
                return _childMenuRepository.DeleteChildMenu(childMenuId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        

        public List<ChildMenuObject> GetChildMenuList()
        {
            try
            {
                var objList = _childMenuRepository.GetChildMenuList();
                if (objList == null || !objList.Any())
                {
                    return new List<ChildMenuObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChildMenuObject>();
            }
        }
        
	}

}
