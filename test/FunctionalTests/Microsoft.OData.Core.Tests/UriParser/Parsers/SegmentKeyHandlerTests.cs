//---------------------------------------------------------------------
// <copyright file="SegmentKeyHandlerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// TODO: cover everything in SegmentKeyHandler, even the functionality copied from RequestUriProcessor.
    /// </summary>
    public class SegmentKeyHandlerTests
    {
        private readonly UrlConvention defaultConvention = UrlConvention.CreateWithExplicitValue(false);
        private readonly UrlConvention keyAsSegmentConvention = UrlConvention.CreateWithExplicitValue(true);
        private ODataPathSegment singleResultSegmentWithSingleKey;
        private ODataPathSegment multipleResultSegmentWithCompositeKey;
        private ODataPathSegment multipleResultSegmentWithSingleKey;

        public SegmentKeyHandlerTests()
        {
            var typeWithStringKey = new EdmEntityType("NS.Foo", "TypeWithStringKey");
            var originalType = new EdmEntityType("NS.Foo", "SourceType");
            var keyProp = typeWithStringKey.AddStructuralProperty("KeyProp", EdmPrimitiveTypeKind.String);
            typeWithStringKey.AddKeys(keyProp);
            var multipleResultsWithSingleKeyNavProp = originalType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "NavProp", Target = typeWithStringKey, TargetMultiplicity = EdmMultiplicity.Many });

            this.singleResultSegmentWithSingleKey = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null) { SingleResult = true };
            this.multipleResultSegmentWithSingleKey = new NavigationPropertySegment(multipleResultsWithSingleKeyNavProp, null) { SingleResult = false };
            this.multipleResultSegmentWithCompositeKey = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyLionsNavProp(), null) { SingleResult = false };
        }

        [Fact]
        public void SegmentWithSingleDollarSignIsNotAKey()
        {
            KeySegment keySegment;
            SegmentKeyHandler.TryHandleSegmentAsKey("$ref", this.multipleResultSegmentWithSingleKey, null, keyAsSegmentConvention, out keySegment).Should().BeFalse();
        }

        [Fact]
        public void SegmentWithMultipleKeyPropertiesWithoutRelatedKeyShouldThrowException()
        {
            KeySegment keySegment;
            Action action = ()=>
            SegmentKeyHandler.TryHandleSegmentAsKey("key", this.multipleResultSegmentWithCompositeKey, null, keyAsSegmentConvention, out keySegment);
            action.ShouldThrow<ODataException>("The number of keys specified in the URI does not match number of key properties for the resource 'Fully.Qualified.Namespace.Lion'.");
        }

        [Fact]
        public void SegmentWithSingleResultIsNotAKey()
        {
            KeySegment keySegment;
            SegmentKeyHandler.TryHandleSegmentAsKey("key", this.singleResultSegmentWithSingleKey, null, keyAsSegmentConvention, out keySegment).Should().BeFalse();
        }

        [Fact]
        public void IfBehaviorKnobIsTurnedOffThenNoSegmentIsAKey()
        {
            KeySegment keySegment;
            SegmentKeyHandler.TryHandleSegmentAsKey("key", this.multipleResultSegmentWithSingleKey, null, defaultConvention, out keySegment).Should().BeFalse();
        }

        [Fact]
        public void SegmentKeyShouldUnescapeDollarSign()
        {
            KeySegment keySegment;
            SegmentKeyHandler.TryHandleSegmentAsKey("$$ref", this.multipleResultSegmentWithSingleKey, null, keyAsSegmentConvention, out keySegment).Should().BeTrue();
            keySegment.Should().NotBeSameAs(this.multipleResultSegmentWithSingleKey);
            keySegment.Should().NotBeNull();
            keySegment.Keys.Should().HaveCount(1);
            keySegment.Keys.Single().Value.Should().Be("$ref");
        }

        [Fact]
        public void KeysCanBeImplicitIfTheyAreSpecifiedByAReferentialIntegrityConstraint()
        {
            ODataPathSegment keySegment;
            SegmentKeyHandler.TryCreateKeySegmentFromParentheses(
                new NavigationPropertySegment(
                    HardCodedTestModel.GetPersonMyLionsNavProp(), 
                    HardCodedTestModel.GetLionSet()),
                    new KeySegment(new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 0)
                    }, 
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()),
                "10",
                out keySegment).Should().BeTrue();
            keySegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 0), new KeyValuePair<string, object>("ID2", 10));
        }

        [Fact]
        public void KeyCannotBeImplicitIfNotSpecifiedByReferentialIntegrityConstraint()
        {
            ODataPathSegment keySegment;
            Action implicitKeyWithOutRefIntegrityConstraint = () => SegmentKeyHandler.TryCreateKeySegmentFromParentheses(
                new NavigationPropertySegment(
                    HardCodedTestModel.GetPersonMyLionsNavProp(),
                    HardCodedTestModel.GetLionSet()),
                new KeySegment(new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("AttackDates", "0")
                    }, 
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()),
                "10",
                out keySegment);
            implicitKeyWithOutRefIntegrityConstraint.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.BadRequest_KeyCountMismatch(HardCodedTestModel.GetLionType().FullTypeName()));
        }
    }
}