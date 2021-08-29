using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;


namespace Cashbox.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.
                UseSqlServer(GetConnectionString()).
                UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(UserConfigure);
            modelBuilder.Entity<Permissions>(PermissionsConfigure);
        }

        private void UserConfigure(EntityTypeBuilder<User> builder)
        {
            builder.
                HasOne(u => u.Permissions).
                WithOne(p => p.User).
                HasForeignKey<Permissions>(k => k.Id);
        }

        private void PermissionsConfigure(EntityTypeBuilder<Permissions> builder)
        {
            builder.ToTable(nameof(Users));
        }

        private static string GetConnectionString()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            IConfigurationRoot config = builder.Build();
            // получаем строку подключения
            return config.GetConnectionString("DefaultConnection");
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
