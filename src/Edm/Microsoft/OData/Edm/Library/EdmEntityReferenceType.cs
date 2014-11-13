//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Edm.Library
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
