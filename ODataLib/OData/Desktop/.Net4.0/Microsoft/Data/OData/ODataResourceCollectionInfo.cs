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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Class representing a resource collection in a workspace of a data service.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Temporary name only.")]
    public sealed class ODataResourceCollectionInfo : ODataAnnotatable
    {
        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) to the collection.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) to the collection.</returns>
        public Uri Url
        {
            get;
            set;
        }

        /// <summary>Gets or sets the name of the collection; this is the entity set name in JSON and the HREF in Atom.</summary>
        /// <returns>The name of the collection.</returns>
        /// <remarks>
        /// This property is required when reading and writing the JSON light format, but has no meaning in the Verbose JSON format.
        /// If present in ATOM, it will be used to populate the title element.
        /// </remarks>
        public string Name
        {
            get;
            set;
        }
    }
}
