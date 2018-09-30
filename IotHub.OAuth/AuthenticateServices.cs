using IotHub.Core.Redis;
using System;
using System.Linq;

namespace IotHub.OAuth
{
    public static class AuthenticateServices
    {
        static string _connectionString;
        public static readonly int EpireAfterMinutes = 30;

       
        public static User Login(string email, string password, out string tokenSession)
        {
            User u;
            tokenSession = string.Empty;
            using (var db = new AuthenticateDbContext(_connectionString))
            {
                u = db.Users.Where(i => i.Username.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                 i.Password.Equals(password, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            if (u == null) return null;

            tokenSession = $"{Guid.NewGuid()}_{email.GetHashCode()}_{DateTime.Now.GetHashCode()}";

            using (var db = new AuthenticateDbContext(_connectionString))
            {
                u.TokenSession = tokenSession;
                db.SaveChanges();
            }

            RedisServices.Set<User>(tokenSession, u, new TimeSpan(0, EpireAfterMinutes, 0));

            return u;
        }

        public static bool IsValidToken(string tokenSession)
        {
            var u = RedisServices.Get<User>(tokenSession);

            return u != null;
        }
    }
}
