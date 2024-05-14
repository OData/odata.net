//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentTemplateIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Metadata;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Evaluation
{
    public class KeyAsSegmentTemplateIntegrationTests
    {
        private const string UrlUsingSlashAsKeyDelimiter = "http://temp.org/FakeSet/1";

        private const string UrlUsingParenthesesAsKeyDelimiter = "http://temp.org/FakeSet(1)";

        [Fact]
        public void ComputedIdentityShouldUseCorrectKeyDelimiter()
        {
            var entry1 = CreateEntryWithKeyAsSegmentConvention(false);
            Assert.Equal(UrlUsingParenthesesAsKeyDelimiter, entry1.Id.OriginalString);

            var entry2 = CreateEntryWithKeyAsSegmentConvention(true);
            Assert.Equal(UrlUsingSlashAsKeyDelimiter, entry2.Id.OriginalString);
        }

        [Fact]
        public void ComputedEditLinkShouldUseCorrectKeyDelimiter()
        {
            var entry1 = CreateEntryWithKeyAsSegmentConvention(false);
            Assert.Equal(UrlUsingParenthesesAsKeyDelimiter, entry1.EditLink.OriginalString);

            var entry2 = CreateEntryWithKeyAsSegmentConvention(true);
            Assert.Equal(UrlUsingSlashAsKeyDelimiter, entry2.EditLink.OriginalString);
        }

        private static ODataResource CreateEntryWithKeyAsSegmentConvention(bool useKeyAsSegment)
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);

            EdmEntityType entityType = new EdmEntityType("Fake", "FakeType");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(entityType);

            var entitySet = new EdmEntitySet(container, "FakeSet", entityType);
            container.AddElement(entitySet);

            var metadataContext = new ODataMetadataContext(
                true,
                null,
                new EdmTypeReaderResolver(model, null),
                model,
                new Uri("http://temp.org/$metadata"),
                null /*requestUri*/);

            var thing = new ODataResource { Properties = new[] { new ODataProperty { Name = "Id", Value = 1 } } };
            thing.TypeAnnotation = new ODataTypeAnnotation(entityType.FullTypeName());
            thing.MetadataBuilder = metadataContext.GetResourceMetadataBuilderForReader(new TestJsonReaderEntryState { Resource = thing, SelectedProperties = new SelectedPropertiesNode("*", null, null), NavigationSource = entitySet}, useKeyAsSegment);
            return thing;
        }
    }
}
