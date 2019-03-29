using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class ChildMenuRepository
    {
        private readonly IShopkeeperRepository<ChildMenu> _repository;
        private readonly UnitOfWork _uoWork;
        private ShopKeeperStoreEntities _db = new ShopKeeperStoreEntities();

        public ChildMenuRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<ChildMenu>(_uoWork);
		}

        public long AddChildMenu(ChildMenuObject childMenu)
        {
            try
            {
                if (childMenu == null)
                {
                    return -2;
                }

                var childMenuEntity = ModelCrossMapper.Map<ChildMenuObject, ChildMenu>(childMenu);
                if (childMenuEntity == null || childMenuEntity.ChildMenuOrder < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(childMenuEntity);
                _uoWork.SaveChanges();
                return returnStatus.ChildMenuId;
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
                if (childMenu == null)
                {
                    return -2;
                }

                var childMenuEntity = ModelCrossMapper.Map<ChildMenuObject, ChildMenu>(childMenu);
                if (childMenuEntity == null || childMenuEntity.ChildMenuId < 1)
                {
                    return -2;
                }
                _repository.Update(childMenuEntity);
                _uoWork.SaveChanges();
                return 5;
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
                var returnStatus = _repository.Remove(childMenuId);
                _uoWork.SaveChanges();
                return returnStatus.ChildMenuId > 0;
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
                var childMenuEntityList = _repository.GetAll().ToList();
                if (!childMenuEntityList.Any())
                {
                    return new List<ChildMenuObject>();
                }
                var childMenuObjList = new List<ChildMenuObject>();
                childMenuEntityList.ForEach(m =>
                {
                    var childMenuObject = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(m);
                    if (childMenuObject != null && childMenuObject.ChildMenuId > 0)
                    {
                        childMenuObjList.Add(childMenuObject);
                    }
                });
                return childMenuObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
    }
}
