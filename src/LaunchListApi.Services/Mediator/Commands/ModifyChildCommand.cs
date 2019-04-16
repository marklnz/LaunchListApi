using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Commands
{
    public class ModifyChildCommand<T>: IRequest<CommandResult<Guid>> where T: class, IAggregateRoot, new()
    {
        public ModifyChildCommand(Guid parentEventStreamId, int childEntityId, string jsonData, Type childEntityType)
        {
            EventData = jsonData;
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
            ParentEventStreamId = parentEventStreamId;
            ChildEntityType = childEntityType;
            ChildEntityId = childEntityId;
        }

        // The event stream id that identifies the parent of the new entity.
        public Guid ParentEventStreamId { get; }
        public string EventData { get; }
        public DateTimeOffset Timestamp { get; }
        public Type ChildEntityType { get; }
        public int ChildEntityId { get; }
    }
}
