using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection OData JSON format using <see cref="ODataMessageWriter"/> async version.
    /// </summary>
    public class ODataAsyncServerWriter : IServerWriter<IEnumerable<Customer>>
    {
        IEdmModel _model;
        bool _useArrayPool = false;
        bool _enableValidation;
        Func<Stream, IODataResponseMessage> _messageFactory;

        public ODataAsyncServerWriter(IEdmModel model, Func<Stream, IODataResponseMessage> messageFactory, bool enableValidation = true, bool useArrayPool = false)
        {
            _model = model;
            _useArrayPool = useArrayPool;
            _enableValidation = enableValidation;
            _messageFactory = messageFactory;
        }
        public async Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var sw = new Stopwatch();
            sw.Start();
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri
            {
                ServiceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/")
            };

            if (!_enableValidation)
            {
                settings.Validations = ValidationKinds.None;
                settings.EnableCharactersCheck = false;
                settings.AlwaysAddTypeAnnotationsForDerivedTypes = false;
            }
            
            var model = _model;
            IODataResponseMessage message = _messageFactory(stream);

            var messageWriter = new ODataMessageWriter(message, settings, model);
            var entitySet = model.EntityContainer.FindEntitySet("Customers");
            var writer = await messageWriter.CreateODataResourceSetWriterAsync(entitySet);

            var resourceSet = new ODataResourceSet();
            //Console.WriteLine("Start writing resource set");
            await writer.WriteStartAsync(resourceSet);
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
                //Console.WriteLine("Finish writing resource {0}", customer.Id);
                //Console.WriteLine("Finised customer {0}", customer.Id);
            }
            await writer.WriteEndAsync();
            await writer.FlushAsync();
        }
    }
}
