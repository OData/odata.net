//---------------------------------------------------------------------
// <copyright file="IJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Interface for a class that can write arbitrary JSON.
    /// Internally we want the interface for mocks.
    /// </summary>
    internal interface IJsonWriterFactory
    {
        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="textWriter">Writer to which text needs to be written.</param>
        /// <param name="indent">If the output should be indented or not.</param>
        /// <param name="jsonFormat">The json-based format to use when writing.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <returns>The JSON writer created.</returns>
        IJsonWriter CreateJsonWriter(TextWriter textWriter, bool indent, ODataFormat jsonFormat, bool isIeee754Compatible);
    }
}