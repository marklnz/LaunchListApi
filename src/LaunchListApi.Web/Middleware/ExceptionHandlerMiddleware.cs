using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace LaunchListApi.Web.Utilities
{
    public static class ExceptionHandlerMiddleware
    {
        public static IApplicationBuilder Use500InternalServerErrorExceptionHandlerWithCorsHeaders(this IApplicationBuilder app, IConfiguration configuration)
        {
            return app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                    if (error != null && error.Error != null)
                    {
                        // For all unhandled exceptions, return a 500, again with no message body - perhaps look at the merits of changing this at some point
                        context.Response.StatusCode = (int)(StatusCodes.Status500InternalServerError);

                        // Set the headers in accordance with the CORS policy
                        var corsPolicy = await appBuilder.ApplicationServices.GetService<ICorsPolicyProvider>().GetPolicyAsync(context, configuration.GetValue<string>("CorsPolicy"));
                        if (corsPolicy != null)
                        {
                            var corsService = appBuilder.ApplicationServices.GetService<ICorsService>();
                            var corsResult = corsService.EvaluatePolicy(context, corsPolicy);
                            corsService.ApplyResult(corsResult, context.Response);
                        }
                    }
                    else await next();
                });
            });
        }
    }
}
