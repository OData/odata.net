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
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Represents an association link.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public sealed class ODataAssociationLink : ODataAnnotatable
    {
        /// <summary>
        /// The name of the association ink.
        /// </summary>
        /// <remarks>This is the name of the navigation property to which the association link belongs.</remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// URI representing the Unified Resource Locator (Url) of the link.
        /// </summary>
        /// <remarks>This URL should point to a resource which can be used to retrieve or modify the association itself
        /// not the URL to traverse the navigation property.</remarks>
        public Uri Url
        {
            get;
            set;
        }
    }
}
