using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreItemCategoryRepository
    {
       private readonly IShopkeeperRepository<StoreItemCategory> _repository;
       private readonly UnitOfWork _uoWork;
        private string _connectionString = string.Empty;
       public StoreItemCategoryRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _connectionString = connectionString;
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreItemCategory>(_uoWork);
		}
       
        public long AddStoreItemCategory(StoreItemCategoryObject storeItemCategory)
        {
            try
            {
                if (storeItemCategory == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == storeItemCategory.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var storeItemCategoryEntity = ModelCrossMapper.Map<StoreItemCategoryObject, StoreItemCategory>(storeItemCategory);
                if (storeItemCategoryEntity == null || string.IsNullOrEmpty(storeItemCategoryEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeItemCategoryEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemCategoryId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreItemCategory(StoreItemCategoryObject storeItemCategory)
        {
            try
            {
                if (storeItemCategory == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == storeItemCategory.Name.Trim().ToLower() && (m.StoreItemCategoryId != storeItemCategory.StoreItemCategoryId)) > 0)
                {
                    return -3;
                }
                
                var storeItemCategoryEntity = ModelCrossMapper.Map<StoreItemCategoryObject, StoreItemCategory>(storeItemCategory);
                if (storeItemCategoryEntity == null || storeItemCategoryEntity.StoreItemCategoryId < 1)
                {
                    return -2;
                }
                _repository.Update(storeItemCategoryEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItemCategory(long storeItemCategoryId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeItemCategoryId);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemCategoryId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemCategoryObject GetStoreItemCategory(long storeItemCategoryId)
        {
            try
            {
                var myItem = _repository.GetById(storeItemCategoryId);
                if (myItem == null || myItem.StoreItemCategoryId < 1)
                {
                    return new StoreItemCategoryObject();
                }
                var storeItemCategoryObject = ModelCrossMapper.Map<StoreItemCategory, StoreItemCategoryObject>(myItem);
                if (storeItemCategoryObject == null || storeItemCategoryObject.StoreItemCategoryId < 1)
                {
                    return new StoreItemCategoryObject();
                }
                return storeItemCategoryObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemCategoryObject();
            }
        }

        public List<StoreItemCategoryObject> GetStoreItemCategoryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreItemCategory> storeItemCategoryEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeItemCategoryEntityList = _repository.GetWithPaging(m => m.StoreItemCategoryId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        storeItemCategoryEntityList = _repository.GetAll().ToList();
                    }

                    if (!storeItemCategoryEntityList.Any())
                    {
                        return new List<StoreItemCategoryObject>();
                    }
                    var storeItemCategoryObjList = new List<StoreItemCategoryObject>();
                    storeItemCategoryEntityList.ForEach(m =>
                    {
                        var storeItemCategoryObject = ModelCrossMapper.Map<StoreItemCategory, StoreItemCategoryObject>(m);
                        if (storeItemCategoryObject != null && storeItemCategoryObject.StoreItemCategoryId > 0)
                        {
                            if (storeItemCategoryObject.ParentCategoryId > 0)
                            {
                                var parentCategory = new StoreItemCategory();
                                parentCategory = storeItemCategoryEntityList.Find(x => x.StoreItemCategoryId == storeItemCategoryObject.ParentCategoryId);
                                if (parentCategory != null && parentCategory.StoreItemCategoryId > 0)
                                {
                                    storeItemCategoryObject.ParentName = parentCategory.Name;
                                }
                                else
                                {
                                    parentCategory = _repository.GetById(storeItemCategoryObject.ParentCategoryId);
                                    if (parentCategory != null && parentCategory.StoreItemCategoryId > 0)
                                    {
                                        storeItemCategoryObject.ParentName = parentCategory.Name;
                                    }
                                }
                                
                            }
                            else
                            {
                                storeItemCategoryObject.ParentName = " ";
                            }
                            storeItemCategoryObjList.Add(storeItemCategoryObject);
                        }
                    });

                return storeItemCategoryObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
            }
        }

        public List<StoreItemCategoryObject> GetStoreItemCategoryObjectsWithParents(int? itemsPerPage, int? pageNumber)
        {
            try
            {

                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    List<StoreItemCategoryObject> storeItemCategoryEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                       storeItemCategoryEntityList = (from c in db.StoreItemCategories.OrderBy(m => m.StoreItemCategoryId).Skip((tpageNumber) * tsize).Take(tsize)
                                                          join x in db.StoreItemCategories on c.ParentCategoryId equals x.StoreItemCategoryId
                                                          where c.ParentCategoryId >= 0 || c.ParentCategoryId == null
                                                           select new StoreItemCategoryObject
                                                           {
                                                               Name = c.Name,
                                                               StoreItemCategoryId = c.StoreItemCategoryId,
                                                               Description = c.Description,
                                                               ImagePath = c.ImagePath,
                                                               ParentCategoryId =  c.ParentCategoryId,
                                                               ParentCategoryObject = x == null ? new StoreItemCategoryObject() : new StoreItemCategoryObject
                                                               {
                                                                   Name = x.Name,
                                                                   StoreItemCategoryId = x.StoreItemCategoryId,
                                                                   Description = x.Description,
                                                                   ImagePath = x.ImagePath,
                                                               }

                                                           }).ToList();


                    }

                    else
                    {
                        storeItemCategoryEntityList = (from c in db.StoreItemCategories
                                                       join x in db.StoreItemCategories on c.ParentCategoryId equals x.StoreItemCategoryId
                                                       where c.ParentCategoryId >= 0 || c.ParentCategoryId == null
                                                       select new StoreItemCategoryObject
                                                       {
                                                           Name = c.Name,
                                                           StoreItemCategoryId = c.StoreItemCategoryId,
                                                           Description = c.Description,
                                                           ImagePath = c.ImagePath,
                                                           ParentCategoryId = c.ParentCategoryId,
                                                           ParentCategoryObject = x == null ? new StoreItemCategoryObject() : new StoreItemCategoryObject
                                                           {
                                                               Name = c.Name,
                                                               StoreItemCategoryId = c.StoreItemCategoryId,
                                                               Description = c.Description,
                                                               ImagePath = c.ImagePath,
                                                           }
                                                       }).ToList();
                    }

                    //if (!storeItemCategoryEntityList.Any())
                    //{
                    //    return new List<StoreItemCategoryObject>();
                    //}
                    //var storeItemCategoryObjList = new List<StoreItemCategoryObject>();
                    //storeItemCategoryEntityList.ForEach(m =>
                    //{
                    //    var storeItemCategoryObject = ModelCrossMapper.Map<StoreItemCategory, StoreItemCategoryObject>(m);
                    //    if (storeItemCategoryObject != null && storeItemCategoryObject.StoreItemCategoryId > 0)
                    //    {
                    //        storeItemCategoryObjList.Add(storeItemCategoryObject);
                    //    }
                    //});

                    return storeItemCategoryEntityList ?? new List<StoreItemCategoryObject>();  
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
            }
        }
        
        public List<StoreItemCategoryObject> Search(string searchCriteria)
        {
            try
            {
                var storeItemCategoryEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storeItemCategoryEntityList.Any())
                {
                    return new List<StoreItemCategoryObject>();
                }
                var storeItemCategoryObjList = new List<StoreItemCategoryObject>();
                storeItemCategoryEntityList.ForEach(m =>
                {
                    var storeItemCategoryObject = ModelCrossMapper.Map<StoreItemCategory, StoreItemCategoryObject>(m);
                    if (storeItemCategoryObject != null && storeItemCategoryObject.StoreItemCategoryId > 0)
                    {
                        storeItemCategoryObjList.Add(storeItemCategoryObject);
                    }
                });
                return storeItemCategoryObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemCategoryObject>();
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _repository.Count();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<StoreItemCategoryObject> GetStoreItemCategories()
        {
            try
            {
                var storeItemCategoryEntityList = _repository.GetAll().ToList();
                if (!storeItemCategoryEntityList.Any())
                {
                    return new List<StoreItemCategoryObject>();
                }
                var storeItemCategoryObjList = new List<StoreItemCategoryObject>();
                storeItemCategoryEntityList.ForEach(m =>
                {
                    var storeItemCategoryObject = ModelCrossMapper.Map<StoreItemCategory, StoreItemCategoryObject>(m);
                    if (storeItemCategoryObject != null && storeItemCategoryObject.StoreItemCategoryId > 0)
                    {
                        storeItemCategoryObjList.Add(storeItemCategoryObject);
                    }
                });
                return storeItemCategoryObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
