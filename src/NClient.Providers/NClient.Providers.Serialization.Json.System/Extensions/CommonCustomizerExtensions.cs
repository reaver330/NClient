﻿using System.Text.Json;
using NClient.Abstractions.Customization;
using NClient.Abstractions.Serialization;
using NClient.Common.Helpers;
using NClient.Providers.Serialization.Json.System;

// ReSharper disable once CheckNamespace
namespace NClient.Providers.Serialization.System
{
    public static class CommonCustomizerExtensions
    {
        /// <summary>
        /// Sets System.Text.Json based <see cref="ISerializerProvider"/> used to create instance of <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="commonCustomizer"></param>
        public static TCustomizer UsingSystemJsonSerializer<TCustomizer, TClient, TRequest, TResponse>(
            this INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse> commonCustomizer)
            where TCustomizer : INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse>
            where TClient : class
        {
            Ensure.IsNotNull(commonCustomizer, nameof(commonCustomizer));

            return commonCustomizer.UsingCustomSerializer(new SystemJsonSerializerProvider());
        }

        /// <summary>
        /// Sets System.Text.Json based <see cref="ISerializerProvider"/> used to create instance of <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="commonCustomizer"></param>
        /// <param name="jsonSerializerOptions">The options to be used with <see cref="JsonSerializer"/>.</param>
        public static TCustomizer UsingSystemJsonSerializer<TCustomizer, TClient, TRequest, TResponse>(
            this INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse> commonCustomizer,
            JsonSerializerOptions jsonSerializerOptions)
            where TCustomizer : INClientCommonCustomizer<TCustomizer, TClient, TRequest, TResponse>
            where TClient : class
        {
            Ensure.IsNotNull(commonCustomizer, nameof(commonCustomizer));
            Ensure.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));

            return commonCustomizer.UsingCustomSerializer(new SystemJsonSerializerProvider(jsonSerializerOptions));
        }
    }
}
