using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection OData JSON format using an <see cref="IJsonWriterAsync"/>
    /// directly.
    /// </summary>
    public class ODataJsonWriterAsyncPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        private readonly Func<Stream, IJsonWriterAsync> jsonWriterFactory;

        public ODataJsonWriterAsyncPayloadWriter(Func<Stream, IJsonWriterAsync> jsonWriterFactory)
        {
            this.jsonWriterFactory = jsonWriterFactory;
        }

        public async Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            var writer = new SimpleAsyncJsonODataWriter(this.jsonWriterFactory(stream), serviceRoot, "Customers");


            var resourceSet = new ODataResourceSet();
            await writer.WriteStartAsync(resourceSet);

            foreach (var customer in payload)
            {
                // Serialization is modelled after the ODataResourceSerializer in AspNetCoreOData
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

                await writer.WriteStartAsync(resource);

                // skip WriterStreamPropertiesAsync
                // WriteComplexPropertiesAsync
                // -- HomeAddress
                var homeAddressInfo = new ODataNestedResourceInfo
                {
                    Name = "HomeAddress",
                    IsCollection = false
                };
                // start write homeAddress
                await writer.WriteStartAsync(homeAddressInfo);

                var homeAddressResource = new ODataResource
                {
                    Properties = new[]
                    {
                        new ODataProperty { Name = "City", Value = customer.HomeAddress.City },
                        new ODataProperty { Name = "Street", Value = customer.HomeAddress.Street }
                    }
                };

                await writer.WriteStartAsync(homeAddressResource);
                await writer.WriteEndAsync();

                // end write homeAddress
                await writer.WriteEndAsync();
                // -- End HomeAddress

                // -- Addresses
                var addressesInfo = new ODataNestedResourceInfo
                {
                    Name = "Addresses",
                    IsCollection = true
                };
                // start addressesInfo
                await writer.WriteStartAsync(addressesInfo);


                var addressesResourceSet = new ODataResourceSet();
                // start addressesResourceSet
                await writer.WriteStartAsync(addressesResourceSet);

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

                    await writer.WriteStartAsync(addressResource);
                    await writer.WriteEndAsync();
                }

                // end addressesResourceSet
                await writer.WriteEndAsync();


                // end addressesInfo
                await writer.WriteEndAsync();

                // -- End Addresses

                // end write resource
                await writer.WriteEndAsync();
            }

            await writer.WriteEndAsync();
            await writer.FlushAsync();
        }
    }
}
