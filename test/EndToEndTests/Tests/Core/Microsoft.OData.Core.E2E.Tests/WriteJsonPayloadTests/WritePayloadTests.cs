//-----------------------------------------------------------------------------
// <copyright file="WritePayloadTests.cs" company=".NET Foundation">
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
using System.Text.RegularExpressions;

namespace Microsoft.OData.Core.E2E.Tests.WriteJsonPayloadTests
{
    public class WritePayloadTests : EndToEndTestBase<WritePayloadTests.TestsStartup>
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

        public WritePayloadTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _model = CommonEndToEndEdmModel.GetEdmModel();
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata, "\"@odata.associationLink\":", "\"@odata.type\":")]
        [InlineData(MimeTypeODataParameterMinimalMetadata, "", "\"@odata.type\":")]
        [InlineData(MimeTypeODataParameterNoMetadata, "", "")]
        public async Task WritingAnODataFeed_WithDifferentMimeTypes_ShouldMatchExpectedPayload(string? mimeType, string associationLink, string odataType)
        {
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri() { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };

            string outputWithModel;
            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);

            var orderType = _model.FindDeclaredType(NameSpacePrefix + "Order") as IEdmEntityType;
            var orderSet = _model.EntityContainer.FindEntitySet("Orders");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync(orderSet, orderType);
                outputWithModel = await WriteAnOrderFeedAsync(responseMessageWithModel, odataWriter, true, mimeType);
            }

            string outputWithoutModel;
            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();
                outputWithoutModel = await WriteAnOrderFeedAsync(responseMessageWithoutModel, odataWriter, false, mimeType);
            }

            if (!string.IsNullOrEmpty(associationLink))
            {
                var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");

                outputWithModel = rex.Replace(outputWithModel, "");
                outputWithoutModel = rex.Replace(outputWithoutModel, "");
            }

            if (!string.IsNullOrEmpty(odataType))
            {
                var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");

                outputWithoutModel = rex.Replace(outputWithoutModel, "");
                outputWithModel = rex.Replace(outputWithModel, "");
            }

            Assert.Equal(outputWithModel, outputWithoutModel);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata, "\"@odata.associationLink\":", "\"@odata.type\":")]
        [InlineData(MimeTypeODataParameterMinimalMetadata, "", "\"@odata.type\":")]
        [InlineData(MimeTypeODataParameterNoMetadata, "", "")]
        public async Task WritingAnExpandedEntry_WithDifferentMimeTypes_ShouldMatchExpectedPayload(
            string? mimeType,
            string associationLink,
            string odataType)
        {
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri() { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };

            string outputWithModel = null;
            string outputWithoutModel = null;

            // Without Model
            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
                outputWithoutModel = await WriteAnExpandedCustomerEntryAsync(responseMessageWithoutModel, odataWriter, false, mimeType);
            }

            // With Model
            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);

            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var customerSet = _model.EntityContainer.FindEntitySet("Customers");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(customerSet, customerType);
                outputWithModel = await WriteAnExpandedCustomerEntryAsync(responseMessageWithModel, odataWriter, false, mimeType);
            }

            if (!string.IsNullOrEmpty(associationLink))
            {
                var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");
                outputWithModel = rex.Replace(outputWithModel, "");
                outputWithoutModel = rex.Replace(outputWithoutModel, "");
            }

            if (!string.IsNullOrEmpty(odataType))
            {
                var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                outputWithModel = rex.Replace(outputWithModel, "");
                outputWithoutModel = rex.Replace(outputWithoutModel, "");
            }

            // Assert final payloads match
            Assert.Equal(outputWithModel, outputWithoutModel);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterMinimalMetadata, "$metadata#People\"", "{\"@odata.type\":\"#Employee\"", "{\"@odata.type\":\"#SpecialEmployee\"")]
        [InlineData(MimeTypeODataParameterFullMetadata, "$metadata#People\"", "{\"@odata.type\":\"#Employee\"", "{\"@odata.type\":\"#SpecialEmployee\"")]
        [InlineData(MimeTypeODataParameterNoMetadata, "", "", "")]
        public async Task WritingAFeedContainingActionsAndDerivedTypes_ShouldMatchExpectedPayload(
            string mimeType,
            string expectedMetadata,
            string expectedDerivedTypeEmployee,
            string expectedDerivedTypeSpecialEmployee)
        {
            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                ODataUri = new ODataUri { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };

            string outputWithModel, outputWithoutModel;

            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var personSet = _model.EntityContainer.FindEntitySet("People");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync(personSet, personType);
                outputWithModel = await WriteAPersonFeedAsync(responseMessageWithModel, odataWriter, false, mimeType);
            }

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync();
                outputWithoutModel = await WriteAPersonFeedAsync(responseMessageWithoutModel, odataWriter, false, mimeType);
            }

            Assert.Equal(outputWithModel, outputWithoutModel);

            if (!string.IsNullOrEmpty(expectedMetadata))
            {
                Assert.Contains(_baseUri + expectedMetadata, outputWithoutModel);
            }

            if (!string.IsNullOrEmpty(expectedDerivedTypeEmployee))
            {
                Assert.True(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpacePrefix + "Employee\","), "odata.type Employee");
            }

            if (!string.IsNullOrEmpty(expectedDerivedTypeSpecialEmployee))
            {
                Assert.True(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpacePrefix + "SpecialEmployee\","), "odata.type SpecialEmployee");
            }
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata, "$metadata#People/$entity", "odata.type")]
        [InlineData(MimeTypeODataParameterMinimalMetadata, "$metadata#People/$entity", "odata.type")]
        [InlineData(MimeTypeODataParameterNoMetadata, "", "")]
        public async Task WritingAnEntryWithOrWithoutTypeCast_ShouldMatchExpectedMetadata(
            string mimeType,
            string expectedMetadata,
            string expectedODataType)
        {
            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                ODataUri = new ODataUri { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };

            async Task<string> WriteEmployeeEntryAsync(bool withTypeCast)
            {
                var responseMessage = new TestStreamResponseMessage(new MemoryStream());
                responseMessage.SetHeader("Content-Type", mimeType);
                using var messageWriter = new ODataMessageWriter(responseMessage, settings);
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
                return await WriteAnEmployeeEntryAsync(responseMessage, odataWriter, withTypeCast, mimeType);
            }

            string outputWithoutTypeCast = await WriteEmployeeEntryAsync(false);
            string outputWithTypeCast = await WriteEmployeeEntryAsync(true);

            if (!string.IsNullOrEmpty(expectedMetadata))
            {
                Assert.Contains(_baseUri + expectedMetadata, outputWithoutTypeCast);
                Assert.Contains(_baseUri + expectedMetadata.Replace("$entity", NameSpacePrefix + "Employee/$entity"), outputWithTypeCast);
            }

            if (!string.IsNullOrEmpty(expectedODataType))
            {
                Assert.Contains(expectedODataType, outputWithoutTypeCast);
            }
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        [InlineData(MimeTypeODataParameterNoMetadata)]
        public async Task WritingAnEntryContainingAStream_ShouldMatchExpectedPayload(string mimeType)
        {
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri() { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };
            string outputWithModel = null;
            string outputWithoutModel = null;

            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);
            responseMessageWithModel.PreferenceAppliedHeader().AnnotationFilter = "*";

            var carType = _model.FindDeclaredType(NameSpacePrefix + "Car") as IEdmEntityType;
            var carSet = _model.EntityContainer.FindEntitySet("Cars");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(carSet, carType);
                outputWithModel = await WriteACarEntryAsync(responseMessageWithModel, odataWriter, true, mimeType);
            }

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            responseMessageWithoutModel.PreferenceAppliedHeader().AnnotationFilter = "*";
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
                outputWithoutModel = await WriteACarEntryAsync(responseMessageWithoutModel, odataWriter, false, mimeType);
            }

            Assert.Equal(outputWithModel, outputWithoutModel);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        [InlineData(MimeTypeODataParameterNoMetadata)]
        public async Task WritingAFeedWithComplexCollections_ShouldMatchExpectedPayload(string mimeType)
        {
            string testMimeType = mimeType.Contains("xml") ? MimeTypes.ApplicationXml : mimeType;

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                ODataUri = new ODataUri() { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };
            string outputWithModel = null;
            string outputWithoutModel = null;

            var contactDetailType = _model.FindDeclaredType(NameSpacePrefix + "ContactDetails") as IEdmComplexType;

            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", testMimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync(null, contactDetailType);
                outputWithModel = await this.WriteACollectionAsync(
                    responseMessageWithModel,
                    odataWriter,
                    true,
                    testMimeType);
            }

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", testMimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceSetWriterAsync(null, contactDetailType);
                outputWithoutModel = await this.WriteACollectionAsync(
                    responseMessageWithoutModel,
                    odataWriter,
                    false,
                    testMimeType);
            }

            Assert.Equal(outputWithModel, outputWithoutModel);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata, true)]
        [InlineData(MimeTypeODataParameterMinimalMetadata, true)]
        [InlineData(MimeTypeODataParameterNoMetadata, false)]
        public async Task WritingEntityReferenceLinks_WithAndWithoutMetadataTypes_ShouldVerifyLinksWithMetadata(string mimeType, bool shouldVerify)
        {
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri() { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };

            var responseMessage = new TestStreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessage, settings, _model))
            {
                var links = new ODataEntityReferenceLinks()
                {
                    Links = new[]
                    {
                        new ODataEntityReferenceLink() {Url = new Uri(_baseUri + "Orders(-10)")},
                        new ODataEntityReferenceLink() {Url = new Uri(_baseUri + "Orders(-7)")},
                    },
                    NextPageLink = new Uri(_baseUri + "Customers(-10)/Orders/$ref?$skiptoken=-7")
                };

                await messageWriter.WriteEntityReferenceLinksAsync(links);
            }

            if (!shouldVerify) return; // Skip verification for no-metadata MIME type

            // Read and verify the response
            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var readerSettings = new ODataMessageReaderSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            ODataEntityReferenceLinks linksRead = await messageReader.ReadEntityReferenceLinksAsync();

            Assert.Equal(2, linksRead.Links.Count());
            Assert.NotNull(linksRead.NextPageLink);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata, true)]
        [InlineData(MimeTypeODataParameterMinimalMetadata, true)]
        [InlineData(MimeTypeODataParameterNoMetadata, false)]
        public async Task WritingASingleEntityReferenceLink_WithAndWithoutMetadataTypes_LinkShouldBeVerifiedWithMetadata(string mimeType, bool shouldVerify)
        {
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri() { ServiceRoot = _baseUri },
                EnableMessageStreamDisposal = false
            };

            var responseMessage = new TestStreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(responseMessage, settings, _model))
            {
                var link = new ODataEntityReferenceLink() { Url = new Uri(_baseUri + "Orders(-10)") };
                await messageWriter.WriteEntityReferenceLinkAsync(link);
            }

            if (!shouldVerify)
            {
                return;
            }

            // Read and verify the response
            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var readerSettings = new ODataMessageReaderSettings { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            ODataEntityReferenceLink linkRead = await messageReader.ReadEntityReferenceLinkAsync();

            Assert.Equal($"{_baseUri}Orders(-10)", linkRead.Url.AbsoluteUri);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        [InlineData(MimeTypeODataParameterNoMetadata)]
        public async Task WritingAnODataRequestMessage_WithAndWithoutModel_ProducesSameOutput(string mimeType)
        {
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri() { ServiceRoot = _baseUri},
                EnableMessageStreamDisposal = false
            };

            string outputWithModel = null;
            string outputWithoutModel = null;

            var orderType = _model.FindDeclaredType(NameSpacePrefix + "Order") as IEdmEntityType;
            var orderSet = _model.EntityContainer.FindEntitySet("Orders");

            var requestMessageWithModel = new TestStreamRequestMessage(
                new MemoryStream(),
                new Uri(_baseUri + "Orders"), "POST");
            requestMessageWithModel.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(requestMessageWithModel, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(orderSet, orderType);
                outputWithModel = await this.WriteARequestMessageAsync(
                    requestMessageWithModel,
                    odataWriter,
                    true,
                    mimeType);
            }

            var requestMessageWithoutModel = new TestStreamRequestMessage(
                new MemoryStream(),
                new Uri(_baseUri + "Orders"), "POST");
            requestMessageWithoutModel.SetHeader("Content-Type", mimeType);

            using (var messageWriter = new ODataMessageWriter(requestMessageWithoutModel, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync();
                outputWithoutModel = await this.WriteARequestMessageAsync(
                    requestMessageWithoutModel,
                    odataWriter, false, mimeType);
            }

            Assert.Equal(outputWithModel, outputWithoutModel);
        }

        private async Task<string> WriteAnOrderFeedAsync(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            var orderFeed = new ODataResourceSet()
            {
                Id = new Uri(_baseUri + "Orders"),
                NextPageLink = new Uri(_baseUri + "Orders?$skiptoken=-9"),
            };

            if (!hasModel)
            {
                orderFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Orders", NavigationSourceEntityTypeName = NameSpacePrefix + "Order" });
            }

            await odataWriter.WriteStartAsync(orderFeed);

            var orderEntry1 = TestsHelper.CreateOrderEntry1(hasModel);
            await odataWriter.WriteStartAsync(orderEntry1);

            var orderEntry1Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-10)/Customer")
            };

            await odataWriter.WriteStartAsync(orderEntry1Navigation1);
            await odataWriter.WriteEndAsync();

            var orderEntry1Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-10)/Login")
            };

            await odataWriter.WriteStartAsync(orderEntry1Navigation2);
            await odataWriter.WriteEndAsync();

            // Finish writing orderEntry1.
            await odataWriter.WriteEndAsync();

            var orderEntry2Wrapper = TestsHelper.CreateOrderEntry2(hasModel);

            var orderEntry2Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-9)/Customer")
            };

            var orderEntry2Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-9)/Login")
            };

            orderEntry2Wrapper.NestedResourceInfoWrappers = orderEntry2Wrapper.NestedResourceInfoWrappers.Concat(
                new[]
                {
                    new ODataNestedResourceInfoWrapper() { NestedResourceInfo = orderEntry1Navigation1 },
                    new ODataNestedResourceInfoWrapper() { NestedResourceInfo = orderEntry2Navigation2 }
                });

            await ODataWriterHelper.WriteResourceAsync(odataWriter, orderEntry2Wrapper);

            // Finish writing the feed.
            await odataWriter.WriteEndAsync();

            Stream stream = await responseMessage.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
        }

        private async Task<string> WriteAnExpandedCustomerEntryAsync(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            ODataResourceWrapper customerEntry = TestsHelper.CreateCustomerEntry(hasModel);

            var loginFeed = new ODataResourceSet() { Id = new Uri(_baseUri + "Customers(-9)/Logins") };
            if (!hasModel)
            {
                loginFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Logins", NavigationSourceEntityTypeName = NameSpacePrefix + "Login", NavigationSourceKind = EdmNavigationSourceKind.EntitySet });
            }

            var loginEntry = TestsHelper.CreateLoginEntry(hasModel);


            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(TestsHelper.CreateCustomerNavigationLinks());
            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(new[]{  new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Logins",
                    IsCollection = true,
                    Url = new Uri(_baseUri + "Customers(-9)/Logins")
                },
                NestedResourceOrResourceSet = new ODataResourceSetWrapper()
                {
                    ResourceSet = loginFeed,
                    Resources = new List<ODataResourceWrapper>()
                    {
                        new ODataResourceWrapper()
                        {
                            Resource = loginEntry,
                            NestedResourceInfoWrappers = TestsHelper.CreateLoginNavigationLinksWrapper().ToList()
                        }
                    }
                }
            }});

            await ODataWriterHelper.WriteResourceAsync(odataWriter, customerEntry);

            Stream stream = await responseMessage.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
        }

        private async Task<string> WriteACarEntryAsync(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            var carEntry = TestsHelper.CreateCarEntry(hasModel);

            await odataWriter.WriteStartAsync(carEntry);

            // Finish writing the entry.
            await odataWriter.WriteEndAsync();

            Stream stream = await responseMessage.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
        }

        private async Task<string> WriteAPersonFeedAsync(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            var personFeed = new ODataResourceSet()
            {
                Id = new Uri(_baseUri + "People"),
                DeltaLink = new Uri(_baseUri + "People")
            };
            if (!hasModel)
            {
                personFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
            }

            await odataWriter.WriteStartAsync(personFeed);

            ODataResource personEntry = TestsHelper.CreatePersonEntry(hasModel);
            await odataWriter.WriteStartAsync(personEntry);

            var personNavigation = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-5)/PersonMetadata", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(personNavigation);
            await odataWriter.WriteEndAsync();

            // Finish writing personEntry.
            await odataWriter.WriteEndAsync();

            ODataResource employeeEntry = TestsHelper.CreateEmployeeEntry(hasModel);
            await odataWriter.WriteStartAsync(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(employeeNavigation1);
            await odataWriter.WriteEndAsync();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/Manager", UriKind.Relative)
            };

            await odataWriter.WriteStartAsync(employeeNavigation2);
            await odataWriter.WriteEndAsync();

            // Finish writing employeeEntry.
            await odataWriter.WriteEndAsync();

            ODataResource specialEmployeeEntry = TestsHelper.CreateSpecialEmployeeEntry(hasModel);
            await odataWriter.WriteStartAsync(specialEmployeeEntry);

            var specialEmployeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-10)/" + NameSpacePrefix + "SpecialEmployee" + "/PersonMetadata", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(specialEmployeeNavigation1);
            await odataWriter.WriteEndAsync();

            var specialEmployeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("People(-10)/" + NameSpacePrefix + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(specialEmployeeNavigation2);
            await odataWriter.WriteEndAsync();

            var specialEmployeeNavigation3 = new ODataNestedResourceInfo()
            {
                Name = "Car",
                IsCollection = false,
                Url = new Uri("People(-10)/" + NameSpacePrefix + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(specialEmployeeNavigation3);
            await odataWriter.WriteEndAsync();

            // Finish writing specialEmployeeEntry.
            await odataWriter.WriteEndAsync();

            // Finish writing the feed.
            await odataWriter.WriteEndAsync();
            Stream stream = await responseMessage.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
        }

        private async Task<string> WriteAnEmployeeEntryAsync(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasExpectedType,
            string mimeType)
        {
            ODataResource employeeEntry = TestsHelper.CreateEmployeeEntry(false);
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo()
            {
                NavigationSourceName = "People",
                NavigationSourceEntityTypeName = NameSpacePrefix + "Person",
            };

            if (hasExpectedType)
            {
                serializationInfo.ExpectedTypeName = NameSpacePrefix + "Employee";
            }

            employeeEntry.SetSerializationInfo(serializationInfo);
            await odataWriter.WriteStartAsync(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(employeeNavigation1);
            await odataWriter.WriteEndAsync();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/Manager", UriKind.Relative)
            };
            await odataWriter.WriteStartAsync(employeeNavigation2);
            await odataWriter.WriteEndAsync();

            // Finish writing employeeEntry.
            await odataWriter.WriteEndAsync();

            Stream stream = await responseMessage.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
        }

        private async Task<string> WriteACollectionAsync(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            var resourceSet = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet
                {
                    Count = 12,
                    NextPageLink = new Uri("http://localhost")
                },
                Resources = new List<ODataResourceWrapper>()
                {
                    TestsHelper.CreatePrimaryContactODataWrapper()
                }
            };

            if (!hasModel)
            {
                resourceSet.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo()
                {
                    ExpectedTypeName = NameSpacePrefix + "ContactDetails"
                });
            }

            await ODataWriterHelper.WriteResourceSetAsync(odataWriter, resourceSet);
            Stream stream = await responseMessage.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
        }

        private async Task<string> WriteARequestMessageAsync(
            TestStreamRequestMessage requestMessageWithoutModel,
            ODataWriter odataWriter,
            bool hasModel, 
            string mimeType)
        {
            var order = new ODataResource()
            {
                Id = new Uri(_baseUri + "Orders(-10)"),
                TypeName = NameSpacePrefix + "Order"
            };

            var orderP1 = new ODataProperty { Name = "OrderId", Value = -10 };
            var orderp2 = new ODataProperty { Name = "CustomerId", Value = 8212 };
            var orderp3 = new ODataProperty { Name = "Concurrency", Value = null };
            order.Properties = new[] { orderP1, orderp2, orderp3 };

            if (!hasModel)
            {
                order.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Orders", NavigationSourceEntityTypeName = NameSpacePrefix + "Order" });
                orderP1.SetSerializationInfo(new ODataPropertySerializationInfo() { PropertyKind = ODataPropertyKind.Key });
            }

            await odataWriter.WriteStartAsync(order);
            await odataWriter.WriteEndAsync();

            var orderType = _model.FindDeclaredType(NameSpacePrefix + "Order") as IEdmEntityType;
            var orderSet = _model.EntityContainer.FindEntitySet("Orders");
            Stream stream = await requestMessageWithoutModel.GetStreamAsync();

            return await TestsHelper.ReadStreamContentAsync(stream);
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
