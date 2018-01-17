//---------------------------------------------------------------------
// <copyright file="ForwardingSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// This is a forwarding transform pipe segment
    /// </summary>
    internal class ForwardingSegment : SpatialPipeline
    {
        /// <summary>
        /// The singleton NoOp implementation of the DrawGeography
        /// </summary>
        internal static readonly SpatialPipeline SpatialPipelineNoOp = new SpatialPipeline(new NoOpGeographyPipeline(), new NoOpGeometryPipeline());

        /// <summary>
        /// The current drawspatial that will be called and whose results will be forwarded to the
        /// next segment
        /// </summary>
        private readonly SpatialPipeline current;

        /// <summary>
        /// The SpatialPipeline to forward the calls to
        /// </summary>
        private SpatialPipeline next = SpatialPipelineNoOp;

        /// <summary>
        /// the cached GeographyForwarder for this instance
        /// </summary>
        private GeographyForwarder geographyForwarder;

        /// <summary>
        /// the cached GeometryForwarder for this instance
        /// </summary>
        private GeometryForwarder geometryForwarder;

        /// <summary>
        /// Constructs a new SpatialPipeline segment
        /// </summary>
        /// <param name="current">The DrawSpatial to draw to before calling next.</param>
        public ForwardingSegment(SpatialPipeline current)
        {
            Debug.Assert(current.GeographyPipeline == null || !(current.GeographyPipeline is GeographyForwarder), "the current should not already be wrapped.");
            Debug.Assert(current.GeometryPipeline == null || !(current.GeometryPipeline is GeometryForwarder), "the current should not already be wrapped.");
            this.current = current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardingSegment"/> class.
        /// </summary>
        /// <param name="currentGeography">The current geography.</param>
        /// <param name="currentGeometry">The current geometry.</param>
        public ForwardingSegment(GeographyPipeline currentGeography, GeometryPipeline currentGeometry)
            : this(new SpatialPipeline(currentGeography, currentGeometry))
        {
        }

        /// <summary>
        /// Gets the geography.
        /// </summary>
        public override GeographyPipeline GeographyPipeline
        {
            get { return this.geographyForwarder ?? (this.geographyForwarder = new GeographyForwarder(this)); }
        }

        /// <summary>
        /// Gets the geometry.
        /// </summary>
        public override GeometryPipeline GeometryPipeline
        {
            get { return this.geometryForwarder ?? (this.geometryForwarder = new GeometryForwarder(this)); }
        }

        /// <summary>
        /// The next geography sink in  the pipeline
        /// </summary>
        public GeographyPipeline NextDrawGeography
        {
            get { return this.next; }
        }

        /// <summary>
        /// The next geometry sink in the pipeline
        /// </summary>
        public GeometryPipeline NextDrawGeometry
        {
            get { return this.next; }
        }

        /// <summary>
        /// Add the next pipeline
        /// </summary>
        /// <param name="destination">the next pipleine</param>
        /// <returns>The last pipesegment in the chain, usually the one just created</returns>
        public override SpatialPipeline ChainTo(SpatialPipeline destination)
        {
            Util.CheckArgumentNull(destination, "destination");

#if DEBUG
            // must be null, a user implemented pipeline, lastly if it is ours, it better be wrapped in a forwarder
            string[] spatialNamespaces = new string[] { "Microsoft.Spatial", "Microsoft.Spatial" };
            Debug.Assert(destination.GeographyPipeline == null || !spatialNamespaces.Any(s => s == destination.GeographyPipeline.GetType().Namespace) || destination.GeographyPipeline is GeographyForwarder, "the destination must be wrapped with the same type of the exception handling/Reset won't work correctly.");
            Debug.Assert(destination.GeometryPipeline == null || !spatialNamespaces.Any(s => s == destination.GeometryPipeline.GetType().Namespace) || destination.GeometryPipeline is GeometryForwarder, "the destination must be wrapped with the same type of the exception handling/Reset won't work correctly.");
#endif
            this.next = destination;
            destination.StartingLink = this.StartingLink;
            return destination;
        }

        /// <summary>
        /// Run one action on a pipeline
        /// </summary>
        /// <param name="handler">what to do at this stage of the pipeline</param>
        /// <param name="handlerReset">The handler reset.</param>
        /// <param name="delegation">what the rest of the pipeline should do</param>
        /// <param name="delegationReset">The delegation reset.</param>
        private static void DoAction(Action handler, Action handlerReset, Action delegation, Action delegationReset)
        {
            try
            {
                handler();
            }
            catch (Exception e)
            {
                if (Util.IsCatchableExceptionType(e))
                {
                    handlerReset();
                    delegationReset();
                }

                throw;
            }

            try
            {
                delegation();
            }
            catch (Exception e)
            {
                if (Util.IsCatchableExceptionType(e))
                {
                    handlerReset();
                }

                throw;
            }
        }

        /// <summary>
        /// Run one action on a pipeline
        /// </summary>
        /// <typeparam name="T">The type taken and returned by the transform style methods.</typeparam>
        /// <param name="handler">what to do at this stage of the pipeline</param>
        /// <param name="handlerReset">The handler reset.</param>
        /// <param name="delegation">what the rest of the pipeline should do</param>
        /// <param name="delegationReset">The delegation reset.</param>
        /// <param name="argument">The argument to pass to both this node and the rest of the pipeline</param>
        private static void DoAction<T>(Action<T> handler, Action handlerReset, Action<T> delegation, Action delegationReset, T argument)
        {
            try
            {
                handler(argument);
            }
            catch (Exception e)
            {
                if (Util.IsCatchableExceptionType(e))
                {
                    handlerReset();
                    delegationReset();
                }

                throw;
            }

            try
            {
                delegation(argument);
            }
            catch (Exception e)
            {
                if (Util.IsCatchableExceptionType(e))
                {
                    handlerReset();
                }

                throw;
            }
        }

        /// <summary>
        /// The forwarding implementation of DrawGeography
        /// </summary>
        internal class GeographyForwarder : GeographyPipeline
        {
            /// <summary>
            /// The ForwardingSegment instance that this pipe is
            /// associated with
            /// </summary>
            private readonly ForwardingSegment segment;

            /// <summary>
            /// Initializes a new instance of the <see cref="GeographyForwarder"/> class.
            /// </summary>
            /// <param name="segment">The segment.</param>
            public GeographyForwarder(ForwardingSegment segment)
            {
                this.segment = segment;
            }

            /// <summary>
            /// Gets the current DrawGeography from the associated ForwardingSegment instance
            /// </summary>
            private GeographyPipeline Current
            {
                get { return this.segment.current; }
            }

            /// <summary>
            /// Gets the next GeographyPipeline from the associated ForwardingSegment instance
            /// </summary>
            private GeographyPipeline Next
            {
                get { return this.segment.next; }
            }

            /// <summary>
            /// Set the system reference to be used by this run of the pipeline
            /// </summary>
            /// <param name="coordinateSystem">the coordinate reference system</param>
            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                DoAction(val => Current.SetCoordinateSystem(val), val => Next.SetCoordinateSystem(val), coordinateSystem);
            }

            /// <summary>
            /// start processing Geography data
            /// </summary>
            /// <param name="type">the sort of Geography data being processed</param>
            public override void BeginGeography(SpatialType type)
            {
                DoAction(val => Current.BeginGeography(val), val => Next.BeginGeography(val), type);
            }

            /// <summary>
            /// finish processing Geography data
            /// </summary>
            public override void EndGeography()
            {
                DoAction(Current.EndGeography, Next.EndGeography);
            }

            /// <summary>
            /// Begin drawing a Geography figure
            /// </summary>
            /// <param name="position">Next position</param>
            public override void BeginFigure(GeographyPosition position)
            {
                Util.CheckArgumentNull(position, "position");
                DoAction(
                    val => Current.BeginFigure(val),
                    val => Next.BeginFigure(val),
                    position);
            }

            /// <summary>
            /// Finish drawing a Geography figure
            /// </summary>
            public override void EndFigure()
            {
                DoAction(Current.EndFigure, Next.EndFigure);
            }

            /// <summary>
            /// Continue drawing a Geography figure
            /// </summary>
            /// <param name="position">Next position</param>
            public override void LineTo(GeographyPosition position)
            {
                Util.CheckArgumentNull(position, "position");
                DoAction(
                    val => Current.LineTo(val),
                    val => Next.LineTo(val),
                    position);
            }

            /// <summary>
            /// Reset the piprline
            /// </summary>
            public override void Reset()
            {
                DoAction(Current.Reset, Next.Reset);
            }

            /// <summary>
            /// Run one action on a Geography pipeline
            /// </summary>
            /// <typeparam name="T">The type taken and returned by the transform style methods.</typeparam>
            /// <param name="handler">what to do at this stage of the pipeline</param>
            /// <param name="delegation">what the rest of the pipeline should do</param>
            /// <param name="argument">The argument to pass to both this node and the rest of the pipeline</param>
            private void DoAction<T>(Action<T> handler, Action<T> delegation, T argument)
            {
                ForwardingSegment.DoAction(handler, Current.Reset, delegation, Next.Reset, argument);
            }

            /// <summary>
            /// Run one action on a Geography pipeline
            /// </summary>
            /// <param name="handler">what to do at this stage of the pipeline</param>
            /// <param name="delegation">what the rest of the pipeline should do</param>
            private void DoAction(Action handler, Action delegation)
            {
                ForwardingSegment.DoAction(handler, Current.Reset, delegation, Next.Reset);
            }
        }

        /// <summary>
        /// The forwarding implementation of DrawGeography
        /// </summary>
        internal class GeometryForwarder : GeometryPipeline
        {
            /// <summary>
            /// The ForwardingSegment instance that this pipe is
            /// associated with
            /// </summary>
            private readonly ForwardingSegment segment;

            /// <summary>
            /// Initializes a new instance of the <see cref="GeometryForwarder"/> class.
            /// </summary>
            /// <param name="segment">The segment.</param>
            public GeometryForwarder(ForwardingSegment segment)
            {
                this.segment = segment;
            }

            /// <summary>
            /// Gets the current DrawGeometry from the associated ForwardingSegment instance
            /// </summary>
            private GeometryPipeline Current
            {
                get { return this.segment.current; }
            }

            /// <summary>
            /// Gets the next GeometryPipeline from the associated ForwardingSegment instance
            /// </summary>
            private GeometryPipeline Next
            {
                get { return this.segment.next; }
            }

            /// <summary>
            /// Set the system reference to be used by this run of the pipeline
            /// </summary>
            /// <param name="coordinateSystem">the coordinate reference system</param>
            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                DoAction(val => Current.SetCoordinateSystem(val), val => Next.SetCoordinateSystem(val), coordinateSystem);
            }

            /// <summary>
            /// start processing Geometry data
            /// </summary>
            /// <param name="type">the sort of Geometry data being processed</param>
            public override void BeginGeometry(SpatialType type)
            {
                DoAction(val => Current.BeginGeometry(val), val => Next.BeginGeometry(val), type);
            }

            /// <summary>
            /// finish processing Geometry data
            /// </summary>
            public override void EndGeometry()
            {
                DoAction(Current.EndGeometry, Next.EndGeometry);
            }

            /// <summary>
            /// Begin drawing a Geometry figure
            /// </summary>
            /// <param name="position">Next position</param>
            public override void BeginFigure(GeometryPosition position)
            {
                Util.CheckArgumentNull(position, "position");
                DoAction(val => Current.BeginFigure(val), val => Next.BeginFigure(val), position);
            }

            /// <summary>
            /// Finish drawing a Geometry figure
            /// </summary>
            public override void EndFigure()
            {
                DoAction(Current.EndFigure, Next.EndFigure);
            }

            /// <summary>
            /// Continue drawing a Geometry figure
            /// </summary>
            /// <param name="position">Next position</param>
            public override void LineTo(GeometryPosition position)
            {
                Util.CheckArgumentNull(position, "position");
                DoAction(val => Current.LineTo(val), val => Next.LineTo(val), position);
            }

            /// <summary>
            /// Reset the piprline
            /// </summary>
            public override void Reset()
            {
                DoAction(Current.Reset, Next.Reset);
            }

            /// <summary>
            /// Run one action on a Geography pipeline
            /// </summary>
            /// <typeparam name="T">The type taken and returned by the transform style methods.</typeparam>
            /// <param name="handler">what to do at this stage of the pipeline</param>
            /// <param name="delegation">what the rest of the pipeline should do</param>
            /// <param name="argument">The argument to pass to both this node and the rest of the pipeline</param>
            private void DoAction<T>(Action<T> handler, Action<T> delegation, T argument)
            {
                ForwardingSegment.DoAction(handler, Current.Reset, delegation, Next.Reset, argument);
            }

            /// <summary>
            /// Run one action on a Geography pipeline
            /// </summary>
            /// <param name="handler">what to do at this stage of the pipeline</param>
            /// <param name="delegation">what the rest of the pipeline should do</param>
            private void DoAction(Action handler, Action delegation)
            {
                ForwardingSegment.DoAction(handler, Current.Reset, delegation, Next.Reset);
            }
        }

        /// <summary>
        /// A noop implementation of DrawGeography
        /// </summary>
        private class NoOpGeographyPipeline : GeographyPipeline
        {
            /// <summary>
            /// Draw a point in the specified coordinate
            /// </summary>
            /// <param name="position">Next position</param>
            public override void LineTo(GeographyPosition position)
            {
            }

            /// <summary>
            /// Begin drawing a figure
            /// </summary>
            /// <param name="position">Next position</param>
            public override void BeginFigure(GeographyPosition position)
            {
            }

            /// <summary>
            /// Begin drawing a spatial object
            /// </summary>
            /// <param name="type">The spatial type of the object</param>
            public override void BeginGeography(SpatialType type)
            {
            }

            /// <summary>
            /// Ends the current figure
            /// </summary>
            public override void EndFigure()
            {
            }

            /// <summary>
            /// Ends the current spatial object
            /// </summary>
            public override void EndGeography()
            {
            }

            /// <summary>
            /// Setup the pipeline for reuse
            /// </summary>
            public override void Reset()
            {
            }

            /// <summary>
            /// Set the coordinate system
            /// </summary>
            /// <param name="coordinateSystem">The CoordinateSystem</param>
            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
            }
        }

        /// <summary>
        /// a noop implementation of DrawGeometry
        /// </summary>
        private class NoOpGeometryPipeline : GeometryPipeline
        {
            /// <summary>
            /// Draw a point in the specified coordinate
            /// </summary>
            /// <param name="position">Next position</param>
            public override void LineTo(GeometryPosition position)
            {
            }

            /// <summary>
            /// Begin drawing a figure
            /// </summary>
            /// <param name="position">Next position</param>
            public override void BeginFigure(GeometryPosition position)
            {
            }

            /// <summary>
            /// Begin drawing a spatial object
            /// </summary>
            /// <param name="type">The spatial type of the object</param>
            public override void BeginGeometry(SpatialType type)
            {
            }

            /// <summary>
            /// Ends the current figure
            /// </summary>
            public override void EndFigure()
            {
            }

            /// <summary>
            /// Ends the current spatial object
            /// </summary>
            public override void EndGeometry()
            {
            }

            /// <summary>
            /// Setup the pipeline for reuse
            /// </summary>
            public override void Reset()
            {
            }

            /// <summary>
            /// Set the coordinate system
            /// </summary>
            /// <param name="coordinateSystem">The CoordinateSystem</param>
            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
            }
        }
    }
}
