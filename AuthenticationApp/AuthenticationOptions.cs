using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationApp
{
    public class AuthenticationOptions
    {
        public const string Authentication = "Authentication";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int Lifetime { get; set; }

        public string Key { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}
