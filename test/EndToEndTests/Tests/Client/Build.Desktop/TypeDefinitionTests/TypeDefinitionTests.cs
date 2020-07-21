//---------------------------------------------------------------------
// <copyright file="TypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TypeDefinitionTests
{
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class TypeDefinitionTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private const string NameSpacePrefix = "microsoft.odata.sampleService.models.typedefinition.";

        public TypeDefinitionTests()
            : base(ServiceDescriptors.TypeDefinitionServiceDescriptor)
        {

        }

        [Fact]
        public void QueryEntryWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entry = this.QueryEntry("People(1)", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.NotNull(entry);
                    Assert.Equal("Bob", entry.Properties.Single(p => p.Name == "FirstName").Value);
                }
            }
        }

        [Fact]
        public void QueryTopLevelPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var property = this.QueryProperty("People(1)/LastName", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Cat", property.Value);
                }
            }
        }

        [Fact]
        public void QueryComplexPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var address = this.QueryComplexProperty("People(1)/Address", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Zixing Road", address.Properties.Single(p => p.Name == "Road").Value);
                    Assert.Equal("Shanghai", address.Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [Fact]
        public void QueryCollectionPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var property = this.QueryProperty("People(1)/Descriptions", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var collectionValue = property.Value as ODataCollectionValue;
                    var items = collectionValue.Items.OfType<object>().ToArray();

                    Assert.Equal(2, items.Length);
                    Assert.Equal("Tall", items[1]);
                }
            }
        }

        [Fact]
        public void QueryPropertyValueWithTypeDefinition()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)/LastName/$value", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var lastNameValue = messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
                Assert.Equal("Cat", lastNameValue);
            }
        }

        [Fact]
        public void QueryAndFilterOnPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("People?$filter=FirstName ne 'Bob'", mimeType, "Person");

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(4, entries.Count);
                    foreach (var entry in entries)
                    {
                        Assert.NotEqual("Bob", entry.Properties.Single(p => p.Name == "FirstName").Value);
                    }
                }
            }
        }

        [Fact]
        public void QueryAndOrderbyPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("People?$orderby=FirstName desc", mimeType, "Person");

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(5, entries.Count);
                    foreach (var entry in entries)
                    {
                        Assert.NotNull(entry.Properties.Single(p => p.Name == "FirstName").Value);
                    }
                }
            }
        }

        [Fact]
        public void InvokeFunctionWithDefinedTypeParameterAndReturnType()
        {
            foreach (var mimeType in mimeTypes)
            {
                var fullName = this.QueryProperty("People(1)/microsoft.odata.sampleService.models.typedefinition.GetFullName(nickname='Moon')", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Bob (Moon) Cat", fullName.Value);
                }
            }
        }

        [Fact]
        public void CreateEntityWithDefinedTypeProperties()
        {
            var entry = new ODataResource() { TypeName = NameSpacePrefix + "Person" };
            entry.Properties = new[]
                {
                    new ODataProperty { Name = "PersonId", Value = 101 },
                    new ODataProperty { Name = "FirstName", Value = "Peter" },
                    new ODataProperty { Name = "LastName", Value = "Zhang" },
                    new ODataProperty
                    {
                        Name = "Descriptions",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = "Collection(Edm.String)",
                            Items = new[] { "Description1", "Description2" }
                        }
                    }
                };

            var entryWrapper = new ODataResourceWrapper()
            {
                Resource = entry,
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "Address",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + "Address",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "Road",
                                        Value = "Road one"
                                    },
                                    new ODataProperty
                                    {
                                        Name = "City",
                                        Value = "Shanghai"
                                    }
                                }
                            }
                        }
                    },
                }
            };
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = Model.EntityContainer.FindEntitySet("People");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(peopleSet, personType);
                ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.Equal(201, responseMessage.StatusCode);
            entry = this.QueryEntry("People(101)", MimeTypes.ApplicationJson);
            Assert.Equal(101, entry.Properties.Single(p => p.Name == "PersonId").Value);
            Assert.Equal("Zhang", entry.Properties.Single(p => p.Name == "LastName").Value);
        }

        [Fact]
        public void QueryEntryWithUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products", mimeType, "Product");

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(5, entries.Count);

                    var productid = entries[0].Properties.Single(p => p.Name == "ProductId").Value;
                    var quantity = entries[0].Properties.Single(p => p.Name == "Quantity").Value;
                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.Equal((UInt16)11, productid);
                    Assert.Equal(100u, quantity);
                    Assert.Equal(3600ul, lifetime);

                    productid = entries[1].Properties.Single(p => p.Name == "ProductId").Value;
                    quantity = entries[1].Properties.Single(p => p.Name == "Quantity").Value;
                    lifetime = entries[1].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.Equal((UInt16)12, productid);
                    Assert.Equal(UInt32.MaxValue, quantity);
                    Assert.Equal(UInt64.MaxValue, lifetime);

                    productid = entries[2].Properties.Single(p => p.Name == "ProductId").Value;
                    quantity = entries[2].Properties.Single(p => p.Name == "Quantity").Value;
                    lifetime = entries[2].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.Equal((UInt16)13, productid);
                    Assert.Equal(UInt32.MinValue, quantity);
                    Assert.Equal(UInt64.MinValue, lifetime);

                }
            }
        }

        [Fact]
        public void QueryUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var idProperty = this.QueryProperty("Products(11)/ProductId", mimeType);
                var quantityProperty = this.QueryProperty("Products(11)/Quantity", mimeType);
                var lifeTimeProperty = this.QueryProperty("Products(11)/LifeTimeInSeconds", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal((UInt16)11, idProperty.Value);
                    Assert.Equal(100u, quantityProperty.Value);
                    Assert.Equal(3600ul, lifeTimeProperty.Value);
                }
            }
        }

        [Fact]
        public void QueryComplexPropertyWithUintMembers()
        {
            foreach (var mimeType in mimeTypes)
            {
                var combo = this.QueryComplexProperty("Products(11)/TheCombo", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal((UInt16)80, combo.Properties.Single(p => p.Name == "Small").Value);
                    Assert.Equal((UInt32)196, combo.Properties.Single(p => p.Name == "Middle").Value);
                    Assert.Equal((UInt64)3, combo.Properties.Single(p => p.Name == "Large").Value);
                }
            }
        }

        [Fact]
        public void QueryCollectionPropertyOfUIntMembers()
        {
            foreach (var mimeType in mimeTypes)
            {
                var property = this.QueryProperty("Products(11)/LargeNumbers", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var collectionValue = property.Value as ODataCollectionValue;
                    var items = collectionValue.Items.OfType<object>().ToArray();

                    Assert.Equal(3, items.Length);
                    Assert.Equal((UInt64)36, items[0]);
                }
            }
        }

        [Fact]
        public void QueryPropertyValueOfUintMembers()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(11)/Quantity/$value", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);
        }

        [Fact]
        public void CreateEntityWithUIntProperties()
        {
            var entry = new ODataResource() { TypeName = NameSpacePrefix + "Product" };
            entry.Properties = new[]
                {
                    new ODataProperty { Name = "ProductId", Value = (UInt16)101 },
                    new ODataProperty { Name = "Quantity", Value = 19u },
                    new ODataProperty { Name = "LifeTimeInSeconds", Value = 86ul },
                    new ODataProperty { Name = "NullableUInt32", Value = 37u },
                    new ODataProperty
                    {
                        Name = "LargeNumbers",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = "Collection(" + NameSpacePrefix + "UInt64)",
                            Items = new object[] { 32ul, 97ul }
                        }
                    }

                };

            var entryWrapper = new ODataResourceWrapper()
            {
                Resource = entry,
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "TheCombo",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + "NumberCombo",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "Small",
                                        Value = (UInt16)10
                                    },
                                    new ODataProperty
                                    {
                                        Name = "Middle",
                                        Value = 33u
                                    },
                                    new ODataProperty
                                    {
                                        Name = "Large",
                                        Value = 101ul
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var productType = Model.FindDeclaredType(NameSpacePrefix + "Product") as IEdmEntityType;
            var productsSet = Model.EntityContainer.FindEntitySet("Products");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(productsSet, productType);
                ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.Equal(201, responseMessage.StatusCode);

            var createdEntry = this.QueryEntry("Products(101)", MimeTypes.ApplicationJson);
            Assert.Equal((UInt16)101, createdEntry.Properties.Single(p => p.Name == "ProductId").Value);
            Assert.Equal(86ul, createdEntry.Properties.Single(p => p.Name == "LifeTimeInSeconds").Value);
        }

        [Fact]
        public void InvokeActionWithUintParameterAndReturnType()
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.BaseUri = ServiceBaseUri;
            var readerSettings = new ODataMessageReaderSettings();
            readerSettings.BaseUri = ServiceBaseUri;

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products(11)/microsoft.odata.sampleService.models.typedefinition.ExtendLifeTime"));

            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "POST";

            using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
            {
                var odataWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
                odataWriter.WriteStart();
                odataWriter.WriteValue("seconds", 1000u);
                odataWriter.WriteEnd();
            }

            // send the http request
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var property = messageReader.ReadProperty();
                Assert.NotNull(property);
                Assert.Equal(4600ul, property.Value);
            }
        }

        [Fact]
        public void QueryAndFilterByUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products?$filter=Quantity eq 100", mimeType, "Product");

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(1, entries.Count);
                    var quantity = entries[0].Properties.Single(p => p.Name == "Quantity").Value;
                    Assert.Equal(100u, quantity);
                }

                entries = this.QueryFeed("Products?$filter=18446744073709551615 eq LifeTimeInSeconds", mimeType, "Product"); //UInt64.Max

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(1, entries.Count);
                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.Equal(UInt64.MaxValue, lifetime);
                }

                entries = this.QueryFeed("Products?$filter=NullableUInt32 eq null", mimeType, "Product"); //null

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(2, entries.Count);
                    var nullable = entries[0].Properties.Single(p => p.Name == "NullableUInt32").Value;
                    Assert.Equal(null, nullable);
                }
            }
        }

        [Fact]
        public void QueryAndOrderByUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products?$orderby=LifeTimeInSeconds desc", mimeType, "Product");

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(5, entries.Count);

                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.Equal(UInt64.MaxValue, lifetime);
                }
            }
        }

        [Fact]
        public void QueryAndSelectUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products?$select=ProductId, LifeTimeInSeconds", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(5, entries.Count);

                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.Equal(3600ul, lifetime);
                }
            }
        }

        #region Helper

        private ODataResource QueryEntry(string uri, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", mimeType);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(200, queryResponseMessage.StatusCode);

            ODataResource entry = null;

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            entry = reader.Item as ODataResource;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }

            return entry;
        }

        private List<ODataResource> QueryFeed(string uri, string mimeType, params string[] entryTypeNames)
        {
            List<ODataResource> entries = new List<ODataResource>();
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceSetReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            ODataResource entry = reader.Item as ODataResource;
                            if (entry != null && (entryTypeNames.Length == 0 || entryTypeNames.Any(e => entry.TypeName.Contains(e))))
                            {
                                entries.Add(entry);
                            }
                        }
                        else if (reader.State == ODataReaderState.ResourceSetEnd)
                        {
                            Assert.NotNull(reader.Item as ODataResourceSet);
                        }
                    }
                    Assert.Equal(ODataReaderState.Completed, reader.State);

                }
            }

            return entries;

        }

        private ODataProperty QueryProperty(string uri, string mimeType)
        {
            ODataProperty property = null;
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    property = messageReader.ReadProperty();
                    Assert.NotNull(property);
                }
            }

            return property;
        }

        private ODataResource QueryComplexProperty(string uri, string mimeType)
        {
            ODataResource complex = null;
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var odataReader = messageReader.CreateODataResourceReader();
                    while (odataReader.Read())
                    {
                        if (odataReader.State == ODataReaderState.ResourceEnd)
                        {
                            complex = odataReader.Item as ODataResource;
                        }
                    }

                    Assert.NotNull(complex);
                }
            }

            return complex;
        }

        #endregion


    }

    public class UInt32ValueConverter : IPrimitiveValueConverter
    {
        private static readonly IPrimitiveValueConverter instance = new UInt32ValueConverter();

        internal static IPrimitiveValueConverter Instance
        {
            get { return instance; }
        }

        public object ConvertToUnderlyingType(object value)
        {
            return Convert.ToString(value);
        }

        public object ConvertFromUnderlyingType(object value)
        {
            return Convert.ToUInt32(value);
        }
    }
}
