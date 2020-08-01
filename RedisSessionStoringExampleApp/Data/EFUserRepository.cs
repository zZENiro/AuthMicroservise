using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Data
{
    //public class EFUserRepository : IRepository<User>
    //{
    //    private readonly UserDbContext _context;

    //    public EFUserRepository(UserDbContext context) =>
    //        _context = context;

    //    public async Task AddAsync(User user)
    //    {
    //        await _context.Users.AddAsync(user);
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task AddManyAsync(params User[] users)
    //    {
    //        await _context.Users.AddRangeAsync(users);
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task EditAsync(User destUser, User newUser)
    //    {
    //        var updateQuery = "UPDATE Users " +
    //                          "SET Login=@newLogin, Password=@newPassword" +
    //                          "WHERE Login = @login";

    //        await _context.Database.ExecuteSqlCommandAsync(updateQuery, new SqlParameter[] {
    //            new SqlParameter("@login", destUser.Login),
    //            new SqlParameter("@newLogin", newUser.Login),
    //            new SqlParameter("@newPassword", (newUser.Password != destUser.Password) ? newUser.Password : destUser.Password)
    //        });
    //    }

    //    public async Task<IEnumerable<User>> GetAllAsync() =>
    //        await _context.Users.ToListAsync();

    //    public async Task<User> GetOneAsync(string login) =>
    //        await _context.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
    //}
}
