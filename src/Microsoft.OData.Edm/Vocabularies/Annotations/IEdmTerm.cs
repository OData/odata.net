//---------------------------------------------------------------------
// <copyright file="IEdmTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM term.
    /// </summary>
    public interface IEdmTerm : IEdmSchemaElement
    {
        /// <summary>
        /// Gets the type of this term.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the AppliesTo of this term.
        /// </summary>
        string AppliesTo { get; }

        /// <summary>
        /// Gets the DefaultValue of this term.
        /// </summary>
        string DefaultValue { get; }
    }
}
