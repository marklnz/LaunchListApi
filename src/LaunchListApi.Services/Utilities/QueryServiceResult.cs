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
    /// A compound result which allows richer feedback through a RESTful interface of why there is no result.
    /// This is intended for queries only
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryServiceResult<T> where T : class
    {
        /// <summary>
        /// Create a query service result with data
        /// </summary>
        /// <param name="content"></param>
        /// <param name="resultType"></param>
        public QueryServiceResult(T content, ResultType resultType)
        {
            Content = content;
            ResultType = resultType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resultType"></param>
        public QueryServiceResult(ResultType resultType) : this(null, resultType)
        {

        }

        /// <summary>
        /// A common pattern throughout the service layer where we are looking for a single item out of the table.
        /// Calls FirstOrDefault rather than SingleOrDefault for efficiency reasons
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <param name="useAsync">There are issues using the async calls with fakes so add this (defaulted on) flag to use non-async alternatives in testing</param>
        /// <returns></returns>
        public static async Task<QueryServiceResult<T>> GetFirstOfSet(IQueryable<T> data, Expression<Func<T, bool>> predicate, bool useAsync = true)
        {
            T content;
            if (useAsync)
            {
                content = await data.FirstOrDefaultAsync(predicate);
            }
            else
            {
                content = data.FirstOrDefault(predicate);
            }

            var resultType = content != null ? ResultType.OkForQuery : ResultType.NothingFound;
            return new QueryServiceResult<T>(content, resultType);
        }

        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResultCode => (int)this.ResultType;

        public ResultType ResultType { get; }
        public T Content { get; }
    }
}
