using AuthenticationApp.Jwt;
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
        private readonly AuthenticationOptions _authenticationOptions;
        private readonly JwtAuthenticationManager _JwtAuthenticationManager;

        public JwtTokenRefresher(
            AuthenticationOptions authenticationOptions, 
            JwtAuthenticationManager jWTAuthenticationManager)
        {
            _authenticationOptions = authenticationOptions;
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

            var loginClaim = principal.Claims.ToList().Where(claim => claim.ValueType == "Login").FirstOrDefault();

            if (jwtRefreshCred.JwtRefreshToken != _JwtAuthenticationManager.UsersRefreshTokens[loginClaim.Value])
                throw new SecurityTokenException("Invalid token passed!");

            return await _JwtAuthenticationManager.AuthenticateAsync(principal.Claims.ToList());
        }

        private bool IsValidToken(JwtSecurityToken securityToken) => securityToken != null || securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}
