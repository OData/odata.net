//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
