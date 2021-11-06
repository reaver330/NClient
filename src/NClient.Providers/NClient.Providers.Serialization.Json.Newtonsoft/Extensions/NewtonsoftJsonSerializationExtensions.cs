﻿using NClient.Common.Helpers;
using NClient.Providers.Serialization.Json.Newtonsoft;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NClient
{
    // TODO: doc
    public static class NewtonsoftJsonSerializationExtensions
    {
        public static INClientOptionalBuilder<TClient, TRequest, TResponse> WithNewtonsoftJsonSerialization<TClient, TRequest, TResponse>(
            this INClientOptionalBuilder<TClient, TRequest, TResponse> clientOptionalBuilder) 
            where TClient : class
        {
            Ensure.IsNotNull(clientOptionalBuilder, nameof(clientOptionalBuilder));
            
            return clientOptionalBuilder.WithCustomSerialization(new NewtonsoftJsonSerializerProvider());
        }

        public static INClientOptionalBuilder<TClient, TRequest, TResponse> WithNewtonsoftJsonSerialization<TClient, TRequest, TResponse>(
            this INClientOptionalBuilder<TClient, TRequest, TResponse> clientOptionalBuilder,
            JsonSerializerSettings jsonSerializerSettings)
            where TClient : class
        {
            Ensure.IsNotNull(clientOptionalBuilder, nameof(clientOptionalBuilder));
            Ensure.IsNotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
            
            return clientOptionalBuilder.WithCustomSerialization(new NewtonsoftJsonSerializerProvider(jsonSerializerSettings));
        }

        public static INClientFactoryOptionalBuilder<TRequest, TResponse> WithNewtonsoftJsonSerialization<TRequest, TResponse>(
            this INClientFactoryOptionalBuilder<TRequest, TResponse> clientOptionalBuilder)
        {
            Ensure.IsNotNull(clientOptionalBuilder, nameof(clientOptionalBuilder));
            
            return clientOptionalBuilder.WithCustomSerialization(new NewtonsoftJsonSerializerProvider());
        }

        public static INClientFactoryOptionalBuilder<TRequest, TResponse> WithNewtonsoftJsonSerialization<TRequest, TResponse>(
            this INClientFactoryOptionalBuilder<TRequest, TResponse> clientOptionalBuilder,
            JsonSerializerSettings jsonSerializerSettings)
        {
            Ensure.IsNotNull(clientOptionalBuilder, nameof(clientOptionalBuilder));
            Ensure.IsNotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
            
            return clientOptionalBuilder.WithCustomSerialization(new NewtonsoftJsonSerializerProvider(jsonSerializerSettings));
        }
    }
}
