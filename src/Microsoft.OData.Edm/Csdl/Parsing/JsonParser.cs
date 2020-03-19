//---------------------------------------------------------------------
// <copyright file="JsonValueParseExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Json;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides for the loading and conversion of one or more CSDL XML readers into Entity Data Model.
    /// </summary>
    internal static class JsonValueParseExtensions
    {
        internal static T ValidateRequiredJsonValue<T>(this IJsonValue jsonValue, JsonPath jsonPath)
            where T : class, IJsonValue
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            T expectedValue = jsonValue as T;
            if (expectedValue != null)
            {
                return expectedValue;
            }

            // Create a default instance to get the expected value type.
            T instance = (T)Activator.CreateInstance(typeof(T));
            throw new CsdlParseException(
                    Strings.CsdlJsonParser_UnexpectedJsonValueKind(jsonValue.ValueKind, jsonPath, instance.ValueKind));
        }
    }
}
