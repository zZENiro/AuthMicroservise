using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RedisSessionStoringExampleApp.Data.UsersRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Data
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddEFUsersRepository(this IServiceCollection services, UserDbContext dbContext) =>
            services.AddSingleton<IUsersRepository>(new EFUserRepository(dbContext));

        public static IServiceCollection AddTokenValidation(this IServiceCollection services, TokenValidationParameters tokenValidationParameters) =>
            services.AddSingleton(typeof(TokenValidationParameters), tokenValidationParameters);
    }
}
