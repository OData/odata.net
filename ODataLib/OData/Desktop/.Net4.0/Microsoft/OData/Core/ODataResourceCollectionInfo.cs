//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
