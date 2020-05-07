//---------------------------------------------------------------------
// <copyright file="EntityReferenceLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ContainmentTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.Test.OData.Tests.Client.QueryOptionTests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HttpWebRequestMessage = Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage;

    [TestClass]
    public class EntityReferenceLinkTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        public EntityReferenceLinkTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        private QueryOptionTestsHelper TestsHelper
        {
            get
            {
                return new QueryOptionTestsHelper(ServiceBaseUri, Model);
            }
        }

        [TestMethod]
        public void EntityReferenceLinkWithAnnotationShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataEntityReferenceLink link = this.TestsHelper.QueryReferenceLink("People(2)/Parent/$ref", mimeType);
                    Assert.AreEqual(1, link.InstanceAnnotations.Count);
                    ODataInstanceAnnotation annotation = link.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Link.Annotation");
                    Assert.IsNotNull(annotation);
                    AssertODataPrimitiveValueAreEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        [TestMethod]
        public void EntityReferenceLinksWithAnnotationShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataEntityReferenceLinks links = this.TestsHelper.QueryReferenceLinks("Products(5)/Details/$ref", mimeType);
                    Assert.AreEqual(1, links.InstanceAnnotations.Count);
                    ODataInstanceAnnotation annotation = links.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Links.Annotation");
                    Assert.IsNotNull(annotation);
                    AssertODataPrimitiveValueAreEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        [TestMethod]
        public void ODataEntryWithAnnotationInReferenceLinkShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = this.TestsHelper.QueryEntries("People(2)?$expand=Parent/$ref", mimeType)
                        .Where(e => e != null && (e.TypeName.EndsWith("Customer") || e.TypeName.EndsWith("Person"))).ToList();
                    Assert.AreEqual(2, entries.Count);
                    ODataInstanceAnnotation annotation = entries.First().InstanceAnnotations.FirstOrDefault(ia => ia.Name == "Link.AnnotationByEntry");
                    Assert.IsNotNull(annotation);
                    AssertODataPrimitiveValueAreEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        // TODO GitHub#346 - Support writting instance annotations for expanded feed
        // [TestMethod] // github issuse: #896
        public void ODataEntryWithAnnotationInReferenceLinksShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResourceSet feed = this.TestsHelper.QueryInnerFeed("Products(5)?$expand=Details/$ref", mimeType);
                    Assert.AreEqual(1, feed.InstanceAnnotations.Count);
                    ODataInstanceAnnotation annotation = feed.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Links.AnnotationByFeed");
                    Assert.IsNotNull(annotation);
                    AssertODataPrimitiveValueAreEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        private static void AssertODataPrimitiveValueAreEqual(ODataValue value1, ODataValue value2)
        {
            Assert.IsNotNull(value1);
            Assert.IsNotNull(value2);
            ODataPrimitiveValue primitiveValue1 = value1 as ODataPrimitiveValue;
            ODataPrimitiveValue primitiveValue2 = value2 as ODataPrimitiveValue;
            Assert.IsNotNull(primitiveValue1);
            Assert.IsNotNull(primitiveValue2);
            Assert.AreEqual(primitiveValue1.Value, primitiveValue2.Value);
        }
    }
}