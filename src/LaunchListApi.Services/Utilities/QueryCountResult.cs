using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Utilities
{
    /// <summary>
    /// Special type of query result for getting counts of the content after optionally applying a filter
    /// </summary>
    public class QueryCountResult
    {
        public QueryCountResult(int count)
        {
            Count = count;
            ResultType = ResultType.OkForQuery;
        }

        public QueryCountResult(ResultType resultType)
        {
            Count = 0;
            ResultType = resultType;
        }

        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResultCode => (int)this.ResultType;

        public int Count { get; }
        public ResultType ResultType { get; }
    }
}
