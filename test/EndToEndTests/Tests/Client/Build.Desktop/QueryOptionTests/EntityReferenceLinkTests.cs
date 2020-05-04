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
    using HttpWebRequestMessage = Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage;
    using Xunit;

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

        [Fact]
        public void EntityReferenceLinkWithAnnotationShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataEntityReferenceLink link = this.TestsHelper.QueryReferenceLink("People(2)/Parent/$ref", mimeType);
                    Assert.Equal(1, link.InstanceAnnotations.Count);
                    ODataInstanceAnnotation annotation = link.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Link.Annotation");
                    Assert.NotNull(annotation);
                    AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        [Fact]
        public void EntityReferenceLinksWithAnnotationShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataEntityReferenceLinks links = this.TestsHelper.QueryReferenceLinks("Products(5)/Details/$ref", mimeType);
                    Assert.Equal(1, links.InstanceAnnotations.Count);
                    ODataInstanceAnnotation annotation = links.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Links.Annotation");
                    Assert.NotNull(annotation);
                    AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        [Fact]
        public void ODataEntryWithAnnotationInReferenceLinkShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = this.TestsHelper.QueryEntries("People(2)?$expand=Parent/$ref", mimeType)
                        .Where(e => e != null && (e.TypeName.EndsWith("Customer") || e.TypeName.EndsWith("Person"))).ToList();
                    Assert.Equal(2, entries.Count);
                    ODataInstanceAnnotation annotation = entries.First().InstanceAnnotations.FirstOrDefault(ia => ia.Name == "Link.AnnotationByEntry");
                    Assert.NotNull(annotation);
                    AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        // TODO GitHub#346 - Support writting instance annotations for expanded feed
        // [Fact] // github issuse: #896
        public void ODataEntryWithAnnotationInReferenceLinksShouldWork()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResourceSet feed = this.TestsHelper.QueryInnerFeed("Products(5)?$expand=Details/$ref", mimeType);
                    Assert.Equal(1, feed.InstanceAnnotations.Count);
                    ODataInstanceAnnotation annotation = feed.InstanceAnnotations.SingleOrDefault(ia => ia.Name == "Links.AnnotationByFeed");
                    Assert.NotNull(annotation);
                    AssertODataPrimitiveValueEqual(new ODataPrimitiveValue(true), annotation.Value);
                }
            }
        }

        private static void AssertODataPrimitiveValueEqual(ODataValue value1, ODataValue value2)
        {
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            ODataPrimitiveValue primitiveValue1 = value1 as ODataPrimitiveValue;
            ODataPrimitiveValue primitiveValue2 = value2 as ODataPrimitiveValue;
            Assert.NotNull(primitiveValue1);
            Assert.NotNull(primitiveValue2);
            Assert.Equal(primitiveValue1.Value, primitiveValue2.Value);
        }
    }
}