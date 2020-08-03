using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedCacheEntryOptions(this IServiceCollection services, DistributedCacheEntryOptions options) =>
            services.AddSingleton(typeof(DistributedCacheEntryOptions), options);
    }
}
