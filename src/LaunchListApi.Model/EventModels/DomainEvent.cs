using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model
{
    public class DomainEvent
    {
        public int Id { get; private set; }
        public string EventData { get; private set; }
        public string Username { get; private set; }
        public DomainEventType DomainEventType { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        public Guid EventStreamId { get; private set; }
        public int? ChildEntityId { get; private set; }

        public DomainEvent() : this(null, null, DomainEventType.Invalid, new Guid())
        {
        }

        public DomainEvent(string eventData, string username, DomainEventType type, Guid eventStreamId, int? childEntityId = null)
        {
            this.EventData = eventData;
            this.Username = username;
            this.DomainEventType = type;
            this.Timestamp = DateTimeOffset.UtcNow;
            this.EventStreamId = eventStreamId;
            this.ChildEntityId = childEntityId;
        }
    }

    public enum DomainEventType
    {
        Invalid,
        AuditLogEvent,
        AccessDeniedAuditEvent,
        CreateClientEvent,
        ModifyClientEvent,
        DeleteClientEvent,
        CreateAgencyEvent,
        ModifyAgencyEvent,
        DeleteAgencyEvent,
        CreateAgencyContactEvent,
        ModifyAgencyContactEvent,
        CreateTransportOperatorEvent,
        ModifyTransportOperatorEvent,
        DeleteTransportOperatorEvent,
        CreateDriverEvent,
        ModifyDriverEvent,
        CreateVehicleEvent,
        ModifyVehicleEvent,
        CreateTripEvent,
        ModifyTripEvent,
        DataAccessedAuditLogEvent        
    }

    public static class DomainEventExtensions
    {
        public static bool IsCreateEvent(this DomainEventType eventType)
        {
            return eventType.ToString().StartsWith("Create");
        }

        //public static bool IsEventFor<T>(this DomainEventType eventType) where T : class, IAggregateRoot, new()
        //{
        //    bool result = false;

        //    Type aggregateType = typeof(T);

        //    if (aggregateType.IsAssignableFrom(typeof(Client)))
        //        result = eventType.IsClientEvent();

        //    return result;
        //}

        //public static bool IsClientEvent(this DomainEventType eventType)
        //{
        //    bool result = false;
        //    switch (eventType)
        //    {
        //        case DomainEventType.CreateClientEvent:
        //        case DomainEventType.ModifyClientEvent:
        //            result = true;
        //            break;
        //        default:
        //            result = false;
        //            break;
        //    }

        //    return result;
        //}
    }
}
