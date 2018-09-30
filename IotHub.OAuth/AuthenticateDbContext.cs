using IotHub.Core;
using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace IotHub.OAuth
{
    public class AuthenticateDbContext : AbstractMsSqlDbContext
    {        
        public AuthenticateDbContext(string connectionString)
            : base(ConfigurationManagerExtensions.GetConnectionString("AuthenticateDbContext"))
        {
        }

        public DbSet<OAuth.User> Users { get; set; }
    }
}
