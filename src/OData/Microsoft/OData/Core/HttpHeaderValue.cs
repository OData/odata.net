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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for http header values.
    /// </summary>
    internal sealed class HttpHeaderValue : Dictionary<string, HttpHeaderValueElement>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="HttpHeaderValue"/>.
        /// </summary>
        internal HttpHeaderValue()
            : base(StringComparer.OrdinalIgnoreCase /*Should be case-insensitive for tokens.*/)
        {
        }

        /// <summary>
        /// Returns the HTTP header value string which can be used to set the header on the requst and response messages.
        /// </summary>
        /// <returns>Returns the HTTP header value string which can be used to set the header on the requst and response messages.</returns>
        public override string ToString()
        {
            return this.Count == 0 ? null : String.Join(HttpHeaderValueLexer.ElementSeparator, this.Values.Select(element => element.ToString()).ToArray());
        }
    }
}
