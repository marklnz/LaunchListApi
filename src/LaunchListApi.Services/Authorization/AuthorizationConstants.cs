using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Authorization {
    public static class AuthorizationClaimTypes
    {
        public static string AuthorizationClaim = "authorization";
        public static string TenantId = "tenantid";
        public static string Role = "role";
        public static string AgencyGuid = "agencyguid";
        public static string TransportOperatorGuid = "transportoperatorguid";
    }

    public static class AuthorizationClaimValues
    {
        public static string SuperUser = nameof(AuthorizationClaimValues.SuperUser).ToLowerInvariant();

        //// Client
        //public static string ViewClientList = nameof(AuthorizationClaimValues.ViewClientList).ToLowerInvariant();
        //public static string ViewClientDetails = nameof(AuthorizationClaimValues.ViewClientDetails).ToLowerInvariant();
        //public static string CreateClient = nameof(AuthorizationClaimValues.CreateClient).ToLowerInvariant();
        //public static string UpdateClient = nameof(AuthorizationClaimValues.UpdateClient).ToLowerInvariant();
        //public static string DeleteClient = nameof(AuthorizationClaimValues.DeleteClient).ToLowerInvariant();

        //// Agency
        //public static string ViewAgencyList = nameof(AuthorizationClaimValues.ViewAgencyList).ToLowerInvariant();
        //public static string ViewAgencyDetails = nameof(AuthorizationClaimValues.ViewAgencyDetails).ToLowerInvariant();
        //public static string CreateAgency = nameof(AuthorizationClaimValues.CreateAgency).ToLowerInvariant();
        //public static string UpdateAgency = nameof(AuthorizationClaimValues.UpdateAgency).ToLowerInvariant();
        //public static string DeleteAgency = nameof(AuthorizationClaimValues.DeleteAgency).ToLowerInvariant();

        //// TransportOperator
        //public static string ViewTransportOperatorList = nameof(AuthorizationClaimValues.ViewTransportOperatorList).ToLowerInvariant();
        //public static string ViewTransportOperatorDetails = nameof(AuthorizationClaimValues.ViewTransportOperatorDetails).ToLowerInvariant();
        //public static string CreateTransportOperator = nameof(AuthorizationClaimValues.CreateTransportOperator).ToLowerInvariant();
        //public static string UpdateTransportOperator = nameof(AuthorizationClaimValues.UpdateTransportOperator).ToLowerInvariant();
        //public static string DeleteTransportOperator = nameof(AuthorizationClaimValues.DeleteTransportOperator).ToLowerInvariant();

        //// Trip
        //public static string CreateTrip = nameof(AuthorizationClaimValues.CreateTrip).ToLowerInvariant();
        //public static string UpdateTrip = nameof(AuthorizationClaimValues.UpdateTrip).ToLowerInvariant();

    }

    public static class PolicyNames
    {
        //// Client
        //public static string ViewClientList = nameof(PolicyNames.ViewClientList).ToLowerInvariant();
        //public static string ViewClientCount = nameof(PolicyNames.ViewClientCount).ToLowerInvariant();
        //public static string ViewClientDetails = nameof(PolicyNames.ViewClientDetails).ToLowerInvariant();
        //public static string CreateClient = nameof(PolicyNames.CreateClient).ToLowerInvariant();
        //public static string UpdateClient = nameof(PolicyNames.UpdateClient).ToLowerInvariant();
        //public static string DeleteClient = nameof(PolicyNames.DeleteClient).ToLowerInvariant();

        //// Agency
        //public static string ViewAgencyList = nameof(PolicyNames.ViewAgencyList).ToLowerInvariant();
        //public static string ViewAgencyCount = nameof(PolicyNames.ViewAgencyCount).ToLowerInvariant();
        //public static string ViewAgencyDetails = nameof(PolicyNames.ViewAgencyDetails).ToLowerInvariant();
        //public static string CreateAgency = nameof(PolicyNames.CreateAgency).ToLowerInvariant();
        //public static string UpdateAgency = nameof(PolicyNames.UpdateAgency).ToLowerInvariant();
        //public static string DeleteAgency = nameof(PolicyNames.DeleteAgency).ToLowerInvariant();

        //// TransportOperator
        //public static string ViewTransportOperatorList = nameof(PolicyNames.ViewTransportOperatorList).ToLowerInvariant();
        //public static string ViewTransportOperatorCount = nameof(PolicyNames.ViewTransportOperatorCount).ToLowerInvariant();
        //public static string ViewTransportOperatorDetails = nameof(PolicyNames.ViewTransportOperatorDetails).ToLowerInvariant();
        //public static string CreateTransportOperator = nameof(PolicyNames.CreateTransportOperator).ToLowerInvariant();
        //public static string UpdateTransportOperator = nameof(PolicyNames.UpdateTransportOperator).ToLowerInvariant();
        //public static string DeleteTransportOperator = nameof(PolicyNames.DeleteTransportOperator).ToLowerInvariant();

        //// Trip
        //public static string CreateTrip = nameof(PolicyNames.CreateTrip).ToLowerInvariant();
        //public static string UpdateTrip = nameof(PolicyNames.UpdateTrip).ToLowerInvariant();

    }
}
