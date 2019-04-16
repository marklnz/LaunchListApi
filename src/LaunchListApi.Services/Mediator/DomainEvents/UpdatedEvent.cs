using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using LaunchListApi.Model;

namespace LaunchListApi.Services.Mediator.DomainEvents
{
    public class UpdatedEvent<T>: DomainEventNotificationBase
    {
        public string EventData { get; }

        public UpdatedEvent(string eventData, string userName, Guid eventStreamId, int eventId) 
            : base((DomainEventType)Enum.Parse(typeof(DomainEventType), string.Format("Modify{0}Event", typeof(T).Name)), userName, eventStreamId, eventId)
        {
            this.EventData = eventData;
        }
    }
}
