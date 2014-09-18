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
