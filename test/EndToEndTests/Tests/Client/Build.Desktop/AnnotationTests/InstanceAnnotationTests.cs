//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.AnnotationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstanceAnnotationTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private const string TestModelNameSpace = "Microsoft.Test.OData.Services.ODataWCFService";
        private const string IncludeAnnotation = "odata.include-annotations";

        private readonly string[] TestMimeTypes = new string[]
        {
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        };

        public InstanceAnnotationTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        #region Non Top Level
        [TestMethod]
        public void EntityInstanceAnnotation()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in TestMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Boss", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();
                        List<ODataResource> entries = new List<ODataResource>();
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entries.Add(reader.Item as ODataResource);
                            }
                        }

                        Assert.AreEqual(true, (entries.Last().InstanceAnnotations.First(i => i.Name.Equals(string.Format("{0}.IsBoss", TestModelNameSpace))).Value as ODataPrimitiveValue).Value);
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void ComplexTypeInstanceAnnotation()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in TestMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();

                        ODataResource entry = null;
                        bool startHomeAddress = false;
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.NestedResourceInfoStart)
                            {
                                ODataNestedResourceInfo navigation = reader.Item as ODataNestedResourceInfo;
                                if (navigation != null && navigation.Name == "HomeAddress")
                                {
                                    startHomeAddress = true;
                                }
                            }
                            else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                            {
                                ODataNestedResourceInfo navigation = reader.Item as ODataNestedResourceInfo;
                                if (navigation != null && navigation.Name == "HomeAddress")
                                {
                                    startHomeAddress = false;
                                }
                            }
                            else if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entry = reader.Item as ODataResource;

                                if (startHomeAddress)
                                {
                                    // Verify Annotation on Complex Type
                                    ODataInstanceAnnotation annotationOnHomeAddress = entry.InstanceAnnotations.Last();
                                    Assert.AreEqual(string.Format("{0}.AddressType", TestModelNameSpace), annotationOnHomeAddress.Name);
                                    Assert.AreEqual("Home", (annotationOnHomeAddress.Value as ODataPrimitiveValue).Value);

                                    // TODO : Fix #625

                                    //// Verify Annotation on Property in Complex Type
                                    //ODataInstanceAnnotation annotationOnCity = entry.Properties.SingleOrDefault(p => p.Name.Equals("City")).InstanceAnnotations.SingleOrDefault();
                                    //Assert.AreEqual(string.Format("{0}.CityInfo", TestModelNameSpace), annotationOnCity.Name);
                                    //Assert.AreEqual(2, (annotationOnCity.Value as ODataComplexValue).Properties.Count());
                                }
                            }
                        }
                        // Verify Annotation on Property of Entity
                        ODataInstanceAnnotation annotationonEmails = entry.Properties.SingleOrDefault(p => p.Name.Equals("Emails")).InstanceAnnotations.SingleOrDefault();
                        Assert.AreEqual(string.Format("{0}.DisplayName", TestModelNameSpace), annotationonEmails.Name);
                        Assert.AreEqual("EmailAddresses", (annotationonEmails.Value as ODataPrimitiveValue).Value);
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }
        #endregion

        #region Top Level
        [TestMethod]
        public void TopLevelComplexTypeInstanceAnnotation()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in TestMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)/HomeAddress", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var odataReader = messageReader.CreateODataResourceReader();
                        ODataResource resource = null;
                        while (odataReader.Read())
                        {
                            if (odataReader.State == ODataReaderState.ResourceEnd)
                            {
                                resource = odataReader.Item as ODataResource;
                            }
                        }

                        // Verify Annotation on Complex Type
                        ODataInstanceAnnotation annotationOnHomeAddress = resource.InstanceAnnotations
                            .SingleOrDefault(annotation=>annotation.Name.Equals(string.Format("{0}.AddressType", TestModelNameSpace)));
                        Assert.AreEqual("Home", (annotationOnHomeAddress.Value as ODataPrimitiveValue).Value);

                        // TODO : Fix #625
                        //// Verify Annotation on Property in Complex Type
                        //ODataInstanceAnnotation annotationOnCity = complexValue.Properties.SingleOrDefault(p => p.Name.Equals("City")).InstanceAnnotations.SingleOrDefault();
                        //Assert.AreEqual(string.Format("{0}.CityInfo", TestModelNameSpace), annotationOnCity.Name);
                        //Assert.AreEqual(2, (annotationOnCity.Value as ODataComplexValue).Properties.Count());
                    }
                }
            }
        }

        // [TestMethod] // github issuse: #896
        public void TopLevelPropertyOfComplexTypeInstanceAnnotation()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in TestMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)/HomeAddress/City", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var property = messageReader.ReadProperty();

                        // TODO: Service does not support add annotation in property now
                    }
                }
            }
        }
        #endregion
    }
}
