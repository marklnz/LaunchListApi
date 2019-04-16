using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LaunchListApi.Web.Utilities
{
    /// <summary>
    /// A custom MVC/WebApi input formatter to handle <code>Json</code> text in the request body (e.g. in a <code>POST</code> request). The default MVC/WebApi json input formatter doesn't like the way we're 
    /// formatting our body text, and in any case we want to bring it across as a string rather than have it go through the Json.Net formatting process, as its default configuration screws up dates as well. 
    /// </summary>
    public class RawJsonStringInputFormatter : InputFormatter
    {
        public RawJsonStringInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        /// <summary>
        /// Allow application/json to be processed as a raw string rather than being auto-munted by the JSON.NET formatter
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            if (contentType == "application/json")
                    return true;

            return false;
        }

        /// <summary>
        /// Handle text/plain or no content type for string results
        /// Handle application/octet-stream for byte[] results
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;


            if (contentType == "application/json")
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }




}
