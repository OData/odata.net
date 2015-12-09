//---------------------------------------------------------------------
// <copyright file="GeometryBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    /// <summary>
    /// Testing Geometry Builder
    /// Integration with GeoDataBuilder and Validators
    /// We do not test the actual GeoData here, see GeoDataBuilderTests for GeoData building validation
    /// </summary>
    public class GeometryBuilderTests
    {
        private GeometryBuilderImplementation builder;
        private List<Geometry> constructedInstances;
        private SpatialImplementation creator;

        public GeometryBuilderTests()
        {
            this.creator = new DataServicesSpatialImplementation();
            this.builder = new GeometryBuilderImplementation(this.creator);
            this.builder.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            this.constructedInstances = new List<Geometry>();
            this.builder.ProduceGeometry += this.constructedInstances.Add;
        }

        [Fact]
        public void BuildCollection()
        {
            this.builder.BeginGeometry(SpatialType.Collection);

            this.builder.BeginGeometry(SpatialType.Point);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.BeginGeometry(SpatialType.Polygon);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsCollection(
                g => g.VerifyAsPoint(new PositionData(10, 20, 30, 40)),
                g => g.VerifyAsPolygon(new[]
                                             {
                                                 new PositionData(10, 20, 30, 40),
                                                 new PositionData(10, 30, 30, 40),
                                                 new PositionData(10, 40, 30, 40),
                                                 new PositionData(10, 20, 30, 40),
                                             }));
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildLineString()
        {
            this.builder.BeginGeometry(SpatialType.LineString);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(10, 30, 30, 40));
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildMultiLineString()
        {
            this.builder.BeginGeometry(SpatialType.MultiLineString);

            this.builder.BeginGeometry(SpatialType.LineString);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.BeginGeometry(SpatialType.LineString);
            this.builder.BeginFigure(new GeometryPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsMultiLineString(new[]
                                                                         {
                                                                             new PositionData(10, 20, 30, 40),
                                                                             new PositionData(10, 30, 30, 40)
                                                                         },
                                                                     new[]
                                                                         {
                                                                             new PositionData(10, 30, 30, 40),
                                                                             new PositionData(10, 20, 30, 40)
                                                                         });
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildMultiPoint()
        {
            this.builder.BeginGeometry(SpatialType.MultiPoint);

            this.builder.BeginGeometry(SpatialType.Point);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.BeginGeometry(SpatialType.Point);
            this.builder.BeginFigure(new GeometryPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsMultiPoint(new PositionData(10, 20, 30, 40), new PositionData(10, 30, 30, 40));
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildMultiPolygon()
        {
            this.builder.BeginGeometry(SpatialType.MultiPolygon);

            this.builder.BeginGeometry(SpatialType.Polygon);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.BeginGeometry(SpatialType.Polygon);
            this.builder.BeginFigure(new GeometryPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsMultiPolygon(new[]
                                                                      {
                                                                          new[]
                                                                              {
                                                                                  new PositionData(10, 20, 30, 40),
                                                                                  new PositionData(10, 30, 30, 40),
                                                                                  new PositionData(10, 40, 30, 40),
                                                                                  new PositionData(10, 20, 30, 40),
                                                                              },
                                                                      },
                                                                  new[]
                                                                      {
                                                                          new[]
                                                                              {
                                                                                  new PositionData(10, 30, 30, 40),
                                                                                  new PositionData(10, 20, 30, 40),
                                                                                  new PositionData(10, 40, 30, 40),
                                                                                  new PositionData(10, 30, 30, 40),
                                                                              },
                                                                      });
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildPoint()
        {
            this.builder.BeginGeometry(SpatialType.Point);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsPoint(new PositionData(10, 20, 30, 40));
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildPolygon()
        {
            this.builder.BeginGeometry(SpatialType.Polygon);
            this.builder.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeometryPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeometry();

            this.builder.ConstructedGeometry.VerifyAsPolygon(new[]
                                                                 {
                                                                     new PositionData(10, 20, 30, 40),
                                                                     new PositionData(10, 30, 30, 40),
                                                                     new PositionData(10, 40, 30, 40),
                                                                     new PositionData(10, 20, 30, 40)
                                                                 });
            this.VerifyBuiltOnly(this.builder.ConstructedGeometry);
        }

        [Fact]
        public void BuildTwoPoints()
        {
            var shapes = new List<Geometry>();
            for (int i = 1; i < 4; i++)
            {
                this.builder.BeginGeometry(SpatialType.Point);
                this.builder.BeginFigure(new GeometryPosition(10 * i, 20, 30, 40));
                this.builder.EndFigure();
                this.builder.EndGeometry();
                shapes.Add(this.builder.ConstructedGeometry);
            }

            // ConstructedGeometry only keeps track of the last one written.
            this.builder.ConstructedGeometry.VerifyAsPoint(new PositionData(30, 20, 30, 40));

            // But the event should have seen them all.
            this.VerifyBuiltOnly(shapes.ToArray());
        }

        private void VerifyBuiltOnly(params Geometry[] shapes)
        {
            SpatialTestUtils.AssertEqualContents(this.constructedInstances, shapes);
            foreach (var instance in constructedInstances)
            {
                Assert.Same(this.creator, instance.Creator);
            }
        }
    }
}