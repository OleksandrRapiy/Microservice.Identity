using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Microservice.Identity.Application.Configurations
{
    public static class IdentityServerConfigurations
    {
        public static class Scopes
        {
            public const string MicroserviceData = "microservice.data.api";
            public const string IdentityData = "microservice.identity.api";
        }

        private static class Clients
        {
            public const string InternalClient = "internalClient";
            public const string ExternalClient = "externalClient";
        }

        public static ICollection<string> DefaultAllowedScopes = new List<string>
        {
            Scopes.IdentityData,
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            IdentityServerConstants.StandardScopes.Email,
        };

        public static IEnumerable<ApiScope> ApiScopes =>
         new List<ApiScope>
         {
            new ApiScope( Scopes.IdentityData, "Full access to microservice.identity.api"),
            new ApiScope( Scopes.MicroserviceData, "Full access to microservice.data.api"),
            new ApiScope( IdentityServerConstants.StandardScopes.OpenId, "Open Id"),
            new ApiScope( IdentityServerConstants.StandardScopes.Profile, "Profile"),
            new ApiScope( IdentityServerConstants.StandardScopes.Email, "Email")
         };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>()
        {
            new ApiResource(Scopes.IdentityData, "Full access to microservice.identity.api")
            {
                Description = "Full access to microservice.identity.api",
                Scopes =
                {
                    Scopes.IdentityData,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                },
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256(), "secret") },
                UserClaims = new List<string> { JwtClaimTypes.Role, JwtClaimTypes.Email,  JwtClaimTypes.FamilyName, JwtClaimTypes.GivenName }
            },
            new ApiResource(Scopes.MicroserviceData, "Full access to microservice.data.api")
            {
                Description = "Full access to microservice.data.api",
                Scopes =
                {
                    Scopes.MicroserviceData,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                },
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256(), "secret") },
                UserClaims = new List<string> {  JwtClaimTypes.Role, JwtClaimTypes.Email,  JwtClaimTypes.FamilyName, JwtClaimTypes.GivenName }
            }
        };

        public static string InternalClientSecret = "internalClientSecret";
        public static Client InternalClient = new Client
        {
            ClientId = Clients.InternalClient,
            ClientName = Clients.InternalClient,
            SlidingRefreshTokenLifetime = 1296000, //15 days
            RefreshTokenExpiration = TokenExpiration.Sliding,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            AccessTokenLifetime = 3600, // one hour
            UpdateAccessTokenClaimsOnRefresh = true,
            RequireConsent = false,
            AllowPlainTextPkce = false,
            AllowOfflineAccess = true,
            ClientSecrets =
            {
                new Secret(InternalClientSecret.ToSha256())
            },
            AllowedScopes = DefaultAllowedScopes
        };

        public static string ExternalClientSecret = "externalClientSecret";
        public static Client ExternalClient = new Client
        {
            ClientId = Clients.ExternalClient,
            ClientName = Clients.ExternalClient,
            SlidingRefreshTokenLifetime = 1296000, //15 days
            RefreshTokenExpiration = TokenExpiration.Sliding,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            AccessTokenLifetime = 3600, // one hour
            UpdateAccessTokenClaimsOnRefresh = true,
            RequireConsent = false,
            AllowPlainTextPkce = false,
            AllowOfflineAccess = true,
            ClientSecrets =
            {
                new Secret(ExternalClientSecret.ToSha256())
            },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                Scopes.MicroserviceData,
            }
        };

        public static IEnumerable<Client> GetClients
            => new List<Client>
            {
                InternalClient,
                ExternalClient
            };
    }
}
