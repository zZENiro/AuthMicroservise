using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApp
{
    public class IdentityManager
    {
        public ClaimsIdentity GetIdentity(IIdentifications identifications)
        {
            var identificationClaims = new List<Claim>()
            {
                new Claim("Login", identifications.Login),
                new Claim("Email", identifications.Email),
                new Claim("Password", identifications.Password)
            };

            return new ClaimsIdentity(
                identificationClaims,
                "ClaimsCookie",
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
