//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM collection type.
    /// </summary>
    public class EdmCollectionType : EdmType, IEdmCollectionType
    {
        private readonly IEdmTypeReference elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionType"/> class.
        /// </summary>
        /// <param name="elementType">The type of the elements in this collection.</param>
        public EdmCollectionType(IEdmTypeReference elementType)
        {
            EdmUtil.CheckArgumentNull(elementType, "elementType");
            this.elementType = elementType;
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Collection; }
        }

        /// <summary>
        /// Gets the element type of this collection type.
        /// </summary>
        public IEdmTypeReference ElementType
        {
            get { return this.elementType; }
        }
    }
}
