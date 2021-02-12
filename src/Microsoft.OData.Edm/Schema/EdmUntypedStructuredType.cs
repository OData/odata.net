//---------------------------------------------------------------------
// <copyright file="EdmUntypedStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base class for definitions of EDM structured types.
    /// </summary>
    public sealed class EdmUntypedStructuredType : EdmStructuredType, IEdmStructuredType, IEdmSchemaElement, IEdmSchemaType, IEdmFullNamedElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly string fullName;

        /// <summary>
        /// The core Edm.Untyped singleton.
        /// </summary>
        public static readonly EdmUntypedStructuredType Instance = new EdmUntypedStructuredType();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuredType"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace this type belongs to.</param>
        /// <param name="name">The name of this type within its namespace.</param>
        public EdmUntypedStructuredType(string namespaceName, string name)
            : base(/*isAbstract*/true, /*isOpen*/true, /*baseType*/ null)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.namespaceName = namespaceName;
            this.name = name;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuredType"/> class.
        /// </summary>
        public EdmUntypedStructuredType()
            : this(EdmConstants.EdmNamespace, CsdlConstants.TypeName_Untyped_Short)
        {
        }

        /// <summary>
        /// Gets the namespace of this element.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
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
            get
            {
                return EdmTypeKind.Untyped;
            }
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get
            {
                return EdmSchemaElementKind.TypeDefinition;
            }
        }
    }
}
