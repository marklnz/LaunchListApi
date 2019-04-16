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
    /// <typeparam name="T"></typeparam>
    public class QueryServiceCountResult<T>
    {
        public QueryServiceCountResult(int count)
        {
            Count = count;
            ResultType = ResultType.OkForQuery;
        }

        public QueryServiceCountResult(ResultType resultType)
        {
            Count = 0;
            ResultType = resultType;
        }

        /// <summary>
        /// Get a count of the content given the supplied filter.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filter"></param>
        /// <param name="useAsync"></param>
        /// <returns></returns>
        /// <remarks>This method does not return a QueryServiceResultList as the result will either be a valid count or an exception will be thrown.
        /// This method does not need to be in this class but is the most sensible choice</remarks>
        public static async Task<QueryServiceCountResult<T>> GetCount(IQueryable<T> content, Func<T, bool> filter = null, bool useAsync = true)
        {

            // if we need to filter the content
            if (filter != null)
            {
                // LINQ Expressions are odd creatures and can only be defined using the syntax below.
                // I think this is a limitation of .NET Core as did not have this problem before
                Expression<Func<T, bool>> filterPredicate = v => filter(v);
                content = content.Where(filterPredicate);
            }
            int count = 0;
            if (useAsync)
            {
                count = await content.CountAsync();
            }
            else
            {
                count = content.Count();
            }
            return new QueryServiceCountResult<T>(count);
        }
        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResultCode => (int)this.ResultType;

        public int Count { get; }
        public ResultType ResultType { get; }
    }
}
