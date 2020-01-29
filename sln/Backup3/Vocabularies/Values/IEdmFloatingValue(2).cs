//---------------------------------------------------------------------
// <copyright file="IEdmFloatingValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM floating point value.
    /// </summary>
    public interface IEdmFloatingValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this floating value.
        /// </summary>
        double Value { get; }
    }
}
