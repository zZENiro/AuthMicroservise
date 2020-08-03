
using AuthenticationApp;
using AuthenticationApp.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.IdentityModel.Tokens;
using RedisSessionStoringExampleApp.Data.UsersRepository;
using RedisSessionStoringExampleApp.Models;
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
        private readonly IUsersRepository _repository;
        private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly CookieOptions _authTokenCookieOptions;
        private readonly CookieOptions _refreshTokenCookieOptions;
        private readonly AuthenticationApp.AuthenticationOptions _authenticationOptions;

        public AdminController(
            IDistributedCache cache,
            IAuthenticationManager authentication,
            IRefreshTokenGenerator refreshTokenGenerator,
            ITokenRefresher tokenRefresherer,
            IUsersRepository repository,
            TokenValidationParameters tokenValidationParameters,
            AuthenticationApp.AuthenticationOptions authenticationOptions,
            DistributedCacheEntryOptions distributedCacheEntryOptions)
        {
            _cache = cache;
            _authentication = authentication;
            _refreshTokenGenerator = refreshTokenGenerator;
            _tokenRefresherer = tokenRefresherer;
            _repository = repository;
            _tokenValidationParameters = tokenValidationParameters;
            _authenticationOptions = authenticationOptions;
            _distributedCacheEntryOptions = distributedCacheEntryOptions;

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
            var requireUser = await _repository.GetOneAsync(user.Login);
            if (requireUser is null)
                return Unauthorized();

            var resp = (JwtAuthenticationResponse)await _authentication.AuthenticateAsync(user);

            AddTokensToCache(resp);

            return RedirectToAction("Index");
        }

        private async Task<IActionResult> UpdateToken(string refreshToken)
        {
            var jwtToken = await _cache.GetStringAsync(refreshToken);

            var resp = await _tokenRefresherer.Refresh(
                new JwtRefreshCred()
                {
                    JwtToken = jwtToken,
                    JwtRefreshToken = refreshToken
                },
                _tokenValidationParameters) as JwtAuthenticationResponse;

            UpdateCachedTokens(resp, refreshToken);

            return RedirectToAction("Index");
        }


        private void UpdateCachedTokens(JwtAuthenticationResponse response, string oldRefreshToken)
        {
            _cache.RemoveAsync(oldRefreshToken);
            AddTokensToCache(response);
        }

        private void AddTokensToCache(JwtAuthenticationResponse response)
        {
            _cache.SetStringAsync(response.RefreshToken, response.JwtToken, _distributedCacheEntryOptions);

            HttpContext.Response.Cookies.Append("zZen.App.Token", response.JwtToken, _authTokenCookieOptions);
            HttpContext.Response.Cookies.Append("zZen.App.RefreshToken", response.RefreshToken, _refreshTokenCookieOptions);
        }
    }
}
