//---------------------------------------------------------------------
// <copyright file="ODataPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Null(new ODataPath().FirstSegment);
        }

        [Fact]
        public void LastSegmentShouldReturnNullIfPathIsEmpty()
        {
            Assert.Null(new ODataPath().LastSegment);
        }

        [Fact]
        public void PathsOfDifferentLengthsAreNotEqual()
        {
            Assert.False(new ODataPath().Equals(new ODataPath(MetadataSegment.Instance)));
        }

        [Fact]
        public void PathsWithDifferentSegmentsAreNotEqual()
        {
            Assert.False(new ODataPath(MetadataSegment.Instance).Equals(new ODataPath(BatchSegment.Instance)));
        }

        [Fact]
        public void PathsWithEqualivalentSegmentsAreEqual()
        {
            Assert.True(new ODataPath(new DynamicPathSegment("foo")).Equals(new ODataPath(new DynamicPathSegment("foo"))));
        }

        [Fact]
        public void PathsShouldNotAllowSegmentsToBeNull()
        {
            Action createWithNull = () => new ODataPath((IEnumerable<ODataPathSegment>)null);
            Assert.Throws<ArgumentNullException>("segments", createWithNull);
        }

        [Fact]
        public void PathsShouldNotAllowAnySegmentToBeNull()
        {
            Action createWithNull = () => new ODataPath((ODataPathSegment)null);
            Assert.Throws<ArgumentNullException>("segments", createWithNull);

            List<ODataPathSegment> segments = new List<ODataPathSegment>(){
                null
            };

            createWithNull = () => new ODataPath(segments);
            Assert.Throws<ArgumentNullException>("segments", createWithNull);
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
            Assert.False(new ODataPath().Equals(new ODataPath(BatchSegment.Instance)));
        }
    }
}
