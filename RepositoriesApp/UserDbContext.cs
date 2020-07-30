using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelsApp.Models;

namespace RepositoryApplication
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) 
            : base(options) =>
            Database.EnsureCreated();

        public UserDbContext() =>
            Database.EnsureCreated();
    }
}
