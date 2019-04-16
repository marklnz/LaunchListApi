using LaunchListApi.Services.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Authorization.Handlers
{
    /// <summary>
    /// An <see cref="IAuthorizationHandler"/> which always allows access
    /// </summary>
    /// <remarks>NEVER, EVER use this in production. EVER. In fact if this is NOT the development environment, this will always DENY access</remarks>
    public class AlwaysAllowHandler : AuthorizationHandler<AlwaysAllowRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AlwaysAllowRequirement requirement)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Alpha")
                context.Succeed(requirement);
             
            return Task.CompletedTask;
        }
    }
}
