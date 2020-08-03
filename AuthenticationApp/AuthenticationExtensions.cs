using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AuthenticationApp
{
    public static class AuthenticationExtensions
    {
        //public static IApplicationBuilder UseRedirectionUnauthorizedUsers(this IApplicationBuilder app) =>
        //    app.Use(async (context, next) =>
        //    {
        //        var response = context.Response;

        //        if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
        //            response.Redirect("/Admin/Login");

        //        await next();
        //    });
    }
}
