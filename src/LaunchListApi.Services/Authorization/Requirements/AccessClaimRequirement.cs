using Microsoft.AspNetCore.Authorization;

namespace LaunchListApi.Services.Authorization.Requirements
{
    /// <summary>
    /// An <see cref="IAuthorizationRequirement"></see> that specifies the claim value that is required for access to a given resource. The claim type is always "authorization".
    /// </summary>
    public class AccessClaimRequirement: IAuthorizationRequirement
    {
        public string ClaimValue { get; }

        public AccessClaimRequirement(string claimValue)
        {
            ClaimValue = claimValue;
        }
    }
}
