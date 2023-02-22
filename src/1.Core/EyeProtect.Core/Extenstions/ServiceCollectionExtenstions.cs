using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtenstions
    {
        /// <summary>
        /// 配置 AutoMapper 
        /// </summary>
        public static IServiceCollection ConfigMapper(this IServiceCollection service, Action<IMapperConfigurationExpression> config)
        {
            var accessor = service.GetObjectOrNull<AutoMapperConfig>();
            if (accessor == null)
                service.AddObjectAccessor(accessor = new AutoMapperConfig());
            accessor.MapActions.Add(config);

            return service;
        }
    }

    internal class AutoMapperConfig
    {
        public List<Action<IMapperConfigurationExpression>> MapActions { get; }
            = new List<Action<IMapperConfigurationExpression>>();
    }
}
