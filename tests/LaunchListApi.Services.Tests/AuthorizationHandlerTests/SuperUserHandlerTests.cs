using Microsoft.AspNetCore.Authorization;
using LaunchListApi.Services.Authorization;
using LaunchListApi.Services.Authorization.Handlers;
using LaunchListApi.Services.Authorization.Requirements;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LaunchListApi.Services.Tests.AuthorizationHandlerTests
{
    public class SuperUserHandlerTests
    {
        // Test that the handler returns Success when the current user has the required claim.
        [Fact]
        public async void HandleAsync_UserHasSuperUserClaim_ReturnsSuccessResult()
        {
            //Arrange
            SuperUserHandler handler = new SuperUserHandler();

            ClaimsPrincipal testUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(AuthorizationClaimTypes.AuthorizationClaim, AuthorizationClaimValues.SuperUser) }));

            AuthorizationHandlerContext authorizationContext = new AuthorizationHandlerContext(new IAuthorizationRequirement[] { new AccessClaimRequirement("test") }, testUser, null);

            // Act
            await handler.HandleAsync(authorizationContext);

            //Assert
            Assert.True(authorizationContext.HasSucceeded);
            Assert.False(authorizationContext.HasFailed);
        }

        // Test that the handler does not return Success when the current user has the required claim. Note that it does not FAIL the context, just refuses to mark it 
        // as successful. This is by design, as one requirement can (and is) handled by more than one handler. 
        [Fact]
        public async void HandleAsync_UserDoesNotHaveSuperUserClaim_ReturnsFailureResult()
        {
            //Arrange
            SuperUserHandler handler = new SuperUserHandler();

            ClaimsPrincipal testUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(AuthorizationClaimTypes.AuthorizationClaim, "someotherclaim") }));

            AuthorizationHandlerContext authorizationContext = new AuthorizationHandlerContext(new IAuthorizationRequirement[] { new AccessClaimRequirement("test") }, testUser, null);

            // Act
            await handler.HandleAsync(authorizationContext);

            //Assert
            Assert.False(authorizationContext.HasSucceeded);
            Assert.False(authorizationContext.HasFailed);
        }
    }
}
