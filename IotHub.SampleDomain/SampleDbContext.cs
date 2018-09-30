using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace IotHub.SampleDomain
{
    public class SampleDbContext : BaseMsSqlDbContext
    {
        public SampleDbContext(string connectionString) : base(connectionString)
        {
        }

    }
}
