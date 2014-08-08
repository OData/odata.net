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
    using Microsoft.Data.Edm;

    /// <summary>
    /// Extends <see cref="IEdmEntitySet"/> to expose the <see cref="ResourceSetWrapper"/> that the entity set was based on.
    /// </summary>
    internal interface IResourceSetBasedEdmEntitySet : IEdmEntitySet
    {
        /// <summary>
        /// The resource-set wrapper that this entity-set was created from.
        /// </summary>
        ResourceSetWrapper ResourceSet { get; }
    }
}
