//---------------------------------------------------------------------
// <copyright file="UriOperationParameterGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Test.Astoria.Util;
using System.Text;
using AstoriaUnitTests.Tests;
using Microsoft.OData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests
{
    /// <summary>
    /// Generates a query string composed of operation parameters and their values in the specified format.
    /// </summary>
    internal class UriOperationParameterGenerator
    {
        internal static string Generate(OperationUriBuilder operationUriBuilder, ODataFormat format, PayloadGeneratorSettings settings)
        {
            var uriStringBuilder = new StringBuilder();
            bool first = true;
            foreach (var parameter in operationUriBuilder.Parameters)
            {
                if (!first)
                {
                    uriStringBuilder.Append(",");
                }

                string literalValue = GetLiteralValue(parameter.Value, format, settings);
                if (parameter.EscapeGeneratedValue)
                {
                    literalValue = Uri.EscapeDataString(literalValue);
                }

                uriStringBuilder.Append(string.Format("{0}={1}", parameter.Name, literalValue));
                first = false;
            }

            return uriStringBuilder.ToString();
        }

        private static string GetLiteralValue(object value, ODataFormat format, PayloadGeneratorSettings settings)
        {
            JsonPayloadGenerator generator;
            if (format == ODataFormat.Json)
            {
                generator = new JsonLightPayloadGenerator(settings);
            }
            else
            {
                Assert.Fail("Format not supported by UriOperationParameterGenerator.");
                return null;
            }

            var payloadBuilder = value as PayloadBuilder;
            if (payloadBuilder != null)
            {
                return generator.GenerateLiteral(payloadBuilder);
            }

            var collectionBuilder = value as CollectionPropertyPayloadBuilder;
            if (collectionBuilder != null)
            {
                return generator.GenerateLiteral(collectionBuilder);
            }

            // Use the key syntax since that will give us single quotes and other formatting that is not used for literals as property values
            return JsonPrimitiveTypesUtil.PrimitiveToKeyString(value, value.GetType());
        }
    }

    internal class OperationUriBuilder
    {
        private readonly List<UriParameter> uriParameters = new List<UriParameter>();

        internal List<UriParameter> Parameters
        {
            get { return this.uriParameters; }
        }

        internal OperationUriBuilder AddParameter(string parameterName, object parameterValue, bool escapeGeneratedValue = true)
        {
            this.uriParameters.Add(new UriParameter(parameterName, parameterValue, escapeGeneratedValue));
            return this;
        }
    }

    internal class UriParameter
    {
        internal string Name { get; private set; }
        internal object Value { get; private set; }
        internal bool EscapeGeneratedValue { get; set; }

        internal UriParameter(string parameterName, object parameterValue, bool escapeValue)
        {
            this.Name = parameterName;
            this.Value = parameterValue;
            this.EscapeGeneratedValue = escapeValue;
        }
    }    
}