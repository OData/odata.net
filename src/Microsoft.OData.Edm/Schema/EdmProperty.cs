//---------------------------------------------------------------------
// <copyright file="EdmProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public abstract class EdmProperty : EdmNamedElement, IEdmProperty
    {
        private readonly IEdmStructuredType declaringType;
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        protected EdmProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringType, "declaringType");
            EdmUtil.CheckArgumentNull(type, "type");

            this.declaringType = declaringType;
            this.type = type;
        }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public abstract EdmPropertyKind PropertyKind
        {
            get;
        }
    }
}
