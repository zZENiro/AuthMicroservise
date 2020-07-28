using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public class JwtAuthenticationResponse : IAuthenticationResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
