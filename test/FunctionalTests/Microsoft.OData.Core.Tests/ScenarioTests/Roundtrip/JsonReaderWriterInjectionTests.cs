//---------------------------------------------------------------------
// <copyright file="JsonReaderWriterInjectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.Test.OData.DependencyInjection;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip
{
    public class JsonReaderWriterInjectionTests
    {
        [Fact]
        public void UseDefaultJsonWriter()
        {
            RunWriterTest(null, "{\"@odata.context\":\"http://test/$metadata#People/$entity\",\"PersonId\":999,\"Name\":\"Jack\"}");
        }

        [Fact]
        public void InjectCustomJsonWriter()
        {
            RunWriterTest(builder => builder.AddService<IJsonWriterFactory, TestJsonWriterFactory>(ServiceLifetime.Transient),
                "<\"@context\":\"http://test/$metadata#People/$entity\",\"PersonId\":999,\"Name\":\"Jack\",>");
        }

        [Fact]
        public void UseDefaultJsonReader()
        {
            RunReaderTest(null, "{\"@odata.context\":\"http://test/$metadata#People/$entity\",\"PersonId\":999,\"Name\":\"Jack\"}");
        }

        [Fact]
        public void InjectCustomJsonReader()
        {
            RunReaderTest(builder => builder.AddService<IJsonReaderFactory, TestJsonReaderFactory>(ServiceLifetime.Transient),
                "<\"@context\":\"http://test/$metadata#People/$entity\",\"PersonId\":999,\"Name\":\"Jack\",>");
        }

        private static void RunWriterTest(Action<IContainerBuilder> action, string expectedOutput)
        {
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "PersonId", Value = 999 },
                    new ODataProperty { Name = "Name", Value = "Jack" }
                }
            };
            var model = BuildModel();
            var entitySet = model.FindDeclaredEntitySet("People");
            var entityType = model.GetEntityType("NS.Person");
            var container = ContainerBuilderHelper.BuildContainer(action);
            var output = GetWriterOutput(resource, model, entitySet, entityType, container);
            Assert.Equal(expectedOutput, output);
        }

        private static void RunReaderTest(Action<IContainerBuilder> action, string messageContent)
        {
            var model = BuildModel();
            var entitySet = model.FindDeclaredEntitySet("People");
            var entityType = model.GetEntityType("NS.Person");
            var container = ContainerBuilderHelper.BuildContainer(action);
            container.GetRequiredService<ODataSimplifiedOptions>().EnableReadingODataAnnotationWithoutPrefix = true;
            var resource = GetReadedResource(messageContent, model, entitySet, entityType, container);
            var propertyList = resource.Properties.ToList();
            Assert.Equal("PersonId", propertyList[0].Name);
            Assert.Equal(999, propertyList[0].Value);
            Assert.Equal("Name", propertyList[1].Name);
            Assert.Equal("Jack", propertyList[1].Value);
        }

        private static EdmModel BuildModel()
        {
            var model = new EdmModel();

            var personType = new EdmEntityType("NS", "Person");
            personType.AddKeys(personType.AddStructuralProperty("PersonId", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            model.AddElement(personType);

            var container = new EdmEntityContainer("NS", "Container");
            container.AddEntitySet("People", personType);
            model.AddElement(container);

            return model;
        }

        private static string GetWriterOutput(ODataResource entry, EdmModel model, IEdmEntitySetBase entitySet, EdmEntityType entityType, IServiceProvider container)
        {
            var outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage
            {
                Stream = outputStream,
                Container = container
            };
            message.SetHeader("Content-Type", "application/json");
            var settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri("http://test"));

            using (var messageWriter = new ODataMessageWriter(message, settings, model))
            {
                var writer = messageWriter.CreateODataResourceWriter(entitySet, entityType);
                writer.WriteStart(entry);
                writer.WriteEnd();
                outputStream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(outputStream).ReadToEnd();
            }
        }

        private static ODataResource GetReadedResource(string messageContent, EdmModel model, IEdmEntitySetBase entitySet, EdmEntityType entityType, IServiceProvider container)
        {
            var outputStream = new MemoryStream();
            var writer = new StreamWriter(outputStream);
            writer.Write(messageContent);
            writer.Flush();
            outputStream.Seek(0, SeekOrigin.Begin);

            IODataResponseMessage message = new InMemoryMessage
            {
                Stream = outputStream,
                Container = container
            };
            message.SetHeader("Content-Type", "application/json");

            var settings = new ODataMessageReaderSettings();
            using (var messageReader = new ODataMessageReader(message, settings, model))
            {
                var reader = messageReader.CreateODataResourceReader(entitySet, entityType);
                Assert.True(reader.Read());
                return reader.Item as ODataResource;
            }
        }

        private class TestJsonWriterFactory : IJsonWriterFactory
        {
            public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
            {
                return new TestJsonWriter(textWriter);
            }
        }

        private class TestJsonReaderFactory : IJsonReaderFactory
        {
            public IJsonReader CreateJsonReader(TextReader textReader, bool isIeee754Compatible)
            {
                return new TestJsonReader();
            }
        }

        private class TestJsonWriter : IJsonWriter
        {
            private readonly TextWriter textWriter;

            public TestJsonWriter(TextWriter textWriter)
            {
                this.textWriter = textWriter;
            }

            public void StartPaddingFunctionScope()
            {
                throw new NotImplementedException();
            }

            public void EndPaddingFunctionScope()
            {
                throw new NotImplementedException();
            }

            public void StartObjectScope()
            {
                this.textWriter.Write('<');
            }

            public void EndObjectScope()
            {
                this.textWriter.Write('>');
            }

            public void StartArrayScope()
            {
                throw new NotImplementedException();
            }

            public void EndArrayScope()
            {
                throw new NotImplementedException();
            }

            public void WriteName(string name)
            {
                if (name.StartsWith("@odata."))
                {
                    name = '@' + name.Substring(7);
                }

                this.textWriter.Write("\"{0}\":", name);
            }

            public void WritePaddingFunctionName(string functionName)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(bool value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(int value)
            {
                this.textWriter.Write("{0},", value);
            }

            public void WriteValue(float value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(short value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(long value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(double value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(Guid value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(decimal value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(DateTimeOffset value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(TimeSpan value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(byte value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(sbyte value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(string value)
            {
                this.textWriter.Write("\"{0}\",", value);
            }

            public void WriteValue(byte[] value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(Date value)
            {
                throw new NotImplementedException();
            }

            public void WriteValue(TimeOfDay value)
            {
                throw new NotImplementedException();
            }

            public void WriteRawValue(string rawValue)
            {
                throw new NotImplementedException();
            }

            public void Flush()
            {
                this.textWriter.Flush();
            }
        }

        private class TestJsonReader : IJsonReader
        {
            private int callCount;
            private object value;
            private JsonNodeType nodeType = JsonNodeType.None;

            public object Value
            {
                get { return this.value; }
            }

            public JsonNodeType NodeType
            {
                get { return this.nodeType; }
            }

            public bool IsIeee754Compatible
            {
                get { return true; }
            }

            public bool Read()
            {
                switch (callCount)
                {
                    case 0:
                        value = null;
                        nodeType = JsonNodeType.StartObject;
                        break;
                    case 1:
                        value = "@context";
                        nodeType = JsonNodeType.Property;
                        break;
                    case 2:
                        value = "http://test/$metadata#People/$entity";
                        nodeType = JsonNodeType.PrimitiveValue;
                        break;
                    case 3:
                        value = "PersonId";
                        nodeType = JsonNodeType.Property;
                        break;
                    case 4:
                        value = 999;
                        nodeType = JsonNodeType.PrimitiveValue;
                        break;
                    case 5:
                        value = "Name";
                        nodeType = JsonNodeType.Property;
                        break;
                    case 6:
                        value = "Jack";
                        nodeType = JsonNodeType.PrimitiveValue;
                        break;
                    case 7:
                        value = null;
                        nodeType = JsonNodeType.EndObject;
                        break;
                    case 8:
                        value = null;
                        nodeType = JsonNodeType.EndOfInput;
                        return false;
                    default:
                        return false;
                }

                ++callCount;
                return true;
            }
        }
    }
}
