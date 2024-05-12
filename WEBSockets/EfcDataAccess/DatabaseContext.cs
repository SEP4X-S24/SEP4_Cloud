﻿using Microsoft.EntityFrameworkCore;
using WEBSockets.Domain.Models;

namespace WEBSockets.EfcDataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<WeatherData> WeatherData { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=sql.freedb.tech;Port=3306;Database=freedb_weatherstation;Uid=freedb_cristi;Pwd=wx*kQ6Ez7gK#6Jg");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherData>().HasKey(w => w.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
        }
    }
}
