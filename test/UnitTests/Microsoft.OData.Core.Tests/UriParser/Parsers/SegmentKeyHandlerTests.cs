//---------------------------------------------------------------------
// <copyright file="SegmentKeyHandlerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// TODO: cover everything in SegmentKeyHandler, even the functionality copied from RequestUriProcessor.
    /// </summary>
    public class SegmentKeyHandlerTests
    {
        private static readonly ODataUriResolver DefaultUriResolver = ODataUriResolver.GetUriResolver(null);
        private readonly ODataUrlKeyDelimiter defaultConvention = ODataUrlKeyDelimiter.Parentheses;
        private readonly ODataUrlKeyDelimiter keyAsSegmentConvention = ODataUrlKeyDelimiter.Slash;
        private readonly IEdmModel model;
        private ODataPathSegment singleResultSegmentWithSingleKey;
        private ODataPathSegment multipleResultSegmentWithCompositeKey;
        private ODataPathSegment multipleResultSegmentWithSingleKey;

        public SegmentKeyHandlerTests()
        {
            this.model = HardCodedTestModel.TestModel;
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
            var result = SegmentKeyHandler.TryHandleSegmentAsKey("$ref", this.multipleResultSegmentWithSingleKey, null, keyAsSegmentConvention, DefaultUriResolver, out keySegment);
            Assert.False(result);
        }

        [Fact]
        public void SegmentWithMultipleKeyPropertiesWithoutRelatedKeyShouldThrowException()
        {
            KeySegment keySegment;
            Action action = () =>
            SegmentKeyHandler.TryHandleSegmentAsKey("key", this.multipleResultSegmentWithCompositeKey, null, keyAsSegmentConvention, DefaultUriResolver, out keySegment);
            action.Throws<ODataException>("The number of keys specified in the URI does not match number of key properties for the resource 'Fully.Qualified.Namespace.Lion'.");
        }

        [Fact]
        public void SegmentWithSingleResultIsNotAKey()
        {
            KeySegment keySegment;
            var result = SegmentKeyHandler.TryHandleSegmentAsKey("key", this.singleResultSegmentWithSingleKey, null, keyAsSegmentConvention, DefaultUriResolver, out keySegment);
            Assert.False(result);
        }

        [Fact]
        public void IfBehaviorKnobIsTurnedOffThenNoSegmentIsAKey()
        {
            KeySegment keySegment;
            var result = SegmentKeyHandler.TryHandleSegmentAsKey("key", this.multipleResultSegmentWithSingleKey, null, defaultConvention, DefaultUriResolver, out keySegment);
            Assert.False(result);
        }

        [Fact]
        public void SegmentKeyShouldUnescapeDollarSign()
        {
            KeySegment keySegment;
            var result = SegmentKeyHandler.TryHandleSegmentAsKey("$$ref", this.multipleResultSegmentWithSingleKey, null, keyAsSegmentConvention, DefaultUriResolver, out keySegment);
            Assert.True(result);
            Assert.NotSame(this.multipleResultSegmentWithSingleKey, keySegment);
            Assert.NotNull(keySegment);
            var key = Assert.Single(keySegment.Keys);
            Assert.Equal("$ref", key.Value);
        }

        [Fact]
        public void KeysCanBeImplicitIfTheyAreSpecifiedByAReferentialIntegrityConstraint()
        {
            ODataPathSegment keySegment;
            var result = SegmentKeyHandler.TryCreateKeySegmentFromParentheses(
                this.model,
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
                DefaultUriResolver,
                out keySegment);
            Assert.True(result);
            keySegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 0), new KeyValuePair<string, object>("ID2", 10));
        }

        [Fact]
        public void KeyCannotBeImplicitIfNotSpecifiedByReferentialIntegrityConstraint()
        {
            ODataPathSegment keySegment;
            Action implicitKeyWithOutRefIntegrityConstraint = () => SegmentKeyHandler.TryCreateKeySegmentFromParentheses(
                this.model,
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
                DefaultUriResolver,
                out keySegment);
            implicitKeyWithOutRefIntegrityConstraint.Throws<ODataException>(Error.Format(SRResources.BadRequest_KeyCountMismatch, HardCodedTestModel.GetLionType().FullTypeName()));
        }
    }
}