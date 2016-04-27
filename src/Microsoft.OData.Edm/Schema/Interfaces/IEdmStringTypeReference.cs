//---------------------------------------------------------------------
// <copyright file="IEdmStringTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM string type.
    /// </summary>
    public interface IEdmStringTypeReference : IEdmPrimitiveTypeReference
    {
        /// <summary>
        /// Gets a value indicating whether this string type specifies the maximum allowed length.
        /// </summary>
        bool IsUnbounded { get; }

        /// <summary>
        /// Gets the maximum length of this string type.
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets a value indicating whether this string type supports unicode encoding.
        /// </summary>
        bool? IsUnicode { get; }
    }
}
