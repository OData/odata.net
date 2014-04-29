//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM metatypes.
    /// </summary>
    public enum EdmTypeKind
    {
        /// <summary>
        /// Represents a type with an unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmPrimitiveType"/>. 
        /// </summary>
        Primitive,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEntityType"/>. 
        /// </summary>
        Entity,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmComplexType"/>. 
        /// </summary>
        Complex,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmCollectionType"/>. 
        /// </summary>
        Collection,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEntityReferenceType"/>.
        /// </summary>
        EntityReference,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEnumType"/>.
        /// </summary>
        Enum
    }

    /// <summary>
    /// Represents the definition of an EDM type.
    /// </summary>
    public interface IEdmType : IEdmElement
    {
        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        EdmTypeKind TypeKind { get; }
    }
}
