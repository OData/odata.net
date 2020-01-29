//---------------------------------------------------------------------
// <copyright file="IEdmStringValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM string value.
    /// </summary>
    public interface IEdmStringValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the definition of this string value.
        /// </summary>
        string Value { get; }
    }
}
