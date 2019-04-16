using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Commands
{
    public class DeleteCommand<T>: IRequest<CommandResult<Guid>> where T: class, IAggregateRoot, new()
    {
        public DeleteCommand(Guid eventStreamId)
        {
            EventStreamId = eventStreamId; 
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
        }

        public Guid EventStreamId { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
