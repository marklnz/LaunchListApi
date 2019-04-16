using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Commands
{
    public class CreateCommand<T>: IRequest<CommandResult<Guid>> where T: class, IAggregateRoot, new()
    {
        public CreateCommand(string jsonData)
        {
            EventData = jsonData;
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
        }

        public string EventData { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
