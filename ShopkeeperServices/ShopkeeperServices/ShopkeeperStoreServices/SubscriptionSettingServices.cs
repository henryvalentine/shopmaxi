using System;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class SubscriptionSettingServices
	{
        private readonly SubscriptionSettingRepository _subscriptionSettingRepository;
        public SubscriptionSettingServices(string connectionString)
		{
            _subscriptionSettingRepository = new SubscriptionSettingRepository(connectionString);
		}

        public long AddSubscriptionSetting(SubscriptionSettingObject subscriptionSettingAccount)
		{
			try
			{
                return _subscriptionSettingRepository.AddSubscriptionSetting(subscriptionSettingAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        
        public int UpdateSubscriptionSetting(SubscriptionSettingObject subscriptionSetting)
		{
			try
			{
                return _subscriptionSettingRepository.UpdateSubscriptionSetting(subscriptionSetting);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

	}

}
