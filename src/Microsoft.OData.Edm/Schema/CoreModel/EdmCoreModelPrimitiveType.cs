//---------------------------------------------------------------------
// <copyright file="EdmCoreModelPrimitiveType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The built-in Edm.PrimitiveType and other concrete primitive types in the core model.
    /// </summary>
    internal sealed class EdmCoreModelPrimitiveType : EdmType, IEdmPrimitiveType, IEdmCoreModelElement, IEdmFullNamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCoreModelPrimitiveType"/> class.
        /// </summary>
        /// <param name="primitiveKind">The primitive type kind.</param>
        public EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind primitiveKind)
        {
            Name = primitiveKind.ToString();
            PrimitiveKind = primitiveKind;
            FullName = this.Namespace + "." + this.Name;
        }

        /// <summary>
        /// Gets the name of this type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the namespace of this type.
        /// </summary>
        public string Namespace
        {
            get { return EdmConstants.EdmNamespace; }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Primitive; }
        }

        /// <summary>
        /// Gets the primitive kind of this type.
        /// </summary>
        public EdmPrimitiveTypeKind PrimitiveKind { get; }

        /// <summary>
        /// Gets the schema element kind of this type.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the full name of this type.
        /// </summary>
        public string FullName { get; }
    }
}
