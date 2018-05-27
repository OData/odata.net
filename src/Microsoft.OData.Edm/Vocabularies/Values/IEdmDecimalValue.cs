//---------------------------------------------------------------------
// <copyright file="IEdmDecimalValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM decimal value.
    /// </summary>
    public interface IEdmDecimalValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this decimal value.
        /// </summary>
        decimal Value { get; }
    }
}
