using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public class ClaimsAuthenticationResponse : IAuthenticationResponse, IEnumerable<Claim>
    {
        public List<Claim> Claims { get; set; }
        public ClaimsPrincipal UserPrincipal { get; set; }
        public ClaimsIdentity UserIdentity { get; set; }

        public IEnumerator<Claim> GetEnumerator()
        {
            foreach (var claim in Claims)
                yield return claim;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
