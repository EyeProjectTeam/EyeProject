using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Cache
{
    /// <summary>
    /// 缓存存储类
    /// </summary>
    public interface ICacheStorage
    {
        /// <summary>
        /// 获取内存缓存
        /// </summary>
        IMemoryCache GetMemoryCache();

        /// <summary>
        /// 获取Redis缓存
        /// </summary>
        IDatabase GetRedisCache();

        /// <summary>
        /// 广播缓存过期
        /// </summary>
        void PublishCacheExpired(string key);

        /// <summary>
        /// 广播缓存过期
        /// </summary>
        Task PublishCacheExpiredAsync(string key);
    }
}
