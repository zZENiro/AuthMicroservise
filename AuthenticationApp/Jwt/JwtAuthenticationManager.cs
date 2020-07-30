using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ModelsApp.Models.ViewModels;
using RepositoryApplication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp.Jwt
{
    public class JwtAuthenticationManager : AuthenticationManager
    {   
        public JwtAuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator, IUsersRepository usersRepository)
            : base(tokenKey, refreshTokenGenerator, usersRepository) { }

        public override async Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, HttpContext httpContext = null)
        {
            var users = (await _usersRepo.GetAllAsync()).ToList();

            if (!users.Any(u => u.Login == userCredentionals.Login && u.Password == userCredentionals.Password))
                return null;

            var jwtToken = GenerateJwtTokenString(userCredentionals.Login, DateTime.UtcNow);
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();

            if (UsersRefreshTokens.ContainsKey(userCredentionals.Login))
                UsersRefreshTokens[userCredentionals.Login] = refreshToken;
            else
                UsersRefreshTokens.Add(userCredentionals.Login, refreshToken);

            return new JwtAuthenticationResponse()
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public override async Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, Claim[] claims, HttpContext httpContext = null)
        {
            var token = GenerateJwtTokenString(userCredentionals.Login, DateTime.UtcNow, claims);
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();

            if (UsersRefreshTokens.ContainsKey(userCredentionals.Login))
                UsersRefreshTokens[userCredentionals.Login] = refreshToken;
            else
                UsersRefreshTokens.Add(userCredentionals.Login, refreshToken);

            return new JwtAuthenticationResponse
            {
                JwtToken = token,
                RefreshToken = refreshToken
            };
        }

        private string GenerateJwtTokenString(string username, DateTime expires, Claim[] claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims ?? new Claim[] { new Claim(ClaimTypes.Name, username) }),
                Expires = expires.AddMinutes(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
