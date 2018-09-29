using System;
using System.Runtime.Serialization;

namespace IotHub.Core.Cqrs.EventSourcingRepository
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException()
        {
        }

        public AggregateNotFoundException(string message) : base(message)
        {
        }

        public AggregateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AggregateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
