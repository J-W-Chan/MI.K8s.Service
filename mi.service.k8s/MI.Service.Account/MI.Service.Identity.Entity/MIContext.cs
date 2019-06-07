using MI.Service.Account.Entity;
using Microsoft.EntityFrameworkCore;

namespace MI.Service.Account.Entity
{
    public class MIContext:DbContext
    {
        public MIContext(DbContextOptions<MIContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().ToTable("Customer");
        }

        public DbSet<UserEntity> UserEntities { get; set; }
    }
}
