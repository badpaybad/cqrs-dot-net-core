using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace IotHub.Core.Ef
{
    public class IotHubDbContext : DbContext
    {
        private string _connectionString = string.Empty;
        
        public IotHubDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IotHubDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(_connectionString);
            optionsBuilder.UseNpgsql(_connectionString);
        }

        
    }
    
}
