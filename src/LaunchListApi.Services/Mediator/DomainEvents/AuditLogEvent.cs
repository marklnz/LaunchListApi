using MediatR;
using LaunchListApi.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.DomainEvents
{
    public class AuditLogEvent : DomainEventNotificationBase
    {
        public Nullable<DataAccessType> DataAccessType { get; }
        public Nullable<DomainEventType> AccessDeniedEventType { get; }
        
        /// <summary>
        /// Use this constructor only when there is no associated domain event available. This will only be the case where we're auditing an error condition of some kind, such as AccessDenied Audit Events.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="userName"></param>
        public AuditLogEvent(DomainEventType eventType, string userName) : this(eventType, userName, Guid.Empty, 0)
        {
        }

        public AuditLogEvent(DomainEventType eventType, string userName, Guid eventStreamId, bool accessDenied = false) : this(accessDenied ? DomainEventType.AccessDeniedAuditEvent : eventType, userName, eventStreamId, 0)
        {
            if (accessDenied)
            {
                this.AccessDeniedEventType = eventType;
            }
        }

        public AuditLogEvent(DomainEventType eventType, String userName, Guid eventStreamId, int eventId) : base(eventType, userName, eventStreamId, eventId)
        {
        }

        public AuditLogEvent(string userName, Guid eventStreamId, DataAccessType dataAccessType, bool accessDenied = false): this(accessDenied ? DomainEventType.AccessDeniedAuditEvent : DomainEventType.DataAccessedAuditLogEvent, userName, eventStreamId, 0)
        {
            this.DataAccessType = dataAccessType;
        }
    }
}
