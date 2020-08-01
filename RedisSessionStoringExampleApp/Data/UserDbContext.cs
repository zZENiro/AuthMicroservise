using Microsoft.EntityFrameworkCore;
using RedisSessionStoringExampleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) => Database.EnsureCreated();

        public UserDbContext() => Database.EnsureCreated();
    }
}
