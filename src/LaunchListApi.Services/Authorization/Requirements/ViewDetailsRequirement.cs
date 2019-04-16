using Microsoft.AspNetCore.Authorization;
using LaunchListApi.Model;

namespace LaunchListApi.Services.Authorization.Requirements
{
    public class ViewDetailsRequirement<T>: IAuthorizationRequirement where T: class, IAggregateRoot, new()
    {
    }
}
