//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for the kind of resource types
    /// </summary>
    public enum ResourceTypeKind
    {
        /// <summary>Resource type with keys.</summary>
        EntityType,

        /// <summary>Resource type without keys.</summary>
        ComplexType,

        /// <summary>A resource type without keys and with no properties.</summary>
        Primitive,

        /// <summary>Resource type representing a collection property of primitive or complex types.</summary>
        Collection,

        /// <summary>Resource type representing a collection of entities.</summary>
        EntityCollection,
    }
}
