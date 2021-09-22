using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SerializationBaselineTests
{
    public class ODataSyncExperimentWriter : IExperimentWriter
    {
        IEdmModel model;
        bool useArrayPool;

        public ODataSyncExperimentWriter(IEdmModel model, bool useArrayPool = false)
        {
            this.model = model;
            this.useArrayPool = useArrayPool;
        }

        public Task WriteCustomers(IEnumerable<Customer> payload, Stream stream)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri
            {
                ServiceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/")
            };

            if (useArrayPool)
            {
                settings.ArrayPool = CharArrayPool.Shared;
            }

            InMemoryMessage message = new InMemoryMessage { Stream = stream };

            var messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model);
            var entitySet = model.EntityContainer.FindEntitySet("Customers");
            var writer = messageWriter.CreateODataResourceSetWriter(entitySet);

            var resourceSet = new ODataResourceSet();
            //Console.WriteLine("Start writing resource set");
            writer.WriteStart(resourceSet);
            //Console.WriteLine("About to write resources {0}", payload.Count());

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

                //Console.WriteLine("Start writing resource {0}", customer.Id);
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

            }

            writer.WriteEnd();

            return Task.CompletedTask;
        }
    }
}
