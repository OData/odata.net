using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes a Customers collection payload using <see cref="Utf8JsonODataWriter"/>.
    /// </summary>
    public class Utf8JsonWriterServerWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        Func<Stream, Utf8JsonWriter> _writerFactory;

        public Utf8JsonWriterServerWriter(Func<Stream, Utf8JsonWriter> writerFactory)
        {
            _writerFactory = writerFactory;
        }

        public async Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var sw = new Stopwatch();
            sw.Start();

            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            var jsonWriter = _writerFactory(stream);
            var writer = new Utf8JsonODataWriter(jsonWriter, serviceRoot, "Customers");

            var resourceSet = new ODataResourceSet();
            //Console.WriteLine("Start writing resource set");
            writer.WriteStart(resourceSet);

            //Console.WriteLine("About to write resources {0}", payload.Count());
            int count = 0;
            foreach (var customer in payload)
            {
                // await resourceSerializer.WriteObjectInlineAsync(item, elementType, writer, writeContext);
                // create resource with only primitive types
                var resource = new ODataResource
                {
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "Id",
                            Value = customer.Id
                        },
                        new ODataProperty { Name = "Name", Value = customer.Name },
                        new ODataProperty
                        {
                            Name = "Emails",
                            Value = new ODataCollectionValue
                            {
                                Items = customer.Emails,
                                TypeName = "Collection(Edm.String)"
                            }
                        }
                    }
                };

                writer.WriteStart(resource);

                // skip WriterStreamPropertiesAsync
                // WriteComplexPropertiesAsync
                // -- HomeAddress
                var homeAddressInfo = new ODataNestedResourceInfo
                {
                    Name = "HomeAddress",
                    IsCollection = false
                };
                // start write homeAddress
                writer.WriteStart(homeAddressInfo);

                var homeAddressResource = new ODataResource
                {
                    Properties = new[]
                    {
                        new ODataProperty { Name = "City", Value = customer.HomeAddress.City },
                        new ODataProperty { Name = "Street", Value = customer.HomeAddress.Street }
                    }
                };

                writer.WriteStart(homeAddressResource);
                writer.WriteEnd();

                // end write homeAddress
                writer.WriteEnd();
                // -- End HomeAddress

                // -- Addresses
                var addressesInfo = new ODataNestedResourceInfo
                {
                    Name = "Addresses",
                    IsCollection = true
                };
                // start addressesInfo
                writer.WriteStart(addressesInfo);


                var addressesResourceSet = new ODataResourceSet();
                // start addressesResourceSet
                writer.WriteStart(addressesResourceSet);

                foreach (var address in customer.Addresses)
                {
                    var addressResource = new ODataResource
                    {
                        Properties = new[]
                        {
                            new ODataProperty { Name = "City", Value = address.City },
                            new ODataProperty { Name = "Street", Value = address.Street }
                        }
                    };

                    writer.WriteStart(addressResource);
                    writer.WriteEnd();
                }

                // end addressesResourceSet
                writer.WriteEnd();


                // end addressesInfo
                writer.WriteEnd();

                // -- End Addresses

                // end write resource
                writer.WriteEnd();
                //Console.WriteLine("Finish writing resource {0}", customer.Id);
                //Console.WriteLine("Finised customer {0}", customer.Id);

                // flush the inner writer periodically to prevent expanding the internal buffer indefinitely
                // JSON writer does not commit data to output until it's flushed
                // each customer accounts for about 220 bytes, after 66 iterations we have about 14k pending
                // bytes in the buffer before flushing. I was trying to achieve similar behavior to JsonSerializer (0.9 * 16k)
                if (count % 66 == 0)
                {
                    await jsonWriter.FlushAsync();
                }

                count++;
            }

            writer.WriteEnd();
            await jsonWriter.FlushAsync();
        }
    }
}
