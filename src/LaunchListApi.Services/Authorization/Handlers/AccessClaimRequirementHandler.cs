using Microsoft.AspNetCore.Authorization;
using LaunchListApi.Services.Authorization.Requirements;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Authorization.Handlers
{
    /// <summary>
    /// An <see cref="IAuthorizationHandler"/> which handles AccessClaimRequirements and grants access if the user has the access claim the requirement specifies
    /// </summary>
    public class AccessClaimRequirementHandler : AuthorizationHandler<AccessClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessClaimRequirement requirement)
        {
            
            // Check if the user has an "authorization" type claim, with a value that matches the one specified by the requirement
            if (context.User.HasClaim(c => c.Type == AuthorizationClaimTypes.AuthorizationClaim && c.Value == requirement.ClaimValue))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
