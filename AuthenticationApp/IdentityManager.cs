using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationApp
{
    public static class IdentityManager
    {
        public static ClaimsIdentity GetIdentity(IIdentifications identifications)
        {
            var identificationClaims = new List<Claim>()
            {
                new Claim("Login", identifications.Login),
                new Claim("Email", identifications.Email),
                new Claim("Password", identifications.Password),
                new Claim(ClaimTypes.Role, identifications.Role)
            };

            return new ClaimsIdentity(
                identificationClaims,
                "ClaimsCookie",
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
