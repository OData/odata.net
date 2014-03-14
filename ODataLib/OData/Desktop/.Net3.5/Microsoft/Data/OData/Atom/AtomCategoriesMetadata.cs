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
