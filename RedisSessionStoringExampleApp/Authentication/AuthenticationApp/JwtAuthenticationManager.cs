using Microsoft.AspNetCore.Http;
using ModelsApp.Models.ViewModels;
using System;
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
            //if (!users.Any(u => u.Key == userCredentionals.Login && u.Value == password))
            //{
            //    return null;
            //}

            var token = GenerateTokenString(userCredentionals.Login, DateTime.UtcNow);
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();

            //if (UsersRefreshTokens.ContainsKey(username))
            //{
            //    UsersRefreshTokens[username] = refreshToken;
            //}
            //else
            //{
            //    UsersRefreshTokens.Add(username, refreshToken);
            //}

            return new JwtAuthenticationResponse()
            {
                JwtToken = token,
                RefreshToken = refreshToken
            };
        }
    }
}
