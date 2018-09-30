using Microsoft.EntityFrameworkCore;

namespace IotHub.Core.DataAccess
{
    public class BaseMySqlDbContext : DbContext
    {
        private string _connectionString = string.Empty;

        public BaseMySqlDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BaseMySqlDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }
    }
}
