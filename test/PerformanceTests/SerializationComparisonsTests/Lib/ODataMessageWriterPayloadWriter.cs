//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterPayloadWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection payload using <see cref="ODataMessageWriter"/>
    /// </summary>
    public class ODataMessageWriterPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        readonly IEdmModel model;
        readonly bool enableValidation;
        readonly Func<Stream, IODataResponseMessage> messageFactory;

        public ODataMessageWriterPayloadWriter(IEdmModel model, Func<Stream, IODataResponseMessage> messageFactory, bool enableValidation = true)
        {
            this.model = model;
            this.messageFactory = messageFactory;
            this.enableValidation = enableValidation;
        }

        /// <inheritdoc/>
        public Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues)
        {
            var settings = new ODataMessageWriterSettings();
            settings.EnableMessageStreamDisposal = false;

            settings.ODataUri = new ODataUri
            {
                ServiceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/"),
                
            };

            if (!this.enableValidation)
            {
                settings.Validations = ValidationKinds.None;
                settings.EnableCharactersCheck = false;
                settings.AlwaysAddTypeAnnotationsForDerivedTypes = false;
            }

            IODataResponseMessage message = messageFactory(stream);

            using var messageWriter = new ODataMessageWriter(message, settings, model);
            var entitySet = model.EntityContainer.FindEntitySet("Customers");
            var writer = messageWriter.CreateODataResourceSetWriter(entitySet);

            var resourceSet = new ODataResourceSet();
            writer.WriteStart(resourceSet);

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
                        },
                        new ODataProperty
                        {
                            Name = "Bio",
                            Value = customer.Bio
                        },
                        new ODataProperty
                        {
                            Name = "Content",
                            Value = customer.Content
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

                ODataResource homeAddressResource;
                if (includeRawValues)
                {
                    homeAddressResource = new ODataResource
                    {
                        Properties = new[]
                        {
                            new ODataProperty { Name = "City", Value = customer.HomeAddress.City },
                            new ODataProperty { Name = "Misc", Value = new ODataUntypedValue() { RawValue = $"\"{customer.HomeAddress.Misc}\"" } },
                            new ODataProperty { Name = "Street", Value = customer.HomeAddress.Street }
                        }
                    };
                }
                else
                {
                    homeAddressResource = new ODataResource
                    {
                        Properties = new[]
                        {
                            new ODataProperty { Name = "City", Value = customer.HomeAddress.City },
                            new ODataProperty { Name = "Street", Value = customer.HomeAddress.Street }
                        }
                    };
                }

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

                if (includeRawValues)
                {
                    foreach (var address in customer.Addresses)
                    {
                        var addressResource = new ODataResource
                        {
                            Properties = new[]
                            {
                            new ODataProperty { Name = "City", Value = address.City },
                            new ODataProperty { Name = "Misc", Value = new ODataUntypedValue() { RawValue = $"\"{address.Misc}\"" } },
                            new ODataProperty { Name = "Street", Value = address.Street }
                        }
                        };

                        writer.WriteStart(addressResource);
                        writer.WriteEnd();
                    }
                }
                else
                {
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
                }

                // end addressesResourceSet
                writer.WriteEnd();


                // end addressesInfo
                writer.WriteEnd();

                // -- End Addresses

                // end write resource
                writer.WriteEnd();
            }

            writer.WriteEnd();
            writer.Flush();
            return Task.CompletedTask;
        }
    }
}
