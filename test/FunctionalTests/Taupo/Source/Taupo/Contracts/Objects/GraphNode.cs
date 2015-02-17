//---------------------------------------------------------------------
// <copyright file="GraphNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data for an Object Graph Node
    /// </summary>
    public class GraphNode
    {
        private Dictionary<string, object> contents = new Dictionary<string, object>();
        private ObjectGraphData graphData = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNode"/> class.
        /// </summary>
        /// <param name="value">The underlying object instance of the Graph Node</param>
        /// <param name="data">An instance of ObjectGraphData, which contains this GraphNode</param>
        internal GraphNode(object value, ObjectGraphData data)
        {
            this.Instance = value;
            this.InstanceType = value.GetType();
            this.graphData = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNode"/> class.
        /// </summary>
        /// <param name="type">The type of the underlying object instance of the Graph Node</param>
        /// <param name="data">An instance of ObjectGraphData, which contains this GraphNode</param>
        internal GraphNode(Type type, ObjectGraphData data)
        {
            this.InstanceType = type;
            this.graphData = data;
        }

        /// <summary>
        /// Gets the underlying instance.
        /// </summary>
        /// <value>The underlying instance of the Graph Node</value>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the type of the underlying instance.
        /// </summary>
        /// <value>The type of the underlying instance.</value>
        public Type InstanceType { get; private set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public IEnumerable<string> Properties 
        {
            get
            {
                return this.contents.Keys;
            }
        } 

        /// <summary>
        /// Gets the value of the propery specified.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value for the specified property.</returns>
        public object GetProperty(string propertyName)
        {
            object value;

            if (!this.TryGetProperty(propertyName, out value))
            {
                throw new TaupoInvalidOperationException("The specified property does not exist in this Graph Node. Property Name: " + propertyName);
            }

            return value;
        }

        /// <summary>
        /// Tries to get the value for the given property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>true, if the property was found, false if not</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Property has to be of object type.")]
        public bool TryGetProperty(string propertyName, out object value)
        {
            return this.contents.TryGetValue(propertyName, out value);
        }

        /// <summary>
        /// Sets a property on the Graph Node
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The instance of GraphNode on which SetProperty was invoked
        /// </returns>
        public GraphNode SetProperty(string propertyName, object value)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");
            object valueToSet = null;

            if (value != null)
            {
                if (DataUtilities.IsAnonymousType(value.GetType()))
                {
                    throw new TaupoArgumentException("Invalid argument type, cannot use anonymous type for Set Property method!");
                }

                if (TypeUtilities.IsPrimitiveType(value.GetType()))
                {
                    valueToSet = value;
                }
                else
                {
                    GraphNode graphNode = value as GraphNode;
                    if (graphNode == null)
                    {
                        bool graphNodeFound = this.graphData.TryGetGraphNode(value, out graphNode);
                        if (!graphNodeFound)
                        {
                            throw new TaupoArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid argument type {0} specified! Arguments must either be primitive/Graph Node types", value.GetType()));
                        }
                    }

                    valueToSet = graphNode;
                }
            }

            this.contents[propertyName] = valueToSet;
            return this;
        }

        /// <summary>
        /// Adds a collection of objects(typically GraphNodes) to a specified property name in GraphNode
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="values">A collection of objects(typically GraphNodes)</param>
        /// <returns>The instance of GraphNode on which AddToCollection was invoked</returns>
        public GraphNode AddToCollection(string propertyName, params object[] values)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");
            ExceptionUtilities.CheckCollectionNotEmpty(values, "values");

            // Confirm the values in the list of values passed in match the allowed types for this method
            List<object> valuesToSet = this.TransformCollectionContents(values);

            object value = null;
            this.contents.TryGetValue(propertyName, out value);
            var collection = value as List<object>;
            if (collection == null)
            {
                collection = new List<object>();
                this.contents.Add(propertyName, collection);
            }

            foreach (var item in valuesToSet)
            {
                collection.Add(item);
            }

            return this;
        }
        
        /// <summary>
        /// Removes the specified instances from a collection
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="values">A collection of GraphNodes to remove from the property</param>
        /// <returns>
        /// The instance of GraphNode on which AddToCollection was invoked
        /// </returns>
        public GraphNode RemoveFromCollection(string propertyName, params object[] values)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");
            ExceptionUtilities.CheckCollectionNotEmpty(values, "values");

            // Confirm the values in the list of values passed in match the allowed types for this method
            List<object> valuesToRemove = this.TransformCollectionContents(values);

            object value = null;
            this.contents.TryGetValue(propertyName, out value);
            if (value == null)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to remove object from {0} collection property as this collection is not present in this Graph Node!", propertyName));
            }

            var collection = (List<object>)value;
            ExceptionUtilities.CheckObjectNotNull(collection, string.Format(CultureInfo.InvariantCulture, "Unable to remove GraphNode(s) from {0} collection as collection was null!", propertyName));
            foreach (var propertyValue in valuesToRemove)
            {
                int index = collection.IndexOf(propertyValue);
                if (index < 0)
                {
                    throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to remove Graph Node as it is not found in the collection {0}!", propertyName));
                }
                else
                {
                    collection.Remove(propertyValue);
                }
            }

            if (collection.Count == 0)
            {
                this.contents.Remove(propertyName); 
            }

            return this;
        }

        private List<object> TransformCollectionContents(object[] values)
        {
            List<object> valuesToSet = new List<object>(values);
            foreach (var item in values)
            {
                if (item != null)
                {
                    if (DataUtilities.IsAnonymousType(item.GetType()))
                    {
                        throw new TaupoArgumentException("Invalid arguments passed in, anonymous types are not supported as part of collection contents!");
                    }

                    if (!TypeUtilities.IsPrimitiveType(item.GetType()))
                    {
                        GraphNode graphNode = item as GraphNode;
                        if (graphNode == null)
                        {
                            bool graphNodeFound = this.graphData.TryGetGraphNode(item, out graphNode);
                            if (!graphNodeFound)
                            {
                                throw new TaupoArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid argument type {0} specified! Arguments must either be primitive/Graph Node types", item.GetType()));
                            }

                            valuesToSet.Remove(item);
                            valuesToSet.Add(graphNode);
                        }
                    }
                }
            }

            return valuesToSet;
        }
    }
}
