//---------------------------------------------------------------------
// <copyright file="SpatialTreeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Tree based builder for spatial types
    /// </summary>
    /// <typeparam name="T">Geography or Geometry</typeparam>
    internal abstract class SpatialTreeBuilder<T> : TypeWashedPipeline where T : class, ISpatial
    {
        /// <summary>
        /// The figure this builder is currently building
        /// </summary>
        private List<T> currentFigure;

        /// <summary>
        /// Current builder tree root
        /// </summary>
        private SpatialBuilderNode currentNode;

        /// <summary>
        /// lastConstructed
        /// </summary>
        private SpatialBuilderNode lastConstructedNode;

        /// <summary>
        /// Fires when the builder creates a top-level spatial object.
        /// </summary>
        public event Action<T> ProduceInstance;

        /// <summary>
        /// Get the constructed spatial instance
        /// </summary>
        /// <returns>The constructed spatial instance</returns>
        public T ConstructedInstance
        {
            get
            {
                if (this.lastConstructedNode == null || this.lastConstructedNode.Instance == null || this.lastConstructedNode.Parent != null)
                {
                    throw new InvalidOperationException(Strings.SpatialBuilder_CannotCreateBeforeDrawn);
                }

                return this.lastConstructedNode.Instance;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is geography.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is geography; otherwise, <c>false</c>.
        /// </value>
        public override bool IsGeography
        {
            get { return typeof(Geography).IsAssignableFrom(typeof(T)); }
        }

        /// <summary>
        /// Draw a point in the specified coordinate
        /// </summary>
        /// <param name="x">X or Latitude Coordinate</param>
        /// <param name="y">Y or Longitude Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="m">M Coordinate</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        internal override void LineTo(double x, double y, double? z, double? m)
        {
            this.currentFigure.Add(this.CreatePoint(false, x, y, z, m));
        }

        /// <summary>
        /// Begin drawing a figure
        /// </summary>
        /// <param name="coordinate1">X or Latitude Coordinate</param>
        /// <param name="coordinate2">Y or Longitude Coordinate</param>
        /// <param name="coordinate3">Z Coordinate</param>
        /// <param name="coordinate4">M Coordinate</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
        internal override void BeginFigure(double coordinate1, double coordinate2, double? coordinate3, double? coordinate4)
        {
            if (this.currentFigure == null)
            {
                this.currentFigure = new List<T>();
            }

            this.currentFigure.Add(this.CreatePoint(false, coordinate1, coordinate2, coordinate3, coordinate4));
        }

        /// <summary>
        /// Begin a new spatial type
        /// </summary>
        /// <param name="type">The spatial type</param>
        internal override void BeginGeo(SpatialType type)
        {
            Debug.Assert(type != SpatialType.Unknown, "cannot build unknown type - should have validated this call");

            // traverse down the tree);
            if (this.currentNode == null)
            {
                this.currentNode = new SpatialBuilderNode { Type = type };

                // we are just getting started, clear the last built
                this.lastConstructedNode = null;
            }
            else
            {
                this.currentNode = this.currentNode.CreateChildren(type);
            }
        }

        /// <summary>
        /// Ends the figure set on the current node
        /// </summary>
        internal override void EndFigure()
        {
            Debug.Assert(this.currentFigure != null, "current figure is null - call AddPointToNode before ending a figure");

            // Attach the figure to the current node
            if (this.currentFigure.Count == 1)
            {
                // point
                SpatialBuilderNode pointNode = this.currentNode.CreateChildren(SpatialType.Point);
                pointNode.Instance = this.currentFigure[0];
            }
            else
            {
                SpatialBuilderNode lineStringNode = this.currentNode.CreateChildren(SpatialType.LineString);
                lineStringNode.Instance = this.CreateShapeInstance(SpatialType.LineString, this.currentFigure);
            }

            // need to clear current figure, since next geography may not contain a figure
            this.currentFigure = null;
        }

        /// <summary>
        /// Ends the current spatial object
        /// </summary>
        internal override void EndGeo()
        {
            // Create the geography instance
            switch (this.currentNode.Type)
            {
                case SpatialType.Point:
                    this.currentNode.Instance = this.currentNode.Children.Count > 0 ? this.currentNode.Children[0].Instance : this.CreatePoint(true, double.NaN, double.NaN, null, null);
                    break;
                case SpatialType.LineString:
                    this.currentNode.Instance = this.currentNode.Children.Count > 0 ? this.currentNode.Children[0].Instance : this.CreateShapeInstance(SpatialType.LineString, new T[0]);
                    break;
                case SpatialType.Polygon:
                case SpatialType.MultiPoint:
                case SpatialType.MultiLineString:
                case SpatialType.MultiPolygon:
                case SpatialType.Collection:
                    this.currentNode.Instance = this.CreateShapeInstance(this.currentNode.Type, this.currentNode.Children.Select(node => node.Instance));
                    break;
                case SpatialType.FullGlobe:
                    this.currentNode.Instance = this.CreateShapeInstance(SpatialType.FullGlobe, new T[0]);
                    break;
                default:
                    Debug.Assert(false, "Unknown geography type");
                    break;
            }

            this.TraverseUpTheTree();
            this.NotifyIfWeJustFinishedBuildingSomething();
        }

        /// <summary>
        /// Setup the pipeline for reuse
        /// </summary>
        internal override void Reset()
        {
            this.currentNode = null;
            this.currentFigure = null;
        }

        /// <summary>
        /// Create a new instance of Point
        /// </summary>
        /// <param name="isEmpty">Whether the point is empty</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <param name="m">M</param>
        /// <returns>A new instance of point</returns>
        protected abstract T CreatePoint(bool isEmpty, double x, double y, double? z, double? m);

        /// <summary>
        /// Create a new instance of T
        /// </summary>
        /// <param name="type">The spatial type to create</param>
        /// <param name="spatialData">The arguments</param>
        /// <returns>A new instance of T</returns>
        protected abstract T CreateShapeInstance(SpatialType type, IEnumerable<T> spatialData);

        /// <summary>
        /// Notifies if we just finished building something.
        /// </summary>
        private void NotifyIfWeJustFinishedBuildingSomething()
        {
            if (this.currentNode == null && this.ProduceInstance != null)
            {
                this.ProduceInstance(this.lastConstructedNode.Instance);
            }
        }

        /// <summary>
        /// Traverses up the tree.
        /// </summary>
        private void TraverseUpTheTree()
        {
            this.lastConstructedNode = this.currentNode;
            this.currentNode = this.currentNode.Parent;
        }

        /// <summary>
        /// A spatial instance node in the builder tree
        /// </summary>
        private class SpatialBuilderNode
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public SpatialBuilderNode()
            {
                this.Children = new List<SpatialBuilderNode>();
            }

            /// <summary>
            /// Children nodes
            /// </summary>
            public List<SpatialBuilderNode> Children { get; private set; }

            /// <summary>
            /// Instance
            /// </summary>
            public T Instance { get; set; }

            /// <summary>
            /// Parent node
            /// </summary>
            public SpatialBuilderNode Parent { get; private set; }

            /// <summary>
            /// Spatial Type
            /// </summary>
            public SpatialType Type { get; set; }

            /// <summary>
            /// Create a child node
            /// </summary>
            /// <param name="type">The node type</param>
            /// <returns>The child node</returns>
            internal SpatialBuilderNode CreateChildren(SpatialType type)
            {
                var node = new SpatialBuilderNode { Parent = this, Type = type };
                this.Children.Add(node);
                return node;
            }
        }
    }
}