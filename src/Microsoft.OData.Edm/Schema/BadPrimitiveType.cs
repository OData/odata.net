//---------------------------------------------------------------------
// <copyright file="BadPrimitiveType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM primitive type definition.
    /// </summary>
    internal class BadPrimitiveType : BadType, IEdmPrimitiveType, IEdmFullNamedElement
    {
        private readonly EdmPrimitiveTypeKind primitiveKind;
        private readonly string name;
        private readonly string namespaceName;
        private readonly string fullName;

        public BadPrimitiveType(string qualifiedName, EdmPrimitiveTypeKind primitiveKind, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.primitiveKind = primitiveKind;
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
        }

        public EdmPrimitiveTypeKind PrimitiveKind
        {
            get { return this.primitiveKind; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Primitive; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
    }
}
