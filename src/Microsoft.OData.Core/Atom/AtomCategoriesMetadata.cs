//---------------------------------------------------------------------
// <copyright file="AtomCategoriesMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary> Atom metadata description for a categories element (app:categories). </summary>
    public sealed class AtomCategoriesMetadata
    {
        /// <summary> Gets or sets a value that indicates whether the list of categories is fixed or an open set. </summary>
        /// <returns>true if the list of categories is fixed; false if the list of categories is an open set.</returns>
        public bool? Fixed
        {
            get;
            set;
        }

        /// <summary> Gets or sets the URI indicating the scheme of the categories without a scheme. </summary>
        /// <returns>The URI indicating the scheme of the categories without a scheme.</returns>
        public string Scheme
        {
            get;
            set;
        }

        /// <summary> Gets or sets the URI of the category document. </summary>
        /// <returns>The URI of the category document.</returns>
        /// <remarks>
        /// If this property is not null, the properties <see cref="Fixed"/> and <see cref="Scheme"/> must be both null
        /// and the <see cref="Categories"/> must be either null or empty collection.
        /// </remarks>
        public Uri Href
        {
            get;
            set;
        }

        /// <summary> Gets or sets the atom category elements inside this categories element. </summary>
        /// <returns>The atom category elements inside this categories element.</returns>
        public IEnumerable<AtomCategoryMetadata> Categories
        {
            get;
            set;
        }
    }
}
