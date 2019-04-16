using System.Security.Claims;

namespace LaunchListApi.Services.Utilities
{
    public class CurrentUser
    {
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;

        /// This constructor is intended for use by mocking framework in unit tests only.
        public CurrentUser()
        {
        }

        public CurrentUser(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> for the current user.
        /// </summary>
        public virtual ClaimsPrincipal Principal => _httpContext?.User;

        /// <summary>
        /// Returns the username of the current user
        /// </summary>
        public virtual string UserName
        {
            get
            {
                return Principal.Identity.Name;
            }
        }
    }
}
