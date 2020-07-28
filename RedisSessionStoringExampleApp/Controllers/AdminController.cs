using AuthenticationApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisSessionStoringExampleApp.Models;
using RedisSessionStoringExampleApp.Models.ViewModels;
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
        private readonly AuthenticationManager _authManager;
        private static int _count = 0;
        private static List<User> Users = new List<User>()
        {
            new User() { Login = "login1", Password = "pwd1" },
            new User() { Login = "login2", Password = "pwd2" },
            new User() { Login = "login3", Password = "pwd3" },
            new User() { Login = "login4", Password = "pwd4" },
            new User() { Login = "login5", Password = "pwd5" },
        };

        public AdminController(IDistributedCache cache, AuthenticationManager authManager)
        {
            this._cache = cache;
            this._authManager = authManager;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return Ok("Hello, World!");
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult AddNewElement()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddNewElement([FromForm] NewElementCred newElementCred)
        {
            HttpContext.Session.SetString(newElementCred.Key, newElementCred.Value);

            return View(newElementCred);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Inspect()  
        {
            var result = new List<string>();

            foreach (var key in HttpContext.Session.Keys)
                result.Add(key);

            return new JsonResult(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Login([FromForm] NewUserCred newUserCred)
        {
            var user = Users.Where(usr => usr.Password == newUserCred.Password && usr.Login == newUserCred.Login).FirstOrDefault();
            if (user is null)
                return NotFound($"User {newUserCred.Login} not found");

            _authManager.Authenticate(newUserCred, HttpContext);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();

            return Ok("You're logout");
        }
    }
}
