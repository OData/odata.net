//---------------------------------------------------------------------
// <copyright file="CoordinateSystemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class CoordinateSystemTests
    {
        private const CoordinateSystem.Topology Geography = CoordinateSystem.Topology.Geography;
        private const CoordinateSystem.Topology Geometry = CoordinateSystem.Topology.Geometry;

        [Fact]
        public void NotEqualToNull()
        {
            Assert.False(CoordinateSystem.DefaultGeometry.Equals(null));
        }

        [Fact]
        public void EqualWhenEverythingMatches()
        {
            Assert.Equal(Coords(), Coords());
        }

        [Fact]
        public void NotEqualWhenIdsDiffer()
        {
            Assert.NotEqual(Coords(epsgId: 1), Coords(epsgId: 2));
        }

        [Fact]
        public void NotEqualWhenFromDifferentTopologies()
        {
            Assert.NotEqual(Coords(topology: Geography), Coords(topology: Geometry));
        }

        [Fact]
        public void EqualWhenOnlyNameDiffers()
        {
            Assert.True(Coords(name:"funny name").Equals(Coords(name:"typical name")));
        }

        [Fact]
        public void EqualWhenSameInstance()
        {
            var testSubject = Coords();
            Assert.True(testSubject.Equals(testSubject));
        }

        [Fact]
        public void GeometryPropertyCaches()
        {
            Assert.Same(CoordinateSystem.Geometry(1), CoordinateSystem.Geometry(1));
        }

        [Fact]
        public void GeographyPropertyCaches()
        {
            Assert.Same(CoordinateSystem.Geography(1), CoordinateSystem.Geography(1));
        }

        [Fact]
        public void GeographyAndGeometyPropertiesReturnDifferentInstancesForSameId()
        {
            Assert.NotEqual(CoordinateSystem.Geography(1), CoordinateSystem.Geometry(1));
        }

        [Fact]
        public void ToWktImplementation()
        {
            Assert.Equal("SRID=1;", CoordinateSystem.Geography(1).ToWktId());
        }

        [Fact]
        public void ToStringImplementation()
        {
            Assert.Equal("GeographyCoordinateSystem(EpsgId=1)", CoordinateSystem.Geography(1).ToString());
        }

        [Fact]
        public void GetHashCodeImplementation()
        {
            Assert.Equal(1234, CoordinateSystem.Geography(1234).GetHashCode());
        }

        private static CoordinateSystem Coords(int epsgId = 99, string name = "usual name", CoordinateSystem.Topology topology = Geography)
        {
            return new CoordinateSystem(epsgId, name, topology);
        }
    }
}
