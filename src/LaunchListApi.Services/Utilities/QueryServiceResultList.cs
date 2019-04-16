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
    /// A compound result similar to Service result but is intended for lists of results and ensure a proper list is returned and not a IQueryable resulting in an oData interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryServiceResultList<T>
    {
        /// <summary>
        /// This is a factory method rather than a constructor as we want it to be asynchronous and constructors aren't
        /// </summary>
        /// <param name="content"></param>
        /// <param name="useAsync">There are issues using the async calls with fakes so add this (defaulted on) flag to use non-async alternatives in testing</param>
        /// <returns></returns>
        public static async Task<QueryServiceResultList<T>> Create(IQueryable<T> content, bool useAsync = true)
        {
            return await Create(content, null, null, useAsync);
        }

        /// <summary>
        /// This is a factory method rather than a constructor as we want it to be asynchronous and constructors aren't
        /// </summary>
        /// <param name="content"></param>
        /// <param name="pagingParams">A <see cref="PagingParameters"/> object that specifies paging, sorting and filter information to use when building the list</param>
        /// <param name="filter">The method that will filter the content (This will have come from the filter string in the paging parameters but before this level)</param>
        /// <param name="useAsync">There are issues using the async calls with fakes so add this (defaulted on) flag to use non-async alternatives in testing</param>
        /// <returns></returns>
        public static async Task<QueryServiceResultList<T>> Create(IQueryable<T> content, PagingParameters pagingParams, Func<T, bool> filter = null, bool useAsync = true)
        {
            if (pagingParams != null)
            {
                // Apply the sorting
                var propertyInfo = typeof(T).GetProperty(pagingParams.SortColumn, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (pagingParams.SortColumn != null && propertyInfo != null)
                {
                    // Name of the column to sort by is in pagingParams.SortColumn, so we need to grab a property access expression, and build an order by expression using that,
                    // and add it to the existing expression tree.
                    var type = typeof(T);
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    MethodCallExpression resultExp;

                    if (pagingParams.Ascending)
                        resultExp = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { type, propertyInfo.PropertyType }, content.Expression, Expression.Quote(orderByExp));
                    else
                        resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, propertyInfo.PropertyType }, content.Expression, Expression.Quote(orderByExp));

                    content = content.Provider.CreateQuery<T>(resultExp);
                }

                // Retrieve only one page of data, unless pagesize is zero, in which case we return all data.
                int takeCount = pagingParams.PageSize == 0 ? content.Count() : pagingParams.PageSize;

                // if we need to filter the content - do it before the paging
                if (filter != null)
                {
                    // LINQ Expressions are odd creatures and can only be defined using the syntax below.
                    // I think this is a limitation of .NET Core as did not have this problem before
                    Expression<Func<T, bool>> filterPredicate = v => filter(v);
                    content = content.Where(filterPredicate);
                }

                content = content.Skip(pagingParams.PageSize * (pagingParams.Page - 1)).Take(takeCount);
            }

            List<T> justAList;
            if (useAsync)
            {
                justAList = await content.ToListAsync();
            }
            else
            {
                justAList = content.ToList();
            }

            return new QueryServiceResultList<T>(justAList, ResultType.OkForQuery);
        }

        /// <summary>
        /// where for any reason we cannot find the resources we were looking for
        /// </summary>
        /// <param name="resultType"></param>
        /// <returns></returns>
        public static QueryServiceResultList<T> CreateEmpty(ResultType resultType)
        {
            return new QueryServiceResultList<T>(new List<T>(), resultType);
        }

        /// <summary>
        /// Look for a subset
        /// </summary>
        /// <param name="content"></param>
        /// <param name="predicate"></param>
        /// <param name="useAsync">There are issues using the async calls with fakes so add this (defaulted on) flag to use non-async alternatives in testing</param>
        /// <returns></returns>
        public static async Task<QueryServiceResultList<T>> Find(IQueryable<T> content, Expression<Func<T, bool>> predicate, bool useAsync = true)
        {
            List<T> justAList;
            if (predicate != null)
            {
                content = content.Where(predicate);
            }
            if (useAsync)
            {
                justAList = await content.ToListAsync();
            }
            else
            {
                justAList = content.ToList();
            }
            ResultType result;
            if (justAList.Any())
            {
                result = ResultType.OkForQuery;
            }
            else
            {
                result = ResultType.NothingFound;
            }
            return new QueryServiceResultList<T>(justAList, result);
        }


        public QueryServiceResultList(List<T> content, ResultType resultType)
        {
            Content = content;
            ResultType = resultType;
        }

        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResultCode => (int)ResultType;

        public ResultType ResultType { get; }
        public List<T> Content { get; }
    }
}
