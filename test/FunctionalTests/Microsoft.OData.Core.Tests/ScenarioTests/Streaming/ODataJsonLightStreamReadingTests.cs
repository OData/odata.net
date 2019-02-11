//---------------------------------------------------------------------
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
using Microsoft.OData.JsonLight;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;
using System.Threading.Tasks;

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
                var comments = resource.Properties.FirstOrDefault(p => p.Name == "comments").ODataValue as ODataCollectionValue; ;
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

                        case ODataReaderState.Stream:
                            ((ODataStreamValue)reader.Item).PropertyName.Should().NotBe("id", "Should never stream id");
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

                        case ODataReaderState.Stream:
                            ((ODataStreamValue)reader.Item).PropertyName.Should().NotBe("id", "Should never stream id");
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

                        case ODataReaderState.Stream:
                            var streamValue = reader.Item as ODataStreamReferenceValue;
                            streamValue.Should().NotBeNull();
                            streamValue.PropertyName.Should().NotBe("id", "Should never stream id");
                            propertyValues.Add(ReadStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(2);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                propertyValues.Count.Should().Be(1);
                propertyValues[0].Should().Be(binaryString);
            }
        }

//        [Fact]
        public void ReadJsonStreamProperty()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream@mediaContentType\":\"application/json\"" +
                ",\"stream\":{stringProp:\"string\",numProp:-10.5,boolProp:true,arrayProp[\"value1\",-10.5,false]}"
                );

            foreach (Variant variant in GetVariants(null))
            {
                ODataResource resource = null;
                
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

                        case ODataReaderState.Stream:
                            var streamValue = reader.Item as ODataStreamReferenceValue;
                            streamValue.Should().NotBeNull();
                            streamValue.PropertyName.Should().NotBe("id", "Should never stream id");
                            streamValue.ContentType.Should().Be("application/json");
                            ReadStream(reader);
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(1);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
            }
        }

        // mikep todo: add one for reading partial textStreams
        // mikep todo: need to make binary value > buffersize (1024)
        // mikep todo: currently throws exception if you don't read to end
        [Fact]
        public void CanReadPartialStream()
        {
            string payload = String.Format(resourcePayload,
                ",\"stream\":\"" + binaryValue + "\"" +
                ",\"untypedCollection@type\":\"Collection(Untyped)\"" +
                ",\"untypedCollection\":[" +
                "\"" + binaryValue + "\"" +
                ",-10.5]"
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
                            ODataStreamValue stream = reader.Item as ODataStreamValue;
                            propertyValues.Add(ReadPartialStream(reader));
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(3);
                resource.Properties.FirstOrDefault(p => p.Name == "id").Should().NotBeNull();
                resource.Properties.FirstOrDefault(p => p.Name == "stream").Value.Should().NotBeNull();
                var collection = resource.Properties.FirstOrDefault(p => p.Name == "untypedCollection").Value as ODataCollectionValue;
                collection.Should().NotBeNull();
                collection.Items.Count().Should().Be(2);
                collection.Items.First().Should().Be(binaryValue);
                ((Decimal)collection.Items.Last()).Should().Be(-10.5m);
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

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
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
                int expectedPropertyCount = variant.IsRequest ? 2 : 3;
                ODataResource resource = null;
                List<string> streamValues = new List<string>();

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
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
                streamValues.Count.Should().Be(1);
                streamValues[0].Should().Be(binaryString);
            }
        }

//        [Fact]
        public void CanSkipStreamProperties()
        {
            string payload = String.Format(resourcePayload,
             ",\"name\":\"Thor\"" +
             ",\"stream\":\"" + binaryValue + "\"" +
             ",\"binaryAsStream\":\"" + binaryValue + "\""
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

                ODataReader reader = CreateODataReader(payload, variant);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            resource = reader.Item as ODataResource;
                            break;

                        case ODataReaderState.Stream:
                            // do not create nor read the stream
                            break;
                    }
                }

                resource.Should().NotBeNull();
                resource.Properties.Count().Should().Be(expectedPropertyCount);
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

        private IEnumerable<Variant> GetVariants(Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStream)
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
                        foreach (bool isStreaming in new bool[] { true, false })
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
                                    ReadAsStream = readAsStream,
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
            ODataStreamValue streamValue = reader.Item as ODataStreamValue;
            streamValue.Should().NotBeNull();
            string result;
            if (streamValue.TypeKind == EdmPrimitiveTypeKind.String)
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

        private void ReadJson(ODataReader reader)
        {
            ODataStreamValue streamValue = reader.Item as ODataStreamValue;
            streamValue.Should().NotBeNull();
            streamValue.TypeKind.Should().Be(EdmPrimitiveTypeKind.Stream);

            using (Stream stream = reader.CreateReadStream())
            {
            }
        }

        private string ReadPartialStream(ODataReader reader)
        {
            ODataStreamValue streamValue = reader.Item as ODataStreamValue;
            streamValue.Should().NotBeNull();
            if (streamValue.TypeKind == EdmPrimitiveTypeKind.String)
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
                    // read two bytes from the stream
                    stream.Read(new byte[2],0,2);
                }
            }

            return "partialStream";
        }

        #endregion Helpers
    }
}
