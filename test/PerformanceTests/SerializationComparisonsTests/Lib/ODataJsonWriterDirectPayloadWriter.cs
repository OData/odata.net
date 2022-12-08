//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterDirectPayloadWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection OData JSON format using <see cref="IJsonWriter"/> directly,
    /// i.e. without creating intermediate <see cref="ODataResource"/> objects.
    /// </summary>
    public class ODataJsonWriterDirectPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        private readonly Func<Stream, IJsonWriter> jsonWriterFactory;

        public ODataJsonWriterDirectPayloadWriter(Func<Stream, IJsonWriter> jsonWriterFactory)
        {
            this.jsonWriterFactory = jsonWriterFactory;
        }

        /// <inheritdoc/>
        public Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues)
        {
            Uri serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            IJsonWriter jsonWriter = this.jsonWriterFactory(stream);

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("@odata.context");
            jsonWriter.WriteValue($"{serviceRoot}$metadata#Customers");
            jsonWriter.WriteName("value");
            jsonWriter.StartArrayScope();

            foreach (Customer customer in payload)
            {
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("Id");
                jsonWriter.WriteValue(customer.Id);
                jsonWriter.WriteName("Name");
                jsonWriter.WriteValue(customer.Name);
                jsonWriter.WriteName("Emails");
                jsonWriter.StartArrayScope();
                foreach (var email in customer.Emails)
                {
                    jsonWriter.WriteValue(email);
                }
                jsonWriter.EndArrayScope();


                // -- HomeAddress
                // start write homeAddress
                jsonWriter.WriteName("HomeAddress");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("City");
                jsonWriter.WriteValue(customer.HomeAddress.City);
                if (includeRawValues)
                {
                    jsonWriter.WriteName("Misc");
                    jsonWriter.WriteRawValue($"\"{customer.HomeAddress.Misc}\"");
                }

                jsonWriter.WriteName("Street");
                jsonWriter.WriteValue(customer.HomeAddress.Street);

                // end write homeAddress
                jsonWriter.EndObjectScope();
                // -- End HomeAddress

                // -- Addresses
                jsonWriter.WriteName("Addresses");
                jsonWriter.StartArrayScope();

                // start addressesResourceSet
                foreach (var address in customer.Addresses)
                {
                    jsonWriter.StartObjectScope();
                    jsonWriter.WriteName("City");
                    jsonWriter.WriteValue(address.City);
                    if (includeRawValues)
                    {
                        jsonWriter.WriteName("Misc");
                        jsonWriter.WriteRawValue($"\"{address.Misc}\"");
                    }

                    jsonWriter.WriteName("Street");
                    jsonWriter.WriteValue(address.Street);
                    jsonWriter.EndObjectScope();
                }

                // end addressesResourceSet
                jsonWriter.EndArrayScope();
                // -- End Addresses

                // end write resource
                jsonWriter.EndObjectScope();
            }

            jsonWriter.EndArrayScope();
            jsonWriter.EndObjectScope();
            jsonWriter.Flush();

            return Task.CompletedTask;
        }
    }
}
