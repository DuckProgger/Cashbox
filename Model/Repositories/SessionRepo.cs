using Cashbox.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Repositories
{
    public static class SessionRepo
    {
        public static Session CreateSession(string userName)
        {
            using ApplicationContext db = new();
            var user = UserRepo.GetUser(userName);
            var session = db.Sessions.Add(new() { UserId = user.Id });
            db.SaveChanges();
            return session.Entity;
        }

        public static void RemoveSession(int id)
        {
            using ApplicationContext db = new();
            db.Sessions.Remove(db.Sessions.Find(id));
            db.SaveChanges();
        }
    }
}
