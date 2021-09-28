//---------------------------------------------------------------------
// <copyright file="IJsonReaderAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Interface for a class that can read arbitrary JSON asynchronously.
    /// </summary>
    public interface IJsonReaderAsync
    {
        /// <summary>
        /// Asynchronously reads the next node from the input.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        bool ReadAsync();
    }
}
