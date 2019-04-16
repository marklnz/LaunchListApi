using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LaunchListApi.Model.DataAccess;
using LaunchListApi.Services.Mediator.DomainEvents;
using LaunchListApi.Services.Utilities;
using LaunchListApi.Web.Utilities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LaunchListApi.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services. 
            // Add Mvc, including our custom Json input formatters.
            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2) // we're using dotnet core 2.2
                .AddAuthorization()  
                .AddJsonFormatters()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

                    /* TODO: See if there is a better way to deal with this - see what we customise/extend with the model binding framework
                       TODO: Add in the list of custom type converters. These are used only in the INPUT to our controllers, or in the event that an action
                             has no default output formatter defined */
                    // example: options.SerializerSettings.Converters.Add(new ClientDetailJsonConverter());
                    
                });

            // Add the framework's HttpContextAccessor as we will need occasional access to the HttpContext from outside the framework (e.g. in our services layer)
            services.AddHttpContextAccessor();

            // Add versioning readers for both http header and querystring param, so either one can be used by a client
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader(),
                    new HeaderApiVersionReader()
                    {
                        HeaderNames = { "api-version" }
                    });
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0, "alpha");
                options.ReportApiVersions = true;
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            });

            // TODO: Enable this when we have the identity provider sorted out 
            //// Add authentication service that checks for Bearer token in request header and uses Identity Server to validate it
            //services.AddAuthentication("Bearer")
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = Configuration.GetValue<string>("IdentityServerOptions:Authority");
            //        options.RequireHttpsMetadata = Configuration.GetValue<bool>("IdentityServerOptions:RequireHttpsMetadata");
            //        options.ApiName = Configuration.GetValue<string>("IdentityServerOptions:ApiName");
            //        options.SupportedTokens = Enum.Parse<SupportedTokens>(Configuration.GetValue<string>("IdentityServerOptions:SupportedTokens"));
            //    });

            services.AddCors(options =>
            {
                // TODO: Consider Alternative approach, reading CORS config from apSettings
                // TODO Decide what the final set or origins should be
                options.AddPolicy("AllowLocalDevOrigin", builder => builder.WithOrigins("http://localhost:4200", "http://localhost:4201", "https://localhost:4202")
                                                                           .AllowAnyHeader()
                                                                           .AllowAnyMethod());

                // TODO: Enable the following to add policies for other environments
                //options.AddPolicy("AllowAlphaOrigin", builder => builder.WithOrigins("https://ridewise2alpha.azurewebsites.net", "https://ridewise2alphaserver.azurewebsites.net")
                //                                                           .AllowAnyHeader()
                //                                                           .AllowAnyMethod());

                //options.AddPolicy("AllowBetaOrigin", builder => builder.WithOrigins("https://ridewise2beta.azurewebsites.net", "https://ridewise2betaserver.azurewebsites.net")
                //                                                           .AllowAnyHeader()
                //                                                           .AllowAnyMethod());
            });

            // Add the data context to the DI container
            services.AddDbContext<LaunchListApiContext>(options => options.UseSqlServer(Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString"), x => x.MigrationsAssembly("LaunchListApi.Model")));

            // Register MemoryCache - used for the user's access claims so we don't go to the database just to get these, on every request
            services.AddMemoryCache();

            // Add authorization policies and services
            services.AddAuthorizationServices();

            // Add Mediatr - using this for an in-process "message bus" - pass commands, queries and events between components in the application, keeping everything separated
            // The type referenced is only used to provide a way to get the System.Reflection.Assembly type that corresponds to the Ridewise.Services assembly, since this is where all the
            // mediation classes (requests, handlers, etc) are defined
            services.AddMediatR(new Assembly[] { Assembly.GetAssembly(typeof(DomainEventNotificationBase)) });

            // Add utility services 
            services.AddScoped<CurrentUser>();

            // Add an instance of the OutputSerialisationContext class
            services.AddScoped<OutputSerialisationContext>();

            // TODO: Enable this when we have output serializers set up
            //// Add in the output serialisers
            //// Agency related serialisers
            //services.AddSingleton<IOutputSerialiser>(s => { return new AgencyDetailsDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new AgencyListDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new AgencyNameListOutputSerialiser(); });
            //// Client related serialisers
            //services.AddSingleton<IOutputSerialiser>(s => { return new ClientListDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new ClientDetailsDefaultSerialiser(); });
            //// Transport Operator related serialisers
            //services.AddSingleton<IOutputSerialiser>(s => { return new TransportOperatorListDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new TransportOperatorDetailsDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new VehicleDetailsDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new VehicleListDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new DriverDetailsDefaultSerialiser(); });
            //services.AddSingleton<IOutputSerialiser>(s => { return new DriverListDefaultSerialiser(); });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Do development environment-only setup

                // Seed database
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<LaunchListApiContext>();
                    // TODO: Seed database
                    //DataSeeder.SeedAll(context).Wait();
                }
            }

            // Enforce HTTP Strict Transport Security Protocol (HSTS) - see here for more: https://www.owasp.org/index.php/HTTP_Strict_Transport_Security_Cheat_Sheet
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            // Redirect all HTTP requests to HTTPS instead
            app.UseHttpsRedirection();

            // Use the specific CorsPolicy named in the appsettings configuration for the running environment
            app.UseCors(Configuration.GetValue<string>("CorsPolicy"));

            // Create an exceptionhandler middleware that checks unhandled exceptions and always return 500 error, but maintains the CORS headers.
            app.Use500InternalServerErrorExceptionHandlerWithCorsHeaders(Configuration);

            app.UseAuthentication();

            // Call our middleware that populates the current user identity's claims from either our cache or the database (if cache has expired)
            app.UseAccessClaims();

            // Enable our Accepts header-based DTO serialisation option.
            app.UseDtoSerialisation();

            app.UseMvc();


        }
    }
}
