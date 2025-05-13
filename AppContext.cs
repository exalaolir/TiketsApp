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
        }
    }
}
