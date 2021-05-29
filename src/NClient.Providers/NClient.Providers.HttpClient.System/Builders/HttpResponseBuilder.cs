﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NClient.Abstractions.Exceptions.Factories;
using NClient.Abstractions.HttpClients;

namespace NClient.Providers.HttpClient.System.Builders
{
    internal interface IHttpResponseBuilder
    {
        Task<HttpResponse> BuildAsync(
            HttpRequest request, HttpResponseMessage httpResponseMessage, Exception? exception = null);
    }

    internal class HttpResponseBuilder : IHttpResponseBuilder
    {
        public async Task<HttpResponse> BuildAsync(
            HttpRequest request, HttpResponseMessage httpResponseMessage, Exception? exception = null)
        {
            var headers = httpResponseMessage.Headers
                .Select(x => new HttpHeader(x.Key!, x.Value?.FirstOrDefault() ?? ""))
                .ToArray();
            var contentHeaders = httpResponseMessage.Content.Headers
                .Select(x => new HttpHeader(x.Key!, x.Value?.FirstOrDefault() ?? ""))
                .ToArray();
            var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var nclientException = exception is not null
                ? OuterExceptionFactory.HttpRequestFailed(httpResponseMessage.StatusCode, exception.Message, content, exception)
                : null;

            return new HttpResponse(request)
            {
                ContentType = httpResponseMessage.Content.Headers.ContentType?.MediaType,
                ContentLength = httpResponseMessage.Content.Headers.ContentLength,
                ContentEncoding = httpResponseMessage.Content.Headers.ContentEncoding.FirstOrDefault(),
                Content = content,
                StatusCode = httpResponseMessage.StatusCode,
                StatusDescription = httpResponseMessage.StatusCode.ToString(),
                ResponseUri = httpResponseMessage.RequestMessage.RequestUri,
                Server = httpResponseMessage.Headers.Server?.ToString(),
                Headers = headers.Concat(contentHeaders).ToArray(),
                ErrorMessage = nclientException?.Message,
                ErrorException = nclientException,
                ProtocolVersion = httpResponseMessage.Version
            };
        }
    }
}