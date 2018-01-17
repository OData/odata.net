//---------------------------------------------------------------------
// <copyright file="GeoJsonWriterAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using Microsoft.Spatial;

    /// <summary>
    /// Convert writer interface from IJsonWriter to IGeoJsonWriter.
    /// This enables writer instance to be passed to Spatial to boost writing performance.
    /// </summary>
    internal sealed class GeoJsonWriterAdapter : IGeoJsonWriter
    {
        /// <summary>
        /// Inner writer of interface IJsonWriter.
        /// </summary>
        private readonly IJsonWriter writer;

        /// <summary>
        /// Constructor (only accessible from OData.Core
        /// </summary>
        /// <param name="writer">Inner writer of interface IJsonWriter.</param>
        internal GeoJsonWriterAdapter(IJsonWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Start the object scope.
        /// </summary>
        void IGeoJsonWriter.StartObjectScope()
        {
            this.writer.StartObjectScope();
        }

        /// <summary>
        /// End the current object scope.
        /// </summary>
        void IGeoJsonWriter.EndObjectScope()
        {
            this.writer.EndObjectScope();
        }

        /// <summary>
        /// Start the array scope.
        /// </summary>
        void IGeoJsonWriter.StartArrayScope()
        {
            this.writer.StartArrayScope();
        }

        /// <summary>
        /// End the current array scope.
        /// </summary>
        void IGeoJsonWriter.EndArrayScope()
        {
            this.writer.EndArrayScope();
        }

        /// <summary>
        /// Add a property name to the current json object.
        /// </summary>
        /// <param name="name">The name to add.</param>
        void IGeoJsonWriter.AddPropertyName(string name)
        {
            this.writer.WriteName(name);
        }

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void IGeoJsonWriter.AddValue(double value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void IGeoJsonWriter.AddValue(string value)
        {
            this.writer.WriteValue(value);
        }
    }
}
