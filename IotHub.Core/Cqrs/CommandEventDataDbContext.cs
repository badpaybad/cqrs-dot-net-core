using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace IotHub.Core.Cqrs
{
    internal class CommandEventStorageDbContext : DbContext
    {
        private string _connectionString = string.Empty;

        public CommandEventStorageDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CommandEventStorageDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
        }

        public DbSet<CommandEventStorage> CommandEventStorages { get; set; }

        public DbSet<CommandEventStorageHistory> CommandEventStorageHistories { get; set; }
    }
    
    [Table("CommandEventStorage")]
    public class CommandEventStorage
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(512)]
        public string DataType { get; set; }

        public string DataJson { get; set; }

        public bool IsCommand { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    [Table("CommandEventStorageHistory")]
    public class CommandEventStorageHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Key]
        public Guid CommandEventId { get; set; }

        public int State { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Message { get; set; }
    }

    public enum CommandEventStorageState
    {
        Pending,
        Done,
        Fail
    }
}