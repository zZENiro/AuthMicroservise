
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Controllers
{
    [Controller]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "admin")]
    public class AdminController : Controller
    {
        private readonly IDistributedCache _cache;
        private static int _count = 0;

        public AdminController(IDistributedCache cache)
        {
            this._cache = cache;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return Ok("Hello, World!");
        }
    }
}
