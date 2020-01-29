//---------------------------------------------------------------------
// <copyright file="IEdmBinaryValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM binary value.
    /// </summary>
    public interface IEdmBinaryValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this binary value.
        /// </summary>
        byte[] Value { get; }
    }
}
