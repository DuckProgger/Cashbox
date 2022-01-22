using Cashbox.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cashbox.Model.Repositories
{
    public static class CommonRepo
    {
        public static T Create<T>(T entity) where T : class
        {
            using ApplicationContext db = new();
            var createdEntity = db.Set<T>().Add(entity);
            db.SaveChanges();
            return createdEntity.Entity;
        }
    }
}