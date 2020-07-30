using ModelsApp.Models;
using RepositoriesApp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoryApplication
{
    public interface IUsersRepository : IRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetOneAsync(string login);

        Task AddManyAsync(params User[] users);

        Task AddAsync(User user);

        Task EditAsync(User destUser, User newUser);
    }
}
