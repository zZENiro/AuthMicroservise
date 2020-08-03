using AuthenticationApp;
using RedisSessionStoringExampleApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Data.UsersRepository
{
    public interface IUsersRepository
    {
        Task AddAsync(User user);

        Task AddManyAsync(params User[] users);

        Task EditAsync(User destUser, User newUser);

        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetOneAsync(string login);
    }
}