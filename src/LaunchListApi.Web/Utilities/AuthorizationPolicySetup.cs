using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using LaunchListApi.Model;
using LaunchListApi.Services.Authorization;
using LaunchListApi.Services.Authorization.Handlers;
using LaunchListApi.Services.Authorization.Requirements;

namespace LaunchListApi.Web.Utilities
{
    public static class AuthorizationPolicySetup
    {
        public static void AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorizationPolicies();

            services.AddAuthorizationHandlers();
        }

        private static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            // TODO: Possibly replace this with a custom policy loader
            services.AddAuthorization(options =>
            {
                options.AddClientAuthorizationPolicies();
                options.AddAgencyAuthorizationPolicies();
                options.AddTransportOperatorAuthorizationPolicies();
                options.AddTripAuthorizationPolicies();
            });
        }

        private static void AddAuthorizationHandlers(this IServiceCollection services)
        {
            // Add authorization handlers here
            // A special - NEVER TO BE USED IN PRODUCTION - "Always Allow" Handler
            services.AddSingleton<IAuthorizationHandler, AlwaysAllowHandler>();

            // General access requirement handlers (e.g. user has permission to access the Edit Clients function)
            services.AddSingleton<IAuthorizationHandler, SuperUserHandler>();
            services.AddSingleton<IAuthorizationHandler, AccessClaimRequirementHandler>();

            // Resource based Authorization handlers (e.g. user has permission to access the specific Client they're trying to edit)
            //services.AddSingleton<IAuthorizationHandler, ViewClientListHandler>();
            //services.AddSingleton<IAuthorizationHandler, ViewClientDetailsHandler>();
            //services.AddSingleton<IAuthorizationHandler, ModifyClientHandler>();

            //services.AddSingleton<IAuthorizationHandler, ViewAgencyListHandler>();
            //services.AddSingleton<IAuthorizationHandler, ViewAgencyDetailsHandler>();
            //services.AddSingleton<IAuthorizationHandler, ModifyAgencyHandler>();

            //services.AddSingleton<IAuthorizationHandler, ViewTransportOperatorListHandler>();
            //services.AddSingleton<IAuthorizationHandler, ViewTransportOperatorDetailsHandler>();
            //services.AddSingleton<IAuthorizationHandler, ModifyTransportOperatorHandler>();

            //services.AddSingleton<IAuthorizationHandler, ModifyTripHandler>();
        }

        private static void AddClientAuthorizationPolicies(this AuthorizationOptions options)
        {
            // See the AuthorizationHandlers in the Ridewise.Services project to determine what the access restrictions are for each specific case. 
            // Note that a policies built up here with multiple requirements will allow access if any ONE of the requirements is met. 
            //options.AddPolicy(PolicyNames.ViewClientList, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewClientList)); // user has access claim that allows them to View the Client List.
            //    policy.Requirements.Add(new ViewListRequirement<Client>()); // user meets the requirements of the ViewClientList resource-based handler (resource based access)
            //});

            //options.AddPolicy(PolicyNames.ViewClientCount, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewClientList)); // i.e. if the user can retrieve a list of clients then they can get a count also
            //});

            //options.AddPolicy(PolicyNames.CreateClient, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.CreateClient)); // User has the CreateClient access claim
            //});

            //options.AddPolicy(PolicyNames.ViewClientDetails, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewClientDetails)); // User has the UpdateClient access claim
            //    policy.Requirements.Add(new ViewDetailsRequirement<Client>()); // User meets the resource-based authorisation requirements for ViewClientDetails
            //});

            //options.AddPolicy(PolicyNames.UpdateClient, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.UpdateClient)); // User has the UpdateClient access claim
            //    policy.Requirements.Add(new ModifyAggregateRequirement<Client>()); // User meets the resource-based authorisation requirements for ModifyClient
            //});

            //options.AddPolicy(PolicyNames.DeleteClient, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.DeleteClient)); // User has the DeleteClient access claim
            //});
        }

        private static void AddAgencyAuthorizationPolicies(this AuthorizationOptions options)
        {
            // See the AuthorizationHandlers in the Ridewise.Services project to determine what the access restrictions are for each specific case. 
            // Note that a policies built up here with multiple requirements will allow access if any ONE of the requirements is met. 
            //options.AddPolicy(PolicyNames.ViewAgencyList, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewAgencyList)); // user has access claim that allows them to View the Agency List.
            //    policy.Requirements.Add(new ViewListRequirement<Agency>()); // user meets the requirements of the ViewAgencyList resource-based handler (resource based access)
            //});

            //options.AddPolicy(PolicyNames.ViewAgencyCount, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewAgencyList)); // i.e. if the user can retrieve a list of Agencys then they can get a count also
            //});

            //options.AddPolicy(PolicyNames.CreateAgency, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.CreateAgency)); // User has the CreateAgency access claim
            //});

            //options.AddPolicy(PolicyNames.ViewAgencyDetails, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewAgencyDetails)); // User has the UpdateAgency access claim
            //    policy.Requirements.Add(new ViewDetailsRequirement<Agency>()); // User meets the resource-based authorisation requirements for ViewAgencyDetails
            //});

            //options.AddPolicy(PolicyNames.UpdateAgency, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.UpdateAgency)); // User has the UpdateAgency access claim
            //    policy.Requirements.Add(new ModifyAggregateRequirement<Agency>()); // User meets the resource-based authorisation requirements for ModifyAgency
            //});

            //options.AddPolicy(PolicyNames.DeleteAgency, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.DeleteAgency)); // User has the DeleteAgency access claim
            //});
        }

        private static void AddTransportOperatorAuthorizationPolicies(this AuthorizationOptions options)
        {
            // See the AuthorizationHandlers in the Ridewise.Services project to determine what the access restrictions are for each specific case. 
            // Note that a policies built up here with multiple requirements will allow access if any ONE of the requirements is met. 
            //options.AddPolicy(PolicyNames.ViewTransportOperatorList, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewTransportOperatorList)); // user has access claim that allows them to View the TransportOperator List.
            //    policy.Requirements.Add(new ViewListRequirement<TransportOperator>()); // user meets the requirements of the ViewTransportOperatorList resource-based handler (resource based access)
            //});

            //options.AddPolicy(PolicyNames.ViewTransportOperatorCount, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewTransportOperatorList)); // i.e. if the user can retrieve a list of TransportOperators then they can get a count also
            //});

            //options.AddPolicy(PolicyNames.CreateTransportOperator, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.CreateTransportOperator)); // User has the CreateTransportOperator access claim
            //});

            //options.AddPolicy(PolicyNames.ViewTransportOperatorDetails, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.ViewTransportOperatorDetails)); // User has the UpdateTransportOperator access claim
            //    policy.Requirements.Add(new ViewDetailsRequirement<TransportOperator>()); // User meets the resource-based authorisation requirements for ViewTransportOperatorDetails
            //});

            //options.AddPolicy(PolicyNames.UpdateTransportOperator, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.UpdateTransportOperator)); // User has the UpdateTransportOperator access claim
            //    policy.Requirements.Add(new ModifyAggregateRequirement<TransportOperator>()); // User meets the resource-based authorisation requirements for ModifyTransportOperator
            //});

            //options.AddPolicy(PolicyNames.DeleteTransportOperator, policy =>
            //{
            //    policy.Requirements.Add(new AccessClaimRequirement(AuthorizationClaimValues.DeleteTransportOperator)); // User has the DeleteTransportOperator access claim
            //});
        }

        private static void AddTripAuthorizationPolicies(this AuthorizationOptions options)
        {
            //options.AddPolicy(PolicyNames.CreateTrip, policy =>
            //{
            //    policy.Requirements.Add(new AlwaysAllowRequirement());
            //});

            //options.AddPolicy(PolicyNames.UpdateTrip, policy =>
            //{
            //    policy.Requirements.Add(new AlwaysAllowRequirement());
            //});
        }
    }
}
