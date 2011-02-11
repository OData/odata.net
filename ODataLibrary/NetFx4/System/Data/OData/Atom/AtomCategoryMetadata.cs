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

namespace System.Data.OData.Atom
{
    /// <summary>
    /// Atom metadata description for a category.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class AtomCategoryMetadata : ODataAnnotatable
#else
    public sealed class AtomCategoryMetadata : ODataAnnotatable
#endif
    {
        /// <summary>
        /// The string value identifying the category.
        /// </summary>
        public string Term
        {
            get;
            set;
        }

        /// <summary>
        /// The IRI indicating the scheme of the category.
        /// </summary>
        public string Scheme
        {
            get;
            set;
        }

        /// <summary>
        /// A human-readable label for display in user interfaces.
        /// </summary>
        public string Label
        {
            get;
            set;
        }
    }
}
