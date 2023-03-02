using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Cache
{
    public interface ICacheManager
    {
        /// <summary>
        /// 获取指定名称的 <see cref="ICache{TData}"/>
        /// </summary>
        ICache<TData> GetCache<TData>(string cacheName = null);
    }
}
