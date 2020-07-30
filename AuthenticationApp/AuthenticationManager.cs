using AuthenticationApp.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ModelsApp.Models.ViewModels;
using RepositoryApplication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public abstract class AuthenticationManager
    {
        public IDictionary<string, string> UsersRefreshTokens { get; set; }

        protected readonly string _tokenKey;
        protected readonly IUsersRepository _usersRepo;
        protected readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public AuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator,IUsersRepository usersRepo)
        {
            _tokenKey = tokenKey;
            _usersRepo = usersRepo;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public abstract Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, Claim[] claims, HttpContext httpContext);

        public abstract Task<IAuthenticationResponse> Authenticate(NewUserCred userCredentionals, HttpContext httpContext);
    }
}
