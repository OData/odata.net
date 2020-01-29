//---------------------------------------------------------------------
// <copyright file="IEdmEnumMemberExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM enumeration member reference expression.
    /// </summary>
    public interface IEdmEnumMemberExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the referenced enum member.
        /// </summary>
        IEnumerable<IEdmEnumMember> EnumMembers { get; }
    }
}