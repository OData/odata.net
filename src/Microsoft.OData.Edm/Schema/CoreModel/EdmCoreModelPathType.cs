//---------------------------------------------------------------------
// <copyright file="EdmCoreModelPathType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The built-in Edm.AnnotationPath, Edm.PropertyPath, Edm.NavigationPropertyPath abstract type in the core model.
    /// </summary>
    internal sealed class EdmCoreModelPathType : EdmType, IEdmPathType, IEdmCoreModelElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCoreModelPathType"/> class.
        /// </summary>
        /// <param name="pathKind">The path kind.</param>
        public EdmCoreModelPathType(EdmPathTypeKind pathKind)
        {
            Name = pathKind.ToString();
            PathKind = pathKind;
        }

        /// <summary>
        /// Gets the path kind of this type.
        /// </summary>
        public EdmPathTypeKind PathKind { get; }

        /// <summary>
        /// Gets the full name of this type.
        /// </summary>
        public string FullName => Namespace + "." + Name;

        /// <summary>
        /// Gets the Edm type kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind => EdmTypeKind.Path;

        /// <summary>
        /// Gets the schema element kind of this type.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

        /// <summary>
        /// Gets the namespace of this type.
        /// </summary>
        public string Namespace => EdmConstants.EdmNamespace;

        /// <summary>
        /// Gets the name of this type.
        /// </summary>
        public string Name { get; }
    }
}
