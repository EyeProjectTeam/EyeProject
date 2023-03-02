using EyeProtect.Core.Cache.Commons;
using EyeProtect.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core
{
    /// <summary>
    /// 缓存扩展方法
    /// </summary>
    public static class CacheExtenstions
    {
        #region ICache

        #region AsyncCache

        /// <summary>
        /// 获取或更新缓存
        /// </summary>
        public static async ValueTask<TData> GetOrAddAsync<TData>(
            this ICache<TData> cache,
            string key, Func<Task<TData>> factory,
            TimeSpan? absolute = null, TimeSpan? sliding = null
        )
        {
            var option = CreateCacheOptionsOrNull(absolute, sliding);
            return await cache.GetOrAddAsync(key, factory, option != null ? () => option : (Func<CacheEntryOptions>)null);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        public static async Task SetAsync<TData>(
            this ICache<TData> cache,
            string key, TData value,
            TimeSpan? absolute = null, TimeSpan? sliding = null
        )
        {
            var option = CreateCacheOptionsOrNull(absolute, sliding);
            await cache.SetAsync(key, value, option);
        }

        #endregion

        #endregion

        private static CacheEntryOptions CreateCacheOptionsOrNull(
            TimeSpan? absolute = null, TimeSpan? sliding = null)
        {
            if (absolute != null || sliding != null)
            {
                return new CacheEntryOptions()
                {
                    AbsoluteExpire = absolute,
                    SlidingExpire = sliding,
                };
            }

            return null;
        }
    }
}
