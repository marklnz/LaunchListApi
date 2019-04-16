using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Commands
{
    public class ModifyCommand<T>: IRequest<CommandResult<Guid>> where T: class, IAggregateRoot, new()
    {
        public ModifyCommand(Guid parentEventStreamId, string jsonData)
        {
            ParentEventStreamId = parentEventStreamId;
            EventData = jsonData;
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
        }

        public Guid ParentEventStreamId { get; }

        public string EventData { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
