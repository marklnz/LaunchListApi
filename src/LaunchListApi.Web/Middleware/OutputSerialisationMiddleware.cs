using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using LaunchListApi.Model;
using LaunchListApi.Services.Authorization;
using LaunchListApi.Model.DataAccess;
using LaunchListApi.Web.OutputSerialisers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Serilog;

namespace LaunchListApi.Web.Utilities
{
    /// <summary>
    /// Middleware that setsup the Output Serialisation Context
    /// </summary>
    public class OutputSerialisationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor for the <see cref="OutputSerialisationMiddleware"/> class
        /// </summary>
        /// <param name="next"></param>
        public OutputSerialisationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Configures the middleware.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="controllerContext"></param>
        /// <returns>A <see cref="Task"/> that allows this method to be run asynchronously via the <see langword="await">await</see> keyword.</returns>
        /// <remarks><para>We're expecting the request to either have NO mime type on it, in which case the type received here will have a full value of "*/*", or if we do have one, it must adhere to the following format:</para>
        /// <para>application/vnd.eyede.com.XXXXX+json</para>
        /// <para>In this format the XXXXX is the short name identifying the serialiser to be used, for example, the agency-name-list serialiser. In this case, the full correctly formatted mimetype would be:</para>
        /// <para>application/vnd.eyede.com.agency-name-list+json</para>
        /// </remarks>
        public async Task InvokeAsync(HttpContext context)
        {
            string expectedMimeTypeThisRequest = null;
            bool validMimeType = true;

            if (string.Equals(context.Request.Method, "GET", StringComparison.InvariantCultureIgnoreCase))
            {
                // Get the mime type for this request - will only have an IOutputSerialiser attached to the context if there is a match to this in the set of registered IOutputSerialisers
                var mimeType = context.Request.GetTypedHeaders().Accept.First();

                // If the mimeType here is * then we either have NO specified accepts header or we're asked for any type, which indicatea that the default serialiser should be used
                // Otherwise we need to check the mime type is in a valid format
                if (mimeType.Type.Value != "*")
                {
                    // If the suffix is NOT json then it's a bad format
                    // If the type is not "application" then it's a bad format
                    if (mimeType.Type.Value != "application" || mimeType.Suffix.Value != "json")
                        validMimeType = false;

                    // If it's not application/json then it must start with vnd.eyede.com else it's a bad format
                    if (mimeType.Facets.Count() != 4 || mimeType.Facets.First().Value != "x-vnd" || mimeType.Facets.Skip(1).First().Value != "eyede" || mimeType.Facets.Skip(2).First().Value != "com")
                        validMimeType = false;
                }

                // Record the value of the mime type requested (the final part of it, which is used to find the output serialiser)
                expectedMimeTypeThisRequest = mimeType.Facets.Last().Value;
            }

            // Create an instance of OutputSerialisationContext here and set it up with the correct serialiser
            var serialisationCtx = context.RequestServices.GetRequiredService<OutputSerialisationContext>();
            serialisationCtx.MimeType = expectedMimeTypeThisRequest;

            if (validMimeType)
                // If there is more than one IOutputSerialiser registered that handle the same "mime type" then take the first one found. If there are none found for this mime type, then set it to null.
                serialisationCtx.Serialiser = context.RequestServices.GetServices<IOutputSerialiser>().FirstOrDefault(s => s.MimeType == expectedMimeTypeThisRequest);

            // Set the MimeTypeHasMatchingSerialiser property to true if we've attached a serialiser to the context, false if not.
            serialisationCtx.MimeTypeHasMatchingSerialiser = serialisationCtx.Serialiser != null;
            
            // Call the next delegate/middleware in the pipeline
            await this._next(context);
        }
    }

    /// <summary>
    /// Extension methods that add custom middleware to the execution pipeline
    /// </summary>
    public static partial class MiddlewareExtensions
    {
        /// <summary>
        /// Extension method that adds a custom middleware that configures output serialisation based on the Mime Type in the request's Accepts header
        /// </summary>
        /// <param name="builder">An instance of <see cref="IApplicationBuilder"/></param>
        /// <returns>The instance of <see cref="IApplicationBuilder"/> provided, but with the custom middleware added</returns>
        public static IApplicationBuilder UseDtoSerialisation(this IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                return builder.UseMiddleware<OutputSerialisationMiddleware>();
            }
        }
    }

    /// <summary>
    /// Extension methods for the controllerbase class
    /// </summary>
    public static partial class ControllerBaseExtensions
    {
        /// <summary>
        /// ControllerBase extension method that returns an OkResult with the content formatted using a configured serialiser
        /// </summary>
        /// <param name="controller">The instance of ControllerBase being extended</param>
        /// <param name="Content">The content to serialise</param>
        /// <returns>An <see cref="OkObjectResult"/> constructed using either the custom-serialised version of <paramref name="Content"/> or the default-serialised version. 
        /// The serialisation performed will be dependent upon whether the OutputSerialisationContext has an instance of IOutputSerialiser or not</returns>
        public static OkObjectResult OkWithDto(this ControllerBase controller, object Content)
        {
            // Note: There are a LOT of null-safe checks done here, and this is largely to deal with the fact that during unit testing, there is no HttpContext unless you do a LOT of faffing around to set one up properly. 
            //       Therefore, we just resort to the default serialisation in unit testing. 

            // Get the output serialisation context
            var serialisationCtx = controller?.HttpContext?.RequestServices.GetService<OutputSerialisationContext>();

            try
            {
                // If there is an IOutputSerialiser and it can serialise this object, then use it to serialise
                if (serialisationCtx.MimeTypeHasMatchingSerialiser && serialisationCtx.Serialiser.CanSerialise(Content.GetType()))
                    return new OkObjectResult(serialisationCtx.Serialiser.Serialise(Content));
                else
                {
                    // Log a message indicating what happened
                    if (serialisationCtx.MimeTypeHasMatchingSerialiser)
                    {
                        Log.Warning($"Serialiser for [{serialisationCtx.MimeType}] unable to serialise provided content of type [{Content.GetType().FullName}]");
                    }
                    else if (serialisationCtx.MimeType != "*")
                    {
                        Log.Warning($"No serialiser found that matched mime type provided [{serialisationCtx.MimeType}]");
                    }
                    // Find out if the calling method has a "default serialiser" defined and use it if so. If not then rely on whatever default serialisation the framework provides.
                    DefaultOutputSerialiserAttribute defaultAttribute = controller.ControllerContext.ActionDescriptor.MethodInfo.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(DefaultOutputSerialiserAttribute)) as DefaultOutputSerialiserAttribute;

                    if (defaultAttribute != null)
                    {
                        // Get the default serialiser class type name and create an instance of it, then use it to serialise
                        IOutputSerialiser defaultSerialiser = Activator.CreateInstance(defaultAttribute.OutputSerialiserType) as IOutputSerialiser;
                        return new OkObjectResult(defaultSerialiser.Serialise(Content));
                    }
                    else
                    {
                        // Rely on the dotnet framework to serialise the content as we have no clues provided as to what we should do.
                        return new OkObjectResult(Content);
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG the exception
                Log.Error(ex, "An error occurred during response output serialisation");
                // Something bad happened when trying to serialise, so fall back on the dotnet framework standard serialisation, so that the content is at least sent back in some form.
                return new OkObjectResult(Content);
            }
        }
    }


    /// <summary>
    /// A custom attribute used to decorate action methods in order to specify which output serialiser to use by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultOutputSerialiserAttribute: Attribute
    {
        /// <summary>
        /// A <see cref="Type"/> indicating the output serialiser to use. The selected type must implement the <see cref="IOutputSerialiser"/> interface 
        /// </summary>
        public Type OutputSerialiserType { get; private set; }

        /// <summary>
        /// Constructor - accepts the <see cref="Type"/> to set the <see cref="OutputSerialiserType"/> property with.
        /// </summary>
        /// <param name="outputSerialiserType"></param>
        public DefaultOutputSerialiserAttribute(Type outputSerialiserType)
        {
            if (outputSerialiserType.GetInterfaces().Any(i => i.Name == typeof(IOutputSerialiser).Name))
                OutputSerialiserType = outputSerialiserType;
            else
                throw new ArgumentException("Error - the outputSerialiserType must implement the IOutputSerialiser interface.");
        }
    }

    /// <summary>
    /// A class containing state information relating to the serialisation of outbound DTOs
    /// </summary>
    public class OutputSerialisationContext
    {
        /// <summary>
        /// A <see cref="bool"/> value that indicates whether a registered IOutputSerialiser was found that matches the value of <see cref="MimeType"/>
        /// </summary>
        public bool MimeTypeHasMatchingSerialiser { get; set; }

        /// <summary>
        /// The DTO format that the request's accepts header has asked for the output to be serialised in
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The custom serialiser class (if any) that is registered to handle serialisation to the <see cref="MimeType"/> DTO format. 
        /// If no class is registered to handle that format, or the format is "*" (any, or unspecified) then this property will be null. 
        /// </summary>
        public IOutputSerialiser Serialiser { get; set; }
    }
}