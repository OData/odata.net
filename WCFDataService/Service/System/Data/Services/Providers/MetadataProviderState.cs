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

namespace System.Data.Services.Providers
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
