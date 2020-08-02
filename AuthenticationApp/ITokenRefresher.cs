using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public interface ITokenRefresher
    {
        Task<IAuthenticationResponse> Refresh(
            IRefreshCred refreshCred,
            TokenValidationParameters tokenValidationParameters);
    }
}
