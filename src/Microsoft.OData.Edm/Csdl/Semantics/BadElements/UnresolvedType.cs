//---------------------------------------------------------------------
// <copyright file="UnresolvedType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Represents information about an EDM type definition that failed to resolve.
    /// </summary>
    internal class UnresolvedType : BadType, IEdmSchemaType, IUnresolvedElement
    {
        private readonly string namespaceName;
        private readonly string name;

        public UnresolvedType(string qualifiedName, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedType, Edm.Strings.Bad_UnresolvedType(qualifiedName)) })
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
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
