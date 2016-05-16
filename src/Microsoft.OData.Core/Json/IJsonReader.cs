//---------------------------------------------------------------------
// <copyright file="IJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Interface for a class that can read arbitrary JSON.
    /// </summary>
    public interface IJsonReader
    {
        /// <summary>
        /// The value of the last reported node.
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
        object Value { get; }

        /// <summary>
        /// The underlying raw Json string.
        /// </summary>
        string RawValue { get; }

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
    }
}
