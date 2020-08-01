using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationApp.Jwt
{
    public static class JwtAuthenticationHandler
    {
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder) =>
            builder.Use(async (context, next) =>
            {
                var jwtToken = context.Request.Cookies["zZen.App.Token"];
                var refreshToken = context.Request.Cookies["zZen.App.RefreshToken"];

                if (string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(refreshToken))
                    context.Response.Redirect("/Admin/UpdateToken");
                else if (string.IsNullOrEmpty(jwtToken) && string.IsNullOrEmpty(refreshToken))
                    context.Response.Redirect("/Admin/Login");

                context.Request.Headers.Add(HeaderNames.Authorization, "Bearer " + jwtToken);

                await next.Invoke();
            });
    }
}
