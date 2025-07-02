//---------------------------------------------------------------------
// <copyright file="IPrimitiveTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    #region Namespaces
    using System.Threading.Tasks;
    using Microsoft.OData.Json;
    #endregion

    /// <summary>
    /// Interface used for serialization and deserialization of primitive types.
    /// </summary>
    [CLSCompliant(false)]
    public interface IPrimitiveTypeConverter
    {
        /// <summary>
        /// Write the Json representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        void WriteJson(object instance, IJsonWriter jsonWriter);

        /// <summary>
        /// Asynchronously writes the JSON representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteJsonAsync(object instance, IJsonWriter jsonWriter);
    }
}
