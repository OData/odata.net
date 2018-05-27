//---------------------------------------------------------------------
// <copyright file="IEdmEnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM enumeration type.
    /// </summary>
    public interface IEdmEnumType : IEdmSchemaType
    {
        /// <summary>
        /// Gets the underlying type of this enumeration type.
        /// </summary>
        IEdmPrimitiveType UnderlyingType { get; }

        /// <summary>
        /// Gets the members of this enumeration type.
        /// </summary>
        IEnumerable<IEdmEnumMember> Members { get; }

        /// <summary>
        /// Gets a value indicating whether the enumeration type can be treated as a bit field.
        /// </summary>
        bool IsFlags { get; }
    }
}
