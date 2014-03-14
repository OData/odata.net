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
    /// <summary>
    /// Atom metadata description for a category.
    /// </summary>
    public sealed class AtomCategoryMetadata : ODataAnnotatable
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.Atom.AtomCategoryMetadata" /> class.</summary>
        public AtomCategoryMetadata()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="AtomCategoryMetadata"/> instance to copy the values from; can be null.</param>
        internal AtomCategoryMetadata(AtomCategoryMetadata other)
        {
            DebugUtils.CheckNoExternalCallers();

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
