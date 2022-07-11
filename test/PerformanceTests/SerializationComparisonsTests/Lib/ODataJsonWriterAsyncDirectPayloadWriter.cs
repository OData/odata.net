using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData;
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
        private bool _simulateTypedResourceGeneration = true;

        public ODataJsonWriterAsyncDirectPayloadWriter(Func<Stream, IJsonWriterAsync> jsonWriterFactory, bool simulateTypedResourceGeneration = true)
        {
            this.jsonWriterFactory = jsonWriterFactory;
            _simulateTypedResourceGeneration = simulateTypedResourceGeneration;
        }

        public async Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var sw = new Stopwatch();
            sw.Start();

            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            var jsonWriter = jsonWriterFactory(stream);


            var resourceSet = new ODataResourceSet();
            //Console.WriteLine("Start writing resource set");
            await jsonWriter.StartObjectScopeAsync();
            await jsonWriter.WriteNameAsync("@odata.context");
            await jsonWriter.WriteValueAsync($"{serviceRoot}$metadata#Customers");
            await jsonWriter.WriteNameAsync("value");
            await jsonWriter.StartArrayScopeAsync();

            //Console.WriteLine("About to write resources {0}", payload.Count());
            foreach (var _customer in payload)
            {
                Customer customer = _customer;
                if (_simulateTypedResourceGeneration)
                {
                    customer = new Customer
                    {
                        Id = _customer.Id,
                        Name = _customer.Name,
                        Emails = new List<string>(_customer.Emails),
                        HomeAddress = new Address
                        {
                            City = _customer.HomeAddress.City,
                            Street = _customer.HomeAddress.Street
                        },
                        Addresses = _customer.Addresses.Select(a => new Address { City = a.City, Street = a.Street }).ToList()
                    };
                }

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
