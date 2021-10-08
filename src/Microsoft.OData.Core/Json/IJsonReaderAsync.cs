//---------------------------------------------------------------------
// <copyright file="IJsonReaderAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a class that can read arbitrary JSON asynchronously.
    /// </summary>
    public interface IJsonReaderAsync
    {
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
    }
}
