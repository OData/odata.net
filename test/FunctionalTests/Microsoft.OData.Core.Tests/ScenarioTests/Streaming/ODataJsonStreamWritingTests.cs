//---------------------------------------------------------------------
// <copyright file="ODataJsonStreamWritingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonStreamWritingTests
    {
        private EdmModel model;
        private IEdmEntitySet customersEntitySet;
        private string binaryValue;
        private string resourcePayload = "{{\"@context\":\"http://testservice/$metadata#customers/$entity\",\"id\":\"1\"{0}}}";

        public ODataJsonStreamWritingTests()
        {
            binaryValue = Convert.ToBase64String(CreateBinaryStreamValue().ToArray());
        }

        private ODataResource resource = new ODataResource
        {
            Properties = new ODataProperty[]
            {
                new ODataProperty { Name = "id", Value = "1" },
            }
        };

        [Fact]
        public void CanWriteIndividualPrimitiveProperty()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"age\":37"
                );

            RunTest((ODataWriter writer) =>
            {
                // write some properties within the resource and others separately
                writer.WriteStart(resource);
                writer.Write(new ODataProperty { Name = "age", Value = 37 });
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWritePrimitivePropertyValue()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"age\":37"
                );

            RunTest((ODataWriter writer) =>
            {
                // write some properties within the resource and others separately
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo { Name = "age"});
                writer.WritePrimitive(new ODataPrimitiveValue(37));
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWritePrimitivePropertyInstanceAnnotationWithoutPropertyValue()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"age@Custom.StartAnnotation\":123"
                );

            RunTest((ODataWriter writer) =>
            {
                ODataPrimitiveValue primitiveValue = new ODataPrimitiveValue(123);
                ODataPropertyInfo propertyInfo = new ODataPropertyInfo { Name = "age" };
                propertyInfo.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", primitiveValue));

                // write some properties within the resource and others separately
                writer.WriteStart(resource);

                writer.WriteStart(propertyInfo);
                writer.WriteEnd(); // property

                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWritePrimitivePropertyInstanceAnnotationWithPropertyValue()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"age@Custom.StartAnnotation\":123,\"age\":37"
                );

            RunTest((ODataWriter writer) =>
            {
                ODataPrimitiveValue primitiveValue = new ODataPrimitiveValue(123);
                ODataPropertyInfo propertyInfo = new ODataPropertyInfo { Name = "age" };
                propertyInfo.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", primitiveValue));

                // write some properties within the resource and others separately
                writer.WriteStart(resource);

                writer.WriteStart(propertyInfo);
                    writer.WritePrimitive(new ODataPrimitiveValue(37));
                writer.WriteEnd(); // property

                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteDynamicPropertyWithInstanceAnnotationsButWithoutPropertyValue()
        {
            string expectedPayload = String.Format(resourcePayload,
                 ",\"aDynamicProperty@Custom.StartAnnotation\":123"
                 );

            RunTest((ODataWriter writer) =>
            {
                ODataPrimitiveValue primitiveValue = new ODataPrimitiveValue(123);
                ODataPropertyInfo propertyInfo = new ODataPropertyInfo { Name = "aDynamicProperty" };
                propertyInfo.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", primitiveValue));

                // write some properties within the resource and others separately
                writer.WriteStart(resource);

                writer.WriteStart(propertyInfo);
                writer.WriteEnd(); // property

                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteBinaryPropertyAsStream()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"binaryAsStream\":\"" + binaryValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.Write(new ODataProperty
                {
                    Name = "binaryAsStream",
                    Value = new ODataBinaryStreamValue(CreateBinaryStreamValue())
                });
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteTextPropertyAsStream()
        {
            string textValue = "My String Value";
            string expectedPayload = String.Format(resourcePayload,
                ",\"textAsStream\":\"" + textValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo
                {
                    Name = "textAsStream",
                });
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    textWriter.Write(textValue);
                    textWriter.Flush();
                };
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteTextPropertyAsValue()
        {
            string textValue = "My String Value";
            string expectedPayload = String.Format(resourcePayload,
                ",\"textAsStream\":\"" + textValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo
                {
                    Name = "textAsStream",
                });
                writer.WritePrimitive(new ODataPrimitiveValue(textValue));
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteIndividualStreamPropertyMetadata()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"stream@mediaEditLink\":\"http://testservice/customers/1/stream\"" +
                ",\"stream@mediaContentType\":\"text/plain\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.Write(new ODataProperty
                {
                    Name = "stream",
                    Value = new ODataStreamReferenceValue
                    {
                        EditLink = new Uri("http://testservice/customers/1/stream", UriKind.Absolute),
                        ContentType = "text/plain"
                    }
                });
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteIndividualStreamPropertyValue()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"stream\":\"" + binaryValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.WriteStart(new ODataStreamPropertyInfo
                {
                    Name = "stream",
                });
                using (Stream binaryStream = writer.CreateBinaryWriteStream())
                {
                    CreateBinaryStreamValue().CopyTo(binaryStream);
                    binaryStream.Flush();
                } // stream must be disposed before continuing
                writer.WriteEnd();  // stream property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteUndeclaredStreamPropertyValue()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"undeclaredStream\":\"" + binaryValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.WriteStart(new ODataStreamPropertyInfo
                {
                    Name = "undeclaredStream",
                });
                using (Stream binaryStream = writer.CreateBinaryWriteStream())
                {
                    CreateBinaryStreamValue().CopyTo(binaryStream);
                    binaryStream.Flush();
                } // stream must be disposed before continuing
                writer.WriteEnd();  // stream property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteIndividualStreamPropertyValueAndMetadata()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"stream@mediaEditLink\":\"http://testservice/customers/1/editStream\"" +
                ",\"stream@mediaReadLink\":\"http://testservice/customers/1/readStream\"" +
                ",\"stream@mediaContentType\":\"text/plain\"" +
                ",\"stream\":\"" + binaryValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.WriteStart(new ODataStreamPropertyInfo
                {
                    Name = "stream",
                    EditLink = new Uri("http://testservice/customers/1/editStream", UriKind.Absolute),
                    ReadLink = new Uri("http://testservice/customers/1/readStream", UriKind.Absolute),
                    ContentType = "text/plain"
                });
                using (Stream binaryStream = writer.CreateBinaryWriteStream())
                {
                    CreateBinaryStreamValue().CopyTo(binaryStream);
                    binaryStream.Flush();
                } // stream must be disposed before continuing
                writer.WriteEnd();  // stream property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteUndeclaredStreamPropertyValueAndMetadata()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"undeclaredStream@mediaEditLink\":\"http://testservice/customers/1/editStream\"" +
                ",\"undeclaredStream@mediaReadLink\":\"http://testservice/customers/1/readStream\"" +
                ",\"undeclaredStream@mediaContentType\":\"text/plain\"" +
                ",\"undeclaredStream\":\"" + binaryValue + "\""
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.WriteStart(new ODataStreamPropertyInfo
                {
                    Name = "undeclaredStream",
                    EditLink = new Uri("http://testservice/customers/1/editStream", UriKind.Absolute),
                    ReadLink = new Uri("http://testservice/customers/1/readStream", UriKind.Absolute),
                    ContentType = "text/plain"
                });
                using (Stream binaryStream = writer.CreateBinaryWriteStream())
                {
                    CreateBinaryStreamValue().CopyTo(binaryStream);
                    binaryStream.Flush();
                } // stream must be disposed before continuing
                writer.WriteEnd();  // stream property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteStreamPropertyAsJson()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"jsonStream@mediaEditLink\":\"http://testservice/customers/1/stream\"" +
                ",\"jsonStream@mediaContentType\":\"application/json\"" +
                ",\"jsonStream\":{\"stringProp\":\"string\",\"numProp\":-10.5,\"boolProp\":true,\"arrayProp\":[\"value1\",-10.5,false]}"
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.WriteStart(new ODataStreamPropertyInfo
                {
                    Name = "jsonStream",
                    EditLink = new Uri("http://testservice/customers/1/stream", UriKind.Absolute),
                    ContentType = "application/json"
                });
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    using (Microsoft.OData.Json.JsonWriter jsonWriter = new Microsoft.OData.Json.JsonWriter(textWriter, false))
                    {
                        jsonWriter.StartObjectScope();
                        jsonWriter.WriteName("stringProp");
                        jsonWriter.WriteValue("string");
                        jsonWriter.WriteName("numProp");
                        jsonWriter.WriteValue(-10.5);
                        jsonWriter.WriteName("boolProp");
                        jsonWriter.WriteValue(true);
                        jsonWriter.WriteName("arrayProp");
                        jsonWriter.StartArrayScope();
                        jsonWriter.WriteValue("value1");
                        jsonWriter.WriteValue(-10.5);
                        jsonWriter.WriteValue(false);
                        jsonWriter.EndArrayScope();
                        jsonWriter.EndObjectScope();
                        jsonWriter.Flush();
                    }
                } // stream must be disposed before continuing
                writer.WriteEnd();  // json stream property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteStreamPropertyAsJsonArray()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"jsonStream@mediaEditLink\":\"http://testservice/customers/1/stream\"" +
                ",\"jsonStream@mediaContentType\":\"application/json\"" +
                ",\"jsonStream\":[{\"stringProp\":\"string\",\"numProp\":-10.5,\"boolProp\":true,\"arrayProp\":[\"value1\",-10.5,false]}]"
                );

            RunTest((ODataWriter writer) =>
            {
                // write metadata for stream property
                writer.WriteStart(resource);
                writer.WriteStart(new ODataStreamPropertyInfo
                {
                    Name = "jsonStream",
                    EditLink = new Uri("http://testservice/customers/1/stream", UriKind.Absolute),
                    ContentType = "application/json"
                });
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    using (Microsoft.OData.Json.JsonWriter jsonWriter = new Microsoft.OData.Json.JsonWriter(textWriter, false))
                    {
                        jsonWriter.StartArrayScope();
                        jsonWriter.StartObjectScope();
                        jsonWriter.WriteName("stringProp");
                        jsonWriter.WriteValue("string");
                        jsonWriter.WriteName("numProp");
                        jsonWriter.WriteValue(-10.5);
                        jsonWriter.WriteName("boolProp");
                        jsonWriter.WriteValue(true);
                        jsonWriter.WriteName("arrayProp");
                        jsonWriter.StartArrayScope();
                        jsonWriter.WriteValue("value1");
                        jsonWriter.WriteValue(-10.5);
                        jsonWriter.WriteValue(false);
                        jsonWriter.EndArrayScope();
                        jsonWriter.EndObjectScope();
                        jsonWriter.EndArrayScope();
                        jsonWriter.Flush();
                    }
                } // stream must be disposed before continuing
                writer.WriteEnd();  // json stream property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteCollectionOfPrimitives()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"comments\":[\"one\",\"two\",null]"
                );

            RunTest((ODataWriter writer) =>
            {
                // write individual elements within a collection
                writer.WriteStart(resource);
                writer.WriteStart(
                     new ODataNestedResourceInfo { Name = "comments" });
                writer.WriteStart(new ODataResourceSet());
                writer.WritePrimitive(new ODataPrimitiveValue("one"));
                writer.WritePrimitive(new ODataPrimitiveValue("two"));
                writer.WritePrimitive(null);
                writer.WriteEnd(); // collection
                writer.WriteEnd(); // nested info
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteDynamicCollectionOfStreams()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"streamCollection@type\":\"Collection(Stream)\"" +
                ",\"streamCollection\":[\"" + binaryValue + "\",\"" + binaryValue + "\"]"
                );

            RunTest((ODataWriter writer) =>
            {
                // write individual elements in a collection writing to a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataNestedResourceInfo { Name = "streamCollection" });
                writer.WriteStart(new ODataResourceSet { TypeName = "Collection(Edm.Stream)" });
                writer.WriteStream(new ODataBinaryStreamValue(CreateBinaryStreamValue()));
                writer.WriteStream(new ODataBinaryStreamValue(CreateBinaryStreamValue()));
                writer.WriteEnd(); // collection
                writer.WriteEnd(); // nestedinfo
                writer.WriteEnd(); //resource

            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteDynamicCollectionOfBinaryAsStream()
        {
            string expectedPayload = String.Format(resourcePayload,
                ",\"binaryCollection@type\":\"Collection(Binary)\"" +
                ",\"binaryCollection\":[\"" + binaryValue + "\",\"" + binaryValue + "\"]"
                );

            RunTest((ODataWriter writer) =>
            {
                // write individual elements in a binary collection
                writer.WriteStart(resource);
                writer.WriteStart(new ODataNestedResourceInfo { Name = "binaryCollection" });
                writer.WriteStart(new ODataResourceSet{TypeName = "Collection(Edm.Binary)"});
                // write a binary value
                writer.WriteStream(new ODataBinaryStreamValue(CreateBinaryStreamValue()));
                writer.WriteStream(new ODataBinaryStreamValue(CreateBinaryStreamValue()));
                writer.WriteEnd(); // collection
                writer.WriteEnd(); // nested property

                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteCollectionOfTextAsStream()
        {
            string textString = "My String Value";
            string expectedPayload = String.Format(resourcePayload,
                ",\"comments\":[\"" + textString + "\",\"" + textString + "\"]"
                );

            RunTest((ODataWriter writer) =>
            {
                // write individual elements in a text collection
                writer.WriteStart(resource);
                writer.WriteStart(new ODataNestedResourceInfo { Name = "comments" });
                writer.WriteStart(new ODataResourceSet());
                // write text values
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    textWriter.Write(textString);
                    textWriter.Flush();
                }
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    textWriter.Write(textString);
                    textWriter.Flush();
                }
                writer.WriteEnd(); // collection
                writer.WriteEnd(); // nested property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CanWriteUntypedCollection()
        {
            string textString = "My String Value";
            string expectedPayload = String.Format(resourcePayload,
//                ",\"untypedCollection@type\":\"Collection(Untyped)\"" +
                ",\"untypedCollection\":[" +
                "\"" + binaryValue + "\"" +
                ",\"" + textString + "\"" +
                ",\"" + textString + "\"" +
                ",null" +
                ",-10.5]"
                );

            RunTest((ODataWriter writer) =>
            {
                // write individual elements in a text collection
                writer.WriteStart(resource);
                writer.WriteStart(new ODataNestedResourceInfo { Name = "untypedCollection", IsCollection = true });
                writer.WriteStart(new ODataResourceSet { TypeName = "Collection(Edm.Untyped)" });
                writer.WriteStream(new ODataBinaryStreamValue(CreateBinaryStreamValue()));
                writer.WritePrimitive(new ODataPrimitiveValue(textString));
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    textWriter.Write(textString);
                    textWriter.Flush();
                }
                writer.WritePrimitive(null);
                writer.WritePrimitive(new ODataPrimitiveValue(-10.5));
                writer.WriteEnd(); // collection
                writer.WriteEnd(); // nested property
                writer.WriteEnd(); // resource
            },
            expectedPayload);
        }

        [Fact]
        public void CannotWriteValueForODataProperty()
        {
            Action writeWithExtraValue = () => RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataProperty
                {
                    Name = "textAsStream",
                    Value = new ODataPrimitiveValue("text")
                });
                writer.WritePrimitive(new ODataPrimitiveValue("text"));
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            null);

            writeWithExtraValue.Throws<ODataException>(Strings.ODataWriterCore_PropertyValueAlreadyWritten("textAsStream"));
        }

        [Fact]
        public void CannotStreamValueForODataProperty()
        {
            Action writeWithExtraValue = () => RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataProperty
                {
                    Name = "textAsStream",
                    Value = new ODataPrimitiveValue("text")
                });
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    textWriter.Write("text");
                    textWriter.Flush();
                };
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            null);

            writeWithExtraValue.Throws<ODataException>(Strings.ODataWriterCore_PropertyValueAlreadyWritten("textAsStream"));
        }

        [Fact]
        public void CannotWriteValueTwice()
        {
            Action writeWithExtraValue = () => RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo
                {
                    Name = "textAsStream"
                });
                writer.WritePrimitive(new ODataPrimitiveValue("text"));
                using (TextWriter textWriter = writer.CreateTextWriter())
                {
                    textWriter.Write("text");
                    textWriter.Flush();
                };
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            null);

            writeWithExtraValue.Throws<ODataException>(Strings.ODataWriterCore_PropertyValueAlreadyWritten("textAsStream"));
        }

        [Fact]
        public void CannotWriteAndStreamProperty()
        {
            Action writeWithExtraValue = () => RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo
                {
                    Name = "textAsStream"
                });
                writer.WritePrimitive(new ODataPrimitiveValue("text"));
                writer.WriteEnd(); // property
                writer.WriteStart(new ODataProperty
                {
                    Name = "textAsStream",
                    Value = new ODataPrimitiveValue("text")
                });
                writer.WriteEnd(); // resource
            },
            null);

            writeWithExtraValue.Throws<ODataException>(Strings.DuplicatePropertyNamesNotAllowed("textAsStream"));
        }

        [Fact]
        public void CannotStreamAndWriteProperty()
        {
            Action writeWithExtraValue = () => RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo
                {
                    Name = "textAsStream"
                });
                writer.WritePrimitive(new ODataPrimitiveValue("text"));
                writer.WriteEnd(); // property
                writer.WriteStart(new ODataProperty
                {
                    Name = "textAsStream",
                    Value = new ODataPrimitiveValue("text")
                });
                writer.WriteEnd(); // resource
            },
            null);

            writeWithExtraValue.Throws<ODataException>(Strings.DuplicatePropertyNamesNotAllowed("textAsStream"));
        }

        [Fact]
        public void CannotWritePrimitiveValueTwice()
        {
            Action writeWithExtraValue = () => RunTest((ODataWriter writer) =>
            {
                // write a binary property using a stream
                writer.WriteStart(resource);
                writer.WriteStart(new ODataPropertyInfo
                {
                    Name = "textAsStream"
                });
                writer.WritePrimitive(new ODataPrimitiveValue("text"));
                writer.WritePrimitive(new ODataPrimitiveValue("text"));
                writer.WriteEnd(); // property
                writer.WriteEnd(); // resource
            },
            null);

            writeWithExtraValue.Throws<ODataException>(Strings.ODataWriterCore_PropertyValueAlreadyWritten("textAsStream"));
        }
        #region Test Helper Methods

        private IEdmModel GetModel()
        {
            if (this.model == null)
            {
                var model = new EdmModel();

                var customerType = new EdmEntityType("test", "customer", null, false, true);
                customerType.AddKeys(customerType.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false));
                customerType.AddStructuralProperty("name", EdmPrimitiveTypeKind.String);
                customerType.AddStructuralProperty("age", EdmPrimitiveTypeKind.Int32, false);
                customerType.AddStructuralProperty("stream", EdmPrimitiveTypeKind.Stream);
                customerType.AddStructuralProperty("binaryAsStream", EdmPrimitiveTypeKind.Binary);
                customerType.AddStructuralProperty("textAsStream", EdmPrimitiveTypeKind.String);
                customerType.AddStructuralProperty("comments", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
                model.AddElement(customerType);

                var container = model.AddEntityContainer("test", "container");
                this.customersEntitySet = container.AddEntitySet("customers", customerType);

                this.model = model;
            }

            return this.model;
        }

        private void RunTest(Action<ODataWriter> test, string expectedPayload)
        {
            Stream stream = new MemoryStream();
            ODataWriter writer = CreateODataWriter(stream);
            test(writer);
            string payload = ReadPayload(stream);
            Assert.Equal(expectedPayload, payload);
        }

        private string ReadPayload(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }

        private MemoryStream CreateBinaryStreamValue()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write("binaryStream");
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private ODataWriter CreateODataWriter(Stream stream)
        {
            return CreateODataWriter(stream, ODataVersion.V401, false, false);
        }

        private ODataWriter CreateODataWriter(Stream stream, ODataVersion version, bool isRequest, bool fullMetadata)
        {
            var settings = new ODataMessageWriterSettings
            {
                Version = version,
                ODataUri = new ODataUri
                {
                    ServiceRoot = new Uri("http://testService"),
                    RequestUri = new Uri("http://testService/customers")
                },
            };
            settings.ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*");

            ODataMessageWriter messageWriter;

            if (isRequest)
            {
                IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
                if (fullMetadata)
                {
                    requestMessage.SetHeader("Content-Type", "application/json;odata.metadata=full");
                }
               messageWriter = new ODataMessageWriter(requestMessage, settings, this.GetModel());
            }
            else
            {
                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = stream };
                if (fullMetadata)
                {
                    responseMessage.SetHeader("Content-Type", "application/json;odata.metadata=full");
                }
                messageWriter =  new ODataMessageWriter(responseMessage, settings, this.GetModel());
            }

            ODataWriter writer = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customersEntitySet.EntityType);
            return writer;
        }

        #endregion
    }
}
