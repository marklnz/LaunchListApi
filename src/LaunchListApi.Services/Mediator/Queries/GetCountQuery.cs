using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Queries
{
    public class GetCountQuery<T> : IRequest<QueryCountResult> where T: class, IAggregateRoot
    {
        public GetCountQuery()
        {
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
        }

        public DateTimeOffset Timestamp { get; }
    }
}
