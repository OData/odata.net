//---------------------------------------------------------------------
// <copyright file="UrlConventionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class UrlConventionTests
    {
        [Fact]
        public void UrlConventionFromAnnotationShouldReturnDefaultIfAnnotationMissing()
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var model = new EdmModel();
            model.AddElement(container);
            model.AddVocabularyAnnotation(new EdmAnnotation(container, new EdmTerm("Fake", "Fake", EdmPrimitiveTypeKind.Stream), EdmNullExpression.Instance));
            UrlConvention.ForModel(model).GenerateKeyAsSegment.Should().BeFalse();
        }

        [Fact]
        public void UrlConventionFromAnnotationShouldReturnDefaultIfAnnotationHasWrongValue()
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var model = new EdmModel();
            model.AddElement(container);
            model.AddVocabularyAnnotation(new EdmAnnotation(container, UrlConventionsConstants.ConventionTerm, new EdmStringConstant("fake")));
            UrlConvention.ForModel(model).GenerateKeyAsSegment.Should().BeFalse();
        }

        [Fact]
        public void UrlConventionFromAnnotationShouldReturnKeyAsSegmentIfAnnotationFound()
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var model = new EdmModel();
            model.AddElement(container);
            model.AddVocabularyAnnotation(new EdmAnnotation(container, UrlConventionsConstants.ConventionTerm, UrlConventionsConstants.KeyAsSegmentAnnotationValue));
            UrlConvention.ForModel(model).GenerateKeyAsSegment.Should().BeTrue();
        }

        [Fact]
        public void UrlConventionFromSettingAndTypeContextShouldReturnKeyAsSegmentIfSettingIsTrue()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false)
            };
            UrlConvention.ForUserSettingAndTypeContext(true, typeContext).GenerateKeyAsSegment.Should().BeTrue();
        }

        [Fact]
        public void UrlConventionFromSettingAndTypeContextShouldReturnDefaultIfSettingIsFalse()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ true)
            };
            UrlConvention.ForUserSettingAndTypeContext(false, typeContext).GenerateKeyAsSegment.Should().BeFalse();
        }

        [Fact]
        public void UrlConventionFromSettingAndTypeContextShouldReturnDefaultIfSettingIsNullAndTypeContextReturnsDefault()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false)
            };
            UrlConvention.ForUserSettingAndTypeContext(null, typeContext).GenerateKeyAsSegment.Should().BeFalse();
        }

        [Fact]
        public void UrlConventionFromSettingAndTypeContextShouldReturnKeyAsSegmentIfSettingIsNullAndTypeContextReturnsKeyAsSegment()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ true)
            };
            UrlConvention.ForUserSettingAndTypeContext(null, typeContext).GenerateKeyAsSegment.Should().BeTrue();
        }
    }
}
