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
    /// <summary>
    /// Enumeration representing the Http methods supported in batches. This is a subset of 
    /// all Http methods specified in HTTP RFC 2616 Section 5.1.1.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>Http 'Get' method.</summary>
        Get = 0,

        /// <summary>Http 'Post' method.</summary>
        Post = 1,

        /// <summary>Http 'Put' method.</summary>
        Put = 2,

        /// <summary>Http 'Delete' method.</summary>
        Delete = 3,

        /// <summary>Http 'Patch' method.</summary>
        Patch = 4,

        /// <summary>Custom Http 'Merge' method.</summary>
        Merge = 5,
    }
}
