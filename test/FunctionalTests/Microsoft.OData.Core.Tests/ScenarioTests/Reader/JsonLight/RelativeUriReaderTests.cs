//---------------------------------------------------------------------
// <copyright file="RelativeUriReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Reader.JsonLight
{
    public class RelativeUriReaderTests
    {
        private readonly ITestOutputHelper output;

        private const string NS = "SampleService";
        private const string ServiceRoot = "http://www.example.com/SampleService/";
        private static readonly Uri ServiceDocumentUri = new Uri(ServiceRoot);

        #region variable

        private EdmModel model;
        private EdmEntityType et1;
        private EdmEntityType derivedET1;
        private EdmComplexType ct1;
        private EdmComplexType derivedCT1;
        private EdmEntityType et2;
        private EdmEntityType et3;

        private EdmEntitySet et1Set;
        private EdmEntitySet et2Set;
        private EdmSingleton et2Singleton;

        private EdmNavigationProperty et1CNav1;
        private EdmNavigationProperty et1CNav2;

        #endregion variable

        public RelativeUriReaderTests(ITestOutputHelper output)
        {
            this.output = output;

            #region GenerateModel

            model = new EdmModel();

            ct1 = new EdmComplexType(NS, "CT1");
            ct1.AddStructuralProperty("CT1P1", EdmCoreModel.Instance.GetString(false));
            model.AddElement(ct1);

            derivedCT1 = new EdmComplexType(NS, "DerivedCT1", ct1);
            derivedCT1.AddStructuralProperty("DerivedCT1P1", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(derivedCT1);

            et1 = new EdmEntityType(NS, "ET1");
            var et1key = new EdmStructuralProperty(et1, "ET1Key", EdmCoreModel.Instance.GetInt32(false));
            et1.AddProperty(et1key);
            et1.AddKeys(et1key);
            et1.AddStructuralProperty("ET1P1", EdmCoreModel.Instance.GetString(false));
            model.AddElement(et1);

            derivedET1 = new EdmEntityType(NS, "DerivedET1", et1);
            derivedET1.AddStructuralProperty("DerivedET1P1", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(derivedET1);

            et2 = new EdmEntityType(NS, "ET2");
            var et2key = new EdmStructuralProperty(et2, "ET2Key", EdmCoreModel.Instance.GetInt32(false));
            et2.AddProperty(et2key);
            et2.AddKeys(et2key);
            et2.AddStructuralProperty("ET2P1", EdmCoreModel.Instance.GetString(false));
            model.AddElement(et2);

            et3 = new EdmEntityType(NS, "ET3");
            var et3key = new EdmStructuralProperty(et3, "ET3Key", EdmCoreModel.Instance.GetInt32(false));
            et3.AddProperty(et3key);
            et3.AddKeys(et3key);
            et3.AddStructuralProperty("ET3P1", EdmCoreModel.Instance.GetString(false));
            model.AddElement(et3);

            var container = new EdmEntityContainer(NS, "SampleService");
            this.model.AddElement(container);

            et1Set = new EdmEntitySet(container, "ET1Set", et1);
            container.AddElement(et1Set);

            et2Set = new EdmEntitySet(container, "ET2Set", et2);
            container.AddElement(et2Set);

            et2Singleton = new EdmSingleton(container, "ET2Singleton", et2);
            container.AddElement(et2Singleton);

            var et1Nav1Info = new EdmNavigationPropertyInfo()
            {
                Name = "ET1Nav1",
                ContainsTarget = false,
                Target = et2,
                TargetMultiplicity = Edm.EdmMultiplicity.Many
            };

            var et1Nav1 = et1.AddUnidirectionalNavigation(et1Nav1Info);
            et1Set.AddNavigationTarget(et1Nav1, et2Set);

            var et1Nav2Info = new EdmNavigationPropertyInfo()
            {
                Name = "ET1Nav2",
                ContainsTarget = false,
                Target = et2,
                TargetMultiplicity = Edm.EdmMultiplicity.One
            };

            var et1Nav2 = et1.AddUnidirectionalNavigation(et1Nav2Info);
            et1Set.AddNavigationTarget(et1Nav2, et2Singleton);

            var et1CNav1Info = new EdmNavigationPropertyInfo()
            {
                Name = "ET1CNav1",
                ContainsTarget = true,
                Target = et3,
                TargetMultiplicity = Edm.EdmMultiplicity.Many
            };

            et1CNav1 = et1.AddUnidirectionalNavigation(et1CNav1Info);

            var et1CNav2Info = new EdmNavigationPropertyInfo()
            {
                Name = "ET1CNav2",
                ContainsTarget = true,
                Target = et3,
                TargetMultiplicity = Edm.EdmMultiplicity.One
            };

            et1CNav2 = et1.AddUnidirectionalNavigation(et1CNav2Info);

            #endregion
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public void RelativeContextUrl_ServiceDocument_RelativeUrlForEntitySet(bool startWithSlash, bool relativeUriForEntitySet)
        {
            string contentType;

            var payload = this.WritePayload(this.model, omWriter =>
            {
                ODataServiceDocument serviceDocument = new ODataServiceDocument()
                {
                    EntitySets = new[] { new ODataEntitySetInfo { Name = "ET1Set", Url = new Uri(ServiceDocumentUri, "ET1Set") } }
                };
                omWriter.WriteServiceDocument(serviceDocument);
            }, null, out contentType);

            // relativeUriForEntitySet == true means that the url for entity set in payload will also be changed to relative uri
            string FixReplacementFormat = relativeUriForEntitySet ? "{0}" : "@odata.context\":\"{0}";
            payload = startWithSlash
                ? payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, "/SampleService/"))
                : payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, ""));

            output.WriteLine(payload);

            ODataServiceDocument serviceDocumentResult = null;
            this.ReadPayload(payload, contentType, model, omReader => serviceDocumentResult = omReader.ReadServiceDocument(), ServiceDocumentUri);

            var entitySets = serviceDocumentResult.EntitySets.ToList();
            Assert.Equal(1, entitySets.Count);
            Assert.Equal(new Uri(ServiceDocumentUri, "ET1Set"), entitySets[0].Url);
        }

        // Test ReadPayloadStart with request Uri and relative Uri
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RelativeContextUrl_ResourceSet(bool startWithSlash)
        {
            string contentType;

            var payload = this.WritePayload(this.model, omWriter =>
            {
                var odataWriter = omWriter.CreateODataResourceSetWriter(et1Set, et1);

                ODataResourceSet resourceSet = new ODataResourceSet();
                ODataResource resource = CreateResource(1);

                odataWriter.WriteStart(resourceSet);
                odataWriter.WriteStart(resource);
                odataWriter.WriteEnd();
                odataWriter.WriteEnd();

            }, null, out contentType);

            string FixReplacementFormat = "@odata.context\":\"{0}";
            payload = startWithSlash
                ? payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, "/SampleService/"))
                : payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, ""));

            output.WriteLine(payload);

            this.ReadPayload(payload, contentType, model, omReader =>
            {
                var odataReader = omReader.CreateODataResourceSetReader();
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(odataReader.Item as ODataResourceSet);
                    }
                }
            }, new Uri(ServiceDocumentUri, et1Set.Name));
        }

        // Test ReadPayloadStart with request Uri and relative Uri
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RelativeContextUrl_Entry_Expand(bool startWithSlash)
        {
            var payload = @"
{
    ""@odata.context"":""http://www.example.com/SampleService/$metadata#ET1Set/$entity"",
    ""ET1Key"":1,
    ""ET1P1"":""P1"",
    ""ET1Nav1"":
    [
        {
            ""@odata.context"":""http://www.example.com/SampleService/$metadata#ET1Set(1)/ET1Nav1/$entity"",
            ""ET2Key"":1,
            ""ET2P1"":""P1""
        }
    ]
}";

            string FixReplacementFormat = "@odata.context\":\"{0}";
            payload = startWithSlash
                ? payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, "/SampleService/"))
                : payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, ""));

            output.WriteLine(payload);

            this.ReadPayload(payload, "application/json", model, omReader =>
            {
                var odataReader = omReader.CreateODataResourceReader();
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        Assert.NotNull(odataReader.Item as ODataResource);
                    }
                }
            }, new Uri(ServiceDocumentUri, "ET1Set(1)?$expand=ET1Nav1"));
        }

        // Test StartNavigationLink with request Uri
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RelativeContextUrl_ContainedNavigation(bool startWithSlash)
        {
            string contentType;

            Uri requestUri = new Uri(ServiceDocumentUri, et1Set.Name + "(1)");

            var payload = this.WritePayload(this.model, omWriter =>
            {
                var odataWriter = omWriter.CreateODataResourceWriter(et1Set, et1);

                ODataResource et1Entry = CreateResource(1);
                ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo()
                {
                    IsCollection = true,
                    Name = "ET1CNav1",
                };

                ODataResourceSet resourceSet = new ODataResourceSet();
                ODataResource et3Entry = CreateResource(3);

                odataWriter.WriteStart(et1Entry);
                odataWriter.WriteStart(navigationLink);
                odataWriter.WriteStart(resourceSet);
                odataWriter.WriteStart(et3Entry);
                odataWriter.WriteEnd(); // end et3Entry
                odataWriter.WriteEnd(); // end resourceSet
                odataWriter.WriteEnd(); // end navigationLink
                odataWriter.WriteEnd(); // end et1Entry

            }, requestUri, out contentType);

            string FixReplacementFormat = "@odata.context\":\"{0}";
            payload = startWithSlash
                ? payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, "/SampleService/"))
                : payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, ""));

            output.WriteLine(payload);

            this.ReadPayload(payload, contentType, model, omReader =>
            {
                var odataReader = omReader.CreateODataResourceReader();
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        var navLink = (odataReader.Item as ODataNestedResourceInfo);
                        if (navLink != null && navLink.Name.Equals("ET1CNav1"))
                        {
                            Assert.Equal(new Uri(requestUri, "ET1Set(1)/ET1CNav1").OriginalString, navLink.Url.OriginalString);
                        }
                    }
                }
            }, requestUri);
        }

        // Test StartNavigationLink with request Uri and single navigation link.
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RelativeContextUrl_ContainedNavigation_SingleNav(bool startWithSlash)
        {
            string contentType;

            Uri requestUri = new Uri(ServiceDocumentUri, et1Set.Name + "(1)");

            var payload = this.WritePayload(this.model, omWriter =>
            {
                var odataWriter = omWriter.CreateODataResourceWriter(et1Set, et1);

                ODataResource entry = CreateResource(1);

                ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo()
                {
                    IsCollection = false,
                    Name = "ET1CNav2",
                };

                ODataResource et3Entry = CreateResource(3);

                odataWriter.WriteStart(entry);
                odataWriter.WriteStart(navigationLink);
                odataWriter.WriteStart(et3Entry);
                odataWriter.WriteEnd(); // end et3Entry
                odataWriter.WriteEnd(); // end navigationLink
                odataWriter.WriteEnd(); // end entry

            }, requestUri, out contentType);

            string FixReplacementFormat = "@odata.context\":\"{0}";
            payload = startWithSlash
                ? payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, "/SampleService/"))
                : payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, ""));

            output.WriteLine(payload);

            this.ReadPayload(payload, contentType, model, omReader =>
            {
                var odataReader = omReader.CreateODataResourceReader();
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        var navLink = (odataReader.Item as ODataNestedResourceInfo);
                        if (navLink != null && navLink.Name.Equals("ET1CNav2"))
                        {
                            Assert.Equal(new Uri(requestUri, "ET1Set(1)/ET1CNav2").OriginalString,
                                navLink.Url.OriginalString);
                        }
                    }
                }
            }, requestUri);
        }

        // Test ReadDeltaStart with request Uri and relative Context Uri
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RelativeContextUrl_Delta_ContainedNavigation(bool startWithSlash)
        {
            string contentType;

            Uri requestUri = new Uri(ServiceDocumentUri, et1Set.Name + "(1)");

            var payload = this.WritePayload(this.model, omWriter =>
            {
                var deltaWriter = omWriter.CreateODataDeltaWriter(et1Set, et1);

                var deltaResourceSet = new ODataDeltaResourceSet();
                ODataResource deltaEntry = CreateResource(1, new Uri(ServiceDocumentUri, "ET1Set(1)"));

                var deletedEntry = new ODataDeltaDeletedEntry(
                    new Uri(ServiceDocumentUri, "ET1Set(2)/ET1CNav1(1)").AbsoluteUri,
                    DeltaDeletedEntryReason.Deleted);
                deletedEntry.SetSerializationInfo(new ODataDeltaSerializationInfo()
                {
                    NavigationSourceName = "ET1Set(2)/ET1CNav1"
                });

                var deletedLink = new ODataDeltaDeletedLink(
                    new Uri(ServiceDocumentUri, "ET1Set(2))"),
                    new Uri(ServiceDocumentUri, "ET1Set(2)/ET1CNav1(1)"),
                    "ET1CNav1");

                ODataResource et3Entry = CreateResource(3, new Uri(ServiceDocumentUri, "ET1Set(2)/ET1CNav1(1)"));

                var addedLink = new ODataDeltaLink(
                    new Uri(ServiceDocumentUri, "ET1Set(2)"),
                    new Uri(ServiceDocumentUri, "ET1Set(2)/ET1CNav1(1)"),
                    "ET1CNav1");

                et3Entry.SetSerializationInfo(new ODataResourceSerializationInfo
                {
                    NavigationSourceEntityTypeName = NS + "." + et3.Name,
                    NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                    NavigationSourceName = "ET1Set(2)/ET1CNav1"
                });

                ODataNestedResourceInfo nav2 = new ODataNestedResourceInfo()
                {
                    IsCollection = true,
                    Name = "ET1Nav1",
                };

                ODataResourceSet nav2ResourceSet = new ODataResourceSet();
                ODataResource et2Entry = CreateResource(2, new Uri(ServiceDocumentUri, "ET1Set(2)/ET1Nav1(1)"));

                deltaWriter.WriteStart(deltaResourceSet);
                deltaWriter.WriteStart(deltaEntry);

                deltaWriter.WriteStart(nav2);
                deltaWriter.WriteStart(nav2ResourceSet);
                deltaWriter.WriteStart(et2Entry);
                deltaWriter.WriteEnd();
                deltaWriter.WriteEnd();
                deltaWriter.WriteEnd();

                deltaWriter.WriteEnd();
                deltaWriter.WriteDeltaDeletedEntry(deletedEntry);
                deltaWriter.WriteDeltaDeletedLink(deletedLink);
                deltaWriter.WriteStart(et3Entry);
                deltaWriter.WriteEnd();
                deltaWriter.WriteDeltaLink(addedLink);

                deltaWriter.WriteEnd();

            }, requestUri, out contentType);

            string FixReplacementFormat = "@odata.context\":\"{0}";
            payload = startWithSlash
                ? payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, "/SampleService/"))
                : payload.Replace(string.Format(FixReplacementFormat, ServiceRoot), string.Format(FixReplacementFormat, ""));

            output.WriteLine(payload);

            this.ReadPayload(payload, "application/json", model, omReader =>
            {
                var deltaReader = omReader.CreateODataDeltaReader(et1Set, et1);
                while (deltaReader.Read()) ;
            }, requestUri);
        }

        private string WritePayload(EdmModel edmModel, Action<ODataMessageWriter> write, Uri requestUri, out string contentType)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream() };

            var writerSettings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false
            };

            writerSettings.SetServiceDocumentUri(ServiceDocumentUri);
            writerSettings.SetContentType(ODataFormat.Json);

            if (requestUri != null)
            {
                ODataUriParser odataUriParser = new ODataUriParser(edmModel, ServiceDocumentUri, requestUri);
                writerSettings.ODataUri = new ODataUri()
                {
                    ServiceRoot = ServiceDocumentUri,
                    Path = odataUriParser.ParsePath()
                };
            }

            using (var msgWriter = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, edmModel))
            {
                write(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream))
            {
                contentType = message.GetHeader("Content-Type");
                return reader.ReadToEnd();
            }
        }

        private void ReadPayload(string payload, string contentType, EdmModel edmModel, Action<ODataMessageReader> test, Uri requestUri)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = new Uri(ServiceDocumentUri, "$metadata"),
                RequestUri = requestUri
            };

            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, edmModel))
            {
                test(msgReader);
            }
        }

        private static ODataResource CreateResource(int seq, Uri id = null)
        {
            return new ODataResource()
            {
                Id = id,
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = $"ET{seq}Key",
                        Value = 1,
                    },
                    new ODataProperty()
                    {
                        Name = string.Format("ET{0}P1", seq),
                        Value = "P1",
                    }
                }
            };
        }
    }
}