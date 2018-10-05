//using Microsoft.EntityFrameworkCore;

//namespace IotHub.Core.DataAccess
//{
//    public abstract class AbstractMySqlDbContext : DbContext
//    {
//        private string _connectionString = string.Empty;

//        public AbstractMySqlDbContext(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public AbstractMySqlDbContext(DbContextOptions options) : base(options)
//        {
//        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseMySql(_connectionString);
//        }
//    }
//}
