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
    /// A query result, returned from a request/response query message placed on the mediator event bus
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResult<T> where T : class
    {
        /// <summary>
        /// Create a query service result with data. <see cref="this.ResultType"/> defaults to <see cref="ResultType.OkForQuery"/> when this constructor is used.
        /// </summary>
        /// <param name="content"></param>
        public QueryResult(T content)
        {
            Content = content;
            ResultType = ResultType.OkForQuery;
        }

        /// <summary>
        /// Create a query service result with data, and a <see cref="ResultType"/>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="resultType"></param>
        public QueryResult(T content, ResultType resultType)
        {
            Content = content;
            ResultType = resultType;
        }

        /// <summary>
        /// Create a query result, where something has gone wrong and there is no data to return, only the result type
        /// </summary>
        /// <param name="resultType"></param>
        public QueryResult(ResultType resultType) : this(null, resultType)        {

        }
                
        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResultCode => (int)this.ResultType;
        public ResultType ResultType { get; }
        public T Content { get; }
    }
}
