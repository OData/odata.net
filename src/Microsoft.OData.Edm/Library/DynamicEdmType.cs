//---------------------------------------------------------------------
// <copyright file="DynamicEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a definition of an dynamically generated entity type.
    /// </summary>
    public sealed class DynamicEdmType : EdmStructuredType, IEdmEntityType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEdmType"/> class.
        /// </summary>
        /// <param name="name">Name of the entity.</param>
        public DynamicEdmType(string name)
            : base(isAbstract: false, isOpen: true, baseStructuredType: null)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the structural properties of the entity type that make up the entity key.
        /// </summary>
        /// <remarks>
        /// Generated types do not have keys
        /// </remarks>
        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get
            {
                return Enumerable.Empty<IEdmStructuralProperty>();
            }
        }

        /// <summary>
        /// Gets the value indicating whether or not this entity is a media type
        /// This value inherits from the base type.
        /// </summary>
        public bool HasStream
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        /// <remarks>
        /// Empty for generated types.
        /// </remarks>
        public string Namespace
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        /// <remarks>
        /// Not supported for generetaed types
        /// </remarks>
        public EdmSchemaElementKind SchemaElementKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the term kind of the entity type.
        /// </summary>
        /// <remarks>
        /// Not supported for generetaed types
        /// </remarks>
        public EdmTermKind TermKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get
            {
                return EdmTypeKind.Entity;
            }
        }
    }

    public sealed class DynamicNestedType : EdmStructuredType, IEdmComplexType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicNestedType"/> class.
        /// </summary>
        /// <param name="name">Name of the entity.</param>
        public DynamicNestedType(string name)
            : base(isAbstract: false, isOpen: true, baseStructuredType: null)
        {
            this.Name = name;
        }

        public string Name
        {
            get; private set;
        }

        public string Namespace
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the schema element kind of this element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        /// <summary>
        /// Gets the kind of this term.
        /// </summary>
        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }
    }
}
