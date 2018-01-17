//---------------------------------------------------------------------
// <copyright file="IEdmDecimalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM decimal type.
    /// </summary>
    public interface IEdmDecimalTypeReference : IEdmPrimitiveTypeReference
    {
        /// <summary>
        /// Gets the precision of this type.
        /// </summary>
        int? Precision { get; }

        /// <summary>
        /// Gets the scale of this type.
        /// </summary>
        int? Scale { get; }
    }
}
