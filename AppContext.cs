using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiketsApp.Models;
using TiketsApp.res;

namespace TiketsApp
{
    internal class AppContext : DbContext
    {
        public DbSet<Role> AllRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Saller> Sallers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring ( DbContextOptionsBuilder optionsBuilder )
        {
            optionsBuilder.UseSqlServer(Consts.SqlConnection);
        }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Events)
                .WithOne(e => e.RootCategory)
                .HasForeignKey(e => e.RootCategoryId);


            modelBuilder.Entity<Event>()
                .HasOne(e => e.SubCategory)
                .WithMany()
                .HasForeignKey(e => e.SubCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Orders)
                .WithOne(o => o.Event)
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Order>()
            //       .HasOne(o => o.Event)
            //       .WithMany(e => e.Orders) 
            //       .HasForeignKey(o => o.EventId)
            //       .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Order>()
                  .HasOne(o => o.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                  .HasOne(o => o.Saller)
                  .WithMany(s => s.Orders)
                  .HasForeignKey(o => o.SallerId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
