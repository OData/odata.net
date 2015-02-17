//---------------------------------------------------------------------
// <copyright file="TypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TypeDefinitionTests
{
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.PrimitiveValueConverters;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class TypeDefinitionTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private const string NameSpacePrefix = "microsoft.odata.sampleService.models.typedefinition.";

        public TypeDefinitionTests()
            : base(ServiceDescriptors.TypeDefinitionServiceDescriptor)
        {

        }

        [TestMethod]
        public void QueryEntryWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entry = this.QueryEntry("People(1)", mimeType);
                
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.IsNotNull(entry);
                    Assert.AreEqual("Bob", entry.Properties.Single(p => p.Name == "FirstName").Value);
                }
            }
        }

        [TestMethod]
        public void QueryTopLevelPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var property = this.QueryProperty("People(1)/LastName", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Cat", property.Value);
                }
            }
        }

        [TestMethod]
        public void QueryComplexPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var comboProperty = this.QueryProperty("People(1)/Address", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataComplexValue complexValue = comboProperty.Value as ODataComplexValue;
                    Assert.AreEqual("Zixing Road", complexValue.Properties.Single(p => p.Name == "Road").Value);
                    Assert.AreEqual("Shanghai", complexValue.Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [TestMethod]
        public void QueryCollectionPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var property = this.QueryProperty("People(1)/Descriptions", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var collectionValue = property.Value as ODataCollectionValue;
                    var items = collectionValue.Items.OfType<object>().ToArray();

                    Assert.AreEqual(2, items.Length);
                    Assert.AreEqual("Tall", items[1]);
                }
            }
        }

        [TestMethod]
        public void QueryPropertyValueWithTypeDefinition()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)/LastName/$value", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var lastNameValue = messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
                Assert.AreEqual("Cat", lastNameValue);
            }
        }

        [TestMethod]
        public void QueryAndFilterOnPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("People?$filter=FirstName ne 'Bob'", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(4, entries.Count);
                    foreach (var entry in entries)
                    {
                        Assert.AreNotEqual("Bob", entry.Properties.Single(p => p.Name == "FirstName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryAndOrderbyPropertyWithTypeDefinition()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("People?$orderby=FirstName desc", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(5, entries.Count);
                    foreach (var entry in entries)
                    {
                        Assert.IsNotNull(entry.Properties.Single(p => p.Name == "FirstName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void InvokeFunctionWithDefinedTypeParameterAndReturnType()
        {
            foreach (var mimeType in mimeTypes)
            {
                var fullName = this.QueryProperty("People(1)/microsoft.odata.sampleService.models.typedefinition.GetFullName(nickname='Moon')", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Bob (Moon) Cat", fullName.Value);
                }
            }
        }

        [TestMethod]
        public void CreateEntityWithDefinedTypeProperties()
        {
            var entry = new ODataEntry() { TypeName = NameSpacePrefix + "Person" };
            entry.Properties = new[]
                {
                    new ODataProperty { Name = "PersonId", Value = 101 },
                    new ODataProperty { Name = "FirstName", Value = "Peter" },
                    new ODataProperty { Name = "LastName", Value = "Zhang" },
                    new ODataProperty
                    {
                        Name = "Address",
                        Value = new ODataComplexValue
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
                    },
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

            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

            var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = Model.EntityContainer.FindEntitySet("People");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                var odataWriter = messageWriter.CreateODataEntryWriter(peopleSet, personType);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.AreEqual(201, responseMessage.StatusCode);
            entry = this.QueryEntry("People(101)", MimeTypes.ApplicationJson);
            Assert.AreEqual(101, entry.Properties.Single(p => p.Name == "PersonId").Value);
            Assert.AreEqual("Zhang", entry.Properties.Single(p => p.Name == "LastName").Value);
        }

        [TestMethod]
        public void QueryEntryWithUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(5, entries.Count);

                    var productid = entries[0].Properties.Single(p => p.Name == "ProductId").Value;
                    var quantity = entries[0].Properties.Single(p => p.Name == "Quantity").Value;
                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.AreEqual((UInt16)11, productid);
                    Assert.AreEqual(100u, quantity);
                    Assert.AreEqual(3600ul, lifetime);

                    productid = entries[1].Properties.Single(p => p.Name == "ProductId").Value;
                    quantity = entries[1].Properties.Single(p => p.Name == "Quantity").Value;
                    lifetime = entries[1].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.AreEqual((UInt16)12, productid);
                    Assert.AreEqual(UInt32.MaxValue, quantity);
                    Assert.AreEqual(UInt64.MaxValue, lifetime);

                    productid = entries[2].Properties.Single(p => p.Name == "ProductId").Value;
                    quantity = entries[2].Properties.Single(p => p.Name == "Quantity").Value;
                    lifetime = entries[2].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.AreEqual((UInt16)13, productid);
                    Assert.AreEqual(UInt32.MinValue, quantity);
                    Assert.AreEqual(UInt64.MinValue, lifetime);

                }
            }
        }

        [TestMethod]
        public void QueryUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var idProperty = this.QueryProperty("Products(11)/ProductId", mimeType);
                var quantityProperty = this.QueryProperty("Products(11)/Quantity", mimeType);
                var lifeTimeProperty = this.QueryProperty("Products(11)/LifeTimeInSeconds", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual((UInt16)11, idProperty.Value);
                    Assert.AreEqual(100u, quantityProperty.Value);
                    Assert.AreEqual(3600ul, lifeTimeProperty.Value);   
                }                            
            }
        }

        [TestMethod]
        public void QueryComplexPropertyWithUintMembers()
        {
            foreach (var mimeType in mimeTypes)
            {
                var comboProperty = this.QueryProperty("Products(11)/TheCombo", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataComplexValue complexValue = comboProperty.Value as ODataComplexValue;
                    Assert.AreEqual((UInt16)80, complexValue.Properties.Single(p => p.Name == "Small").Value);
                    Assert.AreEqual((UInt32)196, complexValue.Properties.Single(p => p.Name == "Middle").Value);
                    Assert.AreEqual((UInt64)3, complexValue.Properties.Single(p => p.Name == "Large").Value);
                }
            }
        }

        [TestMethod]
        public void QueryCollectionPropertyOfUIntMembers()
        {
            foreach (var mimeType in mimeTypes)
            {
                var property = this.QueryProperty("Products(11)/LargeNumbers", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var collectionValue = property.Value as ODataCollectionValue;
                    var items = collectionValue.Items.OfType<object>().ToArray();

                    Assert.AreEqual(3, items.Length);
                    Assert.AreEqual((UInt64)36, items[0]);
                }
            }
        }

        [TestMethod]
        public void QueryPropertyValueOfUintMembers()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(11)/Quantity/$value", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);
        }

        [TestMethod]
        public void CreateEntityWithUIntProperties()
        {
            var entry = new ODataEntry() { TypeName = NameSpacePrefix + "Product" };
            entry.Properties = new[]
                {
                    new ODataProperty { Name = "ProductId", Value = (UInt16)101 },
                    new ODataProperty { Name = "Quantity", Value = 19u },
                    new ODataProperty { Name = "LifeTimeInSeconds", Value = 86ul },
                    new ODataProperty { Name = "NullableUInt32", Value = 37u },
                    new ODataProperty
                    {
                        Name = "TheCombo",
                        Value = new ODataComplexValue
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
                    },
                    new ODataProperty 
                    {
                        Name = "LargeNumbers", 
                        Value = new ODataCollectionValue() 
                        {
                            TypeName = "Collection(" + NameSpacePrefix + "UInt64)", 
                            Items = new[] { 32ul, 97ul }
                        }
                    }

                };

            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

            var productType = Model.FindDeclaredType(NameSpacePrefix + "Product") as IEdmEntityType;
            var productsSet = Model.EntityContainer.FindEntitySet("Products");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                var odataWriter = messageWriter.CreateODataEntryWriter(productsSet, productType);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.AreEqual(201, responseMessage.StatusCode);

            var createdEntry = this.QueryEntry("Products(101)", MimeTypes.ApplicationJson);
            Assert.AreEqual((UInt16)101, createdEntry.Properties.Single(p => p.Name == "ProductId").Value);
            Assert.AreEqual(86ul, createdEntry.Properties.Single(p => p.Name == "LifeTimeInSeconds").Value);
        }

        [TestMethod]
        public void InvokeActionWithUintParameterAndReturnType()
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.PayloadBaseUri = ServiceBaseUri;
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
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var property = messageReader.ReadProperty();
                Assert.IsNotNull(property);
                Assert.AreEqual(4600ul, property.Value);
            }
        }

        [TestMethod]
        public void QueryAndFilterByUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products?$filter=Quantity eq 100", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, entries.Count);
                    var quantity = entries[0].Properties.Single(p => p.Name == "Quantity").Value;
                    Assert.AreEqual(100u, quantity);
                }

                entries = this.QueryFeed("Products?$filter=18446744073709551615 eq LifeTimeInSeconds", mimeType); //UInt64.Max

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, entries.Count);
                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.AreEqual(UInt64.MaxValue, lifetime);
                }

                entries = this.QueryFeed("Products?$filter=NullableUInt32 eq null", mimeType); //null

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(2, entries.Count);
                    var nullable = entries[0].Properties.Single(p => p.Name == "NullableUInt32").Value;
                    Assert.AreEqual(null, nullable);
                }
            }
        }

        [TestMethod]
        public void QueryAndOrderByUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products?$orderby=LifeTimeInSeconds desc", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(5, entries.Count);

                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.AreEqual(UInt64.MaxValue, lifetime);
                }
            }
        }

        [TestMethod]
        public void QueryAndSelectUnsignedIntegerProperties()
        {
            foreach (var mimeType in mimeTypes)
            {
                var entries = this.QueryFeed("Products?$select=ProductId, LifeTimeInSeconds", mimeType);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(5, entries.Count);

                    var lifetime = entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds").Value;
                    Assert.AreEqual(3600ul, lifetime);
                }
            }
        }

        #region Helper

        private ODataEntry QueryEntry(string uri, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", mimeType);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.AreEqual(200, queryResponseMessage.StatusCode);

            ODataEntry entry = null;

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataEntryReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            entry = reader.Item as ODataEntry;
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }
            
            return entry;
        }

        private List<ODataEntry> QueryFeed(string uri, string mimeType)
        {
            List<ODataEntry> entries = new List<ODataEntry>();
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataFeedReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            ODataEntry entry = reader.Item as ODataEntry;
                            Assert.IsNotNull(entry);
                            
                            entries.Add(entry);
                        }
                        else if (reader.State == ODataReaderState.FeedEnd)
                        {
                            Assert.IsNotNull(reader.Item as ODataFeed);
                        }
                    }
                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    
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
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    property = messageReader.ReadProperty();
                    Assert.IsNotNull(property);
                }
            }

            return property;
            
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
