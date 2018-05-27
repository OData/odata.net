//---------------------------------------------------------------------
// <copyright file="EdmCoreModelUntypedType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The built-in Edm.Untyped type in the core model.
    /// </summary>
    internal sealed class EdmCoreModelUntypedType : EdmType, IEdmUntypedType, IEdmCoreModelElement
    {
        /// <summary>
        /// The core Edm.Untyped singleton.
        /// </summary>
        public static readonly EdmCoreModelUntypedType Instance = new EdmCoreModelUntypedType();

        /// <summary>
        /// Gets the Edm type kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind => EdmTypeKind.Untyped;

        /// <summary>
        /// Gets the scheme element type kind of this type.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

        /// <summary>
        /// Gets the name of this type.
        /// </summary>
        public string Name => CsdlConstants.TypeName_Untyped_Short;

        /// <summary>
        /// Gets the namespace of this type.
        /// </summary>
        public string Namespace => EdmConstants.EdmNamespace;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private EdmCoreModelUntypedType()
        {
        }
    }
}
