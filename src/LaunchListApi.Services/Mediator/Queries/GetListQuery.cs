using MediatR;
using LaunchListApi.Model;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Mediator.Queries
{
    public class GetListQuery<T> : IRequest<QueryResultList<T>> where T : class, IAggregateRoot
    {
        public GetListQuery(PagingParameters paging)
        {
            Paging = paging;
            Timestamp = DateTimeOffset.UtcNow;      // this is the only place we set the timestamp
        }

        public PagingParameters Paging { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
