//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;

    /// <summary>Use this class to map header HTTP header names and values.</summary>
    internal static class HttpHeaderToName
    {
        /// <summary>Header names, in the same order as HttpResponseHeader enumerationvalues.</summary>
        private static readonly string[] HeaderStrings = new string[]
        {
            "Cache-Control", 
            "Connection",
            "Date",
            "Keep-Alive", 
            "Pragma", 
            "Trailer",
            "Transfer-Encoding",
            "Upgrade", 
            "Via", 
            "Warning", 
            "Allow", 
            "Content-Length",
            "Content-Type",
            "Content-Encoding", 
            "Content-Language",
            "Content-Location", 
            "Content-MD5",
            "Content-Range",
            "Expires", 
            "Last-Modified",
            "Accept", 
            "Accept-Charset", 
            "Accept-Encoding", 
            "Accept-Language",
            "Authorization", 
            "Cookie", 
            "Expect", 
            "From", 
            "Host", 
            "If-Match", 
            "If-Modified-Since", 
            "If-None-Match", 
            "If-Range",
            "If-Unmodified-Since",
            "Max-Forwards",
            "Proxy-Authorization",
            "Referer",
            "Range",
            "TE",
            "translate", 
            "UserAgent",
            ////"IM", 
            "Wlc-Safe-Agent", 
            "Slug", 
            ////"Content-Disposition", 
            ////"inline", 
            ////"attachment",
            ////"filename",
            ////"with-tombstones", 
            ////"feed", 
            ////"feed-with-tombstones", 
            ////"PagedRead", 
            ////"StatelessClient"
         };

        /// <summary>Given the specified <paramref name="header"/>, returns the name.</summary>
        /// <param name="header">Enumeration value to get name for.</param>
        /// <returns>The name for the specified header value.</returns>
        public static string GetRequestHeaderName(Microsoft.OData.Service.Http.HttpRequestHeader header)
        {
            return HeaderStrings[(int)header];
        }
    }
}

