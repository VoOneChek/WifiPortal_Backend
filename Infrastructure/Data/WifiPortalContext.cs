using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class WifiPortalContext : DbContext
    {
        public WifiPortalContext(DbContextOptions<WifiPortalContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<AuthSession> AuthSessions => Set<AuthSession>();
        public DbSet<AuthMethod> AuthMethods => Set<AuthMethod>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<AuthSession>()
                .HasIndex(s => s.MacAddress);

            modelBuilder.Entity<AuthMethod>()
                .HasData(
                    new AuthMethod { Id = 1, Name = "SMS", Description = "Вход по SMS-коду", IsEnabled = true },
                    new AuthMethod { Id = 2, Name = "RADIUS", Description = "Аутентификация через RADIUS", IsEnabled = true },
                    new AuthMethod { Id = 3, Name = "VK", Description = "Вход через социальную сеть VK", IsEnabled = true }
                );
        }
    }
}
