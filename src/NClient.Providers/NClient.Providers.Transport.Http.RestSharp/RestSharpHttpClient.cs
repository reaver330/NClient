﻿using System.Threading.Tasks;
using NClient.Abstractions.Providers.Transport;
using RestSharp;
using RestSharp.Authenticators;

namespace NClient.Providers.Transport.Http.RestSharp
{
    internal class RestSharpHttpClient : IHttpClient<IRestRequest, IRestResponse>
    {
        private readonly IAuthenticator? _authenticator;

        public RestSharpHttpClient(IAuthenticator? authenticator = null)
        {
            _authenticator = authenticator;
        }

        public async Task<IRestResponse> ExecuteAsync(IRestRequest request)
        {
            var restClient = new RestClient
            {
                Authenticator = _authenticator
            };
            
            return await restClient.ExecuteAsync(request).ConfigureAwait(false);
        }
    }
}
