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
    public interface IAuthenticationManager
    {
        IDictionary<string, string> UsersRefreshTokens { get; set; }

        Task<IAuthenticationResponse> Authenticate(IIdentifications identifications, HttpContext httpContext);
    }
}
