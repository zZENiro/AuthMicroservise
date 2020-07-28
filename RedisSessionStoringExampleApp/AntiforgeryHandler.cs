using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp
{
    public static class AntiforgeryHandler
    {
        public static IApplicationBuilder UseAntiforgery(this IApplicationBuilder builder, IAntiforgery antiforgery) =>
            builder.Use(async (context, next) =>
            {


                await next.Invoke();
            });
    }
}
