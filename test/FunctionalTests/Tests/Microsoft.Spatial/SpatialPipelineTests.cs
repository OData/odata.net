//---------------------------------------------------------------------
// <copyright file="SpatialPipelineTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial.TDDUnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Spatial;
    using DataSpatialUnitTests.Utils;
    using Microsoft.Data.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SpatialPipelineTests
    {
        private List<Geography> constructedGeographyInstances = new List<Geography>();
        private List<Geometry> constructedGeometryInstances = new List<Geometry>();

        [TestMethod]
        public void TestSpatialPipelineEvents()
        {
            var pipeline = SpatialImplementation.CurrentImplementation.CreateBuilder();
            pipeline.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            pipeline.GeometryPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipeline.ProduceGeography += this.constructedGeographyInstances.Add;
            pipeline.ProduceGeometry += this.constructedGeometryInstances.Add;
            pipeline.GeographyPipeline.BeginGeography(SpatialType.Point);
            pipeline.GeometryPipeline.BeginGeometry(SpatialType.Point);
            pipeline.GeographyPipeline.BeginFigure(new GeographyPosition(47.8, -122.05, 500, null));
            pipeline.GeometryPipeline.BeginFigure(new GeometryPosition(10, 10, 10, 10));
            pipeline.GeographyPipeline.EndFigure();
            pipeline.GeometryPipeline.EndFigure();
            pipeline.GeographyPipeline.EndGeography();
            pipeline.GeometryPipeline.EndGeometry();
            Assert.IsFalse(pipeline.ConstructedGeography.IsEmpty);
            Assert.IsFalse(pipeline.ConstructedGeometry.IsEmpty);
            Assert.AreEqual(1, this.constructedGeographyInstances.Count);
            Assert.AreEqual(1, this.constructedGeometryInstances.Count);
            pipeline.ProduceGeography -= this.constructedGeographyInstances.Add;
            pipeline.ProduceGeometry -= this.constructedGeometryInstances.Add;
        }

        [TestMethod]
        public void TestChainTo()
        {
            var pipeline = SpatialImplementation.CurrentImplementation.CreateBuilder();
            var pipeline2 = SpatialImplementation.CurrentImplementation.CreateBuilder();
            var e = new NotImplementedException();
            SpatialTestUtils.VerifyExceptionThrown<NotImplementedException>(() => pipeline.ChainTo(pipeline2), e.Message);
        }

        [TestMethod]
        public void InvalidPointTest()
        {
            var pipeline = SpatialImplementation.CurrentImplementation.CreateBuilder();
            pipeline.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            pipeline.GeographyPipeline.BeginGeography(SpatialType.Point);
            SpatialTestUtils.VerifyExceptionThrown<ArgumentException>(() => pipeline.GeographyPipeline.BeginFigure(new GeographyPosition(double.NaN, 122, null, null)), Strings.InvalidPointCoordinate(double.NaN, "latitude"));
            pipeline.GeographyPipeline.Reset();
            
            pipeline.GeographyPipeline.BeginGeography(SpatialType.Point);
            SpatialTestUtils.VerifyExceptionThrown<ArgumentException>(() => pipeline.GeographyPipeline.BeginFigure(new GeographyPosition(47, double.NegativeInfinity, null, null)), Strings.InvalidPointCoordinate(double.NegativeInfinity, "longitude"));
            pipeline.GeographyPipeline.Reset();

            pipeline.GeometryPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipeline.GeometryPipeline.BeginGeometry(SpatialType.Point);
            SpatialTestUtils.VerifyExceptionThrown<ArgumentException>(() => pipeline.GeometryPipeline.BeginFigure(new GeometryPosition(double.PositiveInfinity, 122, null, null)), Strings.InvalidPointCoordinate(double.PositiveInfinity, "x"));
            pipeline.GeometryPipeline.Reset();

            pipeline.GeometryPipeline.BeginGeometry(SpatialType.Point);
            SpatialTestUtils.VerifyExceptionThrown<ArgumentException>(() => pipeline.GeometryPipeline.BeginFigure(new GeometryPosition(123, double.NaN, null, null)), Strings.InvalidPointCoordinate(double.NaN, "y"));
            pipeline.GeometryPipeline.Reset();
        }

        [TestMethod]
        public void NullPipelines()
        {
            SpatialPipeline pipeline = null;
            GeographyPipeline geographyPipeline = pipeline;
            GeometryPipeline geometryPipeline = pipeline;
            Assert.IsNull(geographyPipeline);
            Assert.IsNull(geometryPipeline);
        }

        [TestMethod]
        public void ChainCustomPipeline()
        {
            var validator = SpatialValidator.Create();
            var customPipeline = new CustomPipeline();
            var pipeline = validator.ChainTo(customPipeline).StartingLink;
            pipeline.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            Assert.IsTrue(customPipeline.GeographySetCoordinateSystemWasCalled);
        }

        public class CustomPipeline : SpatialPipeline
        {
            public bool GeographySetCoordinateSystemWasCalled = false;
            private GPipeline gpipe;
            private MPipeline mpipe;

            public CustomPipeline()
            {
                this.gpipe = new GPipeline(this);
                this.mpipe = new MPipeline();
            }

            public override GeographyPipeline GeographyPipeline
            {
                get { return gpipe; }
            }

            public override GeometryPipeline GeometryPipeline
            {
                get { return mpipe; }
            }

            private class MPipeline : GeometryPipeline
            {

                public override void BeginFigure(GeometryPosition position)
                {
                }

                public override void BeginGeometry(SpatialType type)
                {
                }

                public override void EndFigure()
                {
                }

                public override void EndGeometry()
                {
                }

                public override void LineTo(GeometryPosition position)
                {
                }

                public override void Reset()
                {
                }

                public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
                {
                }
            }

            private class GPipeline : GeographyPipeline
            {
                private CustomPipeline parent;

                public GPipeline(CustomPipeline parent)
                {
                    this.parent = parent;
                }

                public override void BeginFigure(GeographyPosition position)
                {
                }

                public override void BeginGeography(SpatialType type)
                {
                }

                public override void EndFigure()
                {
                }

                public override void EndGeography()
                {
                }

                public override void LineTo(GeographyPosition position)
                {
                }

                public override void Reset()
                {
                }

                public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
                {
                    parent.GeographySetCoordinateSystemWasCalled = true;
                }
            }
        }
    }
}
