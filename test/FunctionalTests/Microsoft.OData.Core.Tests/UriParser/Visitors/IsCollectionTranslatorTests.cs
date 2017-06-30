//---------------------------------------------------------------------
// <copyright file="IsCollectionTranslatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Visitors
{
    /// <summary>
    /// Unit test for the IsCollectionTranslatorTests class.
    /// </summary>
    public class IsCollectionTranslatorTests
    {
        public IsCollectionTranslatorTests()
        {
        }

        [Fact]
        public void TranslateWithNavigationPropertySegmentNonCollectionReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithEntitySetSegmentReturnsTrue()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());

            translator.Translate(segment).Should().BeTrue();
        }

        [Fact]
        public void TranslateWithKeySetSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null);

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithPropertySetSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new PropertySegment(HardCodedTestModel.GetDogColorProp());

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithDynamicPathSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new DynamicPathSegment("test");

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithCountSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = CountSegment.Instance;

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithNavigationPropertyLinkSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithBatchSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = BatchSegment.Instance;

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithBatchReferenceSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new BatchReferenceSegment(
                "$0", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());

            translator.Translate(segment).Should().BeFalse();
        }

        [Fact]
        public void TranslateWithValueSegmentReturnsThrows()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new ValueSegment(ModelBuildingHelpers.BuildValidEntityType());

            Action translateAction = () => translator.Translate(segment);
            translateAction.ShouldThrow<NotImplementedException>(
                segment.ToString());
        }

        [Fact]
        public void TranslateWithMetadataSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = MetadataSegment.Instance;

            translator.Translate(segment).Should().BeFalse();
        }
    }
}
