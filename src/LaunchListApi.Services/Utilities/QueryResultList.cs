using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Utilities
{
    /// <summary>
    /// A compound result similar to Query Service Result List but is intended for use as passing the result of a query back to the initiating query requestor, usually a controller class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResultList<T>
    {
        /// <summary>
        ///  This should be the default variant constructor to use - returns the provided content with a result type of <see cref="ResultType.OkForQuery"/>
        /// </summary>
        /// <param name="content">The content to return</param>
        public QueryResultList(List<T> content)
        {
            Content = content;
            ResultType = ResultType.OkForQuery;
        }

        /// <summary>
        /// Can use this constructor to optionally return the content with a <see cref="ResultType"/> of any sort.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="resultType"></param>
        public QueryResultList(List<T> content, ResultType resultType)
        {
            Content = content;
            ResultType = resultType;
        }

        /// <summary>
        /// Create a query result list, where something has gone wrong and there is no data to return, only the result type
        /// </summary>
        /// <param name="resultType"></param>
        public QueryResultList(ResultType resultType) : this(null, resultType)
        {

        }

        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResultCode => (int)ResultType;

        public ResultType ResultType { get; }
        public List<T> Content { get; }
    }
}
