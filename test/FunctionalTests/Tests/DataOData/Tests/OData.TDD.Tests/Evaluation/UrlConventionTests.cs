//---------------------------------------------------------------------
// <copyright file="UrlConventionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using AstoriaUnitTests.TDD.Common;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UrlConventionTests
    {
        [TestMethod]
        public void UrlConventionFromAnnotationShouldReturnDefaultIfAnnotationMissing()
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var model = new EdmModel();
            model.AddElement(container);
            model.AddVocabularyAnnotation(new EdmAnnotation(container, new EdmTerm("Fake", "Fake", EdmPrimitiveTypeKind.Stream), EdmNullExpression.Instance));
            UrlConvention.ForModel(model).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void UrlConventionFromAnnotationShouldReturnDefaultIfAnnotationHasWrongValue()
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var model = new EdmModel();
            model.AddElement(container);
            model.AddVocabularyAnnotation(new EdmAnnotation(container, UrlConventionsConstants.ConventionTerm, new EdmStringConstant("fake")));
            UrlConvention.ForModel(model).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void UrlConventionFromAnnotationShouldReturnKeyAsSegmentIfAnnotationFound()
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var model = new EdmModel();
            model.AddElement(container);
            model.AddVocabularyAnnotation(new EdmAnnotation(container, UrlConventionsConstants.ConventionTerm, UrlConventionsConstants.KeyAsSegmentAnnotationValue));
            UrlConvention.ForModel(model).GenerateKeyAsSegment.Should().BeTrue();
        }

        [TestMethod]
        public void UrlConventionFromSettingAndTypeContextShouldReturnKeyAsSegmentIfSettingIsTrue()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false)
            };
            UrlConvention.ForUserSettingAndTypeContext(true, typeContext).GenerateKeyAsSegment.Should().BeTrue();
        }

        [TestMethod]
        public void UrlConventionFromSettingAndTypeContextShouldReturnDefaultIfSettingIsFalse()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ true)
            };
            UrlConvention.ForUserSettingAndTypeContext(false, typeContext).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void UrlConventionFromSettingAndTypeContextShouldReturnDefaultIfSettingIsNullAndTypeContextReturnsDefault()
        {
            var typeContext = new TestFeedAndEntryTypeContext()
            {
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false)
            };
            UrlConvention.ForUserSettingAndTypeContext(null, typeContext).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
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
