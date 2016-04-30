//---------------------------------------------------------------------
// <copyright file="JsonWriterInjectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Writer.JsonLight
{
    public class JsonWriterInjectionTests
    {
        [Fact]
        public void UseDefaultJsonWriter()
        {
            RunTest(null, "{\"@odata.context\":\"http://test/$metadata#People/$entity\",\"PersonId\":999,\"Name\":\"Jack\"}");
        }

        [Fact]
        public void InjectCustomJsonWriter()
        {
            RunTest(builder => builder.AddService<IJsonWriterFactory, TestJsonWriterFactory>(ServiceLifetime.Transient),
                "<\"@context\":\"http://test/$metadata#People/$entity\",\"PersonId\":999,\"Name\":\"Jack\",>");
        }

        private static void RunTest(Action<IContainerBuilder> action, string expectedOutput)
        {
            var entry = new ODataResource
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
            var container = BuildContainer(action);
            var output = GetWriterOutput(entry, model, entitySet, entityType, container);
            Assert.Equal(expectedOutput, output);
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

        private static IServiceProvider BuildContainer(Action<IContainerBuilder> action)
        {
            IContainerBuilder builder = new TestContainerBuilder();

            builder.AddDefaultODataServices();

            if (action != null)
            {
                action(builder);
            }

            return builder.BuildContainer();
        }

        private static string GetWriterOutput(ODataResource entry, EdmModel model, IEdmEntitySetBase entitySet, EdmEntityType entityType, IServiceProvider container)
        {
            var outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage
            {
                Stream = outputStream,
                Container = container
            };
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
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

        private class TestJsonWriterFactory : IJsonWriterFactory
        {
            public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool indent, ODataFormat jsonFormat, bool isIeee754Compatible)
            {
                return new TestJsonWriter(textWriter);
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
    }
}
