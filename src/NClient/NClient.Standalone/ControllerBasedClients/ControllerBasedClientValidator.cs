﻿using System;
using Castle.DynamicProxy;
using NClient.Core.Helpers.ObjectToKeyValueConverters;
using NClient.Core.HttpClients;
using NClient.Core.Interceptors;
using NClient.Core.Interceptors.ClientInvocations;
using NClient.Core.MethodBuilders;
using NClient.Core.MethodBuilders.Providers;
using NClient.Core.RequestBuilders;
using NClient.Core.Resilience;
using NClient.Core.Validation;
using NClient.Mappers;

namespace NClient.ControllerBasedClients
{
    internal class ControllerBasedClientValidator
    {
        public void Ensure<TInterface, TController>(IProxyGenerator proxyGenerator)
            where TInterface : class
            where TController : TInterface
        {
            var clientInvocationProvider = new ClientInvocationProvider(proxyGenerator);
            
            var attributeMapper = new AspNetAttributeMapper();
            var pathAttributeProvider = new PathAttributeProvider(attributeMapper);
            var methodAttributeProvider = new MethodAttributeProvider(attributeMapper);
            var paramAttributeProvider = new ParamAttributeProvider(attributeMapper);
            
            var clientMethodParamBuilder = new MethodParamBuilder(paramAttributeProvider);
            var clientMethodBuilder = new MethodBuilder(methodAttributeProvider, pathAttributeProvider, clientMethodParamBuilder);


            var requestBuilder = new RequestBuilder(
                host: new Uri("http://localhost:5000"),
                new RouteTemplateProvider(),
                new RouteProvider(),
                new HttpMethodProvider(),
                new ObjectToKeyValueConverter());

            var resilienceHttpClientProvider = new ResilienceHttpClientProvider(
                new StubHttpClientProvider(),
                new StubResiliencePolicyProvider());

            var interceptor = new ClientInterceptor<TInterface>(
                resilienceHttpClientProvider,
                clientInvocationProvider,
                clientMethodBuilder,
                requestBuilder,
                controllerType: typeof(TController));

            proxyGenerator
                .CreateInterfaceProxyWithoutTarget<TInterface>(interceptor.ToInterceptor())
                .EnsureValidity();
        }
    }
}
