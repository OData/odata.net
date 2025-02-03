//---------------------------------------------------------------------
// <copyright file="LoggingPipelines.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    /// <summary>
    /// Spatial pipeline implementation that tracks all of the method calls made to it and their arguments.
    /// 
    /// Can also be configured to always throw an exception on all method calls, which is useful in a scenario where you need
    /// a pipeline for a test scenario, but don't actually expect it to be used by the code under test.
    /// 
    /// This class is intended to be very general and just do logging of the calls it receives, so that's why it implements
    /// both geometry and geography, and doesn't try to do any coordinate argument reversal.
    /// </summary>
    public class CallSequenceLoggingPipeline : SpatialPipeline
    {
        private readonly bool alwaysThrow;
        private readonly List<KeyValuePair<PipelineMethod, object>> methodCalls;

        public CallSequenceLoggingPipeline()
            : this(false)
        {
        }

        public CallSequenceLoggingPipeline(bool alwaysThrow)
        {
            this.alwaysThrow = alwaysThrow;
            this.methodCalls = new List<KeyValuePair<PipelineMethod, object>>();
        }

        public void Verify(CallSequenceLoggingPipeline expectedPipeline)
        {
            List<KeyValuePair<PipelineMethod, object>> expectedCalls = expectedPipeline.methodCalls;
            List<KeyValuePair<PipelineMethod, object>> actualCalls = this.methodCalls;

            SpatialTestUtils.AssertEqualContents(expectedCalls, actualCalls);
        }

        public void Merge(CallSequenceLoggingPipeline target, bool keepAllSetCrsCalls = false)
        {
            // when merging two pipeline calls, the inner SetCoordinateSystem shouldn't be
            // merged. This is primarily used for constructing expected pipeline calls for collection types
            // from individual member types. Add a flag to this if another usage requires the full pipeline to be merged.
            this.methodCalls.AddRange(target.methodCalls.Where(kvp => keepAllSetCrsCalls || (kvp.Key != PipelineMethod.GeographySetCoordinateSystem && kvp.Key != PipelineMethod.GeometrySetCoordinateSystem)));
        }

        public override GeometryPipeline GeometryPipeline
        {
            get { return new GeometryPipe(this); }
        }

        public override GeographyPipeline GeographyPipeline
        {
            get { return new GeographyPipe(this); }
        }

        private class GeographyPipe : GeographyPipeline
        {
            private CallSequenceLoggingPipeline logger;

            public GeographyPipe(CallSequenceLoggingPipeline logger)
            {
                this.logger = logger;
            }

            public override void BeginGeography(SpatialType type)
            {
                logger.BeginShape(PipelineMethod.BeginGeography, type);
            }

            public override void BeginFigure(GeographyPosition position)
            {
                logger.AddCoordinates(PipelineMethod.GeographyBeginFigure, position.Latitude, position.Longitude, position.Z, position.M);
            }

            public override void LineTo(GeographyPosition position)
            {
                logger.AddCoordinates(PipelineMethod.GeographyAddLineTo, position.Latitude, position.Longitude, position.Z, position.M);
            }

            public override void EndFigure()
            {
                logger.EndFigure(PipelineMethod.GeographyEndFigure);
            }

            public override void EndGeography()
            {
                logger.EndShape(PipelineMethod.EndGeography);
            }

            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                logger.SetCoordinateSystem(PipelineMethod.GeographySetCoordinateSystem, coordinateSystem);
            }

            public override void Reset()
            {
                logger.Reset(PipelineMethod.GeographyReset);
            }
        }


        private class GeometryPipe : GeometryPipeline
        {
            private CallSequenceLoggingPipeline logger;

            public GeometryPipe(CallSequenceLoggingPipeline logger)
            {
                this.logger = logger;
            }
         
            public override void BeginGeometry(SpatialType type)
            {
                logger.BeginShape(PipelineMethod.BeginGeometry, type);
            }

            public override void BeginFigure(GeometryPosition position)
            {
                logger.AddCoordinates(PipelineMethod.GeometryBeginFigure, position.X, position.Y, position.Z, position.M);
            }

            public override void LineTo(GeometryPosition position)
            {
                logger.AddCoordinates(PipelineMethod.GeometryAddLineTo, position.X, position.Y, position.Z, position.M);
            }

            public override void EndFigure()
            {
                logger.EndFigure(PipelineMethod.GeometryEndFigure);
            }

            public override void EndGeometry()
            {
                logger.EndShape(PipelineMethod.EndGeometry);
            }

            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                logger.SetCoordinateSystem(PipelineMethod.GeometrySetCoordinateSystem, coordinateSystem);
            }

            public override void Reset()
            {
                logger.Reset(PipelineMethod.GeometryReset);
            }
        }

        #region Helper Methods for both pipeline types

        private void BeginShape(PipelineMethod pipelineMethod, SpatialType type)
        {
            CheckAlwaysThrow();
            LogMethodCall(pipelineMethod, type);
        }

        private void AddCoordinates(PipelineMethod pipelineMethod, double x, double y, double? z, double? m)
        {
            CheckAlwaysThrow();
            LogMethodCall(pipelineMethod, new double?[] { x, y, z, m });
        }

        private void EndFigure(PipelineMethod pipelineMethod)
        {
            CheckAlwaysThrow();
            LogMethodCall(pipelineMethod, null);
        }

        private void EndShape(PipelineMethod pipelineMethod)
        {
            CheckAlwaysThrow();
            LogMethodCall(pipelineMethod, null);
        }

        private void SetCoordinateSystem(PipelineMethod pipelineMethod, CoordinateSystem coordinateSystem)
        {
            CheckAlwaysThrow();
            LogMethodCall(pipelineMethod, coordinateSystem);
        }

        private void Reset(PipelineMethod pipelineMethod)
        {
            CheckAlwaysThrow();
            LogMethodCall(pipelineMethod, null);
        }

        private void CheckAlwaysThrow()
        {
            if (this.alwaysThrow)
            {
                throw new NotImplementedException();
            }
        }

        private void LogMethodCall(PipelineMethod pipelineMethod, object methodArgs)
        {
            methodCalls.Add(new KeyValuePair<PipelineMethod, object>(pipelineMethod, methodArgs));
        }

        #endregion

        private enum PipelineMethod
        {
            // IGeographyPipeline
            BeginGeography,
            GeographyBeginFigure,
            GeographyAddLineTo,
            GeographyEndFigure,
            EndGeography,
            GeographySetCoordinateSystem,
            GeographyReset,

            // IGeometryPipeline
            BeginGeometry,
            GeometryBeginFigure,
            GeometryAddLineTo,
            GeometryEndFigure,
            EndGeometry,
            GeometrySetCoordinateSystem,
            GeometryReset,
        }
    }

    /// <summary>
    /// Defines an interface that allows configuration of expected call sequences for either geometry or geography, so
    /// that test code can just use the interface and not have to specifically make geometry or geography calls against the
    /// CallSequenceLoggingPipeline itself.
    /// </summary>
    public interface ICommonLoggingPipeline
    {
        // Pipeline methods
        void BeginShape(SpatialType spatialType);
        void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4);
        void AddLineTo(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4);
        void EndFigure();
        void EndShape();
        void SetCoordinateSystem(CoordinateSystem coordinateSystem);

        // Verification method
        void VerifyPipeline(CallSequenceLoggingPipeline actualPipeline);
    }

    /// <summary>
    /// Logging pipeline that can be used to set up a sequence of expected calls that are all using the geography pipeline interface.
    /// </summary>
    public class GeographyLoggingPipeline : ICommonLoggingPipeline
    {
        private readonly bool reverseCoordinates;

        private GeographyPipeline drawGeography;
        private CallSequenceLoggingPipeline pipeline;

        public GeographyLoggingPipeline()
            : this(true)
        {
        }

        /// <summary>
        /// Creates a new instance of the GeographyLoggingPipeline.
        /// </summary>
        /// <param name="reverseCoordinates">
        /// True if calls to BeginFigure and AddLine should reverse the first two coordinates before logging the call.
        /// </param>
        public GeographyLoggingPipeline(bool reverseCoordinates)
        {
            this.reverseCoordinates = reverseCoordinates;
            this.pipeline = new CallSequenceLoggingPipeline();
            this.drawGeography = pipeline;
        }

        public void BeginShape(SpatialType spatialType)
        {
            this.drawGeography.BeginGeography(spatialType);
        }

        public void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            if (this.reverseCoordinates)
            {
                this.drawGeography.BeginFigure(new GeographyPosition(coordinate2, coordinate1, coordinate3, coordinate4));
            }
            else
            {
                this.drawGeography.BeginFigure(new GeographyPosition(coordinate1, coordinate2, coordinate3, coordinate4));
            }
        }

        public void AddLineTo(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            if (this.reverseCoordinates)
            {
                this.drawGeography.LineTo(new GeographyPosition(coordinate2, coordinate1, coordinate3, coordinate4));
            }
            else
            {
                this.drawGeography.LineTo(new GeographyPosition(coordinate1, coordinate2, coordinate3, coordinate4));
            }
        }

        public void EndFigure()
        {
            this.drawGeography.EndFigure();
        }

        public void EndShape()
        {
            this.drawGeography.EndGeography();
        }

        public void SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            Assert.True(coordinateSystem.TopologyIs(CoordinateSystem.Topology.Geography));
            this.drawGeography.SetCoordinateSystem(coordinateSystem);
        }

        public void VerifyPipeline(CallSequenceLoggingPipeline actualPipeline)
        {
            actualPipeline.Verify(pipeline);
        }

        public void MergeCalls(GeographyLoggingPipeline target, bool keepAllSetCrsCalls = false)
        {
            this.pipeline.Merge(target.pipeline, keepAllSetCrsCalls);
        }
    }

    /// <summary>
    /// Logging pipeline that can be used to set up a sequence of expected calls that are all using the geometry pipeline interface.
    /// </summary>
    public class GeometryLoggingPipeline : ICommonLoggingPipeline
    {
        private GeometryPipeline drawGeometry;
        private CallSequenceLoggingPipeline pipeline;

        public GeometryLoggingPipeline()
        {
            this.pipeline = new CallSequenceLoggingPipeline();
            this.drawGeometry = pipeline;
        }

        public void BeginShape(SpatialType spatialType)
        {
            this.drawGeometry.BeginGeometry(spatialType);
        }

        public void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            this.drawGeometry.BeginFigure(new GeometryPosition(coordinate1, coordinate2, coordinate3, coordinate4));
        }

        public void AddLineTo(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            this.drawGeometry.LineTo(new GeometryPosition(coordinate1, coordinate2, coordinate3, coordinate4));
        }

        public void EndFigure()
        {
            this.drawGeometry.EndFigure();
        }

        public void EndShape()
        {
            this.drawGeometry.EndGeometry();
        }

        public void SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            Assert.True(coordinateSystem.TopologyIs(CoordinateSystem.Topology.Geometry));
            this.drawGeometry.SetCoordinateSystem(coordinateSystem);
        }

        public void VerifyPipeline(CallSequenceLoggingPipeline actualPipeline)
        {
            actualPipeline.Verify(pipeline);
        }

        public void MergeCalls(GeometryLoggingPipeline target, bool keepAllSetCrsCalls = false)
        {
            this.pipeline.Merge(target.pipeline, keepAllSetCrsCalls);
        }
    }
}
