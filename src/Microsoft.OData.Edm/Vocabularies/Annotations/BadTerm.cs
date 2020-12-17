//---------------------------------------------------------------------
// <copyright file="BadTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a semantically invalid EDM Term.
    /// </summary>
    internal class BadTerm : BadElement, IEdmTerm
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly string fullName;
        private readonly IEdmTypeReference type;

        public BadTerm(string qualifiedName, IEnumerable<EdmError> errors)
            : base(errors)
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
            type = new BadTypeReference(new BadType(errors), true);
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string FullName
        {
            get { return this.fullName; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        public string AppliesTo
        {
            get { return null; }
        }

        public string DefaultValue
        {
            get { return null; }
        }

        public IEdmTerm BaseTerm
        {
            get { return null; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Term; }
        }
    }
}
