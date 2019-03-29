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
    public class SubscriptionSettingRepository
    {
       private readonly IShopkeeperRepository<SubscriptionSetting> _repository;
       private readonly UnitOfWork _uoWork;

        public SubscriptionSettingRepository(string connectionStringSetting)
        {
            //var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;

           var shopkeeperMasterContext = new ShopKeeperStoreEntities(connectionStringSetting); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<SubscriptionSetting>(_uoWork);
		}

        public long AddSubscriptionSetting(SubscriptionSettingObject store)
        {
            try
            {
                if (store == null)
                {
                    return -2;
                }
                var settings = _repository.GetAll().ToList();
                if (settings.Any())
                {
                    var setting = settings[0];
                    setting.StoreId = store.StoreId;
                    setting.SecreteKey = store.SecreteKey;
                    setting.DatabaseSpace = store.DatabaseSpace;
                    setting.FileStorageSpace = store.FileStorageSpace;
                    setting.Url = store.Url;
                    setting.DateSubscribed = store.DateSubscribed;
                    setting.ExpiryDate = store.ExpiryDate;
                    setting.SubscriptionStatus = store.SubscriptionStatus;
                    _repository.Update(setting);
                    _uoWork.SaveChanges();
                    return setting.StoreId;
                }
                var storeEntity = ModelCrossMapper.Map<SubscriptionSettingObject, SubscriptionSetting>(store);
                if (storeEntity == null || store.StoreId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreId;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateSubscriptionSetting(SubscriptionSettingObject store)
        {
            try
            {
                if (store == null)
                {
                    return -2;
                }

                var settings = _repository.GetAll().ToList();
                if (settings.Any())
                {
                    var setting = settings[0];
                    setting.StoreId = store.StoreId;
                    setting.SecreteKey = store.SecreteKey;
                    setting.DatabaseSpace = store.DatabaseSpace;
                    setting.FileStorageSpace = store.FileStorageSpace;
                    setting.Url = store.Url;
                    setting.DateSubscribed = store.DateSubscribed;
                    setting.ExpiryDate = store.ExpiryDate;
                    setting.SubscriptionStatus = store.SubscriptionStatus;
                    _repository.Update(setting);
                    _uoWork.SaveChanges();
                    return 5;
                }

                var storeEntity = ModelCrossMapper.Map<SubscriptionSettingObject, SubscriptionSetting>(store);
                if (storeEntity == null || store.StoreId < 1)
                {
                    return -2;
                }
                _repository.Add(storeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }
    }
}

