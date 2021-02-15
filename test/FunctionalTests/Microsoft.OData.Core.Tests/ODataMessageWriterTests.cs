//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for ODataMessageWriter.
    /// TODO: These unit tests do not provide complete coverage of ODataMessageWriter.
    /// </summary>
    public class ODataMessageWriterTests
    {
        [Fact]
        public void ConstructorWithRequestMessageAndJsonPaddingSettingEnabledFails()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { JsonPCallback = "functionName" };
            Action constructorCall = () => new ODataMessageWriter(new DummyRequestMessage(), settings);
            constructorCall.Throws<ODataException>(Strings.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage);
        }

        [Fact]
        public void CreateCollectionWriterWithoutTypeShouldPassForJsonLight()
        {
            var settings = new ODataMessageWriterSettings();
            settings.SetContentType(ODataFormat.Json);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, new EdmModel());
            Assert.IsType<ODataJsonLightCollectionWriter>(writer.CreateODataCollectionWriter(null));
        }

        [Fact]
        public void CreateCollectionWriterWithEntityCollectionTypeShouldFail()
        {
            var writer = new ODataMessageWriter(new DummyRequestMessage());
            var entityElementType = new EdmEntityTypeReference(new EdmEntityType("Fake", "Fake"), true);
            Action createWriterWithEntityCollectionType = () => writer.CreateODataCollectionWriter(entityElementType);
            createWriterWithEntityCollectionType.Throws<ODataException>(Strings.ODataMessageWriter_NonCollectionType("Fake.Fake"));
        }

        [Fact]
        public void CreateCollectionWriterWithEnumAsItemType()
        {
            var writer = new ODataMessageWriter(new DummyRequestMessage());
            var entityElementType = new EdmEnumTypeReference(new EdmEnumType("FakeNS", "FakeEnum"), true);
            var collectionWriter = writer.CreateODataCollectionWriter(entityElementType);
            Assert.True(collectionWriter != null, "CreateODataCollectionWriter with enum item type failed.");
        }

        [Fact]
        public void CreateCollectionWriterWithTypeDefinitionAsItemType()
        {
            var writer = new ODataMessageWriter(new DummyRequestMessage());
            var entityElementType = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), false);
            var collectionWriter = writer.CreateODataCollectionWriter(entityElementType);
            Assert.True(collectionWriter != null, "CreateODataCollectionWriter with type definition item type failed.");
        }

        [Fact]
        public void CreateMessageWriterShouldNotSetAnnotationFilterWhenODataAnnotationsIsNotSetOnPreferenceAppliedHeader()
        {
            ODataMessageWriter writer = new ODataMessageWriter((IODataResponseMessage)new InMemoryMessage(), new ODataMessageWriterSettings());
            Assert.Null(writer.Settings.ShouldIncludeAnnotation);
        }

        [Fact]
        public void CreateMessageWriterShouldSetAnnotationFilterWhenODataAnnotationIsSetOnPreferenceAppliedHeader()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            ODataMessageWriter writer = new ODataMessageWriter(responseMessage, new ODataMessageWriterSettings());
            Assert.NotNull(writer.Settings.ShouldIncludeAnnotation);
        }

        [Fact]
        public void WriteTopLevelUIntPropertyShouldWork()
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri.ServiceRoot = new Uri("http://host/service");
            settings.SetContentType(ODataFormat.Json);
            var model = new EdmModel();
            model.GetUInt32("MyNS", false);
            IODataRequestMessage request = new InMemoryMessage() { Stream = new MemoryStream() };
            var writer = new ODataMessageWriter(request, settings, model);
            Action write = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Id",
                Value = (UInt32)123
            });
            write.DoesNotThrow();
            request.GetStream().Position = 0;
            var reader = new StreamReader(request.GetStream());
            string output = reader.ReadToEnd();
            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#MyNS.UInt32\",\"value\":123}", output);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteTopLevelStringPropertyWithStringEscapeOptionShouldWork(ODataStringEscapeOption? stringEscapeOption)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            var request = new InMemoryMessage() { Stream = new MemoryStream() };

            if (stringEscapeOption != null)
            {
                var containerBuilder = new Test.OData.DependencyInjection.TestContainerBuilder();
                containerBuilder.AddDefaultODataServices();
                containerBuilder.AddService(ServiceLifetime.Singleton, sp => new DefaultJsonWriterFactory(stringEscapeOption.Value));
                request.Container = containerBuilder.BuildContainer();
            }

            settings.ODataUri.ServiceRoot = new Uri("http://host/service");
            settings.SetContentType(ODataFormat.Json);
            var model = new EdmModel();
            var writer = new ODataMessageWriter((IODataRequestMessage)request, settings, model);
            Action write = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "ия"
            });
            write.DoesNotThrow();
            request.GetStream().Position = 0;
            var reader = new StreamReader(request.GetStream());
            string output = reader.ReadToEnd();
            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Edm.String\",\"value\":\"\\u0438\\u044f\"}", output);
        }

        [Fact]
        public void WriteTopLevelStringPropertyWithStringEscapeOnlyControlsOptionShouldWork()
        {
            var settings = new ODataMessageWriterSettings();
            var containerBuilder = new Test.OData.DependencyInjection.TestContainerBuilder();
            containerBuilder.AddDefaultODataServices();
            containerBuilder.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, sp => new DefaultJsonWriterFactory(ODataStringEscapeOption.EscapeOnlyControls));

            settings.ODataUri.ServiceRoot = new Uri("http://host/service");
            settings.SetContentType(ODataFormat.Json);
            var model = new EdmModel();
            IODataRequestMessage request = new InMemoryMessage()
            {
                Stream = new MemoryStream(),
                Container = containerBuilder.BuildContainer()
            };
            var writer = new ODataMessageWriter(request, settings, model);
            Action write = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "ия"
            });
            write.DoesNotThrow();
            request.GetStream().Position = 0;
            var reader = new StreamReader(request.GetStream());
            string output = reader.ReadToEnd();
            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Edm.String\",\"value\":\"ия\"}", output);
        }

        [Fact]
        public void WriteDeclaredUIntValueShouldWork()
        {
            var settings = new ODataMessageWriterSettings();
            var model = new EdmModel();
            model.GetUInt32("MyNS", false);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, model);
            Action write = () => writer.WriteValue((UInt32)123);
            write.DoesNotThrow();
        }

        [Fact]
        public void WriteUndeclaredUIntValueShouldFail()
        {
            var settings = new ODataMessageWriterSettings();
            var model = new EdmModel();
            model.GetUInt32("MyNS", false);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, model);
            Action write = () => writer.WriteValue((UInt16)123);
            write.Throws<ODataException>("The value of type 'System.UInt16' could not be converted to a raw string.");
        }

        [Fact]
        public void WriteMetadataDocument_WorksForJsonCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();

            string contentType = "application/json";

#if NETCOREAPP3_1 || NETCOREAPP2_1
            // Act
            string payload = this.WriteAndGetPayload(edmModel, contentType, omWriter =>
            {
                omWriter.WriteMetadataDocument();
            });

            // Assert
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {}
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    }
  }
}", payload);
#else
            Action test = () => this.WriteAndGetPayload(edmModel, contentType, omWriter =>
            {
                omWriter.WriteMetadataDocument();
            });

            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal("The JSON metadata is not supported at this platform. It's only supported at platform implementing .NETStardard 2.0.", exception.Message);
#endif
        }

#if NETCOREAPP3_1 || NETCOREAPP2_1
        [Fact]
        public async Task WriteMetadataDocumentAsync_WorksForJsonCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();


            // Act - JSON
            string payload = await this.WriteAndGetPayloadAsync(edmModel, "application/json", async omWriter =>
            {
                await omWriter.WriteMetadataDocumentAsync();
            });

            // Assert
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {}
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    }
  }
}", payload);
    }
#endif

        [Fact]
        public async Task WriteMetadataDocumentAsync_WorksForXmlCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();

            // Act - XML
            string payload = await this.WriteAndGetPayloadAsync(edmModel, "application/xml", async omWriter =>
            {
                await omWriter.WriteMetadataDocumentAsync();
            });

            // Assert - XML
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                  "<edmx:DataServices>" +
                    "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                      "<EntityType Name=\"Customer\">" +
                        "<Key>" +
                          "<PropertyRef Name=\"Id\" />" +
                        "</Key>" +
                        "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                        "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "</EntityType>" +
                      "<EntityContainer Name=\"Container\">" +
                        "<EntitySet Name=\"Customers\" EntityType=\"NS.Customer\" />" +
                      "</EntityContainer>" +
                    "</Schema>" +
                  "</edmx:DataServices>" +
                "</edmx:Edmx>", payload);
        }

        private static IEdmModel _edmModel;

        private static IEdmModel GetEdmModel()
        {
            if (_edmModel != null)
            {
                return _edmModel;
            }

            EdmModel edmModel = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            edmModel.AddElement(container);

            EdmEntityType customerType = new EdmEntityType("NS", "Customer");
            var idProperty = new EdmStructuralProperty(customerType, "Id", EdmCoreModel.Instance.GetInt32(false));
            customerType.AddProperty(idProperty);
            customerType.AddKeys(new IEdmStructuralProperty[] { idProperty });
            customerType.AddProperty(new EdmStructuralProperty(customerType, "Name", EdmCoreModel.Instance.GetString(false)));
            edmModel.AddElement(customerType);
            container.AddEntitySet("Customers", customerType);

            _edmModel = edmModel;

            return edmModel;
        }

        private string WriteAndGetPayload(IEdmModel edmModel, string contentType, Action<ODataMessageWriter> test)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream() };
            if (contentType != null)
            {
                message.SetHeader("Content-Type", contentType);
            }

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            using (var msgWriter = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, edmModel))
            {
                test(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task<string> WriteAndGetPayloadAsync(IEdmModel edmModel, string contentType, Func<ODataMessageWriter, Task> test)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream() };
            if (contentType != null)
            {
                message.SetHeader("Content-Type", contentType);
            }

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel))
            {
                await test(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
