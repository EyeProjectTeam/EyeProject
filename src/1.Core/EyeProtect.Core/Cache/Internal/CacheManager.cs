using EyeProtect.Core.Cache.Commons;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace EyeProtect.Core.Cache.Internal
{
    public class CacheManager : ICacheManager, ISingletonDependency
    {
        private readonly IOptions<CacheOptions> _config;
        private readonly ICacheStorage _cacheStorage;

        /// <summary>
        /// 缓存管理
        /// </summary>
        public CacheManager(IOptions<CacheOptions> config, ICacheStorage cacheStorage)
        {
            _cacheStorage = cacheStorage;
            _config = config;
        }

        /// <inheritdoc />
        public ICache<TData> GetCache<TData>(string cacheName = null)
        {
            return new Cache<TData>(_cacheStorage, _config, cacheName ?? CacheOptions.DefaultCacheName);
        }
    }
}
