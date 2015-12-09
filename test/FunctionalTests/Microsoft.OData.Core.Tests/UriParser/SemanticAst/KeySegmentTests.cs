//---------------------------------------------------------------------
// <copyright file="KeySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class KeySegmentTests
    {
        private static readonly KeyValuePair<string, object>[] Key = new[] {new KeyValuePair<string, object>("key", "value")};

        [Fact]
        public void KeySetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType(), set);
            segment.Keys.Should().OnlyContain(x => x.Key == "key" && x.Value.As<string>() == "value");
        }

        [Fact]
        public void TypeIsSetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType(), set);
            segment.EdmType.Should().BeSameAs(set.EntityType());
        }

        [Fact]
        public void SetIsCopiedFromPreviousSegment()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType(), set);
            segment.NavigationSource.Should().BeSameAs(set);
        }

        [Fact]
        public void SetAndTypeMustMakeSenseTogether()
        {
            Action create = () => new KeySegment(Key, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogsSet());
            create.ShouldThrow<ODataException>().WithMessage(Strings.PathParser_TypeMustBeRelatedToSet(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogType(), "KeySegments"));
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            List<KeyValuePair<string, object>> key1 = new List<KeyValuePair<string, object>>() {new KeyValuePair<string, object>("key", "value")};
            List<KeyValuePair<string, object>> key2 = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("key", "value") };
            KeySegment segment1 = new KeySegment(key1, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            KeySegment segment2 = new KeySegment(key2, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            KeySegment segment1 = new KeySegment(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("key", "value1")}, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            KeySegment segment2 = new KeySegment(new List<KeyValuePair<string, object>>() {new KeyValuePair<string, object>("key", "value1")}, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            KeySegment segment3 = new KeySegment(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("key", "value2")}, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            CountSegment segment4 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
            segment1.Equals(segment3).Should().BeFalse();
            segment1.Equals(segment4).Should().BeFalse();
        }
    }
}
