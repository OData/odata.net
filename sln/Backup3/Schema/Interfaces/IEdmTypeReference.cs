//---------------------------------------------------------------------
// <copyright file="IEdmTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a references to a type.
    /// </summary>
    public interface IEdmTypeReference : IEdmElement
    {
        /// <summary>
        /// Gets a value indicating whether this type is nullable.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Gets the definition to which this type refers.
        /// </summary>
        IEdmType Definition { get; }
    }
}
