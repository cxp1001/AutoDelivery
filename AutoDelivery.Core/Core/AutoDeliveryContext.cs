using System;
using System.Collections.Generic;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AutoDelivery.Domain.User;

namespace AutoDelivery.Core.Core
{
    public partial class AutoDeliveryContext : DbContext
    {

        public DbSet<UserAccount> Users { get; set; }
        public DbSet<OAuthState> LoginStates { get; set; }
        public DbSet<MailConfig> MailConfigs { get; set; }
        public  DbSet<OrderDetail> Orders { get; set; } = null!;
        public  DbSet<Product> Products { get; set; } = null!;
        public  DbSet<Serial> Serials { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public AutoDeliveryContext()
        {
        }

        public AutoDeliveryContext(DbContextOptions<AutoDeliveryContext> options)
            : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=101.43.15.165;Database=AutoDelivery;User Id=sa;Password=Sunrisep1001;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserAccount>().HasMany(u => u.OrderDetails);
            modelBuilder.Entity<UserAccount>().HasMany(u => u.Products);
            modelBuilder.Entity<UserAccount>().HasOne(u => u.Mailconfiguration);
            modelBuilder.Entity<Product>().HasMany(p=>p.SerialsInventory);
            modelBuilder.Entity<Product>().HasOne(p=>p.ProductCategory);
            modelBuilder.Entity<OrderDetail>().HasMany(o=>o.RelatedSerials);
            modelBuilder.Entity<Serial>().Property(s => s.RowVersion).IsRowVersion();

        }

       
    }
}
