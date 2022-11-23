//---------------------------------------------------------------------
// <copyright file="Utf8JsonWriterDirectPayloadWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customers collection payload using <see cref="Utf8JsonWriter"/> directly,
    /// i.e. without creating intermediate <see cref="ODataResource"/> objects.
    /// </summary>
    public class Utf8JsonWriterDirectPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        private readonly Func<Stream, Utf8JsonWriter> writerFactory;

        public Utf8JsonWriterDirectPayloadWriter(Func<Stream, Utf8JsonWriter> writerFactory)
        {
            this.writerFactory = writerFactory;
        }

        /// <inheritdoc/>
        public async Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues)
        {
            Uri serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");

            using Utf8JsonWriter jsonWriter = this.writerFactory(stream);

            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("@odata.context", $"{serviceRoot}$metadata#Customers");
            jsonWriter.WriteStartArray("value");

            int count = 0;

            foreach (Customer customer in payload)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteNumber("Id", customer.Id);
                jsonWriter.WriteString("Name", customer.Name);
                jsonWriter.WriteStartArray("Emails");

                foreach (var email in customer.Emails)
                {
                    jsonWriter.WriteStringValue(email);
                }

                jsonWriter.WriteEndArray();


                // -- HomeAddress
                // start write homeAddress
                jsonWriter.WriteStartObject("HomeAddress");
                jsonWriter.WriteString("City", customer.HomeAddress.City);

                if (includeRawValues)
                {
#if NET6_0
                    jsonWriter.WritePropertyName("Misc");
                    jsonWriter.WriteRawValue($"\"{customer.HomeAddress.Misc}\"");
#else
                    jsonWriter.WriteString("Misc", customer.HomeAddress.Misc as string);
#endif
                }

                jsonWriter.WriteString("Street", customer.HomeAddress.Street);

                // end write homeAddress
                jsonWriter.WriteEndObject();
                // -- End HomeAddress

                // -- Addresses
                jsonWriter.WriteStartArray("Addresses");

                // start addressesResourceSet
                foreach (var address in customer.Addresses)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WriteString("City", address.City);

                    if (includeRawValues)
                    {
#if NET6_0
                        jsonWriter.WritePropertyName("Misc");
                        jsonWriter.WriteRawValue($"\"{address.Misc}\"");
#else
                    jsonWriter.WriteString("Misc", address.Misc as string);
#endif
                    }

                    jsonWriter.WriteString("Street", address.Street);
                    jsonWriter.WriteEndObject();
                }

                // end addressesResourceSet
                jsonWriter.WriteEndArray();
                // -- End Addresses

                // end write resource
                jsonWriter.WriteEndObject();

                // flush the inner writer periodically to prevent expanding the internal buffer indefinitely
                // JSON writer does not commit data to output until it's flushed
                // each customer accounts for about 220 bytes, after 66 iterations we have about 14k pending
                // bytes in the buffer before flushing. I was trying to achieve similar behavior to JsonSerializer (0.9 * 16k)
                if ((++count) % 66 == 0)
                {
                    await jsonWriter.FlushAsync();
                }
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
            await jsonWriter.FlushAsync();
        }
    }
}