
using System;
using Microsoft.EntityFrameworkCore;

namespace IotHub.Core.Cqrs.EventSourcingRepository
{
    internal class EventSourcingDbContext :  DbContext
    {
        private string _connectionString = string.Empty;

        public EventSourcingDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public EventSourcingDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        public DbSet<EventSourcingDescription> EventSoucings { get; set; }


        //public DbSet<CommandLog> CommandLogs { get; set; }

    }
}