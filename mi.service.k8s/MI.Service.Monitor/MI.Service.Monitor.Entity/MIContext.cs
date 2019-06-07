using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Monitor.Entity
{
    public class MIContext : DbContext
    {
        public MIContext(DbContextOptions<MIContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RabbitMQRegisterInfo>().ToTable("RabbitMQRegisterInfo");
        }
        public DbSet<RabbitMQRegisterInfo> RabbitMqRegisterInfo { get; set; }
    }
}
