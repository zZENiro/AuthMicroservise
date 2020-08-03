using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AuthenticationApp
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationOptions(this IServiceCollection services, AuthenticationOptions options) =>
            services.AddSingleton(typeof(AuthenticationOptions), options);
    }
}
