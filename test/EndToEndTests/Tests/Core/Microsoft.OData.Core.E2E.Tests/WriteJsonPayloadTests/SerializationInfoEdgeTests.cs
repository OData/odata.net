//-----------------------------------------------------------------------------
// <copyright file="SerializationInfoEdgeTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.WriteJsonPayloadTests
{
    /// <summary>
    /// This class tests OData serialization behavior, particularly
    /// how OData payloads are written when serialization information is either included or omitted
    /// </summary>
    public class SerializationInfoEdgeTests : EndToEndTestBase<SerializationInfoEdgeTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly IEdmModel _model;
        private static string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public SerializationInfoEdgeTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _model = CommonEndToEndEdmModel.GetEdmModel();
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingAnEntryWithoutSerializationInfo_WithMetadata_ShouldThrowAnException(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
            var entry = CreatePersonEntryWithoutSerializationInfo();

            var expectedError = Error.Format(SRResources.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
            var exception = await Assert.ThrowsAsync<ODataException>(async () => await odataWriter.WriteStartAsync(entry));

            Assert.Equal(expectedError, exception.Message);
        }

        [Fact]
        public async Task WritingAnEntryWithoutSerializationInfo_WithoutMetadata_ShouldWriteAnEntryWithoutODataMetadata()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
            var entry = CreatePersonEntryWithoutSerializationInfo();
            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.Equal("{\"PersonId\":-5,\"Name\":\"xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu\"}", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingAFeedWithoutSerializationInfo_WithMetadata_ShouldThrowAnException(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();
            var feed = CreatePersonFeed();
            var entry = CreatePersonEntryWithoutSerializationInfo();

            entry.SetSerializationInfo(new ODataResourceSerializationInfo()
            {
                NavigationSourceName = "People",
                NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
            });

            var expectedError = Error.Format(SRResources.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
            var exception = await Assert.ThrowsAsync<ODataException>(async () => await odataWriter.WriteStartAsync(feed));

            Assert.Equal(expectedError, exception.Message);
        }

        [Fact]
        public async Task WritingAFeedWithoutSerializationInfo_WithoutMetadata_ShouldWriteAnEmptyArray()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();
            var feed = CreatePersonFeed();
            var entry = CreatePersonEntryWithoutSerializationInfo();

            entry.SetSerializationInfo(new ODataResourceSerializationInfo()
            {
                NavigationSourceName = "People",
                NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
            });

            await odataWriter.WriteStartAsync(feed);
            await odataWriter.WriteEndAsync();

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.Equal("{\"value\":[]}", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingACollectionWithoutSerializationInfo_WithMetadata_ShouldThrowAnException(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataCollectionWriterAsync();
            var collectionStart = new ODataCollectionStart() { Name = "BackupContactInfo" };

            var expectedError = Error.Format(SRResources.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
            var exception = await Assert.ThrowsAsync<ODataException>(async () => await odataWriter.WriteStartAsync(collectionStart));

            Assert.Equal(expectedError, exception.Message);
        }

        [Fact]
        public async Task WritingACollectionWithoutSerializationInfo_WithoutMetadata_ShouldWriteAnEmptyArray()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataCollectionWriterAsync();
            var collectionStart = new ODataCollectionStart() { Name = "BackupContactInfo" };

            await odataWriter.WriteStartAsync(collectionStart);
            await odataWriter.WriteEndAsync();

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.Equal("{\"value\":[]}", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingAReferenceLinkWithoutSerializationInfo_WithMetadata_ShouldContainMetadataReference(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var link = new ODataEntityReferenceLink() { Url = new Uri(_baseUri + "Orders(-10)") };
            await messageWriter.WriteEntityReferenceLinkAsync(link);

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.Contains("$metadata#$ref", result);
        }

        [Fact]
        public async Task WritingAReferenceLinkWithoutSerializationInfo_WithMetadata_ShouldNotContainMetadataReference()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var link = new ODataEntityReferenceLink() { Url = new Uri(_baseUri + "Orders(-10)") };
            await messageWriter.WriteEntityReferenceLinkAsync(link);

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.DoesNotContain("$metadata#$ref", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        [InlineData(MimeTypeODataParameterNoMetadata)]
        public async Task WritingARequestMessageWithEntry_ShouldContainExpectedContent(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var requestMessage = new TestStreamRequestMessage(new MemoryStream(), new Uri(_baseUri + "People"), "POST");
            requestMessage.SetHeader("Content-Type", mimeType);

            using var messageWriter = new ODataMessageWriter(requestMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
            var entry = CreatePersonEntryWithoutSerializationInfo();

            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();

            Stream stream = await requestMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.Contains("People(-5)\",\"PersonId\":-5,\"Name\":\"xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu\"", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task SpecifyingSerializationInfoForBothFeedAndEntry_WithMetadata_ShouldContainExpectedEntryType(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessage, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();
                var feed = CreatePersonFeed();
                feed.SetSerializationInfo(
                    new ODataResourceSerializationInfo()
                    {
                        NavigationSourceName = "People",
                        NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
                    });

                var entry = new ODataResource()
                {
                    Id = new Uri(_baseUri + "People(-5)"),
                    TypeName = NameSpacePrefix + "Employee"
                };

                var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
                var personEntryP2 = new ODataProperty
                {
                    Name = "Name",
                    Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
                };

                var personEntryP3 = new ODataProperty
                {
                    Name = "ManagersPersonId",
                    Value = -465010984
                };

                entry.Properties = new[] { personEntryP1, personEntryP2, personEntryP3 };
                entry.SetSerializationInfo(
                    new ODataResourceSerializationInfo()
                    {
                        NavigationSourceName = "Person",
                        NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
                    });


                await odataWriter.WriteStartAsync(feed);
                await odataWriter.WriteStartAsync(entry);
                await odataWriter.WriteEndAsync();
                await odataWriter.WriteEndAsync();
            }

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.Contains(NameSpacePrefix + "Employee", result);
            Assert.Contains("$metadata#People", result);
        }

        [Fact]
        public async Task SpecifyingSerializationInfoForBothFeedAndEntry_WithoutMetadata_ShouldNotContainExpectedEntryType()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using (var messageWriter = new ODataMessageWriter(responseMessage, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();
                var feed = CreatePersonFeed();
                feed.SetSerializationInfo(
                    new ODataResourceSerializationInfo()
                    {
                        NavigationSourceName = "People",
                        NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
                    });

                var entry = new ODataResource()
                {
                    Id = new Uri(_baseUri + "People(-5)"),
                    TypeName = NameSpacePrefix + "Employee"
                };

                var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
                var personEntryP2 = new ODataProperty
                {
                    Name = "Name",
                    Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
                };

                var personEntryP3 = new ODataProperty
                {
                    Name = "ManagersPersonId",
                    Value = -465010984
                };

                entry.Properties = new[] { personEntryP1, personEntryP2, personEntryP3 };
                entry.SetSerializationInfo(
                    new ODataResourceSerializationInfo()
                    {
                        NavigationSourceName = "Person",
                        NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
                    });


                await odataWriter.WriteStartAsync(feed);
                await odataWriter.WriteStartAsync(entry);
                await odataWriter.WriteEndAsync();
                await odataWriter.WriteEndAsync();
            }

            Stream stream = await responseMessage.GetStreamAsync();
            string result = await TestsHelper.ReadStreamContentAsync(stream);

            Assert.DoesNotContain(NameSpacePrefix + "Employee", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingAnEntryWithWrongEntitySetName_WithMetadata_ShouldGenerateIncorrectMetadata(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
            var entry = CreatePersonEntryWithoutSerializationInfo();
            entry.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Parsen",
                NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
            });

            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());

            Assert.Contains("\"PersonId\":-5", result);
        }

        [Fact]
        public async Task WritingAnEntryWithWrongEntitySetName_WithoutMetadata_ShouldNotGenerateMetadata()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
            var entry = CreatePersonEntryWithoutSerializationInfo();
            entry.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Parsen",
                NavigationSourceEntityTypeName = NameSpacePrefix + "Person"
            });

            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());

            Assert.Contains("\"PersonId\":-5", result);

            Assert.DoesNotContain("People(-5)\",\"PersonId\":-5", result);
        }

        [Fact]
        public async Task WritingAFeedWithAWrongEntitySetName_WithoutMetadata_ShouldNotGenerateIncorrectMetadata()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();

            var feed = this.CreatePersonFeed();
            feed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
            var entry = this.CreatePersonEntryWithoutSerializationInfo();
            await odataWriter.WriteStartAsync(feed);
            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();
            await odataWriter.WriteEndAsync();
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());

            Assert.Contains("\"PersonId\":-5", result);
            Assert.DoesNotContain("People(-5)\",\"PersonId\":-5", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingAComplexCollectionWithWrongTypeName_WithMetadata_ShouldHaveMetadataWithIncorrectTypeName(string mimeType)
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();

            var complexCollection = new ODataResourceSetWrapper
            {
                ResourceSet = new ODataResourceSet(),
                Resources = new List<ODataResourceWrapper>
                {
                    TestsHelper.CreatePrimaryContactODataWrapper()
                }
            };

            complexCollection.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                ExpectedTypeName = NameSpacePrefix + "ContactDETAIL"
            });

            await ODataWriterHelper.WriteResourceSetAsync(odataWriter, complexCollection);
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());

            Assert.Contains("\"value\":[{\"@odata.type\":\"#Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.ContactDetails\",\"EmailBag", result);
            Assert.Contains("$metadata#Collection(" + NameSpacePrefix + "ContactDETAIL)", result);
        }

        [Fact]
        public async Task WritingAComplexCollectionWithWrongTypeName_WithoutMetadata_ShouldNotHaveMetadata()
        {
            var settings = CreateODataMessageWriterSettings();
            var responseMessage = CreateTestResponseMessage(MimeTypeODataParameterNoMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings);
            var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();

            var complexCollection = new ODataResourceSetWrapper
            {
                ResourceSet = new ODataResourceSet(),
                Resources = new List<ODataResourceWrapper>
                {
                    TestsHelper.CreatePrimaryContactODataWrapper()
                }
            };

            complexCollection.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                ExpectedTypeName = NameSpacePrefix + "ContactDETAIL"
            });

            await ODataWriterHelper.WriteResourceSetAsync(odataWriter, complexCollection);
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());

            Assert.DoesNotContain("\"@odata.type\":\"#Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.ContactDetails\"", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingAnEntryWithWrongSerializationInfo_WithEdmModelAndMetadata_ShouldIncludeIncorrectMetadata(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = _model.EntityContainer.FindEntitySet("People");

            using var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings, _model);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(peopleSet, personType);
            var entry = this.CreatePersonEntryWithoutSerializationInfo();

            entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessageWithoutModel.GetStreamAsync());

            Assert.Contains("\"PersonId\":-5", result);
            Assert.Contains("$metadata#Parsen/$entity", result);
        }

        [Fact]
        public async Task WritingAnEntryWithWrongSerializationInfo_WithEdmModelAndWithoutMetadata_ShouldNotIncludeMetadata()
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", MimeTypeODataParameterNoMetadata);

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = _model.EntityContainer.FindEntitySet("People");

            using var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings, _model);
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(peopleSet, personType);
            var entry = this.CreatePersonEntryWithoutSerializationInfo();

            entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteEndAsync();
            var result = await TestsHelper.ReadStreamContentAsync(await responseMessageWithoutModel.GetStreamAsync());

            Assert.Contains("\"PersonId\":-5", result);
            Assert.DoesNotContain("$metadata", result);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task ChangingSerializationInfoAfterWriteStart_WithMetadata_ShouldUseOriginalMetadata(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();

                // Set initial serialization info
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });

                await odataWriter.WriteStartAsync(entry);

                // Change serialization info after WriteStartAsync
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });

                await odataWriter.WriteEndAsync();

                Stream stream = await responseMessageWithoutModel.GetStreamAsync();
                var result = await TestsHelper.ReadStreamContentAsync(stream);

                // Assert that the metadata still refers to "People", not "Parsen"
                Assert.Contains("People/$entity", result);
                Assert.DoesNotContain("Parsen/$entity", result);
            }
        }

        [Fact]
        public async Task ChangingSerializationInfoAfterWriteStart_WithoutMetadata_ShouldNotContainMetadata()
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", MimeTypeODataParameterNoMetadata);

            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();

                // Set initial serialization info
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });

                await odataWriter.WriteStartAsync(entry);

                // Change serialization info after WriteStartAsync
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });

                await odataWriter.WriteEndAsync();

                Stream stream = await responseMessageWithoutModel.GetStreamAsync();
                var result = await TestsHelper.ReadStreamContentAsync(stream);

                // Assert that the metadata still refers to "People", not "Parsen"
                Assert.DoesNotContain("$metadata", result);
            }
        }

        /// <summary>
        /// Manually create an Edm model and verify OData serialization with metadata.
        /// </summary>
        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task WritingEntryWithHandcraftedEdmModel_WithMetadata_ShouldSerializeCorrectlyAndHasMetadataReference(string mimeType)
        {
            // Create an empty EDM model
            EdmModel edmModel = new EdmModel();

            // Define an EdmEntityType (Person) with a key and properties
            EdmEntityType edmEntityType = new EdmEntityType(NameSpacePrefix, "Person");
            var keyProperty = new EdmStructuralProperty(edmEntityType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            edmEntityType.AddKeys(keyProperty);
            edmEntityType.AddProperty(keyProperty);

            var nameProperty = new EdmStructuralProperty(edmEntityType, "Name", EdmCoreModel.Instance.GetString(true));
            edmEntityType.AddProperty(nameProperty);

            edmModel.AddElement(edmEntityType);

            // Define an entity set (People) inside a container
            var defaultContainer = new EdmEntityContainer(NameSpacePrefix, "DefaultContainer");
            edmModel.AddElement(defaultContainer);

            EdmEntitySet entitySet = new EdmEntitySet(defaultContainer, "People", edmEntityType);
            defaultContainer.AddElement(entitySet);

            // Set up OData writer settings
            var settings = CreateODataMessageWriterSettings();

            var responseMessage = new TestStreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessage, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(entitySet, edmEntityType);

                // Create and write the Person entry
                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                await odataWriter.WriteStartAsync(entry);
                await odataWriter.WriteEndAsync();
            }

            // Read and validate the output
            var responseContent = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());
            Assert.Contains("PersonId", responseContent);
            Assert.Contains("Name", responseContent);
            Assert.Contains("People/$entity", responseContent);
        }

        [Fact]
        public async Task WritingEntryWithHandcraftedEdmModel_WithMetadata_ShouldSerializeCorrectlyAndHasNoMetadataReference()
        {
            // Create an empty EDM model
            EdmModel edmModel = new EdmModel();

            // Define an EdmEntityType (Person) with a key and properties
            EdmEntityType edmEntityType = new EdmEntityType(NameSpacePrefix, "Person");
            var keyProperty = new EdmStructuralProperty(edmEntityType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            edmEntityType.AddKeys(keyProperty);
            edmEntityType.AddProperty(keyProperty);

            var nameProperty = new EdmStructuralProperty(edmEntityType, "Name", EdmCoreModel.Instance.GetString(true));
            edmEntityType.AddProperty(nameProperty);

            edmModel.AddElement(edmEntityType);

            // Define an entity set (People) inside a container
            var defaultContainer = new EdmEntityContainer(NameSpacePrefix, "DefaultContainer");
            edmModel.AddElement(defaultContainer);

            EdmEntitySet entitySet = new EdmEntitySet(defaultContainer, "People", edmEntityType);
            defaultContainer.AddElement(entitySet);

            // Set up OData writer settings
            var settings = CreateODataMessageWriterSettings();

            var responseMessage = new TestStreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", MimeTypeODataParameterNoMetadata);

            using (var messageWriter = new ODataMessageWriter(responseMessage, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(entitySet, edmEntityType);

                // Create and write the Person entry
                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                await odataWriter.WriteStartAsync(entry);
                await odataWriter.WriteEndAsync();
            }

            // Read and validate the output
            var responseContent = await TestsHelper.ReadStreamContentAsync(await responseMessage.GetStreamAsync());
            Assert.Contains("PersonId", responseContent);
            Assert.Contains("Name", responseContent);
            Assert.DoesNotContain("People/$entity", responseContent);
        }

        private ODataResource CreatePersonEntryWithoutSerializationInfo()
        {
            var personEntry = new ODataResource()
            {
                Id = new Uri(_baseUri + "People(-5)"),
                TypeName = NameSpacePrefix + "Person"
            };

            var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
            var personEntryP2 = new ODataProperty
            {
                Name = "Name",
                Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
            };

            personEntry.Properties = new[] { personEntryP1, personEntryP2 };
            personEntry.EditLink = new Uri(_baseUri + "People(-5)");
            return personEntry;
        }

        private ODataResourceSet CreatePersonFeed()
        {
            var orderFeed = new ODataResourceSet()
            {
                Id = new Uri(_baseUri + "People"),
            };

            return orderFeed;
        }

        private ODataMessageWriterSettings CreateODataMessageWriterSettings()
        {
            return new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };
        }

        private TestStreamResponseMessage CreateTestResponseMessage(string mimeType)
        {
            var responseMessage = new TestStreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);
            return responseMessage;
        }

        private WritePayloadTestsHelper TestsHelper
        {
            get
            {
                return new WritePayloadTestsHelper(_baseUri, _model);
            }
        }
    }
}
