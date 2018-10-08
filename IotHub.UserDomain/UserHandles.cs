using IotHub.CommandsEvents.UserDomain;
using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;
using System;

namespace IotHub.UserDomain
{
    public class UserHandles : ICommandHandle<RegisterUser>
        , ICommandHandle<LoginUser>, ICommandHandle<LogoutUser>
    {
        public ICqrsEventSourcingRepository Repository { get; }

        public void Handle(RegisterUser c)
        {
            throw new NotImplementedException();
        }

        public void Handle(LoginUser c)
        {
            throw new NotImplementedException();
        }

        public void Handle(LogoutUser c)
        {
            throw new NotImplementedException();
        }
    }
}
