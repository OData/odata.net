//---------------------------------------------------------------------
// <copyright file="KeySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class KeySegmentTests
    {
        private static readonly KeyValuePair<string, object>[] Key = new[] {new KeyValuePair<string, object>("key", "value")};

        [Fact]
        public void KeySetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType, set);
            var key = Assert.Single(segment.Keys);
            Assert.Equal("key", key.Key);
            Assert.Equal("value", key.Value);
        }

        [Fact]
        public void IdentifierByDefaultIsKeyDefaultName()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType, set);
            Assert.Equal("{key}", segment.Identifier);
        }

        [Fact]
        public void TypeIsSetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType, set);
            Assert.Same(set.EntityType, segment.EdmType);
        }

        [Fact]
        public void SetIsCopiedFromPreviousSegment()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            KeySegment segment = new KeySegment(Key, set.EntityType, set);
            Assert.Same(set, segment.NavigationSource);
        }

        [Fact]
        public void SetAndTypeMustMakeSenseTogether()
        {
            Action create = () => new KeySegment(Key, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogsSet());
            create.Throws<ODataException>(Strings.PathParser_TypeMustBeRelatedToSet(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogType(), "KeySegments"));
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            List<KeyValuePair<string, object>> key1 = new List<KeyValuePair<string, object>>() {new KeyValuePair<string, object>("key", "value")};
            List<KeyValuePair<string, object>> key2 = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("key", "value") };
            KeySegment segment1 = new KeySegment(key1, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            KeySegment segment2 = new KeySegment(key2, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            KeySegment segment1 = new KeySegment(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("key", "value1")}, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            KeySegment segment2 = new KeySegment(new List<KeyValuePair<string, object>>() {new KeyValuePair<string, object>("key", "value1")}, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            KeySegment segment3 = new KeySegment(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("key", "value2")}, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            CountSegment segment4 = CountSegment.Instance;
            Assert.False(segment1.Equals(segment2));
            Assert.False(segment1.Equals(segment3));
            Assert.False(segment1.Equals(segment4));
        }
    }
}
