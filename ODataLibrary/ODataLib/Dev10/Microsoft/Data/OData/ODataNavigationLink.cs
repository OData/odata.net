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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Represents a single link.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class ODataNavigationLink : ODataItem
    {
        /// <summary>
        /// Flag indicating whether the navigation link represents a collection or and entry.
        /// </summary>
        /// <remarks>This property is required to have a value for ATOM payloads and is optional for JSON payloads.</remarks>
        public bool? IsCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// URI representing the Unified Resource Locator (Url) of the link.
        /// </summary>
        public Uri Url 
        { 
            get; 
            set; 
        }
    }
}
