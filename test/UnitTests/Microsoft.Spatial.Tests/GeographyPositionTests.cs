//---------------------------------------------------------------------
// <copyright file="GeographyPositionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GeographyPositionTests
    {
        [Fact]
        public void ToStringTests()
        {
            Assert.Equal("GeographyPosition(latitude:10, longitude:20, z:30, m:40)",
                new GeographyPosition(latitude: 10, longitude: 20, z: 30, m: 40).ToString());

            Assert.Equal("GeographyPosition(latitude:10, longitude:20, z:null, m:null)",
                new GeographyPosition(latitude: 10, longitude: 20).ToString());
        }

        [Fact]
        public void NotEqualToNull()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            Assert.False(position1.Equals(null));
        }

        [Fact]
        public void EqualWhenEverythingMatches()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            object position2 = new GeographyPosition(10, 20, 30, 40);
            Assert.True(position1.Equals(position2));
        }

        [Fact]
        public void ConstructorVariationHasSameEffect()
        {
            Assert.Equal(new GeographyPosition(10, 20, null, null), new GeographyPosition(10, 20));
        }

        [Fact]
        public void EqualWhenSameInstance()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            var testSubject = position1;
            Assert.True(testSubject == position1);
        }

        [Fact]
        public void NotEqualPositions()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            GeographyPosition position2 = new GeographyPosition(10, 20, null, 40);
            Assert.True(position1 != position2);
        }

        [Fact]
        public void NotEqualObjects()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            GeometryPosition position2 = new GeometryPosition(10, 20, 30, 40);
            Assert.False(position1.Equals(position2));
        }

        [Fact]
        public void GetHashCodeImplementation()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            Assert.Equal(2139226112, position1.GetHashCode());

            GeographyPosition position2 = new GeographyPosition(10, 20, null, null);
            Assert.Equal(48234496, position2.GetHashCode());
        }
    }
}
