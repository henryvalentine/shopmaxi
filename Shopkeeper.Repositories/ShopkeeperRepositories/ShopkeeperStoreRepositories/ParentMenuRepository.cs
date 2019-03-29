using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
    public class ParentMenuItemRepository
    {
        private readonly IShopkeeperRepository<ParentMenu> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _db;

        public ParentMenuItemRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<ParentMenu>(_uoWork);
		}

        public long AddParentMenu(ParentMenuObject parentMenu)
        {
            try
            {
                if (parentMenu == null)
                {
                    return -2;
                }

                var parentMenuEntity = ModelCrossMapper.Map<ParentMenuObject, ParentMenu>(parentMenu);
                if (parentMenuEntity == null || parentMenuEntity.MenuOrder < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(parentMenuEntity);
                _uoWork.SaveChanges();
                return returnStatus.ParentMenuId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long AddParentMenuWithRoles(ParentMenuObject parentMenu)
        {
            try
            {
                if (parentMenu == null)
                {
                    return -2;
                }

                var parentMenuEntity = ModelCrossMapper.Map<ParentMenuObject, ParentMenu>(parentMenu);
                if (parentMenuEntity == null || parentMenuEntity.MenuOrder < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    var processedEntity = db.ParentMenus.Add(parentMenuEntity);
                    db.SaveChanges();
                    if (parentMenu.ParentMenuRoleObjects.Any())
                    {
                        parentMenu.ParentMenuRoleObjects.ForEach(r =>
                        {
                            var parentMenuRoleEntity = ModelCrossMapper.Map<ParentMenuRoleObject, ParentMenuRole>(r);
                            if (parentMenuRoleEntity == null || string.IsNullOrEmpty(parentMenuRoleEntity.RoleId))
                            {
                                return;
                            }
                            parentMenuRoleEntity.ParentMenuId = processedEntity.ParentMenuId;
                            db.ParentMenuRoles.Add(parentMenuRoleEntity);
                            db.SaveChanges();
                        });
                    }

                    return processedEntity.ParentMenuId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long AddChildMenuWithRoles(ChildMenuObject childMenu)
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

                using (var db = _db)
                {
                    var processedEntity = db.ChildMenus.Add(childMenuEntity);
                    db.SaveChanges();
                    if (childMenu.ChildMenuRoleObjects.Any())
                    {
                        childMenu.ChildMenuRoleObjects.ForEach(r =>
                        {
                            var childMenuRoleEntity = ModelCrossMapper.Map<ChildMenuRoleObject, ChildMenuRole>(r);
                            if (childMenuRoleEntity == null || string.IsNullOrEmpty(childMenuRoleEntity.RoleId))
                            {
                                return;
                            }
                            childMenuRoleEntity.ChildMenuId = processedEntity.ChildMenuId;
                            db.ChildMenuRoles.Add(childMenuRoleEntity);
                            db.SaveChanges();
                        });
                    }

                    return processedEntity.ChildMenuId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long UpdateChildMenuWithRoles(ChildMenuObject childMenu)
        {
            try
            {
                if (childMenu == null || childMenu.ChildMenuId < 1)
                {
                    return -2;
                }

                var childMenuEntity = ModelCrossMapper.Map<ChildMenuObject, ChildMenu>(childMenu);
                if (childMenuEntity == null || childMenu.ChildMenuId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {
                    db.Entry(childMenuEntity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (childMenu.ChildMenuRoleObjects.Any())
                    {
                        var existingRoles = db.ChildMenuRoles.Where(m => m.ChildMenuId == childMenu.ChildMenuId).ToList();

                        childMenu.ChildMenuRoleObjects.ForEach(r =>
                        {
                            var duplicate = existingRoles.Find(x => x.RoleId == r.RoleId);
                            if (duplicate == null || duplicate.ChildMenuId < 1)
                            {
                                if (r.ChildMenuRoleId < 1)
                                {
                                    var childMenuRoleEntity = ModelCrossMapper.Map<ChildMenuRoleObject, ChildMenuRole>(r);
                                    if (childMenuRoleEntity == null || string.IsNullOrEmpty(childMenuRoleEntity.RoleId))
                                    {
                                        return;
                                    }
                                    childMenuRoleEntity.ChildMenuId = childMenu.ChildMenuId;
                                    db.ChildMenuRoles.Add(childMenuRoleEntity);
                                    db.SaveChanges();
                                }
                            }
                        });

                        if (existingRoles.Any())
                        {
                            existingRoles.ForEach(r =>
                            {
                                if (!childMenu.ChildMenuRoleObjects.Exists(h => h.RoleId == r.RoleId))
                                {
                                    db.ChildMenuRoles.Remove(r);
                                    db.SaveChanges();
                                }
                            });
                        }
                    }

                    return childMenu.ChildMenuId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateParentMenu(ParentMenuObject parentMenu)
        {
            try
            {
                if (parentMenu == null)
                {
                    return -2;
                }

                var parentMenuEntity = ModelCrossMapper.Map<ParentMenuObject, ParentMenu>(parentMenu);
                if (parentMenuEntity == null || parentMenuEntity.ParentMenuId < 1)
                {
                    return -2;
                }
                _repository.Update(parentMenuEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public long UpdateParentMenuWithRoles(ParentMenuObject parentMenu)
        {
            try
            {
                if (parentMenu == null || parentMenu.ParentMenuId < 1)
                {
                    return -2;
                }

                var parentMenuEntity = ModelCrossMapper.Map<ParentMenuObject, ParentMenu>(parentMenu);
                if (parentMenuEntity == null || parentMenu.ParentMenuId < 1)
                {
                    return -2;
                }

                using (var db = _db)
                {

                    db.Entry(parentMenuEntity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (parentMenu.ParentMenuRoleObjects.Any())
                    {
                        var existingRoles = db.ParentMenuRoles.Where(m => m.ParentMenuId == parentMenu.ParentMenuId).ToList();

                        parentMenu.ParentMenuRoleObjects.ForEach(r =>
                        {
                            if (r.ParentMenuRoleId < 1 && !existingRoles.Exists(e => e.RoleId == r.RoleId))
                            {
                                var parentMenuRoleEntity = ModelCrossMapper.Map<ParentMenuRoleObject, ParentMenuRole>(r);
                                if (parentMenuRoleEntity == null || string.IsNullOrEmpty(parentMenuRoleEntity.RoleId))
                                {
                                    return;
                                }
                                parentMenuRoleEntity.ParentMenuId = parentMenu.ParentMenuId;
                                db.ParentMenuRoles.Add(parentMenuRoleEntity);
                                db.SaveChanges();
                            }
                        });

                        if (existingRoles.Any())
                        {
                            existingRoles.ForEach(r =>
                            {
                                if (!parentMenu.ParentMenuRoleObjects.Exists(h => h.RoleId == r.RoleId))
                                {
                                    db.ParentMenuRoles.Remove(r);
                                    db.SaveChanges();
                                }
                            });
                        }
                    }

                    return parentMenu.ParentMenuId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public ParentMenuObject GetMenu(int menuId)
        {
            try
            {
                using (var db = _db)
                {
                    var myItems = db.ParentMenus.Where(x => x.ParentMenuId == menuId).Include("ParentMenuRoles").ToList();

                    if (!myItems.Any())
                    {
                        return new ParentMenuObject();
                    }

                    var parentMenu = myItems[0];
                    var childMenus = db.ChildMenus.Where(h => h.ParentMenuId == parentMenu.ParentMenuId).Include("ChildMenuRoles").ToList();

                    var parentMenuObj = ModelCrossMapper.Map<ParentMenu, ParentMenuObject>(parentMenu);
                    if (parentMenuObj != null && parentMenuObj.ParentMenuId > 0)
                    {
                        if (childMenus.Any())
                        {
                            childMenus.ForEach(m =>
                            {
                                var childMenuObj = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(m);
                                if (childMenuObj != null && childMenuObj.ChildMenuId > 0)
                                {
                                    childMenuObj.ChildMenuRoleObjects = new List<ChildMenuRoleObject>();
                                    if (m.ChildMenuRoles.Any())
                                    {
                                        m.ChildMenuRoles.ToList().ForEach(cr =>
                                        {
                                            var childRoleObj = ModelCrossMapper.Map<ChildMenuRole, ChildMenuRoleObject>(cr);
                                            if (childRoleObj != null && childRoleObj.ChildMenuId > 0)
                                            {
                                                childMenuObj.ChildMenuRoleObjects.Add(childRoleObj);
                                            }

                                        });
                                    }

                                    childMenuObj.SecondLevelChildMenuObjects = new List<ChildMenuObject>();
                                    var nest = db.ChildMenus.Where(q => q.IsParent && q.ParentChildId == childMenuObj.ChildMenuId).Include("ChildMenuRoles").ToList();
                                    if (nest.Any())
                                    {
                                        nest.ForEach(p =>
                                        {
                                            var secondLevelChildMenuObj = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(p);
                                            if (secondLevelChildMenuObj != null && secondLevelChildMenuObj.ChildMenuId > 0)
                                            {
                                                secondLevelChildMenuObj.ChildMenuRoleObjects = new List<ChildMenuRoleObject>();

                                                if (p.ChildMenuRoles.Any())
                                                {
                                                    p.ChildMenuRoles.ToList().ForEach(cr =>
                                                    {
                                                        var secondChildRoleObj = ModelCrossMapper.Map<ChildMenuRole, ChildMenuRoleObject>(cr);
                                                        if (secondChildRoleObj != null && secondChildRoleObj.ChildMenuId > 0)
                                                        {
                                                            secondLevelChildMenuObj.ChildMenuRoleObjects.Add(secondChildRoleObj);
                                                        }

                                                    });
                                                }
                                                childMenuObj.SecondLevelChildMenuObjects.Add(secondLevelChildMenuObj);
                                            }

                                        });
                                    }
                                }

                                parentMenuObj.ChildMenuObjects.Add(childMenuObj);
                            });
                        }
                    }


                    return parentMenuObj;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ParentMenuObject();
            }
        }

        public ParentMenuObject GetParentMenu(int menuId)
        {
            try
            {
                using (var db = _db)
                {
                    var myItems = db.ParentMenus.Where(x => x.ParentMenuId == menuId).ToList();

                    if (!myItems.Any())
                    {
                        return new ParentMenuObject();
                    }

                    var m = myItems[0];
                    var parentMenuObj = ModelCrossMapper.Map<ParentMenu, ParentMenuObject>(m);
                    if (parentMenuObj == null || parentMenuObj.ParentMenuId < 1)
                    {
                        return new ParentMenuObject();
                    }

                    parentMenuObj.ParentMenuRoleObjects = (from pmr in db.ParentMenuRoles
                                                           where pmr.ParentMenuId == parentMenuObj.ParentMenuId
                                                           join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                           select new ParentMenuRoleObject
                                                           {
                                                               RoleName = rr.Name,
                                                               ParentMenuId = pmr.ParentMenuId,
                                                               RoleId = rr.Id
                                                           }).ToList();

                    if (parentMenuObj.ParentMenuRoleObjects.Any())
                    {
                        parentMenuObj.ParentMenuRoleObjects.ToList().ForEach(k =>
                        {
                            parentMenuObj.RoleName += k.RoleName + ", ";
                        });
                    }

                    return parentMenuObj;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ParentMenuObject();
            }
        }

        public List<ChildMenuObject> GetCascades(int menuId)
        {
            try
            {
                using (var db = _db)
                {
                    var menuList = new List<ChildMenuObject>();

                    var childMenus = db.ChildMenus.Where(h => h.ParentMenuId == menuId).OrderBy(c => c.ChildMenuOrder).ToList();
                    if (!childMenus.Any())
                    {
                        return new List<ChildMenuObject>();
                    }
                    childMenus.ForEach(m =>
                    {
                        var childMenuObj = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(m);
                        if (childMenuObj != null && childMenuObj.ChildMenuId > 0)
                        {
                            childMenuObj.ChildMenuRoleObjects = new List<ChildMenuRoleObject>();

                            childMenuObj.ChildMenuRoleObjects = (from pmr in db.ChildMenuRoles
                                                                 where pmr.ChildMenuId == childMenuObj.ChildMenuId
                                                                 join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                                 select new ChildMenuRoleObject
                                                                 {
                                                                     ChildMenuRoleId = pmr.ChildMenuRoleId,
                                                                     RoleName = rr.Name,
                                                                     ChildMenuId = pmr.ChildMenuId,
                                                                     RoleId = rr.Id
                                                                 }).ToList();

                            if (childMenuObj.ChildMenuRoleObjects.Any())
                            {
                                childMenuObj.ChildMenuRoleObjects.ToList().ForEach(k =>
                                {
                                    childMenuObj.RoleName += k.RoleName + ", ";
                                });
                            }

                            childMenuObj.SecondLevelChildMenuObjects = new List<ChildMenuObject>();
                            var nest = db.ChildMenus.Where(q => q.IsParent && q.ParentChildId == childMenuObj.ChildMenuId).ToList();
                            if (nest.Any())
                            {
                                nest.ForEach(p =>
                                {
                                    var secondLevelChildMenuObj = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(p);
                                    if (secondLevelChildMenuObj != null && secondLevelChildMenuObj.ChildMenuId > 0)
                                    {
                                        secondLevelChildMenuObj.ChildMenuRoleObjects = new List<ChildMenuRoleObject>();

                                        secondLevelChildMenuObj.ChildMenuRoleObjects = (from pmr in db.ChildMenuRoles
                                                                                        where pmr.ChildMenuId == p.ChildMenuId
                                                                                        join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                                                        select new ChildMenuRoleObject
                                                                                        {
                                                                                            ChildMenuRoleId = pmr.ChildMenuRoleId,
                                                                                            RoleName = rr.Name,
                                                                                            ChildMenuId = pmr.ChildMenuId,
                                                                                            RoleId = rr.Id

                                                                                        }).ToList();

                                        if (secondLevelChildMenuObj.ChildMenuRoleObjects.Any())
                                        {
                                            secondLevelChildMenuObj.ChildMenuRoleObjects.ToList().ForEach(k =>
                                            {
                                                secondLevelChildMenuObj.RoleName += k.RoleName + ", ";
                                            });
                                        }

                                        childMenuObj.SecondLevelChildMenuObjects.Add(secondLevelChildMenuObj);
                                    }

                                });
                            }
                        }

                        menuList.Add(childMenuObj);
                    });

                    return menuList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChildMenuObject>();
            }
        }

        public List<ParentMenuObject> GetMenuList(List<string> roleIds)
        {
            try
            {
                using (var db = _db)
                {
                    var myItems = (from pm in db.ParentMenus.OrderBy(m => m.MenuOrder)
                                   join pmr in db.ParentMenuRoles on pm.ParentMenuId equals pmr.ParentMenuId
                                   where (roleIds.Any(x => x.Equals(pmr.RoleId)))
                                   select pm).ToList();

                    if (!myItems.Any())
                    {
                        return new List<ParentMenuObject>();
                    }

                    var menuList = new List<ParentMenuObject>();

                    myItems.ForEach(d =>
                    {
                        var parentMenu = d;
                        var childMenus =
                            (from cmr in
                                 db.ChildMenus.Where(h => h.ParentMenuId == parentMenu.ParentMenuId)
                                     .OrderBy(y => y.ChildMenuOrder)
                                     .ToList()
                             join cmrr in db.ChildMenuRoles on cmr.ChildMenuId equals cmrr.ChildMenuId
                             where (roleIds.Any(x => x.Equals(cmrr.RoleId)))
                             select cmr).ToList();

                        var parentMenuObj = ModelCrossMapper.Map<ParentMenu, ParentMenuObject>(parentMenu);

                        if (parentMenuObj != null && parentMenuObj.ParentMenuId > 0)
                        {
                            parentMenuObj.ChildMenuObjects = new List<ChildMenuObject>();

                            parentMenuObj.ParentMenuRoleObjects = (from pmr in db.ParentMenuRoles
                                                                   where pmr.ParentMenuId == parentMenuObj.ParentMenuId
                                                                   join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                                   select new ParentMenuRoleObject
                                                                   {
                                                                       RoleName = rr.Name,
                                                                       ParentMenuId = pmr.ParentMenuId,
                                                                       RoleId = rr.Id
                                                                   }).ToList();

                            if (parentMenuObj.ParentMenuRoleObjects.Any())
                            {
                                parentMenuObj.ParentMenuRoleObjects.ToList().ForEach(k =>
                                {
                                    parentMenuObj.RoleName += k.RoleName + ", ";
                                });
                            }

                            if (childMenus.Any())
                            {
                                childMenus.ForEach(m =>
                                {
                                    var childMenuObj = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(m);

                                    if (childMenuObj != null && childMenuObj.ChildMenuId > 0)
                                    {
                                        childMenuObj.ChildMenuRoleObjects = new List<ChildMenuRoleObject>();

                                        childMenuObj.ChildMenuRoleObjects = (from pmr in db.ChildMenuRoles
                                                                             where pmr.ChildMenuId == childMenuObj.ChildMenuId
                                                                             join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                                             select new ChildMenuRoleObject
                                                                             {
                                                                                 ChildMenuRoleId = pmr.ChildMenuRoleId,
                                                                                 RoleName = rr.Name,
                                                                                 ChildMenuId = pmr.ChildMenuId,
                                                                                 RoleId = rr.Id
                                                                             }).ToList();

                                        if (childMenuObj.ChildMenuRoleObjects.Any())
                                        {
                                            childMenuObj.ChildMenuRoleObjects.ToList().ForEach(k =>
                                            {
                                                childMenuObj.RoleName += k.RoleName + ", ";
                                            });
                                        }

                                        childMenuObj.SecondLevelChildMenuObjects = new List<ChildMenuObject>();
                                        var nest = db.ChildMenus.Where(q => q.IsParent && q.ParentChildId == childMenuObj.ChildMenuId).ToList();
                                        if (nest.Any())
                                        {
                                            nest.ForEach(p =>
                                            {
                                                var secondLevelChildMenuObj = ModelCrossMapper.Map<ChildMenu, ChildMenuObject>(p);
                                                if (secondLevelChildMenuObj != null && secondLevelChildMenuObj.ChildMenuId > 0)
                                                {
                                                    secondLevelChildMenuObj.ChildMenuRoleObjects = new List<ChildMenuRoleObject>();

                                                    secondLevelChildMenuObj.ChildMenuRoleObjects = (from pmr in db.ChildMenuRoles
                                                                                                    where pmr.ChildMenuId == p.ChildMenuId
                                                                                                    join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                                                                    select new ChildMenuRoleObject
                                                                                                    {
                                                                                                        ChildMenuRoleId = pmr.ChildMenuRoleId,
                                                                                                        RoleName = rr.Name,
                                                                                                        ChildMenuId = pmr.ChildMenuId,
                                                                                                        RoleId = rr.Id

                                                                                                    }).ToList();

                                                    if (secondLevelChildMenuObj.ChildMenuRoleObjects.Any())
                                                    {
                                                        secondLevelChildMenuObj.ChildMenuRoleObjects.ToList().ForEach(k =>
                                                        {
                                                            secondLevelChildMenuObj.RoleName += k.RoleName + ", ";
                                                        });
                                                    }

                                                    childMenuObj.SecondLevelChildMenuObjects.Add(secondLevelChildMenuObj);
                                                }

                                            });
                                        }
                                    }

                                    parentMenuObj.ChildMenuObjects.Add(childMenuObj);
                                });
                            }
                        }
                        menuList.Add(parentMenuObj);
                    });
                    return menuList;
                }
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
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var myItems = (db.ParentMenus.OrderBy(m => m.MenuOrder).Skip((tpageNumber) * tsize).Take(tsize)).ToList();

                        if (!myItems.Any())
                        {
                            countG = 0;
                            return new List<ParentMenuObject>();
                        }

                        countG = db.ParentMenus.Count();
                        var retList = new List<ParentMenuObject>();
                        myItems.ForEach(b =>
                        {
                            var parentMenuObj = ModelCrossMapper.Map<ParentMenu, ParentMenuObject>(b);
                            if (parentMenuObj != null && parentMenuObj.ParentMenuId > 0)
                            {
                                parentMenuObj.ParentMenuRoleObjects = (from pmr in db.ParentMenuRoles
                                                                       where pmr.ParentMenuId == b.ParentMenuId
                                                                       join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                                                       select new ParentMenuRoleObject
                                                                       {
                                                                           RoleName = rr.Name,
                                                                           ParentMenuId = pmr.ParentMenuId,
                                                                           RoleId = rr.Id
                                                                       }).ToList();

                                if (parentMenuObj.ParentMenuRoleObjects.Any())
                                {
                                    parentMenuObj.ParentMenuRoleObjects.ToList().ForEach(k =>
                                    {
                                        parentMenuObj.RoleName += k.RoleName + ", ";
                                    });
                                }
                            }

                            retList.Add(parentMenuObj);
                        });

                        return retList;
                    }

                }
                countG = 0;
                return new List<ParentMenuObject>();
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
                using (var db = _db)
                {
                    var parentMenuEntityList = (db.ParentMenus.Where(m => m.Value.ToLower().Contains(searchCriteria.ToLower()))).ToList();

                    if (!parentMenuEntityList.Any())
                    {
                        return new List<ParentMenuObject>();
                    }
                    var parentMenuObjList = new List<ParentMenuObject>();
                    parentMenuEntityList.ForEach(m =>
                    {
                        var parentMenuObject = ModelCrossMapper.Map<ParentMenu, ParentMenuObject>(m);
                        if (parentMenuObject != null && parentMenuObject.ParentMenuId > 0)
                        {
                            parentMenuObject.ParentMenuRoleObjects =
                                (from pmr in db.ParentMenuRoles
                                 where pmr.ParentMenuId == parentMenuObject.ParentMenuId
                                 join rr in db.AspNetRoles on pmr.RoleId equals rr.Id
                                 select new ParentMenuRoleObject
                                 {
                                     RoleName = rr.Name,
                                     ParentMenuId = pmr.ParentMenuId,
                                     RoleId = rr.Id
                                 }).ToList();

                            if (parentMenuObject.ParentMenuRoleObjects.Any())
                            {
                                parentMenuObject.ParentMenuRoleObjects.ToList().ForEach(k =>
                                {
                                    parentMenuObject.RoleName += k.RoleName + ", ";
                                });
                            }
                            parentMenuObjList.Add(parentMenuObject);
                        }
                    });
                    return parentMenuObjList;
                }
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ParentMenuObject>();
            }
        }
       
    }
}
