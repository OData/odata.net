//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Client
{
    using Microsoft.Data.OData;

    /// <summary>
    /// Writing entry arguments
    /// </summary>
    public sealed class WritingEntryArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WritingEntryArgs"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        public WritingEntryArgs(ODataEntry entry, object entity)
        {
            Util.CheckArgumentNull(entry, "entry");
            Util.CheckArgumentNull(entity, "entity");
            this.Entry = entry;
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <value>
        /// The entry.
        /// </value>
        public ODataEntry Entry { get; private set; }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public object Entity { get; private set; }
    }
}
