//---------------------------------------------------------------------
// <copyright file="BadEnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM enumeration type.
    /// </summary>
    internal class BadEnumType : BadType, IEdmEnumType
    {
        private readonly string namespaceName;
        private readonly string name;

        public BadEnumType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public IEnumerable<IEdmEnumMember> Members
        {
            get { return Enumerable.Empty<IEdmEnumMember>(); }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Enum; }
        }

        public IEdmPrimitiveType UnderlyingType
        {
            get { return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32); }
        }

        public bool IsFlags
        {
            get { return false; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
