using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Utilities
{
    // Non-generic class that defaults to using int as the id field.
    public class UpdateServiceResult : UpdateServiceResult<int>
    {
        public UpdateServiceResult(int newId) : base(newId) { }

        public UpdateServiceResult(ResultType resultType, List<string> errorList = null) : base(resultType, errorList) { }
    }

    /// <summary>
    /// Command services do not return data by definition
    /// EXCEPT When we add a new item, we need to return the identity of the new item
    /// </summary>
    public class UpdateServiceResult<T>
    {
        public UpdateServiceResult(ResultType resultType, List<string> errorList = null)
        {
            ResultType = resultType;
            Errors = errorList;
        }

        /// <summary>
        /// Called after adding a new item, the only time data is returned on a command & the result is set to OkResourceCreated
        /// </summary>
        /// <param name="newId"></param>
        public UpdateServiceResult(T newId)
        {
            NewId = newId;
            ResultType = ResultType.OkResourceCreated;
        }

        /// <summary>
        /// Called after processing some command on an item, so we set the result type to OkForCommand
        /// </summary>
        public UpdateServiceResult()
        {
            ResultType = ResultType.OkForCommand;
        }

        /// <summary>
        /// Convert the result enum into an HTTP status code
        /// </summary>
        public int HttpResponseCode => ResultTypeToHttpCode(this.ResultType);

        public T NewId { get; }
        public ResultType ResultType { get; }
        public List<string> Errors { get; }

        public static int ResultTypeToHttpCode(ResultType resultType)
        {
            switch (resultType)
            {
                case ResultType.OkForQuery:
                    return 200;
                case ResultType.OkResourceCreated:
                    return 201;
                case ResultType.OkStillProcessing:
                    return 202;
                case ResultType.OkForCommand:
                    return 204;
                case ResultType.BadRequest:
                    return 400;
                case ResultType.Unauthenticated:
                    return 401;
                case ResultType.AccessDenied:
                    return 403;
                case ResultType.NothingFound:
                    return 404;
                case ResultType.StatusConflict:
                    return 409;
                case ResultType.NotImplementedYet:
                    return 501;
                case ResultType.InternalServerError:
                    return 500;
                case ResultType.ImATeapot:
                default:
                    return 418;
            }
        }

        public bool IsSuccessResult()
        {
            return (this.ResultType == ResultType.OkForCommand || this.ResultType == ResultType.OkForQuery || this.ResultType == ResultType.OkResourceCreated || this.ResultType == ResultType.OkStillProcessing);
        }
    }
}
