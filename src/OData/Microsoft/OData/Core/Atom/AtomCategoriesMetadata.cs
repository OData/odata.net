//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
