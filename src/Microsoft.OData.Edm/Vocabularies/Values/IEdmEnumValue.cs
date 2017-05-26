//---------------------------------------------------------------------
// <copyright file="IEdmEnumValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM enumeration type value.
    /// </summary>
    public interface IEdmEnumValue : IEdmPrimitiveValue
    {
        /// <summary>
        /// Gets the underlying type value of the enumeration type.
        /// </summary>
        IEdmEnumMemberValue Value { get; }
    }
}
