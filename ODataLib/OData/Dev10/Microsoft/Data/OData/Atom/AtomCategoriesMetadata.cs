//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Atom metadata description for a categories element (app:categories).
    /// </summary>
    public sealed class AtomCategoriesMetadata
    {
        /// <summary>
        /// Specified if the list of categories is fixed or an open set.
        /// </summary>
        public bool? Fixed
        {
            get;
            set;
        }

        /// <summary>
        /// The IRI indicating the scheme of the categories without a scheme.
        /// </summary>
        public string Scheme
        {
            get;
            set;
        }

        /// <summary>
        /// The IRI of the category document.
        /// </summary>
        /// <remarks>
        /// If this property is not null, the properties <see cref="Fixed"/> and <see cref="Scheme"/> must be both null
        /// and the <see cref="Categories"/> must be either null or empty collection.
        /// </remarks>
        public Uri Href
        {
            get;
            set;
        }

        /// <summary>
        /// Atom category elements inside this categories element.
        /// </summary>
        public IEnumerable<AtomCategoryMetadata> Categories
        {
            get;
            set;
        }
    }
}
