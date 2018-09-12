//---------------------------------------------------------------------
// <copyright file="BadNamedStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM named structured type definition.
    /// </summary>
    internal abstract class BadNamedStructuredType : BadStructuredType, IEdmSchemaElement
    {
        private readonly string namespaceName;
        private readonly string name;

        protected BadNamedStructuredType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }
    }
}
