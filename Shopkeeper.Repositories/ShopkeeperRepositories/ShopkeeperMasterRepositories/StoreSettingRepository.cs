using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class StoreSettingRepository
    {
       private readonly IShopkeeperRepository<StoreSetting> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperMasterEntities _db = new ShopKeeperMasterEntities();
       public StoreSettingRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            _db = new ShopKeeperMasterEntities(entityCnxStringBuilder);
           _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<StoreSetting>(_uoWork);
		}
        public long AddStoreSetting(StoreSettingObject storeSetting)
        {
            try
            {
                if (storeSetting == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StoreId == storeSetting.StoreId);
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeSettingEntity = ModelCrossMapper.Map<StoreSettingObject, StoreSetting>(storeSetting);
                if (storeSettingEntity == null || storeSettingEntity.StoreId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeSettingEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreSetting(StoreSettingObject storeSetting)
        {
            try
            {
                if (storeSetting == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StoreId == storeSetting.StoreId && (m.StoreId != storeSetting.StoreId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeSettingEntity = ModelCrossMapper.Map<StoreSettingObject, StoreSetting>(storeSetting);
                if (storeSettingEntity == null || storeSettingEntity.StoreId < 1)
                {
                    return -2;
                }
                _repository.Update(storeSettingEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreSetting(long storeSettingId)
        {
            try
            {
                _repository.Remove(storeSettingId);
                _uoWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreSettingObject GetStoreSetting(long storeId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreId == storeId, "Store");
                if (myItem == null || myItem.StoreId < 1)
                {
                    return new StoreSettingObject();
                }
                var storeSettingObject = ModelCrossMapper.Map<StoreSetting, StoreSettingObject>(myItem);
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

        public StoreSettingObject GetStoreSettingByAlias(string storeAlias)
        {
            try
            {
                using (var db = _db)
                {
                    var settings = (from st in db.Stores
                        where st.StoreAlias.ToLower().Trim().Contains(storeAlias.ToLower().Trim())
                        join stt in db.StoreSettings on st.StoreId equals stt.StoreId
                        
                        select new StoreSettingObject
                        {
                            StoreId = st.StoreId,
                            InitialCatalog = stt.InitialCatalog,
                            StorePsswd = stt.StorePsswd,
                            DataSource = stt.DataSource,
                            UserName = stt.UserName,
                            StoreName = st.StoreName,
                            TotalOutlets = st.TotalOutlets,
                            StoreAlias = st.StoreAlias,
                            CompanyName = st.CompanyName,
                            CustomerEmail = st.CustomerEmail,
                            SubscriptionStatus = st.SubscriptionStatus,
                            BillingCycleCode = st.BillingCycleCode,
                            StoreLogoPath = st.StoreLogoPath,
                            DefaultCurrency = st.DefaultCurrency,
                            DateCreated = st.DateCreated,
                            LastUpdated = st.LastUpdated,
                            SecreteKey = st.SecreteKey
                        }).ToList();
                   
                    return !settings.Any() ? new StoreSettingObject() : settings[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
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

        public int GetObjectCount(Expression<Func<StoreSetting, bool>> predicate)
        {
            try
            {
                return _repository.Count(predicate);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<StoreSettingObject> GetStoreSettings()
        {
            try
            {
                var storeSettingEntityList = _repository.GetAll().ToList();
                if (!storeSettingEntityList.Any())
                {
                    return new List<StoreSettingObject>();
                }
                var storeSettingObjList = new List<StoreSettingObject>();
                storeSettingEntityList.ForEach(m =>
                {
                    var storeSettingObject = ModelCrossMapper.Map<StoreSetting, StoreSettingObject>(m);
                    if (storeSettingObject != null && storeSettingObject.StoreId > 0)
                    {
                        storeSettingObjList.Add(storeSettingObject);
                    }
                });
                return storeSettingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
