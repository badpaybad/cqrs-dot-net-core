using Microsoft.EntityFrameworkCore;

namespace IotHub.Core.DataAccess
{
    public abstract class AbstractMsSqlDbContext : DbContext
    {
        protected string _connectionString = string.Empty;
        
        public AbstractMsSqlDbContext()
        {
           
        }
        
        public AbstractMsSqlDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AbstractMsSqlDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
