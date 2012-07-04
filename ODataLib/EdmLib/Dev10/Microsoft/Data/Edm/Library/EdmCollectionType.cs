//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.Edm.Library
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
