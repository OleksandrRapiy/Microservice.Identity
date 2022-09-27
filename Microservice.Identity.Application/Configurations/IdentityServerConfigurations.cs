using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Microservice.Identity.Application.Configurations
{
    public static class IdentityServerConfigurations
    {
        private static class Scopes
        {
            public const string MicroserviceData = "microservice.data.api";
        }

        private static class Clients
        {
            public const string InternalClient = "internal-client";
            public const string ExternalClient = "external-client";
        }

        public static ICollection<string> DefaultAllowedScopes = new List<string>
        {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            IdentityServerConstants.StandardScopes.Email,
            IdentityServerConstants.StandardScopes.OfflineAccess,
            Scopes.MicroserviceData,
        };


        public static IEnumerable<ApiScope> ApiScopes =>
         new List<ApiScope>
         {
            new ApiScope(Scopes.MicroserviceData, "Full access to microservice.data.api"),
         };

        public static string InternalClientSecret = "internal-client-secret";
        public static Client InternalClient = new Client
        {
            ClientId = Clients.InternalClient,
            ClientName = Clients.InternalClient,
            SlidingRefreshTokenLifetime = 1296000, //15 days
            RefreshTokenExpiration = TokenExpiration.Sliding,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AccessTokenLifetime = 3600, // one hour
            UpdateAccessTokenClaimsOnRefresh = true,
            RequireConsent = false,
            AllowPlainTextPkce = false,
            AllowOfflineAccess = true,
            ClientSecrets =
            {
                new Secret(InternalClientSecret.ToSha256())
            },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OfflineAccess,
                IdentityServerConstants.StandardScopes.Address,
                IdentityServerConstants.StandardScopes.Phone,
                Scopes.MicroserviceData,
            }
        };

        public static IEnumerable<Client> GetClients
            => new List<Client>
            {
                InternalClient
            };
    }
}
