//---------------------------------------------------------------------
// <copyright file="ObjectGraphData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data contained in Object Graphs
    /// </summary>
    public class ObjectGraphData
    {
        private List<GraphNode> graphNodes;
        private ReadOnlyCollection<GraphNode> readonlyGraphNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectGraphData"/> class.
        /// </summary>
        public ObjectGraphData()
        {
            this.graphNodes = new List<GraphNode>();
            this.readonlyGraphNodes = new ReadOnlyCollection<GraphNode>(this.graphNodes);
        }

        /// <summary>
        /// Gets the graph nodes contained in ObjectGraphData
        /// </summary>
        /// <value>The graph nodes.</value>
        public IEnumerable<GraphNode> GraphNodes 
        {
            get
            {
                return this.readonlyGraphNodes;        
            }
        }

        /// <summary>
        /// Creates a GraphNode for the instance passed in
        /// and adds it to ObjectGraphData
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The GraphNode instance that was created</returns>
        public GraphNode CreateGraphNode(object instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            GraphNode node = new GraphNode(instance, this);
            this.graphNodes.Add(node);
            return node;
        }

        /// <summary>
        /// Creates a GraphNode and adds it to ObjectGraphData
        /// </summary>
        /// <param name="type">The type of the instance that this GraphNode represents</param>
        /// <returns>The GraphNode instance that was created</returns>
        public GraphNode CreateGraphNode(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            GraphNode node = new GraphNode(type, this);
            this.graphNodes.Add(node);
            return node;
        }

        /// <summary>
        /// Gets the GraphNode corresponding to the instance passed in
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A GraphNode instance</returns>
        public GraphNode GetGraphNode(object instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            GraphNode graphNode = null;
            if (!this.TryGetGraphNode(instance, out graphNode))
            {
                throw new TaupoInvalidOperationException("The instance is not tracked in Object Graph Data!");   
            }

            return graphNode;
        }

        /// <summary>
        /// Tries the get GraphNode based on the underlying instance
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="graphNode">The graph node.</param>
        /// <returns>true, if the GraphNode corresponding to the instance was found, false if not</returns>
        public bool TryGetGraphNode(object instance, out GraphNode graphNode)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            graphNode = this.graphNodes.Where(e => object.ReferenceEquals(e.Instance, instance)).SingleOrDefault();

            return graphNode != null;
        }

        /// <summary>
        /// Removes the GraphNode corresponding to the specified instance
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A GraphNode instance</returns>
        public GraphNode RemoveGraphNode(object instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            
            GraphNode graphNode = null;
            if (!this.TryGetGraphNode(instance, out graphNode))
            {
                throw new TaupoInvalidOperationException("Could not remove the instance from Object Graph Data as it was not found!");
            }

            this.graphNodes.Remove(graphNode);
            return graphNode;
        }
    }
}