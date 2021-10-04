﻿using System.Threading.Tasks;
using FluentAssertions;
using NClient.Abstractions.Builders;
using NClient.Providers.HttpClient.RestSharp;
using NClient.Providers.HttpClient.System;
using NClient.Testing.Common.Apis;
using NClient.Testing.Common.Clients;
using NClient.Tests.Clients;
using NUnit.Framework;

namespace NClient.Api.Tests.CustomClientUseCases
{
    [Parallelizable]
    public class HttpClientTest
    {
        private INClientHttpClientBuilder<IBasicClientWithMetadata> _optionalBuilder = null!;
        private BasicApiMockFactory _api = null!;

        [SetUp]
        public void SetUp()
        {
            _api = new BasicApiMockFactory(5029);
            _optionalBuilder = new CustomNClientBuilder().For<IBasicClientWithMetadata>(_api.ApiUri.ToString());
        }

        [Test]
        public async Task CustomNClientBuilder_WithRestSharp_NotThrow()
        {
            const int id = 1;
            using var api = _api.MockGetMethod(id);
            var client = _optionalBuilder
                .UsingRestSharpHttpClient()
                .UsingJsonSerializer()
                .EnsuringRestSharpSuccess()
                .Build();
            
            var response = await client.GetAsync(id);

            response.Should().Be(id);
        }
    }
}
