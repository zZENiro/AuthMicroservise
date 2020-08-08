using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApp;
using AuthenticationApp.Jwt;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RedisSessionStoringExampleApp.Data;
using RedisSessionStoringExampleApp.Data.UsersRepository;
using RedisSessionStoringExampleApp.Models;

namespace RedisSessionStoringExampleApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        private TokenValidationParameters _tokenValidationParameters;
        private DistributedCacheEntryOptions _distributedCacheEntryOptions;
        private UserDbContext _dbContext;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(Configuration.GetValue<string>("authOptions:Key"))),
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetValue<string>("authOptions:Issuer"),
                ValidateAudience = true,
                ValidAudience = Configuration.GetValue<string>("authOptions:Audience"),
                ValidateLifetime = true,
            };

            _distributedCacheEntryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
            };

            _dbContext = new UserDbContext(Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // TOOD: make configuring Auth options by json
            services.AddOptions<AuthenticationOptions>("authOptions");

            services.AddCors(setup =>
                setup.AddPolicy("SecureCors", config =>
                {
                    config.AllowAnyHeader().AllowCredentials().AllowAnyMethod();
                }));

            services.AddEFUsersRepository(_dbContext);

            services.AddDistributedRedisCache(config =>
            {
                config.Configuration = "localhost";
            });
            services.AddDistributedCacheEntryOptions(_distributedCacheEntryOptions);

            services.AddSession(config =>
            {
                config.Cookie.HttpOnly = false;
                config.Cookie.SameSite = SameSiteMode.Lax;
                config.Cookie.Name = "zZen.Cookies";
                config.IdleTimeout = TimeSpan.FromSeconds(120);
            });

            services.AddAntiforgery(config =>
            {
                config.Cookie = new CookieBuilder()
                {
                    Path = "/",
                    Domain = "localhost",
                    HttpOnly = true,
                    Name = "zZen.Tokens.Antiforgery",
                    SameSite = SameSiteMode.Strict,
                    Expiration = TimeSpan.FromMinutes(60)
                };

                config.FormFieldName = "AntiforgeryField";
                config.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                config.SuppressXFrameOptionsHeader = false;
            });

            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieConfig =>
                {
                    cookieConfig.LoginPath = "/Admin/Login";
                    cookieConfig.LogoutPath = "/Admin/Logout";
                    cookieConfig.AccessDeniedPath = "/Admin/Login";
                    cookieConfig.Cookie.HttpOnly = true;
                    cookieConfig.Cookie.SameSite = SameSiteMode.Strict;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.Audience = Configuration.GetValue<string>("authOptions:Audience");
                    options.SaveToken = true;
                    options.TokenValidationParameters = _tokenValidationParameters;
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("admin", policyCfg =>
                {
                    policyCfg.RequireRole("admin");
                    policyCfg.RequireAuthenticatedUser();
                });
            });

            // TODO: make configuring valid from json
            services.AddTokenValidation(_tokenValidationParameters);

            services.AddJwtRefreshTokenGenerator();

            services.AddJwtAuthenticationManager(_distributedCacheEntryOptions);

            services.AddJwtTokenRefresher();
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
            app.UseCors("SecureCors");

            // angular
            //app.UseAntiforgery(antiforgery);

            app.UseJwtAuthentication();

            app.UseStatusCodePages(async context =>
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    response.Redirect("/Admin/Login");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

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