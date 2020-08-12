using AuthenticationApp.Jwt;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp.Jwt
{
    public class JwtTokenRefresher : ITokenRefresher
    {
        private readonly IAuthenticationManager _JwtAuthenticationManager;

        public JwtTokenRefresher(
            IAuthenticationManager jWTAuthenticationManager)
        {
            _JwtAuthenticationManager = jWTAuthenticationManager;
        }

        public async Task<IAuthenticationResponse> Refresh(
            IRefreshCred refreshCred,
            TokenValidationParameters tokenValidationParameters) 
        {
            var jwtRefreshCred = refreshCred as JwtRefreshCred;

            SecurityToken validatedToken;

            var principal = new JwtSecurityTokenHandler().ValidateToken(
                jwtRefreshCred.JwtToken,
                tokenValidationParameters, 
                out validatedToken);

            if (!IsValidToken(validatedToken as JwtSecurityToken))
                throw new SecurityTokenException("Invalid token passed!");

            var userLogin = principal.Claims.ToList().Where(claim => claim.Type == "Login").FirstOrDefault().Value;

            if (jwtRefreshCred.JwtRefreshToken != await _JwtAuthenticationManager.RefreshTokensDictionary.GetStringAsync(userLogin))
                throw new SecurityTokenException("Invalid token passed!");

            return await _JwtAuthenticationManager.AuthenticateAsync(principal.Claims.ToList());
        }

        private bool IsValidToken(JwtSecurityToken securityToken) => securityToken != null || securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}
