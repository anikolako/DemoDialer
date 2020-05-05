using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialerDemo
{
    /// <summary>
    /// This class is used for caching data. 
    /// It holds state between events in same session.
    /// </summary>
    public static class CacheManager<T>
    {
        //Concurrenct dictionary that holds state between events. It guarantess thread safety.
        private static ConcurrentDictionary<string, List<T>> CachedData = new ConcurrentDictionary<string, List<T>>();


        /// <summary>
        /// insert data into cacche dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isd"></param>
        public static void insertCachedData(string key, List<T> obj)
        {
            if (!CachedData.ContainsKey(key))
            {
                CachedData.TryAdd(key, obj);
            }
            else
            {
                List<T> oldValue;
                if (CachedData.TryGetValue(key, out oldValue))
                {
                    CachedData.TryUpdate(key, obj, oldValue);
                }
            }
        }

        /// <summary>
        /// get value from cached dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> getCahedData(string key)
        {
            if (CachedData.ContainsKey(key))
            {
                return CachedData[key] as List<T>;
            }
            return null;
        }

        /// <summary>
        /// remove data from cache.
        /// </summary>
        /// <param name="key"></param>
        public static void removeCachedData(string key)
        {
            if (CachedData.ContainsKey(key))
            {
                List<T> o;
                CachedData.TryRemove(key, out o);
            }

        }

        ///// <summary>
        ///// Search values in cache by search key.
        ///// </summary>
        ///// <param name="searchkey"></param>
        ///// <returns></returns>
        //public static List<string> searchIncSessCacheKeys(string searchkey)
        //{
        //    return CachedData
        //          .Where(x => x.Key.StartsWith(searchkey))
        //          .Select(x => x.Key).ToList();
        //}


    }
}
