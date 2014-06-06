//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// An internal enumeration to track the differnt states of the metadata caching and materialization.
    /// We use a tri-state enumeration instead of a boolean flag since the URI parser will
    /// look up entity sets and service operations which don't require the full metadata
    /// but will require the entity containers.
    /// </summary>
    internal enum MetadataProviderState
    {
        /// <summary>Incremental materialization state.</summary>
        Incremental,

        /// <summary>Full materialization state.</summary>
        Full,
    }
}
