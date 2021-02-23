// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("pg-read","Payment Gateway Read Transactions"),
                new ApiScope("pg-write","Payment Gateway Write Transactions"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId ="PaymentGatewayAPI",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {
                        new Secret("DF84B468-31FB-493C-A56A-A69C34ED80CE".Sha256())
                    },
                    AllowedScopes =
                    {
                        "pg-read",
                        "pg-write"
                    }
                }
            };
    }
}