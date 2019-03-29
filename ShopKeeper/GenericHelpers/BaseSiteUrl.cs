using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopKeeper.GenericHelpers
{
    /// <summary>
    /// Helper Class for getting the Base Url of the Application
    /// </summary>
    public static class BaseSiteUrl
    {
        /// <summary>
        /// Gets the Base Url of the Application
        /// </summary>
        public static string GetBaseSiteUrl()
        {
            var context = HttpContext.Current;
            if (context.Request.ApplicationPath != null)
            {
                string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority;
                if (context.Request.Url.Port > 0)
                {
                    return baseUrl + ":" + context.Request.Url.Port;
                }
                //+ context.Request.ApplicationPath.TrimEnd('/') + '/';
                return baseUrl;
            }
            return "";
            
        }
    }
}