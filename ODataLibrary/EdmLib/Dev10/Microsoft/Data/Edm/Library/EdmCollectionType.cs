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
        private readonly bool isAtomic;

        /// <summary>
        /// Initializes a new instance of the EdmCollectionType class.
        /// </summary>
        /// <param name="elementType">The type of the elements in this collection.</param>
        /// <param name="isAtomic">A flag representing if the type should be atomic.</param>
        public EdmCollectionType(IEdmTypeReference elementType, bool isAtomic)
            : base(EdmTypeKind.Collection)
        {
            this.elementType = elementType;
            this.isAtomic = isAtomic;
        }

        /// <summary>
        /// Gets the element type of this collection type.
        /// </summary>
        public IEdmTypeReference ElementType
        {
            get { return this.elementType; }
        }

        /// <summary>
        /// Gets a value indicating whether a collection should be treated as an atomic unit. If true, the type will be serialized to a CSDL as MultiValue, if false it will be serialized as Collection
        /// </summary>
        public bool IsAtomic
        {
            get { return this.isAtomic; }
        }
    }
}
