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

        public static IEnumerable<object[]> MimeTypesData
        {
            get
            {
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
            }
        }

        public SerializationInfoEdgeTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _model = CommonEndToEndEdmModel.GetEdmModel();
        }

        /// <summary>
        /// Write payloads without model or serialization info.
        /// Note that ODL will succeed if writing requests or writing nometadata reponses.
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void WriteWithoutSerializationInfo(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            // write entry without serialization info
            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                var expectedError = mimeType.Contains(MimeTypes.ODataParameterNoMetadata)
                                           ? null
                                           : Error.Format(SRResources.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
                AssertThrows<ODataException>(() => odataWriter.WriteStart(entry), expectedError);
            }

            // write feed without serialization info
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter();
                var feed = this.CreatePersonFeed();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
                var expectedError = mimeType.Contains(MimeTypes.ODataParameterNoMetadata)
                                           ? null
                                           : Error.Format(SRResources.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
                AssertThrows<ODataException>(() => odataWriter.WriteStart(feed), expectedError);
            }

            // write collection without serialization info
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataCollectionWriter();
                var collectionStart = new ODataCollectionStart() { Name = "BackupContactInfo" };
                var expectedError = mimeType.Contains(MimeTypes.ODataParameterNoMetadata)
                                           ? null
                                           : Error.Format(SRResources.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
                AssertThrows<ODataException>(() => odataWriter.WriteStart(collectionStart), expectedError);
            }

            // write a reference link without serialization info
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var link = new ODataEntityReferenceLink() { Url = new Uri(_baseUri + "Orders(-10)") };
                messageWriter.WriteEntityReferenceLink(link);

                // No exception is expected. Simply verify the writing succeeded.
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Stream stream = responseMessageWithoutModel.GetStream();
                    Assert.True(TestsHelper.ReadStreamContent(stream).Contains("$metadata#$ref"));
                }
            }

            // write reference links without serialization info
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var links = new ODataEntityReferenceLinks()
                {
                    Links = new[]
                            {
                                    new ODataEntityReferenceLink(){Url = new Uri(_baseUri + "Orders(-10)")}
                                },
                };
                messageWriter.WriteEntityReferenceLinks(links);

                // No exception is expected. Simply verify the writing succeeded.
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Stream stream = responseMessageWithoutModel.GetStream();
                    Assert.True(TestsHelper.ReadStreamContent(stream).Contains("$metadata#Collection($ref)"));
                }
            }

            // write request message containing an entry
            var requestMessageWithoutModel = new TestStreamRequestMessage(new MemoryStream(),
                                                                      new Uri(_baseUri + "People"), "POST");
            requestMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(requestMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
                Stream stream = requestMessageWithoutModel.GetStream();
                Assert.Contains("People(-5)\",\"PersonId\":-5,\"Name\":\"xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu\"", TestsHelper.ReadStreamContent(stream));
            }
        }

        /// <summary>
        /// Specify serialization info for both feed and entry should fail.
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void SpecifySerializationInfoForFeedAndEntry(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter();
                var feed = this.CreatePersonFeed();
                feed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });

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

                var personEntryP3 = new ODataProperty { Name = "ManagersPersonId", Value = -465010984 };

                entry.Properties = new[] { personEntryP1, personEntryP2, personEntryP3 };
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });

                odataWriter.WriteStart(feed);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
                Stream stream = responseMessageWithoutModel.GetStream();
                string result = TestsHelper.ReadStreamContent(stream);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.True(result.Contains(NameSpacePrefix + "Employee"));
                    Assert.True(result.Contains("$metadata#People"));
                }
                else
                {
                    Assert.False(result.Contains(NameSpacePrefix + "Employee"));
                }
            }
        }

        /// <summary>
        /// Write response payload with serialization info containing wrong values
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void WriteEntryWithWrongSerializationInfo(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            // wrong EntitySetName for entry
            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();

                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
                var result = TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                Assert.True(result.Contains("\"PersonId\":-5"));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    // no metadata does not write odata.metadata
                    Assert.True(result.Contains("$metadata#Parsen/$entity"));
                    Assert.True(result.Contains("People(-5)\",\"PersonId\":-5"));
                }
                else
                {
                    Assert.False(result.Contains("People(-5)\",\"PersonId\":-5"));
                }
            }

            // wrong EntitySetName for feed
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter();

                var feed = this.CreatePersonFeed();
                feed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                odataWriter.WriteStart(feed);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
                var result = TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                Assert.Contains("\"PersonId\":-5", result);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    // no metadata does not write odata.metadata
                    Assert.Contains("$metadata#Parsen\"", result);
                    Assert.Contains("People(-5)\",\"PersonId\":-5", result);
                }
                else
                {
                    Assert.DoesNotContain("People(-5)\",\"PersonId\":-5", result);
                }
            }

            // wrong complex collection type name
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter();
                var complexCollection = new ODataResourceSetWrapper()
                {
                    ResourceSet = new ODataResourceSet(),
                    Resources = new List<ODataResourceWrapper>()
                        {
                            TestsHelper.CreatePrimaryContactODataWrapper()
                        }
                };

                complexCollection.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo()
                {
                    ExpectedTypeName = NameSpacePrefix + "ContactDETAIL"
                });
                ODataWriterHelper.WriteResourceSet(odataWriter, complexCollection);
                var result = TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    // [{"@odata.type":"#Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails","EmailBag@odata.type":"#Collection(String)"...
                    Assert.True(result.Contains("\"value\":[{\"@odata.type\":\"#Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.ContactDetails\",\"EmailBag"));

                    // no metadata does not write odata.metadata
                    Assert.True(result.Contains("$metadata#Collection(" + NameSpacePrefix + "ContactDETAIL)"));
                }
                else
                {
                    Assert.False(result.Contains("\"@odata.type\":\"#Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.ContactDetails\""));
                }
            }

            // The following two cases of wrong reference link info is no longer active. The ODataEntityReferenceLinkSerializationInfo is ignored when writer writes the payload.
            // Consider removing them.

            // wrong reference link info
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var link = new ODataEntityReferenceLink() { Url = new Uri(_baseUri + "Orders(-10)") };
                messageWriter.WriteEntityReferenceLink(link);
                var result = TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                Assert.True(result.Contains(_baseUri + "Orders(-10)"));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    // no metadata does not write odata.context
                    Assert.True(result.Contains("$metadata#$ref"));
                }
            }

            // wrong reference links info
            responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var links = new ODataEntityReferenceLinks()
                {
                    Links = new[]
                            {
                                    new ODataEntityReferenceLink(){Url = new Uri(_baseUri + "Orders(-10)")}
                                },
                };

                messageWriter.WriteEntityReferenceLinks(links);
                var result = TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                Assert.True(result.Contains(_baseUri + "Orders(-10)"));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    // no metadata does not write odata.metadata
                    Assert.True(result.Contains("$metadata#Collection($ref)"));
                }
            }
        }

        /// <summary>
        /// Provide EDM model, and then write an entry with wrong serialization info
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void WriteEntryWithWrongSerializationInfoWithModel(string mimeType)
        {
            bool[] autoComputeMetadataBools = new bool[] { true, false, };

            foreach (var autoComputeMetadata in autoComputeMetadataBools)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

                var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);

                var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
                var peopleSet = _model.EntityContainer.FindEntitySet("People");

                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings, _model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(peopleSet, personType);
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();

                    entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                    var result = TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                    Assert.True(result.Contains("\"PersonId\":-5"));
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // no metadata does not write odata.metadata
                        Assert.True(result.Contains("$metadata#Parsen/$entity"));
                    }
                }
            }
        }

        /// <summary>
        /// Change serialization info value after WriteStart
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void ChangeSerializationInfoAfterWriteStart(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                var entry = this.CreatePersonEntryWithoutSerializationInfo();

                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
                odataWriter.WriteStart(entry);
                entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
                odataWriter.WriteEnd();

                Stream stream = responseMessageWithoutModel.GetStream();

                // nometadata option does not write odata.metadata in the payload
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.True(TestsHelper.ReadStreamContent(stream).Contains("People/$entity"));
                }
            }
        }

        /// <summary>
        /// Do not provide model, but hand craft IEdmEntitySet and IEdmEntityType for writer
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void HandCraftEdmType(string mimeType)
        {
            EdmModel edmModel = new EdmModel();

            EdmEntityType edmEntityType = new EdmEntityType(NameSpacePrefix, "Person");
            edmModel.AddElement(edmEntityType);
            var keyProperty = new EdmStructuralProperty(edmEntityType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            edmEntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            edmEntityType.AddProperty(keyProperty);
            var property = new EdmStructuralProperty(edmEntityType, "Name", EdmCoreModel.Instance.GetString(true));
            edmEntityType.AddKeys(new IEdmStructuralProperty[] { property });
            edmEntityType.AddProperty(property);

            var defaultContainer = new EdmEntityContainer(NameSpacePrefix, "DefaultContainer");
            edmModel.AddElement(defaultContainer);

            EdmEntitySet entitySet = new EdmEntitySet(defaultContainer, "People", edmEntityType);
            defaultContainer.AddElement(entitySet);

            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };
            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(entitySet, edmEntityType);

                var entry = this.CreatePersonEntryWithoutSerializationInfo();
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.True(
                        TestsHelper.ReadStreamContent(responseMessageWithoutModel.GetStream())
                            .Contains("People/$entity"));
                }
            }
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

        private static void AssertThrows<T>(Action action, string expectedError) where T : Exception
        {
            try
            {
                action();
                //Expected exception not thrown
                Assert.Null(expectedError);
            }
            catch (T e)
            {
                //Unexpected expection " + e.Message
                Assert.NotNull(expectedError);
                Assert.Equal(expectedError, e.Message);
            }
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
