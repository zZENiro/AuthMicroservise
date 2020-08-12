using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationApp.Jwt
{
    public static class JwtAuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtRefreshTokenGenerator(this IServiceCollection services) =>
            services.AddSingleton<IRefreshTokenGenerator, JwtRefreshTokenGenerator>();

        public static IServiceCollection AddJwtAuthenticationManager(this IServiceCollection services) =>
            services.AddSingleton<IAuthenticationManager>(impl =>
            new JwtAuthenticationManager(
                impl.GetService<IRefreshTokenGenerator>(),
                impl.GetService<IOptions<AuthenticationOptions>>(),
                impl.GetService<IDistributedCache>(),
                impl.GetService<DistributedCacheEntryOptions>())); //

        public static IServiceCollection AddJwtTokenRefresher(this IServiceCollection services) =>
            services.AddSingleton<ITokenRefresher>(impl =>
            new JwtTokenRefresher(
                impl.GetService<IAuthenticationManager>())
            );
    }
}
