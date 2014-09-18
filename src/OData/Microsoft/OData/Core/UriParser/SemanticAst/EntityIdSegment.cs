//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
