using AuthenticationApp.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
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
        IDistributedCache RefreshTokensDictionary { get; }

        Task<IAuthenticationResponse> AuthenticateAsync(IIdentifications identifications);

        Task<IAuthenticationResponse> AuthenticateAsync(IList<Claim> userClaims);
    }
}
