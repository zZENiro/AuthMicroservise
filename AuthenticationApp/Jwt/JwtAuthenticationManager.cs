using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp.Jwt
{
    public class JwtAuthenticationManager : IAuthenticationManager
    {
        private readonly AuthenticationOptions _authOptions;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public IDictionary<string, string> UsersRefreshTokens { get; set; }

        public JwtAuthenticationManager(IRefreshTokenGenerator refreshTokenGenerator, AuthenticationOptions options)
        {
            _authOptions = options;
            _refreshTokenGenerator = refreshTokenGenerator;
            UsersRefreshTokens = new Dictionary<string, string>();
        }

        public async Task<IAuthenticationResponse> Authenticate(IIdentifications identifications, HttpContext httpContext = null)
        {
            var jwtToken = GenerateJwtTokenString(identifications);
            var refreshToken = _refreshTokenGenerator.GenerateRefreshTokenString();

            if (UsersRefreshTokens.ContainsKey(identifications.Login))
                UsersRefreshTokens[identifications.Login] = refreshToken;
            else
                UsersRefreshTokens.Add(identifications.Login, refreshToken);

            return new JwtAuthenticationResponse()
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateJwtTokenString(IIdentifications identifications)
        {
            var identity = new IdentityManager().GetIdentity(identifications);

            var securityToken = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                claims: identity.Claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_authOptions.Lifetime),
                signingCredentials: new SigningCredentials(
                    key: _authOptions.GetSymmetricSecurityKey(),
                    algorithm: SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
