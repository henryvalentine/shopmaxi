using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopKeeper.Master.EF.Models.Master;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using StoreSettingObject = Shopkeeper.DataObjects.DataObjects.Master.StoreSettingObject;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class StoreRepository
    {
       private readonly IShopkeeperRepository<Store> _repository;
       private readonly UnitOfWork _uoWork;
       private ShopKeeperMasterEntities _db = new ShopKeeperMasterEntities();
        public StoreRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            _db = new ShopKeeperMasterEntities(entityCnxStringBuilder);
            _uoWork = new UnitOfWork(_db);
            _repository = new ShopkeeperRepository<Store>(_uoWork);
		}
       
        public long AddStore(StoreObject store)
        {
            try
            {
                if (store == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StoreName.Trim().ToLower().Equals(store.StoreName.Trim().ToLower()) || m.StoreAlias.Trim().ToLower().Equals(store.StoreAlias.Trim().ToLower()) || m.CompanyName.Trim().ToLower().Equals(store.CompanyName.Trim().ToLower()));
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeEntity = ModelCrossMapper.Map<StoreObject, Store>(store);
                if (storeEntity == null || string.IsNullOrEmpty(store.StoreName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreId;
            }

            catch (DbEntityValidationException ex)
            {
                var str = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
            
        }

        public Int32 UpdateStore(StoreObject store)
        {
            try
            {
                if (store == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StoreName.Trim().ToLower().Equals(store.StoreName.Trim().ToLower()) || m.StoreAlias.Trim().ToLower().Equals(store.StoreAlias.Trim().ToLower()) || m.CompanyName.Trim().ToLower().Equals(store.StoreAlias.Trim().ToLower()) && m.StoreId != store.StoreId);
                if (duplicates > 0)
                {
                    return -3;
                }
                
                var storeEntity = ModelCrossMapper.Map<StoreObject, Store>(store);
                if (storeEntity == null || storeEntity.StoreId < 1)
                {
                    return -2;
                }
                _repository.Update(storeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public Int32 UpdateStore(long storeId, string logoPath)
        {
            try
            {
                if (string.IsNullOrEmpty(logoPath))
                {
                    return -2;
                }
                var store = _repository.GetById(storeId);
                if (store == null || store.StoreId < 1)
                {
                    return -2;
                }

                store.StoreLogoPath = logoPath;
                _repository.Update(store);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStore(long storeId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeId);
                _uoWork.SaveChanges();
                return returnStatus.StoreId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        public long VerifyToken(string code, string userId)
        {
            try
            {
                using (var db = _db)
                {
                    var stores = (from st in db.Stores.Where(m => m.SecreteKey.Trim() == code.Trim())
                        join sh in db.StoreSubscriptionHistories on st.StoreId equals sh.StoreId
                        select new
                        {
                           st, sh
                        }).ToList();

                    if (!stores.Any())
                    {
                        return 0;
                    }
                    var store = stores[0].st;
                    var history = stores[0].sh;
                    if (store.StoreId < 1)
                    {
                        return 0;
                    }

                    if (history.StoreSubscriptionHistoryId < 1)
                    {
                        return 0;
                    }

                    if (history.SubscriptionPackageId == (int) SubscriptionPackageInfo.Trial)
                    {
                        store.SubscriptionStatus = (int) SubscriptionStatus.Active;
                        db.Stores.Attach(store);
                        db.Entry(store).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return store.StoreId;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public StoreObject GetStore(long storeId)
        {
            try
            {
                var myItem = _repository.GetById(storeId);
                if (myItem == null || myItem.StoreId < 1)
                {
                    return new StoreObject();
                }
                var storeObject = ModelCrossMapper.Map<Store, StoreObject>(myItem);
                if (storeObject == null || storeObject.StoreId < 1)
                {
                    return new StoreObject();
                }
                
                
                return storeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreObject();
            }
        }

        public StoreSettingObject GetStoreSetting(string storeAlias)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreAlias.Trim().ToLower() == storeAlias, "StoreSetting");
                if (myItem == null || myItem.StoreId < 1)
                {
                    return new StoreSettingObject();
                }

                if (myItem.StoreSetting == null || myItem.StoreSetting.StoreId < 1)
                {
                    return new StoreSettingObject();
                }

                var storeSettingObject = ModelCrossMapper.Map<StoreSetting, StoreSettingObject>(myItem.StoreSetting);
                if (storeSettingObject == null || storeSettingObject.StoreId < 1)
                {
                    return new StoreSettingObject();
                }
                return storeSettingObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
            }
        }

        public List<StoreObject> GetStoreObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<Store> storeEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeEntityList = _repository.GetWithPaging(m => m.StoreId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        storeEntityList = _repository.GetAll().ToList();
                    }

                    if (!storeEntityList.Any())
                    {
                        return new List<StoreObject>();
                    }
                    var storeObjList = new List<StoreObject>();
                    storeEntityList.ForEach(m =>
                    {
                        var storeObject = ModelCrossMapper.Map<Store, StoreObject>(m);
                        if (storeObject != null && storeObject.StoreId > 0)
                        {
                            storeObjList.Add(storeObject);
                        }
                    });

                return storeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreObject>();
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

        public DataObjects.DataObjects.Master.UserProfileObject GetAdminUserProfile(string aspnetUserId)
        {
            try
            {
                using (var db = _db)
                {
                    var profileList = (from asp in db.AspNetUsers.Where(m => m.Id == aspnetUserId)
                                       join ps in db.UserProfiles on asp.UserInfo_Id equals ps.Id
                                       select new DataObjects.DataObjects.Master.UserProfileObject
                                       {
                                           Id = ps.Id,
                                           Name = ps.LastName + " " + ps.OtherNames,
                                           PhotofilePath = ps.PhotofilePath
                                       }).ToList();

                    if (!profileList.Any())
                    {
                        return new DataObjects.DataObjects.Master.UserProfileObject();
                    }

                    var profile = profileList[0];
                    return profile;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new DataObjects.DataObjects.Master.UserProfileObject();
            }
        }

        public List<StoreObject> GetStores()
        {
            try
            {
                var storeEntityList = _repository.GetAll().ToList();
                if (!storeEntityList.Any())
                {
                    return new List<StoreObject>();
                }
                var storeObjList = new List<StoreObject>();
                storeEntityList.ForEach(m =>
                {
                    var storeObject = ModelCrossMapper.Map<Store, StoreObject>(m);
                    if (storeObject != null && storeObject.StoreId > 0)
                    {
                        storeObjList.Add(storeObject);
                    }
                });
                return storeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        public List<StoreObject> Search(string searchCriteria)
        {
            try
            {
                long bcode;
                var res = long.TryParse(searchCriteria, out bcode);
                var storeEntityList = res ? _repository.GetAll(m => m.StoreName.Contains(searchCriteria) || m.StoreAlias.Contains(searchCriteria) || m.CompanyName.Trim().ToLower().Contains(searchCriteria)).ToList() : _repository.GetAll(m => m.StoreName.Contains(searchCriteria) || m.StoreAlias.Contains(searchCriteria) || m.CompanyName.Trim().ToLower().Contains(searchCriteria)).ToList();
                
                if (!storeEntityList.Any())
                {
                    return new List<StoreObject>();
                }
                var storeObjList = new List<StoreObject>();
                storeEntityList.ForEach(m =>
                {
                    var storeObject = ModelCrossMapper.Map<Store, StoreObject>(m);
                    if (storeObject != null && storeObject.StoreId > 0)
                    {
                        storeObjList.Add(storeObject);
                    }
                });
                return storeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreObject>();
            }
        }
    }
}

