using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthenticationApp;

namespace RedisSessionStoringExampleApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //services.AddSingleton<Authentication.AuthenticationManager, ClaimsAuthenticationManager>();

            services.AddDistributedRedisCache(config =>
            {
                config.Configuration = "localhost";
            });

            services.AddSession(config =>
            {
                config.Cookie.HttpOnly = false;
                config.Cookie.SameSite = SameSiteMode.Lax;
                config.Cookie.Name = "zZen.Cookies";
                config.IdleTimeout = TimeSpan.FromSeconds(120);
            });

            // add refresh token

            services.AddAntiforgery(config =>
            {
                config.Cookie.Name = "zZen.AntiforgeryCookies";
                config.Cookie.HttpOnly = true;
                config.Cookie.SameSite = SameSiteMode.Strict;

                config.FormFieldName = "AntiforgeryField";
                config.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                config.SuppressXFrameOptionsHeader = false;
            });

            services.AddAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
                {
                    cookieOptions.LoginPath = "/Admin/Login";
                    cookieOptions.LogoutPath = "/Admin/Logout";
                    cookieOptions.ExpireTimeSpan = TimeSpan.FromSeconds(20);

                    cookieOptions.Cookie.HttpOnly = true;
                    cookieOptions.Cookie.SameSite = SameSiteMode.Strict;
                    cookieOptions.Cookie.Name = "zZen.AuthCookies";
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("admin", policyCfg =>
                {
                    policyCfg.RequireRole("admin");
                    policyCfg.RequireAuthenticatedUser();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAntiforgery(antiforgery);

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action}/{param?}");
            });
        }
    }
}
