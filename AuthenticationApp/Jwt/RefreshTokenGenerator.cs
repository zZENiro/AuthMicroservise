using System;
using System.Security.Cryptography;

namespace AuthenticationApp.Jwt
{
    public class JwtRefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
