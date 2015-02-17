//---------------------------------------------------------------------
// <copyright file="AtomCategoryMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Atom metadata description for a category.
    /// </summary>
    public sealed class AtomCategoryMetadata : ODataAnnotatable
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.Atom.AtomCategoryMetadata" /> class.</summary>
        public AtomCategoryMetadata()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="AtomCategoryMetadata"/> instance to copy the values from; can be null.</param>
        internal AtomCategoryMetadata(AtomCategoryMetadata other)
        {
            if (other == null)
            {
                return;
            }

            this.Term = other.Term;
            this.Scheme = other.Scheme;
            this.Label = other.Label;
        }
    
        /// <summary>Gets or sets the string value identifying the category.</summary>
        /// <returns>The string value identifying the category.</returns>
        public string Term
        {
            get;
            set;
        }

        /// <summary>Gets or sets the URI that indicates the scheme of the category.</summary>
        /// <returns>The URI that indicates the scheme of the category.</returns>
        public string Scheme
        {
            get;
            set;
        }

        /// <summary>Gets or sets a human-readable label for display in user interfaces.</summary>
        /// <returns>A human-readable label.</returns>
        public string Label
        {
            get;
            set;
        }
    }
}
