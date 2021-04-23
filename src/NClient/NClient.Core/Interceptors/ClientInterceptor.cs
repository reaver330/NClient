﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using NClient.Abstractions.HttpClients;
using NClient.Abstractions.Resilience;
using NClient.Core.Exceptions;
using NClient.Core.Exceptions.Factories;
using NClient.Core.HttpClients;
using NClient.Core.Interceptors.ClientInvocations;
using NClient.Core.MethodBuilders;
using NClient.Core.RequestBuilders;
using AsyncInterceptorBase = NClient.Core.Castle.AsyncInterceptorBase;

namespace NClient.Core.Interceptors
{
    internal class ClientInterceptor<T> : AsyncInterceptorBase
    {
        private readonly IResilienceHttpClientProvider _resilienceHttpClientProvider;
        private readonly IClientInvocationProvider _clientInvocationProvider;
        private readonly IMethodBuilder _methodBuilder;
        private readonly IRequestBuilder _requestBuilder;
        private readonly Type? _controllerType;
        private readonly ILogger<T>? _logger;

        public ClientInterceptor(
            IResilienceHttpClientProvider resilienceHttpClientProvider,
            IClientInvocationProvider clientInvocationProvider,
            IMethodBuilder methodBuilder,
            IRequestBuilder requestBuilder,
            Type? controllerType = null,
            ILogger<T>? logger = null)
        {
            _resilienceHttpClientProvider = resilienceHttpClientProvider;
            _clientInvocationProvider = clientInvocationProvider;
            _methodBuilder = methodBuilder;
            _requestBuilder = requestBuilder;
            _controllerType = controllerType;
            _logger = logger;
        }

        protected override async Task InterceptAsync(
            IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> _)
        {
            var requestId = Guid.NewGuid();
            await InvokeWithLoggingExceptionsAsync(ProcessInvocationAsync<HttpResponse>, invocation, requestId).ConfigureAwait(false);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(
            IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> _)
        {
            var requestId = Guid.NewGuid();
            return await InvokeWithLoggingExceptionsAsync(ProcessInvocationAsync<TResult>, invocation, requestId).ConfigureAwait(false);
        }

        private async Task<TResult> ProcessInvocationAsync<TResult>(IInvocation invocation, Guid requestId)
        {
            using var loggingScope = _logger?.BeginScope("Processing request {requestId}.", requestId);

            var clientInvocation = _clientInvocationProvider.Get(interfaceType: typeof(T), _controllerType, invocation);
            var clientMethod = _methodBuilder.Build(clientInvocation.ClientType, clientInvocation.MethodInfo);
            var request = _requestBuilder.Build(requestId, clientMethod, clientInvocation.MethodArguments);
            var result = await ExecuteRequestAsync<TResult>(request, clientInvocation.ResiliencePolicyProvider)
                .ConfigureAwait(false);

            _logger?.LogDebug("Processing request finished. Request id: '{requestId}'.", requestId);
            return result;
        }

        private async Task<TResult> ExecuteRequestAsync<TResult>(HttpRequest request, IResiliencePolicyProvider? resiliencePolicyProvider)
        {
            var responseBodyType = typeof(HttpResponse).IsAssignableFrom(typeof(TResult)) && typeof(TResult).IsGenericType
                ? typeof(TResult).GetGenericArguments().First()
                : typeof(HttpResponse) == typeof(TResult)
                    ? null
                    : typeof(TResult);

            var response = await _resilienceHttpClientProvider
                .Create(resiliencePolicyProvider)
                .ExecuteAsync(request, responseBodyType)
                .ConfigureAwait(false);

            if (typeof(HttpResponse).IsAssignableFrom(typeof(TResult)))
                return (TResult)(object)response;
            if (!response.IsSuccessful)
                throw OuterExceptionFactory.HttpRequestFailed(response.StatusCode, response.ErrorMessage);
            return (TResult)response.GetType().GetProperty("Value")!.GetValue(response);
        }

        private async Task<TResult> InvokeWithLoggingExceptionsAsync<TResult>(
            Func<IInvocation, Guid, Task<TResult>> processInvocation, IInvocation invocation, Guid requestId)
        {
            try
            {
                return await processInvocation(invocation, requestId).ConfigureAwait(false);
            }
            catch (NClientException e)
            {
                _logger?.LogError(e, "Processing request error. Request id: '{requestId}'.", requestId);
                throw;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Unexpected processing request error. Request id: '{requestId}'.", requestId);
                throw;
            }
        }
    }
}
