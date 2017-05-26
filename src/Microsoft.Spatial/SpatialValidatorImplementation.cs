//---------------------------------------------------------------------
// <copyright file="SpatialValidatorImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Semantically validate a GeoData
    /// </summary>
    /// <remarks>
    /// Grammar, states, and actions:
    /// <![CDATA[
    ///     <Document> := SetSRID <Geometry> { Finish }
    ///     <Geometry> := (Begin_Point <Point> | ... | Begin_FullGlobe (: verify depth = 1 :) <FullGlobe>)
    ///     <Point> := [ BeginFigure 1 EndFigure ] 2 End
    ///     <LineString> := [ BeginFigure 1 { LineTo } EndFigure (: verify 2+ points :) ] 2 End
    ///     <Polygon> := { BeginFigure 1 { LineTo } EndFigure (: verify 4+ points and closed :) } End
    ///     <MultiPoint> := { { SetSRID } Begin_Point <Point> } End
    ///     <MultiLineString> := { { SetSRID } Begin_LineString <LineString> } End
    ///     <MultiPolygon> := { { SetSRID } Begin_Polygon <Polygon> } End
    ///     <GeometryCollection> := { { SetSRID } <Geometry> } End
    ///     <FullGlobe> := End
    ///     <CircularString> := [ BeginFigure 1 { AddCircularArc } EndFigure ] 2 End
    ///     <CompoundCurve> := [ BeginFigure 1 { LineTo | AddCircularArc } EndFigure ] | <StructuredCompoundCurve> 2 End
    ///     <StructuredCompoundCurve> := <StructuredCompoundCurveStart> { <StructuredCompoundCurvePart> } EndFigure
    ///     <StructuredCompoundCurveStart> := AddSegmentLine 0 BeginFigure { LineTo } | AddSegmentArc 0 BeginFigure { AddCircularArc }
    ///     <StructuredCompoundCurvePart> := AddSegmentLine { LineTo } | AddSegmentArc { AddCircularArc }
    ///     <CurvePolygon> := { <CurvePolygonImplicitRing> | <CurvePolygonSimpleRing> | <CurvePolygonCompoundCurveRing> EndFigure (: verify closed and three distinct :)} End
    ///     <CurvePolygonImplicitRing> := BeginFigure 1 { LineTo | AddCircularArc }
    ///     <CurvePolygonSimpleRing> := StartSimpleRing 0 <CurvePolygonImplicitRing>
    ///     <CurvePolygonCompoundCurveRing> := <CurvePolygonCompoundCurveRingStart> { <CurvePolygonCompoundCurveRingPart> }
    ///     <CurvePolygonCompoundCurveRingStart> := AddSegmentLine 0 BeginFigure { LineTo } | AddSegmentArc 0 BeginFigure { AddCircularArc }
    ///     <CurvePolygonCompoundCurveRingPart> := AddSegmentLine { LineTo } | AddSegmentArc { AddCircularArc }
    /// ]]>
    /// </remarks>
    internal class SpatialValidatorImplementation : SpatialPipeline
    {
        /// <summary>
        /// Max value for Longitude
        /// </summary>
        /// <remarks>
        /// ~263 radians converted to degrees
        /// </remarks>
        internal const double MaxLongitude = 15069;

        /// <summary>
        /// Max value for latitude
        /// </summary>
        internal const double MaxLatitude = 90;

        /// <summary>
        /// The DrawBoth derived instance of the geography Validator that is nested in this class
        /// </summary>
        private readonly NestedValidator geographyValidatorInstance = new NestedValidator();

        /// <summary>
        /// The DrawBoth derived instance of the geometry Validator that is nested in this class
        /// </summary>
        private readonly NestedValidator geometryValidatorInstance = new NestedValidator();

        /// <summary>
        /// Gets the draw geography.
        /// </summary>
        public override GeographyPipeline GeographyPipeline
        {
            get { return this.geographyValidatorInstance.GeographyPipeline; }
        }

        /// <summary>
        /// Gets the draw geometry.
        /// </summary>
        public override GeometryPipeline GeometryPipeline
        {
            get { return this.geometryValidatorInstance.GeometryPipeline; }
        }

        /// <summary>
        /// this is the actual validator, and derived from DrawBoth
        /// while the real SpatialValidator derives from DrawSpatial.
        /// We simple create an instance of this nested class and pass back
        /// the DrawGeometry, and DrawGeography when the outter classes DataSpatial
        /// properties are accessed.
        /// </summary>
        private class NestedValidator : DrawBoth
        {
            /// <summary>
            /// Set coordinate system
            /// </summary>
            private static readonly ValidatorState CoordinateSystem = new SetCoordinateSystemState();

            /// <summary>
            /// BeginGeo
            /// </summary>
            private static readonly ValidatorState BeginSpatial = new BeginGeoState();

            /// <summary>
            /// Starting a point
            /// </summary>
            private static readonly ValidatorState PointStart = new PointStartState();

            /// <summary>
            /// Building a point
            /// </summary>
            private static readonly ValidatorState PointBuilding = new PointBuildingState();

            /// <summary>
            /// Ending a point
            /// </summary>
            private static readonly ValidatorState PointEnd = new PointEndState();

            /// <summary>
            /// Starting a LineString
            /// </summary>
            private static readonly ValidatorState LineStringStart = new LineStringStartState();

            /// <summary>
            /// Building a LineString
            /// </summary>
            private static readonly ValidatorState LineStringBuilding = new LineStringBuildingState();

            /// <summary>
            /// Ending a LineString
            /// </summary>
            private static readonly ValidatorState LineStringEnd = new LineStringEndState();

            /// <summary>
            /// Starting a Polygon
            /// </summary>
            private static readonly ValidatorState PolygonStart = new PolygonStartState();

            /// <summary>
            /// Building a Polygon
            /// </summary>
            private static readonly ValidatorState PolygonBuilding = new PolygonBuildingState();

            /// <summary>
            /// Starting a MultiPoint
            /// </summary>
            private static readonly ValidatorState MultiPoint = new MultiPointState();

            /// <summary>
            /// Starting a LineString
            /// </summary>
            private static readonly ValidatorState MultiLineString = new MultiLineStringState();

            /// <summary>
            /// Starting a MultiPolygon
            /// </summary>
            private static readonly ValidatorState MultiPolygon = new MultiPolygonState();

            /// <summary>
            /// Starting a Collection
            /// </summary>
            private static readonly ValidatorState Collection = new CollectionState();

            /// <summary>
            /// Starting a FullGlobe
            /// </summary>
            private static readonly ValidatorState FullGlobe = new FullGlobeState();

            /// <summary>
            /// Geometry Functional Specification 3.2.3.4
            /// Max Geometry Collection Depth
            /// </summary>
            private const int MaxGeometryCollectionDepth = 28;

            /// <summary>
            /// States
            /// </summary>
            private readonly Stack<ValidatorState> stack = new Stack<ValidatorState>(16);

            /// <summary>
            /// CoordinateSystem
            /// </summary>
            private CoordinateSystem validationCoordinateSystem;

            /// <summary>
            /// Number of rings in a polygon
            /// </summary>
            private int ringCount;

            /// <summary>
            /// First point's X coordinate
            /// </summary>
            private double initialFirstCoordinate;

            /// <summary>
            /// First point's Y coordinate
            /// </summary>
            private double initialSecondCoordinate;

            /// <summary>
            /// Last point's X coordinate
            /// </summary>
            private double mostRecentFirstCoordinate;

            /// <summary>
            /// Last point's Y coordinate
            /// </summary>
            private double mostRecentSecondCoordinate;

            /// <summary>
            /// we are validating a geography stream
            /// </summary>
            private bool processingGeography;

#if CURVE_SUPPORT
    /// <summary>
    /// Last point's Z coordinate
    /// </summary>
        private double? mostRecentZValue;
#endif

            /// <summary>
            /// Number of points in the GeoData
            /// </summary>
            private int pointCount;

            /// <summary>
            /// Stack depth
            /// </summary>
            private int depth;

            /// <summary>
            /// Constructs a new SpatialValidatorImplementation segment
            /// </summary>
            public NestedValidator()
            {
                InitializeObject();
            }

            /// <summary>
            /// Calls to the pipeline interface Represented as state transition
            /// </summary>
            private enum PipelineCall
            {
                /// <summary>
                /// Set CoordinateSystem
                /// </summary>
                SetCoordinateSystem,

                /// <summary>
                /// BeginGeo()
                /// </summary>
                /// <remarks>fake transition, just for exception</remarks>
                Begin,

                /// <summary>
                /// BeginGeo(point)
                /// </summary>
                BeginPoint,

                /// <summary>
                /// BeginGeo(LineString)
                /// </summary>
                BeginLineString,

                /// <summary>
                /// BeginGeo(Polygon)
                /// </summary>
                BeginPolygon,

                /// <summary>
                /// BeginGeo(MultiPoint)
                /// </summary>
                BeginMultiPoint,

                /// <summary>
                /// BeginGeo(MultiLineString)
                /// </summary>
                BeginMultiLineString,

                /// <summary>
                /// BeginGeo(MultiPolygon)
                /// </summary>
                BeginMultiPolygon,

                /// <summary>
                /// BeginGeo(Collection)
                /// </summary>
                BeginCollection,

                /// <summary>
                /// BeginGeo(FullGlobe)
                /// </summary>
                BeginFullGlobe,

                /// <summary>
                /// BeginFigure
                /// </summary>
                BeginFigure,

                /// <summary>
                /// LineTo
                /// </summary>
                LineTo,
#if CURVE_SUPPORT
            BeginCircularString,
            BeginCompoundCurve,
            BeginCurvePolygon,
            AddCircularArc,
            AddSegmentLine,
            AddSegmentArc,
            StartSimpleRing,
            StartCompoundCurveRing,
#endif
                /// <summary>
                /// EndFigure
                /// </summary>
                EndFigure,

                /// <summary>
                /// EndGeo
                /// </summary>
                End,
            }

            /// <summary>
            /// Implemented by a subclass to handle the setting of a coordinate system
            /// </summary>
            /// <param name="coordinateSystem">the new coordinate system</param>
            /// <returns>the coordinate system to be passed down the pipeline</returns>
            protected override CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                ValidatorState currentState = this.stack.Peek();
                Execute(PipelineCall.SetCoordinateSystem);

                if (currentState == CoordinateSystem)
                {
                    this.validationCoordinateSystem = coordinateSystem;
                }
                else if (this.validationCoordinateSystem != coordinateSystem)
                {
                    throw new FormatException(Strings.Validator_SridMismatch);
                }

                return coordinateSystem;
            }

            /// <summary>
            /// Implemented by a subclass to handle the start of drawing a Geography figure
            /// </summary>
            /// <param name="shape">the  shape to draw</param>
            /// <returns>the SpatialType to be passed down the pipeline</returns>
            protected override SpatialType OnBeginGeography(SpatialType shape)
            {
                if (this.depth > 0 && !this.processingGeography)
                {
                    throw new FormatException(Strings.Validator_UnexpectedGeometry);
                }

                this.processingGeography = true;
                BeginShape(shape);

                return shape;
            }

            /// <summary>
            /// Implemented by a subclass to handle the end of drawing a Geography figure
            /// </summary>
            protected override void OnEndGeography()
            {
                Execute(PipelineCall.End);
                if (!this.processingGeography)
                {
                    throw new FormatException(Strings.Validator_UnexpectedGeometry);
                }

                this.depth -= 1;
            }

            /// <summary>
            /// Implemented by a subclass to handle the start of drawing a Geometry figure
            /// </summary>
            /// <param name="shape">the  shape to draw</param>
            /// <returns>the SpatialType to be passed down the pipeline</returns>
            protected override SpatialType OnBeginGeometry(SpatialType shape)
            {
                if (this.depth > 0 && this.processingGeography)
                {
                    throw new FormatException(Strings.Validator_UnexpectedGeography);
                }

                this.processingGeography = false;
                BeginShape(shape);
                return shape;
            }

            /// <summary>
            /// Implemented by a subclass to handle the end of drawing a Geometry figure
            /// </summary>
            protected override void OnEndGeometry()
            {
                Execute(PipelineCall.End);
                if (this.processingGeography)
                {
                    throw new FormatException(Strings.Validator_UnexpectedGeography);
                }

                this.depth -= 1;
            }

            /// <summary>
            /// Implemented by a subclass to handle the start of a figure
            /// </summary>
            /// <param name="position">Next position</param>
            /// <returns>The position to be passed down the pipeline</returns>
            protected override GeographyPosition OnBeginFigure(GeographyPosition position)
            {
                BeginFigure(ValidateGeographyPosition, position.Latitude, position.Longitude, position.Z, position.M);
                return position;
            }

            /// <summary>
            /// Implemented by a subclass to handle the start of a figure
            /// </summary>
            /// <param name="position">Next position</param>
            /// <returns>The position to be passed down the pipeline</returns>
            protected override GeometryPosition OnBeginFigure(GeometryPosition position)
            {
                BeginFigure(ValidateGeometryPosition, position.X, position.Y, position.Z, position.M);
                return position;
            }

            /// <summary>
            /// Implemented by a subclass to handle the end of a figure
            /// </summary>
            protected override void OnEndFigure()
            {
                Execute(PipelineCall.EndFigure);
            }

            /// <summary>
            /// Implemented by a subclass to return to its initial state
            /// </summary>
            protected override void OnReset()
            {
                InitializeObject();
            }

            /// <summary>
            /// Implemented by a subclass to handle the addition of a waypoint to a Geography figure
            /// </summary>
            /// <param name="position">Next position</param>
            /// <returns>the GeographyPosition to be passed down the pipeline</returns>
            protected override GeographyPosition OnLineTo(GeographyPosition position)
            {
                if (this.processingGeography)
                {
                    ValidateGeographyPosition(position.Latitude, position.Longitude, position.Z, position.M);
                }

                AddControlPoint(position.Latitude, position.Longitude);

                if (!this.processingGeography)
                {
                    throw new FormatException(Strings.Validator_UnexpectedGeometry);
                }

                return position;
            }

            /// <summary>
            /// Implemented by a subclass to handle the addition of a waypoint to a Geometry figure
            /// </summary>
            /// <param name="position">Next position</param>
            /// <returns>the GeometryPosition to be passed down the pipeline</returns>
            protected override GeometryPosition OnLineTo(GeometryPosition position)
            {
                if (!this.processingGeography)
                {
                    ValidateGeometryPosition(position.X, position.Y, position.Z, position.M);
                }

                AddControlPoint(position.X, position.Y);

                if (this.processingGeography)
                {
                    throw new FormatException(Strings.Validator_UnexpectedGeography);
                }

                return position;
            }

            /// <summary>
            /// Test whether a double is finite
            /// </summary>
            /// <param name="value">The double value</param>
            /// <returns>True if the input double is not NaN or INF</returns>
            private static bool IsFinite(double value)
            {
                return !double.IsNaN(value) && !double.IsInfinity(value);
            }

            /// <summary>
            /// Test whether a point is in valid format
            /// </summary>
            /// <param name="first">The first coordinate</param>
            /// <param name="second">The second coordinate</param>
            /// <param name="z">The z coordinate</param>
            /// <param name="m">The m coordinate</param>
            /// <returns>Whether the input coordinate is valid</returns>
            [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
            private static bool IsPointValid(double first, double second, double? z, double? m)
            {
                return IsFinite(first) && IsFinite(second) && (z == null || IsFinite(z.Value)) &&
                       (m == null || IsFinite(m.Value));
            }

            /// <summary>
            /// Validate one position
            /// </summary>
            /// <param name="first">the first two dimensional co-ordinate</param>
            /// <param name="second">the second two dimensional co-ordinate</param>
            /// <param name="z">the altitude</param>
            /// <param name="m">the measure</param>
            private static void ValidateOnePosition(double first, double second, double? z, double? m)
            {
                if (!IsPointValid(first, second, z, m))
                {
                    throw new FormatException(Strings.Validator_InvalidPointCoordinate(first, second, z, m));
                }
            }

            /// <summary>
            /// Validate one Geography position
            /// </summary>
            /// <param name="latitude">the latitude</param>
            /// <param name="longitude">the longitude</param>
            /// <param name="z">the altitude</param>
            /// <param name="m">the measure</param>
            private static void ValidateGeographyPosition(double latitude, double longitude, double? z, double? m)
            {
                ValidateOnePosition(latitude, longitude, z, m);
                if (!IsLatitudeValid(latitude))
                {
                    throw new FormatException(Strings.Validator_InvalidLatitudeCoordinate(latitude));
                }

                if (!IsLongitudeValid(longitude))
                {
                    throw new FormatException(Strings.Validator_InvalidLongitudeCoordinate(longitude));
                }
            }

            /// <summary>
            /// Validate one Geography position
            /// </summary>
            /// <param name="x">the x coordinate</param>
            /// <param name="y">the y coordinate</param>
            /// <param name="z">the altitude</param>
            /// <param name="m">the measure</param>
            private static void ValidateGeometryPosition(double x, double y, double? z, double? m)
            {
                ValidateOnePosition(x, y, z, m);
            }

            /// <summary>
            /// Test whether a latitude value is within acceptable range
            /// </summary>
            /// <param name="latitude">The latitude value</param>
            /// <returns>True if the latitude value is within range</returns>
            private static bool IsLatitudeValid(double latitude)
            {
                return latitude >= -MaxLatitude && latitude <= MaxLatitude;
            }

            /// <summary>
            /// Test whether a longitude value is within acceptable range
            /// </summary>
            /// <param name="longitude">The longitude value</param>
            /// <returns>True if the longitude value is within range</returns>
            private static bool IsLongitudeValid(double longitude)
            {
                return longitude >= -MaxLongitude && longitude <= MaxLongitude;
            }

            /// <summary>
            /// Validate a Geography polygon
            /// </summary>
            /// <param name="numOfPoints">The number of points in the ring</param>
            /// <param name="initialFirstCoordinate">its first latitude</param>
            /// <param name="initialSecondCoordinate">it first longitued</param>
            /// <param name="mostRecentFirstCoordinate">its last latitude</param>
            /// <param name="mostRecentSecondCoordinate">its last longitude</param>
            private static void ValidateGeographyPolygon(
                int numOfPoints,
                double initialFirstCoordinate,
                double initialSecondCoordinate,
                double mostRecentFirstCoordinate,
                double mostRecentSecondCoordinate)
            {
                if (numOfPoints < 4 || initialFirstCoordinate != mostRecentFirstCoordinate ||
                    !AreLongitudesEqual(initialSecondCoordinate, mostRecentSecondCoordinate))
                {
                    throw new FormatException(Strings.Validator_InvalidPolygonPoints);
                }
            }

            /// <summary>
            /// Validate a Geometry polygon
            /// </summary>
            /// <param name="numOfPoints">The number of points in the ring</param>
            /// <param name="initialFirstCoordinate">its first x</param>
            /// <param name="initialSecondCoordinate">it first y</param>
            /// <param name="mostRecentFirstCoordinate">its last x</param>
            /// <param name="mostRecentSecondCoordinate">its last y</param>
            private static void ValidateGeometryPolygon(
                int numOfPoints,
                double initialFirstCoordinate,
                double initialSecondCoordinate,
                double mostRecentFirstCoordinate,
                double mostRecentSecondCoordinate)
            {
                if (numOfPoints < 4 || initialFirstCoordinate != mostRecentFirstCoordinate ||
                    initialSecondCoordinate != mostRecentSecondCoordinate)
                {
                    throw new FormatException(Strings.Validator_InvalidPolygonPoints);
                }
            }

            /// <summary>
            /// Test whether two longitude values are equal
            /// </summary>
            /// <param name="left">Left longitude</param>
            /// <param name="right">Right longitude</param>
            /// <returns>True if the two longitudes are equals</returns>
            private static bool AreLongitudesEqual(double left, double right)
            {
                return left == right || (left - right) % 360 == 0;
            }

            /// <summary>
            /// Begins the figure.
            /// </summary>
            /// <param name="validate">The validate action.</param>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="z">The z.</param>
            /// <param name="m">The m.</param>
            private void BeginFigure(Action<double, double, double?, double?> validate, double x, double y, double? z, double? m)
            {
                validate(x, y, z, m);
                this.Execute(PipelineCall.BeginFigure);
                this.pointCount = 0;
                this.TrackPosition(x, y);
            }

            /// <summary>
            /// Begin drawing a spatial object
            /// </summary>
            /// <param name="type">The spatial type of the object</param>
            private void BeginShape(SpatialType type)
            {
                this.depth += 1;

                switch (type)
                {
                    case SpatialType.Point:
                        Execute(PipelineCall.BeginPoint);
                        break;
                    case SpatialType.LineString:
                        Execute(PipelineCall.BeginLineString);
                        break;
                    case SpatialType.Polygon:
                        Execute(PipelineCall.BeginPolygon);
                        break;
                    case SpatialType.MultiPoint:
                        Execute(PipelineCall.BeginMultiPoint);
                        break;
                    case SpatialType.MultiLineString:
                        Execute(PipelineCall.BeginMultiLineString);
                        break;
                    case SpatialType.MultiPolygon:
                        Execute(PipelineCall.BeginMultiPolygon);
                        break;
                    case SpatialType.Collection:
                        Execute(PipelineCall.BeginCollection);
                        break;
                    case SpatialType.FullGlobe:
                        if (!this.processingGeography)
                        {
                            throw new FormatException(Strings.Validator_InvalidType(type));
                        }

                        Execute(PipelineCall.BeginFullGlobe);
                        break;
#if CURVE_SUPPORT
                case SpatialType.CircularString:
                    Execute(Transition.Begin_CircularString);
                    break;

                case SpatialType.CompoundCurve:
                    Execute(Transition.Begin_CompoundCurve);
                    break;

                case SpatialType.CurvePolygon:
                    Execute(Transition.Begin_CurvePolygon);
                    break;
#endif
                    default:
                        throw new FormatException(Strings.Validator_InvalidType(type));
                }
            }

            /// <summary>
            ///  Add a control point to the current figure.
            /// </summary>
            /// <param name="first">the first coordinate</param>
            /// <param name="second">the second coordinate</param>
            private void AddControlPoint(double first, double second)
            {
                Execute(PipelineCall.LineTo);

                TrackPosition(first, second);
            }

            /// <summary>
            /// Tracks the position.
            /// </summary>
            /// <param name="first">The first.</param>
            /// <param name="second">The second.</param>
            private void TrackPosition(double first, double second)
            {
                if (this.pointCount == 0)
                {
                    this.initialFirstCoordinate = first;
                    this.initialSecondCoordinate = second;
                }

                this.mostRecentFirstCoordinate = first;
                this.mostRecentSecondCoordinate = second;
#if CURVE_SUPPORT
            this.mostRecentZValue = z;
#endif
                this.pointCount += 1;
            }

            /// <summary>
            /// Transit into a new state
            /// </summary>
            /// <param name="transition">The state to transit into</param>
            private void Execute(PipelineCall transition)
            {
                ValidatorState state = this.stack.Peek();
                state.ValidateTransition(transition, this);
            }

            /// <summary>
            ///  initialize the object to a fresh clean smelling state
            /// </summary>
            private void InitializeObject()
            {
                this.depth = default(int);
                this.initialFirstCoordinate = default(double);
                this.initialSecondCoordinate = default(double);
                this.mostRecentFirstCoordinate = default(double);
                this.mostRecentSecondCoordinate = default(double);
                this.pointCount = default(int);
                this.validationCoordinateSystem = null;
                this.ringCount = default(int);

                this.stack.Clear();
                this.stack.Push(CoordinateSystem);
            }

            /// <summary>
            /// Push a new state onto the stack
            /// </summary>
            /// <param name="state">The new state</param>
            private void Call(ValidatorState state)
            {
                if (this.stack.Count > MaxGeometryCollectionDepth)
                {
                    throw new FormatException(Strings.Validator_NestingOverflow(MaxGeometryCollectionDepth));
                }

                this.stack.Push(state);
            }

            /// <summary>
            /// Pop a state from the stack
            /// </summary>
            private void Return()
            {
                this.stack.Pop();
                Debug.Assert(this.stack.Count > 0, "the stack should always have the SetCoordinateSystemState");
            }

            /// <summary>
            /// Replace the current state on the stack with the new state
            /// </summary>
            /// <param name="state">The new state</param>
            private void Jump(ValidatorState state)
            {
                this.stack.Pop();
                Debug.Assert(this.stack.Count > 0, "the stack should always have the SetCoordinateSystemState");
                this.stack.Push(state);
            }

            #region State Validation

            /// <summary>
            /// SpatialValidatorImplementation State
            /// </summary>
            private abstract class ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal abstract void ValidateTransition(
                    PipelineCall transition,
                    NestedValidator validator);

                /// <summary>
                /// Throw an incorrect state exception
                /// </summary>
                /// <param name="transition">The expected state</param>
                /// <param name="actual">The actual state</param>
                protected static void ThrowExpected(PipelineCall transition, PipelineCall actual)
                {
                    throw new FormatException(Strings.Validator_UnexpectedCall(transition, actual));
                }

                /// <summary>
                /// Throw an incorrect state exception
                /// </summary>
                /// <param name="transition1">The expected state1</param>
                /// <param name="transition2">The expected state2</param>
                /// <param name="actual">The actual state</param>
                protected static void ThrowExpected(
                    PipelineCall transition1,
                    PipelineCall transition2,
                    PipelineCall actual)
                {
                    throw new FormatException(Strings.Validator_UnexpectedCall2(transition1, transition2, actual));
                }

                /// <summary>
                /// Throw an incorrect state exception
                /// </summary>
                /// <param name="transition1">The expected state1</param>
                /// <param name="transition2">The expected state2</param>
                /// <param name="transition3">The expected state3</param>
                /// <param name="actual">The actual state</param>
                protected static void ThrowExpected(
                    PipelineCall transition1,
                    PipelineCall transition2,
                    PipelineCall transition3,
                    PipelineCall actual)
                {
                    throw new FormatException(Strings.Validator_UnexpectedCall2(transition1 + ", " + transition2, transition3, actual));
                }
            }

            /// <summary>
            /// SetCoordinateSystem State
            /// Validator is currently waiting for a SetCoordinateSystemCall
            /// </summary>
            private class SetCoordinateSystemState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(
                    PipelineCall transition,
                    NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.SetCoordinateSystem:
                            validator.Call(BeginSpatial);
                            return;
                        default:
                            ThrowExpected(PipelineCall.SetCoordinateSystem, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// Beginning a GeoData
            /// Validator is currently waiting for a BeginGeo() call
            /// </summary>
            private class BeginGeoState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(
                    PipelineCall transition,
                    NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.BeginPoint:
                            validator.Jump(PointStart);
                            return;
                        case PipelineCall.BeginLineString:
                            validator.Jump(LineStringStart);
                            return;
                        case PipelineCall.BeginPolygon:
                            validator.Jump(PolygonStart);
                            return;
                        case PipelineCall.BeginMultiPoint:
                            validator.Jump(MultiPoint);
                            return;
                        case PipelineCall.BeginMultiLineString:
                            validator.Jump(MultiLineString);
                            return;
                        case PipelineCall.BeginMultiPolygon:
                            validator.Jump(MultiPolygon);
                            return;
                        case PipelineCall.BeginCollection:
                            validator.Jump(Collection);
                            return;
                        case PipelineCall.BeginFullGlobe:
                            if (validator.depth != 1)
                            {
                                throw new FormatException(Strings.Validator_FullGlobeInCollection);
                            }

                            validator.Jump(FullGlobe);
                            return;
#if CURVE_SUPPORT
                case Transition.BeginCircularString:
                    Jump(CircularString);
                    return;
                case Transition.BeginCompoundCurve:
                    Jump(CompoundCurve);
                    return;
                case Transition.BeginCurvePolygon:
                    Jump(CurvePolygon);
                    return;
#endif
                        default:
                            ThrowExpected(PipelineCall.Begin, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// Point Start State
            /// After BeginGeo(Point), waiting for BeginFigure() or EndGeo()
            /// </summary>
            private class PointStartState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.BeginFigure:
                            validator.Jump(PointBuilding);
                            return;
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.BeginFigure, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// Point Building State
            /// After BeginFigure(), waiting for EndFigure() immediately
            /// </summary>
            private class PointBuildingState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.LineTo:
                            if (validator.pointCount != 0)
                            {
                                ThrowExpected(PipelineCall.EndFigure, transition);
                            }

                            return;

                        case PipelineCall.EndFigure:
                            if (validator.pointCount == 0)
                            {
                                ThrowExpected(PipelineCall.BeginFigure, transition);
                            }

                            validator.Jump(PointEnd);
                            return;

                        default:
                            ThrowExpected(PipelineCall.EndFigure, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// Point End State
            /// After EndFigure() for a point, waiting for EndGeo()
            /// </summary>
            private class PointEndState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// LineString Start state
            /// After BeginGeo(LineString), waiting for BeginFigure/EndGeo
            /// </summary>
            private class LineStringStartState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.BeginFigure:
                            validator.Jump(LineStringBuilding);
                            return;
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.BeginFigure, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// LineString Building State
            /// After BeginFigure() for a line
            /// Waiting for LineTo/EndFigure
            /// </summary>
            private class LineStringBuildingState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.LineTo:
                            return;
                        case PipelineCall.EndFigure:
                            if (validator.pointCount < 2)
                            {
                                throw new FormatException(Strings.Validator_LineStringNeedsTwoPoints);
                            }

                            validator.Jump(LineStringEnd);
                            return;
                        default:
                            ThrowExpected(PipelineCall.LineTo, PipelineCall.EndFigure, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// LineString End State
            /// After EndFigure() on Line
            /// Waiting for EndGeo
            /// </summary>
            private class LineStringEndState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// PolygonStart State
            /// After polygon started, waiting for Rings to build
            /// </summary>
            private class PolygonStartState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.BeginFigure:
                            validator.Jump(PolygonBuilding);
                            return;
                        case PipelineCall.End:
                            validator.ringCount = 0;
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.BeginFigure, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// Polygon Building State
            /// Drawing rings
            /// </summary>
            private class PolygonBuildingState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.LineTo:
                            return;
                        case PipelineCall.EndFigure:
                            validator.ringCount += 1;
                            if (validator.processingGeography)
                            {
                                ValidateGeographyPolygon(
                                    validator.pointCount,
                                    validator.initialFirstCoordinate,
                                    validator.initialSecondCoordinate,
                                    validator.mostRecentFirstCoordinate,
                                    validator.mostRecentSecondCoordinate);
                            }
                            else
                            {
                                ValidateGeometryPolygon(
                                    validator.pointCount,
                                    validator.initialFirstCoordinate,
                                    validator.initialSecondCoordinate,
                                    validator.mostRecentFirstCoordinate,
                                    validator.mostRecentSecondCoordinate);
                            }

                            validator.Jump(PolygonStart);
                            return;
                        default:
                            ThrowExpected(PipelineCall.LineTo, PipelineCall.EndFigure, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// MultiPoint State
            /// Inside a MultiPoint Container
            /// </summary>
            private class MultiPointState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.SetCoordinateSystem:
                            return;
                        case PipelineCall.BeginPoint:
                            validator.Call(PointStart);
                            return;
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.SetCoordinateSystem, PipelineCall.BeginPoint, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// MultiLineString State
            /// Inside a MultiLineString container
            /// </summary>
            private class MultiLineStringState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.SetCoordinateSystem:
                            return;
                        case PipelineCall.BeginLineString:
                            validator.Call(LineStringStart);
                            return;
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.SetCoordinateSystem, PipelineCall.BeginLineString, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// MultiPolygon State
            /// Inside a MultiPolygon container
            /// </summary>
            private class MultiPolygonState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.SetCoordinateSystem:
                            return;
                        case PipelineCall.BeginPolygon:
                            validator.Call(PolygonStart);
                            return;
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            ThrowExpected(PipelineCall.SetCoordinateSystem, PipelineCall.BeginPolygon, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// Collection State
            /// Inside a Collection container
            /// </summary>
            private class CollectionState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.SetCoordinateSystem:
                            return;
                        case PipelineCall.BeginPoint:
                            validator.Call(PointStart);
                            return;
                        case PipelineCall.BeginLineString:
                            validator.Call(LineStringStart);
                            return;
                        case PipelineCall.BeginPolygon:
                            validator.Call(PolygonStart);
                            return;
                        case PipelineCall.BeginMultiPoint:
                            validator.Call(MultiPoint);
                            return;
                        case PipelineCall.BeginMultiLineString:
                            validator.Call(MultiLineString);
                            return;
                        case PipelineCall.BeginMultiPolygon:
                            validator.Call(MultiPolygon);
                            return;
                        case PipelineCall.BeginCollection:
                            validator.Call(Collection);
                            return;
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        case PipelineCall.BeginFullGlobe:
                            throw new FormatException(Strings.Validator_FullGlobeInCollection);
#if CURVE_SUPPORT
                case Transition.Begin_CircularString:
                    Call(CircularString);
                    return;

                case Transition.Begin_CompoundCurve:
                    Call(CompoundCurve);
                    return;

                case Transition.Begin_CurvePolygon:
                    Call(CurvePolygon);
                    return;
#endif
                        default:
                            ThrowExpected(PipelineCall.SetCoordinateSystem, PipelineCall.Begin, PipelineCall.End, transition);
                            return;
                    }
                }
            }

            /// <summary>
            /// FullGlobe state
            /// Inside a FullGlobe container
            /// </summary>
            private class FullGlobeState : ValidatorState
            {
                /// <summary>
                /// Validate a call to the pipeline interface (a state transition)
                /// </summary>
                /// <param name="transition">The transition</param>
                /// <param name="validator">The validator instance</param>
                internal override void ValidateTransition(PipelineCall transition, NestedValidator validator)
                {
                    switch (transition)
                    {
                        case PipelineCall.End:
                            validator.Return();
                            return;
                        default:
                            throw new FormatException(Strings.Validator_FullGlobeCannotHaveElements);
                    }
                }
            }

            #endregion
        }
    }
}
