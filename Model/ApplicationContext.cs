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
        public DbSet<Worker> Staff { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Salary> Salaries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(GetConnectionString()/*, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)*/)
                //.UseLazyLoadingProxies()
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(UserConfigure);
            modelBuilder.Entity<Permissions>(PermissionsConfigure);
            modelBuilder.Entity<Shift>(ShiftConfigure);
            //modelBuilder.Entity<Worker>(WorkerConfigure);
        }

        private void UserConfigure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasOne(u => u.Permissions)
                .WithOne(p => p.User)
                .HasForeignKey<Permissions>(k => k.Id);
        }

        private void PermissionsConfigure(EntityTypeBuilder<Permissions> builder)
        {
            builder.ToTable(nameof(Users));
        }

        private void ShiftConfigure(EntityTypeBuilder<Shift> builder)
        {
            builder
                .HasOne(s => s.User)
                .WithMany(u => u.Shifts)
                .OnDelete(DeleteBehavior.NoAction);

            builder
              .Property(x => x.CreatedAt)
              .HasDefaultValueSql("GETDATE()");
        }


        //private void WorkerConfigure(EntityTypeBuilder<Worker> builder)
        //{
        //    builder
        //        .HasOne(w => w.User)
        //        .WithMany(u => u.Staff)
        //        .HasForeignKey(k => k.Id);
        //}


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
