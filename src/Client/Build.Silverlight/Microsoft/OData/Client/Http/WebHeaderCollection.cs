//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Contains protocol headers associated with a request or response.</summary>
    internal abstract class WebHeaderCollection
    {
        #region Properties.

        /// <summary>Gets the number of headers in the collection.</summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>Collection of header names.</summary>
        public abstract ICollection<string> AllKeys
        {
            get;
        }

        /// <summary>Gets or sets a named header.</summary>
        /// <param name="name">Header name.</param>
        /// <returns>The header value.</returns>
        public abstract string this[string name]
        {
            get;
            set;
        }

        /// <summary>Gets or sets a known request header.</summary>
        /// <param name="header">Header to get or set.</param>
        /// <returns>The header value.</returns>
        /// <remarks>Request headers are always allowed, the checks should be removed.</remarks>
        public abstract string this[Microsoft.OData.Service.Http.HttpRequestHeader header]
        {
            get;
            set;
        }
        #endregion Properties.

        /// <summary>
        /// Sets a specified header
        /// </summary>
        /// <param name="header">The request header to set</param>
        /// <param name="value">The value for the header</param>
        public virtual void Set(HttpRequestHeader header, string value)
        {
            this[header] = value;
        }
    }
}
