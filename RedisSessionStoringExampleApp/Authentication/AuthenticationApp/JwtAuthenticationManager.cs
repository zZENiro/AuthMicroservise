using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using RedisSessionStoringExampleApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public class JwtAuthenticationManager : AuthenticationManager
    {
        public JwtAuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator) 
            : base(tokenKey, refreshTokenGenerator) 
        { }

        public override async Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, HttpContext httpContext = null)
        {
            if (!users.Any(u => u.Key == userCredentionals.Login && u.Value == password))
            {
                return null;
            }

            var token = GenerateTokenString(username, DateTime.UtcNow);
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();

            if (UsersRefreshTokens.ContainsKey(username))
            {
                UsersRefreshTokens[username] = refreshToken;
            }
            else
            {
                UsersRefreshTokens.Add(username, refreshToken);
            }

            return new JwtAuthenticationResponse()
            {
                JwtToken = token,
                RefreshToken = refreshToken
            };
        }
    }
}
