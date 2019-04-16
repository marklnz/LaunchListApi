using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using LaunchListApi.Model;
using LaunchListApi.Services.Authorization;
using LaunchListApi.Model.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security;

namespace LaunchListApi.Web.Utilities
{
    public class AccessClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMemoryCache claimsCache, LaunchListApiContext dbContext)
        {
            // Get the name from the claims on the current user
            string userName = context.User.Identity.Name;

            // Set up the access rights cache and retrieve the current user's rights
            // TODO: Set the timeout according a config variable
            Func<List<UserClaim>> rightsAccessor = () => GetUserClaims(context.User.Identity.Name, dbContext);
            string key = string.Format("userRightsCache_{0}", context.User.Identity.Name);
            List<UserClaim> claims = await claimsCache.GetOrCreateAsync(key, entry => {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return Task.FromResult(GetUserClaims(context.User.Identity.Name, dbContext)); }
            );

            // Add the user's access rights to the claims in the user identity on the HttpContext
            claims.ForEach(uc => ((ClaimsIdentity)(context.User.Identity)).AddClaim(new Claim(uc.ClaimType, uc.ClaimValue, ClaimValueTypes.String, "Ridewise")));
            
            // Call the next delegate/middleware in the pipeline
            await this._next(context);
        }

        private List<UserClaim> GetUserClaims(string userName, LaunchListApiContext dbContext)
        {
            // First get the user's specific claims from the db
            List<UserClaim> result;
            try
            {

                result = dbContext.UserClaims.Where(uc => uc.UserName.Equals(userName)).ToList();

                // TODO: Allow users to belong to multiple roles - load claims up from ALL roles
                // If they have a "role" claim (i.e. they belong to a Role) then add the claims of that role to the result also.
                if (result.Any(uc => uc.ClaimType == AuthorizationClaimTypes.Role))
                {
                    string roleName = result.FirstOrDefault(uc => uc.ClaimType.Equals(AuthorizationClaimTypes.Role, StringComparison.InvariantCultureIgnoreCase)).ClaimValue;
                    dbContext.RoleClaims.Where(rc => rc.RoleName == roleName).ToList().ForEach(rc => result.Add(new UserClaim() { ClaimType = rc.ClaimType, ClaimValue = rc.ClaimValue, UserName = userName }));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SecurityException("Unable to retrieve user claims from data store", ex);
            }
        }
    }

    public static partial class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAccessClaims(this IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                return builder.UseMiddleware<AccessClaimsMiddleware>();
            }
        }
    }
}