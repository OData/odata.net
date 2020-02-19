//---------------------------------------------------------------------
// <copyright file="EdmEntitySetBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an abstract EDM entity set base.
    /// </summary>
    public abstract class EdmEntitySetBase : EdmNavigationSource, IEdmEntitySetBase
    {
        private readonly IEdmCollectionType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySetBase"/> class.
        /// </summary>
        /// <param name="name">Name of the entity set base.</param>
        /// <param name="elementType">The entity type of the elements in this entity set base.</param>
        protected EdmEntitySetBase(string name, IEdmEntityType elementType)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(elementType, "elementType");

            this.type = new EdmCollectionType(new EdmEntityTypeReference(elementType, false));
        }

        /// <summary>
        /// Gets the type of this navigation source.
        /// </summary>
        public override IEdmType Type
        {
            get { return this.type; }
        }
    }
}
