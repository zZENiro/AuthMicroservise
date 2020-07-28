using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ModelsApp.Models.ViewModels;
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
    public abstract class AuthenticationManager
    {
        public abstract Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, HttpContext httpContext); //

        public virtual IDictionary<string, string> UsersRefreshTokens { get; set; }

        protected readonly string _tokenKey;

        protected readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public AuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator)
        {
            _tokenKey = tokenKey;
            this._refreshTokenGenerator = refreshTokenGenerator;
            UsersRefreshTokens = new Dictionary<string, string>();
        }

        protected string GenerateTokenString(string username, DateTime expires, Claim[] claims = null)
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
