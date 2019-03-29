using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class StoreSubscriptionHistoryRepository
    {
       private readonly IShopkeeperRepository<StoreSubscriptionHistory> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreSubscriptionHistoryRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<StoreSubscriptionHistory>(_uoWork);
		}
       
        public long AddStoreSubscriptionHistory(StoreSubscriptionHistoryObject storeSubscriptionHistory)
        {
            try
            {
                if (storeSubscriptionHistory == null)
                {
                    return -2;
                }
               
                var storeSubscriptionEntity = ModelCrossMapper.Map<StoreSubscriptionHistoryObject, StoreSubscriptionHistory>(storeSubscriptionHistory);
                if (storeSubscriptionEntity == null || storeSubscriptionEntity.SubscriptionPackageId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeSubscriptionEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreSubscriptionHistoryId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreSubscriptionHistory(StoreSubscriptionHistoryObject storeSubscriptionHistory)
        {
            try
            {
                if (storeSubscriptionHistory == null)
                {
                    return -2;
                }
                var storeSubscriptionEntity = ModelCrossMapper.Map<StoreSubscriptionHistoryObject, StoreSubscriptionHistory>(storeSubscriptionHistory);
                if (storeSubscriptionEntity == null || storeSubscriptionEntity.StoreSubscriptionHistoryId < 1)
                {
                    return -2;
                }
                _repository.Update(storeSubscriptionEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreSubscriptionHistory(long storeSubscriptionId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeSubscriptionId);
                _uoWork.SaveChanges();
                return returnStatus.StoreSubscriptionHistoryId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreSubscriptionHistoryObject GetStoreSubscriptionHistory(long storeSubscriptionId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreSubscriptionHistoryId == storeSubscriptionId, "Store, SubscriptionPackage");
                if (myItem == null || myItem.StoreSubscriptionHistoryId < 1)
                {
                    return new StoreSubscriptionHistoryObject();
                }
                var storeSubscriptionObject = ModelCrossMapper.Map<StoreSubscriptionHistory, StoreSubscriptionHistoryObject>(myItem);
                if (storeSubscriptionObject == null || storeSubscriptionObject.StoreSubscriptionHistoryId < 1)
                {
                    return new StoreSubscriptionHistoryObject();
                }
                storeSubscriptionObject.StoreName = myItem.Store.StoreName;
                storeSubscriptionObject.PackageTitle = myItem.SubscriptionPackage.PackageTitle;
                return storeSubscriptionObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSubscriptionHistoryObject();
            }
        }

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptionHistoryObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreSubscriptionHistory> storeSubscriptionEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeSubscriptionEntityList = _repository.GetWithPaging(m => m.StoreSubscriptionHistoryId, tpageNumber, tsize, "Store, SubscriptionPackage").ToList();
                    }

                    else
                    {
                        storeSubscriptionEntityList = _repository.GetAll("Store, SubscriptionPackage").ToList();
                    }

                    if (!storeSubscriptionEntityList.Any())
                    {
                        return new List<StoreSubscriptionHistoryObject>();
                    }
                    var storeSubscriptionObjList = new List<StoreSubscriptionHistoryObject>();
                    storeSubscriptionEntityList.ForEach(m =>
                    {
                        var storeSubscriptionObject = ModelCrossMapper.Map<StoreSubscriptionHistory, StoreSubscriptionHistoryObject>(m);
                        if (storeSubscriptionObject != null && storeSubscriptionObject.StoreSubscriptionHistoryId > 0)
                        {
                            storeSubscriptionObject.StoreName = m.Store.StoreName;
                            storeSubscriptionObject.PackageTitle = m.SubscriptionPackage.PackageTitle;
                            storeSubscriptionObjList.Add(storeSubscriptionObject);
                        }
                    });

                return storeSubscriptionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
            }
        }

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptionsByStore(int? itemsPerPage, int? pageNumber, int storeId)
        {
            try
            {
                List<StoreSubscriptionHistory> storeSubscriptionEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeSubscriptionEntityList = _repository.GetWithPaging(m => m.StoreId == storeId, m => m.StoreSubscriptionHistoryId, tpageNumber, tsize, "Store, SubscriptionPackage").ToList();
                }

                else
                {
                    storeSubscriptionEntityList = _repository.GetAll(m => m.StoreId == storeId, "Store, SubscriptionPackage").ToList();
                }

                if (!storeSubscriptionEntityList.Any())
                {
                    return new List<StoreSubscriptionHistoryObject>();
                }
                var storeSubscriptionObjList = new List<StoreSubscriptionHistoryObject>();
                storeSubscriptionEntityList.ForEach(m =>
                {
                    var storeSubscriptionObject = ModelCrossMapper.Map<StoreSubscriptionHistory, StoreSubscriptionHistoryObject>(m);
                    if (storeSubscriptionObject != null && storeSubscriptionObject.StoreSubscriptionHistoryId > 0)
                    {
                        storeSubscriptionObject.StoreName = m.Store.StoreName;
                        storeSubscriptionObject.PackageTitle = m.SubscriptionPackage.PackageTitle;
                        storeSubscriptionObjList.Add(storeSubscriptionObject);
                    }
                });

                return storeSubscriptionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
            }
        }

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptionsBySubscriptionPackage(int? itemsPerPage, int? pageNumber, int subscriptionPackageId)
        {
            try
            {
                List<StoreSubscriptionHistory> storeSubscriptionEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeSubscriptionEntityList = _repository.GetWithPaging(m => m.SubscriptionPackageId == subscriptionPackageId, m => m.StoreSubscriptionHistoryId, tpageNumber, tsize, "Store, SubscriptionPackage").ToList();
                }

                else
                {
                    storeSubscriptionEntityList = _repository.GetAll(m => m.SubscriptionPackageId == subscriptionPackageId, "Store, SubscriptionPackage").ToList();
                }

                if (!storeSubscriptionEntityList.Any())
                {
                    return new List<StoreSubscriptionHistoryObject>();
                }
                var storeSubscriptionObjList = new List<StoreSubscriptionHistoryObject>();
                storeSubscriptionEntityList.ForEach(m =>
                {
                    var storeSubscriptionObject = ModelCrossMapper.Map<StoreSubscriptionHistory, StoreSubscriptionHistoryObject>(m);
                    if (storeSubscriptionObject != null && storeSubscriptionObject.StoreSubscriptionHistoryId > 0)
                    {
                        storeSubscriptionObject.StoreName = m.Store.StoreName;
                        storeSubscriptionObject.PackageTitle = m.SubscriptionPackage.PackageTitle;
                        storeSubscriptionObjList.Add(storeSubscriptionObject);
                    }
                });

                return storeSubscriptionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreSubscriptionHistoryObject>();
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

        public List<StoreSubscriptionHistoryObject> GetStoreSubscriptions()
        {
            try
            {
                var storeSubscriptionEntityList = _repository.GetAll().ToList();
                if (!storeSubscriptionEntityList.Any())
                {
                    return new List<StoreSubscriptionHistoryObject>();
                }
                var storeSubscriptionObjList = new List<StoreSubscriptionHistoryObject>();
                storeSubscriptionEntityList.ForEach(m =>
                {
                    var storeSubscriptionObject = ModelCrossMapper.Map<StoreSubscriptionHistory, StoreSubscriptionHistoryObject>(m);
                    if (storeSubscriptionObject != null && storeSubscriptionObject.StoreSubscriptionHistoryId > 0)
                    {
                        storeSubscriptionObjList.Add(storeSubscriptionObject);
                    }
                });
                return storeSubscriptionObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}

