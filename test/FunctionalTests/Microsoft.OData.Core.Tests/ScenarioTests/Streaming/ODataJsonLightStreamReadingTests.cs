﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightStreamReadingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightStreamReadingTests
    {
        private EdmModel model;
        private IEdmEntitySet customersEntitySet;
        private string binaryString = "binaryString";
        private string stringValue = "My String Value";
        private string binaryValue = "DGJpbmFyeVN0cmluZw==";
        private string resourcePayload = "{{\"@context\":\"http://testservice/$metadata#customers/$entity\",\"id\":\"1\"{0}}}";

        [Fact]
        public void ReadPrimitiveCollectionProperty()
        {
            string payload = String.Format(resourcePayload,
                ",\"comments\":[\"one\",\"two\",null]"
                );

            foreach (Variant variant in GetVariants(null))
            {
                int expectedPropertyCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                var comments = resource.Properties.FirstOrDefault(p => p.Name == "comments").ODataValue as ODataCollectionValue;
                comments.Should().NotBeNull();
                List<object> collection = comments.Items.ToList();
                collection.Count().Should().Be(3);
                collection[0].Should().Be("one");
                collection[1].Should().Be("two");
                collection[2].Should().BeNull();
            }
        }

        [Fact]
        public void CanReadAllNonKeyPropertiesAsStream()
        {
            string payload = String.Format(resourcePayload,
                ",\"comments\":[\"one\",\"two\",null]"
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
            {
                return true;
            };

            foreach (Variant variant in GetVariants(ShouldStream))
            {
                int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Primitive:
                            Assert.False(true, "Should not read as Primitive if Caller Specifies Stream");
                            break;

                        case ODataReaderState.NestedProperty:
                            ((ODataPropertyInfo)reader.Item).Name.Should().NotBe("id", "Should never stream id");
                            break;

                        case ODataReaderState.Stream:
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(3);
                propertyValues[0].Should().Be("one");
                propertyValues[1].Should().Be("two");
                propertyValues[2].Should().Be("");
            }
        }

        [Fact]
        public void CanStreamCollections()
        {
            string payload = String.Format(resourcePayload,
                ",\"comments\":[\"one\",\"two\",null]"
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return isCollection;
                };

            foreach (Variant variant in GetVariants(ShouldStream))
            {
                int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                ODataResource resource = null;
                List<object> propertyValues = new List<object>();

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Primitive:
                            object primitive = reader.Item as ODataPrimitiveValue;
                            propertyValues.Add(primitive);
                            break;

                        case ODataReaderState.Stream:
                            Assert.False(true, "Should not read as stream if caller returns false");
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(3);
                ((ODataPrimitiveValue)propertyValues[0]).Value.Should().Be("one");
                ((ODataPrimitiveValue)propertyValues[1]).Value.Should().Be("two");
                propertyValues[2].Should().BeNull();
            }
        }

        [Fact]
        public void CanReadUntypedCollection()
        {
            string payload = String.Format(resourcePayload,
                ",\"untypedCollection@type\":\"Collection(Untyped)\"" +
                ",\"untypedCollection\":[" +
                "\"" + binaryValue + "\"" +
                ",\"" + stringValue + "\"" +
                ",\"" + stringValue + "\"" +
                ",null" +
                ",true" +
                ",false" +
                ",-10.5]"
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> StreamCollection =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return isCollection;
                };

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> StreamAll =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return true;
                };

            foreach (Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream in new Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool>[] { StreamCollection, StreamAll })
            {
                foreach (Variant variant in GetVariants(ShouldStream))
                {
                    int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                    ODataResource resource = null;
                    List<object> propertyValues = null;

                    ODataReader reader = CreateODataReader(payload, variant);
                    while (reader.Read())
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                if (resource == null)
                                {
                                    resource = reader.Item as ODataResource;
                                }
                                else
                                {
                                    propertyValues.Add(reader.Item);
                                }
                                break;

                            case ODataReaderState.Primitive:
                                ODataPrimitiveValue primitive = reader.Item as ODataPrimitiveValue;
                                propertyValues.Add(primitive.Value);
                                break;

                            case ODataReaderState.Stream:
                                propertyValues.Add(ReadStream(reader));
                                break;

                            case ODataReaderState.NestedResourceInfoStart:
                                ODataNestedResourceInfo info = reader.Item as ODataNestedResourceInfo;
                                info.Name.Should().Be("untypedCollection");
                                propertyValues.Should().BeNull();
                                propertyValues = new List<object>();
                                break;
                        }
                    }

                    resource.Should().NotBeNull();
                    resource.Properties.Count().Should().Be(expectedPropertyCount);
                    resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                    propertyValues.Count.Should().Be(7);
                    propertyValues[0].Should().Be(binaryValue);
                    propertyValues[1].Should().Be(stringValue);
                    propertyValues[2].Should().Be(stringValue);
                    propertyValues[3].Should().BeNull();
                    propertyValues[4].Should().Be(true);
                    propertyValues[5].Should().Be(false);
                    ((Decimal)propertyValues[6]).Should().Be(-10.5m);
                }
            }
        }

        [Fact]
        public void NotStreamingCollectionsWorks()
        {
            int numberOfTimesCalled = 0;
            string payload = String.Format(resourcePayload,
                ",\"comments\":[\"one\",\"two\",null]" +
                ",\"name\":\"Thor\""
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    numberOfTimesCalled++;
                    return !isCollection;
                };

            foreach (Variant variant in GetVariants(ShouldStream))
            {
                numberOfTimesCalled = 0;
                int expectedPropertyCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                string currentProperty;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Primitive:
                            Assert.False(true, "Should not read as Primitive if collection not streamed");
                            break;

                        case ODataReaderState.NestedProperty:
                            ODataPropertyInfo propertyInfo = reader.Item as ODataPropertyInfo;
                            currentProperty = propertyInfo.Name;
                            propertyInfo.Name.Should().NotBe("id", "Should never stream id");
                            break;

                        case ODataReaderState.Stream:
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                numberOfTimesCalled.Should().Be(2);
                var comments = resource.Properties.FirstOrDefault(p => p.Name == "comments").ODataValue as ODataCollectionValue;
                comments.Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be("Thor");
                List<object> collection = comments.Items.ToList();
                collection.Count().Should().Be(3);
                collection[0].Should().Be("one");
                collection[1].Should().Be("two");
                collection[2].Should().BeNull();
            }
        }

        [Fact]
        public void ReadStreamProperty()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Primitive:
                            Assert.False(true, "Should not read as Primitive if Caller Specifies Stream");
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("stream");

                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            streamInfo.PrimitiveTypeKind.Should().Be(EdmPrimitiveTypeKind.None);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryValue);
            }
        }

        [Fact]
        public void ReadUndeclaredStreamProperty()
        {
            string payload = String.Format(resourcePayload,
                ",\"undeclaredStream@type\":\"Stream\"" +
                ",\"undeclaredStream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                int expectedPropertyCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("undeclaredStream");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryValue);
            }
        }

        [Fact]
        public void ReadUndeclaredStreamPropertyReference()
        {
            string payload = String.Format(resourcePayload,
                ",\"undeclaredStream@mediaContentType\":\"text/plain\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                int expectedPropertyCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            Assert.False(true, "Should Not enter nested property without a value");
                            break;

                        case ODataReaderState.Stream:
                            Assert.False(true, "Should Not enter stream without a value");
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                ODataStreamReferenceValue undeclaredStreamProperty = resource.Properties.FirstOrDefault(p => p.Name == "undeclaredStream").Value as ODataStreamReferenceValue;
                undeclaredStreamProperty.Should().NotBeNull();
                undeclaredStreamProperty.ContentType.Should().Be("text/plain");
            }
        }

        [Fact]
        public void ReadStreamPropertyWithMetadata()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream@mediaContentType\":\"image/jpg\"" +
                ",\"stream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataStreamPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("stream");
                            propertyInfo.ContentType.Should().Be("image/jpg");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryValue);
            }
        }

        [Fact]
        public void ReadTextStreamPropertyWithMetadata()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream@mediaContentType\":\"text/plain\"" +
                ",\"stream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataStreamPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("stream");
                            propertyInfo.ContentType.Should().Be("text/plain");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryValue);
            }
        }

        [Fact]
        public void ReadXmlStreamPropertyWithMetadata()
        {
            string xmlValue = @"<Outer><Element>123</Element></Outer>";
            string payload = String.Format(resourcePayload,
                ",\"stream@mediaContentType\":\"application/xml\"" +
                ",\"stream\":\"" + xmlValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataStreamPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("stream");
                            propertyInfo.ContentType.Should().Be("application/xml");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(xmlValue);
            }
        }

        [Fact]
        public void ReadUndeclaredStreamPropertyWithMetadata()
        {
            string payload = String.Format(resourcePayload,
                ",\"undeclaredStream@mediaContentType\":\"image/jpg\"" +
                ",\"undeclaredStream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                int expectedCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataStreamPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("undeclaredStream");
                            propertyInfo.ContentType.Should().Be("image/jpg");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryValue);
            }
        }

        [Fact]
        public void ReadUndeclaredTextStreamPropertyWithMetadata()
        {
            string payload = String.Format(resourcePayload,
                ",\"undeclaredStream@mediaContentType\":\"text/plain\"" +
                ",\"undeclaredStream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                int expectedCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;
                List<string> propertyValues = new List<string>();
                ODataStreamPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("undeclaredStream");
                            propertyInfo.ContentType.Should().Be("text/plain");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryValue);
            }
        }

        [Fact]
        public void ReadStreamPropertyAsJson()
        {
            string jsonString = "{\"stringProp\":\"string\",\"numProp\":-10.5,\"boolProp\":true,\"arrayProp\":[\"value1\",-10.5,false]}";
            string payload = String.Format(resourcePayload,
                ",\"stream@mediaContentType\":\"application/json\"" +
                ",\"stream\":" + jsonString);

            foreach (Variant variant in GetVariants(null, false))
            {
                ODataResource resource = null;
                string jsonStream = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            ODataStreamPropertyInfo propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("stream");
                            break;

                        case ODataReaderState.Stream:
                            var streamValue = reader.Item as ODataStreamItem;
                            streamValue.Should().NotBeNull();
                            streamValue.ContentType.Should().Be("application/json");
                            using (TextReader textReader = reader.CreateTextReader())
                            {
                                jsonStream = textReader.ReadToEnd();
                            }
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                ODataStreamReferenceValue streamReference = resource.Properties.FirstOrDefault(p => p.Name == "stream").Value as ODataStreamReferenceValue;
                streamReference.Should().NotBeNull();
                streamReference.ContentType.Should().Be("application/json");
                jsonStream.Should().Be(jsonString);
            }
        }

        [Fact]
        public void ReadUndeclaredPropertyAsJson()
        {
            string jsonString = "{\"stringProp\":\"string\",\"numProp\":-10.5,\"boolProp\":true,\"arrayProp\":[\"value1\",-10.5,false]}";

            string payload = String.Format(resourcePayload,
                ",\"jsonStream@mediaContentType\":\"application/json\"" +
                ",\"jsonStream\":" + jsonString
                );

            foreach (Variant variant in GetVariants(null, false))
            {
                ODataResource resource = null;
                int expectedCount = variant.IsRequest ? 2 : 3;
                string jsonStream = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            ODataPropertyInfo propertyInfo = reader.Item as ODataStreamPropertyInfo;
                            propertyInfo.Should().NotBeNull();
                            propertyInfo.Name.Should().Be("jsonStream");
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            streamInfo.Should().NotBeNull();
                            streamInfo.ContentType.Should().Be("application/json");
                            using (TextReader textReader = reader.CreateTextReader())
                            {
                                jsonStream = textReader.ReadToEnd();
                            }
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                ODataStreamReferenceValue streamReference = resource.Properties.FirstOrDefault(p => p.Name == "jsonStream").Value as ODataStreamReferenceValue;
                streamReference.Should().NotBeNull();
                streamReference.ContentType.Should().Be("application/json");
                jsonStream.Should().Be(jsonString);
            }
        }

        [Fact]
        public void CannotReadPartialStream()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                Action readPartialStream = () =>
                {
                    ODataResource resource = null;
                    List<object> propertyValues = new List<object>();

                    ODataReader reader = CreateODataReader(payload, variant);
                    while (reader.Read())
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                resource = reader.Item as ODataResource;
                                break;

                            case ODataReaderState.Stream:
                                ODataStreamItem stream = reader.Item as ODataStreamItem;
                                propertyValues.Add(ReadPartialStream(reader));
                                break;
                        }
                    }
                };

                readPartialStream.ShouldThrow<ODataException>().WithMessage(Strings.ODataReaderCore_ReadCalledWithOpenStream);
            }
        }

        [Fact]
        public void CannotIgnoreInlineStream()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream\":\"" + binaryValue + "\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                Action readPartialStream = () =>
                {
                    ODataResource resource = null;
                    List<object> propertyValues = new List<object>();

                    ODataReader reader = CreateODataReader(payload, variant);
                    while (reader.Read())
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                resource = reader.Item as ODataResource;
                                break;

                            case ODataReaderState.Stream:
                                // Don't read the stream
                                break;
                        }
                    }
                };

                readPartialStream.ShouldThrow<ODataException>().WithMessage(Strings.ODataReaderCore_ReadCalledWithOpenStream);
            }
        }

        [Fact]
        public void CanIgnoreStreamReference()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream@mediaContentType\":\"text/plain\""
                );

            foreach (Variant variant in GetVariants(null))
            {
                ODataResource resource = null;
                List<object> propertyValues = new List<object>();

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Stream:
                            // Don't read the stream
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                ODataStreamReferenceValue propertyInfo = resource.Properties.FirstOrDefault(p => p.Name == "stream").Value as ODataStreamReferenceValue;
                propertyInfo.Should().NotBeNull();
                propertyInfo.ContentType.Should().Be("text/plain");
            }
        }

        [Fact]
        public void ReadStreamCollection()
        {
            string payload = String.Format(resourcePayload,
                ",\"streamCollection@type\":\"Collection(Stream)\"" +
                ",\"streamCollection\":[\"" + binaryValue + "\",\"" + binaryValue + "\"]"
                );

            foreach (Variant variant in GetVariants(null))
            {
                int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                ODataResource resource = null;
                List<string> streamValues = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedResourceInfoStart:
                            ODataNestedResourceInfo nestedInfo = reader.Item as ODataNestedResourceInfo;
                            nestedInfo.Name.Should().Be("streamCollection");
                            streamValues.Should().BeNull();
                            streamValues = new List<string>();
                            break;

                        case ODataReaderState.Stream:
                            streamValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                streamValues.Should().NotBeNull();
                streamValues.Count.Should().Be(2);
                streamValues[0].Should().Be(binaryString);
                streamValues[1].Should().Be(binaryString);
            }
        }

        [Fact]
        public void ReadStringPropertyAsStream()
        {
            string payload = String.Format(resourcePayload,
                ",\"name\":\"Thor\""
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return propertyName == "name";
                };

            foreach (Variant variant in GetVariants(ShouldStream))
            {
                int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                ODataResource resource = null;
                List<string> streamValues = new List<string>();
                ODataPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo.Should().BeNull();
                            propertyInfo = reader.Item as ODataPropertyInfo;
                            break;

                        case ODataReaderState.Stream:
                            streamValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyInfo.Name.Should().Be("name");
                propertyInfo.PrimitiveTypeKind.Should().Be(EdmPrimitiveTypeKind.String);
                streamValues.Should().NotBeNull();
                streamValues.Count.Should().Be(1);
                streamValues[0].Should().Be("Thor");
            }
        }

        [Fact]
        public void CanReadStringAndBinaryPropertiesIndividually()
        {
            string payload = String.Format(resourcePayload,
                ",\"name\":\"Thor\"" +
                ",\"isMarvel\":true" +
                ",\"nickName\":null" +
                ",\"gender\":\"male\"" +
                ",\"age\":4000"
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return true;
                };

            foreach (Variant variant in GetVariants(ShouldStream))
            {
                int expectedPropertyCount = variant.IsRequest ? 4 : 5;
                ODataResource resource = null;
                List<object> propertyValues = new List<object>();

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Stream:
                            propertyValues.Add(ReadStream(reader));
                            break;

                        case ODataReaderState.Primitive:
                            var primitive = reader.Item as ODataPrimitiveValue;
                            propertyValues.Add(primitive.Value);
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                resource.Properties.FirstOrDefault(p => p.Name == "isMarvel").Value.Should().Be(true);
                ((ODataEnumValue)resource.Properties.FirstOrDefault(p => p.Name == "gender").Value).Value.Should().Be("male");
                resource.Properties.FirstOrDefault(p => p.Name == "age").Value.Should().Be(4000);
                propertyValues.Count.Should().Be(2);
                propertyValues[0].Should().Be("Thor");
                propertyValues[1].Should().Be("");
            }
        }

        [Fact]
        public void ReadBinaryPropertyAsStream()
        {
            string payload = String.Format(resourcePayload,
                ",\"binaryAsStream\":\"" + binaryValue + "\""
                );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return type != null && type.IsBinary();
                };

            foreach (Variant variant in GetVariants(ShouldStream))
            {
                int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                ODataResource resource = null;
                List<string> streamValues = new List<string>();
                ODataPropertyInfo propertyInfo = null;

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.NestedProperty:
                            propertyInfo.Should().BeNull();
                            propertyInfo = reader.Item as ODataPropertyInfo;
                            break;

                        case ODataReaderState.Stream:
                            streamValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyInfo.Name.Should().Be("binaryAsStream");
                propertyInfo.PrimitiveTypeKind.Should().Be(EdmPrimitiveTypeKind.Binary);
                streamValues.Should().NotBeNull();
                streamValues.Count.Should().Be(1);
                streamValues[0].Should().Be(binaryString);
            }
        }

        [Fact]
        public void CannotSkipStreamProperties()
        {
            string payload = String.Format(resourcePayload,
             ",\"name\":\"Thor\""
             );

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return true;
                };


            foreach (Variant variant in GetVariants(ShouldStream))
            {
                Action action = () =>
                {
                    ODataReader reader = CreateODataReader(payload, variant);
                    while (reader.Read())
                    {
                    }
                };

                action.ShouldThrow<ODataException>().WithMessage(Strings.ODataReaderCore_ReadCalledWithOpenStream);
            }
        }

        #region Helpers
        private IEdmModel GetModel()
        {
            if (this.model == null)
            {
                var model = new EdmModel();

                var enumType = new EdmEnumType("test", "gender");
                enumType.AddMember(new EdmEnumMember(enumType, "male", new EdmEnumMemberValue(0)));
                enumType.AddMember(new EdmEnumMember(enumType, "female", new EdmEnumMemberValue(1)));

                var customerType = new EdmEntityType("test", "customer", null, false, true);
                customerType.AddKeys(customerType.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false));
                customerType.AddStructuralProperty("name", EdmPrimitiveTypeKind.String);
                customerType.AddStructuralProperty("nickName", EdmPrimitiveTypeKind.String);
                customerType.AddStructuralProperty("age", EdmPrimitiveTypeKind.Int32, false);
                customerType.AddStructuralProperty("isMarvel", EdmPrimitiveTypeKind.Boolean, false);
                customerType.AddStructuralProperty("gender", new EdmEnumTypeReference(enumType, true));
                customerType.AddStructuralProperty("stream", EdmPrimitiveTypeKind.Stream);
                customerType.AddStructuralProperty("binaryAsStream", EdmPrimitiveTypeKind.Binary);
                customerType.AddStructuralProperty("comments", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
                model.AddElement(customerType);

                var container = model.AddEntityContainer("test", "container");
                this.customersEntitySet = container.AddEntitySet("customers", customerType);

                this.model = model;
            }

            return this.model;
        }

        private class Variant
        {
            public string Description;
            public ODataVersion Version;
            public MetadataLevel MetadataLevel;
            public ODataMessageReaderSettings Settings;
            public bool IsRequest;
            public bool IsStreaming;
        }

        private enum MetadataLevel
        {
            None,
            Minimal,
            Full
        }

        private IEnumerable<Variant> GetVariants(Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStream, bool includeUnordered = true)
        {
            List<Variant> variants = new List<Variant>();
            foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
            {
                foreach (MetadataLevel level in new MetadataLevel[]
                {
                    MetadataLevel.None,
                    MetadataLevel.Minimal,
                    MetadataLevel.Full
                })
                {
                    foreach (bool isRequest in new bool[] { true, false })
                    {
                        foreach (bool isStreaming in includeUnordered ? new bool[] { true, false } : new bool[] { true})
                        {
                            variants.Add(new Variant
                            {
                                Description = String.Format("ODataVersion={0}, MetadataLevel={1}, isRequest={2}, Streaming={3}",
                                    version.ToString(),
                                    level.ToString(),
                                    isRequest ? "true" : "false",
                                    isStreaming ? "true" : "false"),
                                MetadataLevel = level,
                                Settings = new ODataMessageReaderSettings
                                {
                                    Version = version,
                                    ReadUntypedAsString = false,
                                    ReadAsStreamFunc = readAsStream,
                                },
                                IsRequest = isRequest,
                                IsStreaming = isStreaming,
                                Version = version,
                            });
                        }
                    }
                }
            }

            return variants;
        }

        private ODataReader CreateODataReader(string payload, Variant variant)
        {
            if(variant.Version < ODataVersion.V401)
            {
                payload = payload.Replace("@", "@odata.");
            }

            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(payload);
            writer.Flush();
            stream.Position = 0;

            ODataMessageReader messageReader;

            if (variant.IsRequest)
            {
                IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
                if (variant.IsStreaming)
                {
                    requestMessage.SetHeader("Content-Type", "application/json;odata.streaming=true");
                }
                messageReader = new ODataMessageReader(requestMessage, variant.Settings, this.GetModel());
            }
            else
            {
                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = stream };
                if (variant.IsStreaming)
                {
                    responseMessage.SetHeader("Content-Type", "application/json;odata.streaming=true");
                }
                    messageReader = new ODataMessageReader(responseMessage, variant.Settings, this.GetModel());
            }

            ODataReader reader = messageReader.CreateODataResourceReader(this.customersEntitySet, this.customersEntitySet.EntityType());
            return reader;
        }

        private string ReadStream(ODataReader reader)
        {
            ODataStreamItem streamValue = reader.Item as ODataStreamItem;
            streamValue.Should().NotBeNull();
            string result;
            if (streamValue.PrimitiveTypeKind == EdmPrimitiveTypeKind.String ||
                streamValue.PrimitiveTypeKind == EdmPrimitiveTypeKind.None)
            {
                using (TextReader textReader = reader.CreateTextReader())
                {
                    result = textReader.ReadToEnd();
                }
            }
            else
            {
                using (Stream stream = reader.CreateReadStream())
                {
                    result = new BinaryReader(stream).ReadString();
                }
            }

            return result;
        }

        private string ReadPartialStream(ODataReader reader)
        {
            ODataStreamItem streamValue = reader.Item as ODataStreamItem;
            streamValue.Should().NotBeNull();
            if (streamValue.PrimitiveTypeKind == EdmPrimitiveTypeKind.String)
            {
                TextReader textReader = reader.CreateTextReader();
                {
                    // read a single character
                    textReader.Read();
                }
            }
            else
            {
                Stream stream = reader.CreateReadStream();
                {
                    stream.Read(new byte[2],0,2);
                }
            }

            return "partialStream";
        }

        #endregion Helpers
    }
}
