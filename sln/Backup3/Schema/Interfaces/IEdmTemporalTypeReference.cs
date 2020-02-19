//---------------------------------------------------------------------
// <copyright file="IEdmTemporalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM temporal (Duration, DateTime, DateTimeOffset) type.
    /// </summary>
    public interface IEdmTemporalTypeReference : IEdmPrimitiveTypeReference
    {
        /// <summary>
        /// Gets the precision of this temporal type.
        /// </summary>
        int? Precision { get; }
    }
}
