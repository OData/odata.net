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
    /// Atom metadata description for a collection (in a workspace).
    /// </summary>
    public sealed class AtomResourceCollectionMetadata
    {
        /// <summary>
        /// The title of the collection.
        /// </summary>
        public AtomTextConstruct Title
        {
            get;
            set;
        }

        /// <summary>
        /// The accept range of media types for this collection.
        /// </summary>
        public string Accept
        {
            get;
            set;
        }

        /// <summary>
        /// The categories for this collection.
        /// </summary>
        public AtomCategoriesMetadata Categories
        {
            get;
            set;
        }
    }
}
