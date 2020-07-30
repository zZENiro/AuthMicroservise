using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesApp
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetOneAsync(string login);

        Task AddManyAsync(params T[] users);

        Task AddAsync(T item);

        Task EditAsync(T dest, T newOne);
    }
}
