using System;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ContextSwitcher;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices
{
	public static class DbContextSwitcherServices 
	{
        public static string SwitchEntityDatabase(StoreSettingObject storeSetings)
		{
			try
			{
                return DbContextSwitcher.SwitchEntityDatabase(storeSetings);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return string.Empty;
			}
		}

        public static string SwitchSqlDatabase(StoreSettingObject storeSetings)
        {
            try
            {
                return DbContextSwitcher.SwitchSqlDatabase(storeSetings);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return string.Empty;
            }
        }

        public static StoreSettingObject GetConnectionStringParameters()
        {
            try
            {
                return DbContextSwitcher.GetConnectionStringParameters();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
            }
        }
	}
}
