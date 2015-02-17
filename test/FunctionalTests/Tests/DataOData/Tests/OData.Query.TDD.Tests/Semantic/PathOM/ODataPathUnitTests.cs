//---------------------------------------------------------------------
// <copyright file="ODataPathUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataPathUnitTests
    {
        [TestMethod]
        public void FirstSegmentSetCorrectly()
        {
            ODataPath path = new ODataPath(BatchSegment.Instance, CountSegment.Instance);
            path.FirstSegment.ShouldBeBatchSegment();
        }

        [TestMethod]
        public void LastSegmentSetCorrectly()
        {
            ODataPath path = new ODataPath(BatchSegment.Instance, CountSegment.Instance);
            path.LastSegment.ShouldBeCountSegment();
        }

        [TestMethod]
        public void FirstSegmentShouldReturnNullIfPathIsEmpty()
        {
            new ODataPath().FirstSegment.Should().BeNull();
        }

        [TestMethod]
        public void LastSegmentShouldReturnNullIfPathIsEmpty()
        {
            new ODataPath().LastSegment.Should().BeNull();
        }

        [TestMethod]
        public void PathsOfDifferentLengthsAreNotEqual()
        {
            new ODataPath().Equals(new ODataPath(BatchSegment.Instance)).Should().BeFalse();
        }

        [TestMethod]
        public void PathsWithDifferentSegmentsAreNotEqual()
        {
            new ODataPath(MetadataSegment.Instance).Equals(new ODataPath(BatchSegment.Instance)).Should().BeFalse();
        }

        [TestMethod]
        public void PathsWithEqualivalentSegmentsAreEqual()
        {
            new ODataPath(new OpenPropertySegment("foo")).Equals(new ODataPath(new OpenPropertySegment("foo"))).Should().BeTrue();
        }

        [TestMethod]
        public void PathsShouldNotAllowSegmentsToBeNull()
        {
            Action createWithNull = () => new ODataPath((IEnumerable<ODataPathSegment>)null);
            createWithNull.ShouldThrow<ArgumentNullException>().WithMessage("segments", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void PathsShouldNotAllowAnySegmentToBeNull()
        {
            Action createWithNull = () => new ODataPath((ODataPathSegment)null);
            // TODO: better error?
            createWithNull.ShouldThrow<ArgumentNullException>().WithMessage("segments", ComparisonMode.EquivalentSubstring);
        }
    }
}
