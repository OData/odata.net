//---------------------------------------------------------------------
// <copyright file="IJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a class that can read arbitrary JSON.
    /// </summary>
    public interface IJsonReader
    {
        /// <summary>
        /// Returns the value of the last reported node.
        /// </summary>
        /// <remarks>This is non-null only if the last node was a PrimitiveValue or Property.
        /// If the last node is a PrimitiveValue this property returns the value:
        /// - null if the null token was found.
        /// - boolean if the true or false token was found.
        /// - string if a string token was found.
        /// - DateTime if a string token formatted as DateTime was found.
        /// - Int32 if a number which fits into the Int32 was found.
        /// - Double if a number which doesn't fit into Int32 was found.
        /// If the last node is a Property this property returns a string which is the name of the property.
        /// </remarks>
        object GetValue();

        /// <summary>
        /// The type of the last node read.
        /// </summary>
        JsonNodeType NodeType { get; }

        /// <summary>
        /// if it is IEEE754 compatible
        /// </summary>
        bool IsIeee754Compatible { get; }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        bool Read();

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

        /// <summary>
        /// Asynchronously reads the next node from the input.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains true if a new node was found, or false if end of input was reached.</returns>
        Task<bool> ReadAsync();

        /// <summary>
        /// Asynchronously returns the value of the last reported node.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the value of the last reported node.</returns>
        /// <remarks>A non-null is returned only if the last node was a PrimitiveValue or Property.
        /// If the last node was a PrimitiveValue this property returns the value:
        /// - null if the null token was found.
        /// - boolean if the true or false token was found.
        /// - string if a string token was found.
        /// - DateTime if a string token formatted as DateTime was found.
        /// - Int32 if a number which fits into the Int32 was found.
        /// - Double if a number which doesn't fit into Int32 was found.
        /// If the last node is a Property this property returns a string which is the name of the property.
        /// </remarks>
        Task<object> GetValueAsync();

        /// <summary>
        /// Asynchronously creates a Stream for reading a binary value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the binary value.</returns>
        Task<Stream> CreateReadStreamAsync();

        /// <summary>
        /// Asynchronously creates a TextReader for reading a string value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading the string value.</returns>
        Task<TextReader> CreateTextReaderAsync();

        /// <summary>
        /// Asynchronously checks whether or not the current value can be streamed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// true if the current value can be streamed; otherwise false.</returns>
        Task<bool> CanStreamAsync();
    }
}
