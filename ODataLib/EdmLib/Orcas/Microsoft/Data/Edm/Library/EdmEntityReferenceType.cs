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
    /// Represents a definition of an EDM entity reference type.
    /// </summary>
    public class EdmEntityReferenceType : EdmType, IEdmEntityReferenceType
    {
        private readonly IEdmEntityType entityType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityReferenceType"/> class.
        /// </summary>
        /// <param name="entityType">The entity referred to by this entity reference.</param>
        public EdmEntityReferenceType(IEdmEntityType entityType)
        {
            EdmUtil.CheckArgumentNull(entityType, "entityType");
            this.entityType = entityType;
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.EntityReference; }
        }

        /// <summary>
        /// Gets the entity type pointed to by this entity reference.
        /// </summary>
        public IEdmEntityType EntityType
        {
            get { return this.entityType; }
        }
    }
}
