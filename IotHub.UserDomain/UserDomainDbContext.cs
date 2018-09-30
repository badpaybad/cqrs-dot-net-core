using IotHub.Core;
using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.UserDomain
{
    public class UserDomainDbContext : AbstractMsSqlDbContext
    {       
        public UserDomainDbContext(string connectionString) 
            : base(ConfigurationManagerExtensions.GetConnectionString("UserDomainDbContext"))
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
