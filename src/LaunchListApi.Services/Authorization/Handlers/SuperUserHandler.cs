using Microsoft.AspNetCore.Authorization;
using LaunchListApi.Services.Authorization.Requirements;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Authorization.Handlers
{
    /// <summary>
    /// An <see cref="IAuthorizationHandler"/> which handles all AccessClaimRequirements regardless of the claim value, and grants access if the user has Super User access
    /// </summary>
    public class SuperUserHandler : AuthorizationHandler<AccessClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessClaimRequirement requirement)
        {
            // Check if the user has an "authorization" type claim, with a value of "superuser"
            if (context.User.HasClaim(c => c.Type == AuthorizationClaimTypes.AuthorizationClaim && c.Value == AuthorizationClaimValues.SuperUser))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
