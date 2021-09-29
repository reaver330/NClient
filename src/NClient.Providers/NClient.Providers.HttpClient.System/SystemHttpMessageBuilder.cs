﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using NClient.Abstractions.HttpClients;
using NClient.Abstractions.Serialization;
using NClient.Providers.HttpClient.System.Builders;

namespace NClient.Providers.HttpClient.System
{
    internal class SystemHttpMessageBuilder : IHttpMessageBuilder<HttpRequestMessage, HttpResponseMessage>
    {
        private readonly ISerializer _serializer;
        private readonly IFinalHttpRequestBuilder _finalHttpRequestBuilder;
        private readonly IHttpClientExceptionFactory<HttpRequestMessage, HttpResponseMessage> _httpClientExceptionFactory;

        public SystemHttpMessageBuilder(
            ISerializer serializer,
            IFinalHttpRequestBuilder finalHttpRequestBuilder,
            IHttpClientExceptionFactory<HttpRequestMessage, HttpResponseMessage> httpClientExceptionFactory)
        {
            _serializer = serializer;
            _finalHttpRequestBuilder = finalHttpRequestBuilder;
            _httpClientExceptionFactory = httpClientExceptionFactory;
        }
        
        public Task<HttpRequestMessage> BuildRequestAsync(HttpRequest httpRequest)
        {
            var parameters = httpRequest.Parameters
                .ToDictionary(x => x.Name, x => x.Value!.ToString());
            var uri = new Uri(QueryHelpers.AddQueryString(httpRequest.Resource.ToString(), parameters));

            var httpRequestMessage = new HttpRequestMessage { Method = httpRequest.Method, RequestUri = uri };

            httpRequestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(_serializer.ContentType));

            if (httpRequest.Body != null)
            {
                var body = _serializer.Serialize(httpRequest.Body);
                httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, _serializer.ContentType);
            }

            foreach (var header in httpRequest.Headers)
            {
                httpRequestMessage.Headers.Add(header.Name, header.Value);
            }

            return Task.FromResult(httpRequestMessage);
        }

        public async Task<HttpResponse> BuildResponseAsync(HttpRequest httpRequest, HttpResponseMessage response)
        {
            var exception = TryGetException(response);
            
            var finalHttpRequest = await _finalHttpRequestBuilder
                .BuildAsync(httpRequest, response.RequestMessage)
                .ConfigureAwait(false);

            var content = response.Content is null 
                ? new HttpResponseContent() 
                : new HttpResponseContent(
                    await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false),
                    new HttpResponseContentHeaderContainer(response.Content.Headers));

            var httpResponse = new HttpResponse(finalHttpRequest)
            {
                Headers = new HttpResponseHeaderContainer(response.Headers),
                Content = content,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusCode.ToString(),
                ResponseUri = response.RequestMessage.RequestUri,
                ErrorMessage = exception?.Message,
                ProtocolVersion = response.Version
            };

            httpResponse.ErrorException = exception is not null
                ? _httpClientExceptionFactory.HttpRequestFailed(response.RequestMessage, response, exception)
                : null;

            return httpResponse;
        }
        
        private static Exception? TryGetException(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
                return null;
            }
            catch (HttpRequestException e)
            {
                return e;
            }
        }
    }
}
