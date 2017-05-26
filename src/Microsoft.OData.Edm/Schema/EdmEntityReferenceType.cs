//---------------------------------------------------------------------
// <copyright file="EdmEntityReferenceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
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
