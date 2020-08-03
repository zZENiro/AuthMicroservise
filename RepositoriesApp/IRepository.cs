using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesApp
{
    public interface IRepository
    {
        Task<IEnumerable<T>> GetAllAsync<T>();

        Task<T> GetOneAsync<T>(string login);

        Task AddManyAsync<T>(params T[] users);

        Task AddAsync<T>(T item);

        Task EditAsync<T>(T dest, T newOne);
    }
}
