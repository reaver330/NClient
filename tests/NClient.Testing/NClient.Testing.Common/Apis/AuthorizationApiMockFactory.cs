﻿using NClient.Providers.Authorization;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace NClient.Testing.Common.Apis
{
    public class AuthorizationApiMockFactory
    {
        public static IWireMockServer MockGetMethodWithAuth(int id, Token token)
        {
            var api = WireMockServer.Start();
            api.Given(Request.Create()
                    .WithPath("/api/authorization")
                    .WithHeader("Accept", "application/json")
                    .WithHeader("Authorization", $"{token.Scheme} {token.Value}")
                    .WithParam("id", id.ToString())
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(id));

            return api;
        }
    }
}
