using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shopkeeper.DataObjects.DataObjects.Master;

namespace Shopkeeper.Repositories.Utilities
{
    public class SessionHelpers
    {
        public static bool GetCurrentSession(string sName)
        {
            return HttpContext.Current.Session["_" + sName] != null;
        }

        public static T GetDataFromCurrentSession<T>(string sName)
        {
            return (T)HttpContext.Current.Session["_" + sName];
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