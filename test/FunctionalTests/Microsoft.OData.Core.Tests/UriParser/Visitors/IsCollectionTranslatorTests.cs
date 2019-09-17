﻿//---------------------------------------------------------------------
// <copyright file="IsCollectionTranslatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Visitors
{
    /// <summary>
    /// Unit test for the IsCollectionTranslatorTests class.
    /// </summary>
    public class IsCollectionTranslatorTests
    {
        [Fact]
        public void TranslateWithNavigationPropertySegmentNonCollectionReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithEntitySetSegmentReturnsTrue()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());

            Assert.True(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithKeySetSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null);

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithPropertySetSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new PropertySegment(HardCodedTestModel.GetDogColorProp());

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithDynamicPathSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new DynamicPathSegment("test");

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithCountSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = CountSegment.Instance;

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithNavigationPropertyLinkSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithBatchSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = BatchSegment.Instance;

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithBatchReferenceSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new BatchReferenceSegment(
                "$0", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());

            Assert.False(translator.Translate(segment));
        }

        [Fact]
        public void TranslateWithValueSegmentReturnsThrows()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = new ValueSegment(ModelBuildingHelpers.BuildValidEntityType());

            Action translateAction = () => translator.Translate(segment);
            translateAction.Throws<NotImplementedException>(segment.ToString());
        }

        [Fact]
        public void TranslateWithMetadataSegmentReturnsFalse()
        {
            IsCollectionTranslator translator = new IsCollectionTranslator();
            var segment = MetadataSegment.Instance;

            Assert.False(translator.Translate(segment));
        }
    }
}
