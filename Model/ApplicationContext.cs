using Cashbox.Model.Entities;
using Cashbox.Model.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Worker> Staff { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Salary> Salaries { get; set; }

        // устанавливаем фабрику логгера
        private static readonly ILoggerFactory myLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new LoggerProvider());    // указываем наш провайдер логгирования
        });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(GetConnectionString())
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(myLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(UserConfigure);
            modelBuilder.Entity<Permissions>(PermissionsConfigure);
            modelBuilder.Entity<Shift>(ShiftConfigure);
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

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
        }
    }
}