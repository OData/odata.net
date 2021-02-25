//---------------------------------------------------------------------
// <copyright file="IEdmInclude.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The interface of include information for referenced model.
    /// </summary>
    public interface IEdmInclude : IEdmElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Gets the namespace to include.
        /// </summary>
        string Namespace { get; }
    }
}
