//---------------------------------------------------------------------
// <copyright file="EdmCoreModelEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The built-in Edm.EntityType abstract type in the core model.
    /// </summary>
    internal sealed class EdmCoreModelEntityType : EdmType, IEdmEntityType, IEdmCoreModelElement, IEdmFullNamedElement
    {
        /// <summary>
        /// The core Edm.EntityType singleton.
        /// </summary>
        public static readonly EdmCoreModelEntityType Instance = new EdmCoreModelEntityType();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private EdmCoreModelEntityType()
        {
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        /// <summary>
        /// Gets the schema element kind of this type.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the name of this type.
        /// </summary>
        public string Name
        {
            get { return CsdlConstants.TypeName_Entity_Short; }
        }

        /// <summary>
        /// Gets the namespace of this type.
        /// </summary>
        public string Namespace
        {
            get { return EdmConstants.EdmNamespace; }
        }

        /// <summary>
        /// Gets the full name of this type.
        /// </summary>
        public string FullName
        {
            get { return CsdlConstants.TypeName_Entity; }
        }

        /// <summary>
        /// Gets the value indicating whether or not this type is a media entity.
        /// The Edm.EntityType has not stream.
        /// </summary>
        public bool HasStream
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this type is abstract.
        /// The Edm.EntityType is always an abstract type.
        /// </summary>
        public bool IsAbstract
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this type is open.
        /// The Edm.EntityType is always non-open type.
        /// </summary>
        public bool IsOpen
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the base type of this type.
        /// The Edm.EntityType is always without base type.
        /// </summary>
        public IEdmStructuredType BaseType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// The Edm.EntityType is always without any declared properties.
        /// </summary>
        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return Enumerable.Empty<IEdmProperty>(); }
        }

        /// <summary>
        /// Gets the structural properties of the entity type that make up the entity key.
        /// The Edm.EntityType is always without any declared keys.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get { return Enumerable.Empty<IEdmStructuralProperty>(); }
        }

        /// <summary>
        /// Searches for a structural or navigation property with the given name in this type.
        /// </summary>
        /// <param name="name">The name of the property being found.</param>
        /// <returns>The Edm.EntityType is always without any declared properties.</returns>
        public IEdmProperty FindProperty(string name)
        {
            return null;
        }
    }
}
