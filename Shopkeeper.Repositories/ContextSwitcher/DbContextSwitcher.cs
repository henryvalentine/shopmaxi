using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using Shopkeeper.DataObjects.DataObjects.Master;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ContextSwitcher
{
    public static class DbContextSwitcher
    {
        public static string SwitchEntityDatabase(StoreSettingObject storeSetings)
        {
            try
            {
                
                if (string.IsNullOrEmpty(storeSetings.InitialCatalog) || string.IsNullOrWhiteSpace(storeSetings.InitialCatalog))
                {
                    return string.Empty;
                }

                var entityCnxStringBuilder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString);
                   
                var sqlCnxStringBuilder = new SqlConnectionStringBuilder(entityCnxStringBuilder.ProviderConnectionString);

                if (!string.IsNullOrEmpty(storeSetings.InitialCatalog))
                {
                    sqlCnxStringBuilder.InitialCatalog = storeSetings.InitialCatalog;
                }
                if (!string.IsNullOrEmpty(storeSetings.DataSource))
                {
                    sqlCnxStringBuilder.DataSource = storeSetings.DataSource;
                }
                if (!string.IsNullOrEmpty(storeSetings.UserName))
                {
                    sqlCnxStringBuilder.UserID = storeSetings.UserName;
                }
                if (!string.IsNullOrEmpty(storeSetings.StorePsswd))
                {
                    sqlCnxStringBuilder.Password = storeSetings.StorePsswd;
                }
                
                entityCnxStringBuilder.ProviderConnectionString = sqlCnxStringBuilder.ToString();
                return entityCnxStringBuilder.ToString();
            }

            catch (Exception ex)
            {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }


        public static string SwitchSqlDatabase(StoreSettingObject storeSetings)
        {
            try
            {

                if (string.IsNullOrEmpty(storeSetings.InitialCatalog) || string.IsNullOrWhiteSpace(storeSetings.InitialCatalog))
                {
                    return string.Empty;
                }
                 
                var cnfigManager = ConfigurationManager.ConnectionStrings["DynamicConnection"];
                var sqlCnxStringBuilder = new SqlConnectionStringBuilder(cnfigManager.ConnectionString);

                if (!string.IsNullOrEmpty(storeSetings.InitialCatalog))
                {
                    sqlCnxStringBuilder.InitialCatalog = storeSetings.InitialCatalog;
                }
                if (!string.IsNullOrEmpty(storeSetings.DataSource))
                {
                    sqlCnxStringBuilder.DataSource = storeSetings.DataSource;
                }
                if (!string.IsNullOrEmpty(storeSetings.UserName))
                {
                    sqlCnxStringBuilder.UserID = storeSetings.UserName;
                }
                if (!string.IsNullOrEmpty(storeSetings.StorePsswd))
                {
                    sqlCnxStringBuilder.Password = storeSetings.StorePsswd;
                }

                sqlCnxStringBuilder.IntegratedSecurity = true;
                sqlCnxStringBuilder.MultipleActiveResultSets = true;
                return sqlCnxStringBuilder.ToString();
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        public static StoreSettingObject GetConnectionStringParameters()
        {
            try
            {
                var sqlCnxStringBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DynamicConnection"].ConnectionString);
                if (string.IsNullOrEmpty(sqlCnxStringBuilder.ConnectionString))
                {
                    return new StoreSettingObject();
                }

                var dbScriptPath = ConfigurationManager.AppSettings["DBScriptPath"];
                if (string.IsNullOrEmpty(dbScriptPath))
                {
                    return new StoreSettingObject();
                }

                var domainExtension = ConfigurationManager.AppSettings["domainExtension"];
                if (string.IsNullOrEmpty(domainExtension))
                {
                    return new StoreSettingObject();
                }

                var k = new StoreSettingObject
                {
                    InitialCatalog = sqlCnxStringBuilder.InitialCatalog,
                    DataSource = sqlCnxStringBuilder.DataSource,
                    UserName = sqlCnxStringBuilder.UserID,
                    StorePsswd = sqlCnxStringBuilder.Password,
                    DBScriptPath = dbScriptPath,
                    SqlConnectionString = sqlCnxStringBuilder.ConnectionString,
                    DomainExtension = domainExtension
                };
                
                return k;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreSettingObject();
            }
        }

    }
}