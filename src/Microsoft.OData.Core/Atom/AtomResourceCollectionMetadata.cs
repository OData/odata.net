//---------------------------------------------------------------------
// <copyright file="AtomResourceCollectionMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Atom metadata description for a collection (in a workspace).
    /// </summary>
    public sealed class AtomResourceCollectionMetadata
    {
        /// <summary>Gets or sets the title of the collection.</summary>
        /// <returns>The title of the collection.</returns>
        public AtomTextConstruct Title
        {
            get;
            set;
        }

        /// <summary>Gets or sets the accept range of media types for this collection.</summary>
        /// <returns>The accept range of media types for this collection.</returns>
        public string Accept
        {
            get;
            set;
        }

        /// <summary>Gets or sets the categories for this collection.</summary>
        /// <returns>The categories for this collection.</returns>
        public AtomCategoriesMetadata Categories
        {
            get;
            set;
        }
    }
}
