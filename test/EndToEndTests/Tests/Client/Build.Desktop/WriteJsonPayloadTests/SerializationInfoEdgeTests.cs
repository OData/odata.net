//---------------------------------------------------------------------
// <copyright file="SerializationInfoEdgeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit.Abstractions;
    using Xunit;

    public class SerializationInfoEdgeTests : WritePayloadTests
    {
        public SerializationInfoEdgeTests(ITestOutputHelper helper)
            : base(helper)
        {
        }
        protected List<string> testMimeTypes = new List<string>()
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

        /// <summary>
        /// Write payloads without model or serialization info.
        /// Note that ODL will succeed if writing requests or writing nometadata reponses.
        /// </summary>
        [Fact]
        public void WriteWithoutSerializationInfo()
        {
            foreach (var mimeType in this.testMimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                // write entry without serialization info
                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();
                    var expectedError = mimeType.Contains(MimeTypes.ODataParameterNoMetadata)
                                               ? null
                                               : "ODataResourceTypeContext_MetadataOrSerializationInfoMissing";
                    AssertThrows<ODataException>(() => odataWriter.WriteStart(entry), expectedError);
                }

                // write feed without serialization info
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    var feed = this.CreatePersonFeed();
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();
                    entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });
                    var expectedError = mimeType.Contains(MimeTypes.ODataParameterNoMetadata)
                                               ? null
                                               : "ODataResourceTypeContext_MetadataOrSerializationInfoMissing";
                    AssertThrows<ODataException>(() => odataWriter.WriteStart(feed), expectedError);
                }

                // write collection without serialization info
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataCollectionWriter();
                    var collectionStart = new ODataCollectionStart() { Name = "BackupContactInfo" };
                    var expectedError = mimeType.Contains(MimeTypes.ODataParameterNoMetadata)
                                               ? null
                                               : "ODataContextUriBuilder_TypeNameMissingForTopLevelCollection";
                    AssertThrows<ODataException>(() => odataWriter.WriteStart(collectionStart), expectedError);
                }

                // write a reference link without serialization info
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var link = new ODataEntityReferenceLink() { Url = new Uri(this.ServiceUri + "Order(-10)") };
                    messageWriter.WriteEntityReferenceLink(link);

                    // No exception is expected. Simply verify the writing succeeded.
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        Stream stream = responseMessageWithoutModel.GetStream();
                        Assert.True(WritePayloadHelper.ReadStreamContent(stream).Contains("$metadata#$ref"));
                    }
                }

                // write reference links without serialization info
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var links = new ODataEntityReferenceLinks()
                    {
                        Links = new[]
                                {
                                    new ODataEntityReferenceLink(){Url = new Uri(this.ServiceUri + "Order(-10)")}
                                },
                    };
                    messageWriter.WriteEntityReferenceLinks(links);

                    // No exception is expected. Simply verify the writing succeeded.
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        Stream stream = responseMessageWithoutModel.GetStream();
                        Assert.True(WritePayloadHelper.ReadStreamContent(stream).Contains("$metadata#Collection($ref)"));
                    }

                }

                // write request message containing an entry
                var requestMessageWithoutModel = new StreamRequestMessage(new MemoryStream(),
                                                                          new Uri(this.ServiceUri + "Person"), "POST");
                requestMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(requestMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                    Stream stream = requestMessageWithoutModel.GetStream();
                    Assert.Contains("Person(-5)\",\"PersonId\":-5,\"Name\":\"xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu\"", WritePayloadHelper.ReadStreamContent(stream));
                }
            }
        }

        /// <summary>
        /// Specify serialization info for both feed and entry should fail.
        /// </summary>
        [Fact]
        public void SpecifySerializationInfoForFeedAndEntry()
        {
            foreach (var mimeType in this.testMimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    var feed = this.CreatePersonFeed();
                    feed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });

                    var entry = new ODataResource()
                    {
                        Id = new Uri(ServiceUri + "Person(-5)"),
                        TypeName = NameSpace + "Employee"
                    };

                    var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
                    var personEntryP2 = new ODataProperty
                    {
                        Name = "Name",
                        Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
                    };

                    var personEntryP3 = new ODataProperty { Name = "ManagersPersonId", Value = -465010984 };

                    entry.Properties = new[] { personEntryP1, personEntryP2, personEntryP3 };
                    entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });

                    odataWriter.WriteStart(feed);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                    odataWriter.WriteEnd();
                    Stream stream = responseMessageWithoutModel.GetStream();
                    string result = WritePayloadHelper.ReadStreamContent(stream);
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        Assert.True(result.Contains(NameSpace + "Employee"));
                        Assert.True(result.Contains("$metadata#Person"));
                    }
                    else
                    {
                        Assert.False(result.Contains(NameSpace + "Employee"));
                    }
                }
            }
        }

        /// <summary>
        /// Write response payload with serialization info containing wrong values
        /// </summary>
        [Fact]
        public void WriteEntryWithWrongSerializationInfo()
        {
            foreach (var mimeType in this.testMimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                // wrong EntitySetName for entry
                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();

                    entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpace + "Person" });
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                    var result = WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                    Assert.True(result.Contains("\"PersonId\":-5"));
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // no metadata does not write odata.metadata
                        Assert.True(result.Contains("$metadata#Parsen/$entity"));
                        Assert.True(result.Contains("Person(-5)\",\"PersonId\":-5"));
                    }
                    else
                    {
                        Assert.False(result.Contains("Person(-5)\",\"PersonId\":-5"));
                    }
                }

                // wrong EntitySetName for feed
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();

                    var feed = this.CreatePersonFeed();
                    feed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpace + "Person" });
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();
                    odataWriter.WriteStart(feed);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                    odataWriter.WriteEnd();
                    var result = WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                    Assert.Contains("\"PersonId\":-5", result);
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // no metadata does not write odata.metadata
                        Assert.Contains("$metadata#Parsen\"", result);
                        Assert.Contains("Person(-5)\",\"PersonId\":-5", result);
                    }
                    else
                    {
                        Assert.DoesNotContain("Person(-5)\",\"PersonId\":-5", result);
                    }
                }

                // wrong complex collection type name
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    var complexCollection = new ODataResourceSetWrapper()
                    {
                        ResourceSet = new ODataResourceSet(),
                        Resources = new List<ODataResourceWrapper>()
                        {
                            WritePayloadHelper.CreatePrimaryContactODataWrapper()
                        }
                    };

                    complexCollection.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo()
                    {
                        ExpectedTypeName = NameSpace + "ContactDETAIL"
                    });
                    ODataWriterHelper.WriteResourceSet(odataWriter, complexCollection);
                    var result = WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // [{"@odata.type":"#Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails","EmailBag@odata.type":"#Collection(String)"...
                        Assert.True(result.Contains("\"value\":[{\"@odata.type\":\"#Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails\",\"EmailBag"));

                        // no metadata does not write odata.metadata
                        Assert.True(result.Contains("$metadata#Collection(" + NameSpace + "ContactDETAIL)"));
                    }
                    else
                    {
                        Assert.False(result.Contains("\"@odata.type\":\"#Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails\""));
                    }
                }

                // The following two cases of wrong reference link info is no longer active. The ODataEntityReferenceLinkSerializationInfo is ignored when writer writes the payload.
                // Consider removing them.

                // wrong reference link info
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var link = new ODataEntityReferenceLink() { Url = new Uri(this.ServiceUri + "Order(-10)") };
                    messageWriter.WriteEntityReferenceLink(link);
                    var result = WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                    Assert.True(result.Contains(this.ServiceUri + "Order(-10)"));
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // no metadata does not write odata.context
                        Assert.True(result.Contains("$metadata#$ref"));
                    }
                }

                // wrong reference links info
                responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var links = new ODataEntityReferenceLinks()
                    {
                        Links = new[]
                                {
                                    new ODataEntityReferenceLink(){Url = new Uri(this.ServiceUri + "Order(-10)")}
                                },
                    };

                    messageWriter.WriteEntityReferenceLinks(links);
                    var result = WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                    Assert.True(result.Contains(this.ServiceUri + "Order(-10)"));
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // no metadata does not write odata.metadata
                        Assert.True(result.Contains("$metadata#Collection($ref)"));
                    }
                }
            }
        }

        /// <summary>
        /// Provide EDM model, and then write an entry with wrong serialization info
        /// </summary>
        [Fact]
        public void WriteEntryWithWrongSerializationInfoWithModel()
        {
            bool[] autoComputeMetadataBools = new bool[] { true, false, };
            foreach (var mimeType in this.testMimeTypes)
            {
                foreach (var autoComputeMetadata in autoComputeMetadataBools)
                {
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                    var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                    responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                    using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings, WritePayloadHelper.Model))
                    {
                        var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType);
                        var entry = this.CreatePersonEntryWithoutSerializationInfo();

                        entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpace + "Person" });
                        odataWriter.WriteStart(entry);
                        odataWriter.WriteEnd();
                        var result = WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream());
                        Assert.True(result.Contains("\"PersonId\":-5"));
                        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                        {
                            // no metadata does not write odata.metadata
                            Assert.True(result.Contains("$metadata#Parsen/$entity"));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Change serialization info value after WriteStart
        /// </summary>
        [Fact]
        public void ChangeSerializationInfoAfterWriteStart()
        {
            foreach (var mimeType in this.testMimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    var entry = this.CreatePersonEntryWithoutSerializationInfo();

                    entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });
                    odataWriter.WriteStart(entry);
                    entry.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Parsen", NavigationSourceEntityTypeName = NameSpace + "Person" });
                    odataWriter.WriteEnd();

                    Stream stream = responseMessageWithoutModel.GetStream();

                    // nometadata option does not write odata.metadata in the payload
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        Assert.True(WritePayloadHelper.ReadStreamContent(stream).Contains("Person/$entity"));
                    }
                }
            }
        }

        /// <summary>
        /// Do not provide model, but hand craft IEdmEntitySet and IEdmEntityType for writer
        /// </summary>
        [Fact]
        public void HandCraftEdmType()
        {
            foreach (var mimeType in this.testMimeTypes)
            {
                EdmModel edmModel = new EdmModel();

                EdmEntityType edmEntityType = new EdmEntityType(NameSpace, "Person");
                edmModel.AddElement(edmEntityType);
                var keyProperty = new EdmStructuralProperty(edmEntityType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
                edmEntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
                edmEntityType.AddProperty(keyProperty);
                var property = new EdmStructuralProperty(edmEntityType, "Name", EdmCoreModel.Instance.GetString(true));
                edmEntityType.AddKeys(new IEdmStructuralProperty[] { property });
                edmEntityType.AddProperty(property);

                var defaultContainer = new EdmEntityContainer(NameSpace, "DefaultContainer");
                edmModel.AddElement(defaultContainer);

                EdmEntitySet entitySet = new EdmEntitySet(defaultContainer, "Person", edmEntityType);
                defaultContainer.AddElement(entitySet);

                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
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
                            WritePayloadHelper.ReadStreamContent(responseMessageWithoutModel.GetStream())
                                .Contains("Person/$entity"));
                    }
                }
            }
        }

        private ODataResource CreatePersonEntryWithoutSerializationInfo()
        {
            var personEntry = new ODataResource()
            {
                Id = new Uri(ServiceUri + "Person(-5)"),
                TypeName = NameSpace + "Person"
            };

            var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
            var personEntryP2 = new ODataProperty
            {
                Name = "Name",
                Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
            };

            personEntry.Properties = new[] { personEntryP1, personEntryP2 };
            personEntry.EditLink = new Uri(ServiceUri + "Person(-5)");
            return personEntry;
        }

        private ODataResourceSet CreatePersonFeed()
        {
            var orderFeed = new ODataResourceSet()
            {
                Id = new Uri(this.ServiceUri + "Person"),
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
                StringResourceUtil.VerifyODataLibString(e.Message, expectedError, false /* isExactMatch */);
            }
        }
    }
}
