//---------------------------------------------------------------------
// <copyright file="IEdmOnDelete.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Enumerates the actions EDM can apply on deletes.
    /// </summary>
    public enum EdmOnDeleteAction
    {
        /// <summary>
        /// Take no action on delete.
        /// </summary>
        None = 0,

        /// <summary>
        /// On delete also delete items on the other end of the association.
        /// </summary>
        Cascade,

        /// <summary>
        /// Meaning all properties of related entities that are tied to properties of the source entity via a referential constraint
        /// and that do not participate in other referential constraints will be set to null.
        /// </summary>
        SetNull,

        /// <summary>
        /// Meaning all properties of related entities that are tied to properties of the source entity via a referential constraint
        /// and that do not participate in other referential constraints will be set to their default value.
        /// </summary>
        SetDefault
    }

    public interface IEdmOnDelete : IEdmElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        EdmOnDeleteAction Action { get; }
    }
}
