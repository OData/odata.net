//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentTemplateIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Evaluation
{
    public class KeyAsSegmentTemplateIntegrationTests
    {
        private const string UrlUsingKeyAsSegmentConvention = "http://temp.org/FakeSet/1";

        private const string UrlUsingDefaultConvention = "http://temp.org/FakeSet(1)";

        [Fact]
        public void ComputedIdentityShouldUseCorrectUrlConvention()
        {
            var entry0 = CreateEntryWithKeyAsSegmentConvention(true, null);
            entry0.Id.Should().Be(UrlUsingKeyAsSegmentConvention);

            var entry1 = CreateEntryWithKeyAsSegmentConvention(true, false);
            entry1.Id.Should().Be(UrlUsingDefaultConvention);

            var entry2 = CreateEntryWithKeyAsSegmentConvention(false, true);
            entry2.Id.Should().Be(UrlUsingKeyAsSegmentConvention);
        }

        [Fact]
        public void ComputedEditLinkShouldUseCorrectUrlConvention()
        {
            var entry0 = CreateEntryWithKeyAsSegmentConvention(true, null);
            entry0.EditLink.Should().Be(UrlUsingKeyAsSegmentConvention);

            var entry1 = CreateEntryWithKeyAsSegmentConvention(true, false);
            entry1.EditLink.Should().Be(UrlUsingDefaultConvention);

            var entry2 = CreateEntryWithKeyAsSegmentConvention(false, true);
            entry2.EditLink.Should().Be(UrlUsingKeyAsSegmentConvention);
        }

        private static ODataResource CreateEntryWithKeyAsSegmentConvention(bool addAnnotation, bool? useKeyAsSegment)
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);
            if (addAnnotation)
            {
                model.AddVocabularyAnnotation(new EdmVocabularyAnnotation(container, UrlConventionsConstants.ConventionTerm, UrlConventionsConstants.KeyAsSegmentAnnotationValue));
            }

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
            thing.SetAnnotation(new ODataTypeAnnotation(entitySet, entityType));
            thing.MetadataBuilder = metadataContext.GetResourceMetadataBuilderForReader(new TestJsonLightReaderEntryState { Resource = thing, SelectedProperties = new SelectedPropertiesNode("*") }, useKeyAsSegment);
            return thing;
        }
    }
}
