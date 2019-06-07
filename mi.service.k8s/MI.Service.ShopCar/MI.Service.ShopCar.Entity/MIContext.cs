using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    public class MIContext : DbContext
    {
        public MIContext(DbContextOptions<MIContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopCarEntity>().ToTable("ShopCar");
            modelBuilder.Entity<CarListEntity>().ToTable("CarList");
            modelBuilder.Entity<ColorVersionEntity>().ToTable("ColorVersion");
            modelBuilder.Entity<PriceEntity>().ToTable("Price");
            modelBuilder.Entity<ProductEntity>().ToTable("Product");
            modelBuilder.Entity<VersionInfoEntity>().ToTable("VersionInfo");
        }

        public DbSet<ShopCarEntity> ShopCarEntitys { get; set; }
        public DbSet<CarListEntity> CarListEntitys { get; set; }
        public DbSet<ColorVersionEntity> ColorVersionEntitys { get; set; }
        public DbSet<PriceEntity> PriceEntitys { get; set; }
        public DbSet<CustomerEntity> CustomerEntitys { get; set; }
        public DbSet<ProductEntity> ProductEntities { get; set; }
        public DbSet<VersionInfoEntity> VersionInfoEntitys { get; set; }
    }
}
