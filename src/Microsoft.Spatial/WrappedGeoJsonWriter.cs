//---------------------------------------------------------------------
// <copyright file="WrappedGeoJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// Writer to convert spatial types to Json that wraps a JsonWriter.
    /// </summary>
    internal sealed class WrappedGeoJsonWriter : GeoJsonWriterBase
    {
        #region Private Fields

        /// <summary>
        /// The actual stream to write Json.
        /// </summary>
        private readonly IGeoJsonWriter writer;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The actual stream to write Json.</param>
        public WrappedGeoJsonWriter(IGeoJsonWriter writer)
        {
            this.writer = writer;
        }

        #region Override Methods

        /// <summary>
        /// Start a new json object scope
        /// </summary>
        protected override void StartObjectScope()
        {
            writer.StartObjectScope();
        }

        /// <summary>
        /// Start a new json array scope
        /// </summary>
        protected override void StartArrayScope()
        {
            writer.StartArrayScope();
        }

        /// <summary>
        /// Add a property name to the current json object
        /// </summary>
        /// <param name="name">The name to add</param>
        protected override void AddPropertyName(String name)
        {
            writer.AddPropertyName(name);
        }

        /// <summary>
        /// Add a value to the current json scope
        /// </summary>
        /// <param name="value">The value to add</param>
        protected override void AddValue(String value)
        {
            writer.AddValue(value);
        }

        /// <summary>
        /// Add a value to the current json scope
        /// </summary>
        /// <param name="value">The value to add</param>
        protected override void AddValue(double value)
        {
            writer.AddValue(value);
        }

        /// <summary>
        /// End the current json array scope
        /// </summary>
        protected override void EndArrayScope()
        {
            writer.EndArrayScope();
        }

        /// <summary>
        /// End the current json object scope
        /// </summary>
        protected override void EndObjectScope()
        {
            writer.EndObjectScope();
        }

        #endregion
    }
}
