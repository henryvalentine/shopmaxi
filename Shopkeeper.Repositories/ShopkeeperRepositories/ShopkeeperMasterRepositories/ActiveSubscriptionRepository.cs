using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Master;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class ActiveSubscriptionRepository
    {
        private readonly IShopkeeperRepository<ActiveSubscription> _repository;
       private readonly UnitOfWork _uoWork;

       public ActiveSubscriptionRepository()
        {
            var conn = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperStoreEntities(conn); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<ActiveSubscription>(_uoWork);
		}

       public long AddactiveSubscription(ActiveSubscriptionObject activeSubscription)
        {
            try
            {
                if (activeSubscription == null)
                {
                    return -2;
                }

                //const int stat = (int) SubscriptionStatus.Active;
                //var listToExtend = _repository.GetAll(m => m.StoreIdId == activeSubscription.StoreIdId && m.SubscriptionPackageId == activeSubscription.SubscriptionPackageId && m.SubscriptionStatus == stat).ToList();
                //var subscriptionToExtend = new ActiveSubscription();
                //if (listToExtend.Any())
                //{
                //    subscriptionToExtend = listToExtend[0];
                //}

                var activeSubscriptionEntity = ModelCrossMapper.Map<ActiveSubscriptionObject, ActiveSubscription>(activeSubscription);
                if (activeSubscriptionEntity == null || activeSubscriptionEntity.ActiveSubscriptionId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(activeSubscriptionEntity);
                _uoWork.SaveChanges();
                return returnStatus.ActiveSubscriptionId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateactiveSubscription(ActiveSubscriptionObject activeSubscription)
        {
            try
            {
                if (activeSubscription == null)
                {
                    return -2;
                }

                if (_repository.Count(m => m.Name.Trim().ToLower() == activeSubscription.Name.Trim().ToLower() && m.activeSubscriptionBrandId == activeSubscription.activeSubscriptionBrandId && m.activeSubscriptionCategoryId == activeSubscription.activeSubscriptionCategoryId && m.activeSubscriptionTypeId == activeSubscription.activeSubscriptionTypeId && (m.activeSubscriptionId != activeSubscription.activeSubscriptionId)) > 0)
                {
                    return -3;
                }
                
                var activeSubscriptionEntity = ModelCrossMapper.Map<ActiveSubscriptionObject, activeSubscription>(activeSubscription);
                if (activeSubscriptionEntity == null || activeSubscriptionEntity.activeSubscriptionId < 1)
                {
                    return -2;
                }
                _repository.Update(activeSubscriptionEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteactiveSubscription(long activeSubscriptionId)
        {
            try
            {
                var returnStatus = _repository.Remove(activeSubscriptionId);
                _uoWork.SaveChanges();
                return returnStatus.activeSubscriptionId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public ActiveSubscriptionObject GetactiveSubscription(long activeSubscriptionId)
        {
            try
            {
                var myItem = _repository.GetById(m => m.activeSubscriptionId == activeSubscriptionId, "ChartOfAccount, activeSubscriptionBrand, activeSubscriptionCategory, activeSubscriptionType");
                if (myItem == null || myItem.activeSubscriptionId < 1)
                {
                    return new ActiveSubscriptionObject();
                }
                var ActiveSubscriptionObject = ModelCrossMapper.Map<activeSubscription, ActiveSubscriptionObject>(myItem);
                if (ActiveSubscriptionObject == null || ActiveSubscriptionObject.activeSubscriptionId < 1)
                {
                    return new ActiveSubscriptionObject();
                }
                ActiveSubscriptionObject.ChartOfAccountTypeName = myItem.ChartOfAccount.AccountType;
                ActiveSubscriptionObject.activeSubscriptionCategoryName = myItem.activeSubscriptionCategory.Name;
                ActiveSubscriptionObject.activeSubscriptionTypeName = myItem.activeSubscriptionType.Name;
                ActiveSubscriptionObject.activeSubscriptionBrandName = myItem.activeSubscriptionBrand.Name;
                if (ActiveSubscriptionObject.ParentItemId > 0)
                {
                    var parentItem = _repository.GetById(ActiveSubscriptionObject.ParentItemId);

                    if (parentItem != null && parentItem.activeSubscriptionId > 0)
                    {
                        ActiveSubscriptionObject.ParentItemName = parentItem.Name;
                    }
                }
                else
                {
                    ActiveSubscriptionObject.ParentItemName = " ";
                }

                return ActiveSubscriptionObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new ActiveSubscriptionObject();
            }
        }

        public List<ActiveSubscriptionObject> GetActiveSubscriptionObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<activeSubscription> activeSubscriptionEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        activeSubscriptionEntityList = _repository.GetWithPaging(m => m.activeSubscriptionId, tpageNumber, tsize, "ChartOfAccount, activeSubscriptionBrand, activeSubscriptionCategory, activeSubscriptionType").ToList();
                    }

                    else
                    {
                        activeSubscriptionEntityList = _repository.GetAll("ChartOfAccount, activeSubscriptionBrand, activeSubscriptionCategory, activeSubscriptionType").ToList();
                    }

                    if (!activeSubscriptionEntityList.Any())
                    {
                        return new List<ActiveSubscriptionObject>();
                    }
                    var activeSubscriptionObjList = new List<ActiveSubscriptionObject>();
                    activeSubscriptionEntityList.ForEach(m =>
                    {
                        var ActiveSubscriptionObject = ModelCrossMapper.Map<activeSubscription, ActiveSubscriptionObject>(m);
                        if (ActiveSubscriptionObject != null && ActiveSubscriptionObject.activeSubscriptionId > 0)
                        {
                            ActiveSubscriptionObject.ChartOfAccountTypeName = m.ChartOfAccount.AccountType;
                            ActiveSubscriptionObject.activeSubscriptionCategoryName = m.activeSubscriptionCategory.Name;
                            ActiveSubscriptionObject.activeSubscriptionTypeName = m.activeSubscriptionType.Name;
                            ActiveSubscriptionObject.activeSubscriptionBrandName = m.activeSubscriptionBrand.Name;
                            if (ActiveSubscriptionObject.ParentItemId > 0)
                            {
                                var parentItem = new activeSubscription();
                                parentItem = activeSubscriptionEntityList.Find(x => x.activeSubscriptionId == ActiveSubscriptionObject.ParentItemId);
                                if (parentItem != null && parentItem.activeSubscriptionId > 0)
                                {
                                    ActiveSubscriptionObject.ParentItemName = parentItem.Name;
                                }
                                else
                                {
                                    parentItem = _repository.GetById(ActiveSubscriptionObject.ParentItemId);
                                    if (parentItem != null && parentItem.activeSubscriptionId > 0)
                                    {
                                        ActiveSubscriptionObject.ParentItemName = parentItem.Name;
                                    }
                                }

                            }
                            else
                            {
                                ActiveSubscriptionObject.ParentItemName = " ";
                            }

                            activeSubscriptionObjList.Add(ActiveSubscriptionObject);
                        }
                    });

                return activeSubscriptionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ActiveSubscriptionObject>();
            }
        }

        public List<ActiveSubscriptionObject> Search(string searchCriteria)
        {
            try
            {
                var activeSubscriptionEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!activeSubscriptionEntityList.Any())
                {
                    return new List<ActiveSubscriptionObject>();
                }
                var activeSubscriptionObjList = new List<ActiveSubscriptionObject>();
                activeSubscriptionEntityList.ForEach(m =>
                {
                    var ActiveSubscriptionObject = ModelCrossMapper.Map<activeSubscription, ActiveSubscriptionObject>(m);
                    if (ActiveSubscriptionObject != null && ActiveSubscriptionObject.activeSubscriptionId > 0)
                    {
                        ActiveSubscriptionObject.ChartOfAccountTypeName = m.ChartOfAccount.AccountType;
                        ActiveSubscriptionObject.activeSubscriptionCategoryName = m.activeSubscriptionCategory.Name;
                        ActiveSubscriptionObject.activeSubscriptionTypeName = m.activeSubscriptionType.Name;
                        ActiveSubscriptionObject.activeSubscriptionBrandName = m.activeSubscriptionBrand.Name;
                        if (ActiveSubscriptionObject.ParentItemId > 0)
                        {
                            var parentItem = new activeSubscription();
                            parentItem = activeSubscriptionEntityList.Find(x => x.activeSubscriptionId == ActiveSubscriptionObject.ParentItemId);
                            if (parentItem != null && parentItem.activeSubscriptionId > 0)
                            {
                                ActiveSubscriptionObject.ParentItemName = parentItem.Name;
                            }
                            else
                            {
                                parentItem = _repository.GetById(ActiveSubscriptionObject.ParentItemId);
                                if (parentItem != null && parentItem.activeSubscriptionId > 0)
                                {
                                    ActiveSubscriptionObject.ParentItemName = parentItem.Name;
                                }
                            }

                        }
                        else
                        {
                            ActiveSubscriptionObject.ParentItemName = " ";
                        }
                        activeSubscriptionObjList.Add(ActiveSubscriptionObject);
                    }
                });
                return activeSubscriptionObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ActiveSubscriptionObject>();
            }
        }

        public List<activeSubscription> SearchByStockItems(string searchCriteria)
        {
            try
            {
                return _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "activeSubscriptionStocks").ToList();
                
            }

            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<activeSubscription>();
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
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<ActiveSubscriptionObject> GetactiveSubscriptions()
        {
            try
            {
                var activeSubscriptionEntityList = _repository.GetAll().ToList();
                if (!activeSubscriptionEntityList.Any())
                {
                    return new List<ActiveSubscriptionObject>();
                }
                var activeSubscriptionObjList = new List<ActiveSubscriptionObject>();
                activeSubscriptionEntityList.ForEach(m =>
                {
                    var ActiveSubscriptionObject = ModelCrossMapper.Map<activeSubscription, ActiveSubscriptionObject>(m);
                    if (ActiveSubscriptionObject != null && ActiveSubscriptionObject.activeSubscriptionId > 0)
                    {
                        activeSubscriptionObjList.Add(ActiveSubscriptionObject);
                    }
                });
                return activeSubscriptionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
