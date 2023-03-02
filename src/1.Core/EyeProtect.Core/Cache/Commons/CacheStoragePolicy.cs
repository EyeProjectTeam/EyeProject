using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Cache.Commons
{
    /// <summary>
    /// 缓存存储策略
    /// </summary>
    [Flags]
    public enum CacheStoragePolicy
    {
        /// <summary>
        /// 内存
        /// </summary>
        Memory = 1,

        /// <summary>
        /// Redis
        /// </summary>
        Redis = 2,

        /// <summary>
        /// 内存加Redis
        /// </summary>
        MemoryAndRedis = Memory | Redis,
    }
}
