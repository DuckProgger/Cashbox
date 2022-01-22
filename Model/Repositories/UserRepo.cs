using Cashbox.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Repositories
{
    public static class UserRepo
    {
        public static User GetUser(string userName)
        {
            using ApplicationContext db = new();
            return (from u in db.Users.Include(u => u.Permissions)
                    where u.Name == userName
                    select u).FirstOrDefault();
        }

        public static User GetUser(int userId)
        {
            using ApplicationContext db = new();
            return (from u in db.Users.Include(u => u.Permissions)
                    where u.Id == userId
                    select u).FirstOrDefault();
        }

        public static async Task<List<string>> GetUserNamesAsync()
        {
            using ApplicationContext db = new();
            return await (from user in db.Users
                          select user.Name).ToListAsync();
        }
    }
}
