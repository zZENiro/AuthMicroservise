using AuthenticationApp.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public abstract class AuthenticationManager
    {
        public IDictionary<string, string> UsersRefreshTokens { get; set; }

        protected readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public AuthenticationManager(IRefreshTokenGenerator refreshTokenGenerator) =>
            _refreshTokenGenerator = refreshTokenGenerator;

        public abstract Task<IAuthenticationResponse> Authenticate(IIdentifications identifications, HttpContext httpContext);
    }
}
