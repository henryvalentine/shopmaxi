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
    public class ParentMenuServices
    {
        private readonly ParentMenuItemRepository _parentMenuRepository;
        public ParentMenuServices()
        {
            _parentMenuRepository = new ParentMenuItemRepository();
        }

        public long AddParentMenu(ParentMenuObject parentMenu)
        {
            try
            {
                return _parentMenuRepository.AddParentMenu(parentMenu);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long AddParentMenuWithRoles(ParentMenuObject parentMenu)
        {
            return _parentMenuRepository.AddParentMenuWithRoles(parentMenu);
        }

        public long AddChildMenuWithRoles(ChildMenuObject childMenu)
        {
            return _parentMenuRepository.AddChildMenuWithRoles(childMenu);
        }
        public long UpdateChildMenuWithRoles(ChildMenuObject childMenu)
        {
            return _parentMenuRepository.UpdateChildMenuWithRoles(childMenu);
        }

        public long UpdateParentMenuWithRoles(ParentMenuObject parentMenu)
        {
            return _parentMenuRepository.UpdateParentMenuWithRoles(parentMenu);
        }

        public int UpdateParentMenu(ParentMenuObject parentMenu)
        {
            try
            {
                return _parentMenuRepository.UpdateParentMenu(parentMenu);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public ParentMenuObject GetMenu(int menuId)
        {
            return _parentMenuRepository.GetMenu(menuId);
        }

        public ParentMenuObject GetParentMenu(int menuId)
        {
            return _parentMenuRepository.GetParentMenu(menuId);
        }

        public List<ChildMenuObject> GetCascades(int menuId)
        {
            return _parentMenuRepository.GetCascades(menuId);
        }

        public List<ParentMenuObject> GetParentMenuList(List<string> roleIds)
        {
            try
            {
                var objList = _parentMenuRepository.GetMenuList(roleIds);
                if (objList == null || !objList.Any())
                {
                    return new List<ParentMenuObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ParentMenuObject>();
            }
        }

        public List<ParentMenuObject> GetParentMenuList(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                var objList = _parentMenuRepository.GetParentMenuList(itemsPerPage, pageNumber, out countG);
                if (objList == null || !objList.Any())
                {
                    return new List<ParentMenuObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<ParentMenuObject>();
            }
        }

        public List<ParentMenuObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _parentMenuRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<ParentMenuObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ParentMenuObject>();
            }
        }
    }

}
