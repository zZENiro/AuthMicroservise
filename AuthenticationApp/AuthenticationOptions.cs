using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationApp
{
    public class AuthenticationOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int Lifetime { get; set; }

        public string Key { get; set; }
    }
}
