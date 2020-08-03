﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp.Jwt
{
    public class JwtAuthenticationManager : IAuthenticationManager
    {
        private readonly AuthenticationOptions _authOptions;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public IDistributedCache RefreshTokensDictionary { get; private set; }
        
        public JwtAuthenticationManager(
            IRefreshTokenGenerator refreshTokenGenerator, 
            AuthenticationOptions options,
            IDistributedCache cache)
        {
            RefreshTokensDictionary = cache;
            _authOptions = options;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<IAuthenticationResponse> AuthenticateAsync(IIdentifications identifications) =>
            await Task.Factory.StartNew(() =>
            {
                var jwtToken = GenerateJwtTokenString(identifications);
                var newRefreshToken = _refreshTokenGenerator.GenerateRefreshTokenString();

                UpdateUsersRefreshTokens(identifications.Login, newRefreshToken);

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

                UpdateUsersRefreshTokens(userLogin, newRefreshToken);

                return new JwtAuthenticationResponse()
                {
                    JwtToken = jwtToken,
                    RefreshToken = newRefreshToken
                };
            });

        private void UpdateUsersRefreshTokens(string login, string newRefreshToken)
        {
            if (RefreshTokensDictionary.GetString(login) is null)
                RefreshTokensDictionary.SetString(login, newRefreshToken);
            else
            {
                RefreshTokensDictionary.Remove(login);
                RefreshTokensDictionary.SetString(login, newRefreshToken);
            }    
        }

        private string GenerateJwtTokenString(IIdentifications identifications)
        {
            var identity = IdentityManager.GetIdentity(identifications);

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

        private string GenerateJwtTokenString(IList<Claim> userClaims)
        {
            var securityToken = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                claims: userClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_authOptions.Lifetime),
                signingCredentials: new SigningCredentials(
                    key: _authOptions.GetSymmetricSecurityKey(),
                    algorithm: SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
