using IotHub.Core.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.CommandsEvents.UserDomain
{
    public class RegisterUser : ICommand
    {
        public Guid PublishedCommandId { get; set; }
        public string TokenSession { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUser : ICommand
    {
        public Guid PublishedCommandId { get; set; }
        public string TokenSession { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LogoutUser : ICommand
    {
        public Guid PublishedCommandId { get; set; }
        public string TokenSession { get; set; }

        public Guid UserId { get; set; }
    }
}
