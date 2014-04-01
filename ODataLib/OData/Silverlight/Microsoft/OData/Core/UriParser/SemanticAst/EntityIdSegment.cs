//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System;

    /// <summary>
    /// A segment representing an entity id represented by $id query option
    /// </summary>
    public sealed class EntityIdSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdSegment"/> class.
        /// </summary>
        /// <param name="id">Uri correspoding to $id</param>
        internal EntityIdSegment(Uri id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the original Id Uri for $id.
        /// </summary>
        public Uri Id { get; private set; }
    }
}
