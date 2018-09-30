using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.OAuth
{
    public class AuthenticateDbContext : BaseMsSqlDbContext
    {
        public AuthenticateDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<OAuth.User> Users { get; set; }
    }
}
