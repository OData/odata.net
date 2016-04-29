//---------------------------------------------------------------------
// <copyright file="IEdmEnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM enumeration type member.
    /// </summary>
    public interface IEdmEnumMember : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the value of this enumeration type member.
        /// </summary>
        IEdmEnumMemberValue Value { get; }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        IEdmEnumType DeclaringType { get; }
    }
}
