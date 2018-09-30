using Microsoft.EntityFrameworkCore;

namespace IotHub.Core.DataAccess
{
    public class BaseMsSqlDbContext : DbContext
    {
        private string _connectionString = string.Empty;

        public BaseMsSqlDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BaseMsSqlDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
