using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model
{
    public class AuditLogEntry
    {
        public int Id { get; private set; }
        public AuditLogEntryType AuditType { get; private set; }
        public DomainEventType DomainEventType { get; private set; }
        public Guid EventStreamId { get; private set; }
        public int EventId { get; private set; } // NOT set up as a foreign key, since we want the event source and the audit log to both stand independently of each other
        public DataAccessType? DataAccessType { get; private set; }
        public string UserName { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }

        public AuditLogEntry() : this(AuditLogEntryType.DomainEvent, DomainEventType.Invalid, null, new Guid(), 0, null)
        {
        }

        public AuditLogEntry(AuditLogEntryType auditType, DomainEventType eventType, string userName, Guid eventStreamId, int eventId, DataAccessType? dataAccessType)
        {
            this.AuditType = auditType;
            this.DomainEventType = eventType;
            this.UserName = userName;
            this.EventStreamId = eventStreamId;
            this.EventId = eventId;
            this.DataAccessType = dataAccessType;
            this.Timestamp = DateTimeOffset.UtcNow;
        }
    }

    public enum AuditLogEntryType
    {
        AccessDenied,
        DomainEvent,
        DataAccess,
        NotInitialised
    }

    // The different types of data access that can be registered when auditing a data access event.
    public enum DataAccessType
    {
        GetClient,
        GetClientList,
        GetClientCount,
        GetAgency,
        GetAgencyList,
        GetAgencyCount,
        GetTransportOperator,
        GetTransportOperatorList,
        GetTransportOperatorCount,
        CreateTrip,
        ModifyTrip,
        ModifyTransportOperator
    }

    public static class DataAccessTypeExtensions
    {
        public static bool IsGetAggregateAccess(this DataAccessType accessType)
        {
            switch (accessType)
            {
                case DataAccessType.GetClient:
                case DataAccessType.GetAgency:
                case DataAccessType.GetTransportOperator:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsGetAggregateSetAccess(this DataAccessType accessType)
        {
            switch (accessType)
            {
                case DataAccessType.GetClientCount:
                case DataAccessType.GetClientList:
                case DataAccessType.GetAgencyCount:
                case DataAccessType.GetAgencyList:
                case DataAccessType.GetTransportOperatorCount:
                case DataAccessType.GetTransportOperatorList:
                    return true;
                default:
                    return false;
            }
        }

    }
}
