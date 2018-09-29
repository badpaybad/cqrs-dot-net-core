using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.Cqrs
{
    public interface ICqrsHandle
    {

    }

    public interface ICommandHandle<T> : ICqrsHandle where T : ICommand
    {
        void Handle(T c);
    }

    public interface ICommand
    {

    }

}
