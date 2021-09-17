//---------------------------------------------------------------------
// <copyright file="ODataExpandPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class ODataExpandPathTests
    {
        private readonly TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
        private readonly NavigationPropertySegment navigationSegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

        [Fact]
        public void ExpandPathShouldNotAllowCountSegment()
        {
            Action createWithCountSegment = () => new ODataExpandPath(CountSegment.Instance, this.navigationSegment);
            createWithCountSegment.Throws<ODataException>(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment("CountSegment"));
        }

        [Fact]
        public void ExpandPathShouldNotAllowValueSegment()
        {
            Action createWithValueSegment = () => new ODataExpandPath(new ValueSegment(HardCodedTestModel.GetPersonType()), this.navigationSegment);
            createWithValueSegment.Throws<ODataException>(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment("ValueSegment"));
        }

        // This test is not relevant since we are now supporting type segments as the last segment in $expand
        //[Fact]
        //public void ExpandPathShouldNotAllowTypeSegmentToBeLast()
        //{
        //    Action createWithTypeSegmentLast = () => new ODataExpandPath(this.typeSegment);
        //    createWithTypeSegmentLast.Throws<ODataException>(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
        //}

        [Fact]
        public void ExpandPathShouldNotAllowMultipleNavigations()
        {
            Action createWithTypeSegmentLast = () => new ODataExpandPath(this.navigationSegment, this.navigationSegment);
            createWithTypeSegmentLast.Throws<ODataException>(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentCanBeNavigationProperty);
        }

        [Fact]
        public void ExpandPathShouldWorkForSingleNavigation()
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>() { this.navigationSegment };
            new ODataExpandPath(segments).ContainExactly(segments.ToArray());
        }

        [Fact]
        public void ExpandPathShouldWorkForMultipleTypeSegmentsFollowedByANavigation()
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>() { this.typeSegment, this.typeSegment, this.navigationSegment };
            new ODataExpandPath(segments).ContainExactly(segments.ToArray());
        }
    }
}
