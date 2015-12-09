//---------------------------------------------------------------------
// <copyright file="ODataExpandPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class ODataExpandPathTests
    {
        private readonly TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
        private readonly NavigationPropertySegment navigationSegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

        [Fact]
        public void ExpandPathShouldNotAllowCountSegment()
        {
            Action createWithCountSegment = () => new ODataExpandPath(CountSegment.Instance, this.navigationSegment);
            createWithCountSegment.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment("CountSegment"));
        }

        [Fact]
        public void ExpandPathShouldNotAllowValueSegment()
        {
            Action createWithValueSegment = () => new ODataExpandPath(new ValueSegment(HardCodedTestModel.GetPersonType()), this.navigationSegment);
            createWithValueSegment.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment("ValueSegment"));
        }

        [Fact]
        public void ExpandPathShouldNotAllowTypeSegmentToBeLast()
        {
            Action createWithTypeSegmentLast = () => new ODataExpandPath(this.typeSegment);
            createWithTypeSegmentLast.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
        }

        [Fact]
        public void ExpandPathShouldNotAllowMultipleNavigations()
        {
            Action createWithTypeSegmentLast = () => new ODataExpandPath(this.navigationSegment, this.navigationSegment);
            createWithTypeSegmentLast.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
        }

        [Fact]
        public void ExpandPathShouldWorkForSingleNavigation()
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>() { this.navigationSegment };
            new ODataExpandPath(segments).Should().ContainExactly(segments.ToArray());
        }

        [Fact]
        public void ExpandPathShouldWorkForMultipleTypeSegmentsFollowedByANavigation()
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>() { this.typeSegment, this.typeSegment, this.navigationSegment };
            new ODataExpandPath(segments).Should().ContainExactly(segments.ToArray());
        }
    }
}
