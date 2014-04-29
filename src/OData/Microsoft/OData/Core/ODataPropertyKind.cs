//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// The enum of property kinds.
    /// </summary>
    public enum ODataPropertyKind
    {
        /// <summary>
        /// Unspecified property kind or if the property is not a key property, an etag property or an open property.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The property is a key property.
        /// </summary>
        Key,

        /// <summary>
        /// The property is an etag property
        /// </summary>
        ETag,

        /// <summary>
        /// The property is an open property
        /// </summary>
        Open
    }
}
