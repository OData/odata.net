using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customers collection payload using <see cref="Utf8JsonWriter"/> directly.
    /// </summary>
    public class Utf8JsonWriterBasicServerWriter : IServerWriter<IEnumerable<Customer>>
    {
        Func<Stream, Utf8JsonWriter> _writerFactory;
        bool _simulateTypedResourceGeneration;

        public Utf8JsonWriterBasicServerWriter(Func<Stream, Utf8JsonWriter> writerFactory, bool simulateResourceGeneration = false)
        {
            _writerFactory = writerFactory;
            _simulateTypedResourceGeneration = simulateResourceGeneration;
        }

        public async Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var sw = new Stopwatch();
            sw.Start();
            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");

            var jsonWriter = _writerFactory(stream);

            var resourceSet = new ODataResourceSet();
            //Console.WriteLine("Start writing resource set");
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("@odata.context", $"{serviceRoot}$metadata#Customers");
            jsonWriter.WriteStartArray("value");

            int count = 0;
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