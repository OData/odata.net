//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.Json;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm.Csdl;
using Xunit;

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
        public void CreateCollectionWriterWithoutTypeShouldPassForJson()
        {
            var settings = new ODataMessageWriterSettings();
            settings.SetContentType(ODataFormat.Json);
            var writer = new ODataMessageWriter(new DummyRequestMessage(), settings, new EdmModel());
            Assert.IsType<ODataJsonCollectionWriter>(writer.CreateODataCollectionWriter(null));
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
            Assert.Null(writer.Settings.ShouldIncludeAnnotationInternal);
        }

        [Fact]
        public void CreateMessageWriterShouldSetAnnotationFilterWhenODataAnnotationIsSetOnPreferenceAppliedHeader()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            ODataMessageWriter writer = new ODataMessageWriter(responseMessage, new ODataMessageWriterSettings());
            Assert.NotNull(writer.Settings.ShouldIncludeAnnotationInternal);
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
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls, "\"ия\"")]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii, "\"\\u0438\\u044f\"")]
        public void WriteTopLevelStringPropertyWithODataJsonWriterFactoryStringEscapeOptionShouldWork(ODataStringEscapeOption stringEscapeOption, string expectedValue)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            var request = new InMemoryMessage() { Stream = new MemoryStream() };

            IServiceProvider container = CreateTestServiceContainer(services =>
            {
                services.AddSingleton<IJsonWriterFactory>(sp => new ODataJsonWriterFactory(stringEscapeOption));
            });

            request.ServiceProvider = container;

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
            Assert.Equal($"{{\"@odata.context\":\"http://host/service/$metadata#Edm.String\",\"value\":{expectedValue}}}", output);
        }

        [Fact]
        public void WriteTopLevelStringPropertyWithStringEscapeOnlyControlsOptionShouldWork()
        {
            var settings = new ODataMessageWriterSettings();
            IServiceCollection services = new ServiceCollection().AddDefaultODataServices();
            services.AddSingleton<IJsonWriterFactory>(sp => new ODataJsonWriterFactory(ODataStringEscapeOption.EscapeOnlyControls));

            settings.ODataUri.ServiceRoot = new Uri("http://host/service");
            settings.SetContentType(ODataFormat.Json);
            var model = new EdmModel();
            IODataRequestMessage request = new InMemoryMessage()
            {
                Stream = new MemoryStream(),
                ServiceProvider = services.BuildServiceProvider()
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

        #region "ODataUtf8JsonWriter support"
        [Fact]
        public void SupportsODataUtf8JsonWriter()
        {
            using MemoryStream stream = new MemoryStream();
            InMemoryMessage request = new InMemoryMessage() { Stream = stream };

            EdmModel model = new EdmModel();
            Action<ODataMessageWriter> writePropertyAction = (writer) => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            string output = WriteAndGetPayload(
                model,
                "application/json",
                writePropertyAction,
                message: request,
                configureServices: (containerBuilder) =>
                {
                    containerBuilder.AddDefaultODataServices();
                    containerBuilder.AddSingleton<IJsonWriterFactory>(sp => ODataUtf8JsonWriterFactory.Default);
                });

            IJsonWriterFactory factory = request.ServiceProvider.GetService<IJsonWriterFactory>();
            Assert.IsType<ODataUtf8JsonWriterFactory>(factory);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test ия\"}", output);
        }

        [Theory]
        [InlineData("utf-8")]
        [InlineData("utf-16")]
        [InlineData("utf-16BE")]
        [InlineData("utf-32")]
        public void WhenInjectingODataUtf8JsonWriterFactory_CreatesWriterUsingConfiguredEncoding(string encodingCharset)
        {
            // Arrange
            MockJsonWriterFactoryWrapper writerFactory =
                new MockJsonWriterFactoryWrapper(ODataUtf8JsonWriterFactory.Default);
            EdmModel model = new EdmModel();

            // Act
            Action<ODataMessageWriter> writePropertyAction = (writer) => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            string output = WriteAndGetPayload(
                model,
                $"application/json; charset={encodingCharset}",
                writePropertyAction,
                encoding: Encoding.GetEncoding(encodingCharset),
                configureServices: (containerBuilder) =>
                {
                    containerBuilder.AddSingleton<IJsonWriterFactory>(sp => writerFactory);
                });

            // Assert
            Assert.IsType<ODataUtf8JsonWriter>(writerFactory.CreatedWriter);
            Assert.Equal(encodingCharset, writerFactory.Encoding.WebName);
            Assert.Equal(1, writerFactory.NumCalls);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test ия\"}", output);
        }

        [Fact]
        public void WhenInjectingODataUtf8JsonWriterFactory_ThrowException_IfFactoryReturnsNull()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            using MemoryStream stream = new MemoryStream();
            InMemoryMessage request = new InMemoryMessage() { Stream = stream };

            IServiceProvider container = CreateTestServiceContainer(containerBuilder =>
            {
                containerBuilder.AddSingleton<IJsonWriterFactory>(new MockJsonWriterFactory(null));
            });

            request.ServiceProvider = container;

            IJsonWriterFactory factory = request.ServiceProvider.GetService<IJsonWriterFactory>();
            Assert.IsType<MockJsonWriterFactory>(factory);

            settings.ODataUri.ServiceRoot = new Uri("http://www.example.com");
            settings.SetContentType(ODataFormat.Json);
            EdmModel model = new EdmModel();
            using ODataMessageWriter writer = new ODataMessageWriter((IODataRequestMessage)request, settings, model);
            Action writePropertyAction = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            writePropertyAction.Throws<ODataException>(Strings.ODataMessageWriter_JsonWriterFactory_ReturnedNull(false, Encoding.UTF8.WebName));
        }

        [Fact]
        public async Task SupportsODataUtf8JsonWriterAsync()
        {
            EdmModel model = new EdmModel();
            string output = await WriteAndGetPayloadAsync(
                model,
                "application/json",
                (writer) => writer.WritePropertyAsync(new ODataProperty()
                {
                    Name = "Name",
                    Value = "This is a test ия"
                }),
                configureServices: (containerBuilder) =>
                {
                    containerBuilder.AddSingleton<IJsonWriterFactory>( sp => ODataUtf8JsonWriterFactory.Default);
                });

            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test ия\"}", output);
        }

        [Theory]
        [InlineData("utf-8")]
        [InlineData("utf-16")]
        [InlineData("utf-16BE")]
        [InlineData("utf-32")]
        public async Task WhenInjectingODataUtf8JsonWriterFactoryAsync_CreatesWriterUsingConfiguredEncoding(string encodingCharset)
        {
            // Arrange
            MockJsonWriterFactoryWrapper writerFactory =
                new MockJsonWriterFactoryWrapper(ODataUtf8JsonWriterFactory.Default);
            EdmModel model = new EdmModel();

            // Act
            Func<ODataMessageWriter, Task> writePropertyAsync = (writer) => writer.WritePropertyAsync(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            string output = await WriteAndGetPayloadAsync(
                model,
                $"application/json; charset={encodingCharset}",
                writePropertyAsync,
                encoding: Encoding.GetEncoding(encodingCharset),
                configureServices: (containerBuilder) =>
                {
                    containerBuilder.AddSingleton<IJsonWriterFactory>(sp => writerFactory);
                });

            // Assert
            Assert.IsType<ODataUtf8JsonWriter>(writerFactory.CreatedWriter);
            Assert.Equal(encodingCharset, writerFactory.Encoding.WebName);
            Assert.Equal(1, writerFactory.NumCalls);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test ия\"}", output);
        }

        #endregion "ODataUtf8JsonWriter support"

        #region "ODataJsonElementValue support"
        [Fact]
        public void WriteEntityWithJsonElementValues()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType personType = model.AddEntityType("ns", "Person", baseType: null, isAbstract: false, isOpen: true);
            personType.AddKeys(
                personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty(
                "Emails",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
            EdmComplexType addressType = model.AddComplexType("ns", "Address");
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            IEdmEntitySet peopleSet = model.AddEntityContainer("ns", "Service").AddEntitySet("People", personType);


            string source = "{" +
                @"""Id"": 1," +
                @"""Name"": ""John""," +
                @"""Emails"":[""john@mailer.com"",""john@work.com""]," +
                @"""Address"":{""City"":""Nairobi"",""Country"":""Kenya""}" +
                @"}";
            var json = JsonDocument.Parse(source);
            var jsonRoot = json.RootElement;
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Id")) },
                    new ODataProperty { Name = "Name", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Name")) },
                    new ODataProperty { Name = "Emails", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Emails")) },
                    new ODataProperty { Name = "Address", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Address")) }
                }
            };

            // Act
            string result = WriteAndGetPayload(
                model,
                "application/json; charset=utf-8",
                writer =>
                {
                    var resourceWriter = writer.CreateODataResourceWriter(peopleSet);
                    resourceWriter.WriteStart(resource);
                    resourceWriter.WriteEnd();
                },
                path: new ODataPath(new EntitySetSegment(peopleSet)));

            // Assert
            string expected = @"{""@odata.context"":""http://www.example.com/$metadata#People/$entity"",""Id"":1,""Name"":""John"",""Emails"":[""john@mailer.com"",""john@work.com""],""Address"":{""City"":""Nairobi"",""Country"":""Kenya""}}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteEntityWithJsonElementValues_Async()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType personType = model.AddEntityType("ns", "Person", baseType: null, isAbstract: false, isOpen: true);
            personType.AddKeys(
                personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty(
                "Emails",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
            EdmComplexType addressType = model.AddComplexType("ns", "Address");
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            IEdmEntitySet peopleSet = model.AddEntityContainer("ns", "Service").AddEntitySet("People", personType);

            string source = "{" +
                @"""Id"": 1," +
                @"""Name"": ""John""," +
                @"""Emails"":[""john@mailer.com"",""john@work.com""]," +
                @"""Address"":{""City"":""Nairobi"",""Country"":""Kenya""}" +
                @"}";
            var json = JsonDocument.Parse(source);
            var jsonRoot = json.RootElement;
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Id")) },
                    new ODataProperty { Name = "Name", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Name")) },
                    new ODataProperty { Name = "Emails", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Emails")) },
                    new ODataProperty { Name = "Address", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Address")) }
                }
            };

            // Act
            string result = await WriteAndGetPayloadAsync(
                model,
                "application/json; charset=utf-8",
                async writer =>
                {
                    var resourceWriter = await writer.CreateODataResourceWriterAsync(peopleSet);
                    await resourceWriter.WriteStartAsync(resource);
                    await resourceWriter.WriteEndAsync();
                },
                path: new ODataPath(new EntitySetSegment(peopleSet)));

            // Assert
            string expected = @"{""@odata.context"":""http://www.example.com/$metadata#People/$entity"",""Id"":1,""Name"":""John"",""Emails"":[""john@mailer.com"",""john@work.com""],""Address"":{""City"":""Nairobi"",""Country"":""Kenya""}}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WriteContainedNestedEntityWithJsonElementValues()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType personType = model.AddEntityType("ns", "Person");
            IEdmStructuralProperty idProperty = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            personType.AddKeys(idProperty);
            personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Friend",
                ContainsTarget = true,
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            IEdmEntitySet peopleSet = model.AddEntityContainer("ns", "Service").AddEntitySet("People", personType);
            
            string source = @"{""Id"": 1,""Name"": ""John""}";
            var json = JsonDocument.Parse(source);
            var jsonRoot = json.RootElement;
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Id")) },
                    new ODataProperty { Name = "Name", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Name")) }
                }
            };

            var nestedResource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Id")) },
                    new ODataProperty { Name = "Name", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Name")) }
                }
            };

            // Act
            string result = WriteAndGetPayload(
                model,
                "application/json; charset=utf-8",
                writer =>
                {
                    var resourceWriter = writer.CreateODataResourceWriter(peopleSet);
                    resourceWriter.WriteStart(resource);

                    resourceWriter.WriteStart(new ODataNestedResourceInfo
                    {
                        Name = "Friend",
                        IsCollection = false
                    });
                    resourceWriter.WriteStart(nestedResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();

                    resourceWriter.WriteEnd();
                },
                path: new ODataPath(new EntitySetSegment(peopleSet)));

            // Result
            string expected = @"{""@odata.context"":""http://www.example.com/$metadata#People/$entity"",""Id"":1,""Name"":""John"",""Friend"":{""Id"":1,""Name"":""John""}}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteContainedNestedEntityWithJsonElementValues_Async()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType personType = model.AddEntityType("ns", "Person");
            IEdmStructuralProperty idProperty = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            personType.AddKeys(idProperty);
            personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Friend",
                ContainsTarget = true,
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            IEdmEntitySet peopleSet = model.AddEntityContainer("ns", "Service").AddEntitySet("People", personType);

            string source = @"{""Id"": 1,""Name"": ""John""}";
            var json = JsonDocument.Parse(source);
            var jsonRoot = json.RootElement;
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Id")) },
                    new ODataProperty { Name = "Name", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Name")) }
                }
            };

            var nestedResource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Id")) },
                    new ODataProperty { Name = "Name", Value = new ODataJsonElementValue(jsonRoot.GetProperty("Name")) }
                }
            };

            // Act
            string result = await WriteAndGetPayloadAsync(
                model,
                "application/json; charset=utf-8",
                async writer =>
                {
                    var resourceWriter = await writer.CreateODataResourceWriterAsync(peopleSet);
                    await resourceWriter.WriteStartAsync(resource);

                    await resourceWriter.WriteStartAsync(new ODataNestedResourceInfo
                    {
                        Name = "Friend",
                        IsCollection = false
                    });
                    await resourceWriter.WriteStartAsync(nestedResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();

                    await resourceWriter.WriteEndAsync();
                },
                path: new ODataPath(new EntitySetSegment(peopleSet)));

            // Result
            string expected = @"{""@odata.context"":""http://www.example.com/$metadata#People/$entity"",""Id"":1,""Name"":""John"",""Friend"":{""Id"":1,""Name"":""John""}}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WriteJsonElementValuesInCollectionProperties()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType personType = model.AddEntityType("ns", "Person", baseType: null, isAbstract: false, isOpen: true);
            personType.AddKeys(
                personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty(
                "Emails",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
            IEdmEntitySet peopleSet = model.AddEntityContainer("ns", "Service")
                .AddEntitySet("People", personType);

            string source = @"{""Email"":""john@mailer.com""}";
            JsonDocument json = JsonDocument.Parse(source);
            JsonElement jsonRoot = json.RootElement;
            ODataResource resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty {
                        Name = "Emails",
                        Value = new ODataCollectionValue
                        {
                            TypeName = "Collection(Edm.String)",
                            Items = new object[] { new ODataJsonElementValue(jsonRoot.GetProperty("Email")) }
                        }
                    },
                }
            };

            // Act
            string result = WriteAndGetPayload(
                model,
                "application/json; charset=utf-8",
                writer =>
                {
                    var resourceWriter = writer.CreateODataResourceWriter(peopleSet);
                    resourceWriter.WriteStart(resource);
                    resourceWriter.WriteEnd();
                },
                path: new ODataPath(new EntitySetSegment(peopleSet)));

            // Result
            string expected = @"{""@odata.context"":""http://www.example.com/$metadata#People/$entity"",""Id"":1,""Emails"":[""john@mailer.com""]}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteJsonElementValuesInCollectionProperties_Async()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType personType = model.AddEntityType("ns", "Person", baseType: null, isAbstract: false, isOpen: true);
            personType.AddKeys(
                personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty(
                "Emails",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
            IEdmEntitySet peopleSet = model.AddEntityContainer("ns", "Service")
                .AddEntitySet("People", personType);

            string source = @"{""Email"":""john@mailer.com""}";
            JsonDocument json = JsonDocument.Parse(source);
            JsonElement jsonRoot = json.RootElement;
            ODataResource resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty {
                        Name = "Emails",
                        Value = new ODataCollectionValue
                        {
                            TypeName = "Collection(Edm.String)",
                            Items = new object[] { new ODataJsonElementValue(jsonRoot.GetProperty("Email")) }
                        }
                    },
                }
            };

            // Act
            string result = await WriteAndGetPayloadAsync(
                model,
                "application/json; charset=utf-8",
                async writer =>
                {
                    var resourceWriter = await writer.CreateODataResourceWriterAsync(peopleSet);
                    await resourceWriter.WriteStartAsync(resource);
                    await resourceWriter.WriteEndAsync();
                },
                path: new ODataPath(new EntitySetSegment(peopleSet)));

            // Result
            string expected = @"{""@odata.context"":""http://www.example.com/$metadata#People/$entity"",""Id"":1,""Emails"":[""john@mailer.com""]}";
            Assert.Equal(expected, result);
        }
        #endregion

        [Fact]
        public void WriteMetadataDocument_WorksForJsonCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();

            string contentType = "application/json";

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
        }

        [Fact]
        public async Task WriteMetadataDocumentAsync_WorksForJsonCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();


            // Act - JSON
            // We allow synchronous I/O for WriteMetadataDocumentAsync because
            // it relies on EdmLib's CsdlWriter which is still synchronous.
            // We should disable synchronous I/O here once CsdlWriter has an async API.
            // See: https://github.com/OData/odata.net/issues/2684
            string payload = await this.WriteAndGetPayloadAsync(edmModel, "application/json", async omWriter =>
            {
                try
                {
                    await omWriter.WriteMetadataDocumentAsync();
                }
                catch (SynchronousIOException)
                {
                    // We allow synchronous I/O for WriteMetadataDocumentAsync because
                    // it relies on EdmLib's CsdlWriter which is still synchronous.
                    // We should disable synchronous I/O here once CsdlWriter has an async API.
                    // See: https://github.com/OData/odata.net/issues/2684
                    // However, disposing the writer should still be truly async.
                }
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

        [Fact]
        public async Task WriteMetadataDocumentAsync_WorksForJsonCsdl_WithNoSynchronousIOSupport()
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

        [Fact]
        public async Task WriteMetadataDocumentPayload_MustEqual_WriteMetadataDocumentAsyncPayload_ForJsonCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();

            // Act
            var contentType = "application/json";

            // Json CSDL generated synchronously
            string syncPayload = this.WriteAndGetPayload(edmModel, contentType, omWriter =>
            {
                omWriter.WriteMetadataDocument();
            });

            // Json CSDL generated asynchronously
            string asyncPayload = await this.WriteAndGetPayloadAsync(edmModel, contentType, async omWriter =>
            {
                await omWriter.WriteMetadataDocumentAsync();
            });

            // Assert
            Assert.Equal(syncPayload, asyncPayload);
        }

        [Fact]
        public async Task WriteLargeMetadataDocumentPayload_MustEqual_WriteLargeMetadataDocumentAsyncPayload_ForJsonCsdl()
        {
            // Arrange
            var contentType = "application/json";

            // Act
            for (int i = 0; i < 10; i++)
            {
                // Json CSDL generated synchronously
                string syncPayload = this.WriteAndGetPayload(_largeEdmModel, contentType, omWriter =>
                {
                    omWriter.WriteMetadataDocument();
                });

                // Json CSDL generated asynchronously
                string asyncPayload = await this.WriteAndGetPayloadWithAsyncYieldStreamAsync(_largeEdmModel, contentType, async omWriter =>
                {
                    await omWriter.WriteMetadataDocumentAsync();
                });

                // Assert
                Assert.Equal(syncPayload, asyncPayload);
            }
        }

        [Fact]
        public async Task WriteLargeMetadataDocumentAsync_CalledMultipleTimes_WorksForJsonCsdl_NoExceptionThrown()
        {
            // Arrange
            string contentType = "application/json";

            var message = new InMemoryMessage() { Stream = new AsyncYieldStream(new MemoryStream()) };

            message.SetHeader("Content-Type", contentType);

            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            // Act
            for (int i = 0; i < 10; i++)
            {
                var exception = await Record.ExceptionAsync(async () =>
                {
                    await using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, _largeEdmModel))
                    {
                        await msgWriter.WriteMetadataDocumentAsync();
                    }
                });

                Assert.Null(exception);
            }
        }

        [Fact]
        public async Task WriteMetadataDocumentAsync_WorksForXmlCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();

            // Act - XML
            string payload = await this.WriteAndGetPayloadAsync(edmModel, "application/xml", async omWriter =>
            {
                try
                {
                    await omWriter.WriteMetadataDocumentAsync();
                }
                catch (SynchronousIOException)
                {
                    // We allow synchronous I/O for WriteMetadataDocumentAsync because
                    // it relies on EdmLib's CsdlWriter which is still synchronous.
                    // We should disable synchronous I/O here once CsdlWriter has an async API.
                    // See: https://github.com/OData/odata.net/issues/2684
                    // However, disposing the writer should still be truly async.
                }
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

        [Fact]
        public async Task WriteLargeMetadataDocumentAsync_CalledMultipleTimes_WorksForXmlCsdl_NoExceptionThrown()
        {
            // Arrange
            string contentType = "application/xml";

            var message = new InMemoryMessage() { Stream = new AsyncYieldStream(new MemoryStream()) };

            message.SetHeader("Content-Type", contentType);

            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            // Act
            for (int i = 0; i < 10; i++)
            {
                var exception = await Record.ExceptionAsync(async () =>
                {
                    await using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, _largeEdmModel))
                    {
                        await msgWriter.WriteMetadataDocumentAsync();
                    }
                });

                Assert.Null(exception);
            }
        }

        [Fact]
        public async Task WriteMetadataDocumentAsync_WorksForXmlCsdl_WithNoSynchronousIOSupport()
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

        [Fact]
        public async Task WriteMetadataDocumentPayload_MustEqual_WriteMetadataDocumentAsyncPayload_ForXmlCsdl()
        {
            // Arrange
            IEdmModel edmModel = GetEdmModel();

            // Act
            var contentType = "application/xml";

            // XML CSDL generated synchronously
            string syncPayload = this.WriteAndGetPayload(edmModel, contentType, omWriter =>
            {
                omWriter.WriteMetadataDocument();
            });

            // XML CSDL generated asynchronously
            string asyncPayload = await this.WriteAndGetPayloadAsync(edmModel, contentType, async omWriter =>
            {
                await omWriter.WriteMetadataDocumentAsync();
            });

            // Assert
            Assert.Equal(asyncPayload, syncPayload);
        }

        [Fact]
        public async Task WriteLargeMetadataDocumentPayload_MustEqual_WriteLargeMetadataDocumentAsyncPayload_ForXmlCsdl()
        {
            // Arrange
            var contentType = "application/xml";

            // Act
            string syncPayload = this.WriteAndGetPayload(_largeEdmModel, contentType, omWriter =>
            {
                omWriter.WriteMetadataDocument();
            });

            string asyncPayload = await this.WriteAndGetPayloadAsync(_largeEdmModel, contentType, async omWriter =>
            {
                await omWriter.WriteMetadataDocumentAsync();
            });

            string asyncPayloadWithAsyncYield = await this.WriteAndGetPayloadWithAsyncYieldStreamAsync(_largeEdmModel, contentType, async omWriter =>
            {
                await omWriter.WriteMetadataDocumentAsync();
            });

            // Assert
            Assert.Equal(syncPayload, asyncPayload);
            Assert.Equal(syncPayload, asyncPayloadWithAsyncYield);
        }

        #region "DisposeAsync"

        [Fact]
        public async Task DisposeAsync_Should_Dispose_Stream_Asynchronously()
        {
            // Arrange
            AsyncStream stream = new AsyncStream(new MemoryStream());
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = stream
            };

            IEdmModel edmModel = GetEdmModel();

            var resource = new ODataResource()
            {
                TypeName = "NS.Customer",
                Properties = new[]
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 10
                    },
                    new ODataProperty
                    {
                        Name = "Name",
                        Value = "John"
                    }
                }
            };

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = true;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));
            message.SetHeader("Content-Type", "application/json");

            // Act
            await using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel))
            {
                var writer = await msgWriter.CreateODataResourceWriterAsync(edmModel.FindDeclaredEntitySet("Customers"));
                await writer.WriteStartAsync(resource);
                await writer.WriteEndAsync();
            }

            // Assert
            Assert.True(stream.Disposed);
        }

        [Fact]
        public async Task DisposeAsync_Should_Dispose_Stream_Asynchronously_AfterWritingJsonMetadata()
        {
            AsyncStream stream = new AsyncStream(new MemoryStream());
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = stream
            };

            IEdmModel edmModel = GetEdmModel();

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = true;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));
            message.SetHeader("Content-Type", "application/json");

            var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel);

            try
            {
                await msgWriter.WriteMetadataDocumentAsync();
            }
            catch (SynchronousIOException)
            {
                // We allow synchronous I/O for WriteMetadataDocumentAsync because
                // it relies on EdmLib's CsdlWriter which is still synchronous.
                // We should disable synchronous I/O here once CsdlWriter has an async API.
                // See: https://github.com/OData/odata.net/issues/2684
                // However, disposing the writer should still be truly async.
            }

            await msgWriter.DisposeAsync();

            Assert.True(stream.Disposed);
        }

        [Fact]
        public async Task DisposeAsync_Should_Dispose_Stream_Asynchronously_AfterWritingJsonMetadata_NoSyncSupport()
        {
            AsyncStream stream = new AsyncStream(new MemoryStream());
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = stream
            };

            IEdmModel edmModel = GetEdmModel();

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = true;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));
            message.SetHeader("Content-Type", "application/json");

            var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel);

            await msgWriter.WriteMetadataDocumentAsync();

            await msgWriter.DisposeAsync();

            Assert.True(stream.Disposed);
        }

        [Fact]
        public async Task DisposeAsync_Should_Dispose_Stream_Asynchronously_AfterWritingXmlMetadata()
        {
            AsyncStream stream = new AsyncStream(new MemoryStream());
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = stream
            };

            IEdmModel edmModel = GetEdmModel();

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = true;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));
            message.SetHeader("Content-Type", "application/xml");

            var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel);

            try
            {
                await msgWriter.WriteMetadataDocumentAsync();
            }
            catch (SynchronousIOException)
            {
                // We allow synchronous I/O for WriteMetadataDocumentAsync because
                // it relies on EdmLib's CsdlWriter which is still synchronous.
                // We should disable synchronous I/O here once CsdlWriter has an async API.
                // See: https://github.com/OData/odata.net/issues/2684
                // However, disposing the writer should still be truly async.
            }

            await msgWriter.DisposeAsync();

            Assert.True(stream.Disposed);
        }

        [Fact]
        public async Task DisposeAsync_Should_Dispose_Stream_Asynchronously_AfterWritingXmlMetadata_NoSyncSupport()
        {
            AsyncStream stream = new AsyncStream(new MemoryStream());
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = stream
            };

            IEdmModel edmModel = GetEdmModel();

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = true;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));
            message.SetHeader("Content-Type", "application/xml");

            var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel);

            await msgWriter.WriteMetadataDocumentAsync();

            await msgWriter.DisposeAsync();

            Assert.True(stream.Disposed);
        }

        [Fact]
        public async void DisposeAsync_Should_Dispose_Stream_Asynchronously_AfterWritingRawValue()
        {
            AsyncStream stream = new AsyncStream(new MemoryStream());
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = stream
            };

            IEdmModel edmModel = GetEdmModel();
            var settings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = true,
            };

            var writer = new ODataMessageWriter((IODataResponseMessageAsync)message, settings, edmModel);
            await writer.WriteValueAsync(123);
            await writer.DisposeAsync();

            Assert.True(stream.Disposed);
        }

        [Fact]
        public void WriteMetadataDocument_WorksForLegacyScaleAndSridVariables()
        {
            // Arrange
            IEdmModel edmModel = GetScaleAndSridModel();

            // Act
            string payload = this.WriteAndGetPayloadForScaleAndSridVariables(edmModel, useLegacyScaleAndSridVariable : true, messageWriter =>
            {
                messageWriter.WriteMetadataDocument();
            });

            // Assert
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices><Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                "<ComplexType Name=\"Complex\"><Property Name=\"GeographyPoint\" Type=\"Edm.GeographyPoint\" SRID=\"Variable\" />" +
                "<Property Name=\"DecimalProperty\" Type=\"Edm.Decimal\" Nullable=\"false\" Scale=\"Variable\" />" +
                "</ComplexType>" +
                "</Schema></edmx:DataServices></edmx:Edmx>", payload);
        }

        [Fact]
        public void WriteMetadataDocument_WorksFor_LowerScaleAndSridVariable()
        {
            // Arrange
            IEdmModel edmModel = GetScaleAndSridModel();

            // Act
            string payload = this.WriteAndGetPayloadForScaleAndSridVariables(edmModel, useLegacyScaleAndSridVariable : false, messageWriter =>
            {
                messageWriter.WriteMetadataDocument();
            });

            // Assert
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices><Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                "<ComplexType Name=\"Complex\"><Property Name=\"GeographyPoint\" Type=\"Edm.GeographyPoint\" SRID=\"variable\" />" +
                "<Property Name=\"DecimalProperty\" Type=\"Edm.Decimal\" Nullable=\"false\" Scale=\"variable\" />" +
                "</ComplexType>" +
                "</Schema></edmx:DataServices></edmx:Edmx>", payload);
        }

        #endregion

        private static IEdmModel _edmModel;

        // Large model with 1000 entity types
        private static IEdmModel _largeEdmModel = GetLargeEdmModel();

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

        /// <summary>
        /// This large EdmModel is used to test issues related to writing large metadata documents. 
        /// For example, async writing of large metadata documents, writing large metadata documents multiple times, etc.
        /// </summary>
        /// <returns>Large EdmModel</returns>
        private static IEdmModel GetLargeEdmModel()
        {
            EdmModel edmModel = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            edmModel.AddElement(container);

            string longString = GenerateLongString();

            // Add 1000 entity types
            for (int i = 0; i < 1000; i++)
            {
                EdmEntityType entityType = new EdmEntityType("NS", $"Entity{i}");
                var idProperty = new EdmStructuralProperty(entityType, $"Entity{i}Id", EdmCoreModel.Instance.GetInt32(false));
                entityType.AddProperty(idProperty);
                entityType.AddKeys(new IEdmStructuralProperty[] { idProperty });

                // Add 300 properties to each entity type
                for (int j = 0; j < 100; j++)
                {
                    entityType.AddProperty(new EdmStructuralProperty(entityType, $"PropertyString{longString}{j}", EdmCoreModel.Instance.GetString(false)));
                    entityType.AddProperty(new EdmStructuralProperty(entityType, $"PropertyInt{longString}{j}", EdmCoreModel.Instance.GetInt32(false)));
                    entityType.AddProperty(new EdmStructuralProperty(entityType, $"PropertyBool{j}{longString}{j}", EdmCoreModel.Instance.GetBoolean(false)));
                }

                edmModel.AddElement(entityType);
                container.AddEntitySet($"Entities{i}{longString}", entityType);
            }

            return edmModel;
        }

        private static string GenerateLongString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 20; i++)
            {
                sb.Append("abcxyz");
            }
            return sb.ToString();
        }


        private static IEdmModel GetScaleAndSridModel()
        {
            string csdlTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
    "<edmx:DataServices>" +
    "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
    "<ComplexType Name=\"Complex\">" +
    "<Property Name=\"GeographyPoint\" Type=\"Edm.GeographyPoint\" SRID=\"variable\" />" +
    "<Property Name=\"DecimalProperty\" Type=\"Edm.Decimal\" Nullable=\"false\" Scale=\"variable\" />" +
    "</ComplexType>" +
    "</Schema>" +
    "</edmx:DataServices>+" +
    "</edmx:Edmx>";

            // Parse into CSDL
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdlTemplate).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }

            return model;
        }

        /// <summary>
        /// Write a message using an <see cref="ODataMessageWriter"/> instance
        /// and returns the written payload as a string.
        /// </summary>
        /// <param name="edmModel">The <see cref="IEdmModel"/> used to initialize the writer.</param>
        /// <param name="contentType">The value of the message's Content-Type header.</param>
        /// <param name="test">The action that writes the payload.</param>
        /// <param name="message">
        /// The message instance that encapsulate the request.
        /// If none is provided, a new instance of <see cref="InMemoryMessage"/>.
        /// The provided instance might be modified by this method.
        /// </param>
        /// <param name="encoding">
        /// The encoding to use when reading from the request stream. This defaults to UTF8.
        /// If you set a custom <paramref name="contentType"/>, you should specify a matching encoding.
        /// </param>
        /// <param name="configureServices">
        /// Action to inject services to the dependency-injection container.
        /// If this is set, then the generated <see cref="IServiceProvider"/> will be added to the <paramref name="message"/>.Container property.
        /// </param>
        /// <param name="path">
        /// The OData Uri path.
        /// </param>
        /// <returns>The written output.</returns>
        private string WriteAndGetPayload(
            IEdmModel edmModel,
            string contentType,
            Action<ODataMessageWriter> test,
            InMemoryMessage message = null,
            Encoding encoding = null,
            Action<IServiceCollection> configureServices = null,
            ODataPath path = null)
        {
            message = message ?? new InMemoryMessage() { Stream = new MemoryStream() };

            if (contentType != null)
            {
                message.SetHeader("Content-Type", contentType);
            }

            if (configureServices != null)
            {
                IServiceProvider container = CreateTestServiceContainer(configureServices);
                message.ServiceProvider = container;
            }

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            if (path != null)
            {
                writerSettings.ODataUri = new ODataUri
                {
                    ServiceRoot = writerSettings.BaseUri,
                    Path = path
                };
            }

            using (var msgWriter = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, edmModel))
            {
                test(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream, encoding: encoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Write a message using an <see cref="ODataMessageWriter"/> instance
        /// and returns the written payload as a string.
        /// </summary>
        /// <param name="edmModel">The <see cref="IEdmModel"/> used to initialize the writer.</param>
        /// <param name="useLegacyScaleAndSridVariable">If true, use legacy variable for Scale and SRID.Otherwise, don't.</param>
        /// <param name="writePayload">The action that writes the payload.</param>
        /// <param name="message">
        /// The message instance that encapsulate the request.
        /// If none is provided, a new instance of <see cref="InMemoryMessage"/>.
        /// The provided instance might be modified by this method.
        /// </param>
        /// <param name="encoding">
        /// The encoding to use when reading from the request stream. This defaults to UTF8.
        /// If you set a custom <paramref name="contentType"/>, you should specify a matching encoding.
        /// </param>
        /// <param name="configureServices">
        /// Action to inject services to the dependency-injection container.
        /// If this is set, then the generated <see cref="IServiceProvider"/> will be added to the <paramref name="message"/>.Container property.
        /// </param>
        /// <param name="path">
        /// The OData Uri path.
        /// </param>
        /// <returns>The written output.</returns>
        private string WriteAndGetPayloadForScaleAndSridVariables(
            IEdmModel edmModel,
            bool useLegacyScaleAndSridVariable,
            Action<ODataMessageWriter> writePayload,
            InMemoryMessage message = null,
            Encoding encoding = null)
        {
            message = message ?? new InMemoryMessage() { Stream = new MemoryStream() };

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                BaseUri = new Uri("http://www.example.com/"),
                LibraryCompatibility = useLegacyScaleAndSridVariable ? ODataLibraryCompatibility.UseLegacyVariableCasing : ODataLibraryCompatibility.None
            };

            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            using (var msgWriter = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, edmModel))
            {
                writePayload(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);

            using (StreamReader reader = new StreamReader(message.Stream, encoding: encoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Asynchronously writes a message using an <see cref="ODataMessageWriter"/> instance
        /// and returns the written payload as a string.
        /// </summary>
        /// <param name="edmModel">The <see cref="IEdmModel"/> used to initialize the writer.</param>
        /// <param name="contentType">The value of the message's Content-Type header.</param>
        /// <param name="test">The action that writes the payload.</param>
        /// <param name="message">
        /// The message instance that encapsulate the request.
        /// If none is provided, a new instance of <see cref="InMemoryMessage"/>.
        /// The provided instance might be modified by this method.
        /// </param>
        /// <param name="encoding">
        /// The encoding to use when reading from the request stream. This defaults to UTF8.
        /// If you set a custom <paramref name="contentType"/>, you should specify a matching encoding.
        /// </param>
        /// <param name="configureServices">
        /// Action to inject services to the dependency-injection container.
        /// If this is set, then the generated <see cref="IServiceProvider"/> will be added to the <paramref name="message"/>.Container property.
        /// </param>
        /// <param name="path">
        /// The OData Uri path.
        /// </param>
        /// <param name="allowSyncIO">
        /// Whether to allow synchronous I/O. When set to false, an exception will be thrown if
        /// the writer attempts to perform synchronous I/O on the underlying stream.
        /// Setting it false helps to detect and remove synchronous I/O in async code paths.
        /// </param>
        /// <returns>A task representing the asynchrnous operation. The result of the task will be the written output.</returns>
        private async Task<string> WriteAndGetPayloadAsync(
            IEdmModel edmModel, string contentType,
            Func<ODataMessageWriter, Task> test,
            InMemoryMessage message = null,
            Encoding encoding = null,
            Action<IServiceCollection> configureServices = null,
            ODataPath path = null,
            bool allowSyncIO = false)
        {
            Stream stream = new MemoryStream();
            if (!allowSyncIO)
            {
                stream = new AsyncStream(stream);
            }

            message = message ?? new InMemoryMessage() {
                Stream = stream
            };

            if (contentType != null)
            {
                message.SetHeader("Content-Type", contentType);
            }

            if (configureServices != null)
            {
                IServiceProvider container = CreateTestServiceContainer(configureServices);
                message.ServiceProvider = container;
            }

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            if (path != null)
            {
                writerSettings.ODataUri = new ODataUri
                {
                    ServiceRoot = writerSettings.BaseUri,
                    Path = path
                };
            }

            await using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel))
            {
                await test(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);

            using (
                StreamReader reader = new StreamReader(
                    message.Stream, encoding: encoding ?? Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024, leaveOpen: true)
            )
            {
                string contents = await reader.ReadToEndAsync();

                // Dispose stream manually to avoid synchronous I/O
                await message.Stream.DisposeAsync();
                return contents;
            }
        }

        /// <summary>
        /// Asynchronously writes a message using an <see cref="ODataMessageWriter"/> instance, AsyncYieldStream
        /// and returns the written payload as a string.
        /// </summary>
        /// <param name="edmModel">The <see cref="IEdmModel"/> used to initialize the writer.</param>
        /// <param name="contentType">The value of the message's Content-Type header.</param>
        /// <param name="test">The action that writes the payload.</param>
        /// <returns>A task representing the asynchrnous operation. The result of the task will be the written output.</returns>
        private async Task<string> WriteAndGetPayloadWithAsyncYieldStreamAsync(IEdmModel edmModel, string contentType, Func<ODataMessageWriter, Task> test)
        {
            var message = new InMemoryMessage() { Stream = new AsyncYieldStream(new MemoryStream()) };

            message.SetHeader("Content-Type", contentType);

            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri("http://www.example.com/");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            await using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel))
            {
                await test(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(message.Stream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
            {
                string contents = await reader.ReadToEndAsync();
                return contents;
            }
        }

        private static IServiceProvider CreateTestServiceContainer(Action<IServiceCollection> configureServices)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDefaultODataServices();

            configureServices?.Invoke(services);

            return services.BuildServiceProvider();
        }
    }
}
