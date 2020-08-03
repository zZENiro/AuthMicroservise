using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesApp
{
    public interface IRepository
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T: new();

        Task<T> GetOneAsync<T>(string login) where T : new();

        Task AddManyAsync<T>(params T[] users) where T : new();

        Task AddAsync<T>(T item) where T : new();

        Task EditAsync<T>(T dest, T newOne) where T : new();
    }
}
