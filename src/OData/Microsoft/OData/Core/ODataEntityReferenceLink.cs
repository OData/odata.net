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
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Represents an entity reference link (the result of a $link query).
    /// </summary>
    [DebuggerDisplay("{Url.OriginalString}")]
    public sealed class ODataEntityReferenceLink : ODataItem
    {
        /// <summary>Gets or sets the URI representing the URL of the referenced entity.</summary>
        /// <returns>The URI representing the URL of the referenced entity.</returns>
        /// <remarks>This URL should be usable to retrieve or modify the referenced entity.</remarks>
        public Uri Url
        {
            get;
            set;
        }
    }
}
