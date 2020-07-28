using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using RedisSessionStoringExampleApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public class ClaimsAuthenticationManager : AuthenticationManager
    {
        public ClaimsAuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator) : base(tokenKey, refreshTokenGenerator)
        {

        }

        public override async Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, HttpContext httpContext)
        {
            var usrClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.NameIdentifier, userCredentionals.Login),
                new Claim("pwd", userCredentionals.Password)
            };

            var usrIdentity = new ClaimsIdentity(
                usrClaims,
                "zZen.AuthCookies",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var usrPrincipal = new ClaimsPrincipal(usrIdentity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usrPrincipal);

            return new ClaimsAuthenticationResponse()
            {
                Claims = usrClaims,
                UserIdentity = usrIdentity,
                UserPrincipal = usrPrincipal
                //RefreshToken = 
            };
        }
    }
}
