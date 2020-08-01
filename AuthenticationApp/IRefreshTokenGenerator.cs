using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public interface IRefreshTokenGenerator
    {
        string GenerateRefreshTokenString();
    }
}
