using System;
using System.Runtime.Caching;

namespace ShopKeeper
{
    public static class UserContext
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        // add to cache
        //AddToCache<string>(username, value);

        // get from cache

         //string value = GetFromCache<string>(username);
         //if (value != null)
         //{
         //    // got item, do something with it.
         //}
         //else
         //{
         //   // item does not exist in cache.
         //}
        
        public static void AddToCache<T>(string token, T item) where T : class
        {
	        Cache.AddOrGetExisting(token, item, DateTime.Now.AddMinutes(15));
        }

        public static T GetFromCache<T>(string cacheKey) where T : class
        {
	        try
	        {
		        return (T)Cache[cacheKey];
	        }
	        catch
	        {
		        return null;
	        }
        }

        public static void  RemoveFromCache(string cacheKey) //todo : Remember to implement at LogOff
        {
            try
            {
                Cache.Remove(cacheKey);
            }
            catch
            {
                return;
            }
        }
    }
}
