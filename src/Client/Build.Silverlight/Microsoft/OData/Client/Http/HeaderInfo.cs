//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;

    /// <summary>Use this class to record read-only information about headers.</summary>
    internal class HeaderInfo
    {
        /// <summary>Header name.</summary>
        internal readonly string HeaderName;

        /// <summary>Whether direct access to header is allowed on header collections.</summary>
        internal readonly bool IsRequestRestricted;

        /// <summary>
        /// Initializes a new <see cref="HeaderInfo"/> instance with AllowMultiValues set to false.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="requestRestricted">Whether direct access to header is allowed on header collections.</param>
        internal HeaderInfo(string name, bool requestRestricted)
        {
            this.HeaderName = name;
            this.IsRequestRestricted = requestRestricted;
        }
    }
}
