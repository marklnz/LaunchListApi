using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LaunchListApi.Services.Tests
{
    /// <summary>
    /// <para>
    /// A "fake" implementation of the <see cref="IAuthorizationService"/>, for use when unit testing the service layer. 
    /// Simply create a new instance of this class, and pass it into the correct parameter of the constructor for the service class being tested.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is intended for use when unit testing any service layer class that requires the IAuthorizationService injected into its constructor. This class can be used in instead of setting
    /// up a DI services container and adding the framework's authorization services into it. 
    /// When using the DI container approach and using the framework's DefaultAuthorizationService, all the authorization requirement and requirement handler classes need to also be 
    /// configured in the DI container, in order that the authorization code in each service method allows processing to continue and therefore be tested.
    /// That approach results in a lot more set up code for each test fixture and each test, the tests will take longer to run, and they will use more memory also. 
    /// </para>
    /// <para>
    /// Also, because we are testing the authorization logic elsewhere, we don't want to run that code when we're testing the functionality of the service classes themselves. 
    /// Therefore, this class always returns a Success result when AuthorizeAsync is called. Just pass a new instance straight into the constructor of the service class under test. 
    /// </para>
    /// </remarks>
    public class AlwaysAllowFakeAuthorizationService : IAuthorizationService
    {
        /// <summary>
        /// An empty implementation of the <see cref="IAuthorizationService"/> interface's <see cref="AuthorizeAsync(ClaimsPrincipal, object, IEnumerable{IAuthorizationRequirement})"/> method, 
        /// that always returns a success result. 
        /// </summary>
        /// <param name="user">Can be any <see cref="ClaimsPrincipal"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="resource">Can be any <see cref="object"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="requirements">Can be any <see cref="IEnumerable{IAuthorizationRequirement}"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <returns>An instance of the </returns>
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
           return Task.FromResult(AuthorizationResult.Success());
        }

        /// <summary>
        /// An empty implementation of the <see cref="IAuthorizationService"/> interface's <see cref="AuthorizeAsync(ClaimsPrincipal, object, IEnumerable{IAuthorizationRequirement})"/> method, 
        /// that always returns a success result. 
        /// </summary>
        /// <param name="user">Can be any <see cref="ClaimsPrincipal"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="resource">Can be any <see cref="object"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="policyName">Can be any <see cref="string"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <returns>An instance of the </returns>
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            return Task.FromResult(AuthorizationResult.Success());
        }
    }


    /// <summary>
    /// <para>
    /// A "fake" implementation of the <see cref="IAuthorizationService"/>, for use when unit testing the service layer, when tests require that Authorization is not granted.
    /// Simply create a new instance of this class, and pass it into the correct parameter of the constructor for the service class being tested.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is intended for use when unit testing any service layer class that requires the IAuthorizationService injected into its constructor. This class can be used in instead of setting
    /// up a DI services container and adding the framework's authorization services into it. 
    /// When using the DI container approach and using the framework's DefaultAuthorizationService, all the authorization requirement and requirement handler classes need to also be 
    /// configured in the DI container, in order that the authorization code in each service method allows processing to continue and therefore be tested.
    /// That approach results in a lot more set up code for each test fixture and each test, the tests will take longer to run, and they will use more memory also. 
    /// </para>
    /// <para>
    /// Also, because we are testing the authorization logic elsewhere, we don't want to run that code when we're testing the functionality of the service classes themselves. 
    /// Therefore, this class always returns a Fail result when AuthorizeAsync is called. Just pass a new instance straight into the constructor of the service class under test. 
    /// </para>
    /// </remarks>
    public class AlwaysDenyFakeAuthorizationService : IAuthorizationService
    {
        /// <summary>
        /// An empty implementation of the <see cref="IAuthorizationService"/> interface's <see cref="AuthorizeAsync(ClaimsPrincipal, object, IEnumerable{IAuthorizationRequirement})"/> method, 
        /// that always returns a failed result. 
        /// </summary>
        /// <param name="user">Can be any <see cref="ClaimsPrincipal"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="resource">Can be any <see cref="object"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="requirements">Can be any <see cref="IEnumerable{IAuthorizationRequirement}"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <returns>An instance of the </returns>
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(AuthorizationResult.Failed());
        }

        /// <summary>
        /// An empty implementation of the <see cref="IAuthorizationService"/> interface's <see cref="AuthorizeAsync(ClaimsPrincipal, object, IEnumerable{IAuthorizationRequirement})"/> method, 
        /// that always returns a failed result. 
        /// </summary>
        /// <param name="user">Can be any <see cref="ClaimsPrincipal"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="resource">Can be any <see cref="object"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <param name="policyName">Can be any <see cref="string"/> as it is never used by this method. Can also safely pass null to this parameter.</param>
        /// <returns>An instance of the </returns>
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            return Task.FromResult(AuthorizationResult.Failed());
        }
    }
}
