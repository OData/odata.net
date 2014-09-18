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
    /// Enumeration for the different kinds of reader and writer behaviors
    /// supported in the OData library.
    /// </summary>
    internal enum ODataBehaviorKind
    {
        /// <summary>The default behavior of the OData library.</summary>
        Default,

        /// <summary>The behavior of the OData server.</summary>
        ODataServer,

        /// <summary>The behavior of the WCF Data Services client.</summary>
        WcfDataServicesClient,
    }
}
