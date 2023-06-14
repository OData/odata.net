//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Tests.Json;
using Microsoft.Test.OData.DependencyInjection;
using Microsoft.OData.UriParser;
using Xunit;
using System.Xml;
#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json;
#endif

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
        public void InjectingDifferentInstancesOfJsonWriterAndIJsonWriterAsync_ShouldThrowException()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage request = new InMemoryMessage() { Stream = stream };

                IServiceProvider container = CreateTestServiceContainer(containerBuilder =>
                {
                    containerBuilder.AddService<IJsonWriterFactory>(
                    ServiceLifetime.Singleton, sp => new MockJsonWriterFactory(new MockSyncOnlyJsonWriter()));
                    containerBuilder.AddService<IJsonWriterFactoryAsync>(
                        ServiceLifetime.Singleton, _ => new MockJsonWriterFactoryAsync(new MockAsyncOnlyJsonWriter()));
                });

                request.Container = container;

                settings.ODataUri.ServiceRoot = new Uri("http://www.example.com");
                settings.SetContentType(ODataFormat.Json);
                EdmModel model = new EdmModel();
                using (ODataMessageWriter writer = new ODataMessageWriter((IODataRequestMessage)request, settings, model))
                {
                    Action writePropertyAction = () => writer.WriteProperty(new ODataProperty()
                    {
                        Name = "Name",
                        Value = "This is a test ия"
                    });

                    writePropertyAction.Throws<ODataException>(Strings.ODataMessageWriter_IJsonWriter_And_IJsonWriterAsync_Are_Different_Instances);
                }
            }
        }

        [Fact]
        public void InjectingAnIJsonWriterThatDoesNotImplementSyncIJsonWriteraAsync_ShouldThrowException()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage request = new InMemoryMessage() { Stream = stream };

                IServiceProvider container = CreateTestServiceContainer(containerBuilder =>
                {
                    containerBuilder.AddService<IJsonWriterFactory>(
                    ServiceLifetime.Singleton, _ => new MockJsonWriterFactory(new MockSyncOnlyJsonWriter()));
                });

                request.Container = container;

                settings.ODataUri.ServiceRoot = new Uri("http://www.example.com");
                settings.SetContentType(ODataFormat.Json);
                EdmModel model = new EdmModel();
                using (ODataMessageWriter writer = new ODataMessageWriter((IODataRequestMessage)request, settings, model))
                {
                    Action writePropertyAction = () => writer.WriteProperty(new ODataProperty()
                    {
                        Name = "Name",
                        Value = "This is a test ия"
                    });

                    writePropertyAction.Throws<ODataException>(Strings.ODataMessageWriter_IJsonWriter_And_IJsonWriterAsync_Are_Different_Instances);
                }
            }
        }

#if NETCOREAPP3_1_OR_GREATER
        #region "ODataUtf8JsonWriter support"
        [Fact]
        public void SupportsStreamBasedJsonWriter()
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
                    containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                        ServiceLifetime.Singleton, sp => DefaultStreamBasedJsonWriterFactory.Default);
                });

            IStreamBasedJsonWriterFactory factory = request.Container.GetService<IStreamBasedJsonWriterFactory>();
            Assert.IsType<DefaultStreamBasedJsonWriterFactory>(factory);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test \\u0438\\u044F\"}", output);
        }

        [Fact]
        public void WhenInjectingStreamBasedJsonWriterFactory_DoesNotCreateSynchronousWriter_IfAsyncWriterImplementsSynchronousInterface()
        {
            // Arrange
            MockStreamBasedJsonWriterFactoryWrapper writerFactory =
                new MockStreamBasedJsonWriterFactoryWrapper(DefaultStreamBasedJsonWriterFactory.Default);
            EdmModel model = new EdmModel();

            // Act
            Action<ODataMessageWriter> writePropertyAction = (writer) => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            string output = WriteAndGetPayload(
                model,
                "application/json",
                writePropertyAction,
                configureServices: (containerBuilder) =>
                {
                    containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                        ServiceLifetime.Singleton, _ => writerFactory);
                });

            // Assert
            Assert.IsType<ODataUtf8JsonWriter>(writerFactory.CreatedAsyncWriter);
            Assert.Null(writerFactory.CreatedWriter);
            Assert.Equal(1, writerFactory.NumCalls);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test \\u0438\\u044F\"}", output);
        }

        [Fact]
        public void WhenInjectingStreamBasedJsonWriterFactory_ThrowsException_IfAsyncWriterDoesNotImplementSynchronousInterface()
        {
            // Arrange
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            MockStreamBasedJsonWriterFactory writerFactory =
                new MockStreamBasedJsonWriterFactory(jsonWriter: null, asyncJsonWriter: new MockAsyncOnlyJsonWriter());

            using MemoryStream stream = new MemoryStream();
            InMemoryMessage request = new InMemoryMessage() { Stream = stream };

            IServiceProvider container = CreateTestServiceContainer(containerBuilder =>
            {
                containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                ServiceLifetime.Singleton, sp => writerFactory);
            });

            request.Container = container;

            IStreamBasedJsonWriterFactory factory = request.Container.GetService<IStreamBasedJsonWriterFactory>();
            Assert.IsType<MockStreamBasedJsonWriterFactory>(factory);

            settings.ODataUri.ServiceRoot = new Uri("http://www.example.com");
            EdmModel model = new EdmModel();
            using ODataMessageWriter writer = new ODataMessageWriter((IODataRequestMessage)request, settings, model);

            // Act
            Action writePropertyAction = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            writePropertyAction.Throws<ODataException>(Strings.ODataMessageWriter_IJsonWriterAsync_Must_Implement_IJsonWriter);
        }

        [Theory]
        [InlineData("utf-8")]
        [InlineData("utf-16")]
        [InlineData("utf-16BE")]
        [InlineData("utf-32")]
        public void WhenInjectingStreamBasedJsonWriterFactory_CreatesWriterUsingConfiguredEncoding(string encodingCharset)
        {
            // Arrange
            MockStreamBasedJsonWriterFactoryWrapper writerFactory =
                new MockStreamBasedJsonWriterFactoryWrapper(DefaultStreamBasedJsonWriterFactory.Default);
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
                    containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                        ServiceLifetime.Singleton, sp => writerFactory);
                });

            // Assert
            Assert.IsType<ODataUtf8JsonWriter>(writerFactory.CreatedAsyncWriter);
            Assert.Equal(encodingCharset, writerFactory.Encoding.WebName);
            Assert.Equal(1, writerFactory.NumCalls);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test \\u0438\\u044F\"}", output);
        }

        [Fact]
        public void WhenInjectingStreamBasedJsonWriterFactory_ThrowException_IfFactoryReturnsNull()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            using MemoryStream stream = new MemoryStream();
            InMemoryMessage request = new InMemoryMessage() { Stream = stream };

            IServiceProvider container = CreateTestServiceContainer(containerBuilder =>
            {
                containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                    ServiceLifetime.Singleton, sp => new MockStreamBasedJsonWriterFactory(null, null));
            });

            request.Container = container;

            IStreamBasedJsonWriterFactory factory = request.Container.GetService<IStreamBasedJsonWriterFactory>();
            Assert.IsType<MockStreamBasedJsonWriterFactory>(factory);

            settings.ODataUri.ServiceRoot = new Uri("http://www.example.com");
            settings.SetContentType(ODataFormat.Json);
            EdmModel model = new EdmModel();
            using ODataMessageWriter writer = new ODataMessageWriter((IODataRequestMessage)request, settings, model);
            Action writePropertyAction = () => writer.WriteProperty(new ODataProperty()
            {
                Name = "Name",
                Value = "This is a test ия"
            });

            writePropertyAction.Throws<ODataException>(Strings.ODataMessageWriter_StreamBasedJsonWriterFactory_ReturnedNull(Encoding.UTF8.WebName, false));
        }

        [Fact]
        public async Task SupportsStreamBasedJsonWriterAsync()
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
                    containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                        ServiceLifetime.Singleton, sp => DefaultStreamBasedJsonWriterFactory.Default);
                });

            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test \\u0438\\u044F\"}", output);
        }

        [Theory]
        [InlineData("utf-8")]
        [InlineData("utf-16")]
        [InlineData("utf-16BE")]
        [InlineData("utf-32")]
        public async Task WhenInjectingStreamBasedJsonWriterFactoryAsync_CreatesWriterUsingConfiguredEncoding(string encodingCharset)
        {
            // Arrange
            MockStreamBasedJsonWriterFactoryWrapper writerFactory =
                new MockStreamBasedJsonWriterFactoryWrapper(DefaultStreamBasedJsonWriterFactory.Default);
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
                    containerBuilder.AddService<IStreamBasedJsonWriterFactory>(
                        ServiceLifetime.Singleton, sp => writerFactory);
                });

            // Assert
            Assert.IsType<ODataUtf8JsonWriter>(writerFactory.CreatedAsyncWriter);
            Assert.Equal(encodingCharset, writerFactory.Encoding.WebName);
            Assert.Equal(1, writerFactory.NumCalls);
            Assert.Equal("{\"@odata.context\":\"http://www.example.com/$metadata#Edm.String\",\"value\":\"This is a test \\u0438\\u044F\"}", output);
        }

        #endregion "ODataUtf8JsonWriter support"
#endif

#if NETCOREAPP3_1_OR_GREATER
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
#endif

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
#endif

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

        #region "DisposeAsync"
#if NETCOREAPP3_1_OR_GREATER

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
#endif
        #endregion


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
            Action<IContainerBuilder> configureServices = null,
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
                message.Container = container;
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
            Action<IContainerBuilder> configureServices = null,
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
                message.Container = container;
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

#if NETCOREAPP3_1_OR_GREATER
            await using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel))
#else
            using (var msgWriter = new ODataMessageWriter((IODataResponseMessageAsync)message, writerSettings, edmModel))
#endif
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
#if NETCOREAPP3_1_OR_GREATER
                await message.Stream.DisposeAsync();
#else
                message.Stream.Dispose();
#endif
                return contents;
            }
        }

        private static IServiceProvider CreateTestServiceContainer(Action<IContainerBuilder> configureServices)
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddDefaultODataServices();

            configureServices.Invoke(builder);

            return builder.BuildContainer();
        }
    }
}
