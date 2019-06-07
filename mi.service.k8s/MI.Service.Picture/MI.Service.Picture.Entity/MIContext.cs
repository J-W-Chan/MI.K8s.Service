using Microsoft.EntityFrameworkCore;

namespace MI.Service.Picture.Entity
{
    public class MIContext:DbContext
    {
        public MIContext(DbContextOptions<MIContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StartProdect>().ToTable("StartProdect");
            modelBuilder.Entity<SlideShowImg>().ToTable("SlideShowImg");
            modelBuilder.Entity<HardWare>().ToTable("HardWare");
        }

        public DbSet<StartProdect> StartProdects { get; set; }
        public DbSet<SlideShowImg> SlideShowImgs { get; set; }
        public DbSet<HardWare> HardWares { get; set; }
    }
}
