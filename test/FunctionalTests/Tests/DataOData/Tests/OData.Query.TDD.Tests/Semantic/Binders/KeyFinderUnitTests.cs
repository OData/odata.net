//---------------------------------------------------------------------
// <copyright file="KeyFinderUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the KeyFinder class.
    /// </summary>
    [TestClass]
    public class KeyFinderUnitTests
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
