using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Storage> Storages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Storage>()
                .HasMany(storage => storage.Collaborators)
                .WithMany(user => user.StorageCollabs);

            modelBuilder.Entity<Storage>()
                .HasOne(storage => storage.Owner)
                .WithMany(user => user.Storages)
                .HasForeignKey(storage => storage.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<File>()
                .HasOne(file => file.Storage)
                .WithMany(storage => storage.Files)
                .HasForeignKey(file => file.StorageId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
