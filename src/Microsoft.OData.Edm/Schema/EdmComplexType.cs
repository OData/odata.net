//---------------------------------------------------------------------
// <copyright file="EdmComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM complex type.
    /// </summary>
    public class EdmComplexType : EdmStructuredType, IEdmComplexType
    {
        private readonly string namespaceName;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        public EdmComplexType(string namespaceName, string name)
            : this(namespaceName, name, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType)
            : this(namespaceName, name, baseType, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        /// <param name="isAbstract">Denotes whether this complex type is abstract.</param>
        public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType, bool isAbstract)
            : this(namespaceName, name, baseType, isAbstract, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        /// <param name="baseType">The base type of this complex type.</param>
        /// <param name="isAbstract">Denotes whether this complex type is abstract.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType, bool isAbstract, bool isOpen)
            : base(isAbstract, isOpen, baseType)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.namespaceName = namespaceName;
            this.name = name;
        }

        /// <summary>
        /// Gets the schema element kind of this element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the namespace of this element.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }
    }
}
