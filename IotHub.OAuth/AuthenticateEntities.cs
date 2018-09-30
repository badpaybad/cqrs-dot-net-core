using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.OAuth
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Actived { get; set; }
        public string TokenSession { get; set; }
    }
}
