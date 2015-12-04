//---------------------------------------------------------------------
// <copyright file="GeometryPositionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GeometryPositionTests
    {
        [Fact]
        public void ToStringTests()
        {
            Assert.Equal("GeometryPosition(10, 20, 30, 40)",
                new GeometryPosition(10, 20, 30, 40).ToString());

            Assert.Equal("GeometryPosition(10, 20, null, null)",
                new GeometryPosition(10, 20).ToString());
        }

        [Fact]
        public void NotEqualToNull()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            Assert.False(position1.Equals(null));
        }

        [Fact]
        public void EqualWhenEverythingMatches()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            object position2 = new GeometryPosition(10, 20, 30, 40);
            Assert.True(position1.Equals(position2));
        }

        [Fact]
        public void ConstructorVariationHasSameEffect()
        {
            Assert.Equal(new GeometryPosition(10, 20, null, null), new GeometryPosition(10, 20));
        }

        [Fact]
        public void EqualWhenSameInstance()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            var testSubject = position1;
            Assert.True(testSubject == position1);
        }

        [Fact]
        public void NotEqualPositions()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            GeometryPosition position2 = new GeometryPosition(40, 30, 20, 10);
            Assert.True(position1 != position2);
        }

        [Fact]
        public void NotEqualObjects()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            GeographyPosition position2 = new GeographyPosition(10, 20, 30, 40);
            Assert.False(position1.Equals(position2));
        }
        
        [Fact]
        public void GetHashCodeImplementation()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            Assert.Equal(2139226112, position1.GetHashCode());

            GeometryPosition position2 = new GeometryPosition(10, 20, null, null);
            Assert.Equal(48234496, position2.GetHashCode());
        }
    }
}
