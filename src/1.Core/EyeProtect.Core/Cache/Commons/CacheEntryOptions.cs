using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Cache.Commons
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheEntryOptions
    {
        private TimeSpan? _absoluteExpire;
        private TimeSpan? _slidingExpire;

        /// <summary>
        /// 决对过期时间
        /// </summary>
        public TimeSpan? AbsoluteExpire
        {
            get => _absoluteExpire;
            set
            {
                if (value.HasValue && value.Value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(AbsoluteExpire), value, "过期时间无效");
                _absoluteExpire = value;
            }
        }

        /// <summary>
        /// 滑动过期时间
        /// </summary>
        public TimeSpan? SlidingExpire
        {
            get => _slidingExpire;
            set
            {
                if (value.HasValue && value.Value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(SlidingExpire), value, "过期时间无效");
                _slidingExpire = value;
            }
        }
    }

    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheEntryConfigOptions : CacheEntryOptions
    {
        /// <summary>
        /// 缓存存储策略
        /// </summary>
        public CacheStoragePolicy StoragePolicy { get; set; } = CacheStoragePolicy.MemoryAndRedis;
    }
}
