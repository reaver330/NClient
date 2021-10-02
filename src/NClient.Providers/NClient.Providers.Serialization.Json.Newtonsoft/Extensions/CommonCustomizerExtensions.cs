﻿using NClient.Abstractions.Customization;
using NClient.Abstractions.Serialization;
using NClient.Common.Helpers;
using NClient.Providers.Serialization.Json.Newtonsoft;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NClient.Providers.Serialization.Newtonsoft
{
    public static class CommonCustomizerExtensions
    {
        /// <summary>
        /// Sets Newtonsoft.Json based <see cref="ISerializerProvider"/> used to create instance of <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="commonCustomizer"></param>
        public static TCustomizer UsingNewtonsoftJsonSerializer<TCustomizer, TClient, TRequest, TResponse>(
            this INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse> commonCustomizer)
            where TCustomizer : INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse>
            where TClient : class
        {
            Ensure.IsNotNull(commonCustomizer, nameof(commonCustomizer));

            return commonCustomizer.UsingCustomSerializer(new NewtonsoftJsonSerializerProvider());
        }

        /// <summary>
        /// Sets Newtonsoft.Json based <see cref="ISerializerProvider"/> used to create instance of <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="commonCustomizer"></param>
        /// <param name="jsonSerializerSettings">The settings to be used with <see cref="JsonSerializer"/>.</param>
        public static TCustomizer UsingNewtonsoftJsonSerializer<TCustomizer, TClient, TRequest, TResponse>(
            this INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse> commonCustomizer,
            JsonSerializerSettings jsonSerializerSettings)
            where TCustomizer : INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse>
            where TClient : class
        {
            Ensure.IsNotNull(commonCustomizer, nameof(commonCustomizer));
            Ensure.IsNotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));

            return commonCustomizer.UsingCustomSerializer(new NewtonsoftJsonSerializerProvider(jsonSerializerSettings));
        }
    }
}
