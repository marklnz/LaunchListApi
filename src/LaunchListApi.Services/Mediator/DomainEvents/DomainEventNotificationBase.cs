using MediatR;
using LaunchListApi.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.DomainEvents
{
    /// <summary>
    /// A wrapper notification interface for use purely so that we can handle ALL events in the <see cref="AuditLogEventHandler"/>, as the INotification interface cannot be used directly 
    /// as T in the generic Event Handlers used with MediatR.
    /// </summary>
    public abstract class DomainEventNotificationBase: INotification
    {
        public string UserName { get; }
        public DateTimeOffset Timestamp { get; }
        public DomainEventType DomainEventType { get; }
        public Guid EventStreamId { get; }
        public int EventId { get; }        

        public DomainEventNotificationBase(DomainEventType domainEventType, string userName, Guid eventStreamId, int eventId)
        {
            this.UserName = userName;
            this.Timestamp = DateTimeOffset.UtcNow;
            this.DomainEventType = domainEventType;
            this.EventStreamId = eventStreamId;
            this.EventId = eventId;
        }
    }
}
