using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
                string path = context.Request.Path.Value;

                if (
                    string.Equals(path, "/Admin", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(path, "/Admin/Index", StringComparison.OrdinalIgnoreCase))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = true });
                }

                await next.Invoke();
            });
    }
}
