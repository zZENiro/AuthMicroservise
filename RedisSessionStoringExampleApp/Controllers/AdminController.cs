
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
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AdminController(
            IDistributedCache cache,
            IAuthenticationManager authentication,
            IRefreshTokenGenerator refreshTokenGenerator,
            ITokenRefresher tokenRefresherer,
            TokenValidationParameters tokenValidationParameters)
        {
            _cache = cache;
            _authentication = authentication;
            _refreshTokenGenerator = refreshTokenGenerator;
            _tokenRefresherer = tokenRefresherer;
            _tokenValidationParameters = tokenValidationParameters;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(User user)
        {
            var resp = (JwtAuthenticationResponse)await _authentication.AuthenticateAsync(user);
            HttpContext.Response.Cookies.Append("zZen.App.Token", resp.JwtToken);
            HttpContext.Response.Cookies.Append("zZen.App.RefreshToken", resp.RefreshToken);

            return View(user);
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public IActionResult UpdateToken(IRefreshCred refreshCred)
        {
            _tokenRefresherer.Refresh(refreshCred, _tokenValidationParameters);

            return RedirectToAction("Index");
        }
    }
}
