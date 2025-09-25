//---------------------------------------------------------------------
// <copyright file="EdmPropertyRef.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    // <summary>
    /// Represents a definition of an EDM property reference element.
    /// </summary>
    public class EdmPropertyRef : IEdmPropertyRef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyRef"/> class.
        /// </summary>
        /// <param name="property">The referenced property without an alias provided.</param>
        public EdmPropertyRef(IEdmStructuralProperty property)
            : this(property, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyRef"/> class.
        /// </summary>
        /// <param name="property">The referenced property.</param>
        /// <param name="alias">The given alias. If it's null, it means to use the property name as the alias.</param>
        public EdmPropertyRef(IEdmStructuralProperty property, string alias)
        {
            EdmUtil.CheckArgumentNull(property, "property");

            ReferencedProperty = property;
            Path = new EdmPropertyPathExpression(property.Name);
            PropertyAlias = alias;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyRef"/> class.
        /// </summary>
        /// <param name="property">The referenced property.</param>
        /// <param name="path">The path to referenced property, the last segment in the path should be the referenced property name.</param>
        /// <param name="alias">The given alias, it should not be null.</param>
        public EdmPropertyRef(IEdmStructuralProperty property, IEdmPathExpression path, string alias)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(path, "path");
            EdmUtil.CheckArgumentNull(alias, "alias");

            if (path.PathSegments == null || path.PathSegments.LastOrDefault() != property.Name)
            {
                throw new ArgumentException(Error.Format(SRResources.PropertyPathMustEndingWithCorrectPropertyName, path.Path, property.Name));
            }

            ReferencedProperty = property;
            Path = path;
            PropertyAlias = alias;
        }

        /// <summary>
        /// Gets the referenced property.
        /// </summary>
        public IEdmStructuralProperty ReferencedProperty { get; }

        /// <summary>
        /// Gets the path to the referenced property.
        /// </summary>
        public IEdmPathExpression Path { get; }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string PropertyAlias { get; }
    }
}
