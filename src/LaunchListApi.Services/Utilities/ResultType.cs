using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Utilities
{
    /// <summary>
    /// A set of possible results from the service layer that can be mapped back to the standard HTTP results
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 200: Used for the successful responses to queries that get data back
        /// </summary>
        OkForQuery = 200,

        /// <summary>
        /// 201: Generally only for successful responses to posts that return just the id of the new item
        /// </summary>
        OkResourceCreated = 201,

        /// <summary>
        /// 202: For responding early to commands that will continue in the background
        /// </summary>
        OkStillProcessing = 202,

        /// <summary>
        /// 204: Used for the succesful responses to commands where there is no content to return
        /// </summary>
        OkForCommand = 204,

        /// <summary>
        /// 400: We don't understand the request - bad parameters or something else
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 401: This should be generated earlier in the pipeline before the controller and service
        /// </summary>
        Unauthenticated = 401,

        /// <summary>
        /// 403: We know who they are, but they are not allowed to do this
        /// </summary>
        AccessDenied = 403,

        /// <summary>
        /// 404: Asking for something that does not exist
        /// </summary>
        NothingFound = 404,

        /// <summary>
        /// 409: Asking for something in the wrong state, editing a suspended user for example
        /// </summary>
        StatusConflict = 409,

        /// <summary>
        /// 501: We could use this while developing so the client code will know at least the route is correct
        /// </summary>
        NotImplementedYet = 501,

        /// <summary>
        /// 500: Internal server error, failure in the service for some reason - usually in the exception handler
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// 418: This is a real code - it's even documented on Wikipedia
        /// </summary>
        ImATeapot = 418
    }
}
