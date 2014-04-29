//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Atom metadata description for a collection (in a workspace).
    /// </summary>
    public sealed class AtomResourceCollectionMetadata
    {
        /// <summary>Gets or sets the title of the collection.</summary>
        /// <returns>The title of the collection.</returns>
        public AtomTextConstruct Title
        {
            get;
            set;
        }

        /// <summary>Gets or sets the accept range of media types for this collection.</summary>
        /// <returns>The accept range of media types for this collection.</returns>
        public string Accept
        {
            get;
            set;
        }

        /// <summary>Gets or sets the categories for this collection.</summary>
        /// <returns>The categories for this collection.</returns>
        public AtomCategoriesMetadata Categories
        {
            get;
            set;
        }
    }
}
