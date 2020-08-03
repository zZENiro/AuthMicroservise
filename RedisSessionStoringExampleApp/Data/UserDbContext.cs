using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RedisSessionStoringExampleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Data
{
    public class UserDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }

        public UserDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) => Database.EnsureCreated();

        public UserDbContext() => Database.EnsureCreated();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("defaultConn"));
        }
    }
}
