//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.AnnotationTests
{
    using System;
    using System.Linq;
    using Microsoft.OData.Core;
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

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.AreEqual(
                                    true,
                                    (entry.InstanceAnnotations.Single(i => i.Name.Equals(string.Format("{0}.IsBoss", TestModelNameSpace))).Value as ODataPrimitiveValue).Value);
                            }
                        }
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

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;

                                // Get Value of Complex Type Property HomeAddress
                                var complexValue = entry.Properties.Single(p => p.Name.Equals("HomeAddress")).Value as ODataComplexValue;

                                // Verify Annotation on Complex Type
                                ODataInstanceAnnotation annotationOnHomeAddress = complexValue.InstanceAnnotations.SingleOrDefault();
                                Assert.AreEqual(string.Format("{0}.AddressType", TestModelNameSpace), annotationOnHomeAddress.Name);
                                Assert.AreEqual("Home", (annotationOnHomeAddress.Value as ODataPrimitiveValue).Value);

                                // Verify Annotation on Property in Complex Type
                                ODataInstanceAnnotation annotationOnCity = complexValue.Properties.SingleOrDefault(p => p.Name.Equals("City")).InstanceAnnotations.SingleOrDefault();
                                Assert.AreEqual(string.Format("{0}.CityInfo", TestModelNameSpace), annotationOnCity.Name);
                                Assert.AreEqual(2, (annotationOnCity.Value as ODataComplexValue).Properties.Count());

                                // Verify Annotation on Property of Entity
                                ODataInstanceAnnotation annotationonEmails = entry.Properties.SingleOrDefault(p => p.Name.Equals("Emails")).InstanceAnnotations.SingleOrDefault();
                                Assert.AreEqual(string.Format("{0}.DisplayName", TestModelNameSpace), annotationonEmails.Name);
                                Assert.AreEqual("EmailAddresses", (annotationonEmails.Value as ODataPrimitiveValue).Value);
                            }
                        }
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
                        var property = messageReader.ReadProperty();

                        // Get Value of Complex Type Property HomeAddress
                        var complexValue = property.Value as ODataComplexValue;

                        // Verify Annotation on Complex Type
                        ODataInstanceAnnotation annotationOnHomeAddress = complexValue.InstanceAnnotations.SingleOrDefault();
                        Assert.AreEqual(string.Format("{0}.AddressType", TestModelNameSpace), annotationOnHomeAddress.Name);
                        Assert.AreEqual("Home", (annotationOnHomeAddress.Value as ODataPrimitiveValue).Value);

                        // Verify Annotation on Property in Complex Type
                        ODataInstanceAnnotation annotationOnCity = complexValue.Properties.SingleOrDefault(p => p.Name.Equals("City")).InstanceAnnotations.SingleOrDefault();
                        Assert.AreEqual(string.Format("{0}.CityInfo", TestModelNameSpace), annotationOnCity.Name);
                        Assert.AreEqual(2, (annotationOnCity.Value as ODataComplexValue).Properties.Count());
                    }
                }
            }
        }

        [Ignore]
        [TestMethod]
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
