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

namespace System.Data.OData
{
    /// <summary>
    /// Names of HTTP headers used by the OData library.
    /// </summary>
#if INTERNAL_DROP
    internal static class ODataHttpHeaders
#else
    public static class ODataHttpHeaders
#endif
    {
        /// <summary>Name of the HTTP content type header.</summary>
        public const string ContentType = "Content-Type";

        /// <summary>Name of the OData 'DataServiceVersion' HTTP header.</summary>
        public const string DataServiceVersion = "DataServiceVersion";
    }
}
