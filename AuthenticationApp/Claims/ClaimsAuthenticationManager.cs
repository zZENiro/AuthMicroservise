using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationApp.Claims
{
    public class ClaimsAuthenticationManager : AuthenticationManager
    {
        public ClaimsAuthenticationManager(IRefreshTokenGenerator refreshTokenGenerator)
            : base(refreshTokenGenerator) { }

        public override async Task<IAuthenticationResponse> Authenticate(IIdentifications identifications, HttpContext httpContext)
        {
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();

            var usrClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.NameIdentifier, identifications.Login),
                new Claim("pwd", identifications.Password)
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
    }
}
