using System;
using System.Collections.Generic;
using System.Diagnostics;
//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterAsyncDirectPayloadWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection OData JSON format using <see cref="IJsonWriterAsync"/> directly
    /// using the Async API.
    /// </summary>
    public class ODataJsonWriterAsyncDirectPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        private readonly Func<Stream, IJsonWriterAsync> jsonWriterFactory;

        public ODataJsonWriterAsyncDirectPayloadWriter(Func<Stream, IJsonWriterAsync> jsonWriterFactory)
        {
            this.jsonWriterFactory = jsonWriterFactory;
        }

        /// <inheritdoc/>
        public async Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues)
        {
            Uri serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            IJsonWriterAsync jsonWriter = this.jsonWriterFactory(stream);

            await jsonWriter.StartObjectScopeAsync();
            await jsonWriter.WriteNameAsync("@odata.context");
            await jsonWriter.WriteValueAsync($"{serviceRoot}$metadata#Customers");
            await jsonWriter.WriteNameAsync("value");
            await jsonWriter.StartArrayScopeAsync();

            foreach (Customer customer in payload)
            {
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("Id");
                await jsonWriter.WriteValueAsync(customer.Id);
                await jsonWriter.WriteNameAsync("Name");
                await jsonWriter.WriteValueAsync(customer.Name);
                await jsonWriter.WriteNameAsync("Emails");
                await jsonWriter.StartArrayScopeAsync ();

                foreach (var email in customer.Emails)
                {
                    await jsonWriter.WriteValueAsync(email);
                }

                await jsonWriter.EndArrayScopeAsync();

                // -- HomeAddress
                // start write homeAddress
                await jsonWriter.WriteNameAsync("HomeAddress");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("City");
                await jsonWriter.WriteValueAsync(customer.HomeAddress.City);
                if (includeRawValues)
                {
                    await jsonWriter.WriteNameAsync("Misc");
                    await jsonWriter.WriteRawValueAsync($"\"{customer.HomeAddress.Misc}\"");
                }
                
                await jsonWriter.WriteNameAsync("Street");
                await jsonWriter.WriteValueAsync(customer.HomeAddress.Street);

                // end write homeAddress
                await jsonWriter.EndObjectScopeAsync();
                // -- End HomeAddress

                // -- Addresses
                await jsonWriter.WriteNameAsync("Addresses");
                await jsonWriter.StartArrayScopeAsync();

                // start addressesResourceSet
                foreach (var address in customer.Addresses)
                {
                    await jsonWriter.StartObjectScopeAsync();
                    await jsonWriter.WriteNameAsync("City");
                    await jsonWriter.WriteValueAsync(address.City);
                    if (includeRawValues)
                    {
                        await jsonWriter.WriteNameAsync("Misc");
                        await jsonWriter.WriteRawValueAsync($"\"{address.Misc}\"");
                    }
                    await jsonWriter.WriteNameAsync("Street");
                    await jsonWriter.WriteValueAsync(address.Street);
                    await jsonWriter.EndObjectScopeAsync();
                }

                // end addressesResourceSet
                await jsonWriter.EndArrayScopeAsync();
                // -- End Addresses

                // end write resource
                await jsonWriter.EndObjectScopeAsync();
            }

            await jsonWriter.EndArrayScopeAsync();
            await jsonWriter.EndObjectScopeAsync();
            await jsonWriter.FlushAsync();
        }
    }
}
