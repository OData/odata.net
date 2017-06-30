//---------------------------------------------------------------------
// <copyright file="UriEdmHelpersTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using System;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit test for the UriEdmHelpersTests class.
    /// </summary>
    public class UriEdmHelpersTests
    {
        public UriEdmHelpersTests()
        {
        }

        [Fact]
        public void GetNavigationPropertyFromExpandPathWithMetadataSegmentThrows()
        {
            var segment = MetadataSegment.Instance;
            ODataPath odataPath = new ODataPath(segment);
            Action uriParserHelpersAction = () => UriEdmHelpers.GetNavigationPropertyFromExpandPath(odataPath);
            uriParserHelpersAction.ShouldThrow<ODataException>(
                Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
        }

        [Fact]
        public void GetNavigationPropertyFromExpandPathWithTypeSegmentThrows()
        {
            var segment = new TypeSegment(HardCodedTestModel.GetHomeAddressType(), null);
            ODataPath odataPath = new ODataPath(segment);
            Action uriParserHelpersAction = () => UriEdmHelpers.GetNavigationPropertyFromExpandPath(odataPath);
            uriParserHelpersAction.ShouldThrow<ODataException>(
                Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
        }

        [Fact]
        public void GetNavigationPropertyFromExpandPathWithNavigationPropertySegmentReturnNavProperty()
        {
            var segment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            ODataPath odataPath = new ODataPath(segment);
            var result = UriEdmHelpers.GetNavigationPropertyFromExpandPath(odataPath);
            result.Should().Be(segment.NavigationProperty);
        }

        [Fact]
        public void GetMostDerivedTypeFromPathWithMetadataSegmentAndNullReturnsType()
        {
            var segment = MetadataSegment.Instance;
            ODataPath odataPath = new ODataPath(segment);
            var result = UriEdmHelpers.GetMostDerivedTypeFromPath(odataPath, null);
            result.Should().BeNull();
        }

        [Fact]
        public void GetMostDerivedTypeFromPathWithTypeSegmentAndNullReturnsNull()
        {
            var segment = new TypeSegment(HardCodedTestModel.GetHomeAddressType(), null);
            ODataPath odataPath = new ODataPath(segment);
            var result = UriEdmHelpers.GetMostDerivedTypeFromPath(odataPath, null);
            result.Should().BeNull();
        }

        [Fact]
        public void GetMostDerivedTypeFromPathWithTypeSegmentAndNotInheritedTypeReturnsNotInheritedType()
        {
            var segment = new TypeSegment(HardCodedTestModel.GetHomeAddressType(), null);
            ODataPath odataPath = new ODataPath(segment);
            IEdmEntityType astonishing = new EdmEntityType("AwesomeNamespace", "AstonishingEntity", null, false, false);
            IEdmEntityReferenceType entityRef = new EdmEntityReferenceType(astonishing);

            var result = UriEdmHelpers.GetMostDerivedTypeFromPath(odataPath, entityRef);
            result.Should().Be(entityRef);
        }

        [Fact]
        public void GetMostDerivedTypeFromPathWithTypeSegmentAndInheritedTypeReturnsInheritedType()
        {
            var segment = new TypeSegment(HardCodedTestModel.GetHomeAddressType(), null);
            ODataPath odataPath = new ODataPath(segment);
            TypeSegment typeSegment = odataPath.FirstSegment as TypeSegment;
            IEdmType type = typeSegment.EdmType;

            var result = UriEdmHelpers.GetMostDerivedTypeFromPath(odataPath, type);
            result.Should().Be(type);
        }
    }
}
