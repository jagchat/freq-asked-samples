using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace app.data
{
    public class SampleDbContext : DbContext
    {

        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<SampleDbContext> logger;
        private readonly string connString = "";

        public SampleDbContext(string connString, ILoggerFactory loggerFactory, ILogger<SampleDbContext> logger)
        {
            this.loggerFactory = loggerFactory;
            this.logger = logger;
            this.connString = connString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging(true)
                    .UseSqlServer(connString, options => options.MaxBatchSize(150));
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            logger.LogTrace("SampleDbContext.OnModelCreating: Started...");

            ////not needed as I did this using attributes in model class
            //modelBuilder.Entity<Employee>(o => o.ToTable("Emp").Property(p => p.Salary).HasField("Sal"));

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dept> Depts { get; set; }
    }
}
