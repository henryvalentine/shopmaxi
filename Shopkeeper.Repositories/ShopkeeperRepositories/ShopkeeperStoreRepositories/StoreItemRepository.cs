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
    public class StoreItemRepository
    {
       private readonly IShopkeeperRepository<StoreItem> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreItemRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreItem>(_uoWork);
		}
       
        public long AddStoreItem(StoreItemObject storeItem)
        {
            try
            {
                if (storeItem == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == storeItem.Name.Trim().ToLower() && m.StoreItemBrandId == storeItem.StoreItemBrandId && m.StoreItemCategoryId == storeItem.StoreItemCategoryId && m.StoreItemTypeId == storeItem.StoreItemTypeId) > 0)
                {
                    return -3;
                }
                var storeItemEntity = ModelCrossMapper.Map<StoreItemObject, StoreItem>(storeItem);
                if (storeItemEntity == null || string.IsNullOrEmpty(storeItemEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeItemEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreItem(StoreItemObject storeItem)
        {
            try
            {
                if (storeItem == null)
                {
                    return -2;
                }

                if (_repository.Count(m => m.Name.Trim().ToLower() == storeItem.Name.Trim().ToLower() && m.StoreItemBrandId == storeItem.StoreItemBrandId && m.StoreItemCategoryId == storeItem.StoreItemCategoryId && m.StoreItemTypeId == storeItem.StoreItemTypeId && (m.StoreItemId != storeItem.StoreItemId)) > 0)
                {
                    return -3;
                }
                
                var storeItemEntity = ModelCrossMapper.Map<StoreItemObject, StoreItem>(storeItem);
                if (storeItemEntity == null || storeItemEntity.StoreItemId < 1)
                {
                    return -2;
                }
                _repository.Update(storeItemEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreItem(long storeItemId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeItemId);
                _uoWork.SaveChanges();
                return returnStatus.StoreItemId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreItemObject GetStoreItem(long storeItemId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreItemId == storeItemId, "ChartOfAccount, StoreItemBrand, StoreItemCategory, StoreItemType");
                if (myItem == null || myItem.StoreItemId < 1)
                {
                    return new StoreItemObject();
                }
                var storeItemObject = ModelCrossMapper.Map<StoreItem, StoreItemObject>(myItem);
                if (storeItemObject == null || storeItemObject.StoreItemId < 1)
                {
                    return new StoreItemObject();
                }
                storeItemObject.ChartOfAccountTypeName = myItem.ChartOfAccount.AccountType;
                storeItemObject.StoreItemCategoryName = myItem.StoreItemCategory.Name;
                storeItemObject.StoreItemTypeName = myItem.StoreItemType.Name;
                storeItemObject.StoreItemBrandName = myItem.StoreItemBrand.Name;
                if (storeItemObject.ParentItemId > 0)
                {
                    var parentItem = _repository.GetById(storeItemObject.ParentItemId);

                    if (parentItem != null && parentItem.StoreItemId > 0)
                    {
                        storeItemObject.ParentItemName = parentItem.Name;
                    }
                }
                else
                {
                    storeItemObject.ParentItemName = " ";
                }

                return storeItemObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreItemObject();
            }
        }

        public List<StoreItemObject> GetStoreItemObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreItem> storeItemEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeItemEntityList = _repository.GetWithPaging(m => m.StoreItemId, tpageNumber, tsize, "ChartOfAccount, StoreItemBrand, StoreItemCategory, StoreItemType").ToList();
                    }

                    else
                    {
                        storeItemEntityList = _repository.GetAll("ChartOfAccount, StoreItemBrand, StoreItemCategory, StoreItemType").ToList();
                    }

                    if (!storeItemEntityList.Any())
                    {
                        return new List<StoreItemObject>();
                    }
                    var storeItemObjList = new List<StoreItemObject>();
                    storeItemEntityList.ForEach(m =>
                    {
                        var storeItemObject = ModelCrossMapper.Map<StoreItem, StoreItemObject>(m);
                        if (storeItemObject != null && storeItemObject.StoreItemId > 0)
                        {
                            storeItemObject.ChartOfAccountTypeName = m.ChartOfAccount.AccountType;
                            storeItemObject.StoreItemCategoryName = m.StoreItemCategory.Name;
                            storeItemObject.StoreItemTypeName = m.StoreItemType.Name;
                            storeItemObject.StoreItemBrandName = m.StoreItemBrand.Name;
                            if (storeItemObject.ParentItemId > 0)
                            {
                                var parentItem = new StoreItem();
                                parentItem = storeItemEntityList.Find(x => x.StoreItemId == storeItemObject.ParentItemId);
                                if (parentItem != null && parentItem.StoreItemId > 0)
                                {
                                    storeItemObject.ParentItemName = parentItem.Name;
                                }
                                else
                                {
                                    parentItem = _repository.GetById(storeItemObject.ParentItemId);
                                    if (parentItem != null && parentItem.StoreItemId > 0)
                                    {
                                        storeItemObject.ParentItemName = parentItem.Name;
                                    }
                                }

                            }
                            else
                            {
                                storeItemObject.ParentItemName = " ";
                            }

                            storeItemObjList.Add(storeItemObject);
                        }
                    });

                return storeItemObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemObject>();
            }
        }

        public List<StoreItemObject> GetStoreItemObjects()
        {
            try
            {
                var storeItemEntityList = _repository.GetWithPaging(m => m.StoreItemId, 0, 20, "ChartOfAccount, StoreItemBrand, StoreItemCategory, StoreItemType").ToList();
                if (!storeItemEntityList.Any())
                {
                    return new List<StoreItemObject>();
                }
                var storeItemObjList = new List<StoreItemObject>();
                storeItemEntityList.ForEach(m =>
                {
                    var storeItemObject = ModelCrossMapper.Map<StoreItem, StoreItemObject>(m);
                    if (storeItemObject != null && storeItemObject.StoreItemId > 0)
                    {
                        storeItemObject.ChartOfAccountTypeName = m.ChartOfAccount.AccountType;
                        storeItemObject.StoreItemCategoryName = m.StoreItemCategory.Name;
                        storeItemObject.StoreItemTypeName = m.StoreItemType.Name;
                        storeItemObject.StoreItemBrandName = m.StoreItemBrand.Name;
                        if (storeItemObject.ParentItemId > 0)
                        {
                            var parentItem = new StoreItem();
                            parentItem = storeItemEntityList.Find(x => x.StoreItemId == storeItemObject.ParentItemId);
                            if (parentItem != null && parentItem.StoreItemId > 0)
                            {
                                storeItemObject.ParentItemName = parentItem.Name;
                            }
                            else
                            {
                                parentItem = _repository.GetById(storeItemObject.ParentItemId);
                                if (parentItem != null && parentItem.StoreItemId > 0)
                                {
                                    storeItemObject.ParentItemName = parentItem.Name;
                                }
                            }

                        }
                        else
                        {
                            storeItemObject.ParentItemName = " ";
                        }

                        storeItemObjList.Add(storeItemObject);
                    }
                });

                return storeItemObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemObject>();
            }
        }

        public List<StoreItemObject> Search(string searchCriteria)
        {
            try
            {
                var storeItemEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storeItemEntityList.Any())
                {
                    return new List<StoreItemObject>();
                }
                var storeItemObjList = new List<StoreItemObject>();
                storeItemEntityList.ForEach(m =>
                {
                    var storeItemObject = ModelCrossMapper.Map<StoreItem, StoreItemObject>(m);
                    if (storeItemObject != null && storeItemObject.StoreItemId > 0)
                    {
                        storeItemObject.ChartOfAccountTypeName = m.ChartOfAccount.AccountType;
                        storeItemObject.StoreItemCategoryName = m.StoreItemCategory.Name;
                        storeItemObject.StoreItemTypeName = m.StoreItemType.Name;
                        storeItemObject.StoreItemBrandName = m.StoreItemBrand.Name;
                        if (storeItemObject.ParentItemId > 0)
                        {
                            var parentItem = new StoreItem();
                            parentItem = storeItemEntityList.Find(x => x.StoreItemId == storeItemObject.ParentItemId);
                            if (parentItem != null && parentItem.StoreItemId > 0)
                            {
                                storeItemObject.ParentItemName = parentItem.Name;
                            }
                            else
                            {
                                parentItem = _repository.GetById(storeItemObject.ParentItemId);
                                if (parentItem != null && parentItem.StoreItemId > 0)
                                {
                                    storeItemObject.ParentItemName = parentItem.Name;
                                }
                            }

                        }
                        else
                        {
                            storeItemObject.ParentItemName = " ";
                        }
                        storeItemObjList.Add(storeItemObject);
                    }
                });
                return storeItemObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItemObject>();
            }
        }

        public List<StoreItem> SearchByStockItems(string searchCriteria)
        {
            try
            {
                return _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "StoreItemStocks").ToList();
                
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreItem>();
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

        public List<StoreItemObject> GetStoreItems()
        {
            try
            {
                var storeItemEntityList = _repository.GetAll().ToList();
                if (!storeItemEntityList.Any())
                {
                    return new List<StoreItemObject>();
                }
                var storeItemObjList = new List<StoreItemObject>();
                storeItemEntityList.ForEach(m =>
                {
                    var storeItemObject = ModelCrossMapper.Map<StoreItem, StoreItemObject>(m);
                    if (storeItemObject != null && storeItemObject.StoreItemId > 0)
                    {
                        storeItemObjList.Add(storeItemObject);
                    }
                });
                return storeItemObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
