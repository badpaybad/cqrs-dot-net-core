using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IotHub.UserDomain
{
   public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Actived { get; set; }
        public string TokenSession { get; set; }
    }
    
    public class UserProfile
    {
        [Key]
        public Guid UserId { get; set; }
        public string Fullname { get; set; }
    }
}
