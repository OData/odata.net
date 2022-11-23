//---------------------------------------------------------------------
// <copyright file="JsonSerializerPayloadWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection payload using <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonSerializerPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        /// <inheritdoc/>
        public async Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues)
        {
            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");

            var response = new ResponseWrapper
            {
                Context = $"{serviceRoot}$metadata#Customers",
                Value = payload
            };

            // we ignore the includeRawValues argument for simplicity
            await JsonSerializer.SerializeAsync(stream, response);
        }

        /// <summary>
        /// Wraps the response payload so that it gets the same shape
        /// as an OData resource set response.
        /// </summary>
        private class ResponseWrapper
        {
            [JsonPropertyName("@odata.context")]
            public string Context { get; set; }

            [JsonPropertyName("value")]
            public IEnumerable<Customer> Value { get; set; }
        }
    }
}
