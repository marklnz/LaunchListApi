using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Utilities
{
    // Non-generic class that defaults to using int as the id field.
    public class CommandResult : CommandResult<int>
    {
        public CommandResult(int newId) : base(newId) { }

        public CommandResult(ResultType resultType, List<string> errorList = null) : base(resultType, errorList) { }
    }

    /// <summary>
    /// Command services do not return data, by definition
    /// EXCEPT When we add a new item, we want to return the identity of the new item
    /// </summary>
    public class CommandResult<T>
    {
        public CommandResult(ResultType resultType, List<string> errorList = null)
        {
            ResultType = resultType;
            Errors = errorList;
        }

        public CommandResult(ResultType resultType, T newId, List<string> errorList = null)
        {
            ResultType = resultType;
            Errors = errorList;
            NewId = newId;
        }

        /// <summary>
        /// Called after adding a new item
        /// </summary>
        /// <param name="newId"></param>
        public CommandResult(T newId)
        {
            NewId = newId;
            ResultType = ResultType.OkResourceCreated;
        }

        /// <summary>
        /// Used when successfully modifying an aggregate - no data is returned
        /// </summary>
        public CommandResult()
        {
            ResultType = ResultType.OkForCommand;
        }

        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResponseCode => (int)this.ResultType;

        public T NewId { get; }
        public ResultType ResultType { get; }
        public List<string> Errors { get; }
    }
}
