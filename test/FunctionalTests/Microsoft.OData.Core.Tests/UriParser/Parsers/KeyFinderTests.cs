//---------------------------------------------------------------------
// <copyright file="KeyFinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the KeyFinder class.
    /// </summary>
    public class KeyFinderTests
    {
        [Fact]
        public void CurrentNavigaitionPropertyMustBePopulated()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0,1", out key, false);
            Action callWithNullNavProp = () => KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty()
                },
                null,
                new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null));
            callWithNullNavProp.ShouldThrow<ArgumentNullException>().WithMessage("currentNavigationProperty", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void RawKeyValuesMustBePopulated()
        {

            Action callWithNullRawKey = () => KeyFinder.FindAndUseKeysFromRelatedSegment(
                null,
                new List<IEdmStructuralProperty>()
                {
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty()
                },
                ModelBuildingHelpers.BuildValidNavigationProperty(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null));
            callWithNullRawKey.ShouldThrow<ArgumentNullException>().WithMessage("rawKeyValues", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void RawKeyWithMoreThanOnePositionalValueIsUnchanged()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0,1", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty()
                },
                ModelBuildingHelpers.BuildValidNavigationProperty(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null));
            newKey.Should().Be(key);
        }

        [Fact]
        public void IfValueExistsInTargetPropertiesAndNotExistingKeysItIsNotWritten()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property(),
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Name", "Stuff")
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.Should().Be(key);
        }

        [Fact]
        public void IfValueExistsinExistingKeysButNotTargetPropertiesItIsNotWritten()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId2Property(),
                    HardCodedTestModel.GetLionAttackDatesProp()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.Should().Be(key);
        }

        [Fact]
        public void IfValueAlreadySpecifiedInRawKeyItIsNotOverwritten()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID1=6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.NamedValues.Should().ContainKey("ID1")
                .And.ContainValue("6");
        }

        [Fact]
        public void PositionalValueAddedAsMissingValueIfOnlyOneMissingValueExists()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.NamedValues.Should().Contain(new KeyValuePair<string, string>("ID1", "32"))
                .And.Contain(new KeyValuePair<string, string>("ID2", "6"));
        }

        [Fact]
        public void PositionalValueNotAddedIfMoreThanOneMissingValueExists()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property(),
                    HardCodedTestModel.GetLionAttackDatesProp()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.Should().Be(key);
        }

        [Fact]
        public void PositionalValuesArrayIsCleared()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.PositionalValues.Should().BeEmpty();
        }

        [Fact]
        public void AreValuesNamedIsAlwaysSet()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.AreValuesNamed.Should().BeTrue();
        }

        [Fact]
        public void LookForKeysOnBothCurrentNavPropAndPartnerIfItExists()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetDogLionWhoAteMeNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.NamedValues.Should().Contain(new KeyValuePair<string, string>("ID1", "32"))
                .And.Contain(new KeyValuePair<string, string>("ID2", "6"));
        }

        [Fact]
        public void LookForKeysOnBothCurrentNavPropAndPartnerIfItExistsWorksForTemplate()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("{6}", out key, true);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetDogLionWhoAteMeNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.NamedValues.Should().Contain(new KeyValuePair<string, string>("ID1", "32"))
                .And.Contain(new KeyValuePair<string, string>("ID2", "{6}"));
        }

        [Fact]
        public void IfNoKeyExistsOnNavPropAndNoPartnerExistsKeyIsUnchanged()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetDogIdProp()
                },
                HardCodedTestModel.GetPersonMyDogNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.Should().Be(key);
        }

        [Fact]
        public void IfNoReferentialIntegrityConstraintExistsOnPartnerKeyIsUnchanged()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetDogIdProp()
                },
                HardCodedTestModel.GetEmployeeOfficeDogNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            newKey.Should().Be(key);
        }
    }
}
