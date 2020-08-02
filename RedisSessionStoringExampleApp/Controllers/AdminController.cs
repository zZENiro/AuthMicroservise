
using AuthenticationApp;
using AuthenticationApp.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RedisSessionStoringExampleApp.Models;
using RepositoriesApp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Controllers
{
    [Controller]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "admin")]
    public class AdminController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IAuthenticationManager _authentication;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ITokenRefresher _tokenRefresherer;
        private readonly IRepository<User> _repository;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly CookieOptions _authTokenCookieOptions;
        private readonly CookieOptions _refreshTokenCookieOptions;
        private readonly AuthenticationApp.AuthenticationOptions _authenticationOptions;

        public AdminController(
            IDistributedCache cache,
            IAuthenticationManager authentication,
            IRefreshTokenGenerator refreshTokenGenerator,
            ITokenRefresher tokenRefresherer,
            //IRepository<User> repository,
            TokenValidationParameters tokenValidationParameters,
            AuthenticationApp.AuthenticationOptions authenticationOptions)
        {
            _cache = cache;
            _authentication = authentication;
            _refreshTokenGenerator = refreshTokenGenerator;
            _tokenRefresherer = tokenRefresherer;
            //_repository = repository;
            _tokenValidationParameters = tokenValidationParameters;
            _authenticationOptions = authenticationOptions;

            _authTokenCookieOptions = new CookieOptions()
            {
                Domain = "localhost",
                Path = "/Admin/",
                Expires = DateTimeOffset.Now.AddMinutes(_authenticationOptions.Lifetime),
                SameSite = SameSiteMode.Strict,
                Secure = true,
                HttpOnly = true
            };

            _refreshTokenCookieOptions = new CookieOptions()
            {
                Domain = "localhost",
                Path = "/Admin/",
                Expires = DateTimeOffset.Now.AddDays(365),
                SameSite = SameSiteMode.Strict,
                Secure = true,
                HttpOnly = true
            };
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return Ok("Hello, World!");
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            string? refreshToken = HttpContext.Request.Cookies["zZen.App.RefreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
                return await UpdateToken(refreshToken);

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(User user)
        {
            var resp = (JwtAuthenticationResponse)await _authentication.AuthenticateAsync(user);

            _cache.SetStringAsync(resp.RefreshToken, resp.JwtToken, 
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365) });

            HttpContext.Response.Cookies.Append("zZen.App.Token", resp.JwtToken, _authTokenCookieOptions);
            HttpContext.Response.Cookies.Append("zZen.App.RefreshToken", resp.RefreshToken, _refreshTokenCookieOptions);

            return RedirectToAction("Index");
        }

        // TODO: Плодятся старые refreshToken-ы
        private async Task<IActionResult> UpdateToken(string refreshToken)
        {
            var jwtToken = await _cache.GetStringAsync(refreshToken);

            var resp = (JwtAuthenticationResponse)await _tokenRefresherer.Refresh(
                new JwtRefreshCred() { JwtToken = jwtToken, JwtRefreshToken = refreshToken }, 
                _tokenValidationParameters);

            _cache.SetStringAsync(resp.RefreshToken, resp.JwtToken,
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365) });

            HttpContext.Response.Cookies.Append("zZen.App.Token", resp.JwtToken, _authTokenCookieOptions);
            HttpContext.Response.Cookies.Append("zZen.App.RefreshToken", resp.RefreshToken, _refreshTokenCookieOptions);

            return RedirectToAction("Index");
        }
    }
}
