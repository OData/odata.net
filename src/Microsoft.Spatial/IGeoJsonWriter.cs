//---------------------------------------------------------------------
// <copyright file="IGeoJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// Represent an actual Json writing stream in Spatial.
    /// </summary>
    public interface IGeoJsonWriter
    {
        /// <summary>
        /// Start the object scope.
        /// </summary>
        void StartObjectScope();

        /// <summary>
        /// End the current object scope.
        /// </summary>
        void EndObjectScope();

        /// <summary>
        /// Start the array scope.
        /// </summary>
        void StartArrayScope();

        /// <summary>
        /// End the current array scope.
        /// </summary>
        void EndArrayScope();

        /// <summary>
        /// Add a property name to the current json object.
        /// </summary>
        /// <param name="name">The name to add.</param>
        void AddPropertyName(string name);

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void AddValue(double value);

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void AddValue(string value);
    }
}
