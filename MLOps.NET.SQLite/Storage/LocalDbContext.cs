﻿using Microsoft.EntityFrameworkCore;
using MLOps.NET.SQLite.Entities;

namespace MLOps.NET.SQLite.Storage
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=Data/local.db");

        public DbSet<Experiment> Experiments { get; set; }
        public DbSet<Metric> Metrics { get; set; }
        public DbSet<Run> Runs { get; set; }
    }
}
