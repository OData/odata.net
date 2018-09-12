//---------------------------------------------------------------------
// <copyright file="EdmCoreModelComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The built-in Edm.ComplexType abstract type in the core model.
    /// </summary>
    internal sealed class EdmCoreModelComplexType : EdmType, IEdmComplexType, IEdmCoreModelElement
    {
        /// <summary>
        /// The core Edm.ComplexType singleton.
        /// </summary>
        public static readonly EdmCoreModelComplexType Instance = new EdmCoreModelComplexType();

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind => EdmTypeKind.Complex;

        /// <summary>
        /// Gets the schema element kind of this type.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

        /// <summary>
        /// Gets the name of this type.
        /// </summary>
        public string Name => CsdlConstants.TypeName_Complex_Short;

        /// <summary>
        /// Gets the namespace of this type.
        /// </summary>
        public string Namespace => EdmConstants.EdmNamespace;

        /// <summary>
        /// Gets a value indicating whether this type is abstract.
        /// The Edm.ComplexType is always an abstract type.
        /// </summary>
        public bool IsAbstract => true;

        /// <summary>
        /// Gets a value indicating whether this type is open.
        /// The Edm.ComplexType is always non-open type.
        /// </summary>
        public bool IsOpen => false;

        /// <summary>
        /// Gets the base type of this type.
        /// The Edm.ComplexType is always without base type.
        /// </summary>
        public IEdmStructuredType BaseType = null;

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// The Edm.ComplexType is always without any declared properties.
        /// </summary>
        public IEnumerable<IEdmProperty> DeclaredProperties => Enumerable.Empty<IEdmProperty>();

        /// <summary>
        /// Gets the base structured type of this type.
        /// The Edm.ComplexType is always without base type.
        /// </summary>
        IEdmStructuredType IEdmStructuredType.BaseType => null;

        /// <summary>
        /// Searches for a structural or navigation property with the given name in this type.
        /// </summary>
        /// <param name="name">The name of the property being found.</param>
        /// <returns>The Edm.ComplexType is always without any declared properties.</returns>
        public IEdmProperty FindProperty(string name) => null;

        /// <summary>
        /// private constructor.
        /// </summary>
        private EdmCoreModelComplexType()
        {
        }
    }
}
