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

namespace Microsoft.OData.Core
{
    using System;

    /// <summary>
    /// Represents a deleted link in delta response. 
    /// </summary>
    public sealed class ODataDeltaDeletedLink : ODataDeltaLinkBase
    {
        /// <summary>
        /// Initializes a new <see cref="ODataDeltaLink"/>.
        /// </summary>
        /// <param name="source">The id of the entity from which the relationship is defined, which may be absolute or relative.</param>
        /// <param name="target">The id of the related entity, which may be absolute or relative.</param>
        /// <param name="relationship">The name of the relationship property on the parent object.</param>
        public ODataDeltaDeletedLink(Uri source, Uri target, string relationship)
            : base(source, target, relationship)
        {
        }
    }
}
