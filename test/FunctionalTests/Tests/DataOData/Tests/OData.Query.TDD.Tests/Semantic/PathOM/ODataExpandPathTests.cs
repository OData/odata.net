//---------------------------------------------------------------------
// <copyright file="ODataExpandPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class ODataExpandPathTests
    {
        private readonly TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
        private readonly NavigationPropertySegment navigationSegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);

        [TestMethod]
        public void ExpandPathShouldNotAllowCountSegment()
        {
            Action createWithCountSegment = () => new ODataExpandPath(CountSegment.Instance, this.navigationSegment);
            createWithCountSegment.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment("CountSegment"));
        }

        [TestMethod]
        public void ExpandPathShouldNotAllowValueSegment()
        {
            Action createWithValueSegment = () => new ODataExpandPath(new ValueSegment(HardCodedTestModel.GetPersonType()), this.navigationSegment);
            createWithValueSegment.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment("ValueSegment"));
        }

        [TestMethod]
        public void ExpandPathShouldNotAllowTypeSegmentToBeLast()
        {
            Action createWithTypeSegmentLast = () => new ODataExpandPath(this.typeSegment);
            createWithTypeSegmentLast.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
        }

        [TestMethod]
        public void ExpandPathShouldNotAllowMultipleNavigations()
        {
            Action createWithTypeSegmentLast = () => new ODataExpandPath(this.navigationSegment, this.navigationSegment);
            createWithTypeSegmentLast.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
        }

        [TestMethod]
        public void ExpandPathShouldWorkForSingleNavigation()
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>() { this.navigationSegment };
            new ODataExpandPath(segments).Should().ContainExactly(segments.ToArray());
        }

        [TestMethod]
        public void ExpandPathShouldWorkForMultipleTypeSegmentsFollowedByANavigation()
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>() { this.typeSegment, this.typeSegment, this.navigationSegment };
            new ODataExpandPath(segments).Should().ContainExactly(segments.ToArray());
        }
    }
}
