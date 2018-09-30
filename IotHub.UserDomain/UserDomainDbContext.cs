using IotHub.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.UserDomain
{
    public class UserDomainDbContext : BaseMsSqlDbContext
    {
        public UserDomainDbContext(string connectionString) : base(connectionString)
        {
        }

    }
}
