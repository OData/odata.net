//---------------------------------------------------------------------
// <copyright file="IJsonStreamReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData.Json
{
    using System.IO;

    /// <summary>
    /// Interface for a class that can read arbitrary JSON and stream binary properties.
    /// </summary>
    public interface IJsonStreamReader : IJsonReader
    {
        /// <summary>
        /// Creates a Stream for reading a binary value.
        /// </summary>
        /// <returns>A Stream for reading the binary value.</returns>
        Stream CreateReadStream();

        /// <summary>
        /// Creates a TextReader for reading a string value.
        /// </summary>
        /// <returns>A TextReader for reading the string value.</returns>
        TextReader CreateTextReader();

        /// <summary>
        /// Whether or not the current value can be streamed
        /// </summary>
        /// <returns>True if the current value can be streamed, otherwise false</returns>
        bool CanStream();
    }
}