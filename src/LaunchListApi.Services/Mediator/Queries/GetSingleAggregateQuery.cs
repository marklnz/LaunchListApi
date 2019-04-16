using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Queries
{
    public class GetSingleAggregateQuery<T>: IRequest<QueryResult<T>> where T : class, IAggregateRoot
    {
        public GetSingleAggregateQuery(Guid eventStreamId)
        {
            RequestedEventStreamId = eventStreamId;
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
        }

        public Guid RequestedEventStreamId { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
