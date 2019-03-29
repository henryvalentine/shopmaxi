using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shopkeeper.DataObjects.DataObjects.Master;
using ShopkeeperServices;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;

namespace ShopKeeper.GenericHelpers
{
    public class SessionHelpers
    {
        public void SetCurrentSession(string sName, dynamic obj)
        {
            HttpContext.Current.Session["_" + sName] = obj;
        }

        public static bool GetCurrentSession(string sName)
        {
            return HttpContext.Current.Session["_" + sName] != null;
        }

        public static T GetDataFromCurrentSession<T>(string sName)
        {
            return (T)HttpContext.Current.Session["_" + sName];
        }
        
        public static void ResetCurrentSession(string sName)
        {
            HttpContext.Current.Session["_" + sName] = null;
        }

        public static bool IsUserInRoles(string r1, string r2 = null)
        {
            try
            {
                if (HttpContext.Current.Session["_userRoles"] == null)
                {
                    return false;
                }
                var userRoles = HttpContext.Current.Session["_userRoles"] as List<string>;
                if (userRoles == null || !userRoles.Any())
                {
                    return false;
                }
                if (r1 != null && r2 != null)
                {
                    if (userRoles.Contains(r1) || userRoles.Contains(r2))
                    {
                        return true;
                    }

                    if (userRoles.Contains(r1) && userRoles.Contains(r2))
                    {
                        return true;
                    }
                    return false;
                }
                if (r1 == null && r2 != null)
                {
                    if (!userRoles.Contains(r2))
                    {
                        return false;
                    }
                    return true;
                }

                if (r1 != null && r2 == null)
                {
                    if (!userRoles.Contains(r1))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreSettingObject GetStoreInfo(string subdomain)
        {
            try
            {
                if (!GetCurrentSession("mySetting")) return GetInfo(subdomain);
                var sessDom = GetDataFromCurrentSession<StoreSettingObject>("mySetting");

                return (sessDom == null || sessDom.StoreId < 1) ? GetInfo(subdomain) : sessDom;
            }
            catch (Exception)
            {
                return new StoreSettingObject();
            }

        }

        private StoreSettingObject GetInfo(string subdomain)
        {
            try
            {
                if (string.IsNullOrEmpty(subdomain))
                {
                    return new StoreSettingObject();
                }

                var domainOj = new StoreSettingServices().GetStoreSettingByAlias(subdomain);

                if (domainOj.StoreId < 1)
                {
                    return new StoreSettingObject();
                }

                var sqlConnection = DbContextSwitcherServices.SwitchSqlDatabase(domainOj);
                var entityConnection = DbContextSwitcherServices.SwitchEntityDatabase(domainOj);

                if (string.IsNullOrEmpty(sqlConnection) || string.IsNullOrEmpty(entityConnection))
                {
                    return new StoreSettingObject();
                }
                
                domainOj.EntityConnectionString = entityConnection;
                domainOj.SqlConnectionString = sqlConnection;
                domainOj.StoreLogoPath = string.IsNullOrEmpty(domainOj.StoreLogoPath) ? "/Content/images/noImage.png" : domainOj.StoreLogoPath.Replace("~", "");
                SetCurrentSession("mySetting", domainOj);

                var defaultCurrency = new StoreItemStockServices().GetStoreDefaultCurrency();
                if (defaultCurrency != null && defaultCurrency.StoreCurrencyId > 0)
                {
                    domainOj.DefaultCurrency = defaultCurrency.Name;
                    domainOj.DefaultCurrencySymbol = defaultCurrency.Symbol;
                }

                return domainOj;
            }
            catch (Exception)
            {
                return new StoreSettingObject();
            }

        }

        public StoreSettingObject GetStoreInfo()
        {
            try
            {
                if (!GetCurrentSession("mySetting")) return new StoreSettingObject();
                var sessDom = GetDataFromCurrentSession<StoreSettingObject>("mySetting");
                return (sessDom == null || sessDom.StoreId < 1) ? new StoreSettingObject() : sessDom;
            }
            catch (Exception)
            {
                return new StoreSettingObject();
            }

        }
    }
}