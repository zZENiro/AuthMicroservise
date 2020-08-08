using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp.Jwt
{
    public class JwtAuthenticationManager : IAuthenticationManager
    {
        private readonly IOptions<AuthenticationOptions> _authOptions;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;

        public IDistributedCache RefreshTokensDictionary { get; private set; }

        public JwtAuthenticationManager(
            IRefreshTokenGenerator refreshTokenGenerator,
            IOptions<AuthenticationOptions> options,
            IDistributedCache cache,
            DistributedCacheEntryOptions distributedCacheEntryOptions)
        {
            RefreshTokensDictionary = cache;
            _distributedCacheEntryOptions = distributedCacheEntryOptions;
            _authOptions = options;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<IAuthenticationResponse> AuthenticateAsync(IIdentifications identifications) =>
            await Task.Factory.StartNew(() =>
            {
                var identity = IdentityManager.GetIdentity(identifications);

                var jwtToken = GenerateJwtTokenString(identity.Claims.ToList());
                var newRefreshToken = _refreshTokenGenerator.GenerateRefreshTokenString();

                UpdateRefreshToken(identifications.Login, newRefreshToken);

                return new JwtAuthenticationResponse()
                {
                    JwtToken = jwtToken,
                    RefreshToken = newRefreshToken
                };
            });

        public async Task<IAuthenticationResponse> AuthenticateAsync(IList<Claim> userClaims) =>
            await Task.Factory.StartNew(() =>
            {
                var jwtToken = GenerateJwtTokenString(userClaims);
                var newRefreshToken = _refreshTokenGenerator.GenerateRefreshTokenString();

                var userLogin = userClaims.Where(claim => claim.Type == "Login").FirstOrDefault().Value;

                UpdateRefreshToken(userLogin, newRefreshToken);

                return new JwtAuthenticationResponse()
                {
                    JwtToken = jwtToken,
                    RefreshToken = newRefreshToken
                };
            });

        private void UpdateRefreshToken(string login, string newRefreshToken)
        {
            if (RefreshTokensDictionary.GetString(login) is null)
                RefreshTokensDictionary.SetString(login, newRefreshToken);
            else
            {
                RefreshTokensDictionary.Remove(login);
                RefreshTokensDictionary.SetString(login, newRefreshToken);
            }
        }

        private string GenerateJwtTokenString(IList<Claim> userClaims)
        {
            var securityToken = new JwtSecurityToken(
                issuer: _authOptions.Value.Issuer,
                audience: _authOptions.Value.Audience,
                claims: /*userClaimsIdentity.Claims*/ userClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_authOptions.Value.Lifetime),
                signingCredentials: new SigningCredentials(
                    key: _authOptions.Value.GetSymmetricSecurityKey(),
                    algorithm: SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
