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
