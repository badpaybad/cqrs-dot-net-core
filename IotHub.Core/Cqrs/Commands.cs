using IotHub.Core.Cqrs.EventSourcingRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.Cqrs
{
    public interface ICqrsHandle
    {
        ICqrsEventSourcingRepository Repository { get; }
    }

    public interface ICommandHandle<T> : ICqrsHandle where T : ICommand        
    {      
        void Handle(T c);
    }

    public interface ICommand
    {
         Guid PublishedCommandId { get; set; }
        string TokenSession { get; set; }
    }

}
