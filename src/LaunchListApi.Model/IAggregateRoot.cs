using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model
{
    public interface IAggregateRoot
    {
        Guid EventStreamId { get; set; }
        DateTimeOffset EventVersionTimestamp { get; set; }
    }
}
