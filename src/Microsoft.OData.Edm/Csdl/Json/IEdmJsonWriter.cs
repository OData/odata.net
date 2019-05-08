//---------------------------------------------------------------------
// <copyright file="IEdmJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// JSON interface for writing Edm model.
    /// </summary>
    public interface IEdmJsonWriter
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
        /// Write a property name to the current json object.
        /// </summary>
        /// <param name="name">The name to write.</param>
        void WritePropertyName(string name);

        /// <summary>
        /// Write null value.
        /// </summary>
        void WriteNull();

        /// <summary>
        /// Write a long value to the current json scope.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(long value);

        /// <summary>
        /// Write a double value to the current json scope.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(double value);

        /// <summary>
        /// Write a decimal value to the current json scope.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(decimal value);

        /// <summary>
        /// Write a string value to the current json scope.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(string value);

        /// <summary>
        /// Write an integer value to the current json scope.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(int value);

        /// <summary>
        /// Write a boolean value to the current json scope.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(bool value);

        /// <summary>
        /// Flush the writer.
        /// </summary>
        void Flush();
    }
}