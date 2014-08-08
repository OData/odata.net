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

#if ODATALIB
namespace Microsoft.Data.OData.Query
#else
namespace System.Data.Services
#endif
{
    /// <summary>
    /// Provides values to describe the source of the request results.
    /// </summary>
    internal enum RequestTargetSource
    {
        /// <summary>No source for data.</summary>
        /// <remarks>
        /// This value is seen when a source hasn't been determined yet, or
        /// when the source is intrinsic to the system - eg a metadata request.
        /// </remarks>
        None,

        /// <summary>An entity set provides the data.</summary>
        EntitySet,

        /// <summary>A service operation provides the data.</summary>
        ServiceOperation,

        /// <summary>A property of an entity or a complex object provides the data.</summary>
        Property,
    }
}
