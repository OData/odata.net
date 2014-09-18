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
    /// <summary>
    /// Enumeration representing the different levels of context URL.
    /// </summary>
    internal enum ODataContextUrlLevel
    {
        /// <summary>
        /// No context URL
        /// Used for json with odata.metadata=none
        /// </summary>
        None = 0,

        /// <summary>
        /// Show root context URL of the payload and the context URL for any deleted entries or added or deleted links in a delta response, 
        /// or for entities or entity collections whose set cannot be determined from the root context URL        
        /// Used for atom and json with odata.metadata=minimal
        /// </summary>
        OnDemand = 1,

        /// <summary>
        /// Show context URL for a collection, entity, primitive value, or service document. 
        /// Used for json with odata.metadata=full
        /// </summary>
        Full = 2
    }
}
