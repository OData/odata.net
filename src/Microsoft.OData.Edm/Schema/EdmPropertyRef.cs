//---------------------------------------------------------------------
// <copyright file="EdmPropertyRef.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM property reference element.
    /// </summary>
    public class EdmPropertyRef : IEdmPropertyRef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyRef"/> class.
        /// </summary>
        /// <param name="property">The referenced property.</param>
        public EdmPropertyRef(IEdmStructuralProperty property)
            : this(property, property?.Name, alias: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyRef"/> class.
        /// </summary>
        /// <param name="property">The referenced property.</param>
        /// <param name="alias">The property reference alias.</param>
        public EdmPropertyRef(IEdmStructuralProperty property, string alias)
            : this(property, property?.Name, alias)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyRef"/> class.
        /// </summary>
        /// <param name="property">The referenced property.</param>
        /// <param name="name">The property reference name.</param>
        /// <param name="alias">The property reference alias.</param>
        public EdmPropertyRef(IEdmStructuralProperty property, string name, string alias)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(name, "name");

            ReferencedProperty = property;
            Name = name;
            PropertyAlias = alias;
        }

        /// <summary>
        /// Gets the referenced property.
        /// </summary>
        public IEdmStructuralProperty ReferencedProperty { get; }

        /// <summary>
        /// Gets the value of Name is a path expression leading to a primitive property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string PropertyAlias { get; }
    }
}
