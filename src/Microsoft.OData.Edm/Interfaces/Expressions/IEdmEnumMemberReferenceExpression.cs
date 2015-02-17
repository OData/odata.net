//---------------------------------------------------------------------
// <copyright file="IEdmEnumMemberReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Expressions
{
    /// <summary>
    /// Represents an EDM enumeration member reference expression.
    /// </summary>
    public interface IEdmEnumMemberReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the referenced enum member.
        /// </summary>
        IEdmEnumMember ReferencedEnumMember { get; }
    }
}
