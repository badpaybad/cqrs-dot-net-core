using IotHub.Core.Cqrs;
using System;

namespace IotHub.UserDomain
{
    public class UserHandles : ICommandHandle<RegisterUser>
        , ICommandHandle<LoginUser>, ICommandHandle<LogoutUser>
    {
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
