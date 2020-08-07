using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationApp.Jwt
{
    public static class JwtAuthenticationExtenstions
    {
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder) =>
            builder.Use(async (context, next) =>
            {
                var jwtToken = context.Request.Cookies["zZen.App.Token"];
                var refreshToken = context.Request.Cookies["zZen.App.RefreshToken"];
               
                if (!string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(refreshToken))
                    context.Request.Headers.Add(HeaderNames.Authorization, "Bearer " + jwtToken);

                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "DENY");

                await next.Invoke();
            });
    }
}
