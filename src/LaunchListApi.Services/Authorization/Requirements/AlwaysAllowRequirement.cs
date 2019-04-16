using Microsoft.AspNetCore.Authorization;

namespace LaunchListApi.Services.Authorization.Requirements
{
    public class AlwaysAllowRequirement: IAuthorizationRequirement
    {
    }
}
