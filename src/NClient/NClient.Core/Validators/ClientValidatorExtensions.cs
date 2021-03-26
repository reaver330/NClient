﻿using System;
using System.Linq;
using System.Reflection;
using NClient.Abstractions.Clients;

namespace NClient.Core.Validators
{
    internal static class ClientValidationExtensions
    {
        public static void EnsureValidity<T>(this T client) where T : class, INClient
        {
            foreach (var methodInfo in typeof(T).GetMethods())
            {
                var parameters = methodInfo.GetParameters().Select(GetDefaultParameter).ToArray();
                methodInfo.Invoke(client, parameters);
            }
        }

        public static object? GetDefaultParameter(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsValueType
                ? Activator.CreateInstance(parameter.ParameterType)
                : null;
        }
    }
}
