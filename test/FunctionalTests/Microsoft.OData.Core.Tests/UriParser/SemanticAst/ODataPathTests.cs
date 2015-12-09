//---------------------------------------------------------------------
// <copyright file="ODataPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class ODataPathTests
    {
        [Fact]
        public void FirstSegmentSetCorrectly()
        {
            ODataPath path = new ODataPath(new ValueSegment(ModelBuildingHelpers.BuildValidEntityType()), CountSegment.Instance);
            path.FirstSegment.ShouldBeValueSegment();
        }

        [Fact]
        public void LastSegmentSetCorrectly()
        {
            ODataPath path = new ODataPath(MetadataSegment.Instance, CountSegment.Instance);
            path.LastSegment.ShouldBeCountSegment();
        }

        [Fact]
        public void FirstSegmentShouldReturnNullIfPathIsEmpty()
        {
            new ODataPath().FirstSegment.Should().BeNull();
        }

        [Fact]
        public void LastSegmentShouldReturnNullIfPathIsEmpty()
        {
            new ODataPath().LastSegment.Should().BeNull();
        }

        [Fact]
        public void PathsOfDifferentLengthsAreNotEqual()
        {
            new ODataPath().Equals(new ODataPath(MetadataSegment.Instance)).Should().BeFalse();
        }

        [Fact]
        public void PathsWithDifferentSegmentsAreNotEqual()
        {
            new ODataPath(MetadataSegment.Instance).Equals(new ODataPath(BatchSegment.Instance)).Should().BeFalse();
        }

        [Fact]
        public void PathsWithEqualivalentSegmentsAreEqual()
        {
            new ODataPath(new OpenPropertySegment("foo")).Equals(new ODataPath(new OpenPropertySegment("foo"))).Should().BeTrue();
        }

        [Fact]
        public void PathsShouldNotAllowSegmentsToBeNull()
        {
            Action createWithNull = () => new ODataPath((IEnumerable<ODataPathSegment>)null);
            createWithNull.ShouldThrow<ArgumentNullException>().WithMessage("segments", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void PathsShouldNotAllowAnySegmentToBeNull()
        {
            Action createWithNull = () => new ODataPath((ODataPathSegment)null);
            // TODO: better error?
            createWithNull.ShouldThrow<ArgumentNullException>().WithMessage("segments", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void FirstSegmentSetCorrectlyBatch()
        {
            ODataPath path = new ODataPath(BatchSegment.Instance, CountSegment.Instance);
            path.FirstSegment.ShouldBeBatchSegment();
        }

        [Fact]
        public void LastSegmentSetCorrectlyBatch()
        {
            ODataPath path = new ODataPath(BatchSegment.Instance, CountSegment.Instance);
            path.LastSegment.ShouldBeCountSegment();
        }

        [Fact]
        public void PathsOfDifferentLengthsAreNotEqualBatch()
        {
            new ODataPath().Equals(new ODataPath(BatchSegment.Instance)).Should().BeFalse();
        }
    }
}
