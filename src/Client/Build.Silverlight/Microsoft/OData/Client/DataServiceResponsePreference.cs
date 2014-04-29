//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    /// <summary>Determines whether the client requests that the data service return inserted or updated entity data as an entry in the response message.</summary>
    public enum DataServiceResponsePreference
    {
        /// <summary>default option, no Prefer header is sent.</summary>
        None = 0,

        /// <summary>Prefer header with value return=representation is sent with all PUT/PATCH/POST requests to entities.</summary>
        IncludeContent,

        /// <summary>Prefer header with value return=minimal is sent with all PUT/PATCH/POST requests to entities.</summary>
        NoContent,
    }
}
