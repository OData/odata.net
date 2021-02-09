//---------------------------------------------------------------------
// <copyright file="ODataJsonLightStreamReadingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                var comments = resource.Properties.FirstOrDefault(p => p.Name == "comments").ODataValue as ODataCollectionValue;
                Assert.NotNull(comments);
                List<object> collection = comments.Items.ToList();
                Assert.Equal(3, collection.Count());
                Assert.Equal("one", collection[0]);
                Assert.Equal("two", collection[1]);
                Assert.Null(collection[2]);
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
                            Assert.Equal("id", ((ODataPropertyInfo)reader.Item).Name);
                            break;

                        case ODataReaderState.Stream:
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(3, propertyValues.Count);
                Assert.Equal("one", propertyValues[0]);
                Assert.Equal("two", propertyValues[1]);
                Assert.Equal("", propertyValues[2]);
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

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(3, propertyValues.Count);
                Assert.Equal("one", ((ODataPrimitiveValue)propertyValues[0]).Value);
                Assert.Equal("two", ((ODataPrimitiveValue)propertyValues[1]).Value);
                Assert.Null(propertyValues[2]);
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
                ", { \"name\": \"Betty\",\"age@type\":\"#Int32\",\"age\": 18}" +
                ", { \"@type\": \"#test.customer\",\"name\": \"Betty\",\"age\": 18}" +
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

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> StreamNone =
                (IEdmPrimitiveType type, bool isCollection, string propertyName, IEdmProperty property) =>
                {
                    return false;
                };

            foreach (Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ShouldStream in new Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool>[] { StreamCollection, StreamAll, StreamNone })
            {
                foreach (Variant variant in GetVariants(ShouldStream))
                {
                    int expectedPropertyCount = variant.IsRequest ? 1 : 2;
                    List<object> collectionValues = null;
                    ODataResource currentResource = null;
                    Stack<ODataResource> resources = new Stack<ODataResource>();
                    ODataPropertyInfo currentProperty = null;
                    Stack<List<ODataProperty>> nestedProperties = new Stack<List<ODataProperty>>();
                    ODataReader reader = CreateODataReader(payload, variant);

                    while (reader.Read())
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                currentResource = reader.Item as ODataResource;
                                if (resources.Count == 1)
                                {
                                    collectionValues.Add(reader.Item);
                                }
                                resources.Push(currentResource);
                                nestedProperties.Push(new List<ODataProperty>());

                                break;

                            case ODataReaderState.ResourceEnd:
                                currentResource = resources.Pop();
                                List<ODataProperty> properties = nestedProperties.Pop();
                                if (currentResource != null)
                                {
                                    properties.AddRange(currentResource.NonComputedProperties);
                                    currentResource.Properties = properties;
                                }
                                break;

                            case ODataReaderState.Primitive:
                                if (resources.Count == 1)
                                {
                                    ODataPrimitiveValue primitive = reader.Item as ODataPrimitiveValue;
                                    collectionValues.Add(primitive.Value);
                                }

                                break;

                            case ODataReaderState.Stream:
                                string streamValue = ReadStream(reader);
                                if (resources.Count == 1)
                                {
                                    collectionValues.Add(streamValue);
                                }
                                else if (currentProperty != null)
                                {
                                    nestedProperties.Peek().Add(new ODataProperty { Name = currentProperty.Name, Value = streamValue });
                                }
                                currentProperty = null;

                                break;

                            case ODataReaderState.NestedProperty:
                                currentProperty = reader.Item as ODataPropertyInfo;
                                break;

                            case ODataReaderState.NestedResourceInfoStart:
                                ODataNestedResourceInfo info = reader.Item as ODataNestedResourceInfo;
                                Assert.Equal("untypedCollection", info.Name);
                                Assert.Null(collectionValues);
                                collectionValues = new List<object>();
                                break;
                        }
                    }

                    Assert.NotNull(currentResource);
                    Assert.Equal(expectedPropertyCount, currentResource.Properties.Count());
                    Assert.NotNull(currentResource.Properties.FirstOrDefault(p => p.Name == "id"));
                    Assert.Equal(9, collectionValues.Count);
                    Assert.Equal(binaryValue, collectionValues[0]);
                    Assert.Equal(stringValue, collectionValues[1]);
                    Assert.Equal(stringValue, collectionValues[2]);
                    ValidateResource(collectionValues[3]);
                    ValidateResource(collectionValues[4]);
                    Assert.Equal("test.customer", ((ODataResource)collectionValues[4]).TypeName);
                    Assert.Null(collectionValues[5]);
                    Assert.Equal(true, collectionValues[6]);
                    Assert.Equal(false, collectionValues[7]);
                    Assert.Equal(-10.5m, (Decimal)collectionValues[8]);
                }
            }
        }

        private void ValidateResource(object value)
        {
            ODataResource resource = value as ODataResource;
            Assert.NotNull(resource);
            ValidateProperty(resource, "name", "Betty");
            ValidateProperty(resource, "age", 18);
        }

        private void ValidateProperty(ODataResource resource, string propertyName, object expectedValue)
        {
            ODataProperty property = resource.Properties.FirstOrDefault(p => p.Name == propertyName);
            if (property != null)
            {
                Assert.Equal(expectedValue, property.Value);
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
                            Assert.NotEqual("id", propertyInfo.Name);
                            break;

                        case ODataReaderState.Stream:
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(2, numberOfTimesCalled);
                var comments = resource.Properties.FirstOrDefault(p => p.Name == "comments").ODataValue as ODataCollectionValue;
                Assert.NotNull(comments);
                Assert.Single(propertyValues);
                Assert.Equal("Thor", propertyValues[0]);
                List<object> collection = comments.Items.ToList();
                Assert.Equal(3, collection.Count());
                Assert.Equal("one", collection[0]);
                Assert.Equal("two", collection[1]);
                Assert.Null(collection[2]);
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("stream", propertyInfo.Name);

                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            Assert.Equal(EdmPrimitiveTypeKind.None, streamInfo.PrimitiveTypeKind);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(2, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Single(propertyValues);
                Assert.Equal(binaryValue, propertyValues[0]);
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("undeclaredStream", propertyInfo.Name);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(binaryValue, Assert.Single(propertyValues));
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

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                ODataStreamReferenceValue undeclaredStreamProperty = resource.Properties.FirstOrDefault(p => p.Name == "undeclaredStream").Value as ODataStreamReferenceValue;
                Assert.NotNull(undeclaredStreamProperty);
                Assert.Equal("text/plain", undeclaredStreamProperty.ContentType);
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("stream", propertyInfo.Name);
                            Assert.Equal("image/jpg", propertyInfo.ContentType);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(2, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(binaryValue, Assert.Single(propertyValues));
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("stream", propertyInfo.Name);
                            Assert.Equal("text/plain", propertyInfo.ContentType);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(2, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(binaryValue, Assert.Single(propertyValues));
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("stream", propertyInfo.Name);
                            Assert.Equal("application/xml", propertyInfo.ContentType);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(2, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(xmlValue, Assert.Single(propertyValues));
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("undeclaredStream", propertyInfo.Name);
                            Assert.Equal("image/jpg", propertyInfo.ContentType);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(binaryValue, Assert.Single(propertyValues));
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("undeclaredStream", propertyInfo.Name);
                            Assert.Equal("text/plain", propertyInfo.ContentType);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(binaryValue, Assert.Single(propertyValues));
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("stream", propertyInfo.Name);
                            break;

                        case ODataReaderState.Stream:
                            var streamValue = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamValue);
                            Assert.Equal("application/json", streamValue.ContentType);
                            using (TextReader textReader = reader.CreateTextReader())
                            {
                                jsonStream = textReader.ReadToEnd();
                            }
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(2, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                ODataStreamReferenceValue streamReference = resource.Properties.FirstOrDefault(p => p.Name == "stream").Value as ODataStreamReferenceValue;
                Assert.NotNull(streamReference);
                Assert.Equal("application/json", streamReference.ContentType);
                Assert.Equal(jsonString, jsonStream);
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
                            Assert.NotNull(propertyInfo);
                            Assert.Equal("jsonStream", propertyInfo.Name);
                            break;

                        case ODataReaderState.Stream:
                            var streamInfo = reader.Item as ODataStreamItem;
                            Assert.NotNull(streamInfo);
                            Assert.Equal("application/json", streamInfo.ContentType);
                            using (TextReader textReader = reader.CreateTextReader())
                            {
                                jsonStream = textReader.ReadToEnd();
                            }
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                ODataStreamReferenceValue streamReference = resource.Properties.FirstOrDefault(p => p.Name == "jsonStream").Value as ODataStreamReferenceValue;
                Assert.NotNull(streamReference);
                Assert.Equal("application/json", streamReference.ContentType);
                Assert.Equal(jsonString, jsonStream);
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

                readPartialStream.Throws<ODataException>(Strings.ODataReaderCore_ReadCalledWithOpenStream);
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

                readPartialStream.Throws<ODataException>(Strings.ODataReaderCore_ReadCalledWithOpenStream);
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

                Assert.NotNull(resource);
                Assert.Equal(2, resource.Properties.Count());
                ODataStreamReferenceValue propertyInfo = resource.Properties.FirstOrDefault(p => p.Name == "stream").Value as ODataStreamReferenceValue;
                Assert.NotNull(propertyInfo);
                Assert.Equal("text/plain", propertyInfo.ContentType);
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
                            Assert.Equal("streamCollection", nestedInfo.Name);
                            Assert.Null(streamValues);
                            streamValues = new List<string>();
                            break;

                        case ODataReaderState.Stream:
                            streamValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.NotNull(streamValues);
                Assert.Equal(2, streamValues.Count);
                Assert.Equal(binaryString, streamValues[0]);
                Assert.Equal(binaryString, streamValues[1]);
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
                            Assert.Null(propertyInfo);
                            propertyInfo = reader.Item as ODataPropertyInfo;
                            break;

                        case ODataReaderState.Stream:
                            streamValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal("name", propertyInfo.Name);
                Assert.Equal(EdmPrimitiveTypeKind.String, propertyInfo.PrimitiveTypeKind);
                Assert.NotNull(streamValues);
                Assert.Equal("Thor", Assert.Single(streamValues));
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

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal(true, resource.Properties.FirstOrDefault(p => p.Name == "isMarvel").Value);
                Assert.Equal("male", ((ODataEnumValue)resource.Properties.FirstOrDefault(p => p.Name == "gender").Value).Value);
                Assert.Equal(4000, resource.Properties.FirstOrDefault(p => p.Name == "age").Value);
                Assert.Equal(2, propertyValues.Count);
                Assert.Equal("Thor", propertyValues[0]);
                Assert.Equal("", propertyValues[1]);
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
                            Assert.Null(propertyInfo);
                            propertyInfo = reader.Item as ODataPropertyInfo;
                            break;

                        case ODataReaderState.Stream:
                            streamValues.Add(ReadStream(reader));
                            break;
                    }
                }

                Assert.NotNull(resource);
                Assert.Equal(expectedPropertyCount, resource.Properties.Count());
                Assert.NotNull(resource.Properties.FirstOrDefault(p => p.Name == "id"));
                Assert.Equal("binaryAsStream", propertyInfo.Name);
                Assert.Equal(EdmPrimitiveTypeKind.Binary, propertyInfo.PrimitiveTypeKind);
                Assert.NotNull(streamValues);
                Assert.Equal(binaryString, Assert.Single(streamValues));
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

                action.Throws<ODataException>(Strings.ODataReaderCore_ReadCalledWithOpenStream);
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
            if (variant.Version < ODataVersion.V401)
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
            Assert.NotNull(streamValue);
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
            Assert.NotNull(streamValue);
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
