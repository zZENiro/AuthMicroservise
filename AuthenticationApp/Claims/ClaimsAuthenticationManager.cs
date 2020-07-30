using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using ModelsApp.Models.ViewModels;
using RepositoryApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public class ClaimsAuthenticationManager : AuthenticationManager
    {
        public ClaimsAuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator, IUsersRepository usersRepo)
            : base(tokenKey, refreshTokenGenerator, usersRepo) { }

        public override async Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, HttpContext httpContext) //
        {
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();

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
                UserPrincipal = usrPrincipal,
                RefreshToken = refreshToken
            };
        }

        public override async Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, Claim[] claims, HttpContext httpContext = null)
        {
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();
            var claimsList = claims.ToList();

            var usrIdentity = new ClaimsIdentity(
                claimsList,
                "zZen.AuthCookies",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var usrPrincipal = new ClaimsPrincipal(usrIdentity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usrPrincipal);

            return new ClaimsAuthenticationResponse()
            {
                Claims = claimsList,
                RefreshToken = refreshToken,
            };
        }
    }
}
